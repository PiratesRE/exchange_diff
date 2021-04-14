using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class TaskUserInfo
	{
		public OrganizationId ExecutingUserOrganizationId { get; private set; }

		public OrganizationId CurrentOrganizationId { get; private set; }

		public ADObjectId ExecutingUserId { get; private set; }

		public string ExecutingUserIdentityName { get; private set; }

		public SmtpAddress ExecutingWindowsLiveId { get; private set; }

		public TaskUserInfo(OrganizationId executingUserOrganizationId, OrganizationId currentOrganizationId, ADObjectId executingUserId, string executingUserIdentityName, SmtpAddress executingWindowsLiveId)
		{
			this.ExecutingUserOrganizationId = executingUserOrganizationId;
			this.CurrentOrganizationId = currentOrganizationId;
			this.ExecutingUserId = executingUserId;
			this.ExecutingUserIdentityName = executingUserIdentityName;
			this.ExecutingWindowsLiveId = executingWindowsLiveId;
		}

		public void UpdateCurrentOrganizationId(OrganizationId currentOrgId)
		{
			this.CurrentOrganizationId = currentOrgId;
		}
	}
}
