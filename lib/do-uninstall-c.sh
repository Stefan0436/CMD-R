#!/bin/bash
if [ -f "/usr/bin/cmd-r" ]; then
    echo Removing CMD-R...
    
    rm -r -f /usr/lib/cmd-r
    rm -f /usr/bin/cmd-r
    rm -f /usr/bin/update-cmdr
    
    echo Successfully uninstalled CMD-R, you can reinstall by running \'make install\' \(but your configs have been deleted permanently\)
else
    echo CMD-R is not installed, nothing to do, exiting...
fi
