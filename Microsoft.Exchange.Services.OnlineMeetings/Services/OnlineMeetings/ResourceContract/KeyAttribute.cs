using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class KeyAttribute : Attribute
	{
		public KeyAttribute(string key)
		{
			this.key = key;
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		private readonly string key;
	}
}
