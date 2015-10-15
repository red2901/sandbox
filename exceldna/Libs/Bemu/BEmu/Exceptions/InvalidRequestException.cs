﻿//------------------------------------------------------------------------------
// <copyright project="BEmu_csh" file="Exceptions/InvalidRequestException.cs" company="Jordan Robinson">
//     Copyright (c) 2013 Jordan Robinson. All rights reserved.
//
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bloomberglp.Blpapi
{
    public class InvalidRequestException : ApplicationException
    {
        public InvalidRequestException() : base()
        {
        }

        public InvalidRequestException(string description) : base(description)
        {
        }

        public InvalidRequestException(string description, Exception cause) : base(description, cause)
        {
        }

        public string Description { get { return base.Message; } }
    }
}
