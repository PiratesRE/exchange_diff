using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class GetMessageTrackingReportCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetMessageTrackingReportCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetMessageTrackingReportResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetMessageTrackingReportResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
