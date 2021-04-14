using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	public class DeprecatedAttribute : Attribute
	{
		public ExchangeVersionType Version { get; set; }

		public DeprecatedAttribute(ExchangeVersionType version)
		{
			this.Version = version;
		}
	}
}
