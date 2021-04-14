using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class GetImItemListCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetImItemListCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetImItemListResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetImItemListResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
