﻿using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IPSCommandWrapperFactory
	{
		IPSCommandWrapper CreatePSCommand();
	}
}
