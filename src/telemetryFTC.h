//
// TelemetryFTC.h
//
// Part of the FTC Team 417 Software Starter Kit: www.ftc417.org/ssk
//
// This file contains support for sending telemetry records in real time to a Microsoft
// Excel spreadsheet. Actually recording the telemetry requires the use of the
// TelemetryFTC program running on the PC; see www.ftc417.org/TelemetryFTC for details
// on how to get that.
//
// Basic Telemetry:
// ---------------
//
// The main idea is that you accumulate telemetry records by making a series of
//
//      TelemetryAddInt32(42);
//      TelemetryAddString("Some pig!");
//      TelemetryAddFloat(3.14159);
//
// etc. calls in order to accumulate data of various types in your record, then call
//
//      TelemetrySend();
//
// to actually transmit the record. Data rates of up to around a thousand telemetry
// records per second should be possible.
//
// But you need to first call
//
//      TelemetryInitialize();
//
// once, before calling any other telemetry function. And when you're done, you
// can call optionally call TelemetryDone(); telemetry records transmitted after
// a TelemetryDone() (maybe in the next run of your program) will appear in a
// separate, new Excel spreadsheet workbook.
//
// Advanced Telemetry Techniques:
// -----------------------------
//
// Telemetry records can be transmitted to the PC by one of three mechanisms:
//      * over Bluetooth
//      * over USB, with the USB cable connected to the PC
//      * over USB, with the USB cable connected to a Samantha module (not yet fully functional)
// Telemetry is (due to RobotC limitations) configured by default so that records can be
// transmitted only over USB; if Bluetooth transmission is desired, then one must call
//
//      TelemetryUseBluetooth(true);
//
// and one must be sure during telemetry transmission to *not* at that momsent be
// debugging over Bluetooth using RobotC. Additionally, if USB transmission is not needed,
// some small performance improvement can be achieved by calling
//
//      TelemetryUseUSB(false);
//
// though this is not required.
//
// When sending data over Bluetooth, the NXT just spontaneously and immediately sends
// out the data over the radio; if no TelemetryFTC program is connected and listening,
// the data just vanishes into the night. However, for obscure technical reasons, when
// the USB approaches are used, TelemetryFTC on the PC must poll the NXT periodically to see
// if new records are available. By default, TelemetryFTC only polls every so often (currently
// 30 ms). If, when using USB, you find you want telemetry records to be polled faster
// than that, call
//
//      TelemetrySetUSBPollInterval(ms);
//
// where ms is how often the poll should occur (in milliseconds), and where a value of
// zero indicates that polling should be carried out as fast as possible. If you're
// actually running up against this issue, then simply calling
//
//      TelemetrySetUSBPollInterval(0);
//
// is probably your best bet.
//
// Accumulation and transmission of telemetry records can be temporarily suppressed
// by calling
//
//      TelemetryEnable(false);
//
// and can be unsupressed by later calling TelemetryEnable(true). With this technique,
// you can leave TelemetryAddInt32(), TelemetryAddString(), etc and TelemetrySend()
// calls in your code on a permanent basis, and only selectively later decide when
// you're actually interested in really transmitting. Telemetry transmission is
// enabled by default.
//
// By default, telemetry is recorded in the first sheet of the Excel spreadsheet
// workbook. Optionally, you can direct a telemetry record to a different sheet in the
// workbook simply by calling
//
//      TelemetrySendSheet(sheetNumber);
//
// instead of the usual
//
//      TelemetrySend();
//
// 'sheetNumber' is the zero-based index of the sheet to be used. TelemetrySend()
// is thus equivalent to TelemetrySendSheet(0).
//
// Finally, it's very often the case that, in order to reliably determine whether
// all telemetry records have been recorded in Excel and that none have been lost,
// and to permanently record the order in which the records were created, it's worthwhile
// to tag each telemetry record with its own unique, monotonically increasing serial
// number, usually as the first datum in the record. This paradigm is so common, in fact,
// that we've defined for you the variable
//
//      telemetry.serialNumber
//
// so you have a place to do this. For example, if you want to send two integer pieces of
// data in each telemetry record, then sending the serial number along with them, together with
// a time stamp (also often handy), would look something like this:
//
//      // at startup do:
//      long msStart = nSysTime;
//
//      // for each record do:
//      long msNow = nSysTime - msStart;
//      TelemetryAddInt32(telemetry.serialNumber);      // you can use TelemetryAddInt16 or even TelemetryAddInt8 to save space if you won't have many records
//      TelemetryAddInt32(msNow);
//      TelemetryAddInt32(variableWithMyFirstDatum);    // or whatever other...
//      TelemetryAddInt32(variableWithMySecondDatum);   // ...data you want to send, of course
//      TelemetrySend();
//      telemetry.serialNumber++;
//
// Note that telemetry records are of limited capacity. If you try to add a datum to the accumulating
// record and it won't fit, the attempt at adding will simply be ignored, as will all further data until the
// record is ultimately sent. Thus, your record when transmitted will have fewer columns than expected.
//
// Definitions:
// -----------
//
// Below are the defintions of the functions, etc. that you may use in your program.
// DO NOT USE any definitions that begin with an underscore ('_'), as these are
// internal to the implementation of how telemetry works, and may change in a subsequent
// release of this software. YOU HAVE BEEN WARNED! :-)
//
// Use the following functions to add data of various types to your accumulating telemetry record:
//
#define TelemetryAddInt8(bVal)      _TelemetryAddInt8_(telemetry,bVal)
#define TelemetryAddInt16(shVal)    _TelemetryAddInt16_(telemetry,shVal)
#define TelemetryAddInt32(lVal)     _TelemetryAddInt32_(telemetry,lVal)
#define TelemetryAddUInt8(bVal)     _TelemetryAddUInt8_(telemetry,bVal)
#define TelemetryAddUInt16(shVal)   _TelemetryAddUInt16_(telemetry,shVal)
#define TelemetryAddUInt32(lVal)    _TelemetryAddUInt32_(telemetry,lVal)
#define TelemetryAddFloat(flVal)    _TelemetryAddFloat_(telemetry,flVal)
#define TelemetryAddBool(boolVal)   _TelemetryAddBool_(telemetry,boolVal)
#define TelemetryAddChar(charVal)   _TelemetryAddChar_(telemetry,charVal)
#define TelemetryAddString(str)     _TelemetryAddString_(telemetry,str)
//
// Call TelemetrySend() or TelemetrySendSheet() to transmit the accumulated telemetry record
// and reset for accumulation of the next one.
//
#define TelemetrySend()             _TelemetrySend_(telemetry,0)
#define TelemetrySendSheet(isheet)  _TelemetrySend_(telemetry,isheet)
//
// Call TelemetryInitialize(), once, before you call any other telemetry functions.
//
#define TelemetryInitialize()       _TelemetryInitialize_(telemetry)
//
// Cause any subsequent telemetry transmission, either in the currently
// running program or in the execution of a subsqeuent program on this
// NXT, to be recorded in a fresh spanking new Excel workbook.
//
#define TelemetryDone()             _TelemetryDone_(telemetry)
//
// Control whether telemetry is transmitted over Bluetooth and/or USB.
//
#define TelemetryUseBluetooth(useBluetooth)  { telemetry._useBluetooth = (useBluetooth); }
#define TelemetryUseUSB(useUSB)              { telemetry._useUSB = (useUSB); }
#define TelemetryUse(useBluetooth,useUSB)    { TelemetryUseBluetooth(useBluetooth); TelemetryUseUSB(useUSB); }
//
// Control whether telemetry accumulation and transmission is active or is suppressed.
//
#define TelemetryEnable(isActive)            { telemetry._isActive = (isActive); }
#define TelemetryIsEnabled()                 (telemetry._isActive)
//
// Control the rate at which TelemetryFTC on the PC polls for data when using USB.
//
#define TelemetrySetUSBPollInterval(ms) _TelemetrySetUSBPollInterval_(telemetry, ms)

