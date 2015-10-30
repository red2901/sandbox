
cmake_minimum_required (VERSION 2.6)

# build the make files to show the running commands
set(CMAKE_VERBOSE_MAKEFILE 1)

project (test_boost)

add_executable(test_boost example.cpp)

include_directories ("c:/dev/src/thirdparty/boost_1_59_0")
