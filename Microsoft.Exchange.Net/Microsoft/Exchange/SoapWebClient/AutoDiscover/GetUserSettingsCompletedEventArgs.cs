using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	public class GetUserSettingsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetUserSettingsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GetUserSettingsResponse Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GetUserSettingsResponse)this.results[0];
			}
		}

		private object[] results;
	}
}
