# CMake generated Testfile for 
# Source directory: C:/dev/src/sandbox/boost_python
# Build directory: C:/dev/src/sandbox/boost_python/build
# 
# This file includes the relevant testing commands required for 
# testing this directory and lists subdirectories to be tested as well.
add_test(test1 "c:/Anaconda2/python.exe" "C:/dev/src/sandbox/boost_python/lib/hello.py")
set_tests_properties(test1 PROPERTIES  PASS_REGULAR_EXPRESSION ".*hello, world.*" WORKING_DIRECTORY "C:/dev/src/sandbox/boost_python/lib")
