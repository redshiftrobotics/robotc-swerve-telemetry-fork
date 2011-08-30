//
// TelemetrySample.c, v0.8
//
//      Brought to you by FTC 417, September 2011
//
// This file contains several examples of telemetry transmission.
//
#include "..\TelemetryFTC.h"

void DoCompletenessTest()
// Exercise all the various telemetry data types
    {
    // Capture the starting time so we can include the increment thereon
    // in each of our telemetry records.
    long msBase = nSysTime;

    // The default USB poll interval is relatively long, and can often
    // miss records. By setting the interval to zero, we ask the telemetry
    // recorder to poll us as fast as it reasonably can.
    TelemetrySetUSBPollInterval(0);

    // Emit a header row for easily remembering things.
    TelemetryAddString("ms");
    TelemetryAddString("#");
    TelemetryAddString("i8");
    TelemetryAddString("i16");
    TelemetryAddString("i32");
    TelemetryAddString("ui8");
    TelemetryAddString("ui16");
    TelemetryAddString("ui32");
    TelemetryAddString("f");
    TelemetryAddString("s");
    TelemetryAddString("b");
    TelemetryAddString("ch");
    TelemetrySend();

    // Transmit the data. This is a typical example, in that it transmits
    // a timestamp at which the record was made, a serial# to support
    // detection of dropouts, and then some actual data.
    for (telemetry.serialNumber = 0; telemetry.serialNumber < 1000; telemetry.serialNumber++)
        {
        TelemetryAddInt16(nSysTime - msBase);
        TelemetryAddInt32(telemetry.serialNumber);
        //
        long   iDatum   = -telemetry.serialNumber;
        bool   fDatum   = (iDatum % 2 == 0);
        char   chDatum  = 'a' + (telemetry.serialNumber % 26);
        char   rgch[4]; rgch[0] = chDatum; rgch[1] = (chDatum+1); rgch[2] = (chDatum+2); rgch[3] = 0;
        string sDatum; StringFromChars(sDatum, rgch[0]);
        float  flDatum  = 3.14159 * telemetry.serialNumber / 180.0;
        //
        TelemetryAddInt8(iDatum);
        TelemetryAddInt16(iDatum);
        TelemetryAddInt32(iDatum);
        TelemetryAddUInt8(iDatum);
        TelemetryAddUInt16(iDatum);
        TelemetryAddUInt32(iDatum);
        TelemetryAddFloat(flDatum);
        TelemetryAddString(sDatum);
        TelemetryAddBool(fDatum);
        TelemetryAddChar(chDatum);
        //
        TelemetrySend();
        }

    TelemetryDone();
    }

void DoSpeedTest()
// Send one datum of the smallest size as fast as we can, many times.
// We use a negation trick to see whether any values get dropped:
// the sign of the data in the records received should always alternate
// from record to record; the (absolute) value itself is the number of
// milliseconds since the start of the run.
    {
    long msBase = nSysTime;
    TelemetrySetUSBPollInterval(0);

    long s = 1;
    for (int i = 0; i < 1000; i++)
        {
        long value = (nSysTime - msBase) * s;
        s = -s;
        TelemetryAddInt16(value);
        TelemetrySend();
        }

    TelemetryDone();
    }

void DoOne()
// Pretty much the simplest telemetry transmission one can imagine: just one record is sent
    {
    TelemetrySetUSBPollInterval(0);
    //
    TelemetryAddInt16(42);
    TelemetrySend();
    //
    TelemetryDone();
    }

task main()
    {
    writeDebugStreamLine("main...");

    // Hog the CPU so our code here can transmit as fast as possible, thus
    // stressing the telemetry recorder as much as we can. Normally, though,
    // you won't do this. nDebugTaskMode
    hogCPU();

    // Initialize the telemetry.
    TelemetryInitialize();
    TelemetryUseBluetooth(false);   // set to true if use of Bluetooth is desired

    // Actually send some
    DoCompletenessTest();
    DoSpeedTest();

    // Undo the effect of the hogCPU()
    releaseCPU();

    writeDebugStreamLine("...main");
    }
