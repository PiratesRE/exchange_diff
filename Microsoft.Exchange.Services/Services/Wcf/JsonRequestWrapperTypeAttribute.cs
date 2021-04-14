using System;

namespace Microsoft.Exchange.Services.Wcf
{
	[AttributeUsage(AttributeTargets.Method)]
	public class JsonRequestWrapperTypeAttribute : Attribute
	{
		public Type Type { get; set; }

		public JsonRequestWrapperTypeAttribute(Type type)
		{
			this.Type = type;
		}
	}
}
