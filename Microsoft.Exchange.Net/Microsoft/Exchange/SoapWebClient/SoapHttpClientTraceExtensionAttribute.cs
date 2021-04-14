using System;
using System.Web.Services.Protocols;

namespace Microsoft.Exchange.SoapWebClient
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class SoapHttpClientTraceExtensionAttribute : SoapExtensionAttribute
	{
		public override Type ExtensionType
		{
			get
			{
				return typeof(SoapHttpClientTraceExtension);
			}
		}

		public override int Priority { get; set; }
	}
}
