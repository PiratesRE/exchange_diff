using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlatformSipUriParameter
	{
		public PlatformSipUriParameter(string name, string value)
		{
			ValidateArgument.NotNullOrEmpty(name, "name");
			ValidateArgument.NotNullOrEmpty(value, "value");
			this.Name = name;
			this.Value = value;
		}

		public string Name { get; private set; }

		public string Value { get; private set; }
	}
}
