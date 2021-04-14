using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class DeleteItemCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DeleteItemCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public DeleteItemResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (DeleteItemResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
