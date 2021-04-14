using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("enable", "SystemAttendantMailbox", DefaultParameterSetName = "Identity")]
	public sealed class EnableSystemAttendantMailbox : RecipientObjectActionTask<SystemAttendantIdParameter, ADSystemAttendantMailbox>
	{
		protected override ObjectId RootId
		{
			get
			{
				return this.ConfigurationSession.GetOrgContainerId();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			ADSystemAttendantMailbox dataObject = this.DataObject;
			if (string.IsNullOrEmpty(dataObject.DisplayName))
			{
				dataObject.DisplayName = dataObject.Name;
			}
			((IRecipientSession)base.DataSession).UseConfigNC = true;
			if (string.IsNullOrEmpty(this.DataObject.LegacyExchangeDN))
			{
				string parentLegacyDN = string.Format(CultureInfo.InvariantCulture, "/o={0}/ou={1}/cn=Recipients", new object[]
				{
					this.ConfigurationSession.GetOrgContainerId().Name,
					((ITopologyConfigurationSession)this.ConfigurationSession).GetAdministrativeGroupId().Name
				});
				this.DataObject.LegacyExchangeDN = LegacyDN.GenerateLegacyDN(parentLegacyDN, this.DataObject, true, null);
			}
			if (base.IsProvisioningLayerAvailable)
			{
				ProvisioningLayer.UpdateAffectedIConfigurable(this, this.ConvertDataObjectToPresentationObject(dataObject), false);
			}
			else
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		private bool LegacyDNIsUnique(string legacyDN)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, legacyDN),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, this.DataObject.Id)
			});
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.TenantGlobalCatalogSession, typeof(ADRecipient), filter, null, true));
			ADRecipient[] array = base.TenantGlobalCatalogSession.Find(null, QueryScope.SubTree, filter, null, 1);
			return 0 == array.Length;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return RecipientTaskHelper.ConvertRecipientToPresentationObject((ADRecipient)dataObject);
		}
	}
}
