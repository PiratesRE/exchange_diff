using System;

namespace Microsoft.Exchange.Services.Wcf
{
	[AttributeUsage(AttributeTargets.Method)]
	public class JsonResponseOptionsAttribute : Attribute
	{
		public bool IsCacheable { get; set; }

		public JsonResponseOptionsAttribute()
		{
			this.IsCacheable = false;
		}
	}
}
