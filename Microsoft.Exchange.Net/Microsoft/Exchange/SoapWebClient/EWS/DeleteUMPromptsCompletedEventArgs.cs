using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class DeleteUMPromptsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DeleteUMPromptsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public DeleteUMPromptsResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (DeleteUMPromptsResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
