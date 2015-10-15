﻿//------------------------------------------------------------------------------
// <copyright project="BEmu_cpp" file="headers/ReferenceDataRequest/ReferenceElementSecurityData.h" company="Jordan Robinson">
//     Copyright (c) 2013 Jordan Robinson. All rights reserved.
//
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </copyright>
//------------------------------------------------------------------------------

#pragma once

#include "BloombergTypes/ElementPtr.h"
#include "Types/ObjectType.h"
#include <map>

namespace BEmu
{
	class Name;

	namespace ReferenceDataRequest
	{
        class ReferenceElementString;
        class ReferenceElementInt;
        class ReferenceElementFieldData;
        class ReferenceElementFieldExceptionsArray;
        class ReferenceElementSecurityError;

		class ReferenceElementSecurityData : public ElementPtr
		{
			private:
				boost::shared_ptr<ReferenceElementString> _elmSecurityName;
				boost::shared_ptr<ReferenceElementInt> _elmSequenceNumber;
				boost::shared_ptr<ReferenceElementFieldData> _elmFieldData;
				boost::shared_ptr<ReferenceElementFieldExceptionsArray> _elmFieldExceptions;
				boost::shared_ptr<ReferenceElementSecurityError> _elmSecError;

				bool _isNull_elmFieldExceptions;

				bool _isSecurityError;

			public:
				ReferenceElementSecurityData(const std::string& securityName, std::map<std::string, ObjectType> * fieldData, int sequenceNumber);
				~ReferenceElementSecurityData();

				virtual Name name() const;
				virtual size_t numValues() const;
				virtual size_t numElements() const;
				virtual SchemaElementDefinition elementDefinition() const;

				virtual const char* getElementAsString(const char* name) const;
				
				virtual boost::shared_ptr<ElementPtr> getElement(const char* name) const;

				virtual bool hasElement(const char* name, bool excludeNullElements = false) const;

				virtual std::ostream& print(std::ostream& stream, int level = 0, int spacesPerLevel = 4) const;
		};
	}
}