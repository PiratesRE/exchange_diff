using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal abstract class MigrationIssue : ServiceIssue
	{
		public MigrationIssue(string errorClass, string organization, string jobName, string error) : base(errorClass)
		{
			this.Organization = organization;
			this.JobName = jobName;
			this.MigrationError = error;
		}

		public string Organization { get; private set; }

		public string JobName { get; private set; }

		public string MigrationError { get; private set; }

		public override string IdentifierString
		{
			get
			{
				return string.Format("{0}-{1}-{2}", base.Error, this.Organization, this.JobName);
			}
		}

		public override XElement GetDiagnosticInfo(SICDiagnosticArgument arguments)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(arguments);
			diagnosticInfo.Add(new object[]
			{
				new XElement("Organization", this.Organization),
				new XElement("JobName", this.JobName),
				new XElement("MigrationError", this.MigrationError)
			});
			return diagnosticInfo;
		}
	}
}
