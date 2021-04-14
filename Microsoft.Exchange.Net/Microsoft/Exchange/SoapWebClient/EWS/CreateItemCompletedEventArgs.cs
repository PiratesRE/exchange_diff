using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class CreateItemCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal CreateItemCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public CreateItemResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (CreateItemResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
