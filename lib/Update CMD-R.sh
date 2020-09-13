DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
DIROLD=$(pwd)
cd "$DIR"

echo Cloning git repo...
git clone -q https://github.com/Stefan0436/CMD-R.git Update
if [ -d "Update" ]; then
    cd Update
    echo Configuring package and downloading dependencies...
    chmod +x ./configure
    ./configure
    echo Compiling...
    make
    echo Installing...
    cp build/* .. -r -f
    cd ..
    echo Removing repo folder...
    rm -r Update -f
    echo Finished.
else
    echo Cloning failed.
fi

cd "$DIROLD"
