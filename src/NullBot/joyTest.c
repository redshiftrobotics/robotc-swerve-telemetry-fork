//
// JoyTest.c
//
#define USE_DISPLAY_DIAGNOSTICS 0
#include "..\JoystickDriver.h"
#include "..\TelemetryFTC.h"
//
#define Display(i, format, value)                   \
    {                                               \
    nxtDisplayCenteredTextLine(i, format, value);   \
    }
#define Display2(i, format, value, value2)                  \
    {                                                       \
    nxtDisplayCenteredTextLine(i, format, value, value2);   \
    }

void Beep(int note = 440, int csDuration=1)
    {
    PlayTone(note, csDuration);
    }

#define TestButton(jyc,buttonName)                  \
    {                                               \
    if (joyBtnOnce(jyc,buttonName))                 \
        {                                           \
        Beep();                                     \
        Display(3, "button %d", (int)buttonName);   \
        }                                           \
    }

void TraceJoystickController(int jyc)
    {
    for (int jButton=JOYBTN_FIRST; jButton <= (int)JOYBTN_LAST; jButton++)
        {
	    TestButton(jyc, jButton);
	    }

	bool fFound = false;
	for (int hat=JOYHAT_FIRST; hat<=(int)JOYHAT_LAST; hat++)
	    {
	    if (joyHatOnce(jyc, hat))
	        {
	        Beep();
	        Display(2, "hat %d", hat);
	        fFound = true;
	        }
	    }

	if (joyFlickOnce(jyc, JOY_LEFT, JOYDIR_UP))    { Beep(); Display(0, "left up",   0); }
	if (joyFlickOnce(jyc, JOY_LEFT, JOYDIR_DOWN))  { Beep(); Display(0, "left down", 0); }
	if (joyFlickOnce(jyc, JOY_LEFT, JOYDIR_LEFT))  { Beep(); Display(0, "left left", 0); }
	if (joyFlickOnce(jyc, JOY_LEFT, JOYDIR_RIGHT)) { Beep(); Display(0, "left right",0); }

	if (joyFlickOnce(jyc, JOY_RIGHT, JOYDIR_UP))    { Beep(); Display(1, "right up",   0); }
	if (joyFlickOnce(jyc, JOY_RIGHT, JOYDIR_DOWN))  { Beep(); Display(1, "right down", 0); }
	if (joyFlickOnce(jyc, JOY_RIGHT, JOYDIR_LEFT))  { Beep(); Display(1, "right left", 0); }
	if (joyFlickOnce(jyc, JOY_RIGHT, JOYDIR_RIGHT)) { Beep(); Display(1, "right right",0); }

	nxtDisplayCenteredTextLine(4, "left=%d,%d", (int)joyX(jyc, JOY_LEFT), (int)joyY(jyc,JOY_LEFT));
	nxtDisplayCenteredTextLine(5, "right=%d,%d", (int)joyX(jyc, JOY_RIGHT), (int)joyY(jyc,JOY_RIGHT));
	nxtDisplayCenteredTextLine(6, "hat=%d btn=%x", joyHatState(jyc), joyBtnState(jyc));
	}

task main()
    {
    BOOL fLogLost = true;
    for (;;)
        {
        if (getJoystickSettings(joystick))
            {
            if (fLogLost)
                {
                fLogLost = false;
                Display(7, "joystick connection gained", 0);
                }
            TraceJoystickController(1);
            }
        else if (nSysTime - joystick.msg.msReceived > MS_JOYSTICK_FCS_DISCONNECTED_THRESHOLD)
            {
            /* We haven't received a new message from the FCS in WAY too long  */
            /* So we have to consider ourselves disconnected. We take steps to */
            /* reign in a possibly runaway robot.                              */
            if (!fLogLost)
                {
                fLogLost = true;
                Display(7, "joystick connection lost", 0);
                }
            }

        /* Be nice: let other tasks run */
        EndTimeSlice();
        }
    }
