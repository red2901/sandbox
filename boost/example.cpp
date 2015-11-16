#include <iostream>
#include <iterator>
#include <algorithm>

#include <boost/date_time/gregorian/gregorian.hpp>

using namespace boost::gregorian;

void test_boost_from_simple_string()
{
   std::string s("2001-10-9"); //2001-October-09
   date d(from_simple_string(s));
   std::cout << d << std::endl;
}

void test_boost_from_undelimited_string()
{
   //Read ISO Standard(CCYYMMDD) and output ISO Extended
   std::string ud("20011009"); //2001-Oct-09
   date d1(from_undelimited_string(ud));
   std::cout << to_iso_extended_string(d1) << std::endl;
}

void test_boost_date_parts()
{
   std::string ud("20011009"); //2001-Oct-09
   date d1(from_undelimited_string(ud));
   //Output the parts of the date - Tuesday October 9, 2001
   date::ymd_type ymd = d1.year_month_day();
   greg_weekday wd = d1.day_of_week();
   std::cout << wd.as_long_string() << " "
			 << ymd.month.as_long_string() << " "
			 << ymd.day << ", " << ymd.year
			 << std::endl;
}

void test_boost_date_exception()
{
   //Let's send in month 25 by accident and create an exception
   std::string bad_date("20012509"); //2001-??-09
   std::cout << "An expected exception is next: " << std::endl;
   date wont_construct(from_undelimited_string(bad_date));
   //use wont_construct so compiler doesn't complain, but you wont get here!
   std::cout << "oh oh, you shouldn't reach this line: " 
			 << to_iso_string(wont_construct) << std::endl;
}

int main()
{
   test_boost_from_simple_string();
}
