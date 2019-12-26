﻿using System;

namespace HL7
{
    public class HL7V2
    {
        HL7.Core.V2.Message _message;
        public HL7V2(string hl7Message)
        {
            _message = new HL7.Core.V2.Message(hl7Message);
        }

        public string Get()
        {
            return _message.Get();
        }
        public string[] GetSegment(string segment_name)
        {
            return _message.GetSegment(segment_name);
        }

        public string Get(string field_name)
        {
            return _message.Get(field_name);
        }
        public void Set(string field_name, string value)
        {
            _message.Set(field_name, value);
        }
    }
}
