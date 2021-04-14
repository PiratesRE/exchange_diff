﻿using System;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Net.XropService
{
	internal interface IServerSessionProvider
	{
		IServerSession Create(TokenValidationResults tokenValidationResults);
	}
}
