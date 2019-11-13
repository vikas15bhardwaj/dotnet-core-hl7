using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HL7.Core.V2
{
    internal class SubComponent
    {
        List<(string sub_component_name, string sub_component_value)> sub_components_list = new List<(string sub_component_name, string sub_component_value)>();

        internal static SubComponent GetSubComponent(string component_name, string component_value, string sub_component_separator)
        {
            if (!component_value.Contains(sub_component_separator))
                return null;
            else
                return new SubComponent(component_name, component_value, sub_component_separator);
        }
        private SubComponent(string component_name, string component_value, string sub_component_separator)
        {
            var sub_components = component_value.Split(sub_component_separator.ToCharArray());
            int index = 1;
            foreach (var sub_component_value in sub_components)
            {
                sub_components_list.Add(($"{component_name}_{index++}", sub_component_value));
            }
        }

    }

    internal class Component
    {
        List<(string component_name, string component_value, SubComponent component)> components_list = new List<(string component_name, string component_value, SubComponent component)>();

        internal static Component GetComponent(string field_name, string field_value, string component_separator, string sub_component_separator)
        {
            if (!field_value.Contains(component_separator))
                return null;
            else
                return new Component(field_name, field_value, component_separator, sub_component_separator);
        }
        private Component(string field_name, string field_value, string component_separator, string sub_component_separator)
        {
            var components = field_value.Split(component_separator.ToCharArray());
            int index = 1;
            foreach (var compoent_value in components)
            {
                string component_name = $"{field_name}_{index++}";

                components_list.Add((component_name, compoent_value, SubComponent.GetSubComponent(component_name, compoent_value, sub_component_separator)));
            }
        }
    }

    internal class Segment
    {
        List<(string field_name, int index, string field_value, Component component)> fields = new List<(string field_name, int index, string field_value, Component component)>();

        internal Segment(string segment, string field_separator, string component_separator, string sub_component_separator, string field_array_separator)
        {
            var segment_fields = segment.Split(field_separator.ToCharArray());
            string name = segment_fields[0];

            int index = (name == "MSH" ? 2 : 1);

            foreach (var field in segment_fields.Skip(1))
            {
                string field_name = $"{name}_{index}";

                if (field.Contains(field_array_separator))
                {
                    var array_fields = field.Split(field_array_separator.ToCharArray());
                    int arr_index = 0;
                    foreach (var afield in array_fields)
                    {
                        fields.Add((field_name, arr_index++, field, Component.GetComponent(field_name, afield, component_separator, sub_component_separator)));
                    }
                    index++;
                }
                else
                    fields.Add((field_name, 0, field, Component.GetComponent(field_name, field, component_separator, sub_component_separator)));
            }

        }

        internal string Get(string field_name)
        {
            return "";
        }
    }

    internal class Message
    {
        List<(string segment_name, Segment segment)> message_segment_list = new List<(string segment_name, Segment segment)>();

        string segment_separator = "\r";
        string field_separator = "|";
        string component_separator = "^";
        string sub_component_separator = "&";
        string field_array_separator = "~";

        char split_char = '\r';

        internal Message(string message)
        {
            if (message.Contains(Environment.NewLine))
                segment_separator = Environment.NewLine;
            else if (message.Contains("\n"))
            {
                segment_separator = "\n";
                split_char = '\n';
            }
            var message_segments = message.Split(split_char);

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

                message_segment_list.Add((segment_name, new Segment(segment, field_separator, component_separator, sub_component_separator, field_array_separator)));
            }
        }

        internal string[] Get(string field_name)
        {
            //field_name needs to like MSH_2
            var field_detail = field_name.Split('_');

            var segment_name = GetSegmentName(field_detail[0]);

            if (field_detail.Length == 1)
                return message_segment_list.Where(x => x.segment_name == segment_name.name)?
                .Select(s => s.segment_name + field_separator + s.segment.ToString()).ToArray();
            //.Aggregate((s1, s2) => s1 + segment_separator + s2);
            else
                return message_segment_list.Where(x => x.segment_name == segment_name.name)?
                .Select(s => s.segment.Get(field_name)).ToArray();
        }

        private (string name, int index) GetSegmentName(string field_name)
        {
            Regex regex = new Regex(@"[A-Z0-9]+");
            var segment_detail = regex.Split(field_name);
            var segment_name = segment_detail[0];
            int index = 0;
            if (segment_detail.Length == 2)
                index = Convert.ToInt32(segment_detail[1]);

            return (segment_name, index);
        }
    }

}