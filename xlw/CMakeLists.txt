#
# some pre-requisites
enable_testing()
set(MODULE_NAME examplexll)
project( ${MODULE_NAME} )

cmake_minimum_required( VERSION 2.8 )
set(CMAKE_VERBOSE_MAKEFILE 1)
message(STATUS "This is the home ${CMAKE_HOME_DIRECTORY}")
set(CMAKE_INSTALL_PREFIX "${CMAKE_HOME_DIRECTORY}")
message(STATUS "${CMAKE_BUILD_TYPE}")

add_library(${MODULE_NAME} SHARED
  xlwAsynchExample.cpp
  xlwCommands.cpp
  xlwExample.cpp
  xlwMatrixTest.cpp
  xlwXlmMacros.cpp
  )

set_target_properties(${MODULE_NAME} PROPERTIES SUFFIX ".xll")

include_directories( "c:/XLW/xlw-5.0.2f0/xlw/include" )
target_link_libraries(${MODULE_NAME} debug "c:/XLW/xlw-5.0.2f0/xlw/lib/xlw-vc120-mt-gd-5_0_2f0.lib" optimized "c:/XLW/xlw-5.0.2f0/xlw/lib/xlw-vc120-mt-5_0_2f0.lib" )

