//
// FTCField.h
//
// Part of the FTC Team 417 Software Starter Kit: www.ftc417.org/ssk
//
// Implementation of a user interface on the NXT by which one can:
//
//      (a) choose between autonomous mode and teleop mode of a program
//      (b) for automous mode, choose between
//          (i)  being the red vs blue team, and
//          (ii) starting on the left or right side of the field
//      (c) choosing between a 'demo' and 'normal' mode of the program
//
// This UI is executed during the waitForStart() that one calls to await the start
// of a game round by the Field Control System (FCS). The UI herein, for debugging
// purposes, also allows waitForStart() to be manually forced by pressing the NXT
// orange button. The (a) and (b) parts of the UI are executed using the left and
// right gray arrows, while (c) is executed by pressing the gray button.
//
// Be sure to #include this file *before* you #include "joystickDriver.h" as it preempts
// some defaults that will be found therein.
//
// Note: the concept of 'left' vs 'right' side of the field was appropriate for
// the 2010-11 Get Over It competition, and may not be the most useful for later
// years. For now, the notion remains here in the code, but that may be adapted
// to something more useful in later games.

//-------------------------------------------------------------------------------------
// Variables and defaults
//-------------------------------------------------------------------------------------

typedef enum
    {
    PROGRAM_FLAVOR_TELEOP,
    PROGRAM_FLAVOR_AUTONOMOUS
    } PROGRAM_FLAVOR;

typedef enum
    {
    TEAM_COLOR_BLUE,
    TEAM_COLOR_RED,
    } TEAM_COLOR;

typedef enum
    {
    STARTING_SIDE_LEFT,
    STARTING_SIDE_RIGHT
    } STARTING_SIDE;

string rgstrProgramFlavor[] = { "tele op",   "auto"     };
string rgstrTeamColor[]     = { "blue team", "red team" };
string rgstrStartingSide[]  = { "left",      "right"    };

// Teleop can optionally be excluded from the UI (it is included by default).
// To do so, do a #define USE_TELEOP_UI 0 before including this file.
#ifndef DEFAULT_PROGRAM_FLAVOR
    #ifndef USE_TELEOP_UI
        #define USE_TELEOP_UI 1
    #endif
    #if USE_TELEOP_UI
        #define DEFAULT_PROGRAM_FLAVOR PROGRAM_FLAVOR_TELEOP
    #else
        #define DEFAULT_PROGRAM_FLAVOR PROGRAM_FLAVOR_AUTONOMOUS
    #endif
#endif

#ifndef DEFAULT_TEAM_COLOR
#define DEFAULT_TEAM_COLOR  TEAM_COLOR_BLUE
#endif

#ifndef DEFAULT_STARTING_SIDE
#define DEFAULT_STARTING_SIDE STARTING_SIDE_LEFT
#endif

#ifndef BOOL
#define BOOL int
#endif

// Examine these variables in your code determine the output of this user interface
//
PROGRAM_FLAVOR  programFlavor    = DEFAULT_PROGRAM_FLAVOR;      // are we to run teleop or autonomous mode once waitForStart returns?
TEAM_COLOR      teamColor        = DEFAULT_TEAM_COLOR;          // are we playing as the red team or the blue team?
STARTING_SIDE   startingSide     = DEFAULT_STARTING_SIDE;       // are starting on the left or right side of the field?
BOOL            fUseProgramDemoMode = false;                    // are we to use our 'demo' mode (if we have one) or not?

//--------------------------------------------------------------------------------------
// Team selection and field side selection
//--------------------------------------------------------------------------------------

// Alter some of the default behavior found in joystickDriver.h
#define OrangeButtonWaitDoFeedback()        false
#define OrangeButtonWaitTimeThreshold()     0x7FFFFFFF

// Define here in a simple way so that, at least, this file is self contained.
// See also display.h, where a better sol'n is provided (wherein the message
// is in fact displayed only temporarily).
#ifndef DisplayMessageTemporarily
#define DisplayMessageTemporarily(duration, message)    DisplayMessage(message)
#endif

// Our UI here displays temporary messages for this long
#define MS_DISPLAYMSG 750

// Some of the messages we wish to show need some composition
#define DisplayFormattedMessageTemporarily1(message, param)            { string s; StringFormat(s, message, param); DisplayMessageTemporarily(MS_DISPLAYMSG, s); }
#define DisplayFormattedMessageTemporarily2(message, param1, param2)   { string s; StringFormat(s, message, param1, param2); DisplayMessageTemporarily(MS_DISPLAYMSG, s); }
#define DisplayFormattedMessage1(message, param)                       { string s; StringFormat(s, message, param); DisplayMessage(s); }
#define DisplayFormattedMessage2(message, param1, param2)              { string s; StringFormat(s, message, param1, param2); DisplayMessage(s); }

// Define here so this file is self-contained. However, a better solution
// is to actually play the music; see Music.h.
#ifndef PlayMusic
#define PlayMusic(music,tempo,beat)
#endif
#ifndef PlayMusicNoWait
#define PlayMusicNoWait(music,tempo,beat) PlayMusic(music,tempo,beat)
#endif
#ifndef Click()
#define Click() PlayTone(440, 1)
#endif

