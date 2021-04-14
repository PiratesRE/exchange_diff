using System;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobIssue : MigrationIssue
	{
		public MigrationJobIssue(MigrationJob job) : base("JobIssue", job.TenantName, job.JobName, job.StatusData.InternalError)
		{
		}

		public const string ErrorClass = "JobIssue";
	}
}
