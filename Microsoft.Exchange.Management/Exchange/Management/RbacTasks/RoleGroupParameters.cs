using System;

namespace Microsoft.Exchange.Management.RbacTasks
{
	internal static class RoleGroupParameters
	{
		internal const string DefaultParameterSet = "default";

		internal const string CrossForestParameterSet = "crossforest";

		internal const string ExchangeDatacenterCrossForestParameterSet = "ExchangeDatacenterCrossForestParameterSet";

		internal const string LinkedPartnerGroupParameterSet = "linkedpartnergroup";

		internal static readonly string ParameterLinkedForeignGroup = "LinkedForeignGroup";

		internal static readonly string ParameterLinkedForeignGroupSid = "LinkedForeignGroupSid";

		internal static readonly string ParameterLinkedDomainController = "LinkedDomainController";

		internal static readonly string ParameterLinkedCredential = "LinkedCredential";

		internal static readonly string ParameterMembers = "Members";

		internal static readonly string ParameterLinkedPartnerGroupId = "LinkedPartnerGroupId";

		internal static readonly string PartnerLinkedPartnerOrganizationId = "LinkedPartnerOrganizationId";

		internal static readonly string PartnerLinkedPartnerManaged = "LinkedPartnerManaged";
	}
}
