using System;
using System.Collections.Generic;
using System.Linq;

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

        public string Get(Field field)
        {
            var field2 = sub_components_list.Where(f => f.sub_component_name == field.SubComponentName);

            if (field2.Count() > 0)
                return field2.Select(f => f.sub_component_value).FirstOrDefault();

            return null;
        }
    }
}