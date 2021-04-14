using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class SlaveAccountTokenMunger : ITokenMunger
	{
		public SlaveAccountTokenMunger() : this(new GenericExecuter(new DirectoryExceptionTranslator()))
		{
		}

		public SlaveAccountTokenMunger(IExecuter directoryAccessor)
		{
			this.directoryAccessor = directoryAccessor;
		}

		public ClientSecurityContext MungeToken(ClientSecurityContext clientSecurityContext, OrganizationId tenantOrganizationId)
		{
			ClientSecurityContext result;
			if (!this.TryMungeToken(clientSecurityContext, tenantOrganizationId, null, out result))
			{
				throw new TokenMungingException(string.Format("Slave account was not found for SID {0}", clientSecurityContext.UserSid));
			}
			return result;
		}

		public bool TryMungeToken(ClientSecurityContext clientSecurityContext, OrganizationId tenantOrganizationId, SecurityIdentifier slaveAccountSid, out ClientSecurityContext mungedClientSecurityContext)
		{
			mungedClientSecurityContext = null;
			ArgumentValidator.ThrowIfNull("clientSecurityContext", clientSecurityContext);
			ArgumentValidator.ThrowIfNull("tenantOrganizationId", tenantOrganizationId);
			SecurityIdentifier securityIdentifier = slaveAccountSid;
			if (securityIdentifier == null && !this.TryFindSlaveAccount(clientSecurityContext.UserSid, tenantOrganizationId, out securityIdentifier))
			{
				return false;
			}
			SecurityAccessTokenEx securityAccessTokenEx = new SecurityAccessTokenEx();
			try
			{
				using (AuthzContextHandle authzContext = AuthzAuthorization.GetAuthzContext(securityIdentifier, false))
				{
					using (ClientSecurityContext clientSecurityContext2 = new ClientSecurityContext(authzContext))
					{
						clientSecurityContext2.SetSecurityAccessToken(securityAccessTokenEx);
					}
				}
			}
			catch (Win32Exception innerException)
			{
				throw new TokenMungingException(string.Format("Unable to munge token for sid[{0}] and linked account sid[{1}], organizationId[{2}]", clientSecurityContext.UserSid, securityIdentifier, tenantOrganizationId), innerException);
			}
			bool result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IdentityReferenceCollection groups = clientSecurityContext.GetGroups();
				AuthzContextHandle authzContextHandle = AuthzAuthorization.AddSidsToContext(clientSecurityContext.ClientContextHandle, SlaveAccountTokenMunger.FilterGroups(securityAccessTokenEx.GroupSids, groups), SlaveAccountTokenMunger.FilterGroups(securityAccessTokenEx.RestrictedGroupSids, groups));
				disposeGuard.Add<AuthzContextHandle>(authzContextHandle);
				mungedClientSecurityContext = new ClientSecurityContext(authzContextHandle);
				disposeGuard.Success();
				result = true;
			}
			return result;
		}

		private bool TryFindSlaveAccount(SecurityIdentifier masterAccountSid, OrganizationId tenantOrganizationId, out SecurityIdentifier slaveAccountSid)
		{
			slaveAccountSid = null;
			MiniRecipient[] slaveAccounts = this.directoryAccessor.GetSlaveAccounts(masterAccountSid, tenantOrganizationId, SlaveAccountTokenMunger.miniRecipientProperties);
			MiniRecipient miniRecipient = slaveAccounts.FirstOrDefault<MiniRecipient>();
			if (slaveAccounts.Length == 0 || miniRecipient == null)
			{
				return false;
			}
			if (slaveAccounts.Length > 1)
			{
				throw new TokenMungingException(string.Format("More than one slave account was found for SID {0}: {1} and {2}", masterAccountSid, slaveAccounts[0].Sid, slaveAccounts[1].Sid));
			}
			if (miniRecipient.RecipientType != RecipientType.User && miniRecipient.RecipientType != RecipientType.MailUser && miniRecipient.RecipientType != RecipientType.UserMailbox)
			{
				throw new TokenMungingException(string.Format("Slave account {0} for SID {1} is not of a supported type {2}", miniRecipient.DistinguishedName, masterAccountSid, miniRecipient.RecipientType));
			}
			if (((UserAccountControlFlags)miniRecipient[ADUserSchema.UserAccountControl] & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.None)
			{
				throw new TokenMungingException(string.Format("Slave account {0} for SID {1} is not disabled. UserAccountControl: {2}", miniRecipient.DistinguishedName, masterAccountSid, (UserAccountControlFlags)miniRecipient[ADUserSchema.UserAccountControl]));
			}
			slaveAccountSid = miniRecipient.Sid;
			return true;
		}

		private static Dictionary<SecurityIdentifier, uint> FilterGroups(SidBinaryAndAttributes[] sidAndAttributes, IEnumerable<IdentityReference> groupsToFilterOut)
		{
			Dictionary<SecurityIdentifier, uint> dictionary = (sidAndAttributes ?? Array<SidBinaryAndAttributes>.Empty).ToDictionary((SidBinaryAndAttributes sidAndAttribute) => sidAndAttribute.SecurityIdentifier, (SidBinaryAndAttributes sidAndAttribute) => sidAndAttribute.Attributes);
			foreach (IdentityReference identityReference in groupsToFilterOut)
			{
				SecurityIdentifier securityIdentifier = identityReference as SecurityIdentifier;
				if (securityIdentifier != null)
				{
					dictionary.Remove(securityIdentifier);
				}
			}
			return dictionary;
		}

		private readonly IExecuter directoryAccessor;

		private static readonly PropertyDefinition[] miniRecipientProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.MAPIBlockOutlookVersions,
			ADRecipientSchema.MAPIBlockOutlookRpcHttp,
			ADRecipientSchema.MAPIEnabled,
			ADUserSchema.UserAccountControl
		};
	}
}
