////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// JoystickDriver.h
//
// Part of the FTC Team 417 Software Starter Kit: www.ftc417.org/ssk
//
// A drop-in-compatible replacment for JoystickDriver.c, with enhancements and fixes.
//
// With the TETRIX system, the PC Controller Station sends messages over Bluetooth or Samantha to the NXT
// containing current settings of a PC joystick. The joystick settings arrive using the NXT
// "message mailbox" facility.
//
// End users almost never need to modify this file; rather, one simply #includes it, periodically
// calls getJoystickSettings(joystick), and then uses joyBtn(), joyHat(), joyX(), joyY() etc to
// examine the joystick state.
//
// Note that for the Logictech Gamepad F310 controller this code presently only functions
// correctly when the mode switch on the bottom of the controller is in 'D' mode rather
// than 'X' mode. The 'D' mode emulates a Logitech Dual Action controller, which was found
// in the original FTC kits.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

#if defined(NXT) || defined(TETRIX)
    #pragma autoStartTasks        // Automatically start tasks (which ones?) when the main user program starts.
#endif

////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Compability stuff for when this file stands alone
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef BOOL
#define BOOL    bool
#endif

////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Constants: some reasonable names for joystick-related buttons etc
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

// Easy-to-remember names for the various buttons on a joystick controller
typedef enum
    {
    JOYBTN_FIRST                =1,
    JOYBTN_1                    =JOYBTN_FIRST,
    JOYBTN_2                    =2,
    JOYBTN_3                    =3,
    JOYBTN_4                    =4,
    JOYBTN_X                    =JOYBTN_1,
    JOYBTN_Y                    =JOYBTN_4,
    JOYBTN_A                    =JOYBTN_2,
    JOYBTN_B                    =JOYBTN_3,
    JOYBTN_LEFTTRIGGER_UPPER    =5,
    JOYBTN_RIGHTTRIGGER_UPPER   =6,
    JOYBTN_LEFTTRIGGER_LOWER    =7,
    JOYBTN_RIGHTTRIGGER_LOWER   =8,
    JOYBTN_TOP_LEFT             =9,
    JOYBTN_TOP_RIGHT            =10,
    JOYBTN_JOYSTICK_LEFT        =11,
    JOYBTN_JOYSTICK_RIGHT       =12,
    JOYBTN_LAST                 =JOYBTN_JOYSTICK_RIGHT,
    } JOYBTN;

// Easy-to-remember names for the eight directions that the hat on a controller may be pushed
typedef enum
    {
    JOYHAT_NONE      = -1,              // hat not pressed
    JOYHAT_FIRST     = 0,
    JOYHAT_UP        = JOYHAT_FIRST,    // hat pressed up
    JOYHAT_UPRIGHT   = 1,               // hat pressed up and to the right
    JOYHAT_RIGHT     = 2,               // etc...
    JOYHAT_DOWNRIGHT = 3,
    JOYHAT_DOWN      = 4,
    JOYHAT_DOWNLEFT  = 5,
    JOYHAT_LEFT      = 6,
    JOYHAT_UPLEFT    = 7,
    JOYHAT_LAST      = JOYHAT_UPLEFT,
    } JOYHAT;

// Easy-to-remember names for the two joysticks on each joystick controller
typedef enum
    {
    JOY_LEFT=0,
    JOY_RIGHT=1,
    } IJOY;

// Names for the directions in which a joystick may be flicked. Note that
// that these may be 'or'd together, as in the following example:
//
//        if (joyFlickOnce(1,JOY_LEFT,JOYDIR_UP|JOYDIR_DOWN))
//            {
//            // Here do stuff common to both up and down
//            if (joyFlick(1,JOY_LEFT,JOYDIR_UP))
//                {
//                // Here do up stuff
//                }
//            else
//                {
//                // Here do down stuff
//                }
//            }
//
typedef enum
    {
    JOYDIR_UP    = 1,
    JOYDIR_DOWN  = 2,
    JOYDIR_LEFT  = 4,
    JOYDIR_RIGHT = 8,
    } JOYDIR;

// Value for size of the the neutral region of a joystick within which the
// joystick is to be considered not to have moved.
int joyFlickDeadZone    = 45;
int joyThrottleDeadZone = 15;

