//------------------------------------------------------------------------------------------------
// <copyright project="BEmu_cpp" file="src/IntradayBarRequest/IntradayBarMessage.cpp" company="Jordan Robinson">
//     Copyright (c) 2013 Jordan Robinson. All rights reserved.
//
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </copyright>
//------------------------------------------------------------------------------------------------

#include "BloombergTypes/MessagePtr.h"
#include "IntradayBarRequest/IntradayBarMessage.h"
#include "IntradayBarRequest/IntradayBarElementData.h"
#include "IntradayBarRequest/IntradayBarMessage.h"
#include "IntradayBarRequest/IntradayBarElementResponseError.h"
#include "BloombergTypes/Name.h"

namespace BEmu
{
	namespace IntradayBarRequest
	{
		IntradayBarMessage::IntradayBarMessage(const CorrelationId& corr, const Service& service, const std::string& security) : 
			MessagePtr(Name("IntradayBarResponse"), corr),
			_responseError(new IntradayBarElementResponseError(security)),
			_parent()
		{
            this->_isResponseError = true;
		}

		IntradayBarMessage::IntradayBarMessage(const CorrelationId& corr, const Service& service, const std::vector< boost::shared_ptr<IntradayBarTickDataType> >& bars) :
			MessagePtr(Name("IntradayBarResponse"), corr),
			_responseError(),
			_parent(new IntradayBarElementData(bars))
		{
            this->_isResponseError = false;
		}

		IntradayBarMessage::~IntradayBarMessage()
		{
		}

		const char* IntradayBarMessage::topicName() const
		{
			return "";
		}

		size_t IntradayBarMessage::numElements() const
		{
			return 1;
		}

		boost::shared_ptr<ElementPtr> IntradayBarMessage::getElement(const char* name) const
		{
            if (this->_isResponseError)
			{
				if(strncmp(name, "responseError", 14) == 0)
					return boost::dynamic_pointer_cast<ElementPtr>(this->_responseError);
				else
					throw messageEx;
			}
			else
			{
				if(strncmp(name, "barData", 8) == 0)
					return boost::dynamic_pointer_cast<ElementPtr>(this->_parent);
				else
					throw messageEx;
			}
		}

		bool IntradayBarMessage::hasElement(const char* name, bool excludeNullElements) const
		{
			return
				(strncmp(name, "responseError", 14) == 0 && this->_isResponseError) ||
				(strncmp(name, "barData", 8) == 0 && !this->_isResponseError);
		}

		std::ostream& IntradayBarMessage::print(std::ostream& stream, int level, int spacesPerLevel) const
		{
			stream << "IntradayBarResponse (choice) = {" << std::endl;

			if(this->_isResponseError)
				this->_responseError->print(stream, level, spacesPerLevel);
			else
				this->_parent->print(stream, level, spacesPerLevel);

			stream << '}' << std::endl;
			return stream;
		}

	}
}