// We can press the left or right arrow keys to indicate
//
//  (a) teleop vs autonomous mode
//  (b) our team color (red vs blue)
//  (b) the side of the field from which we are starting (left vs right)
//
// The cycle order traversed by the arrow keys is as follows:
//
//   (left/right) # red # tele # blue # (left/right)
//
// Pressing the gray rectangular button will toggle the program demo mode
// (however, a left or right arrow must be pressed between each toggle, lest
// the program be terminated by the firmware).
//
#define ToggleStartingSide()                                                                            \
    {                                                                                                   \
    if (STARTING_SIDE_LEFT==startingSide)                                                               \
        {                                                                                               \
        startingSide = STARTING_SIDE_RIGHT;                                                             \
        PlaySadNoWait();                                                                                \
        }                                                                                               \
    else                                                                                                \
        {                                                                                               \
        startingSide = STARTING_SIDE_LEFT;                                                              \
        PlayHappyNoWait();                                                                              \
        }                                                                                               \
    DisplayFormattedMessageTemporarily1("%s side", rgstrStartingSide[startingSide]);                    \
    }

#define WaitForStartInit()                                                                              \
    {                                                                                                   \
    nNxtExitClicks = 2;                                                                                 \
    OrangeButtonWaitInit();                                                                             \
    DisplayMessage(rgstrProgramFlavor[programFlavor]);                                                  \
    PlayTeamColorFeedback();                                                                            \
    }

#define WaitForStartDone()                                                                              \
    {                                                                                                   \
    Beep();                                                                                             \
    OrangeButtonWaitDone();                                                                             \
    nNxtExitClicks = 1;                                                                                 \
    }

#define WaitNoButtonPressed()    { while (kNoButton != nNxtButtonPressed) {} }

#define PlayTeamColorFeedback()                                                                         \
    {                                                                                                   \
    if (PROGRAM_FLAVOR_TELEOP==programFlavor)                                                           \
        {                                                                                               \
        PlayMusic(musicRobotDance, 240, NOTE_QUARTER);                                                  \
        }                                                                                               \
    else if (TEAM_COLOR_BLUE==teamColor)                                                                \
        {                                                                                               \
        PlayMusic(musicBluesRiff, 120, 3*NOTE_EIGHTH);   /* in 6/8 time! */                             \
        }                                                                                               \
    else                                                                                                \
        {                                                                                               \
        PlayMusic(musicRedRedWine, 200, NOTE_QUARTER);                                                  \
        }                                                                                               \
    }

#define WaitForStartLoop()                                                                              \
    {                                                                                                   \
    if (kExitButton == nNxtButtonPressed)                                                               \
        {                                                                                               \
        /* toggle demo mode */                                                                          \
        fUseProgramDemoMode = !fUseProgramDemoMode;                                                     \
        DisplayMessageTemporarily(MS_DISPLAYMSG, fUseProgramDemoMode ? "demo mode" : "normal mode");    \
        Beep();                                                                                         \
        }                                                                                               \
    else if (kLeftButton == nNxtButtonPressed)                                                          \
        {                                                                                               \
        if (PROGRAM_FLAVOR_AUTONOMOUS==programFlavor && TEAM_COLOR_RED==teamColor)                      \
            {                                                                                           \
            ToggleStartingSide();                                                                       \
            }                                                                                           \
        else                                                                                            \
            {                                                                                           \
            if (PROGRAM_FLAVOR_TELEOP==programFlavor || DEFAULT_PROGRAM_FLAVOR==PROGRAM_FLAVOR_AUTONOMOUS) \
                {                                                                                       \
                programFlavor = PROGRAM_FLAVOR_AUTONOMOUS;                                              \
                teamColor     = TEAM_COLOR_RED;                                                         \
                DisplayFormattedMessage2("%s %s", rgstrProgramFlavor[programFlavor], rgstrTeamColor[teamColor]); \
                }                                                                                       \
            else                                                                                        \
                {                                                                                       \
                programFlavor = PROGRAM_FLAVOR_TELEOP;                                                  \
                DisplayFormattedMessage1("%s", rgstrProgramFlavor[programFlavor]); \
                }                                                                                       \
            PlayTeamColorFeedback();                                                                    \
            }                                                                                           \
        }                                                                                               \
    else if (kRightButton == nNxtButtonPressed)                                                         \
        {                                                                                               \
        if (PROGRAM_FLAVOR_AUTONOMOUS==programFlavor && TEAM_COLOR_BLUE==teamColor)                     \
            {                                                                                           \
            ToggleStartingSide();                                                                       \
            }                                                                                           \
        else                                                                                            \
            {                                                                                           \
            if (PROGRAM_FLAVOR_TELEOP==programFlavor || DEFAULT_PROGRAM_FLAVOR==PROGRAM_FLAVOR_AUTONOMOUS) \
                {                                                                                       \
                programFlavor = PROGRAM_FLAVOR_AUTONOMOUS;                                              \
                teamColor   = TEAM_COLOR_BLUE;                                                          \
                DisplayFormattedMessage2("%s %s", rgstrProgramFlavor[programFlavor], rgstrTeamColor[teamColor]); \
                }                                                                                       \
            else                                                                                        \
                {                                                                                       \
                programFlavor = PROGRAM_FLAVOR_TELEOP;                                                  \
                DisplayFormattedMessage1("%s", rgstrProgramFlavor[programFlavor]); \
                }                                                                                       \
            PlayTeamColorFeedback();                                                                    \
            }                                                                                           \
        }                                                                                               \
    WaitNoButtonPressed();                                                                              \
    }