#ifndef Max
#define Max(a,b)    ((a) > (b) ? (a) : (b))
#endif

#ifndef Min
#define Min(a,b)    ((a) < (b) ? (a) : (b))
#endif

////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Structure declarations
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

// State of one joystick (there are two of these per controller)
typedef struct
    {
    short   x;                      // -128 to +127
    short   y;                      // -128 to +127
    } JOYMSG;

// State of one joystick controller as messaged from the PC
typedef struct
    {
    JOYMSG  rgjoy[2];
    short   buttons;                // bitmap of values computed from JOYBTN
    short   hat;                    // values are from JOYHAT
    } JOYCNTMSG;

// Auxiliary state we keep for each controller beyond that sent from the PC
typedef struct
    {
    short   buttonsOnce;            // Bitmap for managing single button presses
    short   hatOnce;                // Bitmap for managing single hat presses
    short   rgflicks[2];            // Bit map for supporting 'flicks' with the two joysticks in this controller
    } JOYCNTAUX;

// State of all the joystick controllers as updated from messages from the PC
typedef struct
    {
    BOOL        fTeleOp;            // autonomous(false) vs tele-operated(true) mode.
    BOOL        fWaitForStart;      // becomes false when the FTC Field Control System permits the program to proceed.
    long        serialNumber;       // # of current msg used to set this state. Used to prevent replay of messages.
    long        msReceived;         // time on the system clock (nSysTime) at which this message was received. Only updated when serialNumber is incremented.
    JOYCNTMSG   rgcnt[2];
    } JOYSTICKSMSG;

// State of all the joystick controllers, both the state from the PC and the once-management state
typedef struct
    {
    JOYSTICKSMSG    msg;            // as updated from the PC. This MUST come first in TJoystick
    JOYCNTAUX       rgaux[2];       // auxiliary state we maintain for each controller beyond that sent from the PC
    } TJoystick;

////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// User-callable joystick functions and macros.
//
// Call getJoystickSettings() to read the latest values from the joystick. It returns true if there's
// new data available to process since the last call; false otherwise.
//
// joyBtn() will return true or false accoring to the current state of the requested button; if the button
// is held, true is continually returned. By contrast, joyBtnOnce() will return true immediately after the
// button is pressed but will not again return true until the button is released and then pressed again.
//
// joyHat() and joyHatOnce() behave similarly
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

// The typical/usual variable used as the parameter for getJoystickSettings
TJoystick joystick;

// Update the joystick state into the indicated variable (almost always 'joystick'). Answer whether
// there's any new information about the joystick since the last time getJoystickSettings was called: if
// 'false' is returned, you probably should avoid processing the joystick data as you likely did previously
// when 'true' was returned.
BOOL getJoystickSettings(TJoystick& joystickVar);

// Manually update the state of this joystick varaible to the 'nothing pressed state'.
// Handy, but not perhaps as much as one might think.
#define ResetJoystickToNothingPressed(joystickVar)                              \
    {                                                                           \
    hogCpu(); /* avoid race with readMsgFromPC task */                          \
    memset(joystickVar.msg.rgcnt[0], 0, sizeof(JOYCNTMSG));                     \
    memset(joystickVar.msg.rgcnt[1], 0, sizeof(JOYCNTMSG));                     \
    memset(joystickVar.rgaux[0], 0, sizeof(JOYCNTAUX));                         \
    memset(joystickVar.rgaux[1], 0, sizeof(JOYCNTAUX));                         \
    joystickVar.msg.rgcnt[0].hat = JOYHAT_NONE;                                 \
    joystickVar.msg.rgcnt[1].hat = JOYHAT_NONE;                                 \
    releaseCpu();                                                               \
    }

// What's a reasonable length of time beyond which we believe that the Field Control System
// has just gone away.
#ifndef MS_JOYSTICK_FCS_DISCONNECTED_THRESHOLD
#define MS_JOYSTICK_FCS_DISCONNECTED_THRESHOLD  1000        // a reasonable value, but not exact by any means
#endif

