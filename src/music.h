//
// Music.h
//
// Part of the FTC Team 417 Software Starter Kit: www.ftc417.org/ssk
//
// Handy and fun definitions for creating beeps and tunes on the NXT.
//
//--------------------------------------------------------------------------------------
// Constants
//--------------------------------------------------------------------------------------

typedef enum
// Constants for all the notes of a scale. Multiply or divide notes by powers of two
// to shift octaves. Also constants for note durations.
    {
    // Tones
    NOTE_A=440,               NOTE_A_SHARP=466,         NOTE_B=494,       NOTE_C=523,
    NOTE_C_SHARP=554,         NOTE_D=587,               NOTE_D_SHARP=622, NOTE_E=659,
    NOTE_F=698,               NOTE_F_SHARP=740,         NOTE_G=784,       NOTE_G_SHARP=831,
    NOTE_B_FLAT=NOTE_A_SHARP, NOTE_D_FLAT=NOTE_C_SHARP,
    NOTE_E_FLAT=NOTE_D_SHARP, NOTE_G_FLAT=NOTE_F_SHARP,
    NOTE_A_FLAT=NOTE_G_SHARP,
    NOTE_REST=0,              // not a real note: silence

    // Durations, in arbitrary units. Note that these may be added together to create intermediate durations
    NOTE_WHOLE      = 64,
    NOTE_HALF       = 32,
    NOTE_QUARTER    = 16,
    NOTE_EIGHTH     = 8,
    NOTE_SIXTEENTH  = 4,
    NOTE_32ND       = 2,
    NOTE_64TH       = 1,

    NOTE_DOT=0x8000,    // 'or' this with another value to add 50% to a note's duration
    NOTE_TIE=0x4000,    // 'or' this to indicate that a note is tied to the next note
    } NOTE;

//--------------------------------------------------------------------------------------
// Low-level
//--------------------------------------------------------------------------------------

void Beep(int note = NOTE_A, int msDuration=10)
// Note: this is a non-blocking beep
    {
    PlayTone(note, (msDuration + 5) / 10);
    }

#undef Click
#define Click() Beep()

//--------------------------------------------------------------------------------------
// Scores and music
//--------------------------------------------------------------------------------------

// The exact way we have to declare MUSIC here is a RobotC
// bogosity, but mostly a mild-ish one.
typedef int MUSIC[32767];   // 32767 so as to fool bounds check

int musicSOS[] =
    {
    NOTE_D,       NOTE_SIXTEENTH,  NOTE_REST, NOTE_SIXTEENTH,
    NOTE_D,       NOTE_SIXTEENTH,  NOTE_REST, NOTE_SIXTEENTH,
    NOTE_D,       NOTE_SIXTEENTH,  NOTE_REST, NOTE_SIXTEENTH,
    NOTE_D,       NOTE_QUARTER,
    NOTE_D,       NOTE_QUARTER,
    NOTE_D,       NOTE_QUARTER,
    NOTE_D,       NOTE_SIXTEENTH,  NOTE_REST, NOTE_SIXTEENTH,
    NOTE_D,       NOTE_SIXTEENTH,  NOTE_REST, NOTE_SIXTEENTH,
    NOTE_D,       NOTE_SIXTEENTH,  NOTE_REST, NOTE_SIXTEENTH,
    };

int musicFifth[] =
    {
    NOTE_G,      NOTE_EIGHTH,
    NOTE_G,      NOTE_EIGHTH,
    NOTE_G,      NOTE_EIGHTH,
    NOTE_E_FLAT, NOTE_HALF,
    };

int musicHappy[] =
    {
    NOTE_A,       NOTE_EIGHTH,
    NOTE_C_SHARP, NOTE_EIGHTH,
    NOTE_E,       NOTE_EIGHTH,
    };

int musicSad[] =
    {
    NOTE_E,       NOTE_EIGHTH,
    NOTE_C_SHARP, NOTE_EIGHTH,
    NOTE_A,       NOTE_EIGHTH,
    };

int musicLadyInRed[] =
    {
    NOTE_C,       NOTE_QUARTER,               // la
    NOTE_D,       NOTE_QUARTER,               // dy
    NOTE_E_FLAT,  NOTE_QUARTER,               // in
    NOTE_F,       NOTE_QUARTER + NOTE_WHOLE,  // red
    NOTE_G,       NOTE_QUARTER,               // is
    NOTE_G,       NOTE_QUARTER | NOTE_DOT,    // dan
    NOTE_F,       NOTE_EIGHTH | NOTE_TIE,     // cing
    NOTE_E_FLAT,  NOTE_QUARTER,               // ...
    NOTE_E_FLAT,  NOTE_EIGHTH | NOTE_TIE,     // with
    NOTE_D,       NOTE_EIGHTH + NOTE_EIGHTH,  // ...
    NOTE_D,       NOTE_HALF | NOTE_DOT,       // me
    };

