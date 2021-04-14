using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	public class GetClientAccessTokenCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetClientAccessTokenCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetClientAccessTokenResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetClientAccessTokenResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
