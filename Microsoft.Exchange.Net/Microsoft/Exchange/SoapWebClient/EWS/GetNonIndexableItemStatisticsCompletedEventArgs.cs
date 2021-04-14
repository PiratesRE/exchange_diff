using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class GetNonIndexableItemStatisticsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetNonIndexableItemStatisticsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetNonIndexableItemStatisticsResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetNonIndexableItemStatisticsResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
