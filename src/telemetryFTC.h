//
// TelemetryFTC.h
//
// Support for sending a data log in real time to an Excel spreadsheet
// rather than accumulating it in a file on the NXT for later uploading.
// This requires the additional support of the TelemetryFTC program running
// on the PC. See www.ftc417.org/TelemetryFTC for details.

//--------------------------------------------------------------------------
// Public functions

// Call TelemetryInitialize() once, before any of the TelemetryAddXXX functions
#define TelemetryInitialize()       TelemetryInitialize_(telemetry)

// Call the following with appropriate data to construct a telemetry record
#define TelemetryAddInt8(bVal)      TelemetryAddInt8_(telemetry,bVal)
#define TelemetryAddInt16(shVal)    TelemetryAddInt16_(telemetry,shVal)
#define TelemetryAddInt32(lVal)     TelemetryAddInt32_(telemetry,lVal)
#define TelemetryAddUInt8(bVal)     TelemetryAddUInt8_(telemetry,bVal)
#define TelemetryAddUInt16(shVal)   TelemetryAddUInt16_(telemetry,shVal)
#define TelemetryAddUInt32(lVal)    TelemetryAddUInt32_(telemetry,lVal)
#define TelemetryAddFloat(flVal)    TelemetryAddFloat_(telemetry,flVal)
#define TelemetryAddString(str)     TelemetryAddString_(telemetry,str)

// Call to transmit the currently constructed telemetry record and
// then initialize a new, empty one. TelemetrySendStream allows
// independent streams of telemetry records to be transmitted.
#define TelemetrySend()             TelemetrySend_(telemetry,0)
#define TelemetrySendStream(istm)   TelemetrySend_(telemetry,istm)

// (Optional) inform the recevier that no more telemetry will be
// forthcoming from this program.
#define TelemetryDone()             TelemetryDone_(telemetry)

// Note: the variable 'telemetry.serialNumber' is also for public use. It is a
// handy variable, useful for keep track of the number of telemetry records which
// have been transmitted. Note that if you wish to include the serialNumber
// in a record, you must do so manually:
//      TelemetryAddInt32(telemetry.serialNumber)
// Further note that you must also manually increment the serial number.

//--------------------------------------------------------------------------
// Private implementation. Please do not become reliant upon the details herein.

typedef enum
    {
    TELEM_TYPE_NONE = 0,
    TELEM_TYPE_INT8,
    TELEM_TYPE_INT16,
    TELEM_TYPE_INT32,
    TELEM_TYPE_UINT8,
    TELEM_TYPE_UINT16,
    TELEM_TYPE_UINT32,
    TELEM_TYPE_FLOAT,
    TELEM_TYPE_STRING,
    TELEM_TYPE_EOF,
    } TELEM_TYPE;

#define TELEM_CB_PAYLOAD_MAX 58

typedef struct
// NOTE: we are VERY sensitive to the layout of this struct.
// If ANY members are added, a round of correctness testing
// is in order. If you insist on adding members, you're most
// likely to have success if you add them to the end.
    {
    byte        rgbMsg[TELEM_CB_PAYLOAD_MAX];     // data to be transmitted
    int         cbMsg;                            // # of valid bytes currently in rgbMsg
    TMailboxIDs mailbox;                          // the mailbox / queue over which we are to communicate

    // This is a tricky little device for getting at the
    // internal memory layout of scalar data types.
    union
        {
        float fl;
        short sh;
        long  l;
        struct
            {
            unsigned char b0;
            unsigned char b1;
            unsigned char b2;
            unsigned char b3;
            };
        };

    long fActive;       // logging should actually be done or not
    long serialNumber;  // handy for clients; not used internally
    } TELEMETRY;

// The one global variable typically used for transmission
TELEMETRY telemetry;

#define TelemetryInitialize_(log)                       \
    {                                                   \
    log.cbMsg = 0;                                      \
    log.mailbox = mailbox2; /* numerically == 1 */      \
    log.fActive = false;                                \
    log.serialNumber = 0;                               \
    }

