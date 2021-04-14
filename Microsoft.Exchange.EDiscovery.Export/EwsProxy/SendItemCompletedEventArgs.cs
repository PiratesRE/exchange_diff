using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class SendItemCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal SendItemCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public SendItemResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SendItemResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
