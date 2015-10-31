char const* greet()
{
   return "hello, world";
}

int add(int a, int b)
{
   return a + b;
}

#include <boost/python.hpp>

BOOST_PYTHON_MODULE(hello)
{
    using namespace boost::python;
    def("greet", greet);
}

