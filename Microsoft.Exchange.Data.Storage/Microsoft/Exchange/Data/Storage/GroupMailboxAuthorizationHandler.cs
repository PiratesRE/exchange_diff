using System;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMailboxAuthorizationHandler
	{
		public GroupMailboxAuthorizationHandler(IExchangePrincipal groupMailboxToLogon, AccessingUserInfo accessingUserInfo, string clientInfoString, ClientSecurityContext clientSecurityContext, VariantConfigurationSnapshot variantConfig)
		{
			ArgumentValidator.ThrowIfNull("groupMailboxToLogon", groupMailboxToLogon);
			ArgumentValidator.ThrowIfInvalidValue<IExchangePrincipal>("groupMailboxToLogon.ModernGroupType", groupMailboxToLogon, (IExchangePrincipal x) => x.ModernGroupType != ModernGroupObjectType.None);
			ArgumentValidator.ThrowIfNull("accessingUserInfo", accessingUserInfo);
			ArgumentValidator.ThrowIfNullOrEmpty("clientInfoString", clientInfoString);
			ArgumentValidator.ThrowIfNull("clientSecurityContext", clientSecurityContext);
			this.accessingUserInfo = accessingUserInfo;
			this.groupMailboxToLogon = groupMailboxToLogon;
			this.clientInfoString = clientInfoString;
			this.IsOnGroupAuthForOlkDesktop = (variantConfig != null && variantConfig.DataStorage.GroupsForOlkDesktop.Enabled);
			this.clientSecurityContext = clientSecurityContext;
			this.organizationId = groupMailboxToLogon.MailboxInfo.OrganizationId;
		}

		public bool IsOnGroupAuthForOlkDesktop { get; set; }

		public UnifiedGroupMemberType GetMembershipType()
		{
			if (this.DenyCurrentLogon())
			{
				GroupMailboxAuthorizationHandler.Tracer.TraceWarning<string>(0L, "Not OWA logon and flight not enabled for group. ClientInfoString:{0} ", this.clientInfoString);
				return UnifiedGroupMemberType.Unknown;
			}
			IRecipientSession recipientSession = AccessingUserInfo.GetRecipientSession(this.organizationId);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfInvalidValue<IRecipientSession>("recipientSession.ReadOnly", recipientSession, (IRecipientSession x) => x.ReadOnly);
			if (this.accessingUserInfo.UserObjectId != null)
			{
				ADRecipient adrecipient = recipientSession.FindByExchangeGuid(this.groupMailboxToLogon.MailboxInfo.MailboxGuid);
				if (adrecipient != null)
				{
					recipientSession.DomainController = adrecipient.OriginatingServer;
					MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)adrecipient[ADUserSchema.Owners];
					if (multiValuedProperty.Contains(this.accessingUserInfo.UserObjectId))
					{
						this.TraceMembershipType(UnifiedGroupMemberType.Owner);
						return UnifiedGroupMemberType.Owner;
					}
				}
				if (this.groupMailboxToLogon.ModernGroupType == ModernGroupObjectType.Public)
				{
					this.TraceMembershipType(UnifiedGroupMemberType.Member);
					return UnifiedGroupMemberType.Member;
				}
				if (this.groupMailboxToLogon.ModernGroupType == ModernGroupObjectType.Private)
				{
					QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, IUnifiedGroupMailboxSchema.UnifiedGroupMembersLink, this.accessingUserInfo.UserObjectId);
					QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, this.groupMailboxToLogon.ObjectId);
					ADRawEntry[] array = recipientSession.Find(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
					{
						queryFilter2,
						queryFilter
					}), null, 0, GroupMailboxAuthorizationHandler.PropertiesToReturnForGroupQuery);
					if (array != null && array.Length == 1)
					{
						this.TraceMembershipType(UnifiedGroupMemberType.Member);
						return UnifiedGroupMemberType.Member;
					}
				}
			}
			else
			{
				GroupMailboxAuthorizationHandler.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to retrieve object identity of user {0} from AD", this.accessingUserInfo.Identity);
			}
			if (this.groupMailboxToLogon.ModernGroupType == ModernGroupObjectType.Public)
			{
				this.TraceMembershipType(UnifiedGroupMemberType.Member);
				return UnifiedGroupMemberType.Member;
			}
			if (this.groupMailboxToLogon.ModernGroupType == ModernGroupObjectType.Private && this.IsGroupMemberInMailboxAssociation())
			{
				this.TraceMembershipType(UnifiedGroupMemberType.Member);
				return UnifiedGroupMemberType.Member;
			}
			this.TraceMembershipType(UnifiedGroupMemberType.None);
			return UnifiedGroupMemberType.None;
		}

		public static SidBinaryAndAttributes[] GetGroupMailboxSidBinaryAndAttributes(Guid groupMailboxGuid, SecurityIdentity.GroupMailboxMemberType groupMailboxMemberType)
		{
			SidBinaryAndAttributes[] result = null;
			try
			{
				SecurityIdentifier groupSecurityIdentifier = SecurityIdentity.GetGroupSecurityIdentifier(groupMailboxGuid, groupMailboxMemberType);
				result = new SidBinaryAndAttributes[]
				{
					new SidBinaryAndAttributes(groupSecurityIdentifier, 4U)
				};
			}
			catch (ArgumentException arg)
			{
				GroupMailboxAuthorizationHandler.Tracer.TraceError<Guid, SecurityIdentity.GroupMailboxMemberType, ArgumentException>(0L, "Unable to construct SidBinaryAndAttributes for group mailbox {0} and user type {1} : {2}", groupMailboxGuid, groupMailboxMemberType, arg);
				throw new ObjectNotFoundException(ServerStrings.GroupMailboxAccessSidConstructionFailed(groupMailboxGuid, groupMailboxMemberType.ToString()));
			}
			return result;
		}

		private bool DenyCurrentLogon()
		{
			return !this.clientInfoString.Equals("Client=WebServices;ExchangeInternalEwsClient-EwsFanoutClient;", StringComparison.OrdinalIgnoreCase) && !ClientInfo.OWA.IsMatch(this.clientInfoString) && (!ClientInfo.MOMT.IsMatch(this.clientInfoString) || !this.IsOnGroupAuthForOlkDesktop);
		}

		private bool IsPublicToGroupSidInUserToken()
		{
			bool flag = false;
			Exception ex = null;
			int num = this.groupMailboxToLogon.PublicToGroupSids.Count<SecurityIdentifier>();
			if (num > 0)
			{
				try
				{
					int num2 = 0;
					RawAcl rawAcl = new RawAcl(0, num);
					foreach (SecurityIdentifier sid in this.groupMailboxToLogon.PublicToGroupSids)
					{
						rawAcl.InsertAce(num2++, new CommonAce(AceFlags.None, AceQualifier.AccessAllowed, 131072, sid, false, null));
					}
					RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor(ControlFlags.DiscretionaryAclPresent, GroupMailboxAuthorizationHandler.LocalSystemSid, GroupMailboxAuthorizationHandler.LocalSystemSid, null, rawAcl);
					int grantedAccess = this.clientSecurityContext.GetGrantedAccess(rawSecurityDescriptor, (AccessMask)131072);
					flag = (grantedAccess == 131072);
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				catch (AuthzException ex3)
				{
					ex = ex3;
				}
				catch (OutOfMemoryException ex4)
				{
					ex = ex4;
				}
			}
			if (ex != null)
			{
				GroupMailboxAuthorizationHandler.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Unable to verify that the user's token contain publicToGroupSid: {0}", ex);
			}
			if (flag)
			{
				GroupMailboxAuthorizationHandler.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Found that the user {0} is part of a security group present in PublicToGroupSids", this.accessingUserInfo.Identity);
			}
			else
			{
				GroupMailboxAuthorizationHandler.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Found that the user {0} is not part of any security group present in PublicToGroupSids", this.accessingUserInfo.Identity);
			}
			return flag;
		}

		private bool IsGroupMemberInMailboxAssociation()
		{
			IExtensibleLogger logger = MailboxAssociationDiagnosticsFrameFactory.Default.CreateLogger(this.groupMailboxToLogon.MailboxInfo.MailboxGuid, this.organizationId);
			IMailboxAssociationPerformanceTracker performanceTracker = MailboxAssociationDiagnosticsFrameFactory.Default.CreatePerformanceTracker(null);
			bool result;
			using (MailboxAssociationDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("XSO", "CheckMembership", logger, performanceTracker))
			{
				try
				{
					using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.groupMailboxToLogon, CultureInfo.InvariantCulture, this.clientInfoString))
					{
						using (IAssociationStore associationStore = this.GetAssociationStore(mailboxSession, performanceTracker))
						{
							using (IMailboxAssociationUser memberAssociationInformation = this.GetMemberAssociationInformation(associationStore))
							{
								result = (memberAssociationInformation != null && (bool)memberAssociationInformation[MailboxAssociationBaseSchema.IsMember]);
							}
						}
					}
				}
				catch (AssociationNotFoundException)
				{
					GroupMailboxAuthorizationHandler.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "MemberRetrieval: Unable to make IsMember() check for the user {0} on the mailbox {1}", this.accessingUserInfo.Identity, this.groupMailboxToLogon.MailboxInfo.DisplayName);
					result = false;
				}
				catch (LocalizedException)
				{
					GroupMailboxAuthorizationHandler.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "MemberRetrieval: Unable to make IsMember() check for the user {0} on the mailbox {1}", this.accessingUserInfo.Identity, this.groupMailboxToLogon.MailboxInfo.DisplayName);
					result = false;
				}
			}
			return result;
		}

		private LocalAssociationStore GetAssociationStore(MailboxSession adminMailboxSession, IMailboxAssociationPerformanceTracker performanceTracker)
		{
			IRecipientSession adrecipientSession = adminMailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
			IMailboxLocator mailboxLocator = new GroupMailboxLocator(adrecipientSession, this.groupMailboxToLogon.ExternalDirectoryObjectId, this.groupMailboxToLogon.LegacyDn);
			return new LocalAssociationStore(mailboxLocator, adminMailboxSession, false, new XSOFactory(), performanceTracker, MailboxAssociationDiagnosticsFrameFactory.Default.CreateLogger(this.groupMailboxToLogon.MailboxInfo.MailboxGuid, this.organizationId));
		}

		private IMailboxAssociationUser GetMemberAssociationInformation(IAssociationStore associationStore)
		{
			GroupMailboxAuthorizationHandler.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "MemberRetrieval: Retrieving member information {0} from the mailbox = {1}", this.accessingUserInfo.Identity, this.groupMailboxToLogon.MailboxInfo.DisplayName);
			IMailboxAssociationUser userAssociationByIdProperty;
			if (!string.IsNullOrEmpty(this.accessingUserInfo.ExternalDirectoryObjectId))
			{
				userAssociationByIdProperty = associationStore.GetUserAssociationByIdProperty(MailboxAssociationBaseSchema.ExternalId, new object[]
				{
					this.accessingUserInfo.ExternalDirectoryObjectId
				});
			}
			else
			{
				userAssociationByIdProperty = associationStore.GetUserAssociationByIdProperty(MailboxAssociationBaseSchema.LegacyDN, new object[]
				{
					this.accessingUserInfo.LegacyExchangeDN
				});
			}
			return userAssociationByIdProperty;
		}

		private void TraceMembershipType(UnifiedGroupMemberType groupMemberType)
		{
			GroupMailboxAuthorizationHandler.Tracer.TraceDebug<string, UnifiedGroupMemberType>((long)this.GetHashCode(), "Determined the membership type of accessing user {0} to be {1}.", this.accessingUserInfo.Identity, groupMemberType);
		}

		internal static void AddGroupMailboxAccessSid(ClientSecurityContext clientSecurityContext, Guid groupMailboxGuid, UnifiedGroupMemberType memberType)
		{
			ArgumentValidator.ThrowIfInvalidValue<UnifiedGroupMemberType>("memberType", memberType, (UnifiedGroupMemberType x) => memberType != UnifiedGroupMemberType.Unknown || memberType != UnifiedGroupMemberType.None);
			SecurityIdentity.GroupMailboxMemberType groupMailboxMemberType = (memberType == UnifiedGroupMemberType.Member) ? SecurityIdentity.GroupMailboxMemberType.Member : SecurityIdentity.GroupMailboxMemberType.Owner;
			if (!clientSecurityContext.AddGroupSids(GroupMailboxAuthorizationHandler.GetGroupMailboxSidBinaryAndAttributes(groupMailboxGuid, groupMailboxMemberType)))
			{
				GroupMailboxAuthorizationHandler.Tracer.TraceError<SecurityIdentifier, Guid, uint>(0L, "GroupMailboxAccess: Unable to add well known group sid to user {0} for the mailbox = {1}...Error = {2}", clientSecurityContext.UserSid, groupMailboxGuid, NativeMethods.GetLastError());
				return;
			}
			GroupMailboxAuthorizationHandler.Tracer.TraceDebug<SecurityIdentifier, Guid>(0L, "GroupMailboxAccess: Successfully munged the token of the user {0} for the mailbox = {1}", clientSecurityContext.UserSid, groupMailboxGuid);
		}

		internal static bool IsGroupSidInUserToken(ClientSecurityContext clientSecurityContext, SecurityIdentifier groupSid)
		{
			SecurityAccessTokenEx securityAccessTokenEx = new SecurityAccessTokenEx();
			clientSecurityContext.SetSecurityAccessToken(securityAccessTokenEx);
			SidBinaryAndAttributes[] groupSids = securityAccessTokenEx.GroupSids;
			if (groupSids != null && groupSids.Length > 0)
			{
				int i = 0;
				while (i < groupSids.Length)
				{
					if (groupSids[i++].SecurityIdentifier == groupSid)
					{
						return true;
					}
				}
			}
			return false;
		}

		private const string OperationContext = "XSO";

		private const string Operation = "CheckMembership";

		private const string EWSClientString = "Client=WebServices;ExchangeInternalEwsClient-EwsFanoutClient;";

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxSessionTracer;

		private static readonly SecurityIdentifier LocalSystemSid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);

		private static readonly PropertyDefinition[] PropertiesToReturnForGroupQuery = new PropertyDefinition[]
		{
			ADUserSchema.Owners
		};

		private static readonly PropertyDefinition[] PropertiesToReturnForUserQuery = new PropertyDefinition[]
		{
			ADObjectSchema.Id
		};

		private readonly AccessingUserInfo accessingUserInfo;

		private readonly OrganizationId organizationId;

		private readonly IExchangePrincipal groupMailboxToLogon;

		private readonly string clientInfoString;

		private readonly ClientSecurityContext clientSecurityContext;
	}
}
