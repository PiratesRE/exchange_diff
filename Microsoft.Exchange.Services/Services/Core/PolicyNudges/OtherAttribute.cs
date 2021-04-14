using System;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	public class OtherAttribute
	{
		public OtherAttribute(string name, string val)
		{
			this.Name = name;
			this.Value = val;
		}

		public string Name { get; private set; }

		public string Value { get; private set; }
	}
}
