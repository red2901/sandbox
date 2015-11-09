This page finally gave me an example of something which worked.

http://www.recheliu.org/memo/pythonboostcmakemucheasierthabboostbuild

There is still a dll dependency on boost_python-vc120-mt-gd-1_59.dll,
which is no copied to the directory when compiled to I needed to do
that myself.

This is a complete full round trip example. There are a couple of
missing items.

1. clarify debug and release target naming and isolate the relevant
   libraries, at the moment I just copy over all libs
2. change the behaviour to be able to do build clean test where clean
   will clean not only the files generated, but also the copied files
  

TODO

- why is b2 not building the boost_python dll's and only the libs?
- how do you get CMake to copy over the relevant libary into a test
  folder and then run a test?


