using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7.Core.V2
{
    internal class SubComponent
    {
        List<(string sub_component_name, string sub_component_value)> _sub_components_list = new List<(string sub_component_name, string sub_component_value)>();
        string _sub_component_separator;

        internal static SubComponent GetSubComponent(string component_name, string component_value, string sub_component_separator)
        {
            if (!component_value.Contains(sub_component_separator))
                return null;
            else
                return new SubComponent(component_name, component_value, sub_component_separator);
        }
        internal SubComponent(string component_name, string component_value, string sub_component_separator)
        {
            _sub_component_separator = sub_component_separator;
            var sub_components = component_value.Split(sub_component_separator.ToCharArray());
            int index = 1;
            foreach (var sub_component_value in sub_components)
            {
                string sub_component_name = $"{component_name}_{index++}";

                if (_sub_components_list.Where(s => s.sub_component_name == sub_component_name).Count() > 0)
                    _sub_components_list.Where(s => s.sub_component_name == sub_component_name).ToList()
                    .ForEach(s => s = (sub_component_name, sub_component_value));
                else
                    _sub_components_list.Add((sub_component_name, sub_component_value));
            }
        }

        public string Get(Field field)
        {
            return _sub_components_list.Where(f => f.sub_component_name == field.SubComponentName)
                                        .Select(f => f.sub_component_value).FirstOrDefault();
        }

        public void Remove(Field field)
        {
            _sub_components_list.RemoveAll(s => s.sub_component_name == field.SubComponentName);
        }
        public void Set(Field field, string value)
        {
            for (int i = _sub_components_list.Count() + 1; i <= field.SubComponentIndex; i++)
            {
                string sub_component_name = $"{field.ComponentName}_{i}";
                _sub_components_list.Add((sub_component_name, ""));
            }
            var index = _sub_components_list.FindIndex(s => s.sub_component_name == field.SubComponentName);
            _sub_components_list[index] = (field.SubComponentName, value);

        }
        public override string ToString()
        {
            return _sub_components_list.Select(s => s.sub_component_value).Aggregate((s1, s2) => s1 + _sub_component_separator + s2);
        }
    }
}