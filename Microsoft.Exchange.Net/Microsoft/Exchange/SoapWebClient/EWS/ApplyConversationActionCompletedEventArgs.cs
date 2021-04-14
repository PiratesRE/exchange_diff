using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class ApplyConversationActionCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal ApplyConversationActionCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public ApplyConversationActionResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ApplyConversationActionResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
