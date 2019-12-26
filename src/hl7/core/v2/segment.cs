using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7.Core.V2
{
    internal class Segment
    {
        string _field_separator;
        string _segment_name;
        string _component_separator;

        string _sub_component_separator;
        string _field_array_separator;

        List<(string field_name, int index, string field_value, Component component)> _fields
            = new List<(string field_name, int index, string field_value, Component component)>();

        internal Segment(string segment, string field_separator, string component_separator, string sub_component_separator, string field_array_separator)
        {
            _field_separator = field_separator;
            _component_separator = component_separator;
            _sub_component_separator = sub_component_separator;
            _field_array_separator = field_array_separator;

            var segment_fields = segment.Split(field_separator.ToCharArray());
            _segment_name = segment_fields[0];

            int index = (_segment_name == "MSH" ? 2 : 1);

            foreach (var field in segment_fields.Skip(1))
            {
                string field_name = $"{_segment_name}_{index}";
                SaveField(field_name, field);
                index++;
            }

        }

        public override string ToString()
        {
            return _segment_name + _field_separator + _fields.Where(f => f.index == -1).Select(f => f.field_value)?.Aggregate((f1, f2) => f1 + _field_separator + f2);
        }
        internal string Get(Field field)
        {
            var field2 = _fields.Where(f => f.field_name == field.FieldName && f.index == field.FieldIndex);
            if (field2.Count() > 0 && String.IsNullOrEmpty(field.ComponentName))
                return field2.Select(f => f.field_value).FirstOrDefault();
            else if (field2.Count() > 0)
                return field2.Select(f => f.component?.Get(field))?.FirstOrDefault();

            return null;
        }

        internal void Set(Field field, string value)
        {

            var fieldIndex = _fields.FindIndex(f => f.field_name == field.FieldName && f.index == field.FieldIndex);

            if (fieldIndex < 0)
            {
                int count = (_segment_name == "MSH" ? 2 : 1);
                for (int i = _fields.Count() + count; i <= field.FieldNumber; i++)
                {
                    string field_name = $"{_segment_name}_{i}";
                    SaveField(field_name, "");
                }
                fieldIndex = _fields.FindIndex(f => f.field_name == field.FieldName && f.index == field.FieldIndex);
            }

            if (String.IsNullOrEmpty(field.ComponentName))
                SaveField(field.FieldName, value);
            else
            {
                var thisField = _fields[fieldIndex];

                if (thisField.component == null)
                    thisField.component = new Component(_component_separator, _sub_component_separator);

                thisField.component.Set(field, value);
                thisField = (field.FieldName, thisField.index, thisField.component.ToString() ?? value, thisField.component);
                _fields[fieldIndex] = thisField;
            }
        }

        private void SaveField(string field_name, string field)
        {
            if (field_name != "MSH_2" && field.Contains(_field_array_separator))
            {
                //for an array field, add one entry with entire field and then one entry for each array element
                SaveField(field_name, -1, field, null);

                var array_fields = field.Split(_field_array_separator.ToCharArray());
                int arr_index = 0;
                foreach (var afield in array_fields)
                {
                    SaveField(field_name, arr_index++, afield, Component.GetComponent(field_name, afield, _component_separator, _sub_component_separator));
                }
            }
            else
                SaveField(field_name, -1, field, Component.GetComponent(field_name, field, _component_separator, _sub_component_separator));
        }
        private void SaveField(string field_name, int index, string value, Component component)
        {
            var fieldIndex = _fields.FindIndex(f => f.field_name == field_name && f.index == index);
            if (fieldIndex >= 0)
                _fields[fieldIndex] = (field_name, index, value, component);
            else
                _fields.Add((field_name, index, value, component));
        }
    }
}