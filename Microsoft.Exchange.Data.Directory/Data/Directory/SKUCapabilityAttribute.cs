using System;

namespace Microsoft.Exchange.Data.Directory
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class SKUCapabilityAttribute : Attribute
	{
		public SKUCapabilityAttribute(string capabilityGuid)
		{
			this.Guid = new Guid(capabilityGuid);
		}

		public Guid Guid { get; private set; }

		public bool AddOnSKU { get; set; }

		public bool Free { get; set; }
	}
}
