using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	public class SyncFolderItemsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal SyncFolderItemsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public SyncFolderItemsResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SyncFolderItemsResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
