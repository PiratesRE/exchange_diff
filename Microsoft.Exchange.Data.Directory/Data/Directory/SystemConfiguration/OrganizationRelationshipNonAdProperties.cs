using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class OrganizationRelationshipNonAdProperties
	{
		internal static readonly ADPropertyDefinition FreeBusyAccessScopeCache = new ADPropertyDefinition("FreeBusyAccessScopeCache", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "FreeBusyAccessScopeCache", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MailTipsAccessScopeScopeCache = new ADPropertyDefinition("MailTipsAccessScopeScopeCache", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "MailTipsAccessScopeScopeCache", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