//===================================================================================
//===================================================================================
//===
//===  Private implementation. Please do not become reliant upon the details
//===  in what follows below.
//===
//===================================================================================
//===================================================================================

typedef enum
    {
    _TELEM_TYPE_NONE = 0,
    _TELEM_TYPE_ENDOFRECORDSET,
    _TELEM_TYPE_INT8,
    _TELEM_TYPE_INT16,
    _TELEM_TYPE_INT32,
    _TELEM_TYPE_UINT8,
    _TELEM_TYPE_UINT16,
    _TELEM_TYPE_UINT32,
    _TELEM_TYPE_FLOAT,
    _TELEM_TYPE_STRING,
    _TELEM_TYPE_BOOL,
    _TELEM_TYPE_CHAR,
    } _TELEM_TYPE;

#define _TELEM_CB_PAYLOAD_MAX 64

typedef struct
    {
    union                              // internal use only
        {
        float _fl;
        short _sh;
        long  _l;
        struct
            {
            unsigned byte _b0;
            unsigned byte _b1;
            unsigned byte _b2;
            unsigned byte _b3;
            };
        };
    //
    long            serialNumber;      // a handy variable useful for programs, perhaps in order to count telemetry records as they are sent
    //
    int             _isActive;                        // internal use only
    int             _recordOverflowed;                // internal use only
    int             _useBluetooth;                    // internal use only
    int             _useUSB;                          // internal use only
    TMailboxIDs     _bluetoothMailbox;                // internal use only
    unsigned byte   _ibMsgNext;                       // internal use only
    unsigned byte   _rgbMsg[_TELEM_CB_PAYLOAD_MAX];   // internal use only
    } _TELEMETRY;

