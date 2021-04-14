using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSPoisonedJobIssue : ServiceIssue
	{
		public MRSPoisonedJobIssue(MoveJob job) : base("PoisonedJob")
		{
			this.MoveJob = job;
		}

		public MoveJob MoveJob { get; private set; }

		public override string IdentifierString
		{
			get
			{
				return string.Format("{0}-{1}", "PoisonedJob", this.MoveJob.RequestGuid);
			}
		}

		public override XElement GetDiagnosticInfo(SICDiagnosticArgument arguments)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(arguments);
			diagnosticInfo.Add(new object[]
			{
				new XElement("ExchangeGuid", this.MoveJob.ExchangeGuid),
				new XElement("RequestGuid", this.MoveJob.RequestGuid),
				new XElement("TargetDatabaseGuid", this.MoveJob.TargetDatabaseGuid),
				new XElement("RequestType", this.MoveJob.RequestType),
				new XElement("PoisonCount", this.MoveJob.PoisonCount),
				new XElement("FailureType", this.MoveJob.FailureType),
				new XElement("Status", this.MoveJob.Status)
			});
			return diagnosticInfo;
		}

		private const string ErrorClass = "PoisonedJob";
	}
}
