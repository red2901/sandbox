#
# some pre-requisites
cmake_minimum_required( VERSION 2.8 )
set(CMAKE_VERBOSE_MAKEFILE 1)

project( BoostPythonHelloWorld )

# python setup
set(PYTHON_LIBRARY        "c:/Anaconda2/libs/python27.lib")              
set(PYTHON_INCLUDE_DIR    "c:/Anaconda2/include")                        
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
  
add_library(hello MODULE hello.cpp)
set_target_properties(hello PROPERTIES SUFFIX ".pyd")
target_link_libraries(hello ${Boost_LIBRARIES} ${PYTHON_LIBRARIES})

