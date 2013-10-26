# Telemetry

## Overview

Telemetry lets you transmist arbitrary data back to an Excel spreadsheet from the NXT. It's pretty great.

## Usage

See the header file. It's extensively commented.

## Bugs

The TelemetryFTC program is known to crash when it recieves too much data at once (or something like that). These results were derived from a program that adds x pieces of data, then transmits. It will do this y times (although note that in testing y was always 10). You can find this program in the testcases/ directory.

Your mileage may vary.

### Bluetooth

Tests have shown that when you use 12 as x in the program described above, TelemetryFTC will crash, whereas using 11 as x will work just fine. If you're running into this issue, try USB.

### USB

USB has not been systematically tested, but cursory testing shows that it yields better performance than Bluetooth.