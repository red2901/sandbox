set MINGW_PATH=c:\MinGW
set PATH=%PATH%;%MINGW_PATH%\bin
set PATH=%PATH%;%MINGW_PATH%\dll

mkdir build
pushd build
cmake -G"MinGW Makefiles" ..
mingw32-make %*
popd
