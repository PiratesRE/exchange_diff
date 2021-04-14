using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class SaveUMPinCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal SaveUMPinCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public SaveUMPinResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SaveUMPinResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
