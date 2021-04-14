using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class SubscribeCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal SubscribeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public SubscribeResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SubscribeResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
