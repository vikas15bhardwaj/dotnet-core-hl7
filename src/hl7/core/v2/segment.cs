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

        internal void Remove(Field field)
        {
            var fieldIndex = _fields.FindIndex(f => f.field_name == field.FieldName && f.index == field.FieldIndex);
            var thisField = _fields[fieldIndex];
            if (String.IsNullOrEmpty(field.ComponentName))
            {
                //if specific array element being removed, then update main to reflect the changes
                if (thisField.index > -1)
                {
                    var arr_top = _fields.FindIndex(f => f.field_name == field.FieldName && f.index == -1);
                    var all_array_fields = _fields.FindAll(f => f.field_name == field.FieldName && f.index != -1 && f.index != thisField.index);
                    var arr_top_value = all_array_fields.Select(a => a.field_value).Aggregate((v1, v2) => v1 + _field_array_separator + v2);
                    _fields[arr_top] = (field.FieldName, -1, arr_top_value, null);

                    _fields.Remove(thisField);
                }
                else
                    _fields.RemoveAll(f => f.field_name == field.FieldName);
            }
            else if (thisField.component != null)
            {
                thisField.component.Remove(field);
                thisField = (field.FieldName, thisField.index, thisField.component.ToString() ?? null, thisField.component);
                _fields[fieldIndex] = thisField;
                //update array field if needed
                if (thisField.index > -1)
                {
                    var arr_top = _fields.FindIndex(f => f.field_name == field.FieldName && f.index == -1);
                    var all_array_fields = _fields.FindAll(f => f.field_name == field.FieldName && f.index != -1);
                    var arr_top_value = all_array_fields.Select(a => a.field_value).Aggregate((v1, v2) => v1 + _field_array_separator + v2);
                    _fields[arr_top] = (field.FieldName, -1, arr_top_value, null);
                }
            }

        }
        internal void Set(Field field, string value)
        {

            var fieldIndex = _fields.FindIndex(f => f.field_name == field.FieldName && f.index == field.FieldIndex);

            if (fieldIndex < 0)
            {
                AddNewFields(field);
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

                //update array field if needed
                if (thisField.index > -1)
                {
                    var arr_top = _fields.FindIndex(f => f.field_name == field.FieldName && f.index == -1);
                    var all_array_fields = _fields.FindAll(f => f.field_name == field.FieldName && f.index != -1);
                    var arr_top_value = all_array_fields.Select(a => a.field_value).Aggregate((v1, v2) => v1 + _field_array_separator + v2);
                    _fields[arr_top] = (field.FieldName, -1, arr_top_value, null);
                }
            }


        }

        private void SaveField(string field_name, string field)
        {
            if (field_name != "MSH_2" && field != null && field.Contains(_field_array_separator))
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

        private void AddNewFields(Field field)
        {
            //if array item add it in a differnet way.
            if (field.FieldIndex > -1)
            {
                AddArrayField(field);
            }
            else
            {
                int start_with_index = (_segment_name == "MSH" ? _fields.Count() + 2 : _fields.Count() + 1);

                for (int i = start_with_index; i <= field.FieldNumber; i++)
                {
                    string field_name = $"{_segment_name}_{i}";
                    SaveField(field_name, "");
                }
            }
        }


        private void AddArrayField(Field field)
        {
            int start_with_index = _fields.Where(f => f.field_name == field.FieldName && f.index > -1).Count();

            for (int i = start_with_index; i <= field.FieldIndex; i++)
            {
                var original_field_index = _fields.FindIndex(f => f.index == -1 && f.field_name == field.FieldName);

                if (original_field_index < 0)
                {
                    SaveField(field.FieldName, -1, "", null);
                    SaveField(field.FieldName, i, "", null);
                }
                else
                {
                    string arr_item_value = "";
                    Component component = null;
                    if (i == 0)
                    {
                        arr_item_value = _fields[original_field_index].field_value;
                        component = _fields[original_field_index].component;
                        _fields[original_field_index] = (field.FieldName, -1, arr_item_value, null);
                        if (component == null)
                        {
                            component = new Component(_component_separator, _sub_component_separator);
                            component.Set(field.FieldName, arr_item_value);
                        }
                        SaveField(field.FieldName, i, arr_item_value, component);
                    }
                    else
                        SaveField(field.FieldName, i, "", null);

                }
            }
        }
    }
}