_TELEMETRY telemetry;   // The one _TELEMETRY variable

#define _CbTelemetryRecord_(t)          ((int)(t._ibMsgNext - 1))
#define _PackTelemetryByteCount_(t)     { t._rgbMsg[0]=_CbTelemetryRecord_(t); }
#define _ResetTelemetryByteCount_(t)    { t._ibMsgNext = 1; }
#define _AppendTelemetryByte_(t,b)      { t._rgbMsg[t._ibMsgNext] = ((byte)b); t._ibMsgNext++; }
#define _HasCbAvailableTelemetry_(t,cb) (_TELEM_CB_PAYLOAD_MAX - t._ibMsgNext >= cb)
#define _FirstTelemetryTag_(t)          t._rgbMsg[1]

// mailbox1 is used by joysticks
// mailbox2 is used by the Samantha module
// we use mailbox3 to avoid conflicts
#define _TelemetryInitialize_(t)                          \
    {                                                     \
    _TelemetryResetRecord_(t);                            \
    t._isActive = true;                                   \
    t.serialNumber = 0;                                   \
    t._useBluetooth = false;                              \
    t._useUSB = true;                                     \
    t._bluetoothMailbox = mailbox3;                       \
    _TelemetryEmptyPollBuffer_();                         \
    }

// Prepare the telemetry for sending a new record
#define _TelemetryResetRecord_(t)                         \
    {                                                     \
    _ResetTelemetryByteCount_(t);                         \
    t._recordOverflowed = false;                          \
    }


#define _TelemetryAdd_(t, tag, bFirst, cb)                  \
    {                                                       \
    if (_HasCbAvailableTelemetry_(t,cb))                    \
        {                                                   \
        if (!t._recordOverflowed)                           \
            {                                               \
            _AppendTelemetryByte_(t, (byte)(tag));          \
            memcpy(t._rgbMsg[t._ibMsgNext], t.bFirst, cb);  \
            t._ibMsgNext += cb;                             \
            }                                               \
        }                                                   \
    else                                                    \
        {                                                   \
        t._recordOverflowed = true;                         \
        }                                                   \
    }
#define _TelemetryAddFloat_(t, flVal)                     \
    {                                                     \
    if (t._isActive) {                                    \
        t._fl = (float)flVal;                             \
        _TelemetryAdd_(t, _TELEM_TYPE_FLOAT, _b0, 4);     \
        }                                                 \
    }

