# CMAKE generated file: DO NOT EDIT!
# Generated by "NMake Makefiles" Generator, CMake Version 3.1

#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:

.SUFFIXES: .hpux_make_needs_suffix_list

# Produce verbose output by default.
VERBOSE = 1

# Suppress display of executed commands.
$(VERBOSE).SILENT:

# A target that is always out of date.
cmake_force:
.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE
NULL=nul
!ENDIF
SHELL = cmd.exe

# The CMake executable.
CMAKE_COMMAND = c:\CMake\bin\cmake.exe

# The command to remove a file.
RM = c:\CMake\bin\cmake.exe -E remove -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = C:\dev\src\sandbox\boost_python

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = C:\dev\src\sandbox\boost_python\build

# Include any dependencies generated for this target.
include CMakeFiles\boost_hello_world.dir\depend.make

# Include the progress variables for this target.
include CMakeFiles\boost_hello_world.dir\progress.make

# Include the compile flags for this target's objects.
include CMakeFiles\boost_hello_world.dir\flags.make

CMakeFiles\boost_hello_world.dir\hello.cpp.obj: CMakeFiles\boost_hello_world.dir\flags.make
CMakeFiles\boost_hello_world.dir\hello.cpp.obj: ..\hello.cpp
	$(CMAKE_COMMAND) -E cmake_progress_report C:\dev\src\sandbox\boost_python\build\CMakeFiles $(CMAKE_PROGRESS_1)
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Building CXX object CMakeFiles/boost_hello_world.dir/hello.cpp.obj"
	C:\PROGRA~2\MICROS~1.0\VC\bin\amd64\cl.exe  @<<
 /nologo /TP $(CXX_FLAGS) $(CXX_DEFINES) /FoCMakeFiles\boost_hello_world.dir\hello.cpp.obj /FdCMakeFiles\boost_hello_world.dir\ /FS -c C:\dev\src\sandbox\boost_python\hello.cpp
<<

CMakeFiles\boost_hello_world.dir\hello.cpp.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing CXX source to CMakeFiles/boost_hello_world.dir/hello.cpp.i"
	C:\PROGRA~2\MICROS~1.0\VC\bin\amd64\cl.exe  > CMakeFiles\boost_hello_world.dir\hello.cpp.i @<<
 /nologo /TP $(CXX_FLAGS) $(CXX_DEFINES) -E C:\dev\src\sandbox\boost_python\hello.cpp
<<

CMakeFiles\boost_hello_world.dir\hello.cpp.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling CXX source to assembly CMakeFiles/boost_hello_world.dir/hello.cpp.s"
	C:\PROGRA~2\MICROS~1.0\VC\bin\amd64\cl.exe  @<<
 /nologo /TP $(CXX_FLAGS) $(CXX_DEFINES) /FoNUL /FAs /FaCMakeFiles\boost_hello_world.dir\hello.cpp.s /c C:\dev\src\sandbox\boost_python\hello.cpp
<<

CMakeFiles\boost_hello_world.dir\hello.cpp.obj.requires:
.PHONY : CMakeFiles\boost_hello_world.dir\hello.cpp.obj.requires

CMakeFiles\boost_hello_world.dir\hello.cpp.obj.provides: CMakeFiles\boost_hello_world.dir\hello.cpp.obj.requires
	$(MAKE) -f CMakeFiles\boost_hello_world.dir\build.make /nologo -$(MAKEFLAGS) CMakeFiles\boost_hello_world.dir\hello.cpp.obj.provides.build
.PHONY : CMakeFiles\boost_hello_world.dir\hello.cpp.obj.provides

CMakeFiles\boost_hello_world.dir\hello.cpp.obj.provides.build: CMakeFiles\boost_hello_world.dir\hello.cpp.obj

# Object files for target boost_hello_world
boost_hello_world_OBJECTS = \
"CMakeFiles\boost_hello_world.dir\hello.cpp.obj"

# External object files for target boost_hello_world
boost_hello_world_EXTERNAL_OBJECTS =

boost_hello_world.pyd: CMakeFiles\boost_hello_world.dir\hello.cpp.obj
boost_hello_world.pyd: CMakeFiles\boost_hello_world.dir\build.make
boost_hello_world.pyd: c:\local\boost_1_59_0\lib64-msvc-12.0\boost_python-vc120-mt-1_59.lib
boost_hello_world.pyd: c:\Anaconda2\libs\python27.lib
boost_hello_world.pyd: CMakeFiles\boost_hello_world.dir\objects1.rsp
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --red --bold "Linking CXX shared module boost_hello_world.pyd"
	c:\CMake\bin\cmake.exe -E vs_link_dll C:\PROGRA~2\MICROS~1.0\VC\bin\amd64\link.exe /nologo @CMakeFiles\boost_hello_world.dir\objects1.rsp @<<
 /out:boost_hello_world.pyd /implib:boost_hello_world.lib /pdb:C:\dev\src\sandbox\boost_python\build\boost_hello_world.pdb /dll /version:0.0  /machine:x64 /INCREMENTAL:NO c:\local\boost_1_59_0\lib64-msvc-12.0\boost_python-vc120-mt-1_59.lib c:\Anaconda2\libs\python27.lib kernel32.lib user32.lib gdi32.lib winspool.lib shell32.lib ole32.lib oleaut32.lib uuid.lib comdlg32.lib advapi32.lib  
<<

# Rule to build all files generated by this target.
CMakeFiles\boost_hello_world.dir\build: boost_hello_world.pyd
.PHONY : CMakeFiles\boost_hello_world.dir\build

CMakeFiles\boost_hello_world.dir\requires: CMakeFiles\boost_hello_world.dir\hello.cpp.obj.requires
.PHONY : CMakeFiles\boost_hello_world.dir\requires

CMakeFiles\boost_hello_world.dir\clean:
	$(CMAKE_COMMAND) -P CMakeFiles\boost_hello_world.dir\cmake_clean.cmake
.PHONY : CMakeFiles\boost_hello_world.dir\clean

CMakeFiles\boost_hello_world.dir\depend:
	$(CMAKE_COMMAND) -E cmake_depends "NMake Makefiles" C:\dev\src\sandbox\boost_python C:\dev\src\sandbox\boost_python C:\dev\src\sandbox\boost_python\build C:\dev\src\sandbox\boost_python\build C:\dev\src\sandbox\boost_python\build\CMakeFiles\boost_hello_world.dir\DependInfo.cmake --color=$(COLOR)
.PHONY : CMakeFiles\boost_hello_world.dir\depend

