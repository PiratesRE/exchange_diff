using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.FaultInjection
{
	internal class FaultInjectionPointConfig
	{
		internal FaultInjectionPointConfig(FaultInjectionType type, List<string> parameters)
		{
			this.type = type;
			this.parameters = parameters;
		}

		public FaultInjectionType Type
		{
			get
			{
				return this.type;
			}
		}

		public List<string> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		private readonly FaultInjectionType type;

		private List<string> parameters;
	}
}
