using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class GetEventsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetEventsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetEventsResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetEventsResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
