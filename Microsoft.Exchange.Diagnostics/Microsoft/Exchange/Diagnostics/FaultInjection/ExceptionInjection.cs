﻿using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.FaultInjection
{
	internal class ExceptionInjection : Dictionary<Guid, ExceptionInjectionCallback>
	{
	}
}
