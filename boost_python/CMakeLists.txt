#
# some pre-requisites
enable_testing()
set(MODULE_NAME boost_hello_world)
project( ${MODEUL_NAME}_py )

cmake_minimum_required( VERSION 2.8 )
set(CMAKE_VERBOSE_MAKEFILE 1)
message(STATUS "This is the home ${CMAKE_HOME_DIRECTORY}")
set(CMAKE_INSTALL_PREFIX "${CMAKE_HOME_DIRECTORY}")
set(CMAKE_BUILD_TYPE "Release")
message(STATUS "${CMAKE_BUILD_TYPE}")

# python setup
set(PYTHON_ROOT           "c:/Anaconda2")
set(PYTHON_EXECUTABLE     "${PYTHON_ROOT}/python.exe")              
set(PYTHON_LIBRARY        "${PYTHON_ROOT}/libs/python27.lib")              
set(PYTHON_INCLUDE_DIR    "${PYTHON_ROOT}/include")                        
set(BOOST_INCLUDEDIR      "c:/local/boost_1_59_0")
set(BOOST_LIBRARYDIR      "c:/local/boost_1_59_0/lib64-msvc-12.0")

# Find necessary packages
find_package( PythonLibs 2.7 REQUIRED )
if(PYTHONLIBS_FOUND)  
  include_directories( ${PYTHON_INCLUDE_DIRS} )
else()
  message(FATAL_ERROR "Unable to find PythonLibs")
endif()

find_package(Boost)
if(Boost_FOUND)
  include_directories("${Boost_INCLUDE_DIRS}")
  set(Boost_USE_STATIC_LIBS OFF)
  set(Boost_USE_MULTITHREADED ON)
  set(Boost_USE_STATIC_RUNTIME OFF)
  find_package( Boost COMPONENTS python REQUIRED )
elseif(NOT Boost_FOUND)
  message(FATAL_ERROR "Unable to find Boost")
endif()
  
message(STATUS "Boost  ${BOOST_PYTHON}")
message(STATUS "Python ${PYTHON_LIBRARIES}")
message(STATUS "Python ${PYTHON_EXECUTABLE}")

add_library(${MODULE_NAME} MODULE
  hello.cpp
  )

set_target_properties(${MODULE_NAME} PROPERTIES SUFFIX ".pyd")

target_link_libraries(${MODULE_NAME}
  ${Boost_LIBRARIES}
  ${PYTHON_LIBRARIES}
  )

install (TARGETS ${MODULE_NAME} DESTINATION lib)
install (DIRECTORY "${BOOST_LIBRARYDIR}/" DESTINATION lib FILES_MATCHING PATTERN "*python*.dll")
install (FILES "${CMAKE_HOME_DIRECTORY}/hello.py" DESTINATION lib)

add_test (NAME test1 WORKING_DIRECTORY "${CMAKE_HOME_DIRECTORY}/lib"
  COMMAND ${PYTHON_EXECUTABLE} ${CMAKE_HOME_DIRECTORY}/lib/hello.py)
set_tests_properties (test1
  PROPERTIES 
  PASS_REGULAR_EXPRESSION ".*hello, world.*"
  )
