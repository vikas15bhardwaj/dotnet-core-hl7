using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7.Core.V2
{
    internal class Segment
    {
        string _field_separator;
        string _segment_name;
        List<(string field_name, int index, string field_value, Component component)> fields
            = new List<(string field_name, int index, string field_value, Component component)>();

        internal Segment(string segment, string field_separator, string component_separator, string sub_component_separator, string field_array_separator)
        {
            _field_separator = field_separator;
            var segment_fields = segment.Split(field_separator.ToCharArray());
            _segment_name = segment_fields[0];

            int index = (_segment_name == "MSH" ? 2 : 1);

            foreach (var field in segment_fields.Skip(1))
            {
                string field_name = $"{_segment_name}_{index}";

                if (field_name != "MSH_2" && field.Contains(field_array_separator))
                {
                    //for an array field, add one entry with entire field and then one entry for each array element
                    fields.Add((field_name, -1, field, null));

                    var array_fields = field.Split(field_array_separator.ToCharArray());
                    int arr_index = 0;
                    foreach (var afield in array_fields)
                    {
                        fields.Add((field_name, arr_index++, afield, Component.GetComponent(field_name, afield, component_separator, sub_component_separator)));
                    }

                }
                else
                    fields.Add((field_name, -1, field, Component.GetComponent(field_name, field, component_separator, sub_component_separator)));

                index++;
            }

        }

        public override string ToString()
        {
            return _segment_name + _field_separator + fields.Where(f => f.index == -1).Select(f => f.field_value)?.Aggregate((f1, f2) => f1 + _field_separator + f2);
        }
        internal string Get(string field_name)
        {
            return "";
        }
    }
}