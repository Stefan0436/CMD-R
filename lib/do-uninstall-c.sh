#!/bin/bash
if [ -f "/usr/bin/cmd-r" ]; then
    echo Removing CMD-R...
    
    rm -r -f /usr/lib/cmd-r
    rm -f /usr/bin/cmd-r
    rm -f /usr/bin/update-cmdr
    
    echo Successfully uninstalled CMD-R.
else
    echo CMD-R is not installed, nothing to do, exiting...
fi