// 'Functions' for accessing the various state of the two joystick controllers. Note that these *assume*
// that getJoystickSettings() was called with the variable 'joystick' as its parameter.
#define joyBtn(jyc,btn)             ((_joyBtnState(jyc) & _joyBtnBit(btn)) != 0)
#define joyHat(jyc,hatVal)          (joystick.msg.rgcnt[jyc-1].hat==(hatVal))
#define joyX(jyc,ijoy)              (joystick.msg.rgcnt[jyc-1].rgjoy[ijoy].x)
#define joyY(jyc,ijoy)              (joystick.msg.rgcnt[jyc-1].rgjoy[ijoy].y)

// Functions that test button and hat state but only return 'true' once per press
BOOL joyBtnOnce(int jyc, int btn);
BOOL joyHatOnce(int jyc, int hat);

// 'Flick'ing a joystick allows the four directions on each joystick (on each controller) to be used
// very much like additional controller buttons (instead of, say, a throttle as is typically done
// when manually driving).
//
// A 'flick once' will fire only once; it will not refire until the joystick is returned to the neutral
// dead zone (this is similar to joyBtnOnce()). If instead you wish to repeatedly fire so long as
// the joystick remains displaced, use joyFlick().
BOOL joyFlickOnce(int jyc, int ijoy, int flick);
BOOL joyFlick(int jyc, int ijoy, int flick);

// When was the last joystick message received?
#define joyMessageTime()    joystick.msg.msReceived

// How many joystick messages have we seen?
#define joyMessageCount()   joystick.msg.serialNumber

////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Legacy support - compatibility definitions - deprecated; to be eliminated over time.
// Try to avoid using these in new code - use joyBtn(), joyX(), etc instead (see above).
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

#define joy1_x1         msg.rgcnt[0].rgjoy[JOY_LEFT].x
#define joy1_y1         msg.rgcnt[0].rgjoy[JOY_LEFT].y
#define joy1_x2         msg.rgcnt[0].rgjoy[JOY_RIGHT].x
#define joy1_y2         msg.rgcnt[0].rgjoy[JOY_RIGHT].y
#define joy1_Buttons    msg.rgcnt[0].buttons
#define joy1_TopHat     msg.rgcnt[0].hat

#define joy2_x1         msg.rgcnt[1].rgjoy[JOY_LEFT].x
#define joy2_y1         msg.rgcnt[1].rgjoy[JOY_LEFT].y
#define joy2_x2         msg.rgcnt[1].rgjoy[JOY_RIGHT].x
#define joy2_y2         msg.rgcnt[1].rgjoy[JOY_RIGHT].y
#define joy2_Buttons    msg.rgcnt[1].buttons
#define joy2_TopHat     msg.rgcnt[1].hat

#define joy1Btn(btn)    ((joystick.msg.rgcnt[0].buttons & _joyBtnBit(btn)) != 0)
#define joy2Btn(btn)    ((joystick.msg.rgcnt[1].buttons & _joyBtnBit(btn)) != 0)

#define joyLeftX(jyc)   joyX(jyc,JOY_LEFT)
#define joyLeftY(jyc)   joyY(jyc,JOY_LEFT)
#define joyRightX(jyc)  joyX(jyc,JOY_RIGHT)
#define joyRightY(jyc)  joyY(jyc,JOY_RIGHT)


////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Internals
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

#define _joyBtnOnce_(jyc,btn)                                                   \
    (joyBtn(jyc,btn)                                                            \
        ? ((_joyBtnOnceState(jyc) & _joyBtnBit(btn)) != 0                       \
                ? false                                                         \
                : _trueOf(_joyBtnOnceState(jyc) |= _joyBtnBit(btn)))            \
        : _falseOf(_joyBtnOnceState(jyc) &= ~_joyBtnBit(btn)))

#define _joyHatOnce_(jyc,hat)                                                   \
    ((joyHat(jyc,hat))                                                          \
        ? ((_joyHatOnceState(jyc) & _joyHatBit(hat)) != 0                       \
                ? false                                                         \
                : _trueOf(_joyHatOnceState(jyc) |= _joyHatBit(hat)))            \
        : _falseOf(_joyHatOnceState(jyc) &= ~_joyHatBit(hat)))

