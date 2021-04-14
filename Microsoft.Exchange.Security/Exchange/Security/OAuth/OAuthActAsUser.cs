using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class OAuthActAsUser
	{
		private OAuthActAsUser(OrganizationId organizationId, OAuthActAsUser.VerifiedUserInfo verifiedUser)
		{
			this.organizationId = organizationId;
			this.verifiedUser = verifiedUser;
		}

		private OAuthActAsUser(OrganizationId organizationId, Dictionary<string, string> userAttributes)
		{
			this.organizationId = organizationId;
			this.rawAttributes = userAttributes;
			this.rawAttributeString = string.Join("|", from p in this.rawAttributes
			orderby p.Key
			select p.Key + ":" + p.Value);
		}

		protected OAuthActAsUser(OrganizationId organizationId, string attributeType, string attributeValue)
		{
			this.organizationId = organizationId;
			this.rawAttributes = new Dictionary<string, string>();
			this.rawAttributes.Add(attributeType, attributeValue);
			this.rawAttributeString = attributeType + ":" + attributeValue;
		}

		public void VerifyUser()
		{
			this.verifiedUser = OAuthActAsUser.VerifiedUserInfoResultCache.Instance.Get(this);
			if (this.verifiedUser.Exception != null)
			{
				throw this.verifiedUser.Exception;
			}
		}

		public bool IsUserVerified
		{
			get
			{
				return this.verifiedUser != null;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				this.EnsureUserIsVerified();
				return this.verifiedUser.Sid;
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				this.EnsureUserIsVerified();
				return this.verifiedUser.MasterAccountSid;
			}
		}

		public string UserPrincipalName
		{
			get
			{
				this.EnsureUserIsVerified();
				return this.verifiedUser.UserPrincipalName;
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				this.EnsureUserIsVerified();
				return this.verifiedUser.WindowsLiveID;
			}
		}

		public string DistinguishedName
		{
			get
			{
				this.EnsureUserIsVerified();
				return this.verifiedUser.DistinguishedName;
			}
		}

		public ADRawEntry ADRawEntry
		{
			get
			{
				this.EnsureUserIsVerified();
				return this.verifiedUser.ADRawEntry;
			}
		}

		public void EnsureUserIsVerified()
		{
			if (!this.IsUserVerified)
			{
				this.VerifyUser();
			}
		}

		protected virtual OAuthActAsUser.VerifiedUserInfo InternalVerifyUser()
		{
			IDirectorySession directorySession = AuthCommon.IsWindowsLiveIDEnabled ? this.GetTenantRecipientSession() : this.GetRecipientSession();
			QueryFilter filter = this.GetQueryFilter();
			ADRawEntry[] users = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				users = directorySession.Find(null, QueryScope.SubTree, filter, null, 3, OAuthActAsUser.ADRawEntryPropertySet);
			});
			if (adoperationResult != ADOperationResult.Success)
			{
				return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithADOperationResult(adoperationResult);
			}
			if (users == null || users.Length == 0)
			{
				return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithNoUserFound(filter.ToString());
			}
			string text;
			this.rawAttributes.TryGetValue("sid", out text);
			ADRawEntry adrawEntry = null;
			if (users.Length == 1)
			{
				adrawEntry = users[0];
			}
			else
			{
				bool flag;
				if (users.Length == 2 && !string.IsNullOrEmpty(text))
				{
					adrawEntry = this.ChooseADRawEntryBasedOnSID(users[0], users[1], text);
					flag = (adrawEntry == null);
				}
				else if (AuthCommon.IsWindowsLiveIDEnabled)
				{
					adrawEntry = (directorySession as ITenantRecipientSession).ChooseBetweenAmbiguousUsers(users);
					flag = (adrawEntry == null);
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					string userDNs = string.Join(",", from r in users
					select (string)r[ADObjectSchema.DistinguishedName]);
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithAmbiguousUsersFound(filter, userDNs);
				}
			}
			string text2 = (string)adrawEntry[ADObjectSchema.DistinguishedName];
			ExTraceGlobals.OAuthTracer.TraceDebug<QueryFilter, string>((long)this.GetHashCode(), "[InternalVerifyUser] Found exactly 1 matched user with {0}: {1}", filter, text2);
			if (!string.IsNullOrEmpty(text))
			{
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)adrawEntry[ADRecipientSchema.MasterAccountSid];
				SecurityIdentifier securityIdentifier2 = (SecurityIdentifier)adrawEntry[ADMailboxRecipientSchema.Sid];
				if (!this.IsValidADRawEntryBasedOnSID(securityIdentifier, securityIdentifier2, text))
				{
					ExTraceGlobals.OAuthTracer.TraceWarning((long)this.GetHashCode(), "[InternalVerifyUser] SID claim is {0}, the recipient found is {1}, with SID {2}, MasterAccountSid {3}", new object[]
					{
						text,
						text2,
						securityIdentifier2,
						securityIdentifier
					});
					return OAuthActAsUser.VerifiedUserInfo.FromException(new InvalidOAuthTokenException(OAuthErrors.NameIdNotMatchMasterAccountSid, null, null));
				}
			}
			string text3;
			this.rawAttributes.TryGetValue("netid", out text3);
			if (!string.IsNullOrEmpty(text3))
			{
				SmtpAddress smtpAddress = (SmtpAddress)adrawEntry[ADRecipientSchema.WindowsLiveID];
				NetID netID = (NetID)adrawEntry[ADUserSchema.NetID];
				if (!this.IsValidADRawEntryBasedOnNetID(smtpAddress, netID, text3))
				{
					ExTraceGlobals.OAuthTracer.TraceWarning((long)this.GetHashCode(), "[InternalVerifyUser] NetID claim is {0}, the recipient found is {1}, with NetID {2}, WindowsLiveID  {3}", new object[]
					{
						text3,
						text2,
						netID,
						smtpAddress
					});
					return OAuthActAsUser.VerifiedUserInfo.FromException(new InvalidOAuthTokenException(OAuthErrors.NameIdNotMatchLiveIDInstanceType, null, null));
				}
			}
			return OAuthActAsUser.VerifiedUserInfo.FromADObject(adrawEntry);
		}

		private ADRawEntry ChooseADRawEntryBasedOnSID(ADRawEntry recipient1, ADRawEntry recipient2, string sidQueryValue)
		{
			SecurityIdentifier securityIdentifier = (SecurityIdentifier)recipient1[ADRecipientSchema.MasterAccountSid];
			SecurityIdentifier securityIdentifier2 = (SecurityIdentifier)recipient2[ADRecipientSchema.MasterAccountSid];
			SecurityIdentifier securityIdentifier3 = (SecurityIdentifier)recipient1[ADMailboxRecipientSchema.Sid];
			SecurityIdentifier securityIdentifier4 = (SecurityIdentifier)recipient2[ADMailboxRecipientSchema.Sid];
			bool flag = this.IsValidADRawEntryBasedOnSID(securityIdentifier, securityIdentifier3, sidQueryValue);
			bool flag2 = this.IsValidADRawEntryBasedOnSID(securityIdentifier2, securityIdentifier4, sidQueryValue);
			if (flag && flag2)
			{
				ExTraceGlobals.OAuthTracer.TraceWarning((long)this.GetHashCode(), "[ChooseADRawEntryBasedOnSID] both recipients have valid SID in place. {0}, {1}, {2} and {3} {4} {5}", new object[]
				{
					recipient1[ADObjectSchema.DistinguishedName],
					securityIdentifier3,
					securityIdentifier,
					recipient2[ADObjectSchema.DistinguishedName],
					securityIdentifier4,
					securityIdentifier2
				});
				if (securityIdentifier != null && string.Equals(securityIdentifier.ToString(), sidQueryValue, StringComparison.OrdinalIgnoreCase))
				{
					return recipient1;
				}
				if (securityIdentifier2 != null && string.Equals(securityIdentifier2.ToString(), sidQueryValue, StringComparison.OrdinalIgnoreCase))
				{
					return recipient2;
				}
			}
			ExTraceGlobals.OAuthTracer.TraceWarning<object, object, string>((long)this.GetHashCode(), "[ChooseADRawEntryBasedOnSID] at least 1 recipient from '{0}' and '{1}' has no valid SID in place, the sid claim is '{2}'", recipient1[ADObjectSchema.DistinguishedName], recipient2[ADObjectSchema.DistinguishedName], sidQueryValue);
			return null;
		}

		private bool IsValidADRawEntryBasedOnSID(SecurityIdentifier masterSid, SecurityIdentifier sid, string sidQueryValue)
		{
			if (masterSid != null)
			{
				return string.Equals(masterSid.ToString(), sidQueryValue, StringComparison.OrdinalIgnoreCase);
			}
			return sid != null && string.Equals(sid.ToString(), sidQueryValue, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsValidADRawEntryBasedOnNetID(SmtpAddress windowsLiveId, NetID netId, string netIdQueryValue)
		{
			if (windowsLiveId == SmtpAddress.Empty)
			{
				return false;
			}
			if (netId == null)
			{
				return false;
			}
			LiveIdInstanceType? liveIdInstanceType = DomainPropertyCache.Singleton.Get(new SmtpDomainWithSubdomains(windowsLiveId.Domain)).LiveIdInstanceType;
			return liveIdInstanceType != null && liveIdInstanceType.Value == LiveIdInstanceType.Business && netId == new NetID(netIdQueryValue);
		}

		private QueryFilter GetQueryFilter()
		{
			string text;
			this.rawAttributes.TryGetValue("sid", out text);
			string text2;
			this.rawAttributes.TryGetValue("netid", out text2);
			string text3;
			this.rawAttributes.TryGetValue("upn", out text3);
			string text4;
			this.rawAttributes.TryGetValue("smtp", out text4);
			string text5;
			this.rawAttributes.TryGetValue("sip", out text5);
			string text6;
			this.rawAttributes.TryGetValue("liveid", out text6);
			List<QueryFilter> list = new List<QueryFilter>(6);
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Sid, text));
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MasterAccountSid, text));
			}
			if (!string.IsNullOrEmpty(text2))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.NetID, new NetID(text2)));
			}
			if (!string.IsNullOrEmpty(text3))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.UserPrincipalName, text3));
			}
			if (!string.IsNullOrEmpty(text4))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.PrimarySmtpAddress, text4));
			}
			if (!string.IsNullOrEmpty(text5))
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse("sip", text5);
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, proxyAddress.ProxyAddressString));
			}
			if (!string.IsNullOrEmpty(text6))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.WindowsLiveID, text6));
			}
			QueryFilter queryFilter = (list.Count == 1) ? list.First<QueryFilter>() : new OrFilter(list.ToArray());
			ExTraceGlobals.OAuthTracer.TraceDebug<QueryFilter>((long)this.GetHashCode(), "[GetQueryFilter] The query filter used to find the user: {0}", queryFilter);
			return queryFilter;
		}

		public void AddExtensionDataToCommonAccessToken(CommonAccessToken token)
		{
			if (this.rawAttributes != null)
			{
				token.ExtensionData["RawUserInfo"] = this.rawAttributes.SerializeToJson();
			}
			if (this.IsUserVerified)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
				dictionary.Add("UserDn", this.DistinguishedName);
				if (!string.IsNullOrEmpty(this.UserPrincipalName))
				{
					dictionary.Add("UserPrincipalName", this.UserPrincipalName);
				}
				if (this.Sid != null)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "AddExtensionDataToCommonAccessToken Sid value: {0}", this.Sid.Value);
					dictionary.Add("UserSid", this.Sid.Value);
				}
				if (this.MasterAccountSid != null)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "AddExtensionDataToCommonAccessToken MasterAccountSid value: {0}", this.MasterAccountSid.Value);
					dictionary.Add("MasterSid", this.MasterAccountSid.Value);
				}
				if (this.WindowsLiveID != SmtpAddress.Empty)
				{
					dictionary.Add("MemberName", this.WindowsLiveID.ToString());
				}
				token.ExtensionData["VerifiedUserInfo"] = dictionary.SerializeToJson();
			}
		}

		public static OAuthActAsUser CreateFromPrimarySid(OrganizationId organizationId, SecurityIdentifier securityIdentifier)
		{
			return new OAuthActAsUser.SidOnlyActAsUser(organizationId, securityIdentifier);
		}

		public static OAuthActAsUser CreateFromExternalDirectoryObjectId(OrganizationId organizationId, string externalDirectoryObjectId)
		{
			return new OAuthActAsUser.OidOnlyActAsUser(organizationId, externalDirectoryObjectId);
		}

		public static OAuthActAsUser CreateFromPuidOnly(OrganizationId organizationId, NetID puid)
		{
			return new OAuthActAsUser.PuidOnlyActAsUser(organizationId, puid);
		}

		public static OAuthActAsUser CreateFromSmtpOnly(OrganizationId organizationId, string smtpAddress)
		{
			return new OAuthActAsUser.SmtpOnlyActAsUser(organizationId, smtpAddress);
		}

		public static OAuthActAsUser CreateFromPuid(OAuthTokenHandler handler, OrganizationId organizationId, string puid, string smtpOrLiveId)
		{
			ITenantRecipientSession tenantRecipientSession;
			ADRawEntry adrawEntry = DirectoryHelper.GetADRawEntry(puid, organizationId.OrganizationalUnit.Name, smtpOrLiveId, new PropertyDefinition[]
			{
				ADMailboxRecipientSchema.Sid
			}, true, out tenantRecipientSession);
			if (adrawEntry == null)
			{
				handler.Throw(OAuthErrors.NoUserFoundWithGivenClaims, null, null, null);
			}
			return new OAuthActAsUser.SidOnlyActAsUser(organizationId, (SecurityIdentifier)adrawEntry[ADMailboxRecipientSchema.Sid]);
		}

		public static OAuthActAsUser CreateFromOuterToken(OAuthTokenHandler handler, OrganizationId organizationId, JwtSecurityToken jwt, bool acceptSecurityIdentifierInformation)
		{
			string upn = null;
			string smtp = null;
			string sip = null;
			string nameid = null;
			string nii = null;
			OAuthCommon.TryGetClaimValue(jwt, Constants.ClaimTypes.NameIdentifier, out nameid);
			OAuthCommon.TryGetClaimValue(jwt, Constants.ClaimTypes.Nii, out nii);
			OAuthCommon.TryGetClaimValue(jwt, Constants.ClaimTypes.Upn, out upn);
			OAuthCommon.TryGetClaimValue(jwt, Constants.ClaimTypes.Smtp, out smtp);
			OAuthCommon.TryGetClaimValue(jwt, Constants.ClaimTypes.Sip, out sip);
			Dictionary<string, string> dictionary = OAuthActAsUser.AdjustClaimToUserAttribute(handler, nii, nameid, upn, smtp, sip, acceptSecurityIdentifierInformation);
			return OAuthActAsUser.InternalCreateFromAttributes(organizationId, true, dictionary, null);
		}

		public static OAuthActAsUser CreateFromAppContext(OAuthTokenHandler handler, OrganizationId organizationId, Dictionary<string, string> claimsFromAppContext, bool acceptSecurityIdentifierInformation)
		{
			string upn = null;
			string smtp = null;
			string sip = null;
			string nameid = null;
			string nii = null;
			claimsFromAppContext.TryGetValue(Constants.ClaimTypes.NameIdentifier, out nameid);
			claimsFromAppContext.TryGetValue(Constants.ClaimTypes.Nii, out nii);
			claimsFromAppContext.TryGetValue(Constants.ClaimTypes.Upn, out upn);
			claimsFromAppContext.TryGetValue(Constants.ClaimTypes.Smtp, out smtp);
			claimsFromAppContext.TryGetValue(Constants.ClaimTypes.Sip, out sip);
			Dictionary<string, string> dictionary = OAuthActAsUser.AdjustClaimToUserAttribute(handler, nii, nameid, upn, smtp, sip, acceptSecurityIdentifierInformation);
			return OAuthActAsUser.InternalCreateFromAttributes(organizationId, true, dictionary, null);
		}

		internal static Dictionary<string, string> AdjustClaimToUserAttribute(OAuthTokenHandler handler, string nii, string nameid, string upn, string smtp, string sip, bool acceptSecurityIdentifierInformation)
		{
			if (string.IsNullOrEmpty(nii))
			{
				return OAuthActAsUser.CollectUserAttributes(handler, null, null, nameid, smtp, sip, null);
			}
			if (!acceptSecurityIdentifierInformation)
			{
				if (AuthCommon.IsWindowsLiveIDEnabled)
				{
					return OAuthActAsUser.CollectUserAttributes(handler, null, null, null, smtp, sip, upn);
				}
				return OAuthActAsUser.CollectUserAttributes(handler, null, null, upn, smtp, sip, null);
			}
			else if (string.Equals(nii, Constants.NiiClaimValues.ActiveDirectory, StringComparison.OrdinalIgnoreCase))
			{
				if (AuthCommon.IsWindowsLiveIDEnabled)
				{
					return OAuthActAsUser.CollectUserAttributes(handler, null, null, null, smtp, sip, upn);
				}
				return OAuthActAsUser.CollectUserAttributes(handler, nameid, null, upn, smtp, sip, null);
			}
			else
			{
				if (!string.Equals(nii, Constants.NiiClaimValues.BusinessLiveId, StringComparison.OrdinalIgnoreCase) && !string.Equals(nii, Constants.NiiClaimValues.LegacyBusinessLiveId, StringComparison.OrdinalIgnoreCase))
				{
					return OAuthActAsUser.CollectUserAttributes(handler, null, null, null, smtp, sip, null);
				}
				if (!AuthCommon.IsWindowsLiveIDEnabled)
				{
					return OAuthActAsUser.CollectUserAttributes(handler, null, null, upn, smtp, sip, null);
				}
				return OAuthActAsUser.CollectUserAttributes(handler, null, nameid, null, smtp, sip, upn);
			}
		}

		private static Dictionary<string, string> CollectUserAttributes(OAuthTokenHandler handler, string sid, string netId, string upn, string smtp, string sip, string liveId)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			OAuthActAsUser.CollectIfNotNull(dictionary, "sid", sid);
			NetID netID;
			if (NetID.TryParse(netId, out netID))
			{
				OAuthActAsUser.CollectIfNotNull(dictionary, "netid", netId);
			}
			else
			{
				ExTraceGlobals.OAuthTracer.TraceWarning<string>(0L, "[CollectUserAttributes] Invalid NetID claim value {0}", netId);
			}
			OAuthActAsUser.CollectIfNotNull(dictionary, "upn", upn);
			OAuthActAsUser.CollectIfNotNull(dictionary, "smtp", smtp);
			OAuthActAsUser.CollectIfNotNull(dictionary, "sip", sip);
			OAuthActAsUser.CollectIfNotNull(dictionary, "liveid", liveId);
			if (dictionary.Count == 0)
			{
				ExTraceGlobals.OAuthTracer.TraceWarning(0L, "[CollectUserAttributes] No valid user context claims found");
				handler.Throw(OAuthErrors.NoUserClaimsFound, null, null, null);
			}
			return dictionary;
		}

		private static void CollectIfNotNull(Dictionary<string, string> attributes, string attributeType, string attributeValue)
		{
			if (!string.IsNullOrEmpty(attributeValue))
			{
				attributes.Add(attributeType, attributeValue);
			}
		}

		public static OAuthActAsUser CreateFromMiniRecipient(OrganizationId organizationId, MiniRecipient recipient)
		{
			if (recipient == null)
			{
				return null;
			}
			return new OAuthActAsUser(organizationId, OAuthActAsUser.VerifiedUserInfo.FromADObject(recipient));
		}

		internal static OAuthActAsUser InternalCreateFromAttributes(OrganizationId organizationId, bool calledAtFrontEnd, Dictionary<string, string> rawAttributes, Dictionary<string, string> verifiedAttributes = null)
		{
			OAuthActAsUser oauthActAsUser;
			if (verifiedAttributes != null)
			{
				if (rawAttributes == null)
				{
					return new OAuthActAsUser(organizationId, OAuthActAsUser.VerifiedUserInfo.FromVerifiedAttributes(verifiedAttributes));
				}
				oauthActAsUser = new OAuthActAsUser.FrontEndVerifiedOAuthActAsUser(organizationId, rawAttributes, verifiedAttributes);
			}
			else if (rawAttributes.Count == 1)
			{
				string text = rawAttributes.Keys.First<string>();
				string text2 = rawAttributes[text];
				string a;
				if ((a = text) != null)
				{
					if (a == "sid")
					{
						oauthActAsUser = new OAuthActAsUser.SidOnlyActAsUser(organizationId, new SecurityIdentifier(text2));
						goto IL_89;
					}
					if (a == "oid")
					{
						oauthActAsUser = new OAuthActAsUser.OidOnlyActAsUser(organizationId, text2);
						goto IL_89;
					}
				}
				oauthActAsUser = new OAuthActAsUser(organizationId, rawAttributes);
			}
			else
			{
				oauthActAsUser = new OAuthActAsUser(organizationId, rawAttributes);
			}
			IL_89:
			if (!calledAtFrontEnd)
			{
				oauthActAsUser.VerifyUser();
			}
			return oauthActAsUser;
		}

		protected IRecipientSession GetRecipientSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 907, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\OAuthActAsUser.cs");
		}

		protected ITenantRecipientSession GetTenantRecipientSession()
		{
			return DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 919, "GetTenantRecipientSession", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\OAuthActAsUser.cs");
		}

		public override int GetHashCode()
		{
			return this.rawAttributeString.GetHashCode() ^ this.organizationId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			OAuthActAsUser oauthActAsUser = obj as OAuthActAsUser;
			return oauthActAsUser != null && this.rawAttributeString.Equals(oauthActAsUser.rawAttributeString) && this.organizationId.Equals(oauthActAsUser.organizationId);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.rawAttributeString))
			{
				return "actas1(" + this.rawAttributeString + ")";
			}
			return this.verifiedUser.ToString();
		}

		protected static readonly IEnumerable<PropertyDefinition> ADRawEntryPropertySet = new PropertyDefinition[]
		{
			ADObjectSchema.ExchangeVersion,
			ADObjectSchema.OrganizationId,
			ADMailboxRecipientSchema.Database,
			ADMailboxRecipientSchema.Sid,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.ExternalEmailAddress,
			ADUserSchema.UserPrincipalName,
			ADRecipientSchema.WindowsLiveID,
			ADUserSchema.NetID,
			ADRecipientSchema.MasterAccountSid,
			ADUserSchema.UserAccountControl,
			ADObjectSchema.DistinguishedName
		};

		private readonly OrganizationId organizationId;

		private readonly Dictionary<string, string> rawAttributes;

		private readonly string rawAttributeString;

		private OAuthActAsUser.VerifiedUserInfo verifiedUser;

		private static class UserAttributeType
		{
			public const string Sid = "sid";

			public const string ExternalDirectoryObjectId = "oid";

			public const string SmtpAddress = "smtp";

			public const string SipAddress = "sip";

			public const string NetId = "netid";

			public const string UserPrincipalName = "upn";

			public const string LiveId = "liveid";
		}

		protected class VerifiedUserInfo
		{
			private VerifiedUserInfo()
			{
			}

			public SecurityIdentifier Sid { get; private set; }

			public SecurityIdentifier MasterAccountSid { get; private set; }

			public string UserPrincipalName { get; private set; }

			public SmtpAddress WindowsLiveID { get; private set; }

			public string DistinguishedName { get; private set; }

			public ADRawEntry ADRawEntry { get; private set; }

			public Exception Exception { get; private set; }

			public static OAuthActAsUser.VerifiedUserInfo FromVerifiedAttributes(Dictionary<string, string> verifiedAttributes)
			{
				OAuthActAsUser.VerifiedUserInfo verifiedUserInfo = new OAuthActAsUser.VerifiedUserInfo();
				string userPrincipalName;
				if (verifiedAttributes.TryGetValue("UserPrincipalName", out userPrincipalName))
				{
					verifiedUserInfo.UserPrincipalName = userPrincipalName;
				}
				string address;
				if (verifiedAttributes.TryGetValue("MemberName", out address) && SmtpAddress.IsValidSmtpAddress(address))
				{
					verifiedUserInfo.WindowsLiveID = SmtpAddress.Parse(address);
				}
				string text;
				if (verifiedAttributes.TryGetValue("UserSid", out text))
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "FromVerifiedAttributes Sid value : {0}", text);
					verifiedUserInfo.Sid = new SecurityIdentifier(text);
				}
				string text2;
				if (verifiedAttributes.TryGetValue("MasterSid", out text2))
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "FromVerifiedAttributes MasterSid value : {0}", text2);
					verifiedUserInfo.MasterAccountSid = new SecurityIdentifier(text2);
				}
				string distinguishedName;
				if (verifiedAttributes.TryGetValue("UserDn", out distinguishedName))
				{
					verifiedUserInfo.DistinguishedName = distinguishedName;
				}
				return verifiedUserInfo;
			}

			public static OAuthActAsUser.VerifiedUserInfo FromADObject(ADRawEntry entry)
			{
				OAuthActAsUser.VerifiedUserInfo verifiedUserInfo = new OAuthActAsUser.VerifiedUserInfo();
				verifiedUserInfo.ADRawEntry = entry;
				verifiedUserInfo.Sid = (entry[ADMailboxRecipientSchema.Sid] as SecurityIdentifier);
				UserAccountControlFlags userAccountControlFlags = (UserAccountControlFlags)entry[ADUserSchema.UserAccountControl];
				if ((userAccountControlFlags & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.AccountDisabled)
				{
					verifiedUserInfo.MasterAccountSid = (entry[ADRecipientSchema.MasterAccountSid] as SecurityIdentifier);
				}
				verifiedUserInfo.UserPrincipalName = (entry[ADUserSchema.UserPrincipalName] as string);
				verifiedUserInfo.WindowsLiveID = (SmtpAddress)entry[ADRecipientSchema.WindowsLiveID];
				verifiedUserInfo.DistinguishedName = (entry[ADObjectSchema.DistinguishedName] as string);
				return verifiedUserInfo;
			}

			public static OAuthActAsUser.VerifiedUserInfo FromExceptionWithADOperationResult(ADOperationResult result)
			{
				ExTraceGlobals.OAuthTracer.TraceWarning<ADOperationErrorCode, Exception>(0L, "[VerifiedUserInfo] Failed to query AD with error code {0}, exception {1}", result.ErrorCode, result.Exception);
				InvalidOAuthTokenException exception = new InvalidOAuthTokenException(OAuthErrors.ADOperationFailed, null, result.Exception);
				return OAuthActAsUser.VerifiedUserInfo.FromException(exception);
			}

			public static OAuthActAsUser.VerifiedUserInfo FromExceptionWithNoUserFound(string queryFilter)
			{
				ExTraceGlobals.OAuthTracer.TraceWarning<string>(0L, "[VerifiedUserInfo] Did not find matched user with query {0}", queryFilter);
				InvalidOAuthTokenException exception = new InvalidOAuthTokenException(OAuthErrors.NoUserFoundWithGivenClaims, null, null)
				{
					ExtraData = queryFilter
				};
				return OAuthActAsUser.VerifiedUserInfo.FromException(exception);
			}

			public static OAuthActAsUser.VerifiedUserInfo FromExceptionWithAmbiguousUsersFound(QueryFilter queryFilter, string userDNs)
			{
				ExTraceGlobals.OAuthTracer.TraceWarning<string, QueryFilter>(0L, "[VerifiedUserInfo] Found more than 1 matched user: {0}. Queryfilter: {1}", userDNs, queryFilter);
				InvalidOAuthTokenException exception = new InvalidOAuthTokenException(OAuthErrors.MoreThan1UserFoundWithGivenClaims, null, null)
				{
					ExtraData = queryFilter + "+" + userDNs
				};
				return OAuthActAsUser.VerifiedUserInfo.FromException(exception);
			}

			public static OAuthActAsUser.VerifiedUserInfo FromException(Exception exception)
			{
				return new OAuthActAsUser.VerifiedUserInfo
				{
					Exception = exception
				};
			}

			public override string ToString()
			{
				return string.Format("actas2(dn:{0})", this.DistinguishedName);
			}
		}

		private sealed class VerifiedUserInfoResultCache : LazyLookupTimeoutCache<OAuthActAsUser, OAuthActAsUser.VerifiedUserInfo>
		{
			private VerifiedUserInfoResultCache() : base(2, OAuthActAsUser.VerifiedUserInfoResultCache.cacheSize.Value, false, OAuthActAsUser.VerifiedUserInfoResultCache.cacheTimeToLive.Value)
			{
			}

			public static OAuthActAsUser.VerifiedUserInfoResultCache Instance
			{
				get
				{
					return OAuthActAsUser.VerifiedUserInfoResultCache.instance;
				}
			}

			protected override OAuthActAsUser.VerifiedUserInfo CreateOnCacheMiss(OAuthActAsUser key, ref bool shouldAdd)
			{
				shouldAdd = true;
				return key.InternalVerifyUser();
			}

			private static readonly TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("OAuthUserCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(15.0), ExTraceGlobals.OAuthTracer);

			private static readonly IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("OAuthUserCacheMaxItems", 4000, ExTraceGlobals.OAuthTracer);

			private static OAuthActAsUser.VerifiedUserInfoResultCache instance = new OAuthActAsUser.VerifiedUserInfoResultCache();
		}

		private class SidOnlyActAsUser : OAuthActAsUser
		{
			public SidOnlyActAsUser(OrganizationId organizationId, SecurityIdentifier sid) : base(organizationId, "sid", sid.Value)
			{
				this.SecurityIdentifier = sid;
			}

			public SecurityIdentifier SecurityIdentifier { get; private set; }

			protected override OAuthActAsUser.VerifiedUserInfo InternalVerifyUser()
			{
				IRecipientSession recipientSession = base.GetRecipientSession();
				ADRawEntry recipient = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipient = recipientSession.FindADRawEntryBySid(this.SecurityIdentifier, OAuthActAsUser.ADRawEntryPropertySet);
				});
				if (adoperationResult != ADOperationResult.Success)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithADOperationResult(adoperationResult);
				}
				if (recipient == null)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithNoUserFound(this.SecurityIdentifier.Value);
				}
				return OAuthActAsUser.VerifiedUserInfo.FromADObject(recipient);
			}
		}

		private class OidOnlyActAsUser : OAuthActAsUser
		{
			public OidOnlyActAsUser(OrganizationId organizationId, string externalDirectoryObjectId) : base(organizationId, "oid", externalDirectoryObjectId)
			{
				this.ExternalDirectoryObjectId = externalDirectoryObjectId;
			}

			public string ExternalDirectoryObjectId { get; private set; }

			protected override OAuthActAsUser.VerifiedUserInfo InternalVerifyUser()
			{
				ITenantRecipientSession recipientSession = base.GetTenantRecipientSession();
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ExternalDirectoryObjectId, this.ExternalDirectoryObjectId);
				ADUser[] users = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					users = recipientSession.Find<ADUser>(null, QueryScope.SubTree, filter, null, 4);
				});
				if (adoperationResult != ADOperationResult.Success)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithADOperationResult(adoperationResult);
				}
				if (users == null || users.Length == 0)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithNoUserFound(this.ExternalDirectoryObjectId);
				}
				ADRawEntry adrawEntry = recipientSession.ChooseBetweenAmbiguousUsers(users);
				if (adrawEntry == null)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithAmbiguousUsersFound(filter, string.Join(",", from r in users
					select r.DistinguishedName));
				}
				return OAuthActAsUser.VerifiedUserInfo.FromADObject(adrawEntry);
			}
		}

		private class PuidOnlyActAsUser : OAuthActAsUser
		{
			public PuidOnlyActAsUser(OrganizationId organizationId, NetID puid) : base(organizationId, "netid", puid.ToString())
			{
				this.Puid = puid;
			}

			public NetID Puid { get; private set; }

			protected override OAuthActAsUser.VerifiedUserInfo InternalVerifyUser()
			{
				ITenantRecipientSession recipientSession = base.GetTenantRecipientSession();
				ADRawEntry recipient = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipient = recipientSession.FindUniqueEntryByNetID(this.Puid.ToString(), this.organizationId.OrganizationalUnit.Name, OAuthActAsUser.ADRawEntryPropertySet.ToArray<PropertyDefinition>());
				});
				if (adoperationResult != ADOperationResult.Success)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithADOperationResult(adoperationResult);
				}
				if (recipient == null)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithNoUserFound(this.Puid.ToString());
				}
				return OAuthActAsUser.VerifiedUserInfo.FromADObject(recipient);
			}
		}

		private class SmtpOnlyActAsUser : OAuthActAsUser
		{
			public SmtpOnlyActAsUser(OrganizationId organizationId, string smtp) : base(organizationId, "smtp", smtp)
			{
				this.Smtp = smtp;
			}

			public string Smtp { get; private set; }

			protected override OAuthActAsUser.VerifiedUserInfo InternalVerifyUser()
			{
				IRecipientSession recipientSession = base.GetTenantRecipientSession();
				ADRawEntry recipient = null;
				SmtpProxyAddress proxyaddress = new SmtpProxyAddress(this.Smtp, true);
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipient = recipientSession.FindByProxyAddress(proxyaddress, OAuthActAsUser.ADRawEntryPropertySet);
				});
				if (adoperationResult != ADOperationResult.Success)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithADOperationResult(adoperationResult);
				}
				if (recipient == null)
				{
					return OAuthActAsUser.VerifiedUserInfo.FromExceptionWithNoUserFound(this.Smtp);
				}
				return OAuthActAsUser.VerifiedUserInfo.FromADObject(recipient);
			}
		}

		private class FrontEndVerifiedOAuthActAsUser : OAuthActAsUser
		{
			public FrontEndVerifiedOAuthActAsUser(OrganizationId organizationId, Dictionary<string, string> rawAttributes, Dictionary<string, string> verifiedAttributes) : base(organizationId, rawAttributes)
			{
				this.verified = verifiedAttributes;
			}

			protected override OAuthActAsUser.VerifiedUserInfo InternalVerifyUser()
			{
				return OAuthActAsUser.VerifiedUserInfo.FromVerifiedAttributes(this.verified);
			}

			private Dictionary<string, string> verified;
		}
	}
}
