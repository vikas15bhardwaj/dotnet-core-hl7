# dotnet-core-hl7
dotnet-core-hl7 library provides functionality to parse HL7 V2 version. Using this library, you can easily access HL7 segments, fields and manipulate them as required. Parsing the hl7 becomes as simple as writing one line of code:

```
var adt = File.ReadAllText("../../../test-files/adt.hl7");

HL7.HL7V2 hl7 = new HL7.HL7V2(adt);
string[] msh = hl7.GetSegment("MSH");
Assert.Equal("MSH|^~\\&||COCQA1A|||201709050917||ADT^A08|AGTADM.1.260506.567|D|2.1", msh[0]);
Assert.Equal("04444", hl7.Get("ZCS[1]_6"));

```

To build and run the library execute following commands from terminal
```
$dotnet build
$dotnet test
```




Library provides following functions:

### Get(string field_name)
Get a specific field value from hl7 message. field_name can be a segment field, component, or sub component. e.g. PID_3, PID_3_1 or PID_3_1_2. It also allows to get an array element at specific index if field is repeating e.g. PID_3[0]_1 returns PID_3_1 field for first occurrence of PID_3 in array.

You can refer to test cases in hl7_v2_test.cs file on different aspect of usuage of this method.

### Get()
Get the whole message. This method is typically used after manipulating the message and then to get the entire message.

### GetSegment(string segment_name)
This method returns the segment list for the specific segment.

### Set(string field_name, string value)
This method updates a specific field value if field is already present, otherwise, it adds the new field at appropriate level. field_name is similar to Get method, e.g. if PID18 needs to updated in message
```
hl7.Set("PID_18", "newvalue")
```

### AddSegment(string segment_name)
This method adds a new segment to the message.

### Remove(string field_name)
This method removes a field from segment. field can be a segment field or component or sub component and is supplied similar to Get method.

### RemoveSegment(string segment_name)
This method removes segment(s) from the message. You can remove a specific segment if there are more than one by specifying its index. e.g. segment_name can be ZCS[0] to remove first ZCS segment from the message or it can be ZCS to remove all ZCS segments from the message.