#define _joyFlick_(jyc,ijoy,flick)                                                  \
    (((flick) & JOYDIR_UP) && (joyY(jyc,ijoy) > joyFlickDeadZone)                   \
        ? true                                                                      \
        : (((flick) & JOYDIR_DOWN) && (joyY(jyc,ijoy) < -joyFlickDeadZone)          \
            ? true                                                                  \
            : (((flick) & JOYDIR_RIGHT) && (joyX(jyc,ijoy) > joyFlickDeadZone)      \
                ? true                                                              \
                : (((flick) & JOYDIR_LEFT) && (joyX(jyc,ijoy) < -joyFlickDeadZone)  \
                    ? true                                                          \
                    : false))))

#define _joyFlickOnce_(jyc,ijoy,flick)                                          \
    (_joyFlick_(jyc,ijoy,flick)                                                 \
        ? ((_joyFlickState(jyc,ijoy) & (flick)) != 0                            \
            ? false                                                             \
            : _trueOf(_joyFlickState(jyc,ijoy) |= (flick)))                     \
        : _falseOf(_joyFlickState(jyc,ijoy) &= ~(flick)))

// The PC delivers joystick messages to a particular mailbox address in the NXT firmware, namely this one
const TMailboxIDs kJoystickQueueID = mailbox1;

// Internal buffer to hold the last received message from the PC (do not directly use)
JOYSTICKSMSG _joystickInternal;

// Copy the msg portion of the structure from the internal messaging state.
// NB: memcpy, being one opcode, is atomic
#define _getJoystickSettingsPrim(joystickVar)     memcpy(joystickVar, _joystickInternal, sizeof(_joystickInternal))

// Update the joystick state. Answer whether there's anything new there to process.
BOOL getJoystickSettings(TJoystick& joystickVar)
    {
    long serialNumberPrev = joystickVar.msg.serialNumber;
    _getJoystickSettingsPrim(joystickVar);
    return joystickVar.msg.serialNumber != serialNumberPrev;
    }

#define _joyBtnBit(btn)              (1 << (btn - 1))
#define _joyHatBit(hat)              (1 << (hat))                        // 'hat' is from JOY_HAT. NB: don't use with '-1'
#define _joyBtnState(jyc)            joystick.msg.rgcnt[jyc-1].buttons
#define _joyHatState(jyc)            joystick.msg.rgcnt[jyc-1].hat

#define _joyBtnOnceState(jyc)        joystick.rgaux[jyc-1].buttonsOnce
#define _joyHatOnceState(jyc)        joystick.rgaux[jyc-1].hatOnce
#define _joyFlickState(jyc,ijoy)     joystick.rgaux[jyc-1].rgflicks[ijoy]

#define _trueOf(expr)                ((expr) || 1)
#define _falseOf(expr)               ((expr) && 0)

BOOL joyBtnOnce(int jyc, int btn)
    {
    return _joyBtnOnce_(jyc, btn);
    }
BOOL joyHatOnce(int jyc, int hat)
    {
    return _joyHatOnce_(jyc, hat);
    }
BOOL joyFlickOnce(int jyc, int ijoy, int flick)
    {
    return _joyFlickOnce_(jyc, ijoy, flick);
    }
BOOL joyFlick(int jyc, int ijoy, int flick)
    {
    return _joyFlick_(jyc, ijoy, flick);
    }
////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//                                        Receive Messages Task
//
// Dedicated task that continuously polls for a Bluetooth/Samantha message from the PC containing the joystick
// values. Operation of this task is nearly transparent to the end user as the earlier "#pragma autoStartTasks"
// statement above will cause it to start running as soon as the user program is started.
//
// The task is very simple. It's an endless loop that continuously looks for a mailbox
// message from the PC. When one is found, it reformats and copies the contents into the internal
// "_joystickInternal" buffer.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

#define _InitializeJoystickGlobals_()                        \
    {                                                        \
    memset(_joystickInternal, 0, sizeof(_joystickInternal)); \
    _joystickInternal.fWaitForStart  = true;                 \
    _joystickInternal.rgcnt[0].hat   = -1;                   \
    _joystickInternal.rgcnt[1].hat   = -1;                   \
    }

// Logically, _rgbJoyMsgT is a local variable to the readMsgFromPC task. However,
// in RobotC 3.0, this needs to be truly global or a code-generation bug
// (of as yet unknown nature) is tickled and the joystick no longer functions.
const int _cbJoyMsgTMax = 19;       // 18 was good enough for RobotC 2.26, but ok if we always give more (and 3.0 needs 19 for some reason)
sbyte _rgbJoyMsgT[_cbJoyMsgTMax];

