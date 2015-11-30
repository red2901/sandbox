cmake_minimum_required(VERSION 2.8.11)

set (CMAKE_PREFIX_PATH "c:/Qt/Qt5.5.1/5.5/msvc2013/")

project(testproject)

# Find includes in corresponding build directories
set(CMAKE_INCLUDE_CURRENT_DIR ON)
# Instruct CMake to run moc automatically when needed.
set(CMAKE_AUTOMOC ON)

# Find the QtWidgets library
find_package(Qt5Widgets)

# Tell CMake to create the helloworld executable
add_executable(helloworld WIN32 main.cc)

# Use the Widgets module from Qt 5.
target_link_libraries(helloworld Qt5::Widgets)
