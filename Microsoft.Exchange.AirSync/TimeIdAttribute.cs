using System;

namespace Microsoft.Exchange.AirSync
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class TimeIdAttribute : Attribute
	{
		public TimeIdAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; set; }
	}
}
