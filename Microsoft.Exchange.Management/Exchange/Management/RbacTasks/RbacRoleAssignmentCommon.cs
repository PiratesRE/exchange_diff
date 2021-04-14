using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RbacTasks
{
	internal static class RbacRoleAssignmentCommon
	{
		internal static void CheckMutuallyExclusiveParameters(Task task)
		{
			task.CheckExclusiveParameters(new object[]
			{
				RbacCommonParameters.ParameterRecipientWriteScope,
				RbacCommonParameters.ParameterCustomRecipientWriteScope,
				RbacCommonParameters.ParameterRecipientOrganizationalUnitScope,
				"ExclusiveRecipientWriteScope",
				RbacCommonParameters.ParameterRecipientRelativeWriteScope,
				"WritableServer",
				"WritableRecipient",
				"ParameterWritableDatabase"
			});
			task.CheckExclusiveParameters(new object[]
			{
				RbacCommonParameters.ParameterConfigWriteScope,
				RbacCommonParameters.ParameterCustomConfigWriteScope,
				"ExclusiveConfigWriteScope",
				"WritableServer",
				"WritableRecipient",
				"ParameterWritableDatabase"
			});
		}

		internal const string ParameterSetUser = "User";

		internal const string ParameterSetSecurityGroup = "SecurityGroup";

		internal const string ParameterSetPolicy = "Policy";

		internal const string ParameterSetComputer = "Computer";

		internal const string ParameterSetRelativeRecipientWriteScope = "RelativeRecipientWriteScope";

		internal const string ParameterSetCustomRecipientWriteScope = "CustomRecipientWriteScope";

		internal const string ParameterSetDomainOrganizationalUnit = "RecipientOrganizationalUnitScope";

		internal const string ParameterSetExclusiveScope = "ExclusiveScope";

		internal static readonly RecipientWriteScopeType[] AllowedRecipientRelativeWriteScope = new RecipientWriteScopeType[]
		{
			RecipientWriteScopeType.None,
			RecipientWriteScopeType.Organization,
			RecipientWriteScopeType.Self,
			RecipientWriteScopeType.MyDistributionGroups,
			RecipientWriteScopeType.MailboxICanDelegate
		};
	}
}
