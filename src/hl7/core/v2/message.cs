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

        internal string[] Get(string field_name)
        {
            //field_name needs to like MSH_2
            var field_detail = field_name.Split('_');

            var segment_list = GetSegmentList(field_detail[0]);

            //if requested field is just segment e.g. PID, then entire PID segment should be returned
            if (field_detail.Length == 1)
                return segment_list?
                .Select(s => s.segment_name + field_separator + s.segment.ToString()).ToArray();
            else
                return segment_list?
                .Select(s => s.segment.Get(field_name)).ToArray();
        }

        private List<(string segment_name, int index, Segment segment)> GetSegmentList(string field_name)
        {
            //if field_name is in format having index e.g. PID[1], the this regex will split into two
            //PID and 1. if no index then return all segments, otherwise return the one requested
            Regex regex = new Regex("[^A-Z0-9+]");
            var segment_detail = regex.Split(field_name);
            var segment_name = segment_detail[0];
            int index = -1;
            if (segment_detail.Length >= 2)
                index = Convert.ToInt32(segment_detail[1]);

            if (index >= 0)
                return message_segment_list.Where(x => x.segment_name == segment_name && x.index == index).ToList();
            else
                return message_segment_list.Where(x => x.segment_name == segment_name).ToList();

        }
    }
}