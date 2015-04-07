cmake_minimum_required( VERSION 2.8 )
 
if(NOT CMAKE_BUILD_TYPE)
  # SET(CMAKE_BUILD_TYPE "DEBUG")
  set(CMAKE_BUILD_TYPE "RELEASE")
endif()

project( BoostPythonHelloWorld )

# we can test whether we are in linux or windows.
# if windows we need to set the relevant library directories
if (WIN32)
  set(PYTHON_LIBRARY        "c:/Python27/libs/python27.lib")
  set(PYTHON_INCLUDE_DIR    "c:/Python27/include")
  set(BOOST_ROOT            "c:/dev/thirdparty/boost/boost_1_57_0")
  set(BOOST_INCLUDEDIR      "c:/dev/thirdparty/boost/boost_1_57_0/include")
  set(BOOST_LIBRARYDIR      "c:/dev/thirdparty/boost/boost_1_57_0/stage/lib")
endif (WIN32)

# Find necessary packages
find_package( PythonLibs 2.7 REQUIRED )
include_directories( ${PYTHON_INCLUDE_DIRS} )
 
find_package( Boost COMPONENTS python REQUIRED )
include_directories( ${Boost_INCLUDE_DIR} )
 
if (UNIX)
  # Build our library
  add_library( greet SHARED greet.cpp )
   
  # Define the wrapper library that wraps our library
  add_library( greet_ext SHARED greet_ext.cpp )
  target_link_libraries( greet_ext ${Boost_LIBRARIES} greet )
  # don't prepend wrapper library name with lib
  set_target_properties( greet_ext PROPERTIES PREFIX "" )
endif (UNIX)

if (WIN32)
  add_library(greet_ext MODULE greet_py.cpp)

  set_target_properties(greet_ext PROPERTIES SUFFIX ".pyd")
    
  target_link_libraries(greet_ext ${Boost_LIBRARIES} ${PYTHON_LIBRARIES})
endif (WIN32)