using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Approval;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class RetentionPolicyTagUtility
	{
		public static void ClearDefaultPolicyTag(IConfigurationSession session, IList<RetentionPolicyTag> defaultPolicies, ApprovalApplicationId approvalApplicationType)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session is null probably due to not of IConfigurationSession type");
			}
			if (defaultPolicies != null && defaultPolicies.Count > 0)
			{
				switch (approvalApplicationType)
				{
				case ApprovalApplicationId.AutoGroup:
					using (IEnumerator<RetentionPolicyTag> enumerator = defaultPolicies.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							RetentionPolicyTag retentionPolicyTag = enumerator.Current;
							retentionPolicyTag.IsDefaultAutoGroupPolicyTag = false;
							session.Save(retentionPolicyTag);
						}
						return;
					}
					break;
				case ApprovalApplicationId.ModeratedRecipient:
					break;
				default:
					return;
				}
				foreach (RetentionPolicyTag retentionPolicyTag2 in defaultPolicies)
				{
					retentionPolicyTag2.IsDefaultModeratedRecipientsPolicyTag = false;
					session.Save(retentionPolicyTag2);
				}
			}
		}
	}
}
