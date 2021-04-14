using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	public class QueryGroupCompletedEventArgs : AsyncCompletedEventArgs
	{
		public QueryGroupCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Group[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Group[])this.results[0];
			}
		}

		private object[] results;
	}
}
