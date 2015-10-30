This page finally gave me an example of something which worked.

http://www.recheliu.org/memo/pythonboostcmakemucheasierthabboostbuild

There is still a dll dependency on boost_python-vc120-mt-gd-1_59.dll,
which is no copied to the directory when compiled to I needed to do
that myself.

TODO

- why is b2 not building the boost_python dll's and only the libs?
- how do you get CMake to copy over the relevant libary into a test
  folder and then run a test?


