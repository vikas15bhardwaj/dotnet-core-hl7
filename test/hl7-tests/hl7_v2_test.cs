using System;
using System.IO;
using Xunit;

namespace HL7_Tests
{
    public class HL7_V2_Test
    {
        [Fact]
        public void TestName()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.NotNull(hl7);
        }
        [Fact]
        public void Get_MSH10()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);
            string msh10 = hl7.Get("MSH.10");
            Assert.Same("AGTADM.1.260506.567", msh10);
        }
    }
}
