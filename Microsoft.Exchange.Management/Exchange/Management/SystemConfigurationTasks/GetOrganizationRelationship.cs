using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OrganizationRelationship", DefaultParameterSetName = "Identity")]
	public sealed class GetOrganizationRelationship : GetMultitenancySystemConfigurationObjectTask<OrganizationRelationshipIdParameter, OrganizationRelationship>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			OrganizationRelationship organizationRelationship = (OrganizationRelationship)dataObject;
			ADObjectId freeBusyAccessScope = organizationRelationship.FreeBusyAccessScope;
			if (freeBusyAccessScope != null)
			{
				ADGroup adgroup = (ADGroup)base.GetDataObject<ADGroup>(new GroupIdParameter(freeBusyAccessScope), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(freeBusyAccessScope.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(freeBusyAccessScope.ToString())));
				organizationRelationship[OrganizationRelationshipNonAdProperties.FreeBusyAccessScopeCache] = adgroup.Id;
			}
			ADObjectId mailTipsAccessScope = organizationRelationship.MailTipsAccessScope;
			if (mailTipsAccessScope != null)
			{
				ADGroup adgroup2 = (ADGroup)base.GetDataObject<ADGroup>(new GroupIdParameter(mailTipsAccessScope), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(mailTipsAccessScope.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(mailTipsAccessScope.ToString())));
				organizationRelationship[OrganizationRelationshipNonAdProperties.MailTipsAccessScopeScopeCache] = adgroup2.Id;
			}
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}
	}
}
