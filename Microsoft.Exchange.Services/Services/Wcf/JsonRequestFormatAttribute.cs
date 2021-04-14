using System;

namespace Microsoft.Exchange.Services.Wcf
{
	[AttributeUsage(AttributeTargets.Method)]
	public class JsonRequestFormatAttribute : Attribute
	{
		public JsonRequestFormat Format { get; set; }

		public JsonRequestFormatAttribute()
		{
			this.Format = JsonRequestFormat.HeaderBodyFormat;
		}
	}
}
