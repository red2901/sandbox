//// 
//// Autogenerated by xlw 
//// Do not edit this file, it will be overwritten 
//// by InterfaceGenerator 
////

#include "xlw/MyContainers.h"
#include <xlw/CellMatrix.h>
#include "Commands.h"
#include <xlw/xlw.h>
#include <xlw/XlFunctionRegistration.h>
#include <stdexcept>
#include <xlw/XlOpenClose.h>
#include <xlw/HiResTimer.h>
using namespace xlw;

namespace {
const char* LibraryName = "MyTestLibrary";
};


// registrations start here


namespace
{
  XLRegistration::XLCommandRegistrationHelper
registertestCommand("xltestCommand",
"testCommand",
" Test Menu Command ",
LibraryName,
" Test Menu Command ");
}



extern "C"
{
int EXCEL_EXPORT
xltestCommand()
{
EXCEL_BEGIN;
	testCommand();
EXCEL_END_CMD;
}
}



//////////////////////////

//////////////////////////
// Methods that will get registered to execute in AutoOpen
//////////////////////////

void welcome();
namespace {
	MacroCache<xlw::Open>::MacroRegistra welcome_registra("welcome","welcome",welcome);
}


//////////////////////////
// Methods that will get registered to execute in AutoClose
//////////////////////////

void goodbye();
namespace {
	MacroCache<xlw::Close>::MacroRegistra goodbye_registra("goodbye","goodbye",goodbye);
}


