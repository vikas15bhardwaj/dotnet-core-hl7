using System;
using System.Collections.Generic;
using System.Linq;
namespace HL7.Core.V2
{
    internal class Component
    {
        List<(string component_name, string component_value, SubComponent sub_component)> components_list = new List<(string component_name, string component_value, SubComponent sub_component)>();
        string _component_seperator;
        string _sub_component_separator;

        internal static Component GetComponent(string field_name, string field_value, string component_separator, string sub_component_separator)
        {
            if (field_value == null || !field_value.Contains(component_separator))
                return null;
            else
                return new Component(field_name, field_value, component_separator, sub_component_separator);

        }
        private Component(string field_name, string field_value, string component_separator, string sub_component_separator)
        {
            _component_seperator = component_separator;
            _sub_component_separator = sub_component_separator;

            var components = field_value.Split(component_separator.ToCharArray());
            int index = 1;
            foreach (var component_value in components)
            {
                var component_name = $"{field_name}_{index++}";
                SaveField(component_name, component_value);
            }
        }

        internal Component(string component_separator, string sub_component_separator)
        {
            _component_seperator = component_separator;
            _sub_component_separator = sub_component_separator;
        }
        public string Get(Field field)
        {
            var component = components_list.Where(f => f.component_name == field.ComponentName);

            if (component.Count() > 0 && String.IsNullOrEmpty(field.SubComponentName))
                return component.Select(f => f.component_value).FirstOrDefault();
            else if (component.Count() > 0)
                return component.Select(f => f.sub_component?.Get(field)).FirstOrDefault();

            return null;
        }

        public void Remove(Field field)
        {
            if (String.IsNullOrEmpty(field.SubComponentName))
                components_list.RemoveAll(c => c.component_name == field.ComponentName);
            else
            {
                var component_index = components_list.FindIndex(c => c.component_name == field.ComponentName);
                var component = components_list[component_index];
                if (component.sub_component != null)
                {
                    component.sub_component.Remove(field);
                    component = (component.component_name, component.sub_component?.ToString() ?? null, component.sub_component);
                    components_list[component_index] = component;
                }
            }

        }
        public void Set(string field_name, string value)
        {
            var component_name = $"{field_name}_1";
            SaveField(component_name, value);
        }
        public void Set(Field field, string value)
        {
            var component_index = components_list.FindIndex(c => c.component_name == field.ComponentName);

            if (component_index < 0)
            {
                for (int i = 1; i <= field.ComponentIndex; i++)
                {
                    var component_name = $"{field.FieldName}_{i}";
                    if (components_list.FindIndex(f => f.component_name == component_name) < 0)
                        components_list.Add((component_name, "", null));
                }
                component_index = components_list.FindIndex(c => c.component_name == field.ComponentName);
            }

            var component = components_list[component_index];

            if (String.IsNullOrEmpty(field.SubComponentName))
                components_list[component_index] = (component.component_name, value, component.sub_component);
            else if (component.sub_component == null)
            {
                var sub_component = new SubComponent(component.component_name, component.component_value, _sub_component_separator);
                sub_component.Set(field, value);
                components_list[component_index] = (component.component_name, sub_component?.ToString() ?? value, sub_component);
            }
            else
            {
                var sub_component = components_list[component_index].sub_component;
                sub_component.Set(field, value);
                components_list[component_index] = (component.component_name, sub_component?.ToString() ?? value, sub_component);
            }

        }

        public override string ToString()
        {
            return components_list.Select(c => c.component_value).Aggregate((c1, c2) => c1 + _component_seperator + c2);
        }

        private void SaveField(string component_name, string component_value)
        {
            if (components_list.Where(c => c.component_name == component_name).Count() > 0)
                components_list.Where(c => c.component_name == component_name).ToList()
                                .ForEach(c => c = (component_name, component_value, SubComponent.GetSubComponent(component_name, component_value, _sub_component_separator)));
            else
                components_list.Add((component_name, component_value, SubComponent.GetSubComponent(component_name, component_value, _sub_component_separator)));

        }
    }
}