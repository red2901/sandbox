#
# some pre-requisites
# build the make files to show the running commands
cmake_minimum_required (VERSION 2.6)
set(CMAKE_VERBOSE_MAKEFILE 1)

message("compiler is ${CMAKE_CXX_COMPILER_ID}")
message("building in ${CMAKE_FILES_DIRECTORY}")

enable_testing()
set(MODULE_NAME test_boost)
project(${MODULE_NAME}_py)

# boost setup
set(BOOST_INCLUDEDIR      "c:/dev/src/thirdparty/boost/boost_1_59_0")
set(BOOST_LIBRARYDIR      "c:/dev/src/thirdparty/boost/boost_1_59_0/lib64-msvc-12.0")

find_package(Boost)
if(Boost_FOUND)
  include_directories("${Boost_INCLUDE_DIRS}")
  # this defines the lib... 
  set(Boost_USE_STATIC_LIBS ON)
  set(Boost_USE_MULTITHREADED ON)
  find_package( Boost COMPONENTS date_time REQUIRED )
elseif(NOT Boost_FOUND)
  message(FATAL_ERROR "Unable to find Boost")
endif()

add_executable(${MODULE_NAME} example.cpp)

target_link_libraries(${MODULE_NAME}
  ${Boost_LIBRARIES}
  )

add_test (test1 ${MODULE_NAME})
set_tests_properties (test1
  PROPERTIES 
  PASS_REGULAR_EXPRESSION "2001-Oct-09"
  )
