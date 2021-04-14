using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class QueryConstraintCompletedEventArgs : AsyncCompletedEventArgs
	{
		public QueryConstraintCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Constraint[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Constraint[])this.results[0];
			}
		}

		private object[] results;
	}
}
