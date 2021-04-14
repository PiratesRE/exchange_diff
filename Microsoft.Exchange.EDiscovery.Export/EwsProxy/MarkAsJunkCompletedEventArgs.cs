using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class MarkAsJunkCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal MarkAsJunkCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public MarkAsJunkResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (MarkAsJunkResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
