#!/bin/bash
if [ ! -d "./build" ]; then
    echo No build folder, nothing to do, exiting...
    exit
fi

if [ ! -f "/usr/bin/cmd-r" ]; then
    echo Installing CMD-R...
    
    mkdir /usr/lib/cmd-r
    cp build/* /usr/lib/cmd-r -r -f
    
    cp lib/cmd-r /usr/bin/cmd-r -f
    cp lib/update-cmdr /usr/bin/update-cmdr -f
    chmod 777 /usr/lib/cmd-r
    
    echo CMD-R has been installed.
else
    echo Updating CMD-R...
    cp build/* /usr/lib/cmd-r -r -f
    
    cp lib/cmd-r /usr/bin/cmd-r -f
    cp lib/update-cmdr /usr/bin/update-cmdr -f
    chmod 777 /usr/lib/cmd-r
        
    echo CMD-R has been updated.
fi
