using System;
using System.Collections.Generic;
using System.Linq;
namespace HL7.Core.V2
{
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
}