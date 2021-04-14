using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal class DsnParameters : Dictionary<string, object>
	{
		public DsnParameters() : base(StringComparer.OrdinalIgnoreCase)
		{
		}
	}
}
