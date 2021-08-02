#!/bin/bash
echo Compiling CMD-R...
cd CMD-R
dotnet build -verbosity:quiet
cd ..
pwd
echo
echo Copying start script...
cp lib/Start\ CMD-R CMD-R/bin/Debug/net5.0/cmd-r -T -f
echo Copying update scripts and commands...
cp lib/Update\ and\ start\ CMD-R.sh CMD-R/bin/Debug/net5.0/update-and-run -f -T
cp lib/Update\ CMD-R.sh CMD-R/bin/Debug/net5.0/update -f -T
echo Copying compiled files to build folder...
rm -rf build
mkdir build
cp -rf CMD-R/bin/Debug/net5.0/. ./build
