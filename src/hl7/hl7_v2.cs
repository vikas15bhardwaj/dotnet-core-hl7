using System;

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
        public string[] Get(string field_name)
        {
            return _message.Get(field_name);
        }
    }
}