task readMsgFromPC()
    {

    // Initialize setting to default values in case communications with PC is broken.
    _InitializeJoystickGlobals_();

    while (true)
        {

        // Check to see if a message is available.
        BOOL fMsgFound = false;
        while (true)
            {
            // There may be more than one message in the queue. We want to get to the last received
            // message and discard the earlier "stale" messages. This loop simply discards all but
            // the last message.
            //
            int cbMessage = cCmdMessageGetSize(kJoystickQueueID);
            if (cbMessage <= 0)
                {
                if (!fMsgFound)
                    {
                    wait1Msec(4);    // Give other tasks a chance to run
                    continue;        // No message this time. Loop again
                    }
                //
                // No more messages available and at least one message found. We simply discard earlier
                // messages: each joystick message is self contained, representing the entire state of
                // of the joysticks, so the earlier ones have no use.
                //
                break;
                }

            // OK: there's at least one message there; read it!
            if (cbMessage > sizeof(_rgbJoyMsgT))
                cbMessage = sizeof(_rgbJoyMsgT);

            // cCmdMessageRead returns
            //      ioRsltSuccess      on success
            //      ioRsltEmptyMailbox if the message queue is empty (but we call cCmdMessageGetSize first above)
            //      ERR_INVALID_SIZE   if the buffer passed here is too small
            //      a few others
            TFileIOResult nBTCmdRdErrorStatus = cCmdMessageRead((ubyte)_rgbJoyMsgT, cbMessage, kJoystickQueueID);

            if (ioRsltSuccess == nBTCmdRdErrorStatus)
                {
                // Repeat loop until there are no more messages in the queue. We only want to process the
                // last message in the queue.
                fMsgFound = true;
                }
            }

        hogCPU();   // grab CPU for duration of critical section

        _joystickInternal.fTeleOp                       = _rgbJoyMsgT[1];
        _joystickInternal.fWaitForStart                 = _rgbJoyMsgT[2];

        _joystickInternal.rgcnt[0].rgjoy[JOY_LEFT].x    = _rgbJoyMsgT[3];
        _joystickInternal.rgcnt[0].rgjoy[JOY_LEFT].y    = _rgbJoyMsgT[4];
        _joystickInternal.rgcnt[0].rgjoy[JOY_RIGHT].x   = _rgbJoyMsgT[5];
        _joystickInternal.rgcnt[0].rgjoy[JOY_RIGHT].y   = _rgbJoyMsgT[6];
        _joystickInternal.rgcnt[0].buttons              = (_rgbJoyMsgT[7] & 0x00FF) | (_rgbJoyMsgT[8] << 8);
        _joystickInternal.rgcnt[0].hat                  = _rgbJoyMsgT[9];

        _joystickInternal.rgcnt[1].rgjoy[JOY_LEFT].x    = _rgbJoyMsgT[10];
        _joystickInternal.rgcnt[1].rgjoy[JOY_LEFT].y    = _rgbJoyMsgT[11];
        _joystickInternal.rgcnt[1].rgjoy[JOY_RIGHT].x   = _rgbJoyMsgT[12];
        _joystickInternal.rgcnt[1].rgjoy[JOY_RIGHT].y   = _rgbJoyMsgT[13];
        _joystickInternal.rgcnt[1].buttons              = (_rgbJoyMsgT[14] & 0x00FF) | (_rgbJoyMsgT[15] << 8);
        _joystickInternal.rgcnt[1].hat                  = _rgbJoyMsgT[16];

        // If control is started with *no* joysticks attached (or at least none logically connected
        // to RobotC) then the message that arrives from the PC has *entirely* zero values for all joysticks.
        // Unfortunately, and annoyingly, in that condition, the hat is logically JOYHAT_UP, which
        // will be interpreted by many programs as the hat being actively pushed, which will cause
        // some action to be taken by the program, almost certainly in error. As a work around, we
        // refuse to process any received messages until we see at least one with the first joystick's
        // hat not seemingly in the 'up' position. So: hands off the hat at the start of your program!
        if ((_joystickInternal.serialNumber != 0) || (_joystickInternal.rgcnt[0].hat != 0))
            {
            _joystickInternal.serialNumber++;
            _joystickInternal.msReceived = nSysTime;
            }

        _joystickInternal.rgcnt[0].rgjoy[JOY_LEFT].y    = -_joystickInternal.rgcnt[0].rgjoy[JOY_LEFT].y;  // Negate to "natural" position
        _joystickInternal.rgcnt[0].rgjoy[JOY_RIGHT].y   = -_joystickInternal.rgcnt[0].rgjoy[JOY_RIGHT].y; // Negate to "natural" position

        _joystickInternal.rgcnt[1].rgjoy[JOY_LEFT].y    = -_joystickInternal.rgcnt[1].rgjoy[JOY_LEFT].y;  // Negate to "natural" position
        _joystickInternal.rgcnt[1].rgjoy[JOY_RIGHT].y   = -_joystickInternal.rgcnt[1].rgjoy[JOY_RIGHT].y; // Negate to "natural" position

        releaseCPU(); // end of critical section
        }
    }

