using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class GetClientExtensionCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetClientExtensionCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public ClientExtensionResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ClientExtensionResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
