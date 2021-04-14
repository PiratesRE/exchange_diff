using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class CreateUMPromptCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal CreateUMPromptCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public CreateUMPromptResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (CreateUMPromptResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