#if defined(TETRIX)

///////////////////////////////////////////////////////////////////////////////////////////
//
//                                        getUserControlProgram
//
// This function returns the name of the TETRIX User Control program. It reads the file
// "FTCConfig.txt" and builds the name of the file from the contents.
//
// Note that the filename includes the ".rxe" (robot executable file) file extension.
//
///////////////////////////////////////////////////////////////////////////////////////////

const string kConfigName = "FTCConfig.txt";

#define getUserControlProgram(szFileName)                   \
    {                                                       \
    szFileName = "";                                        \
                                                            \
    byte rgb[2];                                            \
    rgb[1] = 0;                                             \
                                                            \
    int             cbFile;                                 \
    TFileIOResult   nIoResult;                              \
    TFileHandle     hFileHandle = 0;                        \
                                                            \
    OpenRead(hFileHandle, nIoResult, kConfigName, cbFile);  \
    if (nIoResult == ioRsltSuccess)                         \
        {                                                   \
        for (int ib = 0; ib < cbFile; ++ib)                 \
            {                                               \
            ReadByte(hFileHandle, nIoResult,rgb[0]);        \
            strcat(szFileName, rgb);                        \
            }                                               \
                                                            \
        /* Delete the ".rxe" file extension */              \
        int ichFileExt = strlen(szFileName) - 4;            \
        if (ichFileExt > 0)                                 \
            StringDelete(szFileName, ichFileExt, 4);        \
        }                                                   \
    Close(hFileHandle, nIoResult);                          \
    }

///////////////////////////////////////////////////////////////////////////////////////////
//
//                                        displayDiagnostics
//
// This task will display diagnostic information about a TETRIX robot on the NXT LCD.
//
// If you want to use the LCD for your own debugging use, call the function
// "disableDiagnosticsDisplay()
//
///////////////////////////////////////////////////////////////////////////////////////////

#ifndef USE_DISPLAY_DIAGNOSTICS
#define USE_DISPLAY_DIAGNOSTICS 1
#endif

#if USE_DISPLAY_DIAGNOSTICS
BOOL bDisplayDiagnostics = true;  // Set to false in user program to disable diagnostic display

#define disableDiagnosticsDisplay()       \
    {                                     \
    bDisplayDiagnostics = false;          \
    }

task displayDiagnostics()
    {
    string szFileName;
    getUserControlProgram(szFileName);

    bNxtLCDStatusDisplay = true;
    while (true)
        {
        if (bDisplayDiagnostics)
            {
            nxtDisplayTextLine(6, "Teleop FileName:");
            nxtDisplayTextLine(7, szFileName);

            _getJoystickSettingsPrim(joystick);               // Update variables with current joystick values

            if (joystick.msg.fWaitForStart)
                nxtDisplayCenteredTextLine(1, "Wait for Start");
            else if (joystick.msg.fTeleOp)
                nxtDisplayCenteredTextLine(1, "TeleOp Running");
            else
                nxtDisplayCenteredTextLine(1, "Auton Running");

            if ( externalBatteryAvg < 0)
                nxtDisplayTextLine(3, "Ext Batt: OFF");       //External battery is off or not connected
            else
                nxtDisplayTextLine(3, "Ext Batt:%4.1f V", externalBatteryAvg / (float) 1000);

            nxtDisplayTextLine(4, "NXT Batt:%4.1f V", nAvgBatteryLevel / (float) 1000);   // Display NXT Battery Voltage

            nxtDisplayTextLine(5, "FMS Msgs: %d", joystick.msg.serialNumber);   // Display Count of FMS messages
            }

        wait1Msec(200);
        }
    }
