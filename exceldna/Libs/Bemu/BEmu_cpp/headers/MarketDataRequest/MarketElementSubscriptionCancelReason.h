﻿//------------------------------------------------------------------------------
// <copyright project="BEmu_cpp" file="headers/MarketDataRequest/MarketElementSubscriptionCancelReason.h" company="Jordan Robinson">
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
	namespace MarketDataRequest
	{
		class MarketElementString;
		class MarketElementInt;

		class MarketElementSubscriptionCancelReason : public ElementPtr
		{
			private:
				boost::shared_ptr<MarketElementString> _source, _category, _description, _subCategory;
				boost::shared_ptr<MarketElementInt> _errorCode;

			public:
				MarketElementSubscriptionCancelReason();
				~MarketElementSubscriptionCancelReason();

				virtual Name name() const;
				virtual size_t numValues() const;
				virtual size_t numElements() const;
		
				virtual bool isNull() const;
				virtual bool isArray() const;
				virtual bool isComplexType() const;

				virtual std::ostream& print(std::ostream& stream, int level = 0, int spacesPerLevel = 4) const;
		};
	}
}