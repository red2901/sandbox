char const* greet()
{
   return "hello, world";
}

int add(int a, int b)
{
   return a + b;
}

#include <boost/python.hpp>

// the name of the module needs to match the name of the binary
// we can do this is CMAKE
BOOST_PYTHON_MODULE(boost_hello_world)
{
    using namespace boost::python;
    def("greet", greet);
}

