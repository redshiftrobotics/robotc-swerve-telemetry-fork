# Telemetry and other projects

## Introduction

Swerve has a great [software kit][1] for FTC and FRC. Unfortunately, it's only built for an old version of RobotC. Therefore, we forked their project, and are maintaining it.

## Components

### Telemetry

Telemetry lets you record arbitrary data on the NXT and send it back to be dumped into an Excel sheet on the computer. This is the main focus.

### Music

Music.h is a small library letting you play sounds on the NXT. It's currently broken.

### FTC Field control

This is a much improved version of the FTC field control's waitForStart() method. When you call waitForStart() with this updated library, it will ask you which of several programs you want to run.

### Improved Joystick driver

This is a rewritten version of the RobotC joystick driver. It has some extra bugfixes, and may or may not work (I haven't tested).

## Contributing

Use the GitHub Flow. Here are the rules for contributing:

 0. Don't break the build

That's it. Just send a Pull Request.

 [1]: http://www.ftc417.org/ssk