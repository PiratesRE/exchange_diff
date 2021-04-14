using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class GetDomainSettingsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetDomainSettingsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetDomainSettingsResponse Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetDomainSettingsResponse)this.results[0];
			}
		}

		private object[] results;
	}
}
