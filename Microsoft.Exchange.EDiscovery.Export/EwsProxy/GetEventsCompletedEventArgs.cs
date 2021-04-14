using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
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
