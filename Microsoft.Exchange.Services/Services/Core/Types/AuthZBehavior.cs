using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class AuthZBehavior
	{
		public virtual bool IsAllowedToCallWebMethod(WebMethodEntry webMethodEntry)
		{
			return !webMethodEntry.IsPartnerAppOnly;
		}

		public virtual bool IsAllowedToOpenMailbox(ExchangePrincipal mailboxToAccess)
		{
			return true;
		}

		public virtual bool IsAllowedToPrivilegedOpenMailbox(ExchangePrincipal mailboxToAccess)
		{
			return false;
		}

		public virtual bool IsAllowedToOptimizeMoveCopyCommand()
		{
			return true;
		}

		public virtual RequestedLogonType GetMailboxLogonType()
		{
			return RequestedLogonType.Default;
		}

		public virtual void OnCreateMessageItem(bool isAssociated)
		{
		}

		public virtual void OnUpdateMessageItem(MessageItem messageItem)
		{
		}

		public virtual void OnGetAttachment(StoreObjectId itemId)
		{
		}

		public virtual void OnGetItem(StoreObjectId itemId)
		{
		}

		public static AuthZBehavior DefaultBehavior = new AuthZBehavior();

		internal sealed class PrivilegedSessionAuthZBehavior : AuthZBehavior
		{
			public PrivilegedSessionAuthZBehavior(RequestedLogonType logonType)
			{
				this.requestedLogonType = logonType;
			}

			public override bool IsAllowedToPrivilegedOpenMailbox(ExchangePrincipal mailboxToAccess)
			{
				return true;
			}

			public override RequestedLogonType GetMailboxLogonType()
			{
				return this.requestedLogonType;
			}

			private readonly RequestedLogonType requestedLogonType;
		}

		internal class AdminRoleUserAuthZBehavior : AuthZBehavior
		{
			public AdminRoleUserAuthZBehavior(AuthZClientInfo authZClientInfo)
			{
				SecurityIdentifier userSid = authZClientInfo.ClientSecurityContext.UserSid;
				SidWithGroupsIdentity sidIdentity = new SidWithGroupsIdentity(userSid.ToString(), string.Empty, authZClientInfo.ClientSecurityContext);
				this.runspaceConfig = AuthZBehavior.WebServiceRunspaceConfigurationCache.Singleton.Get(sidIdentity);
				if (this.runspaceConfig == null)
				{
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageUnableToLoadRBACSettings);
				}
				this.userRoleTypes = authZClientInfo.UserRoleTypes;
				foreach (RoleType roleType in this.userRoleTypes)
				{
					if (!this.runspaceConfig.HasRoleOfType(roleType))
					{
						ExTraceGlobals.AuthorizationTracer.TraceDebug<SecurityIdentifier, RoleType>(0L, "AdminRoleUserAuthZBehavior.ctor: user with sid {0} has no role {1} assigned.", userSid, roleType);
						throw new ServiceAccessDeniedException(CoreResources.IDs.MessageCallerHasNoAdminRoleGranted);
					}
				}
			}

			public override bool IsAllowedToCallWebMethod(WebMethodEntry webMethodEntry)
			{
				string methodName = webMethodEntry.Name;
				return this.userRoleTypes.Any((RoleType roleType) => this.runspaceConfig.IsWebMethodInRole(methodName, roleType));
			}

			public override bool IsAllowedToPrivilegedOpenMailbox(ExchangePrincipal mailboxToAccess)
			{
				ADSessionSettings adsessionSettings = mailboxToAccess.MailboxInfo.OrganizationId.ToADSessionSettings();
				adsessionSettings.IncludeInactiveMailbox = true;
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, adsessionSettings, 133, "IsAllowedToPrivilegedOpenMailbox", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\Types\\AuthZBehavior.cs");
				foreach (RoleType roleType in this.userRoleTypes)
				{
					if (ExchangeRole.IsAdminRole(roleType) && this.runspaceConfig.IsTargetObjectInRoleScope(roleType, DirectoryHelper.ReadADRecipient(mailboxToAccess.MailboxInfo.MailboxGuid, mailboxToAccess.MailboxInfo.IsArchive, tenantOrRootOrgRecipientSession)))
					{
						return true;
					}
				}
				return false;
			}

			public override RequestedLogonType GetMailboxLogonType()
			{
				foreach (RoleType roleToCheck in this.userRoleTypes)
				{
					if (ExchangeRole.IsAdminRole(roleToCheck))
					{
						return RequestedLogonType.AdminFromManagementRoleHeader;
					}
				}
				return base.GetMailboxLogonType();
			}

			public RoleType[] UserRoleTypes
			{
				get
				{
					return this.userRoleTypes;
				}
			}

			private readonly RoleType[] userRoleTypes;

			private readonly WebServiceRunspaceConfiguration runspaceConfig;
		}

		internal class OfficeExtensionRoleAuthZBehavior : AuthZBehavior
		{
			public OfficeExtensionRoleAuthZBehavior(OAuthIdentity identity)
			{
				this.identity = identity;
			}

			public override bool IsAllowedToCallWebMethod(WebMethodEntry webMethodEntry)
			{
				return PartnerApplicationRunspaceConfiguration.IsWebMethodInOfficeExtensionRole(webMethodEntry.Name);
			}

			public override bool IsAllowedToOpenMailbox(ExchangePrincipal mailboxToAccess)
			{
				ExTraceGlobals.AuthorizationTracer.TraceDebug<SecurityIdentifier, SecurityIdentifier>(0L, "OfficeExtensionRoleAuthZBehavior.IsAllowedToOpenMailbox: act-as user sid {0}, mailbox-to-open sid {1}", this.identity.ActAsUser.Sid, mailboxToAccess.Sid);
				return this.identity.ActAsUser.Sid.Equals(mailboxToAccess.Sid);
			}

			public override void OnCreateMessageItem(bool isAssociated)
			{
				if (isAssociated)
				{
					ExTraceGlobals.AuthorizationTracer.TraceDebug(0L, "OfficeExtensionRoleAuthZBehavior.OnCreateMessageItem: trying to create FAI, throw");
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageExtensionNotAllowedToCreateFAI);
				}
			}

			public override void OnUpdateMessageItem(MessageItem messageItem)
			{
				MessageFlags messageFlags = (MessageFlags)messageItem[MessageItemSchema.Flags];
				if ((messageFlags & MessageFlags.IsAssociated) == MessageFlags.IsAssociated)
				{
					ExTraceGlobals.AuthorizationTracer.TraceDebug(0L, "OfficeExtensionRoleAuthZBehavior.OnUpdateMessageItem: trying to update FAI, throw");
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageExtensionNotAllowedToUpdateFAI);
				}
			}

			public override bool IsAllowedToOptimizeMoveCopyCommand()
			{
				return false;
			}

			protected readonly OAuthIdentity identity;
		}

		internal class ScopedOfficeExtensionRoleAuthZbehavior : AuthZBehavior.OfficeExtensionRoleAuthZBehavior
		{
			public ScopedOfficeExtensionRoleAuthZbehavior(OAuthIdentity identity) : base(identity)
			{
				if (!identity.IsOfficeExtension || !identity.OfficeExtension.IsScopedToken)
				{
					throw new ServiceAccessDeniedException((CoreResources.IDs)3579904699U);
				}
				this.scope = identity.OfficeExtension.Scope;
			}

			public override bool IsAllowedToCallWebMethod(WebMethodEntry webMethodEntry)
			{
				bool flag = "GetAttachment".Equals(webMethodEntry.Name, StringComparison.OrdinalIgnoreCase) || "GetItem".Equals(webMethodEntry.Name, StringComparison.OrdinalIgnoreCase);
				if (!flag)
				{
					MSDiagnosticsHeader.AppendToResponse(OAuthErrorCategory.InvalidGrant, "The web method in the request is not allowed with this token.", null);
				}
				return flag;
			}

			public override void OnGetAttachment(StoreObjectId itemId)
			{
				this.CheckItemIdMatch(itemId, "The specified attachment id does not correspond to an attachment on the item that the token is scoped for.");
			}

			public override void OnGetItem(StoreObjectId itemId)
			{
				this.CheckItemIdMatch(itemId, "The specified item id does not correspond to an item that the token is scoped for.");
			}

			private void CheckItemIdMatch(StoreObjectId itemId, string errorMessage)
			{
				try
				{
					if (this.scope.IndexOf("ParentItemId:", StringComparison.OrdinalIgnoreCase) < 0 || !IdConverter.EwsIdToMessageStoreObjectId(this.scope.Remove(0, "ParentItemId:".Length)).Equals(itemId))
					{
						MSDiagnosticsHeader.AppendToResponse(OAuthErrorCategory.InvalidGrant, errorMessage, null);
						throw new ServiceAccessDeniedException((CoreResources.IDs)3579904699U);
					}
				}
				catch (InvalidStoreIdException innerException)
				{
					MSDiagnosticsHeader.AppendToResponse(OAuthErrorCategory.InvalidGrant, errorMessage, null);
					throw new ServiceAccessDeniedException((CoreResources.IDs)3579904699U, innerException);
				}
			}

			private const string GetAttachmentWebMethodName = "GetAttachment";

			private const string GetItemWebMethodName = "GetItem";

			private const string ParentItemIdScopeName = "ParentItemId:";

			private readonly string scope;
		}

		internal abstract class ApplicationAuthZBehaviorBase : AuthZBehavior
		{
			public ApplicationAuthZBehaviorBase(OAuthIdentity identity, AuthZBehavior userAuthZBehavior, AuthZBehavior.ActAsUserRequirement actAsUserRequirement, RoleType applicationRoleType)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<AuthZBehavior.ActAsUserRequirement>(2328243517U, ref actAsUserRequirement);
				switch (actAsUserRequirement)
				{
				case AuthZBehavior.ActAsUserRequirement.MustPresent:
					if (identity.IsAppOnly)
					{
						throw new ServiceAccessDeniedException(CoreResources.IDs.MessageActAsUserRequiredForSuchApplicationRole);
					}
					break;
				case AuthZBehavior.ActAsUserRequirement.MustNotPresent:
					if (!identity.IsAppOnly)
					{
						throw new ServiceAccessDeniedException(CoreResources.IDs.MessageApplicationTokenOnly);
					}
					break;
				}
				this.oAuthIdentity = identity;
				this.applicationRoleType = applicationRoleType;
				this.userAuthZBehavior = userAuthZBehavior;
				this.applicationRunspaceConfig = AuthZBehavior.WebServiceRunspaceConfigurationCache.Singleton.Get(identity.OAuthApplication.PartnerApplication);
				if (this.applicationRunspaceConfig == null)
				{
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageUnableToLoadRBACSettings);
				}
				if (!this.applicationRunspaceConfig.HasRoleOfType(applicationRoleType))
				{
					throw new ServiceAccessDeniedException((applicationRoleType == RoleType.UserApplication) ? CoreResources.IDs.MessageApplicationHasNoUserApplicationRoleAssigned : ((CoreResources.IDs)3901728717U));
				}
				if (!this.IsAppAllowedToActAsUser())
				{
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageApplicationUnableActAsUser);
				}
				AuthZBehavior.AdminRoleUserAuthZBehavior adminRoleUserAuthZBehavior = userAuthZBehavior as AuthZBehavior.AdminRoleUserAuthZBehavior;
				if (adminRoleUserAuthZBehavior != null)
				{
					foreach (RoleType roleType in adminRoleUserAuthZBehavior.UserRoleTypes)
					{
						if (!this.applicationRunspaceConfig.HasRoleOfType(roleType))
						{
							throw new ServiceAccessDeniedException((CoreResources.IDs)3607262778U);
						}
					}
				}
			}

			public override bool IsAllowedToCallWebMethod(WebMethodEntry webMethodEntry)
			{
				return this.applicationRunspaceConfig.IsWebMethodInRole(webMethodEntry.Name, this.applicationRoleType);
			}

			public abstract bool IsAppAllowedToActAsUser();

			protected PartnerApplicationRunspaceConfiguration applicationRunspaceConfig;

			protected OAuthIdentity oAuthIdentity;

			protected RoleType applicationRoleType;

			protected AuthZBehavior userAuthZBehavior;
		}

		internal sealed class UserApplicationRoleAuthZBehavior : AuthZBehavior.ApplicationAuthZBehaviorBase
		{
			public UserApplicationRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustPresent, RoleType.UserApplication)
			{
			}

			public override bool IsAppAllowedToActAsUser()
			{
				if (this.oAuthIdentity.IsAppOnly)
				{
					return true;
				}
				if (this.oAuthIdentity.ActAsUser.Sid != null)
				{
					return this.applicationRunspaceConfig.IsTargetObjectInRoleScope(this.applicationRoleType, this.oAuthIdentity.ADRecipient);
				}
				ExTraceGlobals.AuthorizationTracer.TraceDebug(0L, "UserApplicationRoleAuthZBehavior.IsAppAllowedToActAsUser: act-as user has no SID");
				return this.oAuthIdentity.IsKnownFromSameOrgExchange;
			}
		}

		internal abstract class PrivilegedApplicationAuthZBehaviorBase : AuthZBehavior.ApplicationAuthZBehaviorBase
		{
			public PrivilegedApplicationAuthZBehaviorBase(OAuthIdentity identity, AuthZBehavior userAuthZBehavior, AuthZBehavior.ActAsUserRequirement actAsUserRequirement, RoleType applicationRoleType, RequestedLogonType requestedLogonType) : base(identity, userAuthZBehavior, actAsUserRequirement, applicationRoleType)
			{
				this.requestedLogonType = requestedLogonType;
			}

			public override RequestedLogonType GetMailboxLogonType()
			{
				return this.requestedLogonType;
			}

			public override bool IsAppAllowedToActAsUser()
			{
				return true;
			}

			public override bool IsAllowedToPrivilegedOpenMailbox(ExchangePrincipal mailboxToAccess)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, mailboxToAccess.MailboxInfo.OrganizationId.ToADSessionSettings(), 552, "IsAllowedToPrivilegedOpenMailbox", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\Types\\AuthZBehavior.cs");
				return this.applicationRunspaceConfig.IsTargetObjectInRoleScope(this.applicationRoleType, DirectoryHelper.ReadADRecipient(mailboxToAccess.MailboxInfo.MailboxGuid, mailboxToAccess.MailboxInfo.IsArchive, tenantOrRootOrgRecipientSession));
			}

			protected RequestedLogonType requestedLogonType;
		}

		internal sealed class LegalHoldApplicationRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public LegalHoldApplicationRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustNotPresent, RoleType.LegalHoldApplication, RequestedLogonType.AdminFromManagementRoleHeader)
			{
			}
		}

		internal sealed class MailboxSearchApplicationRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public MailboxSearchApplicationRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustNotPresent, RoleType.MailboxSearchApplication, RequestedLogonType.AdminFromManagementRoleHeader)
			{
			}
		}

		internal sealed class ArchiveApplicationRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public ArchiveApplicationRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustPresent, RoleType.ArchiveApplication, RequestedLogonType.SystemServiceFromManagementRoleHeader)
			{
			}
		}

		internal sealed class MailboxSearchRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public MailboxSearchRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustPresent, RoleType.MailboxSearch, RequestedLogonType.AdminFromManagementRoleHeader)
			{
				this.adminRoleUserAuthZBehavior = (userAuthZBehavior as AuthZBehavior.AdminRoleUserAuthZBehavior);
			}

			public override bool IsAllowedToPrivilegedOpenMailbox(ExchangePrincipal mailboxToAccess)
			{
				return base.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess) && this.adminRoleUserAuthZBehavior.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess);
			}

			private AuthZBehavior.AdminRoleUserAuthZBehavior adminRoleUserAuthZBehavior;
		}

		internal sealed class LegalHoldRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public LegalHoldRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustPresent, RoleType.LegalHold, RequestedLogonType.AdminFromManagementRoleHeader)
			{
				this.adminRoleUserAuthZBehavior = (userAuthZBehavior as AuthZBehavior.AdminRoleUserAuthZBehavior);
			}

			public override bool IsAllowedToPrivilegedOpenMailbox(ExchangePrincipal mailboxToAccess)
			{
				return base.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess) && this.adminRoleUserAuthZBehavior.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess);
			}

			private AuthZBehavior.AdminRoleUserAuthZBehavior adminRoleUserAuthZBehavior;
		}

		internal sealed class TeamMailboxLifecycleApplicationRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public TeamMailboxLifecycleApplicationRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustNotPresent, RoleType.TeamMailboxLifecycleApplication, RequestedLogonType.Default)
			{
			}
		}

		internal sealed class ExchangeCrossServiceIntegrationRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public ExchangeCrossServiceIntegrationRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.Either, RoleType.ExchangeCrossServiceIntegration, RequestedLogonType.AdminFromManagementRoleHeader)
			{
			}
		}

		internal sealed class OrganizationConfigurationRoleAuthZBehavior : AuthZBehavior.PrivilegedApplicationAuthZBehaviorBase
		{
			public OrganizationConfigurationRoleAuthZBehavior(OAuthIdentity identity, AuthZBehavior userAuthZBehavior) : base(identity, userAuthZBehavior, AuthZBehavior.ActAsUserRequirement.MustPresent, RoleType.OrganizationConfiguration, RequestedLogonType.AdminFromManagementRoleHeader)
			{
				this.adminRoleUserAuthZBehavior = (userAuthZBehavior as AuthZBehavior.AdminRoleUserAuthZBehavior);
			}

			public override bool IsAllowedToPrivilegedOpenMailbox(ExchangePrincipal mailboxToAccess)
			{
				return base.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess) && this.adminRoleUserAuthZBehavior.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess);
			}

			private AuthZBehavior.AdminRoleUserAuthZBehavior adminRoleUserAuthZBehavior;
		}

		internal sealed class WebServiceRunspaceConfigurationCache : LazyLookupTimeoutCache<AuthZBehavior.WebServiceRunspaceConfigurationCache.CacheKey, WebServiceRunspaceConfiguration>
		{
			private WebServiceRunspaceConfigurationCache() : base(1, AuthZBehavior.WebServiceRunspaceConfigurationCache.cacheSize.Value, false, AuthZBehavior.WebServiceRunspaceConfigurationCache.cacheTimeToLive.Value)
			{
			}

			public static AuthZBehavior.WebServiceRunspaceConfigurationCache Singleton
			{
				get
				{
					if (AuthZBehavior.WebServiceRunspaceConfigurationCache.singleton == null)
					{
						lock (AuthZBehavior.WebServiceRunspaceConfigurationCache.lockObj)
						{
							if (AuthZBehavior.WebServiceRunspaceConfigurationCache.singleton == null)
							{
								AuthZBehavior.WebServiceRunspaceConfigurationCache.singleton = new AuthZBehavior.WebServiceRunspaceConfigurationCache();
							}
						}
					}
					return AuthZBehavior.WebServiceRunspaceConfigurationCache.singleton;
				}
			}

			public PartnerApplicationRunspaceConfiguration Get(PartnerApplication partnerApplication)
			{
				return base.Get(new AuthZBehavior.WebServiceRunspaceConfigurationCache.PartnerApplicationRunspaceConfigurationCacheKey(partnerApplication)) as PartnerApplicationRunspaceConfiguration;
			}

			public WebServiceRunspaceConfiguration Get(SidWithGroupsIdentity sidIdentity)
			{
				return base.Get(new AuthZBehavior.WebServiceRunspaceConfigurationCache.WebServiceRunspaceConfigurationCacheKey(sidIdentity));
			}

			protected override WebServiceRunspaceConfiguration CreateOnCacheMiss(AuthZBehavior.WebServiceRunspaceConfigurationCache.CacheKey key, ref bool shouldAdd)
			{
				try
				{
					shouldAdd = true;
					return key.Create();
				}
				catch (CmdletAccessDeniedException ex)
				{
					ExTraceGlobals.AuthorizationTracer.TraceWarning<AuthZBehavior.WebServiceRunspaceConfigurationCache.CacheKey, CmdletAccessDeniedException>(0L, "WebServiceRunspaceConfigurationCache.CreateOnCacheMiss: hit CmdletAccessDeniedException for key {0}, exception detail: {1} ", key, ex);
					RequestDetailsLogger.Current.AppendGenericInfo("WsrcKey", key);
					RequestDetailsLogger.Current.AppendGenericInfo("WsrcException", ex);
				}
				shouldAdd = false;
				return null;
			}

			private static readonly object lockObj = new object();

			private static AuthZBehavior.WebServiceRunspaceConfigurationCache singleton = null;

			private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("WebServiceRunspaceConfigurationCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(30.0), ExTraceGlobals.AuthorizationTracer);

			private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("WebServiceRunspaceConfigurationCacheMaxItems", 50, ExTraceGlobals.AuthorizationTracer);

			internal abstract class CacheKey
			{
				public abstract WebServiceRunspaceConfiguration Create();
			}

			internal sealed class WebServiceRunspaceConfigurationCacheKey : AuthZBehavior.WebServiceRunspaceConfigurationCache.CacheKey
			{
				public WebServiceRunspaceConfigurationCacheKey(SidWithGroupsIdentity sidIdentity)
				{
					this.identity = sidIdentity;
					this.keyString = AuthZBehavior.WebServiceRunspaceConfigurationCache.WebServiceRunspaceConfigurationCacheKey.sidPrefix + sidIdentity.Sid.ToString();
				}

				public override WebServiceRunspaceConfiguration Create()
				{
					ExTraceGlobals.AuthorizationTracer.TraceDebug<IIdentity>(0L, "WebServiceRunspaceConfigurationCacheKey.Create: create runspace configuration for user identity {0}", this.identity);
					return new WebServiceRunspaceConfiguration(this.identity);
				}

				public override bool Equals(object obj)
				{
					AuthZBehavior.WebServiceRunspaceConfigurationCache.WebServiceRunspaceConfigurationCacheKey webServiceRunspaceConfigurationCacheKey = obj as AuthZBehavior.WebServiceRunspaceConfigurationCache.WebServiceRunspaceConfigurationCacheKey;
					return webServiceRunspaceConfigurationCacheKey != null && this.keyString.Equals(webServiceRunspaceConfigurationCacheKey.keyString);
				}

				public override int GetHashCode()
				{
					return this.keyString.GetHashCode();
				}

				public override string ToString()
				{
					return this.keyString;
				}

				private static readonly string sidPrefix = "sid~";

				private readonly string keyString;

				private readonly IIdentity identity;
			}

			internal sealed class PartnerApplicationRunspaceConfigurationCacheKey : AuthZBehavior.WebServiceRunspaceConfigurationCache.CacheKey
			{
				public PartnerApplicationRunspaceConfigurationCacheKey(PartnerApplication partnerApplication)
				{
					this.partnerApplication = partnerApplication;
				}

				public override WebServiceRunspaceConfiguration Create()
				{
					ExTraceGlobals.AuthorizationTracer.TraceDebug<PartnerApplication>(0L, "PartnerApplicationRunspaceConfigurationCacheKey.Create: create runspace configuration for partner application {0}", this.partnerApplication);
					return PartnerApplicationRunspaceConfiguration.Create(this.partnerApplication);
				}

				public override bool Equals(object obj)
				{
					AuthZBehavior.WebServiceRunspaceConfigurationCache.PartnerApplicationRunspaceConfigurationCacheKey partnerApplicationRunspaceConfigurationCacheKey = obj as AuthZBehavior.WebServiceRunspaceConfigurationCache.PartnerApplicationRunspaceConfigurationCacheKey;
					return partnerApplicationRunspaceConfigurationCacheKey != null && this.partnerApplication.Id.Equals(partnerApplicationRunspaceConfigurationCacheKey.partnerApplication.Id);
				}

				public override int GetHashCode()
				{
					return this.partnerApplication.Id.GetHashCode();
				}

				public override string ToString()
				{
					return this.partnerApplication.Id.ToString();
				}

				private readonly PartnerApplication partnerApplication;
			}
		}

		internal enum ActAsUserRequirement
		{
			MustPresent,
			MustNotPresent,
			Either
		}
	}
}
