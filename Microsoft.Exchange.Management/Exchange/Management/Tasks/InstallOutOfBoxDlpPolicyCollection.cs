using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "OutOfBoxDlpPolicyCollection", SupportsShouldProcess = true)]
	public sealed class InstallOutOfBoxDlpPolicyCollection : NewFixedNameSystemConfigurationObjectTask<ADComplianceProgramCollection>
	{
		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 49, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallOutOfBoxDlpPolicyCollection.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADComplianceProgramCollection adcomplianceProgramCollection = (ADComplianceProgramCollection)base.PrepareDataObject();
			adcomplianceProgramCollection.Name = DlpUtils.OutOfBoxDlpPoliciesCollectionName;
			ADObjectId adobjectId = base.RootOrgContainerId;
			adobjectId = adobjectId.GetChildId("Transport Settings");
			adobjectId = adobjectId.GetChildId("Rules");
			adobjectId = adobjectId.GetChildId(adcomplianceProgramCollection.Name);
			adcomplianceProgramCollection.SetId(adobjectId);
			return adcomplianceProgramCollection;
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				if (base.DataSession.Read<ADComplianceProgramCollection>(this.DataObject.Id) == null)
				{
					base.InternalProcessRecord();
				}
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(InstallOutOfBoxDlpPolicyCollection.DlpPolicyTemplatesXmlResourceId))
			{
				DlpUtils.DeleteOutOfBoxDlpPolicies(base.DataSession);
				DlpUtils.SaveOutOfBoxDlpTemplates(base.DataSession, DlpPolicyParser.ParseDlpPolicyTemplates(manifestResourceStream));
			}
		}

		private static string DlpPolicyTemplatesXmlResourceId = "OutOfBoxDlpPolicyTemplates.xml";
	}
}