#define _TelemetryAddBool_(t, boolVal)                    \
    {                                                     \
    if (t._isActive) {                                    \
        t._b0 = (unsigned byte)(!!(boolVal));             \
        _TelemetryAdd_(t, _TELEM_TYPE_BOOL, _b0, 1);      \
        }                                                 \
    }
#define _TelemetryAddChar_(t, charVal)                    \
    {                                                     \
    if (t._isActive) {                                    \
        t._b0 = (unsigned byte)(charVal);                 \
        _TelemetryAdd_(t, _TELEM_TYPE_CHAR, _b0, 1);      \
        }                                                 \
    }

#define _TelemetryAddInt8_(t, bVal)                       \
    {                                                     \
    if (t._isActive) {                                    \
        t._sh = (short)bVal;                              \
        _TelemetryAdd_(t, _TELEM_TYPE_INT8, _b0, 1);      \
        }                                                 \
    }
#define _TelemetryAddInt16_(t, shVal)                     \
    {                                                     \
    if (t._isActive) {                                    \
        t._sh = (short)shVal;                             \
        _TelemetryAdd_(t, _TELEM_TYPE_INT16, _b0, 2);     \
        }                                                 \
    }
#define _TelemetryAddInt32_(t, lVal)                      \
    {                                                     \
    if (t._isActive) {                                    \
        t._l = (long)lVal;                                \
        _TelemetryAdd_(t, _TELEM_TYPE_INT32, _b0, 4);     \
        }                                                 \
    }

#define _TelemetryAddUInt8_(t, bVal)                      \
    {                                                     \
    if (t._isActive) {                                    \
        t._sh = (short)bVal;                              \
        _TelemetryAdd_(t, _TELEM_TYPE_UINT8, _b0, 1);     \
        }                                                 \
    }
#define _TelemetryAddUInt16_(t, shVal)                    \
    {                                                     \
    if (t._isActive) {                                    \
        t._sh = (short)shVal;                             \
        _TelemetryAdd_(t, _TELEM_TYPE_UINT16, _b0, 2);    \
        }                                                 \
    }
#define _TelemetryAddUInt32_(t, lVal)                     \
    {                                                     \
    if (t._isActive) {                                    \
        t._l = (long)lVal;                                \
        _TelemetryAdd_(t, _TELEM_TYPE_UINT32, _b0, 4);    \
        }                                                 \
    }

#define _TelemetryAddString_(t, str)                                                    \
    {                                                                                   \
    if (t._isActive) {                                                                  \
        int cch = strlen(str);                                                          \
        if (_HasCbAvailableTelemetry_(t, 2+cch))                                        \
            {                                                                           \
            if (!t._recordOverflowed)                                                   \
                {                                                                       \
                /* This is slow, but careful. 'Would like more zip */                   \
                _AppendTelemetryByte_(t,(byte)(_TELEM_TYPE_STRING));                    \
                _AppendTelemetryByte_(t, cch);                                          \
                for (int ich = 0; ich < cch; ich++)                                     \
                    {                                                                   \
                    char ch = strIndex(str,ich);                                        \
                    _AppendTelemetryByte_(t, ch);                                       \
                    }                                                                   \
                }                                                                       \
            }                                                                           \
        else                                                                            \
            {                                                                           \
            t._recordOverflowed = true;                                                 \
            }                                                                           \
        }                                                                               \
    }

#define _TelemetryDone_(t)                                                            \
    {                                                                                 \
    if (t._isActive) {                                                                \
        _TelemetryResetRecord_(t);                                                    \
        _AppendTelemetryByte_(t, _TELEM_TYPE_ENDOFRECORDSET);                         \
        _TelemetrySend_(t,0);                                                         \
        }                                                                             \
    }

// Prepare the record for transmission to the telemetry recorder
#define _TelemetryFinalizeRecord_(t, isheet)                                          \
    {                                                                                 \
    _PackTelemetryByteCount_(t);                                                      \
    /* hi bits of first tag are sheet number */                                       \
    _FirstTelemetryTag_(t) |= ((isheet) << 4);                                        \
    }                                                                                 \

