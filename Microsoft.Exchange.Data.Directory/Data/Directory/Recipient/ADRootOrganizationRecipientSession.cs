using System;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ADRootOrganizationRecipientSession : ADRecipientObjectSession, IRootOrganizationRecipientSession, IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		public ADRootOrganizationRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
		}

		public ADRootOrganizationRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope) : this(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.CheckConfigScopeParameter(configScope);
			base.ConfigScope = configScope;
		}

		public SecurityIdentifier GetExchangeServersUsgSid()
		{
			return base.GetWellKnownExchangeGroupSid(WellKnownGuid.ExSWkGuid);
		}
	}
}
