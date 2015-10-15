﻿//------------------------------------------------------------------------------
// <copyright project="BEmu_cpp" file="headers/HistoricalDataRequest/HistoricElementErrorInfo.h" company="Jordan Robinson">
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
		class HistoricElementInt;
		class HistoricElementString;

		class HistoricElementErrorInfo : public ElementPtr
		{
			private:
				boost::shared_ptr<HistoricElementString> _source, _category, _message, _subCategory;
				boost::shared_ptr<HistoricElementInt> _code;

			public:
				HistoricElementErrorInfo();
				~HistoricElementErrorInfo();

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