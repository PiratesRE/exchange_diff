using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	public class GetPilotUsersCompletedEventArgs : AsyncCompletedEventArgs
	{
		public GetPilotUsersCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public UserWorkloadStatusInfo[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (UserWorkloadStatusInfo[])this.results[0];
			}
		}

		private object[] results;
	}
}
