using System;

namespace Microsoft.Exchange.Data
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class RightGuidAttribute : Attribute
	{
		public RightGuidAttribute(string guid)
		{
			this.Guid = new Guid(guid);
		}

		public Guid Guid { get; private set; }
	}
}
