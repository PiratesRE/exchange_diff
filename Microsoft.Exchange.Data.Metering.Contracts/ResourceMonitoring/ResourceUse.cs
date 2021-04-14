using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class ResourceUse
	{
		public ResourceUse(ResourceIdentifier resource, UseLevel currentUseLevel, UseLevel previousUseLevel)
		{
			this.Resource = resource;
			this.CurrentUseLevel = currentUseLevel;
			this.PreviousUseLevel = previousUseLevel;
		}

		internal ResourceIdentifier Resource { get; private set; }

		internal UseLevel CurrentUseLevel { get; private set; }

		internal UseLevel PreviousUseLevel { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[ResourceUse: Resource={0} CurrentUseLevel={1} PreviousUseLevel={2}]", new object[]
			{
				this.Resource,
				this.CurrentUseLevel,
				this.PreviousUseLevel
			});
		}
	}
}
