using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobItemIssue : MigrationIssue
	{
		public MigrationJobItemIssue(MigrationJobItem jobItem) : base("JobItemIssue", jobItem.TenantName, jobItem.JobName, jobItem.StatusData.InternalError)
		{
			this.Identity = jobItem.Identifier;
		}

		public string Identity { get; private set; }

		public override string IdentifierString
		{
			get
			{
				return string.Format("{0}-{1}", base.IdentifierString, this.Identity);
			}
		}

		public override XElement GetDiagnosticInfo(SICDiagnosticArgument arguments)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(arguments);
			diagnosticInfo.Add(new XElement("Identity", this.Identity));
			return diagnosticInfo;
		}

		public const string ErrorClass = "JobItemIssue";
	}
}
