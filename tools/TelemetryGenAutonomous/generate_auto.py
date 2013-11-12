#!/usr/bin/env python

import re

def evalReplacement(str, letterId, replacement):
	str = re.sub(r'/\*INSP' + letterId + '\*/', replacement, str)
	#return str

def genArray(encoders):	
	string = "{"
	j = 0
        print("encoder length is " + str(len(encoders)))
        # TODO refactor this loop, it could be prettier
	for i in encoders:
                j += 1
        	string += str(i)
                # special case, for if we're at the end of the data
                # TODO fix this to use the array length somehow
		if (j != len(encoders)):
			string += ", "
		else:
			string += "}"

                print(string)

# open and read the file. for now, this assumes this file is read from the same directory.
# TODO: accept arguments for what file to operate on.
templatefd = open('autoteliop.c')
templatestr = templatefd.read()

# calculate the array
# FIXME: this is fake and needs real calculation
encoderDataLength = 5

# insert the length of the array into the template
print evalReplacement(templatestr, 'A', str(encoderDataLength))

print genArray([1, 2, 3, 4])