// Empty out the polling buffer used in the non-bluetooth case
#define _TelemetryEmptyPollBuffer_()            \
    {                                           \
    short ioResult = 0;                         \
    ubyte rgbPtr[2];                            \
    rgbPtr[0] = 0;                              \
    rgbPtr[1] = 0;                              \
    nxtWriteIOMap(_strCommMap, ioResult, rgbPtr[0], _dibHsInBuf + _dibHSBUFInPtr, 2);  \
    }

typedef enum
    {
    _TELEMETRY_META_COMMAND_POLLING_INTERVAL        = 0,
    _TELEMETRY_META_COMMAND_BACKCHANNEL_MAILBOX     = 1,
    _TELEMETRY_META_COMMAND_ZERO_TELEMETRY_DATA     = 2,
    } _TELEMETRY_META_COMMAND;

// Tell the telemetry recorder to poll us with the indicated polling interval
#define _TelemetrySetUSBPollInterval_(t, ms)                                \
    {                                                                       \
    t._rgbMsg[0] = 0x80 | 3; /* 0x80==meta, 3==#bytes of payload */         \
    t._rgbMsg[1] = _TELEMETRY_META_COMMAND_POLLING_INTERVAL;                \
    t._rgbMsg[2] = ( (ms)       & 0xFF);                                    \
    t._rgbMsg[3] = (((ms) >> 8) & 0xFF);                                    \
    t._ibMsgNext = 4;                                                       \
    _TelemetrySend_(t, 0, false);                                           \
    /* give TelemetryFTC program a chance to change the polling */          \
    wait1Msec(10);                                                          \
    }

//===================================================================================
//
// IOMap.h
//
// Definitions relating to various 'IOMaps' available on the NXT. IOMaps
// are a means by which particular parts of the RAM memory of the NXT are
// exposed to RobotC programs for reading and writing (using the nxtReadIOMap
// and nxtWriteIOMap APIs). By this means, access to various particular
// low-level parts of the firmware state is possible.

//--------------------------------------------------------------------
// A place holder as big as a void* in the firmware
typedef long _INTPTR;

//--------------------------------------------------------------------
// Size constants taken from Lego Firmware 1.29
#define   _SIZE_OF_BT_NAME               16
#define   _SIZE_OF_BRICK_NAME            8
#define   _SIZE_OF_CLASS_OF_DEVICE       4
#define   _SIZE_OF_BT_PINCODE            16
#define   _SIZE_OF_BDADDR                7     // NOTE: real address is only six bytes. But extra character for NULL termination char
                                               //       But also note that address bytes can contain zeroes. So can't use standard
                                               //       'string' manipulation functions because they assum strings do not contain
                                               //       zeroes in the value bytes!

#define   _SIZE_OF_USBBUF                64
#define   _SIZE_OF_HSBUF                 128
#define   _SIZE_OF_BTBUF                 128
#define   _SIZE_OF_BT_DEVICE_TABLE       30
#define   _SIZE_OF_BT_CONNECT_TABLE      4      /* Index 0 is alway incomming connections */

//--------------------------------------------------------------------
// Struct definitions, with additional padding added
// as necessary so that IOMAPCOMM lays out correctly

typedef struct
    {
    byte        Name[_SIZE_OF_BT_NAME];
    byte        ClassOfDevice[_SIZE_OF_CLASS_OF_DEVICE];
    byte        BdAddr[_SIZE_OF_BDADDR];
    byte        DeviceStatus;
    byte        Spare1;
    byte        Spare2;
    byte        Spare3;

    byte        padding0;
    } _BDDEVICETABLE;

typedef struct
    {
    byte        Name[_SIZE_OF_BT_NAME];
    byte        ClassOfDevice[_SIZE_OF_CLASS_OF_DEVICE];
    byte        PinCode[_SIZE_OF_BT_PINCODE];
    byte        BdAddr[_SIZE_OF_BDADDR];
    byte        HandleNr;
    byte        StreamStatus;
    byte        LinkQuality;
    byte        Spare;

    byte        padding0;
    } _BDCONNECTTABLE;

