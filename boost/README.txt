either

To build
cd build
cmake -G"NMake Makefiles" ..
nmake

or run the cm.cmd file which should exist in one of you paths ... something like the below

--

@echo off
rem
rem script which will create the relevant build files
rem
rem todo :
rem

rem check for the build directory
if exist build\NUL goto runcmake
mkdir build

:runcmake
pushd build
rem check that the cmake files have been generated already
if exist CMakeFiles\NULL goto runnmake
c:\CMake\bin\cmake.exe -G"NMake Makefiles" ..

:runnmake
nmake %*
popd