int musicBluesRiff[] = // in 6/8 time
    {
    NOTE_A,       NOTE_EIGHTH,
    NOTE_D,       NOTE_EIGHTH,                // beat
    NOTE_A,       NOTE_EIGHTH,
    NOTE_C,       NOTE_EIGHTH,
    NOTE_A,       NOTE_EIGHTH,                // beat
    };

int musicRedRedWine[] =
    {
    NOTE_G/2,     NOTE_QUARTER,
    NOTE_B,       NOTE_QUARTER,
    NOTE_C,       NOTE_HALF,
    NOTE_A,       NOTE_QUARTER,
    };

int musicRobotDance[] =
    {
    NOTE_C,         NOTE_QUARTER,
    NOTE_C,         NOTE_EIGHTH,
    NOTE_D,         NOTE_EIGHTH,
    NOTE_E_FLAT,    NOTE_QUARTER,
    NOTE_E_FLAT,    NOTE_EIGHTH,
    NOTE_D,         NOTE_EIGHTH,
    NOTE_C,         NOTE_QUARTER,
    NOTE_E_FLAT,    NOTE_QUARTER,
    NOTE_G,         NOTE_HALF,
    };

void _PlayMusic_(
    MUSIC& rgNote, int cNote,   // the array of notes to play, along with its size
    int beatsPerMinute,         // the number of beats per minute
    int cTickBeat,              // number of music tick units that should be one beat
    BOOL fWait                  // whether we should wait until the music is finished or not before
                                // returning from this function: 'false' only works for five notes or less
    )
// Play the indicated music, consisting of cNote pairs of (tone, duration), at the indicated tempo
    {
    long mintesPerBeatWorthOfTicks = (long)(60000/cTickBeat);
    for (int iNote=0; iNote < cNote; iNote++)
        {
        int  tone          = rgNote[iNote*2];
        long ctickDuration = rgNote[iNote*2+1];
        BOOL fTie          = false;
        //
        if (ctickDuration & NOTE_TIE)
            {
            ctickDuration = ctickDuration & ~NOTE_TIE & 0xFFFF;
            fTie = true;
            }
        //
        if (ctickDuration & NOTE_DOT)
            {
            ctickDuration = (ctickDuration & ~NOTE_DOT & 0xFFFF);
            ctickDuration = ctickDuration + (ctickDuration >> 1);
            }
        //
        // tempo is in beats per minute. The math we actually use is a simplification of the following computation:
        //
        // msPerBeat = 60 * 1000 / tempo
        // cBeat     = ctickDuration / beat
        // cBeat * msPerBeat
        //
        int ms = (int) ((mintesPerBeatWorthOfTicks * (long)ctickDuration) / (long)beatsPerMinute);
        // TRACE(("%d %d %d", tone, ctickDuration, ms));
        //
        if (fTie)
            {
            PlayTone(tone, ms / 10);
            }
        else
            {
            int csNoteSpacing = 1;
            PlayTone(tone,      ms/10 - csNoteSpacing);
            PlayTone(NOTE_REST, csNoteSpacing);
            }
        //
        if (fWait)
            {
            while (bSoundActive)
                {
                }
            }
        }
    }

#undef PlayMusic
#undef PlayMusicNoWait

// Play the indicated music at the indicated tempo, blocking until the music completes.
#define PlayMusic(music,tempo,beat) _PlayMusic_(music, sizeof(music) / (sizeof(music[0])*2), tempo, beat, true)

// Play the music but don't block. Note: can only be used with scores of 10 (5?) notes/rests or less.
#define PlayMusicNoWait(music,tempo,beat) _PlayMusic_(music, sizeof(music) / (sizeof(music[0])*2), tempo, beat, false)

#define PlayHappy()         PlayMusic      (musicHappy, 400, NOTE_QUARTER)
#define PlayHappyNoWait()   PlayMusicNoWait(musicHappy, 400, NOTE_QUARTER)
void    PlaySad()           { PlayMusic    (musicSad,   400, NOTE_QUARTER); }  // we call this often, so fn is much smaller
#define PlaySadNoWait()     PlayMusicNoWait(musicSad,   400, NOTE_QUARTER)
#define PlayFifth()         PlayMusic      (musicFifth, 250, NOTE_QUARTER)
#define PlayFifthNoWait()   PlayMusicNoWait(musicFifth, 250, NOTE_QUARTER)

#define PlaySOS()           PlayMusic      (musicSOS,   160, NOTE_QUARTER)
