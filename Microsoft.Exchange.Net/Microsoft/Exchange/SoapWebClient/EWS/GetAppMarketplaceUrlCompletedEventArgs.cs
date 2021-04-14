using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class GetAppMarketplaceUrlCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetAppMarketplaceUrlCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetAppMarketplaceUrlResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetAppMarketplaceUrlResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
