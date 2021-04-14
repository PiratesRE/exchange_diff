using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class QueryTenantReadinessCompletedEventArgs : AsyncCompletedEventArgs
	{
		public QueryTenantReadinessCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public TenantReadiness[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (TenantReadiness[])this.results[0];
			}
		}

		private object[] results;
	}
}
