using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DiagnosableServiceIssue : ServiceIssue
	{
		public DiagnosableServiceIssue(IDiagnosableObject diagnosableObject, string error) : base(error)
		{
			this.DiagnosableObject = diagnosableObject;
		}

		private IDiagnosableObject DiagnosableObject { get; set; }

		public override string IdentifierString
		{
			get
			{
				return string.Format("{0} : {1}", this.DiagnosableObject.GetType().ToString(), this.DiagnosableObject.HashableIdentity);
			}
		}

		public override void DeriveFromIssue(ServiceIssue issue)
		{
			base.DeriveFromIssue(issue);
			DiagnosableServiceIssue diagnosableServiceIssue = issue as DiagnosableServiceIssue;
			this.DiagnosableObject = diagnosableServiceIssue.DiagnosableObject;
		}

		public override XElement GetDiagnosticInfo(SICDiagnosticArgument arguments)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(arguments);
			diagnosticInfo.Add(this.DiagnosableObject.GetDiagnosticInfo(null));
			return diagnosticInfo;
		}
	}
}
