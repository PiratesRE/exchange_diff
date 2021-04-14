using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Initialize", "ExchangeLocalGroups", SupportsShouldProcess = true)]
	public sealed class InitializeExchangeLocalGroups : SetupTaskBase
	{
		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (!Datacenter.IsMicrosoftHostedOnly(true))
			{
				base.ThrowTerminatingError(new DatacenterEnvironmentOnlyOperationException(), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADContainer container = this.FindRootUsersContainer(this.domainConfigurationSession, this.rootDomain.Id);
			string text = string.Format("{0}$$$", this.rootDomain.Name);
			ADGroup adgroup = this.FindADGroup(this.domainConfigurationSession, container, text);
			if (adgroup != null)
			{
				base.WriteVerbose(Strings.InfoGroupAlreadyPresent(text));
			}
			else
			{
				LocalizedString exchangeMigrationSidHistoryAuditingDSGDescription = Strings.ExchangeMigrationSidHistoryAuditingDSGDescription;
				this.CreateDomainLocalSecurityGroup(container, text, exchangeMigrationSidHistoryAuditingDSGDescription);
			}
			TaskLogger.LogExit();
		}

		private ADContainer FindRootUsersContainer(IConfigurationSession session, ADObjectId domain)
		{
			ADContainer[] array = session.Find<ADContainer>(domain, QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "Users"), null, 1);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		private ADGroup FindADGroup(IConfigurationSession session, ADContainer container, string groupName)
		{
			ADGroup[] array = session.Find<ADGroup>(container.Id, QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, groupName), null, 1);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		private ADGroup CreateDomainLocalSecurityGroup(ADContainer container, string groupName, LocalizedString groupDescription)
		{
			GroupTypeFlags groupType = GroupTypeFlags.DomainLocal | GroupTypeFlags.SecurityEnabled;
			return this.CreateGroup(this.rootDomainRecipientSession, container.Id, groupName, groupDescription, groupType);
		}

		private ADGroup CreateGroup(IRecipientSession session, ADObjectId containerId, string groupName, LocalizedString groupDescription, GroupTypeFlags groupType)
		{
			ADGroup adgroup = new ADGroup(session, groupName, containerId, groupType);
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			multiValuedProperty.Add(groupDescription);
			adgroup[ADRecipientSchema.Description] = multiValuedProperty;
			adgroup.SamAccountName = groupName;
			SetupTaskBase.Save(adgroup, session);
			base.WriteVerbose(Strings.InfoCreatedGroup(adgroup.DistinguishedName));
			return adgroup;
		}

		private const string rootUsersContainerCommonName = "Users";

		private const GroupTypeFlags DSG_GROUPTYPE_FLAGS = GroupTypeFlags.DomainLocal | GroupTypeFlags.SecurityEnabled;
	}
}
