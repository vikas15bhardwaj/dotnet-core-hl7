using System;
using System.Collections.Generic;
using System.Linq;
namespace HL7.Core.V2
{
    internal class Field
    {
        public string SegmentName { get; set; }
        public int SegmentIndex { get; set; }
        public string FieldName { get; set; }
        public int FieldIndex { get; set; }

        public int FieldNumber { get; set; }
        public string ComponentName { get; set; }
        public int ComponentIndex { get; set; }
        public string SubComponentName { get; set; }
        public int SubComponentIndex { get; set; }
    }
}