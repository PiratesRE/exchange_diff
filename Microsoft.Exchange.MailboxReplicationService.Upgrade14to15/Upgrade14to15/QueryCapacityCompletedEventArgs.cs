using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class QueryCapacityCompletedEventArgs : AsyncCompletedEventArgs
	{
		public QueryCapacityCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public GroupCapacity[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (GroupCapacity[])this.results[0];
			}
		}

		private object[] results;
	}
}
