using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7.Core.V2
{
    internal class Segment
    {
        string _field_separator;
        List<(string field_name, int index, string field_value, Component component)> fields = new List<(string field_name, int index, string field_value, Component component)>();

        internal Segment(string segment, string field_separator, string component_separator, string sub_component_separator, string field_array_separator)
        {
            _field_separator = field_separator;
            var segment_fields = segment.Split(field_separator.ToCharArray());
            string name = segment_fields[0];

            int index = (name == "MSH" ? 2 : 1);

            foreach (var field in segment_fields.Skip(1))
            {
                string field_name = $"{name}_{index}";

                if (field_name != "MSH_2" && field.Contains(field_array_separator))
                {
                    var array_fields = field.Split(field_array_separator.ToCharArray());
                    int arr_index = 0;
                    foreach (var afield in array_fields)
                    {
                        fields.Add((field_name, arr_index++, field, Component.GetComponent(field_name, afield, component_separator, sub_component_separator)));
                    }

                }
                else
                    fields.Add((field_name, 0, field, Component.GetComponent(field_name, field, component_separator, sub_component_separator)));

                index++;
            }

        }

        public override string ToString()
        {
            return fields.Select(f => f.field_value)?.Aggregate((f1, f2) => f1 + _field_separator + f2);
        }
        internal string Get(string field_name)
        {
            return "";
        }
    }
}