using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Cmdlet("Export", "DlpPolicyCollection", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ExportDlpPolicyCollection : GetMultitenancySystemConfigurationObjectTask<DlpPolicyIdParameter, ADComplianceProgram>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageExportDlpPolicyCollection;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				if (configurationSession == null)
				{
					return null;
				}
				return configurationSession.GetOrgContainerId().GetChildId("Transport Settings").GetChildId("Rules").GetChildId(DlpUtils.TenantDlpPoliciesCollectionName);
			}
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			IList<DlpPolicyMetaData> list = (from dataObject in (IEnumerable<ADComplianceProgram>)dataObjects
			select DlpPolicyParser.ParseDlpPolicyInstance(dataObject.TransportRulesXml)).ToList<DlpPolicyMetaData>();
			foreach (DlpPolicyMetaData dlpPolicyMetaData in list)
			{
				dlpPolicyMetaData.PolicyCommands = DlpUtils.GetEtrsForDlpPolicy(dlpPolicyMetaData.ImmutableId, base.DataSession);
			}
			this.WriteResult(new BinaryFileDataObject
			{
				FileData = DlpPolicyParser.SerializeDlpPolicyInstances(list)
			});
		}
	}
}
