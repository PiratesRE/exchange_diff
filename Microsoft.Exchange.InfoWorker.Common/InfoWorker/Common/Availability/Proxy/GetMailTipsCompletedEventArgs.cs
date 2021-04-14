using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	public class GetMailTipsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetMailTipsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetMailTipsResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetMailTipsResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
