using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class GenerateIncidentReportAction : NotifyActionBase
	{
		public GenerateIncidentReportAction(List<Argument> arguments, string externalName = null) : base(arguments, externalName)
		{
		}

		public override string Name
		{
			get
			{
				return "GenerateIncidentReport";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return GenerateIncidentReportAction.minVersion;
			}
		}

		protected ReadOnlyDictionary<string, KeyValuePair<string, string>> DefaultSubjectBodyTable
		{
			get
			{
				return GenerateIncidentReportAction.defaultSubjectBodyTable;
			}
		}

		private static readonly Version minVersion = new Version("1.00.0002.000");

		private static ReadOnlyDictionary<string, KeyValuePair<string, string>> defaultSubjectBodyTable = new ReadOnlyDictionary<string, KeyValuePair<string, string>>(new Dictionary<string, KeyValuePair<string, string>>
		{
			{
				"en",
				new KeyValuePair<string, string>("toBeDefinedSubject-GenerateIncidentReport", "toBeDefinedBody-GenerateIncidentReport")
			}
		});
	}
}
