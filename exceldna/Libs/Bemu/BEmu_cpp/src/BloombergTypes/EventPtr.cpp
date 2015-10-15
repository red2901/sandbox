//------------------------------------------------------------------------------------------------
// <copyright project="BEmu_cpp" file="src/BloombergTypes/EventPtr.cpp" company="Jordan Robinson">
//     Copyright (c) 2013 Jordan Robinson. All rights reserved.
//
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </copyright>
//------------------------------------------------------------------------------------------------

#include "BloombergTypes/Event.h"
#include "BloombergTypes/EventPtr.h"
#include "BloombergTypes/RequestPtr.h"
#include <exception>
#include <vector>

#include "IntradayTickRequest/IntradayTickRequest.h"
#include "IntradayTickRequest/IntradayTickEvent.h"

#include "IntradayBarRequest/IntradayBarRequest.h"
#include "IntradayBarRequest/IntradayBarEvent.h"

#include "HistoricalDataRequest/HistoricRequest.h"
#include "HistoricalDataRequest/HistoricEvent.h"

#include "ReferenceDataRequest/ReferenceRequest.h"
#include "ReferenceDataRequest/ReferenceEvent.h"

namespace BEmu
{
	EventPtr::EventPtr(boost::shared_ptr<RequestPtr> request) : 
		_requestP(request)
	{
	}

	EventPtr::EventPtr(const EventPtr &src) : 
		_requestP(src._requestP), 
		_type(src._type)
	{
	}

	EventPtr::~EventPtr()
	{

	}

	std::vector< boost::shared_ptr<MessagePtr> > EventPtr::getMessages() const
	{
		throw eventPtrEx;
	}

	Event::EventType EventPtr::getEventType() const
	{
		return this->_type;
	}

	void EventPtr::setEventType(Event::EventType evtType)
	{
		this->_type = evtType;
	}

	boost::shared_ptr<EventPtr> EventPtr::EventFactory(boost::shared_ptr<RequestPtr> request, bool isLastRequest)
	{
		if(request->getRequestType() == RequestPtr::historic)
		{
			boost::shared_ptr<HistoricalDataRequest::HistoricRequest> reqP = boost::dynamic_pointer_cast<HistoricalDataRequest::HistoricRequest>(request);
			boost::shared_ptr<HistoricalDataRequest::HistoricEvent> evtP(new HistoricalDataRequest::HistoricEvent(reqP));

			return evtP;
		}
		else if(request->getRequestType() == RequestPtr::intradayBar)
		{
			boost::shared_ptr<IntradayBarRequest::IntradayBarRequest> reqP = boost::dynamic_pointer_cast<IntradayBarRequest::IntradayBarRequest>(request);
			boost::shared_ptr<IntradayBarRequest::IntradayBarEvent> evtP(new IntradayBarRequest::IntradayBarEvent(reqP));
			
			return evtP;
		}
		else if(request->getRequestType() == RequestPtr::intradayTick)
		{
			boost::shared_ptr<IntradayTickRequest::IntradayTickRequest> reqP = boost::dynamic_pointer_cast<IntradayTickRequest::IntradayTickRequest>(request);
			boost::shared_ptr<IntradayTickRequest::IntradayTickEvent> evtP(new IntradayTickRequest::IntradayTickEvent(reqP));

			return evtP;
		}
		else if(request->getRequestType() == RequestPtr::reference)
		{
			boost::shared_ptr<ReferenceDataRequest::ReferenceRequest> reqP = boost::dynamic_pointer_cast<ReferenceDataRequest::ReferenceRequest>(request);
			boost::shared_ptr<ReferenceDataRequest::ReferenceEvent> evtP(new ReferenceDataRequest::ReferenceEvent(reqP));

			return evtP;
		}
		throw request->requestEx;
	}

}