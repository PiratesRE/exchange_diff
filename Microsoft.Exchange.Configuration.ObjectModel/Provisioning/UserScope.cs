using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Provisioning
{
	public class UserScope
	{
		internal ScopeSet CurrentScopeSet { get; private set; }

		public string UserId
		{
			get
			{
				return this.userId;
			}
		}

		public OrganizationId ExecutingUserOrganizationId
		{
			get
			{
				return this.executingUserOrganizationId;
			}
		}

		public OrganizationId CurrentOrganizationId
		{
			get
			{
				return this.currentOrganizationId;
			}
		}

		public UserScopeFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		internal UserScope(string userId, OrganizationId executingUserOrganizationId, OrganizationId currentOrganizationId, UserScopeFlags flags, ScopeSet currentScopeSet)
		{
			this.userId = userId;
			this.executingUserOrganizationId = executingUserOrganizationId;
			this.currentOrganizationId = currentOrganizationId;
			this.CurrentScopeSet = currentScopeSet;
			this.flags = flags;
		}

		public UserScope(string userId, OrganizationId executingUserOrganizationId, OrganizationId currentOrganizationId, UserScopeFlags flags) : this(userId, executingUserOrganizationId, currentOrganizationId, flags, null)
		{
		}

		private string userId;

		private OrganizationId executingUserOrganizationId;

		private OrganizationId currentOrganizationId;

		private UserScopeFlags flags;
	}
}
