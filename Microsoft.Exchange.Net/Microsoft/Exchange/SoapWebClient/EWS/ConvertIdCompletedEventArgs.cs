using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class ConvertIdCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal ConvertIdCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public ConvertIdResponseType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ConvertIdResponseType)this.results[0];
			}
		}

		private object[] results;
	}
}