typedef struct
    {
    byte        Name[_SIZE_OF_BT_NAME];     // 16
    byte        BluecoreVersion[2];         // 2
    byte        BdAddr[_SIZE_OF_BDADDR];    // 7
    byte        BtStateStatus;
    byte        BtHwStatus;
    byte        TimeOutValue;
    byte        Spare1;
    byte        Spare2;
    byte        Spare3;
    } _BRICKDATA;

#define _cbBRICKDATA (_SIZE_OF_BT_NAME + 2 + _SIZE_OF_BDADDR + 6)

typedef struct
    {
    byte        Buf[_SIZE_OF_BTBUF];
    byte        InPtr;
    byte        OutPtr;
    byte        Spare1;
    byte        Spare2;
    } _BTBUF;

#define _cbBTBUF (_SIZE_OF_BTBUF + 4)

typedef struct
    {
    byte        Buf[_SIZE_OF_HSBUF];
    byte        InPtr;
    byte        OutPtr;
    byte        Spare1;
    byte        Spare2;
    } _HSBUF;

#define _dibHSBUFInPtr       _SIZE_OF_HSBUF
#define _dibHSBUFOutPtr      (_SIZE_OF_HSBUF + 1)
#define _cbHSBUF             (_SIZE_OF_HSBUF + 4)

typedef struct
    {
    byte        Buf[_SIZE_OF_USBBUF];
    byte        InPtr;
    byte        OutPtr;     // in UsbOutBuf, this is #bytes in Buf that should be xmitd. c_comm.c(278) (at least in the *lego* firmware; robotc is seemingly different)
    byte        Spare1;
    byte        Spare2;
    } _USBBUF;

#define _dibUSBBUFInPtr      _SIZE_OF_USBBUF
#define _dibUSBBUFOutPtr     (_SIZE_OF_USBBUF + 1)
#define _cbUSBBUF            (_SIZE_OF_USBBUF + 4)

typedef struct
    {
    _INTPTR         pFunc;                                      // 0      // UWORD (*pFunc)(byte , byte , byte , byte , byte *, UWORD*);
    _INTPTR         pFunc2;                                     // 4      // void  (*pFunc2)(byte *);

    // BT related entries
    _BDDEVICETABLE  BtDeviceTable[_SIZE_OF_BT_DEVICE_TABLE];    // 8
    _BDCONNECTTABLE BtConnectTable[_SIZE_OF_BT_CONNECT_TABLE];  // 968

    //General brick data
    _BRICKDATA      BrickData;                                  // 1160

    _BTBUF          BtInBuf;                                    // 1191
    _BTBUF          BtOutBuf;                                   // 1323

    // HI Speed related entries
    _HSBUF          HsInBuf;                                    // 1455
    _HSBUF          HsOutBuf;                                   // 1587

    // USB related entries
    _USBBUF         UsbInBuf;                                   // 1719
    _USBBUF         UsbOutBuf;                                  // 1787
    _USBBUF         UsbPollBuf;                                 // 1855

    byte            BtDeviceCnt;                                // 1923
    byte            BtDeviceNameCnt;                            // 1924
    byte            HsFlags;                                    // 1925
    byte            HsSpeed;                                    // 1926
    byte            HsState;                                    // 1927
    byte            UsbState;                                   // 1928
    } _IOMAPCOMM; // see c_comm.iom in the Lego firmware sources


#define _dibBtDeviceTable  8
#define _dibBtConnectTable 968
#define _dibBrickData      1160
#define _dibBtInBuf        1191
#define _dibBtOutBuf       1323
#define _dibHsInBuf        1455
#define _dibHsOutBuf       1587
#define _dibUsbInBuf       1719
#define _dibUsbOutBuf      1787
#define _dibUsbPollBuf     1855
#define _dibBtDeviceCnt    1923
#define _cbIOMAPCOMM       1929

const string _strCommMap = "Comm.mod";

//===================================================================================

// Define a handy utlity macro
#ifndef _Min
#define _Min(a,b) ((a) < (b) ? (a) : (b))
#endif