#define TelemetryAdd_(log, tag, bFirst, cb)             \
    {                                                   \
    if (log.fActive) {                                  \
        log.rgbMsg[log.cbMsg] = (byte)(tag);            \
        log.cbMsg += 1;                                 \
        memcpy(log.rgbMsg[log.cbMsg], log.bFirst, cb);  \
        log.cbMsg += cb;                                \
        }                                               \
    }
#define TelemetryAddFloat_(log, flVal)                  \
    {                                                   \
    if (log.fActive) {                                  \
        log.fl = (float)flVal;                          \
        TelemetryAdd_(log, TELEM_TYPE_FLOAT, b2, 4);    \
        }                                               \
    }

#define TelemetryAddInt8_(log, bVal)                    \
    {                                                   \
    if (log.fActive) {                                  \
        log.sh = (short)bVal;                           \
        TelemetryAdd_(log, TELEM_TYPE_INT8, b0, 1);     \
        }                                               \
    }
#define TelemetryAddInt16_(log, shVal)                  \
    {                                                   \
    if (log.fActive) {                                  \
        log.sh = (short)shVal;                          \
        TelemetryAdd_(log, TELEM_TYPE_INT16, b0, 2);    \
        }                                               \
    }
#define TelemetryAddInt32_(log, lVal)                   \
    {                                                   \
    if (log.fActive) {                                  \
        log.l = (long)lVal;                             \
        TelemetryAdd_(log, TELEM_TYPE_INT32, b2, 4);    \
        }                                               \
    }

#define TelemetryAddUInt8_(log, bVal)                   \
    {                                                   \
    if (log.fActive) {                                  \
        log.sh = (short)bVal;                           \
        TelemetryAdd_(log, TELEM_TYPE_UINT8, b0, 1);    \
        }                                               \
    }
#define TelemetryAddUInt16_(log, shVal)                 \
    {                                                   \
    if (log.fActive) {                                  \
        log.sh = (short)shVal;                          \
        TelemetryAdd_(log, TELEM_TYPE_UINT16, b0, 2);   \
        }                                               \
    }
#define TelemetryAddUInt32_(log, lVal)                  \
    {                                                   \
    if (log.fActive) {                                  \
        log.l = (long)lVal;                             \
        TelemetryAdd_(log, TELEM_TYPE_UINT32, b2, 4);   \
        }                                               \
    }

#define TelemetryAddString_(log, str)                           \
    {                                                           \
    if (log.fActive) {                                          \
        log.rgbMsg[log.cbMsg] = (byte)(TELEM_TYPE_STRING);      \
        log.cbMsg += 1;                                         \
        int cch = strlen(str);                                  \
        if (log.cbMsg + cch > TELEM_CB_PAYLOAD_MAX)             \
            {                                                   \
            cch = TELEM_CB_PAYLOAD_MAX - log.cbMsg;             \
            }                                                   \
        /* This is slow, but careful. 'Would like more zip */   \
        log.rgbMsg[log.cbMsg] = cch;                            \
        for (int ich = 0; ich < cch; ich++)                     \
            {                                                   \
            char ch = strIndex(str,ich);                        \
            log.rgbMsg[log.cbMsg+ich+1] = ch;                   \
            }                                                   \
        log.cbMsg += 1 + cch;                                   \
        }                                                       \
    }

#define TelemetryDone_(log)                                     \
    {                                                           \
    if (log.fActive) {                                          \
        log.rgbMsg[0] = (byte)(TELEM_TYPE_EOF);                 \
        log.cbMsg = 1;                                          \
        TelemetrySend_(log);                                    \
        }                                                       \
    }

#define TelemetrySend_(log, istm)                                               \
    {                                                                           \
    if (log.fActive) {                                                          \
        while (bBTBusy)                                                         \
            {                                                                   \
            EndTimeSlice();                                                     \
            }                                                                   \
        log.rgbMsg[0] |= ((istm) << 4);                                         \
        cCmdMessageWriteToBluetooth(log.rgbMsg[0], log.cbMsg, log.mailbox);     \
        while (nBluetoothCmdStatus==ioRsltCommPending)                          \
            {                                                                   \
            EndTimeSlice();                                                     \
            }                                                                   \
        log.cbMsg = 0;                                                          \
        }                                                                       \
    }
