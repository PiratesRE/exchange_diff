using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class SearchMailboxesCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal SearchMailboxesCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public SearchMailboxesResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SearchMailboxesResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
