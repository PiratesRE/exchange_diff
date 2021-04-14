using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	[Serializable]
	public class SidBasedIdentity : GenericSidIdentity
	{
		public SidBasedIdentity(string userPrincipal, string userSid, string memberName, string authType, string partitionId = null) : base(userSid, authType, new SecurityIdentifier(userSid), partitionId)
		{
			this.userPrincipalName = userPrincipal;
			this.memberName = memberName;
		}

		public SidBasedIdentity(string userPrincipal, string userSid, string memberName) : this(userPrincipal, userSid, memberName, "", null)
		{
		}

		public string PrincipalName
		{
			get
			{
				return this.userPrincipalName;
			}
		}

		public string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		public IEnumerable<string> PrepopulatedGroupSidIds
		{
			get
			{
				return this.prepopulatedGroupSidIds;
			}
		}

		public OrganizationId UserOrganizationId
		{
			get
			{
				return this.userOrganizationId;
			}
			protected internal set
			{
				this.userOrganizationId = value;
			}
		}

		public OrganizationProperties UserOrganizationProperties
		{
			get
			{
				return this.userOrganizationProperties;
			}
			protected internal set
			{
				this.userOrganizationProperties = value;
			}
		}

		internal void PrepopulateGroupSidIds(List<string> groupSidIds)
		{
			if (groupSidIds == null)
			{
				throw new ArgumentNullException("groupSidIds");
			}
			this.prepopulatedGroupSidIds = groupSidIds;
		}

		internal override ClientSecurityContext CreateClientSecurityContext()
		{
			SecurityAccessToken securityAccessToken = new SecurityAccessToken();
			securityAccessToken.UserSid = this.Sid.Value;
			ClientSecurityContext result;
			if (this.prepopulatedGroupSidIds != null)
			{
				ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "[SidBasedIdentity::CreateClientSecurityContext] Attempting to explicitly populate group SIDs with values: '{0}'.", string.Join(", ", this.prepopulatedGroupSidIds));
				securityAccessToken.GroupSids = (from s in this.prepopulatedGroupSidIds
				select new SidStringAndAttributes(s, 4U)).ToArray<SidStringAndAttributes>();
				result = new ClientSecurityContext(securityAccessToken, AuthzFlags.AuthzSkipTokenGroups);
			}
			else
			{
				result = new ClientSecurityContext(securityAccessToken);
			}
			return result;
		}

		private readonly string userPrincipalName;

		private readonly string memberName;

		private OrganizationId userOrganizationId;

		private OrganizationProperties userOrganizationProperties;

		protected IEnumerable<string> prepopulatedGroupSidIds;
	}
}
