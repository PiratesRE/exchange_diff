using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PlatformSignalingHeader
	{
		public PlatformSignalingHeader(string name, string value)
		{
			ValidateArgument.NotNullOrEmpty(name, "name");
			ValidateArgument.NotNullOrEmpty(value, "value");
			this.Name = name;
			this.Value = value.Trim();
		}

		public string Name { get; private set; }

		public string Value { get; private set; }

		public abstract PlatformSipUri ParseUri();

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.Name, this.Value);
		}
	}
}
