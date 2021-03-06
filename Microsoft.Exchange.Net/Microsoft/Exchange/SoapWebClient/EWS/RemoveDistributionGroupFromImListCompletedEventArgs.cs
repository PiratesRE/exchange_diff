using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class RemoveDistributionGroupFromImListCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal RemoveDistributionGroupFromImListCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public RemoveDistributionGroupFromImListResponseMessageType Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (RemoveDistributionGroupFromImListResponseMessageType)this.results[0];
			}
		}

		private object[] results;
	}
}
