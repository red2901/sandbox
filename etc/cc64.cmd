rem
rem command line for compiling with visual studio 2014 in 64 bit mode
rem

@echo off
%comspec% /k ""C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\vcvarsall.bat"" amd64
cl.exe $*
