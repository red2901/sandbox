mkdir build
pushd build
cmake -G"NMake Makefiles" ..
nmake %*
popd
