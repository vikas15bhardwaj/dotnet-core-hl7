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
        }

        internal string Get()
        {
            return message_segment_list.Select(s => s.segment.ToString()).Aggregate((s1, s2) => s1 + segment_separator + s2);
        }

        internal string[] Get(string field_name)
        {
            //field_name needs to like MSH_2
            var field = ParseField(field_name);

            var segment_list = GetSegmentList(field.SegmentName, field.SegmentIndex);

            //if requested field is just segment e.g. PID, then entire PID segment should be returned
            if (field.FieldName == null)
                return segment_list?
                .Select(s => s.segment.ToString()).ToArray();
            else
                return segment_list?
                .Select(s => s.segment.Get(field)).ToArray();
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
                        if (f_detail.Length >= 2)
                            field_index = Convert.ToInt32(f_detail[1]);

                        break;
                    case 2:
                        component_name = field_name + "_" + field;
                        break;
                    case 3:
                        sub_component_name = component_name + "_" + field;
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
                SubComponentName = sub_component_name
            };
        }
    }
}