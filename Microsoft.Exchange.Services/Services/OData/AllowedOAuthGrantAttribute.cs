using System;

namespace Microsoft.Exchange.Services.OData
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal sealed class AllowedOAuthGrantAttribute : Attribute
	{
		public AllowedOAuthGrantAttribute(string grant)
		{
			this.Grant = grant;
		}

		public string Grant { get; private set; }
	}
}