#endif


/////////////////////////////////////////////////////////////////////////////////////////////////////
//
//                                    waitForStart
//
// Wait for the start of either the autonomous or tele-op phase. User program is running on the NXT
// but the phase has not yet started. The FC (Field Control System) is continually sending information
// to the NXT. This program loops getting the latest value of joystick settings. When it finds that
// the FCS has started the phase, it immediately returns.
//
// Note: this has been modified to provide an optional escape hatch: pressing the Orange Button
// will manually force a return from the waitForStart() call. This is useful for debugging and
// prototyping situations.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef USE_WAIT_FOR_START
#define USE_WAIT_FOR_START  1
#endif

#ifndef DISPLAY_MESSAGE_DEFAULT
#define DISPLAY_MESSAGE_DEFAULT "<-all is well->"
#endif

#if USE_WAIT_FOR_START

// Define here in a simple way so that, at least, this file is self contained.
// See also display.h, where a better sol'n is provided.
#ifndef DisplayMessage
#define DisplayMessage(message)     nxtDisplayCenteredTextLine(7, "%s", message)
#endif

// With this escape hatch, if the orange button is pressed within the first
// two seconds, the waitForStart() terminates. Note that the !(nNxtButtonTask = -1)
// at the end of OrangeButtonWaitBreak() is not a bug, but rather is intentionally
// both restoring the default button processing after the two seconds and returning
// false so as to not terminate the waitForStart().
#define OrangeButtonWaitInit()      { nNxtButtonTask = -2; /* tell the NXT OS that we want the buttons */ }
#define OrangeButtonWaitCond()      (MsSinceWaitForStart() <= OrangeButtonWaitTimeThreshold())
#define OrangeButtonWaitBreak()     (OrangeButtonWaitCond() ? nNxtButtonPressed==kEnterButton: !(nNxtButtonTask = -1))
#define OrangeButtonWaitFeedback()  {                                                                                        \
                                    if (OrangeButtonWaitDoFeedback())                                                        \
                                        {                                                                                    \
                                        DisplayMessage((MsSinceWaitForStart() / 2000 % 2 == 0) ? "orange==go" : "wait for start"); \
                                        }                                                                                    \
                                    }
#define OrangeButtonWaitDone()      {                                                                                        \
                                    nNxtButtonTask = -1; /* return buttons to their default processing */                    \
                                    DisplayMessage(DISPLAY_MESSAGE_DEFAULT);                                                 \
                                    }

#ifndef OrangeButtonWaitDoFeedback
#define OrangeButtonWaitDoFeedback() true
#endif

#ifndef OrangeButtonWaitTimeThreshold
#define OrangeButtonWaitTimeThreshold()     0x7FFFFFFF
#endif

// Unless some tells us not to, we turn on the orange button processing
#ifndef WAITFORSTART_USE_ORANGE_BUTTON
#define WAITFORSTART_USE_ORANGE_BUTTON  1
#endif

#if WAITFORSTART_USE_ORANGE_BUTTON
#ifndef WaitForStartInit
#define WaitForStartInit()        OrangeButtonWaitInit()
#endif
#ifndef WaitForStartBreak
#define WaitForStartBreak()       OrangeButtonWaitBreak()
#endif
#ifndef WaitForStartLoop
#define WaitForStartLoop()        OrangeButtonWaitFeedback()
#endif
#ifndef WaitForStartDone
#define WaitForStartDone()        OrangeButtonWaitDone()
#endif
#endif

long msWaitForStart;
#define MsSinceWaitForStart()   (nSysTime - msWaitForStart)

#define waitForStart()                      \
    {                                       \
    msWaitForStart = nSysTime;              \
    WaitForStartInit();                     \
    while (true)                            \
        {                                   \
        WaitForStartLoop();                 \
        getJoystickSettings(joystick);      \
        if (!joystick.msg.fWaitForStart)    \
            break;                          \
        if (WaitForStartBreak())            \
            break;                          \
        }                                   \
    WaitForStartDone();                     \
    }

#endif

#endif
