using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	public class GetCompanyAccountsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal GetCompanyAccountsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Account[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Account[])this.results[0];
			}
		}

		private object[] results;
	}
}
