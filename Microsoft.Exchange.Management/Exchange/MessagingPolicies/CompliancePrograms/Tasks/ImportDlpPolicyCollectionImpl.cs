using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class ImportDlpPolicyCollectionImpl : CmdletImplementation
	{
		public ImportDlpPolicyCollectionImpl(ImportDlpPolicyCollection dataObject)
		{
			this.dataObject = dataObject;
		}

		public override void Validate()
		{
			if (this.dataObject.FileData == null)
			{
				this.dataObject.WriteError(new ArgumentException(Strings.ImportDlpPolicyFileDataIsNull), ErrorCategory.InvalidArgument, "FileData");
			}
		}

		public override void ProcessRecord()
		{
			if (!this.dataObject.Force && !base.ShouldContinue(Strings.PromptToOverwriteDlpPoliciesOnImport))
			{
				return;
			}
			ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(Utils.RuleCollectionNameFromRole(), base.DataSession);
			adruleStorageManager.LoadRuleCollection();
			foreach (TransportRuleHandle transportRuleHandle in adruleStorageManager.GetRuleHandles())
			{
				Guid guid;
				if (transportRuleHandle.Rule.TryGetDlpPolicyId(out guid))
				{
					base.DataSession.Delete(transportRuleHandle.AdRule);
				}
			}
			DlpUtils.GetInstalledTenantDlpPolicies(base.DataSession).ToList<ADComplianceProgram>().ForEach(new Action<ADComplianceProgram>(base.DataSession.Delete));
			List<DlpPolicyMetaData> list = DlpUtils.LoadDlpPolicyInstances(this.dataObject.FileData).ToList<DlpPolicyMetaData>();
			foreach (DlpPolicyMetaData dlpPolicy in list)
			{
				IEnumerable<PSObject> enumerable;
				DlpUtils.AddTenantDlpPolicy(base.DataSession, dlpPolicy, Utils.GetOrganizationParameterValue(this.dataObject.Fields), new CmdletRunner(DlpPolicyTemplateMetaData.AllowedCommands, DlpPolicyTemplateMetaData.RequiredParams, null), out enumerable);
			}
		}

		private ImportDlpPolicyCollection dataObject;

		private static readonly Type[] KnownExceptions = new Type[]
		{
			typeof(DirectoryNotFoundException),
			typeof(IOException),
			typeof(DlpPolicyParsingException),
			typeof(DlpPolicyScriptExecutionException)
		};
	}
}
