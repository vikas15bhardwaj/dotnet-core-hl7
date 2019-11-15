using System;
using System.IO;
using Xunit;

namespace HL7_Tests
{
    public class HL7_V2_Test
    {
        [Fact]
        public void Get_MSH()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);
            string[] msh = hl7.Get("MSH");
            Assert.Equal("MSH|^~\\&||COCQA1A|||201709050917||ADT^A08|AGTADM.1.260506.567|D|2.1", msh[0]);
        }

        [Fact]
        public void Get_MultipleSegmentTest()
        {

            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] zcs = hl7.Get("ZCS");
            Assert.Equal(2, zcs.Length);
            Assert.Equal("ZCS|1|^^^^||||04446", zcs[0]);
            Assert.Equal("ZCS|2|^^^^||||04444", zcs[1]);

        }
    }
}
