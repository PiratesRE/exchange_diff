using System;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal static class TaskConstants
	{
		internal const string ComplianceProgram = "ComplianceProgram";

		internal const string DlpPolicy = "DlpPolicy";

		internal const string DlpPolicyCollection = "DlpPolicyCollection";

		internal const string DlpPolicyTemplate = "DlpPolicyTemplate";

		internal const string Get = "Get";

		internal const string Import = "Import";

		internal const string Export = "Export";

		internal const string Install = "Install";

		internal const string Set = "Set";

		internal const string Uninstall = "Uninstall";

		internal const string Remove = "Remove";

		internal const string New = "New";

		internal const string FileDataParameterName = "FileData";

		internal const string ForceParameterName = "Force";

		internal const string TemplateParameterName = "Template";

		internal const string StateParameterName = "State";

		internal const string TemplateDataParameterName = "TemplateData";

		internal const string ParametersParameterName = "Parameters";

		internal const string NameParameterName = "Name";

		internal const string DescriptionParameterName = "Description";

		internal const string TemplateParameterDelimiter = "%%";

		internal const string TemplateParameterFormat = "%%{0}%%";

		internal const string TemplateParameterRegex = "%%(?>\\d|\\w|-|_)+%%";

		internal const string TemplateDlpPolicyNameParameter = "%%DlpPolicyName%%";
	}
}
