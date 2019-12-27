using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HL7.Core.V2
{
    internal class Message
    {
        List<(string segment_name, int index, Segment segment)> message_segment_list = new List<(string segment_name, int index, Segment segment)>();

        string segment_separator = "\r";
        string field_separator = "|";
        string component_separator = "^";
        string sub_component_separator = "&";
        string field_array_separator = "~";
        internal Message(string message)
        {
            if (message.Contains("\r\n"))
                segment_separator = "\r\n";
            else if (message.Contains("\n"))
                segment_separator = "\n";
            else
                segment_separator = "\r";

            Regex regex = new Regex(segment_separator);
            var message_segments = regex.Split(message);

            foreach (var segment in message_segments)
            {
                AddSegment(segment);
            }
        }

        internal string Get()
        {
            return message_segment_list.Select(s => s.segment.ToString()).Aggregate((s1, s2) => s1 + segment_separator + s2);
        }

        internal string[] GetSegment(string segment_name)
        {
            //field_name needs to like MSH_2
            var field = ParseField(segment_name);

            var segment_list = GetSegmentList(field.SegmentName, field.SegmentIndex);

            //if requested field is just segment e.g. PID, then entire PID segment should be returned
            if (field.FieldName == null)
                return segment_list?
                .Select(s => s.segment.ToString()).ToArray();

            return null;
        }
        internal string Get(string field_name)
        {
            //field_name needs to like MSH_2
            var field = ParseField(field_name);

            var segment_list = GetSegmentList(field.SegmentName, field.SegmentIndex);

            //if requested field is just segment e.g. PID, then entire PID segment should be returned
            if (field.FieldName != null)
                return segment_list?
                .Select(s => s.segment.Get(field)).FirstOrDefault();

            return null;
        }

        internal void Set(string field_name, string value)
        {
            var field = ParseField(field_name);
            var segment_list = GetSegmentList(field.SegmentName, field.SegmentIndex);
            //if requested field is just segment e.g. PID, then entire PID segment should be returned
            if (field.FieldName == null && segment_list?.Count() <= 0)
                AddSegment(value);
            else if (field.FieldName == null)
                UpdateSegment(field, value);
            else
                segment_list?
                .ForEach(s => s.segment.Set(field, value));
        }

        internal void Remove(string field_name)
        {
            var field = ParseField(field_name);
            var segment_list = GetSegmentList(field.SegmentName, field.SegmentIndex);
            segment_list?
            .ForEach(s => s.segment.Remove(field));
        }

        private void UpdateSegment(Field field, string segment)
        {
            var segments = message_segment_list.Where(s => s.segment_name == field.SegmentName).ToList();
            if (field.SegmentIndex >= 0)
                segments = segments.Where(s => s.index == field.SegmentIndex).ToList();

            segments.ForEach(x => x = (field.SegmentName, field.SegmentIndex, new Segment(segment, field_separator, component_separator, sub_component_separator, field_array_separator)));


        }
        internal void AddSegment(string segment)
        {
            if (segment.StartsWith("MSH"))
            {
                field_separator = segment.Substring(3, 1);
                component_separator = segment.Substring(4, 1);
                sub_component_separator = segment.Substring(7, 1);
                field_array_separator = segment.Substring(5, 1);
            }
            var segment_fields = segment.Split(field_separator.ToCharArray());
            string segment_name = segment_fields[0];

            int index = message_segment_list.Count(s => s.segment_name == segment_name);
            message_segment_list.Add((segment_name, index, new Segment(segment, field_separator, component_separator, sub_component_separator, field_array_separator)));
        }

        internal void RemoveSegment(string segment_name)
        {
            var field = ParseField(segment_name);
            var segment_list = GetSegmentList(field.SegmentName, field.SegmentIndex);

            segment_list.ForEach(x => message_segment_list.Remove(x));
        }
        private List<(string segment_name, int index, Segment segment)> GetSegmentList(string segment_name, int index)
        {
            if (index >= 0)
                return message_segment_list.Where(x => x.segment_name == segment_name && x.index == index).ToList();
            else
                return message_segment_list.Where(x => x.segment_name == segment_name).ToList();

        }

        private Field ParseField(string hl7Field)
        {
            //field name can be in this format max PID[0]_3[0]_1_6. This mean PID3.6.1 field of first PID segment and first PID3 in that segment
            //this method parse it the field separately as follows:
            //segment_name = PID, segment_index = 0, field_name = PID_3, field_index = 0, component_name = PID_3_1 and sub_component_name = PID_3_1_6
            //default index is -1
            var field_details = hl7Field.Split('_');
            string segment_name = null;
            int segment_index = -1;
            string field_name = null;
            int field_index = -1;
            string component_name = null;
            string sub_component_name = null;
            int component_index = -1;
            int sub_component_index = -1;
            int field_number = -1;

            Regex regex = new Regex("[^A-Z0-9+]");

            int index = 0;

            foreach (var field in field_details)
            {
                switch (index)
                {
                    case 0:
                        var segment_detail = regex.Split(field);

                        segment_name = segment_detail[0];
                        if (segment_detail.Length >= 2)
                            segment_index = Convert.ToInt32(segment_detail[1]);

                        break;
                    case 1:
                        var f_detail = regex.Split(field);

                        field_name = segment_name + "_" + f_detail[0];
                        field_number = Convert.ToInt32(f_detail[0]);
                        if (f_detail.Length >= 2)
                            field_index = Convert.ToInt32(f_detail[1]);

                        break;
                    case 2:
                        component_name = field_name + "_" + field;
                        component_index = Convert.ToInt32(field);
                        break;
                    case 3:
                        sub_component_name = component_name + "_" + field;
                        sub_component_index = Convert.ToInt32(field);
                        break;

                }
                index++;
            }

            return new Field
            {
                SegmentName = segment_name,
                SegmentIndex = segment_index,
                FieldName = field_name,
                FieldIndex = field_index,
                ComponentName = component_name,
                ComponentIndex = component_index,
                SubComponentName = sub_component_name,
                SubComponentIndex = sub_component_index,
                FieldNumber = field_number
            };
        }
    }
}