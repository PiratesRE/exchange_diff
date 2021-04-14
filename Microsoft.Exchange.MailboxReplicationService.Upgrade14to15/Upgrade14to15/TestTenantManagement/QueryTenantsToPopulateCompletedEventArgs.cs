using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	public class QueryTenantsToPopulateCompletedEventArgs : AsyncCompletedEventArgs
	{
		public QueryTenantsToPopulateCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Tenant[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Tenant[])this.results[0];
			}
		}

		private object[] results;
	}
}
