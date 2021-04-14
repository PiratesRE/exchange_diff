using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RbacTasks
{
	internal static class RoleAssignmentHelper
	{
		internal static string GenerateUniqueRoleAssignmentName(IConfigurationSession configurationSession, ADObjectId orgContainerId, string roleName, string roleAssigneeName, RoleAssignmentDelegationType roleAssignmentDelegationType, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			if (orgContainerId == null)
			{
				throw new ArgumentNullException("orgContainerId");
			}
			string text = roleName + "-" + roleAssigneeName;
			if (roleAssignmentDelegationType != RoleAssignmentDelegationType.Regular)
			{
				text += "-Delegating";
			}
			text = text.Trim();
			if (text.Length > 64)
			{
				text = text.Substring(0, 64).Trim();
			}
			if (writeVerbose == null)
			{
				throw new ArgumentNullException("writeVerbose");
			}
			ADObjectId descendantId = orgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer);
			string text2 = text;
			if (text2.Length > 61)
			{
				text2 = text2.Substring(0, 61).Trim();
			}
			int num = 1;
			for (;;)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, text);
				writeVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(configurationSession, typeof(ExchangeRoleAssignment), filter, descendantId, false));
				ExchangeRoleAssignment[] array = configurationSession.Find<ExchangeRoleAssignment>(descendantId, QueryScope.OneLevel, filter, null, 1);
				if (array.Length == 0)
				{
					break;
				}
				text = text2 + "-" + num.ToString();
				num++;
				if (num >= 100)
				{
					return text;
				}
			}
			return text;
		}
	}
}