// Write data into a circular buffer in the COMM IoMap
#ifndef _CircularCommWrite_
#define _CircularCommWrite_(rgbData, dibSrc, dibDest, cbToWrite, ioResult)                                      \
    {                                                                                                           \
    const int cbQuantum = 64;                                                                                   \
    for (int cbWritten = 0; cbWritten < (cbToWrite); cbWritten += cbQuantum)                                    \
        {                                                                                                       \
        int cbQuantumWrite = _Min(cbQuantum, cbToWrite - cbWritten);                                            \
        nxtWriteIOMap(_strCommMap, ioResult, rgbData[(dibSrc)+cbWritten], (dibDest)+cbWritten, cbQuantumWrite); \
        }                                                                                                       \
    }
#endif

// Send the telemetry record to the telemetry recorder
void _TelemetrySend_(_TELEMETRY& t, int isheet, bool fFinalize=true)
    {
    // Don't do anything if telemetry isn't actually active
    if (t._isActive) {
        // Finalize the record if asked to
        if (fFinalize)
            {
            _TelemetryFinalizeRecord_(t, isheet);
            }

        if (t._useBluetooth)
            {
            // We are to spontaneously transmit the record as soon as Bluetooth is free
            while (bBTBusy)
                {
                EndTimeSlice();
                }
            cCmdMessageWriteToBluetooth(_FirstTelemetryTag_(t), _CbTelemetryRecord_(t), t._bluetoothMailbox);
            while (nBluetoothCmdStatus==ioRsltCommPending)
                {
                EndTimeSlice();
                }
            }
        if (t._useUSB)
            {
            // Append the current contents of the telemetry buffer to IoMapComm.HsInBuf. This will be readable
            // remotely using a Poll Command (0xA2) with buffer number == 0x00 (Poll buffer) [sic].
            short ioResult = 0;

            // Find out the current location of the circular buffer pointers
            ubyte rgbPtr[2];
            nxtReadIOMap(_strCommMap, ioResult, rgbPtr[0], _dibHsInBuf + _dibHSBUFInPtr, 2);
            int ibInPtr  = rgbPtr[0];
            int ibOutPtr = rgbPtr[1];   // this may become stale while we run, but that will only increase available space, so it's safe

            // Figure out how much room there is in the buffer
            const int cbBuffer      = _SIZE_OF_USBBUF;                 // you'd think this should be SIZE_OF_HSBUF, but it isn't be due to a NXT firmware bug at c_comm.c(1127)
            const int cbUsed        = (ibInPtr - ibOutPtr) & (cbBuffer-1);
            const int cbFree        = cbBuffer - cbUsed - 1;           // need the -1 so that ptrs don't coincide when full
            const int cbWriteWanted = 1 + _CbTelemetryRecord_(t);      // +1 because we include the initial byte count at the start of the chunk
            const int cbToWrite     = _Min(cbFree, cbWriteWanted);

            // Write all or nothing
            if (cbToWrite == cbWriteWanted)
                {
                // Due to circular wrapping, we might have to write the data in two chunks. How much will we write in each?
                const int cbChunkFirst  = _Min(cbToWrite, cbBuffer - ibInPtr);
                const int cbChunkSecond = cbToWrite - cbChunkFirst;

                // Write each chunk. Note that we can only write in quanta of 64 bytes at a time, as that is all
                // that the nxtWriteIOMap API will allow (see _CircularCommWrite_).
                _CircularCommWrite_(t._rgbMsg, 0,            _dibHsInBuf+ibInPtr, cbChunkFirst,  ioResult);
                _CircularCommWrite_(t._rgbMsg, cbChunkFirst, _dibHsInBuf,         cbChunkSecond, ioResult);

                // Update the in ptr. This will, atomically, make the data available for transmission in the 'Poll' command
                rgbPtr[0] = (ibInPtr + cbToWrite) & (cbBuffer-1);
                nxtWriteIOMap(_strCommMap, ioResult, rgbPtr[0], _dibHsInBuf + _dibHSBUFInPtr, 1);
                }
            else
                {
                // writeDebugStreamLine("telemetry full");
                }
            }

        // Reset things so that the next telemetry record will be assembled correctly
        _TelemetryResetRecord_(t);
        }
    }
