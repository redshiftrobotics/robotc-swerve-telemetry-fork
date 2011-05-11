//
// TelemetryTest.c
//
#include "..\TelemetryFTC.h"

void DoCompletenessTest()
// Exercise all the various data types and a bit of speed too.
    {
    long msBase = nSysTime;

    // Emit a header row for easily remembering things. Note: this almost
    // completely fills up one telemetry record. If the record size is
    // exceed (e.g., by adding more strings to the record) then an array
    // out of bounds error occurs.
    TelemetryAddString("ms");
    TelemetryAddString("ser#");
    TelemetryAddString("int16");
    TelemetryAddString("int8");
    TelemetryAddString("Pi");
    TelemetryAddString("uint32");
    TelemetryAddString("uint16");
    TelemetryAddString("uint8");
    TelemetryAddString("str");
    TelemetryAddString("e");
    TelemetrySend();

    // Transmit the data. This is typical, in that it transmits
    // a timestamp at which the record was made, a serial# to support
    // detection of dropouts, and then some actual data.
    for (telemetry.serialNumber = 0; telemetry.serialNumber < 3000; telemetry.serialNumber++)
        {
        TelemetryAddInt16(nSysTime - msBase);
        TelemetryAddInt32(telemetry.serialNumber);
        //
        long datum = telemetry.serialNumber;
        TelemetryAddInt16(32 * datum);
        TelemetryAddInt8(datum);
        TelemetryAddFloat(PI);
        TelemetryAddUInt32(datum);
        TelemetryAddUInt16(32 * datum);
        TelemetryAddUInt8(datum);
        TelemetryAddString("hello");
        TelemetryAddFloat(2.718281828);
        TelemetrySend();
        }
    }

void DoSpeedTest()
// Just send one datum of the smallest size as fast as we can.
// We use a negation trick to see whether any values get dropped:
// the sign of the records received should always alternate.
    {
    long msBase = nSysTime;
    long s = 1;
    for (int i = 0; i < 1000; i++)
        {
        long value = (nSysTime - msBase) * s;
        s = -s;
        TelemetryAddInt16(value);
        TelemetrySend();
        }
    }

task main()
    {
    hogCpu();
    TelemetryInitialize();
    telemetry.fActive = true;
    //
    DoCompletenessTest();
    //
    TelemetryDone();
    releaseCpu();
    }
