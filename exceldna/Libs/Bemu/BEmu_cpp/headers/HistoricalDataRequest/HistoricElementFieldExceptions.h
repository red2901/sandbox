﻿//------------------------------------------------------------------------------
// <copyright project="BEmu_cpp" file="headers/HistoricalDataRequest/HistoricElementFieldExceptions.h" company="Jordan Robinson">
//     Copyright (c) 2013 Jordan Robinson. All rights reserved.
//
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include "BloombergTypes/ElementPtr.h"

namespace BEmu
{
	namespace HistoricalDataRequest
	{
		class HistoricElementString;
		class HistoricElementErrorInfo;

		class HistoricElementFieldExceptions : public ElementPtr
		{
			private:
				boost::shared_ptr<HistoricElementString> _fieldId;
				boost::shared_ptr<HistoricElementErrorInfo> _errorInfo;

			public:
				HistoricElementFieldExceptions(const std::string& badField);
				~HistoricElementFieldExceptions();

				virtual Name name() const;
				virtual size_t numValues() const;
				virtual size_t numElements() const;
				virtual SchemaElementDefinition elementDefinition() const;
		
				virtual bool isNull() const;
				virtual bool isArray() const;
				virtual bool isComplexType() const;
				
				virtual boost::shared_ptr<ElementPtr> getElement(const char* name) const;

				virtual bool hasElement(const char* name, bool excludeNullElements = false) const;

				virtual int getElementAsInt32(const char* name) const;
				virtual const char* getElementAsString(const char* name) const;

				virtual std::ostream& print(std::ostream& stream, int level = 0, int spacesPerLevel = 4) const;
		};
	}
}