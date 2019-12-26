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
            string[] msh = hl7.GetSegment("MSH");
            Assert.Equal("MSH|^~\\&||COCQA1A|||201709050917||ADT^A08|AGTADM.1.260506.567|D|2.1", msh[0]);
        }

        [Fact]
        public void GetMultipleSegmentTest()
        {

            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] zcs = hl7.GetSegment("ZCS");
            Assert.Equal(2, zcs.Length);
            Assert.Equal("ZCS|1|^^^^||||04446", zcs[0]);
            Assert.Equal("ZCS|2|^^^^||||04444", zcs[1]);

        }

        [Fact]
        public void GetOneOfTheSegmentTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string[] zcs = hl7.GetSegment("ZCS[1]");
            Assert.Single(zcs);
            Assert.Equal("ZCS|2|^^^^||||04444", zcs[0]);
            zcs = hl7.GetSegment("ZCS[0]");
            Assert.Single(zcs);
            Assert.Equal("ZCS|1|^^^^||||04446", zcs[0]);

        }

        [Fact]
        public void GetOneOfTheSegmentFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string zcs1_1 = hl7.Get("ZCS[1]_1");

            Assert.Equal("2", zcs1_1);
            Assert.Equal("04444", hl7.Get("ZCS[1]_6"));
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

            string control_id = hl7.Get("MSH_10");

            Assert.Equal("AGTADM.1.260506.567", control_id);
        }

        [Fact]
        public void GetSegmentEntireFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string msh9 = hl7.Get("MSH_9");

            Assert.Equal("ADT^A08", msh9);

            string pid3 = hl7.Get("PID_3");

            Assert.Equal("J000XXXXX^akjsaks~J121212^aksaksj&ABS", pid3);
        }

        [Fact]
        public void GetComponentFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string msh91 = hl7.Get("MSH_9_1");
            string msh92 = hl7.Get("MSH_9_2");

            Assert.Equal("ADT", msh91);
            Assert.Equal("A08", msh92);

        }

        [Fact]
        public void GetSubComponentFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);
            string al1_33 = hl7.Get("AL1_3_3");

            string al1_331 = hl7.Get("AL1_3_3_1");

            string al1_332 = hl7.Get("AL1_3_3_2");
            Assert.Equal("No Known Allergies&NA", al1_33);

            Assert.Equal("No Known Allergies", al1_331);

            Assert.Equal("NA", al1_332);
        }

        [Fact]
        public void GetArrayFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string pid3_0 = hl7.Get("PID_3[0]");
            Assert.Equal("J000XXXXX^akjsaks", pid3_0);

            string pid3_1 = hl7.Get("PID_3[1]");
            Assert.Equal("J121212^aksaksj&ABS", pid3_1);

            string pid3_1_1 = hl7.Get("PID_3[1]_1");
            Assert.Equal("J121212", pid3_1_1);

            string pid3_1_2 = hl7.Get("PID_3[1]_2");
            Assert.Equal("aksaksj&ABS", pid3_1_2);

            string pid3_1_2_1 = hl7.Get("PID_3[1]_2_1");
            Assert.Equal("aksaksj", pid3_1_2_1);

            string pid3_1_2_2 = hl7.Get("PID_3[1]_2_2");
            Assert.Equal("ABS", pid3_1_2_2);

        }

        [Fact]
        public void GetArrayDefaultFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            string pid3_1 = hl7.Get("PID_3_1");
            Assert.Null(pid3_1);
        }

        [Fact]
        public void SetSegmentFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Equal("AGTADM.1.260506.567", hl7.Get("MSH_10"));
            hl7.Set("MSH_10", "AGTADM.1.260506.567.Test");
            Assert.Equal("AGTADM.1.260506.567.Test", hl7.Get("MSH_10"));

            string adt2 = hl7.Get();
            Assert.NotEqual(adt2, adt);

        }

        [Fact]
        public void SetComponentTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Equal("A08", hl7.Get("MSH_9_2"));
            hl7.Set("MSH_9_2", "A01");
            Assert.Equal("A01", hl7.Get("MSH_9_2"));

            string adt2 = hl7.Get();
            Assert.NotEqual(adt2, adt);
        }

        [Fact]
        public void SetComponent_NoChangeTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Equal("A08", hl7.Get("MSH_9_2"));
            hl7.Set("MSH_9_2", "A08");
            Assert.Equal("A08", hl7.Get("MSH_9_2"));

            string adt2 = hl7.Get();
            Assert.Equal(adt2, adt);
        }

        [Fact]
        public void AddNewFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Null(hl7.Get("MSH_13"));

            hl7.Set("MSH_13", "newField");
            Assert.Equal("newField", hl7.Get("MSH_13"));
            string adt2 = hl7.Get();
            Assert.NotEqual(adt2, adt);
        }

        [Fact]
        public void AddNewFieldWithComponentTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Null(hl7.Get("MSH_13"));

            hl7.Set("MSH_13", "newFieldC1^newFieldC2");
            Assert.Equal("newFieldC1^newFieldC2", hl7.Get("MSH_13"));
            Assert.Equal("newFieldC1", hl7.Get("MSH_13_1"));
            Assert.Equal("newFieldC2", hl7.Get("MSH_13_2"));

            hl7.Set("MSH_13_2", "newFieldC22");
            Assert.Equal("newFieldC1", hl7.Get("MSH_13_1"));
            Assert.Equal("newFieldC22", hl7.Get("MSH_13_2"));
            string adt2 = hl7.Get();
            Assert.NotEqual(adt2, adt);
        }

        [Fact]
        public void AddNewComponentFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Null(hl7.Get("MSH_13"));

            hl7.Set("MSH_13_1", "newComponent1");
            Assert.Equal("newComponent1", hl7.Get("MSH_13_1"));

            hl7.Set("MSH_13_2", "newComponent2");
            Assert.Equal("newComponent2", hl7.Get("MSH_13_2"));

            Assert.Equal("newComponent1^newComponent2", hl7.Get("MSH_13"));

        }

        [Fact]
        public void AddNewComponentFieldInMiddleTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Null(hl7.Get("MSH_13"));

            hl7.Set("MSH_13_2", "newComponent2");
            Assert.Equal("newComponent2", hl7.Get("MSH_13_2"));
            Assert.Equal("", hl7.Get("MSH_13_1"));
            Assert.Equal("^newComponent2", hl7.Get("MSH_13"));

            hl7.Set("MSH_13_1", "newComponent1");
            Assert.Equal("newComponent1", hl7.Get("MSH_13_1"));

            Assert.Equal("newComponent1^newComponent2", hl7.Get("MSH_13"));

        }

        [Fact]
        public void AddNewNonExistingFieldTest()
        {
            var adt = File.ReadAllText("../../../test-files/adt.hl7");

            HL7.HL7V2 hl7 = new HL7.HL7V2(adt);

            Assert.Null(hl7.Get("MSH_21"));
            Assert.Null(hl7.Get("MSH_20"));
            Assert.Null(hl7.Get("MSH_19"));
            Assert.Null(hl7.Get("EVN_6"));

            Assert.Equal("MSH|^~\\&||COCQA1A|||201709050917||ADT^A08|AGTADM.1.260506.567|D|2.1", hl7.GetSegment("MSH")[0]);
            Assert.Equal("EVN|A08|201709050917|||^^^^^^", hl7.GetSegment("EVN")[0]);

            hl7.Set("MSH_21", "newField");
            hl7.Set("EVN_6", "EVNField");
            hl7.Set("EVN_5_1", "Component1");

            Assert.Equal("newField", hl7.Get("MSH_21"));
            Assert.Equal("", hl7.Get("MSH_20"));
            Assert.Equal("", hl7.Get("MSH_19"));

            Assert.Equal("MSH|^~\\&||COCQA1A|||201709050917||ADT^A08|AGTADM.1.260506.567|D|2.1|||||||||newField", hl7.GetSegment("MSH")[0]);
            Assert.Equal("EVN|A08|201709050917|||Component1^^^^^^|EVNField", hl7.GetSegment("EVN")[0]);


        }
    }
}
