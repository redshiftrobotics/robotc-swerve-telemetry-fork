#!/usr/bin/env python

import re

def evalReplacement(str, letterId, replacement):
	str = re.sub(r'/\*INSP' + letterId + '\*/', replacement, str)

# open and read the file. for now, this assumes this file is read from the same directory.
# TODO: accept arguments for what file to operate on.
templatefd = open('autoteliop.c')
templatestr = templatefd.read()

# calculate the array
# FIXME: this is fake and needs real calculation
encoderDataLength = 5

# insert the length of the array into the template
evalReplacement(templatestr, 'A', str(encoderDataLength))

