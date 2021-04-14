using System;

namespace Microsoft.Exchange.Management.Tasks
{
	public class ManageOldMailSubmissionService : ManageMailSubmissionService
	{
		internal string ServiceName
		{
			get
			{
				return this.Name;
			}
		}

		protected override string Name
		{
			get
			{
				return "MSExchMailSubmissionSvc";
			}
		}
	}
}
