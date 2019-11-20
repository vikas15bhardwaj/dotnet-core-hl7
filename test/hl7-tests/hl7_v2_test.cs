using System;
using System.IO;
using Xunit;

namespace HL7_Tests
{
    public class HL7_V2_Test
    {
        [Fact]
        public void GetMSHTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);
            string[] msh = hl7.Get("MSH");
            Assert.Equal("MSH|^~\\&||COCQA1A|||201709050917||ADT^A08|AGTADM.1.260506.567|D|2.1", msh[0]);
        }

        [Fact]
        public void GetMultipleSegmentTest()
        {

            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] zcs = hl7.Get("ZCS");
            Assert.Equal(2, zcs.Length);
            Assert.Equal("ZCS|1|^^^^||||04446", zcs[0]);
            Assert.Equal("ZCS|2|^^^^||||04444", zcs[1]);

        }

        [Fact]
        public void GetOneOfTheSegmentTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] zcs = hl7.Get("ZCS[1]");
            Assert.Single(zcs);
            Assert.Equal("ZCS|2|^^^^||||04444", zcs[0]);
            zcs = hl7.Get("ZCS[0]");
            Assert.Single(zcs);
            Assert.Equal("ZCS|1|^^^^||||04446", zcs[0]);

        }
        [Fact]
        public void GetMessageTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string adt2 = hl7.Get();
            Assert.Equal(adt2, adt);

        }

        [Fact]
        public void GetSegmentFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] control_id = hl7.Get("MSH_10");

            Assert.Equal("AGTADM.1.260506.567", control_id[0]);
        }

        [Fact]
        public void GetSegmentEntireFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] msh9 = hl7.Get("MSH_9");

            Assert.Equal("ADT^A08", msh9[0]);

            string[] pid3 = hl7.Get("PID_3");

            Assert.Equal("J000XXXXX^akjsaks~J121212^aksaksj", pid3[0]);
        }

        [Fact]
        public void GetComponentFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] msh91 = hl7.Get("MSH_9_1");

            Assert.Equal("ADT", msh91[0]);

        }
    }
}
