using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class RoleAssignmentObjectResolverExtension
	{
		public static bool HasMultipleScopeTypes(this IEnumerable<RoleAssignmentObjectResolverRow> roleAssignments)
		{
			string text = null;
			string text2 = null;
			foreach (RoleAssignmentObjectResolverRow roleAssignmentObjectResolverRow in roleAssignments)
			{
				if (!roleAssignmentObjectResolverRow.IsDelegating)
				{
					if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
					{
						return true;
					}
					if (roleAssignmentObjectResolverRow.RecipientWriteScopeType == RecipientWriteScopeType.ExclusiveRecipientScope || roleAssignmentObjectResolverRow.ConfigWriteScopeType == ConfigWriteScopeType.ExclusiveConfigScope)
					{
						return true;
					}
					if (roleAssignmentObjectResolverRow.CustomRecipientWriteScope != null && roleAssignmentObjectResolverRow.CustomConfigWriteScope != null)
					{
						return true;
					}
					if (roleAssignmentObjectResolverRow.ConfigWriteScopeType != ConfigWriteScopeType.None)
					{
						if (text == null)
						{
							if (roleAssignmentObjectResolverRow.CustomConfigWriteScope != null)
							{
								text = roleAssignmentObjectResolverRow.CustomConfigWriteScope.Name;
							}
							else
							{
								text = string.Empty;
							}
						}
						else if (roleAssignmentObjectResolverRow.CustomConfigWriteScope != null)
						{
							if (string.Compare(text, roleAssignmentObjectResolverRow.CustomConfigWriteScope.Name, true) != 0)
							{
								return true;
							}
						}
						else if (text != string.Empty)
						{
							return true;
						}
					}
					if (roleAssignmentObjectResolverRow.RecipientWriteScopeType != RecipientWriteScopeType.None)
					{
						if (roleAssignmentObjectResolverRow.RecipientWriteScopeType != RecipientWriteScopeType.CustomRecipientScope && roleAssignmentObjectResolverRow.RecipientWriteScopeType != RecipientWriteScopeType.OU && roleAssignmentObjectResolverRow.RecipientWriteScopeType != RecipientWriteScopeType.Organization)
						{
							return true;
						}
						if (text2 == null)
						{
							if (roleAssignmentObjectResolverRow.CustomRecipientWriteScope != null)
							{
								text2 = roleAssignmentObjectResolverRow.CustomRecipientWriteScope.Name;
							}
							else
							{
								text2 = string.Empty;
							}
						}
						else if (roleAssignmentObjectResolverRow.CustomRecipientWriteScope != null)
						{
							if (string.Compare(text2, roleAssignmentObjectResolverRow.CustomRecipientWriteScope.Name, true) != 0)
							{
								return true;
							}
						}
						else if (text2 != string.Empty)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
