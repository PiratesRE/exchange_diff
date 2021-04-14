using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.ThrottlingService.Client;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;
using Microsoft.Mapi.Security;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSession : StoreSession, IMailboxSession, IStoreSession, IDisposable
	{
		public static HashSet<DefaultFolderType> DefaultFoldersToForceInit { get; set; }

		private static MailboxSession InternalCreateMailboxSession(LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegatedUser, CultureInfo cultureInfo, string clientInfoString, IBudget budget, Action<MailboxSession> initializeMailboxSession, MailboxSession.InitializeMailboxSessionFailure initializeMailboxSessionFailure)
		{
			return MailboxSession.InternalCreateMailboxSession(logonType, owner, delegatedUser, cultureInfo, clientInfoString, budget, initializeMailboxSession, initializeMailboxSessionFailure, null);
		}

		private static MailboxSession InternalCreateMailboxSession(LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegatedUser, CultureInfo cultureInfo, string clientInfoString, IBudget budget, Action<MailboxSession> initializeMailboxSession, MailboxSession.InitializeMailboxSessionFailure initializeMailboxSessionFailure, MailboxSessionSharableDataManager sharedDataManager)
		{
			if (logonType != LogonType.Admin && logonType != LogonType.DelegatedAdmin && logonType != LogonType.Transport && logonType != LogonType.SystemService && logonType != LogonType.Owner && logonType != LogonType.BestAccess && (owner.MailboxInfo.IsArchive || owner.MailboxInfo.IsAggregated))
			{
				throw new InvalidOperationException("Archive and aggregated mailbox logon not valid for " + logonType.ToString());
			}
			if ((logonType != LogonType.Admin || clientInfoString == null || !MailboxSession.AllowedClientsForPublicFolderMailbox.IsMatch(clientInfoString)) && owner.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
			{
				throw new AccessDeniedException(ServerStrings.OperationNotSupportedOnPublicFolderMailbox);
			}
			if (logonType != LogonType.Admin && !MailboxSession.IsAdminAuditSession(logonType, owner, clientInfoString))
			{
				MailboxSession.InternalValidateServerVersion(owner);
			}
			MailboxSession.InternalValidateDatacenterAccess(logonType, owner, delegatedUser);
			bool flag = false;
			MailboxSession mailboxSession = null;
			sharedDataManager = (sharedDataManager ?? new MailboxSessionSharableDataManager());
			try
			{
				mailboxSession = new MailboxSession(cultureInfo, clientInfoString, budget, sharedDataManager);
				if (owner.MailboxInfo.IsArchive || owner.MailboxInfo.MailboxType == MailboxLocationType.AuxArchive)
				{
					mailboxSession.Capabilities = SessionCapabilities.ArchiveSessionCapabilities;
				}
				else if (owner.MailboxInfo.IsAggregated)
				{
					mailboxSession.Capabilities = SessionCapabilities.MirrorSessionCapabilities;
				}
				initializeMailboxSession(mailboxSession);
				int? num = PropertyBag.CheckNullablePropertyValue<int>(InternalSchema.LocaleId, mailboxSession.Mailbox.TryGetProperty(InternalSchema.LocaleId));
				if (num != null)
				{
					int lcidFromCulture = LocaleMap.GetLcidFromCulture(mailboxSession.InternalPreferedCulture);
					if (num.Value != lcidFromCulture)
					{
						throw new ConnectionFailedPermanentException(ServerStrings.CultureMismatchAfterConnect(lcidFromCulture.ToString(), num.Value.ToString()));
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					initializeMailboxSessionFailure();
					if (mailboxSession != null)
					{
						mailboxSession.Dispose();
						mailboxSession = null;
					}
				}
			}
			return mailboxSession;
		}

		public MailboxSession.MailboxItemCountsAndSizes ReadMailboxCountsAndSizes()
		{
			base.Mailbox.Load(MailboxSession.mailboxItemCountsAndSizesProperties);
			MailboxSession.MailboxItemCountsAndSizes result;
			result.ItemCount = (base.Mailbox.TryGetProperty(InternalSchema.ItemCount) as int?);
			result.TotalItemSize = (base.Mailbox.TryGetProperty(MailboxSchema.QuotaUsedExtended) as long?);
			result.DeletedItemCount = (base.Mailbox.TryGetProperty(InternalSchema.DeletedMsgCount) as int?);
			result.TotalDeletedItemSize = (base.Mailbox.TryGetProperty(MailboxSchema.DumpsterQuotaUsedExtended) as long?);
			return result;
		}

		public void MarkAsEhaMailbox()
		{
			base.Mailbox.Load(new PropertyDefinition[]
			{
				StoreObjectSchema.RetentionFlags
			});
			base.Mailbox[StoreObjectSchema.RetentionFlags] = RetentionAndArchiveFlags.EHAMigration;
			base.Mailbox.Save();
			base.Mailbox.Load();
		}

		private static void CheckSystemFolderBypass(LogonType logonType, IList<DefaultFolderType> folders)
		{
			if (logonType != LogonType.Admin && logonType != LogonType.SystemService && !Util.Contains(folders, DefaultFolderType.System))
			{
				throw new AccessDeniedException(ServerStrings.ExSystemFolderAccessDenied);
			}
		}

		private static MailboxSession CreateMailboxSession(LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegateUser, object identity, OpenMailboxSessionFlags flags, CultureInfo cultureInfo, string clientInfoString, PropertyDefinition[] mailboxProperties, IList<DefaultFolderType> foldersToInit, GenericIdentity auxiliaryIdentity, IBudget budget, MailboxSessionSharableDataManager sharedDataManager)
		{
			return MailboxSession.CreateMailboxSession(logonType, owner, delegateUser, identity, flags, cultureInfo, clientInfoString, mailboxProperties, foldersToInit, auxiliaryIdentity, budget, false, sharedDataManager, UnifiedGroupMemberType.Unknown);
		}

		private static MailboxSession CreateMailboxSession(LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegateUser, object identity, OpenMailboxSessionFlags flags, CultureInfo cultureInfo, string clientInfoString, PropertyDefinition[] mailboxProperties, IList<DefaultFolderType> foldersToInit, GenericIdentity auxiliaryIdentity, IBudget budget, bool unifiedSession, MailboxSessionSharableDataManager sharedDataManager, UnifiedGroupMemberType memberType)
		{
			MailboxSession.CheckSystemFolderBypass(logonType, foldersToInit);
			return MailboxSession.InternalCreateMailboxSession(logonType, owner, delegateUser, cultureInfo, clientInfoString, budget, delegate(MailboxSession mailboxSession)
			{
				mailboxSession.mailboxProperties = mailboxProperties;
				mailboxSession.foldersToInit = foldersToInit;
				mailboxSession.unifiedGroupMemberType = memberType;
				try
				{
					mailboxSession.Initialize(null, logonType, owner, delegateUser, identity, flags, auxiliaryIdentity, unifiedSession);
				}
				catch (StorageTransientException e)
				{
					MailboxSession.TriggerSiteMailboxSyncIfNeeded(e, owner, clientInfoString);
					throw;
				}
			}, delegate()
			{
				ExTraceGlobals.SessionTracer.TraceError(0L, "MailboxSession::CreateMailboxSession. Operation failed. mailboxOwner = {0}, delegateUser = {1}, flag = {2}, cultureInfo = {3}, clientInfoString = {4}.", new object[]
				{
					owner,
					delegateUser,
					(int)flags,
					cultureInfo,
					clientInfoString
				});
			}, sharedDataManager);
		}

		private static MailboxSession CreateAlternateMailboxSession(MailboxSession linkedSession, LogonType logonType, IExchangePrincipal owner, object identity, OpenMailboxSessionFlags flags, IList<DefaultFolderType> foldersToInit)
		{
			MailboxSession.CheckSystemFolderBypass(logonType, foldersToInit);
			DelegateLogonUser delegatedUser = new DelegateLogonUser(linkedSession.UserLegacyDN);
			if (owner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox && owner.MailboxInfo.MailboxGuid != linkedSession.MailboxOwner.MailboxInfo.MailboxGuid)
			{
				throw new AccessDeniedException(ServerStrings.AttemptingSessionCreationAgainstWrongGroupMailbox(owner.MailboxInfo.PrimarySmtpAddress.ToString(), linkedSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()));
			}
			return MailboxSession.InternalCreateMailboxSession(logonType, owner, delegatedUser, linkedSession.InternalCulture, linkedSession.ClientInfoString, null, delegate(MailboxSession mailboxSession)
			{
				mailboxSession.mailboxProperties = null;
				mailboxSession.foldersToInit = foldersToInit;
				mailboxSession.unifiedGroupMemberType = linkedSession.unifiedGroupMemberType;
				try
				{
					mailboxSession.Initialize(linkedSession.Mailbox.MapiStore, logonType, owner, new DelegateLogonUser(linkedSession.UserLegacyDN), identity, flags, null, false);
				}
				catch (StorageTransientException e)
				{
					MailboxSession.TriggerSiteMailboxSyncIfNeeded(e, owner, linkedSession.ClientInfoString);
					throw;
				}
			}, delegate()
			{
				ExTraceGlobals.SessionTracer.TraceError<IExchangePrincipal, int>(0L, "MailboxSession::CreateAlternateMailboxSession. Operation failed. mailboxOwner = {0}, flag = {1}.", owner, (int)flags);
			});
		}

		private static void InternalBuildOpenMailboxSessionFlags(MailboxSession.InitializationFlags initFlags, LogonType logonType, IList<DefaultFolderType> foldersToInit, out OpenMailboxSessionFlags openFlags)
		{
			EnumValidator.ThrowIfInvalid<LogonType>(logonType, "logonType");
			EnumValidator.ThrowIfInvalid<MailboxSession.InitializationFlags>(initFlags, "initFlags");
			if ((initFlags & MailboxSession.InitializationFlags.DefaultFolders) == MailboxSession.InitializationFlags.DefaultFolders && (foldersToInit == null || foldersToInit.Count == 0))
			{
				throw new ArgumentException("Must have foldersToInit if initFlags includes InitializationFlags.DefaultFolders", "foldersToInit");
			}
			if ((initFlags & MailboxSession.InitializationFlags.DefaultFolders) != MailboxSession.InitializationFlags.DefaultFolders && foldersToInit != null && foldersToInit.Count > 0)
			{
				throw new ArgumentException("initFlags must include InitializationFlags.DefaultFolders if foldersToInit is specified", "foldersToInit");
			}
			if ((initFlags & (MailboxSession.InitializationFlags.RequestLocalRpc | MailboxSession.InitializationFlags.OverrideHomeMdb | MailboxSession.InitializationFlags.DisconnectedMailbox | MailboxSession.InitializationFlags.XForestMove | MailboxSession.InitializationFlags.MoveUser)) != MailboxSession.InitializationFlags.None && logonType != LogonType.Admin && logonType != LogonType.SystemService && logonType != LogonType.DelegatedAdmin)
			{
				throw new ArgumentException("Flags not valid for non-Admin logon", "initFlags");
			}
			if ((initFlags & (MailboxSession.InitializationFlags.QuotaMessageDelivery | MailboxSession.InitializationFlags.NormalMessageDelivery | MailboxSession.InitializationFlags.SpecialMessageDelivery)) != MailboxSession.InitializationFlags.None && logonType != LogonType.Transport)
			{
				throw new ArgumentException("Flags not valid for non-Transport logon", "initFlags");
			}
			openFlags = OpenMailboxSessionFlags.None;
			if ((initFlags & MailboxSession.InitializationFlags.DefaultFolders) == MailboxSession.InitializationFlags.DefaultFolders)
			{
				openFlags |= OpenMailboxSessionFlags.InitDefaultFolders;
			}
			if ((initFlags & MailboxSession.InitializationFlags.UserConfigurationManager) == MailboxSession.InitializationFlags.UserConfigurationManager)
			{
				openFlags |= OpenMailboxSessionFlags.InitUserConfigurationManager;
			}
			if ((initFlags & MailboxSession.InitializationFlags.CopyOnWrite) == MailboxSession.InitializationFlags.CopyOnWrite)
			{
				openFlags |= OpenMailboxSessionFlags.InitCopyOnWrite;
				if ((openFlags & OpenMailboxSessionFlags.UseRecoveryDatabase) != OpenMailboxSessionFlags.None)
				{
					throw new ArgumentException("No CopyOnWrite allowed for Recovery DB logons", "initFlags");
				}
			}
			if ((initFlags & MailboxSession.InitializationFlags.DeadSessionChecking) == MailboxSession.InitializationFlags.DeadSessionChecking)
			{
				openFlags |= OpenMailboxSessionFlags.InitDeadSessionChecking;
			}
			if ((initFlags & MailboxSession.InitializationFlags.CheckPrivateItemsAccess) == MailboxSession.InitializationFlags.CheckPrivateItemsAccess)
			{
				openFlags |= OpenMailboxSessionFlags.InitCheckPrivateItemsAccess;
			}
			if ((initFlags & MailboxSession.InitializationFlags.SuppressFolderIdPrefetch) == MailboxSession.InitializationFlags.SuppressFolderIdPrefetch)
			{
				openFlags |= OpenMailboxSessionFlags.SuppressFolderIdPrefetch;
			}
			if ((initFlags & MailboxSession.InitializationFlags.UseNamedProperties) == MailboxSession.InitializationFlags.UseNamedProperties)
			{
				openFlags |= OpenMailboxSessionFlags.UseNamedProperties;
			}
			if ((initFlags & MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization) == MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization)
			{
				openFlags |= OpenMailboxSessionFlags.DeferDefaultFolderIdInitialization;
			}
			if ((initFlags & MailboxSession.InitializationFlags.UseRecoveryDatabase) == MailboxSession.InitializationFlags.UseRecoveryDatabase)
			{
				openFlags |= OpenMailboxSessionFlags.UseRecoveryDatabase;
				if ((openFlags & OpenMailboxSessionFlags.InitCopyOnWrite) != OpenMailboxSessionFlags.None)
				{
					throw new ArgumentException("No CopyOnWrite allowed for Recovery DB logons", "initFlags");
				}
			}
			if ((initFlags & MailboxSession.InitializationFlags.NonInteractiveSession) == MailboxSession.InitializationFlags.NonInteractiveSession)
			{
				openFlags |= OpenMailboxSessionFlags.NonInteractiveSession;
			}
			if ((initFlags & MailboxSession.InitializationFlags.IgnoreForcedFolderInit) == MailboxSession.InitializationFlags.IgnoreForcedFolderInit)
			{
				openFlags |= OpenMailboxSessionFlags.IgnoreForcedFolderInit;
			}
			if ((initFlags & MailboxSession.InitializationFlags.ReadOnly) == MailboxSession.InitializationFlags.ReadOnly)
			{
				openFlags |= OpenMailboxSessionFlags.ReadOnly;
			}
			switch (logonType)
			{
			case LogonType.Admin:
			case LogonType.DelegatedAdmin:
				openFlags |= OpenMailboxSessionFlags.RequestAdminAccess;
				if ((initFlags & MailboxSession.InitializationFlags.RequestLocalRpc) == MailboxSession.InitializationFlags.RequestLocalRpc)
				{
					openFlags |= OpenMailboxSessionFlags.RequestLocalRpcConnection;
				}
				if ((initFlags & MailboxSession.InitializationFlags.OverrideHomeMdb) == MailboxSession.InitializationFlags.OverrideHomeMdb)
				{
					openFlags |= OpenMailboxSessionFlags.OverrideHomeMdb;
				}
				if ((initFlags & MailboxSession.InitializationFlags.AllowAdminLocalization) == MailboxSession.InitializationFlags.AllowAdminLocalization)
				{
					openFlags |= OpenMailboxSessionFlags.AllowAdminLocalization;
					return;
				}
				break;
			case LogonType.Delegated:
			case LogonType.BestAccess:
				break;
			case LogonType.Transport:
				openFlags |= OpenMailboxSessionFlags.RequestTransportAccess;
				if ((initFlags & MailboxSession.InitializationFlags.QuotaMessageDelivery) == MailboxSession.InitializationFlags.QuotaMessageDelivery)
				{
					openFlags |= OpenMailboxSessionFlags.OpenForQuotaMessageDelivery;
				}
				if ((initFlags & MailboxSession.InitializationFlags.NormalMessageDelivery) == MailboxSession.InitializationFlags.NormalMessageDelivery)
				{
					openFlags |= OpenMailboxSessionFlags.OpenForNormalMessageDelivery;
				}
				if ((initFlags & MailboxSession.InitializationFlags.SpecialMessageDelivery) == MailboxSession.InitializationFlags.SpecialMessageDelivery)
				{
					openFlags |= OpenMailboxSessionFlags.OpenForSpecialMessageDelivery;
					return;
				}
				break;
			case LogonType.SystemService:
				openFlags |= OpenMailboxSessionFlags.RequestAdminAccess;
				if (initFlags.HasFlag(MailboxSession.InitializationFlags.RequestLocalRpc))
				{
					openFlags |= OpenMailboxSessionFlags.RequestLocalRpcConnection;
				}
				if (initFlags.HasFlag(MailboxSession.InitializationFlags.OverrideHomeMdb))
				{
					openFlags |= OpenMailboxSessionFlags.OverrideHomeMdb;
				}
				if (initFlags.HasFlag(MailboxSession.InitializationFlags.DisconnectedMailbox))
				{
					openFlags |= OpenMailboxSessionFlags.DisconnectedMailbox;
				}
				if (initFlags.HasFlag(MailboxSession.InitializationFlags.XForestMove))
				{
					openFlags |= OpenMailboxSessionFlags.XForestMove;
				}
				if (initFlags.HasFlag(MailboxSession.InitializationFlags.MoveUser))
				{
					openFlags |= OpenMailboxSessionFlags.MoveUser;
				}
				if (initFlags.HasFlag(MailboxSession.InitializationFlags.OlcSync))
				{
					openFlags |= OpenMailboxSessionFlags.OlcSync;
				}
				if ((initFlags & MailboxSession.InitializationFlags.AllowAdminLocalization) == MailboxSession.InitializationFlags.AllowAdminLocalization)
				{
					openFlags |= OpenMailboxSessionFlags.AllowAdminLocalization;
				}
				break;
			default:
				return;
			}
		}

		public static MailboxSession ConfigurableOpen(IExchangePrincipal mailbox, MailboxAccessInfo accessInfo, CultureInfo cultureInfo, string clientInfoString, LogonType logonType, PropertyDefinition[] mailboxProperties, MailboxSession.InitializationFlags initFlags, IList<DefaultFolderType> foldersToInit)
		{
			return MailboxSession.ConfigurableOpen(mailbox, accessInfo, cultureInfo, clientInfoString, logonType, mailboxProperties, initFlags, foldersToInit, null);
		}

		private static MailboxSession ConfigurableOpenAlternate(MailboxSession linkedSession, IExchangePrincipal mailbox, object identity, LogonType logonType, MailboxSession.InitializationFlags initFlags, IList<DefaultFolderType> foldersToInit)
		{
			Util.ThrowOnNullArgument(linkedSession, "linkedSession");
			Util.ThrowOnNullArgument(mailbox, "mailbox");
			OpenMailboxSessionFlags flags;
			MailboxSession.InternalBuildOpenMailboxSessionFlags(initFlags, logonType, foldersToInit, out flags);
			return MailboxSession.CreateAlternateMailboxSession(linkedSession, logonType, mailbox, identity, flags, foldersToInit);
		}

		private static bool IsMemberLogonToGroupMailbox(IExchangePrincipal mailboxOwner, LogonType logonType)
		{
			return mailboxOwner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox && logonType != LogonType.Admin && logonType != LogonType.SystemService && logonType != LogonType.Transport && logonType != LogonType.Owner;
		}

		protected MailboxSession(CultureInfo cultureInfo, string clientInfoString, IBudget budget, MailboxSessionSharableDataManager sharedDataManager) : base(cultureInfo, clientInfoString, budget)
		{
			this.activitySessionHook = Hookable<IActivitySession>.Create(true, () => this.activitySession, delegate(IActivitySession value)
			{
				this.activitySession = value;
			});
			this.sharedDataManager = sharedDataManager;
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.delegateSessionManager = new DelegateSessionManager(this);
				disposeGuard.Success();
			}
		}

		public static MailboxSession Open(IExchangePrincipal mailboxOwner, AuthzContextHandle authenticatedUserHandle, CultureInfo cultureInfo, string clientInfoString)
		{
			if (authenticatedUserHandle == null)
			{
				throw new ArgumentNullException("authenticatedUserHandle");
			}
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(authenticatedUserHandle);
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Owner, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
		}

		public DelegateSessionHandle GetDelegateSessionHandle(IExchangePrincipal principal)
		{
			DelegateSessionHandle result;
			using (base.CheckDisposed("GetDelegateSessionHandle"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveDelegateUsers, "CanHaveDelegateUsers");
				if (this.logonType != LogonType.Owner && principal.RecipientTypeDetails != RecipientTypeDetails.GroupMailbox)
				{
					throw new InvalidOperationException("This session itself is a delegated session.");
				}
				if (principal == null)
				{
					throw new ArgumentNullException("principal");
				}
				DelegateSessionEntry entry = this.InternalGetDelegateSessionEntry(principal, OpenBy.Consumer);
				result = new DelegateSessionHandle(entry);
			}
			return result;
		}

		internal DelegateSessionEntry InternalGetDelegateSessionEntry(IExchangePrincipal principal, OpenBy openBy)
		{
			DelegateSessionEntry delegateSessionEntry;
			using (base.CreateSessionGuard("InternalGetDelegateSessionEntry"))
			{
				delegateSessionEntry = this.delegateSessionManager.GetDelegateSessionEntry(principal, openBy);
			}
			return delegateSessionEntry;
		}

		public static MailboxSession Open(IExchangePrincipal mailboxOwner, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(clientSecurityContext);
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Owner, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
		}

		public MailboxSession OpenAlternate(IExchangePrincipal mailboxOwner)
		{
			MailboxSession result;
			using (base.CheckDisposed("OpenAlternate"))
			{
				if (mailboxOwner == null)
				{
					throw new ArgumentNullException("mailboxOwner");
				}
				MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
				result = MailboxSession.ConfigurableOpenAlternate(this, mailboxOwner, this.identity, LogonType.BestAccess, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
			}
			return result;
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, string accessingUserDn, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString)
		{
			return MailboxSession.OpenWithBestAccess(mailboxOwner, accessingUserDn, clientSecurityContext, cultureInfo, clientInfoString, null);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, string accessingUserLegacyDn, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb)
		{
			return MailboxSession.OpenAsAdmin(mailboxOwner, accessingUserLegacyDn, clientSecurityContext, cultureInfo, clientInfoString, useLocalRpc, ignoreHomeMdb, false);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, string accessingUserLegacyDn, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb, bool useRecoveryDatabase)
		{
			MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(accessingUserLegacyDn, clientSecurityContext);
			return MailboxSession.OpenAsAdmin(mailboxOwner, accessInfo, LogonType.DelegatedAdmin, cultureInfo, clientInfoString, useLocalRpc, ignoreHomeMdb, useRecoveryDatabase, false, false);
		}

		internal static MailboxSession InternalOpenDelegateAccess(MailboxSession delegateMailboxSession, IExchangePrincipal principal)
		{
			if (delegateMailboxSession == null)
			{
				throw new ArgumentNullException("delegateMailboxSession");
			}
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			delegateMailboxSession.CheckMasterSessionForCalendarDelegate();
			MailboxAccessInfo accessInfo = null;
			string clientInfoString = delegateMailboxSession.clientInfoString;
			string accessingUserDn;
			if (principal.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
			{
				accessingUserDn = delegateMailboxSession.UserLegacyDN;
			}
			else
			{
				accessingUserDn = delegateMailboxSession.MailboxOwnerLegacyDN;
			}
			if (delegateMailboxSession.Identity is WindowsIdentity)
			{
				accessInfo = new MailboxAccessInfo(accessingUserDn, new WindowsPrincipal((WindowsIdentity)delegateMailboxSession.Identity));
			}
			else if (delegateMailboxSession.Identity is ClientIdentityInfo)
			{
				if (principal.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox && principal.MailboxInfo.MailboxGuid != delegateMailboxSession.MailboxOwner.MailboxInfo.MailboxGuid)
				{
					throw new AccessDeniedException(ServerStrings.AttemptingSessionCreationAgainstWrongGroupMailbox(principal.MailboxInfo.PrimarySmtpAddress.ToString(), delegateMailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()));
				}
				accessInfo = new MailboxAccessInfo(accessingUserDn, (ClientIdentityInfo)delegateMailboxSession.Identity);
			}
			LogonType logonType = (delegateMailboxSession.LogonType == LogonType.Admin) ? LogonType.Admin : LogonType.BestAccess;
			MailboxSession mailboxSession = MailboxSession.ConfigurableOpen(principal, accessInfo, delegateMailboxSession.InternalPreferedCulture, clientInfoString, logonType, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders, null, false, null, delegateMailboxSession.unifiedGroupMemberType);
			mailboxSession.ExTimeZone = delegateMailboxSession.ExTimeZone;
			mailboxSession.MasterMailboxSession = delegateMailboxSession;
			mailboxSession.SetClientIPEndpoints(delegateMailboxSession.ClientIPAddress, delegateMailboxSession.ServerIPAddress);
			return mailboxSession;
		}

		private void CheckMasterSessionForCalendarDelegate()
		{
			if (!base.IsConnected)
			{
				throw new InvalidOperationException(string.Format("The master mailbox session is not connected.", new object[0]));
			}
		}

		private static void CheckNoRemoteExchangePrincipal(IExchangePrincipal ep)
		{
			if (ep.MailboxInfo.IsRemote)
			{
				throw new NotSupportedException("This operation is not supported for remote connections.");
			}
		}

		public void SetReceiveFolder(string messageClass, StoreObjectId folderId)
		{
			using (this.CheckObjectState("SetReceiveFolder"))
			{
				if (messageClass == null)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "MailboxSession::SetReceiveFolder. SetReceiveFolder cannot be called with messageClass being null.");
					throw new ArgumentNullException("messageClass");
				}
				if (folderId == null || folderId.ObjectType != StoreObjectType.Folder)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "MailboxSession::SetReceiveFolder. SetReceiveFolder called with an invaild folder id parameter.");
					throw new ArgumentException("folderId");
				}
				byte[] providerLevelItemId = folderId.ProviderLevelItemId;
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					base.Mailbox.MapiStore.SetReceiveFolder(messageClass, providerLevelItemId);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReceiveFolder, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::SetReceiveFolder.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReceiveFolder, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::SetReceiveFolder.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
		}

		public void ClearReceiveFolder(string messageClass)
		{
			using (this.CheckObjectState("ClearReceiveFolder"))
			{
				if (string.IsNullOrEmpty(messageClass))
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "MailboxSession::ClearReceiveFolder. ClearReceiveFolder cannot be called with messageClass being null or empty.");
					if (messageClass == null)
					{
						throw new ArgumentNullException("messageClass");
					}
					throw new ArgumentException("messageClass");
				}
				else
				{
					bool flag = false;
					try
					{
						if (this != null)
						{
							this.BeginMapiCall();
							this.BeginServerHealthCall();
							flag = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						base.Mailbox.MapiStore.SetReceiveFolder(messageClass, null);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReceiveFolder, ex, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession::SetReceiveFolder.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReceiveFolder, ex2, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession::SetReceiveFolder.", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (this != null)
							{
								this.EndMapiCall();
								if (flag)
								{
									this.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
				}
			}
		}

		public StoreObjectId GetReceiveFolder(string messageClass)
		{
			StoreObjectId receiveFolderId;
			using (base.CreateSessionGuard("GetReceiveFolder"))
			{
				string text;
				receiveFolderId = this.GetReceiveFolderId(messageClass, out text);
			}
			return receiveFolderId;
		}

		public ReceiveFolderInfo[] GetReceiveFolderInfo()
		{
			ReceiveFolderInfo[] result;
			using (this.CheckObjectState("GetReceiveFolderInfo"))
			{
				PropValue[][] array = null;
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					array = base.Mailbox.MapiStore.GetReceiveFolderInfo();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetReceiveFolderInfo, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetReceiveFolderInfo", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetReceiveFolderInfo, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetReceiveFolderInfo", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				ReceiveFolderInfo[] array2 = new ReceiveFolderInfo[array.Length];
				for (long num = 0L; num < (long)array.Length; num += 1L)
				{
					checked
					{
						PropValue[] array3 = array[(int)((IntPtr)num)];
						array2[(int)((IntPtr)num)] = new ReceiveFolderInfo(array3[0].GetBytes(), array3[1].GetString(), new ExDateTime(this.ExTimeZone, array3[2].GetDateTime()));
					}
				}
				result = array2;
			}
			return result;
		}

		public StoreObjectId GetReceiveFolderId(string messageClass, out string explicitMessageClass)
		{
			StoreObjectId result;
			using (this.CheckObjectState("GetReceiveFolderId"))
			{
				byte[] array = null;
				string text = null;
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					array = base.Mailbox.MapiStore.GetReceiveFolderEntryId(messageClass, out text);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetReceiveFolder, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetReceiveFolderId.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetReceiveFolder, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetReceiveFolderId.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				explicitMessageClass = text;
				if (array != null && array.Length > 0)
				{
					result = StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Folder);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public StoreObjectId GetTransportQueueFolderId()
		{
			StoreObjectId result;
			using (this.CheckObjectState("GetTransportQueueFolderId"))
			{
				byte[] array = null;
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					array = base.Mailbox.MapiStore.GetTransportQueueFolderId();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetTransportQueueFolderId, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetTransportQueueFolderId.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetTransportQueueFolderId, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetTransportQueueFolderId.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				if (array != null && array.Length > 0)
				{
					result = StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Folder);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public void UpdateDeferredActionMessages(byte[] serverSvrEId, byte[] clientSvrEId)
		{
			ExTraceGlobals.SessionTracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxSession::UpdateDeferredActionMessages. Operation started. mailbox = {0}.", this.mailboxOwner.LegacyDn);
			using (this.CheckObjectState("UpdateDeferredActionMessages"))
			{
				ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.OriginalMessageSvrEId, serverSvrEId);
				try
				{
					using (Folder folder = Folder.Bind(this, DefaultFolderType.DeferredActionFolder))
					{
						List<StoreObjectId> list = new List<StoreObjectId>();
						using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, MailboxSession.DeferredActionMessagesDefinitions))
						{
							byte[] array = (clientSvrEId != null) ? IdConverter.CreateEntryIdFromForeignServerId(clientSvrEId) : null;
							while (queryResult.SeekToCondition(SeekReference.OriginCurrent, seekFilter))
							{
								object[][] rows = queryResult.GetRows(1);
								if (rows.Length != 1)
								{
									break;
								}
								StoreObjectId objectId = ((VersionedId)rows[0][0]).ObjectId;
								if (array != null && array.Length != 0)
								{
									using (Item item = Item.Bind(this, objectId))
									{
										item.OpenAsReadWrite();
										item[ItemSchema.OriginalMessageEntryId] = array;
										item[ItemSchema.DeferredActionMessageBackPatched] = true;
										item.Save(SaveMode.NoConflictResolutionForceSave);
										continue;
									}
								}
								list.Add(objectId);
							}
							if (list.Count > 0)
							{
								folder.DeleteObjects(DeleteItemFlags.HardDelete, list.ToArray());
							}
						}
					}
				}
				finally
				{
					StoreObjectId storeObjectId = base.IdConverter.CreateMessageIdFromSvrEId(serverSvrEId);
					using (Folder folder2 = Folder.Bind(this, DefaultFolderType.AllItems))
					{
						folder2.DeleteObjects(DeleteItemFlags.SoftDelete, new StoreId[]
						{
							storeObjectId
						});
					}
				}
			}
			ExTraceGlobals.SessionTracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxSession::UpdateDeferredActionMessages. Operation succeeded. mailbox = {0}.", this.mailboxOwner.LegacyDn);
		}

		public DefaultFolderType IsDefaultFolderType(StoreId folderId)
		{
			DefaultFolderType result;
			using (this.CheckObjectState("IsDefaultFolderType"))
			{
				ExTraceGlobals.SessionTracer.Information<StoreId>((long)this.GetHashCode(), "MailboxSession::IsDefaultFolderType. FolderId = {0}.", folderId);
				if (folderId == null)
				{
					throw new ArgumentNullException("folderId");
				}
				if (this.isDefaultFolderManagerBeingInitialized)
				{
					ExTraceGlobals.SessionTracer.TraceError<string, StoreId>((long)this.GetHashCode(), "MailboxSession::IsDefaultFolderType. The method is called when the default folders are not initialized completely yet. Mailbox = {0}, folderId = {1}.", this.MailboxOwnerLegacyDN, folderId);
					result = DefaultFolderType.None;
				}
				else
				{
					if (this.defaultFolderManager == null)
					{
						throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
					}
					result = this.defaultFolderManager.IsDefaultFolderType(folderId);
				}
			}
			return result;
		}

		public StoreObjectId CreateDefaultFolder(DefaultFolderType defaultFolderType)
		{
			StoreObjectId result;
			using (this.CheckObjectState("CreateDefaultFolder"))
			{
				base.CheckCapabilities(base.Capabilities.CanCreateDefaultFolders, "CanCreateDefaultFolders");
				EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				result = this.defaultFolderManager.CreateDefaultFolder(defaultFolderType);
			}
			return result;
		}

		public override bool TryFixDefaultFolderId(DefaultFolderType defaultFolderType, out StoreObjectId id)
		{
			bool result;
			using (this.CheckObjectState("TryFixDefaultFolderId"))
			{
				EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
				if (!base.Capabilities.CanCreateDefaultFolders)
				{
					id = null;
					result = false;
				}
				else if (this.defaultFolderManager == null)
				{
					id = null;
					result = false;
				}
				else
				{
					result = this.defaultFolderManager.TryFixDefaultFolderId(defaultFolderType, out id);
				}
			}
			return result;
		}

		public StoreObjectId RefreshDefaultFolder(DefaultFolderType defaultFolderType)
		{
			StoreObjectId result;
			using (this.CheckObjectState("RefreshDefaultFolder"))
			{
				base.CheckCapabilities(base.Capabilities.CanCreateDefaultFolders, "CanCreateDefaultFolders");
				EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				result = this.defaultFolderManager.RefreshDefaultFolder(defaultFolderType);
			}
			return result;
		}

		public void DeleteDefaultFolder(DefaultFolderType defaultFolderType, DeleteItemFlags deleteItemFlags)
		{
			using (this.CheckObjectState("DeleteDefaultFolder"))
			{
				base.CheckCapabilities(base.Capabilities.CanCreateDefaultFolders, "CanCreateDefaultFolders");
				EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				this.CheckSystemFolderAccess(this.defaultFolderManager.GetDefaultFolderId(defaultFolderType));
				this.defaultFolderManager.DeleteDefaultFolder(defaultFolderType, deleteItemFlags);
			}
		}

		public StoreObjectId CreateSystemFolder()
		{
			StoreObjectId result;
			using (this.CheckObjectState("CreateSystemFolder"))
			{
				if (!this.UseSystemFolder)
				{
					throw new AccessDeniedException(ServerStrings.ExSystemFolderAccessDenied);
				}
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				if (base.LogonType != LogonType.SystemService)
				{
					throw new AccessDeniedException(ServerStrings.ExSystemFolderAccessDenied);
				}
				result = this.defaultFolderManager.CreateDefaultSystemFolder();
			}
			return result;
		}

		public StoreObjectId GetSystemFolderId()
		{
			StoreObjectId defaultFolderId;
			using (this.CheckObjectState("GetSystemFolderId"))
			{
				if (!this.UseSystemFolder)
				{
					throw new AccessDeniedException(ServerStrings.ExSystemFolderAccessDenied);
				}
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				if (base.LogonType != LogonType.SystemService && base.LogonType != LogonType.Transport)
				{
					throw new AccessDeniedException(ServerStrings.ExSystemFolderAccessDenied);
				}
				defaultFolderId = this.defaultFolderManager.GetDefaultFolderId(DefaultFolderType.System);
			}
			return defaultFolderId;
		}

		internal bool UseSystemFolder
		{
			get
			{
				bool result;
				using (this.CheckObjectState("UseSystemFolder"))
				{
					result = Util.Contains(this.foldersToInit, DefaultFolderType.System);
				}
				return result;
			}
		}

		public StoreObjectId GetAdminAuditLogsFolderId()
		{
			StoreObjectId defaultFolderId;
			using (this.CheckObjectState("GetAdminAuditLogsFolderId"))
			{
				if (!this.UseAdminAuditLogsFolder)
				{
					throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsFolderAccessDenied);
				}
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				if (!this.bypassAuditsFolderAccessChecking && base.LogonType != LogonType.SystemService)
				{
					throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsFolderAccessDenied);
				}
				defaultFolderId = this.defaultFolderManager.GetDefaultFolderId(DefaultFolderType.AdminAuditLogs);
			}
			return defaultFolderId;
		}

		private static bool IsAdminAuditSession(LogonType logonType, IExchangePrincipal owner, string clientInfoString)
		{
			return owner != null && !string.IsNullOrEmpty(owner.MailboxInfo.PrimarySmtpAddress.ToString()) && !string.IsNullOrEmpty(clientInfoString) && logonType == LogonType.SystemService && owner.MailboxInfo.PrimarySmtpAddress.ToString().Contains("SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}") && clientInfoString.Contains("AdminLog");
		}

		internal bool UseAdminAuditLogsFolder
		{
			get
			{
				bool result;
				using (this.CheckObjectState("UseAdminAuditLogsFolder"))
				{
					result = Util.Contains(this.foldersToInit, DefaultFolderType.AdminAuditLogs);
				}
				return result;
			}
		}

		public StoreObjectId GetAuditsFolderId()
		{
			StoreObjectId defaultFolderId;
			using (this.CheckObjectState("GetAuditsFolderId"))
			{
				if (!this.UseAuditsFolder)
				{
					throw new AccessDeniedException(ServerStrings.ExAuditsFolderAccessDenied);
				}
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				if (!this.bypassAuditsFolderAccessChecking && base.LogonType != LogonType.SystemService)
				{
					throw new AccessDeniedException(ServerStrings.ExAuditsFolderAccessDenied);
				}
				defaultFolderId = this.defaultFolderManager.GetDefaultFolderId(DefaultFolderType.Audits);
			}
			return defaultFolderId;
		}

		internal bool UseAuditsFolder
		{
			get
			{
				bool result;
				using (this.CheckObjectState("UseAuditsFolder"))
				{
					result = Util.Contains(this.foldersToInit, DefaultFolderType.Audits);
				}
				return result;
			}
		}

		public override StoreObjectId GetDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			StoreObjectId defaultFolderId;
			using (this.CheckObjectState("GetDefaultFolderId"))
			{
				EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
				if (defaultFolderType == DefaultFolderType.System)
				{
					throw new InvalidOperationException(ServerStrings.ExCannotAccessSystemFolderId);
				}
				if (defaultFolderType == DefaultFolderType.AdminAuditLogs)
				{
					throw new InvalidOperationException(ServerStrings.ExCannotAccessAdminAuditLogsFolderId);
				}
				if (defaultFolderType == DefaultFolderType.Audits)
				{
					throw new InvalidOperationException(ServerStrings.ExCannotAccessAuditsFolderId);
				}
				if (this.defaultFolderManager == null)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				defaultFolderId = this.defaultFolderManager.GetDefaultFolderId(defaultFolderType);
			}
			return defaultFolderId;
		}

		public VersionedId GetLocalFreeBusyMessageId(StoreObjectId freeBusyFolderId)
		{
			VersionedId result;
			using (this.CheckObjectState("GetLocalFreeBusyMessageId"))
			{
				if (freeBusyFolderId != null)
				{
					using (Folder folder = Folder.Bind(this, freeBusyFolderId))
					{
						using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, MailboxSession.ItemSchemaIdStoreDefinition))
						{
							if (queryResult.SeekToCondition(SeekReference.OriginBeginning, MailboxSession.FreeBusyQueryFilter))
							{
								object[][] rows = queryResult.GetRows(1);
								if (rows != null && rows.Length > 0)
								{
									return PropertyBag.CheckPropertyValue<VersionedId>(ItemSchema.Id, rows[0][0]);
								}
							}
						}
					}
				}
				ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::GetLocalFreeBusyMessageId. No FreeBusyMessage was found from the FreeBusy folder. Mailbox = {0}.", this.InternalMailboxOwner);
				result = null;
			}
			return result;
		}

		internal DefaultFolder InternalGetDefaultFolder(DefaultFolderType defaultFolderType)
		{
			DefaultFolder defaultFolder;
			using (base.CreateSessionGuard("InternalGetDefaultFolder"))
			{
				defaultFolder = this.defaultFolderManager.GetDefaultFolder(defaultFolderType);
			}
			return defaultFolder;
		}

		public OperationResult LocalizeDefaultFolders(out Exception[] problems)
		{
			OperationResult result;
			using (this.CheckObjectState("LocalizeDefaultFolders"))
			{
				if (this.defaultFolderManager == null || base.LogonType == LogonType.Delegated || (base.StoreFlag & OpenStoreFlag.NoLocalization) == OpenStoreFlag.NoLocalization)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				result = this.defaultFolderManager.Localize(out problems);
			}
			return result;
		}

		public bool VerifyDefaultFolderLocalization()
		{
			bool result;
			using (this.CheckObjectState("VerifyDefaultFolderLocalization"))
			{
				if (this.defaultFolderManager == null || (base.StoreFlag & OpenStoreFlag.NoLocalization) == OpenStoreFlag.NoLocalization)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				result = this.defaultFolderManager.VerifyLocalization();
			}
			return result;
		}

		public PropertyError RenameDefaultFolder(DefaultFolderType folderType, string newName)
		{
			PropertyError result;
			using (this.CheckObjectState("RenameDefaultFolder"))
			{
				EnumValidator.ThrowIfInvalid<DefaultFolderType>(folderType, DefaultFolderType.ElcRoot);
				if (newName == null)
				{
					throw new ArgumentNullException("newName");
				}
				if (this.defaultFolderManager == null || base.LogonType != LogonType.Admin)
				{
					throw new InvalidOperationException(ServerStrings.ExDefaultFoldersNotInitialized);
				}
				if (folderType == DefaultFolderType.System && base.LogonType != LogonType.SystemService && base.LogonType != LogonType.Transport)
				{
					throw new InvalidOperationException(ServerStrings.ExSystemFolderAccessDenied);
				}
				if (folderType == DefaultFolderType.AdminAuditLogs && base.LogonType != LogonType.SystemService)
				{
					throw new InvalidOperationException(ServerStrings.ExAdminAuditLogsFolderAccessDenied);
				}
				if (folderType == DefaultFolderType.Audits && base.LogonType != LogonType.SystemService)
				{
					throw new InvalidOperationException(ServerStrings.ExAuditsFolderAccessDenied);
				}
				DefaultFolder defaultFolder = this.InternalGetDefaultFolder(folderType);
				PropertyError propertyError;
				if (!defaultFolder.Rename(newName, out propertyError))
				{
					result = propertyError;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public DefaultFolderType[] DefaultFolders
		{
			get
			{
				DefaultFolderType[] defaultFolderInitializationOrder;
				using (this.CheckObjectState("DefaultFolders::get"))
				{
					defaultFolderInitializationOrder = DefaultFolderManager.defaultFolderInitializationOrder;
				}
				return defaultFolderInitializationOrder;
			}
		}

		IUserConfigurationManager IMailboxSession.UserConfigurationManager
		{
			get
			{
				return this.UserConfigurationManager;
			}
		}

		public UserConfigurationManager UserConfigurationManager
		{
			get
			{
				UserConfigurationManager result;
				using (this.CheckObjectState("UserConfigurationManager::get"))
				{
					base.CheckCapabilities(base.Capabilities.CanHaveUserConfigurationManager, "CanHaveUserConfigurationManager");
					result = this.userConfigurationManager;
				}
				return result;
			}
		}

		public MapiStore __ContainedMapiStore
		{
			get
			{
				MapiStore mapiStore;
				using (this.CheckObjectState("__ContainedMapiStore::get"))
				{
					mapiStore = base.Mailbox.MapiStore;
				}
				return mapiStore;
			}
		}

		public override string GccResourceIdentifier
		{
			get
			{
				string result;
				using (base.CheckDisposed("GccResourceIdentifier::get"))
				{
					result = this.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
				}
				return result;
			}
		}

		public override ExTimeZone ExTimeZone
		{
			get
			{
				ExTimeZone result;
				using (base.CheckDisposed("ExTimeZone::get"))
				{
					result = this.exTimeZone;
				}
				return result;
			}
			set
			{
				using (base.CheckDisposed("ExTimeZone::set"))
				{
					if (value == null)
					{
						throw new ArgumentNullException("ExTimeZone");
					}
					this.exTimeZone = value;
					this.delegateSessionManager.SetTimeZone(this.exTimeZone);
				}
			}
		}

		public ExternalUserCollection GetExternalUsers()
		{
			ExternalUserCollection result;
			using (this.CheckObjectState("GetExternalUsers"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveExternalUsers, "CanHaveExternalUsers");
				result = new ExternalUserCollection(this);
			}
			return result;
		}

		public IADOrgPerson DelegateUser
		{
			get
			{
				IADOrgPerson result;
				using (base.CheckDisposed("DelegateUser::get"))
				{
					base.CheckCapabilities(base.Capabilities.CanHaveDelegateUsers, "CanHaveDelegateUsers");
					result = this.delegateUser;
				}
				return result;
			}
		}

		public string MailboxOwnerLegacyDN
		{
			get
			{
				string mailboxLegacyDn;
				using (base.CheckDisposed("MailboxOwnerLegacyDN::get"))
				{
					mailboxLegacyDn = this.mailboxOwner.MailboxInfo.GetMailboxLegacyDn(this.mailboxOwner.LegacyDn);
				}
				return mailboxLegacyDn;
			}
		}

		public override IExchangePrincipal MailboxOwner
		{
			get
			{
				IExchangePrincipal result;
				using (base.CheckDisposed("MailboxOwner::get"))
				{
					if (this.mailboxOwner == null)
					{
						throw new InvalidOperationException(ServerStrings.ExNoMailboxOwner);
					}
					result = this.mailboxOwner;
				}
				return result;
			}
		}

		public override CultureInfo PreferedCulture
		{
			get
			{
				CultureInfo internalPreferedCulture;
				using (base.CheckDisposed("PreferedCulture::get"))
				{
					base.CheckCapabilities(base.Capabilities.CanHaveCulture, "CanHaveCulture");
					internalPreferedCulture = this.InternalPreferedCulture;
				}
				return internalPreferedCulture;
			}
		}

		internal override CultureInfo InternalPreferedCulture
		{
			get
			{
				CultureInfo result;
				using (base.CheckDisposed("InternalPreferedCulture::get"))
				{
					if (this.preferedCultureInfoCache == null)
					{
						CultureInfo[] cultures = this.InternalGetMailboxCultures();
						CultureInfo preferedCulture = Util.CultureSelector.GetPreferedCulture(cultures);
						this.preferedCultureInfoCache = MapiCultureInfo.AdjustFromClientRequest(base.InternalPreferedCulture, preferedCulture);
						ExTraceGlobals.StorageTracer.TraceError<CultureInfo>((long)this.GetHashCode(), "Picked PreferedCulture: {0}.", this.preferedCultureInfoCache);
					}
					result = this.preferedCultureInfoCache;
				}
				return result;
			}
		}

		public override string DisplayAddress
		{
			get
			{
				return this.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		public bool CanActAsOwner
		{
			get
			{
				bool result;
				using (base.CheckDisposed("CanActAsOwner::get"))
				{
					bool? flag = base.Mailbox.TryGetProperty(InternalSchema.CanActAsOwner) as bool?;
					if (flag == null)
					{
						result = false;
					}
					else
					{
						result = flag.Value;
					}
				}
				return result;
			}
		}

		public bool CanSendAs
		{
			get
			{
				bool result;
				using (base.CheckDisposed("CanSendAs::get"))
				{
					bool? flag = base.Mailbox.TryGetProperty(InternalSchema.CanSendAs) as bool?;
					if (flag == null)
					{
						result = false;
					}
					else
					{
						result = flag.Value;
					}
				}
				return result;
			}
		}

		public bool IsMailboxLocalized
		{
			get
			{
				bool result;
				using (base.CheckDisposed("IsMailboxLocalized::get"))
				{
					if (this.mailboxProperties != null && !this.mailboxProperties.Contains(InternalSchema.IsMailboxLocalized))
					{
						result = true;
					}
					else
					{
						bool? flag = base.Mailbox.TryGetProperty(InternalSchema.IsMailboxLocalized) as bool?;
						if (flag == null)
						{
							result = false;
						}
						else
						{
							result = flag.Value;
						}
					}
				}
				return result;
			}
		}

		public bool ConnectWithStatus()
		{
			bool result;
			using (base.CheckDisposed("ConnectWithStatus"))
			{
				bool flag = false;
				if (base.IsConnected)
				{
					ExTraceGlobals.SessionTracer.TraceError<string, Type, int>((long)this.GetHashCode(), "MailboxSession::{0}. Object type = {1}, hashcode = {2} trying to call Connect when already connected.", "Connect", base.GetType(), this.GetHashCode());
					throw new ConnectionFailedPermanentException(ServerStrings.ExAlreadyConnected);
				}
				ExTraceGlobals.SessionTracer.Information((long)this.GetHashCode(), "MailboxSession::Connect.");
				bool flag2 = false;
				try
				{
					if (base.IsDead)
					{
						bool flag3 = base.StopDeadSessionChecking();
						this.ForceOpen(null);
						if (flag3)
						{
							base.StartDeadSessionChecking();
						}
						flag = true;
					}
					base.IsConnected = true;
					flag2 = true;
				}
				finally
				{
					if (!flag2)
					{
						ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "MailboxSession::Connect. Operation failed. mailbox = {0}.", this.mailboxOwner.LegacyDn);
					}
					else
					{
						ExTraceGlobals.SessionTracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxSession::Connect. Operation succeeded. mailbox = {0}.", this.mailboxOwner.LegacyDn);
					}
				}
				result = flag;
			}
			return result;
		}

		public override void Connect()
		{
			using (base.CreateSessionGuard("Connect"))
			{
				this.ConnectWithStatus();
			}
		}

		protected override MapiStore ForceOpen(MapiStore linkedStore)
		{
			return this.ForceOpen(linkedStore, false);
		}

		protected MapiStore ForceOpen(MapiStore linkedStore, bool unifiedSession)
		{
			MapiStore result;
			using (base.CreateSessionGuard("ForceOpen"))
			{
				bool flag = false;
				MapiStore mapiStore = null;
				ClientSecurityContext clientSecurityContext = null;
				try
				{
					if (this.ShouldThrowWrongServerException(this.mailboxOwner))
					{
						MapiExceptionMailboxInTransit innerException = new MapiExceptionMailboxInTransit("Detected site violation", 0, 1292, null, null);
						throw new WrongServerException(ServerStrings.PrincipalFromDifferentSite, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.mailboxOwner.MailboxInfo.Location.ServerVersion, innerException);
					}
					if (MailboxSession.IsMemberLogonToGroupMailbox(this.mailboxOwner, base.LogonType))
					{
						if (base.UnifiedGroupMemberType == UnifiedGroupMemberType.Unknown)
						{
							if (ExTraceGlobals.GroupMailboxSessionTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.GroupMailboxSessionTracer.TraceError<StackTrace>((long)this.GetHashCode(), "GroupMailboxAccess: Not a valid way to create session for the group mailbox: {0}", new StackTrace(true));
							}
							throw new AccessDeniedException(ServerStrings.InvalidMechanismToAccessGroupMailbox);
						}
						base.StoreFlag |= OpenStoreFlag.ShowAllFIDCs;
					}
					byte[] tenantHint = StoreSession.GetTenantHint(this.mailboxOwner);
					bool flag2 = false;
					if (this.mailboxOwner.MailboxInfo.IsArchive && this.mailboxOwner.MailboxInfo.IsRemote)
					{
						flag2 = true;
						if ((this.mailboxOwner.MailboxInfo.ArchiveStatus & ArchiveStatusFlags.Active) != ArchiveStatusFlags.Active)
						{
							throw new MailboxOfflineException(ServerStrings.RemoteArchiveOffline);
						}
					}
					flag2 = (flag2 || (!this.mailboxOwner.MailboxInfo.IsArchive && this.mailboxOwner.MailboxInfo.IsRemote));
					if (flag2)
					{
						mapiStore = this.InternalGetRemoteConnection();
					}
					else
					{
						this.connectFlag |= ConnectFlag.UseRpcContextPool;
						if (StoreSession.UseRPCContextPoolResiliency)
						{
							this.connectFlag |= ConnectFlag.UseResiliency;
						}
						WindowsIdentity windowsIdentity = this.identity as WindowsIdentity;
						if (windowsIdentity == null)
						{
							ClientIdentityInfo clientIdentityInfo = this.identity as ClientIdentityInfo;
							if (clientIdentityInfo != null)
							{
								if (this.mailboxOwner.MailboxInfo.IsArchive)
								{
									base.StoreFlag |= OpenStoreFlag.MailboxGuid;
									if (linkedStore == null)
									{
										bool flag3 = false;
										try
										{
											try
											{
												if (this != null)
												{
													this.BeginMapiCall();
													this.BeginServerHealthCall();
													flag3 = true;
												}
												if (StorageGlobals.MapiTestHookBeforeCall != null)
												{
													StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
												}
												mapiStore = MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), null, null, null, this.connectFlag, base.StoreFlag, this.InternalPreferedCulture, clientIdentityInfo, base.ClientInfoString, tenantHint, unifiedSession);
											}
											catch (MapiPermanentException ex)
											{
												throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex, this, this, "{0}. MapiException = {1}.", new object[]
												{
													string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
													{
														this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
														this.userLegacyDn,
														this.mailboxOwner.LegacyDn,
														this.mailboxOwner.MailboxInfo.MailboxGuid
													}),
													ex
												});
											}
											catch (MapiRetryableException ex2)
											{
												throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex2, this, this, "{0}. MapiException = {1}.", new object[]
												{
													string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
													{
														this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
														this.userLegacyDn,
														this.mailboxOwner.LegacyDn,
														this.mailboxOwner.MailboxInfo.MailboxGuid
													}),
													ex2
												});
											}
											goto IL_1790;
										}
										finally
										{
											try
											{
												if (this != null)
												{
													this.EndMapiCall();
													if (flag3)
													{
														this.EndServerHealthCall();
													}
												}
											}
											finally
											{
												if (StorageGlobals.MapiTestHookAfterCall != null)
												{
													StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
												}
											}
										}
									}
									bool flag4 = false;
									try
									{
										try
										{
											if (this != null)
											{
												this.BeginMapiCall();
												this.BeginServerHealthCall();
												flag4 = true;
											}
											if (StorageGlobals.MapiTestHookBeforeCall != null)
											{
												StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
											}
											mapiStore = linkedStore.OpenAlternateMailbox(this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), base.StoreFlag, this.InternalPreferedCulture, base.ClientInfoString, tenantHint);
										}
										catch (MapiPermanentException ex3)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex3, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
												{
													this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
													this.userLegacyDn,
													this.mailboxOwner.LegacyDn,
													this.mailboxOwner.MailboxInfo.MailboxGuid
												}),
												ex3
											});
										}
										catch (MapiRetryableException ex4)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex4, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
												{
													this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
													this.userLegacyDn,
													this.mailboxOwner.LegacyDn,
													this.mailboxOwner.MailboxInfo.MailboxGuid
												}),
												ex4
											});
										}
										goto IL_1790;
									}
									finally
									{
										try
										{
											if (this != null)
											{
												this.EndMapiCall();
												if (flag4)
												{
													this.EndServerHealthCall();
												}
											}
										}
										finally
										{
											if (StorageGlobals.MapiTestHookAfterCall != null)
											{
												StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
											}
										}
									}
								}
								if (this.mailboxOwner.MailboxInfo.IsAggregated)
								{
									mapiStore = this.InternalGetAggregatedMailboxConnection(linkedStore, clientIdentityInfo, tenantHint);
									goto IL_1790;
								}
								if (this.mailboxOwner.MailboxInfo.MailboxGuid != Guid.Empty && !this.mailboxOwner.MailboxInfo.MailboxDatabase.IsNullOrEmpty())
								{
									base.StoreFlag |= OpenStoreFlag.MailboxGuid;
									if (this.mailboxOwner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
									{
										clientSecurityContext = ClientSecurityContext.DuplicateAuthZContextHandle(clientIdentityInfo.hAuthZ);
										GroupMailboxAuthorizationHandler.AddGroupMailboxAccessSid(clientSecurityContext, this.mailboxOwner.MailboxInfo.MailboxGuid, base.UnifiedGroupMemberType);
										clientIdentityInfo = new ClientIdentityInfo(clientSecurityContext.ClientContextHandle.DangerousGetHandle(), clientIdentityInfo.sidUser, clientIdentityInfo.sidPrimaryGroup);
									}
									ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::ForceOpen. Use user's MailboxGuid. MailboxGuid = {0}.", this.InternalMailboxOwner);
									if (linkedStore == null)
									{
										bool flag5 = false;
										try
										{
											try
											{
												if (this != null)
												{
													this.BeginMapiCall();
													this.BeginServerHealthCall();
													flag5 = true;
												}
												if (StorageGlobals.MapiTestHookBeforeCall != null)
												{
													StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
												}
												mapiStore = MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), null, null, null, this.connectFlag, base.StoreFlag, this.InternalPreferedCulture, clientIdentityInfo, base.ClientInfoString, tenantHint, unifiedSession);
											}
											catch (MapiPermanentException ex5)
											{
												throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex5, this, this, "{0}. MapiException = {1}.", new object[]
												{
													string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
													ex5
												});
											}
											catch (MapiRetryableException ex6)
											{
												throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex6, this, this, "{0}. MapiException = {1}.", new object[]
												{
													string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
													ex6
												});
											}
											goto IL_1790;
										}
										finally
										{
											try
											{
												if (this != null)
												{
													this.EndMapiCall();
													if (flag5)
													{
														this.EndServerHealthCall();
													}
												}
											}
											finally
											{
												if (StorageGlobals.MapiTestHookAfterCall != null)
												{
													StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
												}
											}
										}
									}
									bool flag6 = false;
									try
									{
										try
										{
											if (this != null)
											{
												this.BeginMapiCall();
												this.BeginServerHealthCall();
												flag6 = true;
											}
											if (StorageGlobals.MapiTestHookBeforeCall != null)
											{
												StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
											}
											mapiStore = linkedStore.OpenAlternateMailbox(this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), base.StoreFlag, this.InternalPreferedCulture, base.ClientInfoString, tenantHint);
										}
										catch (MapiPermanentException ex7)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex7, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex7
											});
										}
										catch (MapiRetryableException ex8)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex8, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex8
											});
										}
										goto IL_1790;
									}
									finally
									{
										try
										{
											if (this != null)
											{
												this.EndMapiCall();
												if (flag6)
												{
													this.EndServerHealthCall();
												}
											}
										}
										finally
										{
											if (StorageGlobals.MapiTestHookAfterCall != null)
											{
												StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
											}
										}
									}
								}
								base.StoreFlag &= ~OpenStoreFlag.MailboxGuid;
								if (linkedStore == null)
								{
									bool flag7 = false;
									try
									{
										try
										{
											if (this != null)
											{
												this.BeginMapiCall();
												this.BeginServerHealthCall();
												flag7 = true;
											}
											if (StorageGlobals.MapiTestHookBeforeCall != null)
											{
												StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
											}
											mapiStore = MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn, null, null, null, this.connectFlag, base.StoreFlag, clientIdentityInfo, this.InternalPreferedCulture, base.ClientInfoString, tenantHint, unifiedSession);
										}
										catch (MapiPermanentException ex9)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex9, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex9
											});
										}
										catch (MapiRetryableException ex10)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex10, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex10
											});
										}
										goto IL_1790;
									}
									finally
									{
										try
										{
											if (this != null)
											{
												this.EndMapiCall();
												if (flag7)
												{
													this.EndServerHealthCall();
												}
											}
										}
										finally
										{
											if (StorageGlobals.MapiTestHookAfterCall != null)
											{
												StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
											}
										}
									}
								}
								bool flag8 = false;
								try
								{
									try
									{
										if (this != null)
										{
											this.BeginMapiCall();
											this.BeginServerHealthCall();
											flag8 = true;
										}
										if (StorageGlobals.MapiTestHookBeforeCall != null)
										{
											StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
										}
										mapiStore = linkedStore.OpenAlternateMailbox(this.mailboxOwner.LegacyDn, base.StoreFlag, this.InternalPreferedCulture, base.ClientInfoString, tenantHint);
									}
									catch (MapiPermanentException ex11)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex11, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
											ex11
										});
									}
									catch (MapiRetryableException ex12)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex12, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
											ex12
										});
									}
									goto IL_1790;
								}
								finally
								{
									try
									{
										if (this != null)
										{
											this.EndMapiCall();
											if (flag8)
											{
												this.EndServerHealthCall();
											}
										}
									}
									finally
									{
										if (StorageGlobals.MapiTestHookAfterCall != null)
										{
											StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
										}
									}
								}
							}
							throw new NotSupportedException(string.Format("The type of the identity  {0} is not supported.", this.identity.GetType()));
						}
						if (this.mailboxOwner.MailboxInfo.IsArchive)
						{
							base.StoreFlag |= OpenStoreFlag.MailboxGuid;
							ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::ForceOpen. Use user's MailboxGuid. MailboxGuid = {0}.", this.InternalMailboxOwner);
							if (linkedStore == null)
							{
								bool flag9 = false;
								try
								{
									try
									{
										if (this != null)
										{
											this.BeginMapiCall();
											this.BeginServerHealthCall();
											flag9 = true;
										}
										if (StorageGlobals.MapiTestHookBeforeCall != null)
										{
											StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
										}
										mapiStore = MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), null, null, null, this.connectFlag, base.StoreFlag, this.InternalPreferedCulture, windowsIdentity, base.ClientInfoString, tenantHint, unifiedSession);
									}
									catch (MapiPermanentException ex13)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex13, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
											{
												this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
												this.userLegacyDn,
												this.mailboxOwner.LegacyDn,
												this.mailboxOwner.MailboxInfo.MailboxGuid
											}),
											ex13
										});
									}
									catch (MapiRetryableException ex14)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex14, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
											{
												this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
												this.userLegacyDn,
												this.mailboxOwner.LegacyDn,
												this.mailboxOwner.MailboxInfo.MailboxGuid
											}),
											ex14
										});
									}
									goto IL_1790;
								}
								finally
								{
									try
									{
										if (this != null)
										{
											this.EndMapiCall();
											if (flag9)
											{
												this.EndServerHealthCall();
											}
										}
									}
									finally
									{
										if (StorageGlobals.MapiTestHookAfterCall != null)
										{
											StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
										}
									}
								}
							}
							bool flag10 = false;
							try
							{
								try
								{
									if (this != null)
									{
										this.BeginMapiCall();
										this.BeginServerHealthCall();
										flag10 = true;
									}
									if (StorageGlobals.MapiTestHookBeforeCall != null)
									{
										StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
									}
									mapiStore = linkedStore.OpenAlternateMailbox(this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), base.StoreFlag, this.InternalPreferedCulture, base.ClientInfoString, tenantHint);
								}
								catch (MapiPermanentException ex15)
								{
									throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex15, this, this, "{0}. MapiException = {1}.", new object[]
									{
										string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
										{
											this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
											this.userLegacyDn,
											this.mailboxOwner.LegacyDn,
											this.mailboxOwner.MailboxInfo.MailboxGuid
										}),
										ex15
									});
								}
								catch (MapiRetryableException ex16)
								{
									throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex16, this, this, "{0}. MapiException = {1}.", new object[]
									{
										string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, archive Guid = {3}.", new object[]
										{
											this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
											this.userLegacyDn,
											this.mailboxOwner.LegacyDn,
											this.mailboxOwner.MailboxInfo.MailboxGuid
										}),
										ex16
									});
								}
								goto IL_1790;
							}
							finally
							{
								try
								{
									if (this != null)
									{
										this.EndMapiCall();
										if (flag10)
										{
											this.EndServerHealthCall();
										}
									}
								}
								finally
								{
									if (StorageGlobals.MapiTestHookAfterCall != null)
									{
										StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
									}
								}
							}
						}
						if (!this.mailboxOwner.MailboxInfo.IsAggregated)
						{
							if (this.mailboxOwner.MailboxInfo.MailboxGuid != Guid.Empty && !this.mailboxOwner.MailboxInfo.MailboxDatabase.IsNullOrEmpty())
							{
								base.StoreFlag |= OpenStoreFlag.MailboxGuid;
								ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::ForceOpen. Use user's MailboxGuid. MailboxGuid = {0}.", this.InternalMailboxOwner);
								if (linkedStore == null)
								{
									bool flag11 = false;
									try
									{
										try
										{
											if (this != null)
											{
												this.BeginMapiCall();
												this.BeginServerHealthCall();
												flag11 = true;
											}
											if (StorageGlobals.MapiTestHookBeforeCall != null)
											{
												StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
											}
											mapiStore = MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), null, null, null, this.connectFlag, base.StoreFlag, this.InternalPreferedCulture, windowsIdentity, base.ClientInfoString, tenantHint, unifiedSession);
										}
										catch (MapiPermanentException ex17)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex17, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex17
											});
										}
										catch (MapiRetryableException ex18)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex18, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex18
											});
										}
										goto IL_1790;
									}
									finally
									{
										try
										{
											if (this != null)
											{
												this.EndMapiCall();
												if (flag11)
												{
													this.EndServerHealthCall();
												}
											}
										}
										finally
										{
											if (StorageGlobals.MapiTestHookAfterCall != null)
											{
												StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
											}
										}
									}
								}
								bool flag12 = false;
								try
								{
									try
									{
										if (this != null)
										{
											this.BeginMapiCall();
											this.BeginServerHealthCall();
											flag12 = true;
										}
										if (StorageGlobals.MapiTestHookBeforeCall != null)
										{
											StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
										}
										mapiStore = linkedStore.OpenAlternateMailbox(this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), base.StoreFlag, this.InternalPreferedCulture, base.ClientInfoString, tenantHint);
									}
									catch (MapiPermanentException ex19)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex19, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
											ex19
										});
									}
									catch (MapiRetryableException ex20)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex20, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
											ex20
										});
									}
									goto IL_1790;
								}
								finally
								{
									try
									{
										if (this != null)
										{
											this.EndMapiCall();
											if (flag12)
											{
												this.EndServerHealthCall();
											}
										}
									}
									finally
									{
										if (StorageGlobals.MapiTestHookAfterCall != null)
										{
											StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
										}
									}
								}
							}
							if (!string.IsNullOrEmpty(this.mailboxOwner.LegacyDn))
							{
								base.StoreFlag &= ~OpenStoreFlag.MailboxGuid;
								ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::ForceOpen. Use User's LegacyDistinguishedName. LegacyDistinguishedName = {0}.", this.InternalMailboxOwner);
								if (linkedStore == null)
								{
									bool flag13 = false;
									try
									{
										try
										{
											if (this != null)
											{
												this.BeginMapiCall();
												this.BeginServerHealthCall();
												flag13 = true;
											}
											if (StorageGlobals.MapiTestHookBeforeCall != null)
											{
												StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
											}
											mapiStore = MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn, null, null, null, this.connectFlag, base.StoreFlag, this.InternalPreferedCulture, windowsIdentity, base.ClientInfoString, tenantHint, unifiedSession);
										}
										catch (MapiPermanentException ex21)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex21, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex21
											});
										}
										catch (MapiRetryableException ex22)
										{
											throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex22, this, this, "{0}. MapiException = {1}.", new object[]
											{
												string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
												ex22
											});
										}
										goto IL_1790;
									}
									finally
									{
										try
										{
											if (this != null)
											{
												this.EndMapiCall();
												if (flag13)
												{
													this.EndServerHealthCall();
												}
											}
										}
										finally
										{
											if (StorageGlobals.MapiTestHookAfterCall != null)
											{
												StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
											}
										}
									}
								}
								bool flag14 = false;
								try
								{
									try
									{
										if (this != null)
										{
											this.BeginMapiCall();
											this.BeginServerHealthCall();
											flag14 = true;
										}
										if (StorageGlobals.MapiTestHookBeforeCall != null)
										{
											StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
										}
										mapiStore = linkedStore.OpenAlternateMailbox(this.mailboxOwner.LegacyDn, base.StoreFlag, this.InternalPreferedCulture, base.ClientInfoString, tenantHint);
									}
									catch (MapiPermanentException ex23)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex23, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
											ex23
										});
									}
									catch (MapiRetryableException ex24)
									{
										throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex24, this, this, "{0}. MapiException = {1}.", new object[]
										{
											string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}.", this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.LegacyDn),
											ex24
										});
									}
									goto IL_1790;
								}
								finally
								{
									try
									{
										if (this != null)
										{
											this.EndMapiCall();
											if (flag14)
											{
												this.EndServerHealthCall();
											}
										}
									}
									finally
									{
										if (StorageGlobals.MapiTestHookAfterCall != null)
										{
											StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
										}
									}
								}
							}
							ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "MailboxSession::ForceOpen. The user has neither MailboxGuid nor LegacyDistinguishedName. Owner = {0}.", this.mailboxOwner.MailboxInfo.DisplayName);
							throw new NotSupportedException("Must have legacyDN or Guids");
						}
						mapiStore = this.InternalGetAggregatedMailboxConnection(linkedStore, windowsIdentity, tenantHint);
					}
					IL_1790:
					base.IsDead = false;
					MapiStore mapiStore2 = mapiStore;
					PropertyDefinition[] array = this.mailboxProperties;
					base.SetMailboxStoreObject(MailboxStoreObject.Bind(this, mapiStore2, (array != null) ? ((ICollection<PropertyDefinition>)array) : MailboxSchema.Instance.AllProperties, this.useNamedProperties, this.mailboxProperties != null));
					flag = true;
				}
				finally
				{
					if (clientSecurityContext != null)
					{
						clientSecurityContext.Dispose();
					}
					if (!flag)
					{
						ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "MailboxSession::ForceOpen. Operation failed. mailbox = {0}.", this.mailboxOwner.LegacyDn);
						this.isConnected = false;
						base.SetMailboxStoreObject(null);
						if (mapiStore != null)
						{
							mapiStore.Dispose();
							mapiStore = null;
						}
					}
					else
					{
						ExTraceGlobals.SessionTracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxSession::ForceOpen. Operation succeeded. mailbox = {0}.", this.mailboxOwner.LegacyDn);
					}
				}
				result = mapiStore;
			}
			return result;
		}

		public override void Disconnect()
		{
			using (base.CheckDisposed("Disconnect"))
			{
				ExTraceGlobals.SessionTracer.Information((long)this.GetHashCode(), "MailboxSession::Disconnect.");
				if (!base.IsConnected)
				{
					ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "MailboxSession::Disconnect. The mailbox has not been connected yet. Mailbox = {0}.", this.MailboxOwnerLegacyDN);
					throw new ConnectionFailedPermanentException(ServerStrings.ExNotConnected);
				}
				ExTraceGlobals.SessionTracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxSession::Disconnect. Disconnect succeeded. mailbox = {0}.", this.MailboxOwnerLegacyDN);
				base.IsConnected = false;
			}
		}

		public MasterCategoryList GetMasterCategoryList()
		{
			return this.GetMasterCategoryList(false);
		}

		public MasterCategoryList GetMasterCategoryList(bool forceReload)
		{
			MasterCategoryList result;
			using (base.CheckDisposed("GetMasterCategoryList"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveMasterCategoryList, "CanHaveMasterCategoryList");
				MasterCategoryList masterCategoryList = this.InternalGetMasterCategoryList();
				masterCategoryList.Load(forceReload);
				result = masterCategoryList;
			}
			return result;
		}

		public void DeleteMasterCategoryList()
		{
			using (base.CheckDisposed("DeleteMasterCategoryList"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveMasterCategoryList, "CanHaveMasterCategoryList");
				MasterCategoryList.Delete(this);
			}
		}

		public void SaveMasterCategoryList()
		{
			using (base.CheckDisposed("SaveMasterCategoryList"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveMasterCategoryList, "CanHaveMasterCategoryList");
				if (this.masterCategoryList != null && this.masterCategoryList.IsLoaded)
				{
					this.masterCategoryList.Save();
				}
			}
		}

		public AggregateOperationResult UnsafeCopyItemsAndSetProperties(StoreId destinationFolderId, StoreId[] ids, PropertyDefinition[] propertyDefinitions, object[] values)
		{
			AggregateOperationResult result;
			using (this.CheckObjectState("UnsafeCopyItemsAndSetProperties"))
			{
				ExTraceGlobals.SessionTracer.Information<int>((long)this.GetHashCode(), "MailboxSession::UnsafeCopyItemsAndSetProperties. HashCode = {0}", this.GetHashCode());
				List<GroupOperationResult> list = new List<GroupOperationResult>();
				Dictionary<StoreObjectId, List<StoreId>> dictionary = new Dictionary<StoreObjectId, List<StoreId>>();
				base.GroupNonOccurrenceByFolder(ids, dictionary, list);
				base.ExecuteOperationOnObjects(dictionary, list, delegate(Folder sourceFolder, StoreId[] sourceObjectIds)
				{
					GroupOperationResult groupOperationResult = sourceFolder.UnsafeCopyItemsAndSetProperties(destinationFolderId, sourceObjectIds, propertyDefinitions, values);
					return new AggregateOperationResult(groupOperationResult.OperationResult, new GroupOperationResult[]
					{
						groupOperationResult
					});
				});
				result = Folder.CreateAggregateOperationResult(list);
			}
			return result;
		}

		public AggregateOperationResult UnsafeMoveItemsAndSetProperties(StoreId destinationFolderId, StoreId[] ids, PropertyDefinition[] propertyDefinitions, object[] values)
		{
			AggregateOperationResult result;
			using (this.CheckObjectState("UnsafeMovItemsAndSetProperties"))
			{
				ExTraceGlobals.SessionTracer.Information<int>((long)this.GetHashCode(), "MailboxSession::UnsafeMoveItemsAndSetProperties. HashCode = {0}", this.GetHashCode());
				List<GroupOperationResult> list = new List<GroupOperationResult>();
				Dictionary<StoreObjectId, List<StoreId>> dictionary = new Dictionary<StoreObjectId, List<StoreId>>();
				base.GroupNonOccurrenceByFolder(ids, dictionary, list);
				base.ExecuteOperationOnObjects(dictionary, list, delegate(Folder sourceFolder, StoreId[] sourceObjectIds)
				{
					GroupOperationResult groupOperationResult = sourceFolder.UnsafeMoveItemsAndSetProperties(destinationFolderId, sourceObjectIds, propertyDefinitions, values);
					return new AggregateOperationResult(groupOperationResult.OperationResult, new GroupOperationResult[]
					{
						groupOperationResult
					});
				});
				result = Folder.CreateAggregateOperationResult(list);
			}
			return result;
		}

		public CultureInfo[] GetMailboxCultures()
		{
			CultureInfo[] result;
			using (base.CreateSessionGuard("GetMailboxCultures"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveCulture, "CanHaveCulture");
				result = this.InternalGetMailboxCultures();
			}
			return result;
		}

		internal CultureInfo[] InternalGetMailboxCultures()
		{
			CultureInfo[] result;
			using (base.CheckDisposed("InternalGetMailboxCultures"))
			{
				List<CultureInfo> list = new List<CultureInfo>();
				if (base.InternalCulture != null)
				{
					list.Add(base.InternalCulture);
				}
				if (this.InternalMailboxOwner != null)
				{
					foreach (CultureInfo item in this.MailboxOwner.PreferredCultures)
					{
						list.Add(item);
					}
					ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::GetMailboxCultures. Get culture from AD for {0}.", this.InternalMailboxOwner);
				}
				list.Add(CultureInfo.CurrentCulture);
				list.Add(MailboxSession.productDefaultCulture);
				result = (from culture in list.Distinct<CultureInfo>()
				where !culture.Equals(CultureInfo.InvariantCulture)
				select culture).ToArray<CultureInfo>();
			}
			return result;
		}

		public bool IsGroupMailbox()
		{
			object obj = base.Mailbox.TryGetProperty(MailboxSchema.MailboxTypeDetail);
			return obj is int && StoreSession.IsGroupMailbox((int)obj);
		}

		public bool IsMailboxOof()
		{
			bool result;
			using (this.CheckObjectState("IsMailboxOof"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveOof, "CanHaveOof");
				base.Mailbox.ForceReload(new PropertyDefinition[]
				{
					MailboxSchema.MailboxOofState
				});
				bool? flag = base.Mailbox.TryGetProperty(MailboxSchema.MailboxOofState) as bool?;
				if (flag == null)
				{
					result = false;
				}
				else
				{
					result = flag.Value;
				}
			}
			return result;
		}

		public Guid GetPerUserGuid(Guid replGuid, byte[] globCount)
		{
			Guid perUserGuid;
			using (this.CheckObjectState("GetPerUserGuid"))
			{
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					perUserGuid = base.Mailbox.MapiStore.GetPerUserGuid(replGuid, globCount);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetPerUserGuid, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetPerUserGuid.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetPerUserGuid, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetPerUserGuid.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			return perUserGuid;
		}

		public byte[][] GetPerUserLongTermIds(Guid guid)
		{
			byte[][] perUserLongTermIds;
			using (this.CheckObjectState("GetPerUserLongTermIds"))
			{
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					perUserLongTermIds = base.Mailbox.MapiStore.GetPerUserLongTermIds(guid);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetPerUserLongTermIds, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetPerUserLongTermIds.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetPerUserLongTermIds, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetPerUserLongTermIds.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			return perUserLongTermIds;
		}

		public bool GetAllPerUserLongTermIds(byte[] lastLtid, out PerUserData[] perUserDatas)
		{
			bool allPerUserLongTermIds;
			using (this.CheckObjectState("GetAllPerUserLongTermIds"))
			{
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					allPerUserLongTermIds = base.Mailbox.MapiStore.GetAllPerUserLongTermIds(lastLtid, out perUserDatas);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetAllPerUserLongTermIds, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetAllPerUserLongTermIds.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetAllPerUserLongTermIds, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetAllPerUserLongTermIds.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			return allPerUserLongTermIds;
		}

		public void PrereadMessages(params StoreId[] storeIds)
		{
			if (storeIds != null)
			{
				byte[][] entryIds = StoreId.StoreIdsToEntryIds(storeIds);
				using (base.CheckDisposed("PrereadMessages"))
				{
					bool flag = false;
					try
					{
						if (this != null)
						{
							this.BeginMapiCall();
							this.BeginServerHealthCall();
							flag = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						base.Mailbox.MapiStore.PrereadMessages(entryIds);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSubmitMessage, ex, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession.PrereadMessages", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSubmitMessage, ex2, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession.PrereadMessages", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (this != null)
							{
								this.EndMapiCall();
								if (flag)
								{
									this.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
				}
			}
		}

		public COWSession CowSession
		{
			get
			{
				COWSession result;
				using (base.CheckDisposed("CowSession::get"))
				{
					result = this.copyOnWriteNotification;
				}
				return result;
			}
		}

		public COWSettings COWSettings
		{
			get
			{
				COWSettings result;
				using (base.CheckDisposed("COWSettings::get"))
				{
					result = ((this.copyOnWriteNotification == null) ? null : this.copyOnWriteNotification.Settings);
				}
				return result;
			}
		}

		public ulong? DumpsterSize
		{
			get
			{
				ulong? result;
				using (base.CheckDisposed("DumpsterSize::get"))
				{
					base.Mailbox.ForceReload(new PropertyDefinition[]
					{
						MailboxSchema.DumpsterQuotaUsedExtended
					});
					object obj = base.Mailbox.TryGetProperty(MailboxSchema.DumpsterQuotaUsedExtended);
					if (obj is PropertyError)
					{
						ExTraceGlobals.SessionTracer.TraceError<MailboxSession, PropertyError>((long)this.GetHashCode(), "{0}: We could not get size of this mailbox due to PropertyError {1}. Skipping it.", this, (PropertyError)obj);
						result = null;
					}
					else
					{
						ulong value = (ulong)((long)obj);
						result = new ulong?(value);
					}
				}
				return result;
			}
		}

		public bool? IsDumpsterOverQuota(Unlimited<ByteQuantifiedSize> dumpsterQuota)
		{
			bool? result;
			using (base.CheckDisposed("IsDumpsterOverQuota"))
			{
				if (dumpsterQuota.IsUnlimited)
				{
					result = new bool?(false);
				}
				else
				{
					ulong? dumpsterSize = this.DumpsterSize;
					if (dumpsterSize != null)
					{
						result = new bool?(dumpsterSize.Value > dumpsterQuota.Value.ToBytes());
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		protected override ObjectAccessGuard CheckObjectState(string methodName)
		{
			ObjectAccessGuard objectAccessGuard = base.CheckObjectState(methodName);
			bool flag = false;
			ObjectAccessGuard result;
			try
			{
				if (!base.IsConnected)
				{
					ExTraceGlobals.SessionTracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxSession::{0}. The mailbox session is not connected yet.", methodName);
					throw new InvalidOperationException(ServerStrings.ExStoreSessionDisconnected);
				}
				flag = true;
				result = objectAccessGuard;
			}
			finally
			{
				if (!flag)
				{
					objectAccessGuard.Dispose();
				}
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.IsInternallyOpenedDelegateAccess && !this.CanDispose)
				{
					throw new InvalidOperationException("Consumer should not have disposed this session as it's opened for calendar delegate access and managed by XSO internally.");
				}
				this.InternalDisposeServerObjects();
				if (this.copyOnWriteNotification != null)
				{
					this.copyOnWriteNotification.Dispose();
					this.copyOnWriteNotification = null;
				}
				Util.DisposeIfPresent(this.delegateSessionManager);
				this.delegateSessionManager = null;
				this.mailboxOwner = null;
			}
			base.InternalDispose(disposing);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			DisposeTracker result;
			using (base.CreateSessionGuard("GetDisposeTracker"))
			{
				result = DisposeTracker.Get<MailboxSession>(this);
			}
			return result;
		}

		internal bool CanDispose
		{
			get
			{
				bool result;
				using (base.CreateSessionGuard("CanDispose::get"))
				{
					result = this.canDispose;
				}
				return result;
			}
			set
			{
				using (base.CreateSessionGuard("CanDispose::set"))
				{
					if (this.IsInternallyOpenedDelegateAccess)
					{
						this.canDispose = value;
					}
				}
			}
		}

		internal MailboxSessionSharableDataManager SharedDataManager
		{
			get
			{
				MailboxSessionSharableDataManager result;
				using (base.CreateSessionGuard("SharedDataManager::get"))
				{
					result = this.sharedDataManager;
				}
				return result;
			}
		}

		public override bool IsRemote
		{
			get
			{
				bool isRemote;
				using (base.CheckDisposed("IsRemote::get"))
				{
					isRemote = this.MailboxOwner.MailboxInfo.IsRemote;
				}
				return isRemote;
			}
		}

		public override Guid MdbGuid
		{
			get
			{
				Guid databaseGuid;
				using (base.CheckDisposed("MdbGuid::get"))
				{
					databaseGuid = this.MailboxOwner.MailboxInfo.GetDatabaseGuid();
				}
				return databaseGuid;
			}
		}

		public override Guid MailboxGuid
		{
			get
			{
				Guid mailboxGuid;
				using (base.CheckDisposed("MailboxGuid::get"))
				{
					mailboxGuid = this.MailboxOwner.MailboxInfo.MailboxGuid;
				}
				return mailboxGuid;
			}
		}

		public override string ServerFullyQualifiedDomainName
		{
			get
			{
				string serverFqdn;
				using (base.CheckDisposed("ServerFullyQualifiedDomainName::get"))
				{
					serverFqdn = this.MailboxOwner.MailboxInfo.Location.ServerFqdn;
				}
				return serverFqdn;
			}
		}

		public override OrganizationId OrganizationId
		{
			get
			{
				OrganizationId organizationId;
				using (base.CheckDisposed("OrganizationId::get"))
				{
					organizationId = this.MailboxOwner.MailboxInfo.OrganizationId;
				}
				return organizationId;
			}
		}

		internal bool IsInternallyOpenedDelegateAccess
		{
			get
			{
				bool result;
				using (base.CreateSessionGuard("IsInternallyOpenedDelegateAccess::get"))
				{
					if (this.masterMailboxSession != null)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}
		}

		public MailboxSession MasterMailboxSession
		{
			get
			{
				MailboxSession result;
				using (base.CheckDisposed("MasterMailboxSession::get"))
				{
					result = this.masterMailboxSession;
				}
				return result;
			}
			set
			{
				using (base.CheckDisposed("MasterMailboxSession::set"))
				{
					this.masterMailboxSession = value;
				}
			}
		}

		internal void CheckPrivateItemsAccessPermission(string delegateUserLegacyDn)
		{
			using (base.CreateSessionGuard("CheckPrivateItemsAccessPermission"))
			{
				this.filterPrivateItems = true;
				if (this.IsGroupMailbox())
				{
					this.filterPrivateItems = false;
				}
				ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(new Participant(null, delegateUserLegacyDn, "EX"), ParticipantEntryIdConsumer.SupportsADParticipantEntryId);
				try
				{
					byte[] delegateBytes = participantEntryId.ToByteArray();
					StoreObjectId defaultFolderId = this.defaultFolderManager.GetDefaultFolderId(DefaultFolderType.FreeBusyData);
					if (defaultFolderId != null)
					{
						this.CheckFilterPrivateItemsFromFreeBusy(defaultFolderId, delegateBytes);
					}
				}
				catch (StoragePermanentException arg)
				{
					ExTraceGlobals.SessionTracer.TraceError<string, string, StoragePermanentException>(0L, "MailboxSession::CheckPrivateItemsAccessPermission. Hit unknown exception and we ignore. Mailbox = {0}, DelegateUser = {1}, Exception = {2}.", this.mailboxOwner.MailboxInfo.DisplayName, delegateUserLegacyDn, arg);
				}
				catch (StorageTransientException arg2)
				{
					ExTraceGlobals.SessionTracer.TraceError<string, string, StorageTransientException>(0L, "MailboxSession::CheckPrivateItemsAccessPermission. Hit unknown exception and we ignore. Mailbox = {0}, DelegateUser = {1}, Exception = {2}.", this.mailboxOwner.MailboxInfo.DisplayName, delegateUserLegacyDn, arg2);
				}
			}
		}

		private void CheckFilterPrivateItemsFromFreeBusy(StoreObjectId freeBusyFolderId, byte[] delegateBytes)
		{
			VersionedId localFreeBusyMessageId = this.GetLocalFreeBusyMessageId(freeBusyFolderId);
			if (localFreeBusyMessageId != null)
			{
				using (Item item = Item.Bind(this, localFreeBusyMessageId, MailboxSession.DelegateDefinitions))
				{
					byte[][] valueOrDefault = item.GetValueOrDefault<byte[][]>(InternalSchema.DelegateEntryIds2);
					int[] valueOrDefault2 = item.GetValueOrDefault<int[]>(InternalSchema.DelegateFlags);
					if (valueOrDefault == null || valueOrDefault2 == null)
					{
						return;
					}
					if (valueOrDefault.Length != valueOrDefault2.Length)
					{
						return;
					}
					for (int i = 0; i < valueOrDefault.Length; i++)
					{
						byte[] y = valueOrDefault[i];
						if (ArrayComparer<byte>.Comparer.Equals(delegateBytes, y))
						{
							this.filterPrivateItems = (valueOrDefault2[i] != 1);
							break;
						}
					}
					return;
				}
			}
			ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::CheckFilterPrivateItemsFromFreeBusy. No FreeBusyMessage was found from the FreeBusy folder. Mailbox = {0}.", this.InternalMailboxOwner);
		}

		public string DisplayName
		{
			get
			{
				string displayName;
				using (this.CheckObjectState("DisplayName::get"))
				{
					displayName = this.MailboxOwner.MailboxInfo.DisplayName;
				}
				return displayName;
			}
		}

		public IExchangePrincipal InternalMailboxOwner
		{
			get
			{
				IExchangePrincipal result;
				using (base.CheckDisposed("InternalMailboxOwner::get"))
				{
					result = this.mailboxOwner;
				}
				return result;
			}
		}

		private bool AreIdsEquivalent(StoreObjectId systemFolderId, StoreObjectId idToCompare)
		{
			if (systemFolderId != null && systemFolderId.ProviderLevelItemId.Length > 0 && idToCompare != null && idToCompare.ProviderLevelItemId.Length > 0)
			{
				if (!idToCompare.IsFolderId)
				{
					idToCompare = IdConverter.GetParentIdFromMessageId(idToCompare);
				}
				return systemFolderId.Equals(idToCompare);
			}
			return false;
		}

		public override IActivitySession ActivitySession
		{
			get
			{
				IActivitySession value;
				using (base.CheckDisposed("ActivitySession::get"))
				{
					value = this.activitySessionHook.Value;
				}
				return value;
			}
		}

		internal override void CheckSystemFolderAccess(StoreObjectId id)
		{
			using (base.CreateSessionGuard("CheckSystemFolderAccess"))
			{
				if (!this.isDefaultFolderManagerBeingInitialized && base.LogonType != LogonType.SystemService)
				{
					if (this.UseSystemFolder && base.LogonType != LogonType.Transport)
					{
						StoreObjectId defaultFolderId = this.defaultFolderManager.GetDefaultFolderId(DefaultFolderType.System);
						if (this.AreIdsEquivalent(defaultFolderId, id))
						{
							throw new AccessDeniedException(ServerStrings.ExSystemFolderAccessDenied);
						}
					}
					if (this.UseAdminAuditLogsFolder && !this.bypassAuditsFolderAccessChecking)
					{
						StoreObjectId adminAuditLogsFolderId = null;
						this.BypassAuditsFolderAccessChecking(delegate
						{
							adminAuditLogsFolderId = this.defaultFolderManager.GetDefaultFolderId(DefaultFolderType.AdminAuditLogs);
						});
						if (this.AreIdsEquivalent(adminAuditLogsFolderId, id))
						{
							throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsFolderAccessDenied);
						}
					}
					if (this.UseAuditsFolder && !this.bypassAuditsFolderAccessChecking)
					{
						StoreObjectId auditsFolderId = null;
						this.BypassAuditsFolderAccessChecking(delegate
						{
							auditsFolderId = this.defaultFolderManager.GetDefaultFolderId(DefaultFolderType.Audits);
						});
						if (this.AreIdsEquivalent(auditsFolderId, id))
						{
							throw new AccessDeniedException(ServerStrings.ExAuditsFolderAccessDenied);
						}
					}
				}
			}
		}

		public void EnablePrivateItemsFilter()
		{
			using (base.CheckDisposed("EnablePrivateItemsFilter"))
			{
				this.disableFilterPrivateItems = false;
			}
		}

		public void DisablePrivateItemsFilter()
		{
			using (base.CheckDisposed("DisablePrivateItemsFilter"))
			{
				this.disableFilterPrivateItems = true;
			}
		}

		public bool PrivateItemsFilterDisabled
		{
			get
			{
				bool result;
				using (base.CheckDisposed("PrivateItemsFilterDisabled::get"))
				{
					result = this.disableFilterPrivateItems;
				}
				return result;
			}
		}

		public bool FilterPrivateItems
		{
			get
			{
				bool result;
				using (base.CheckDisposed("FilterPrivateItems::get"))
				{
					result = (this.ShouldFilterPrivateItems && !this.disableFilterPrivateItems);
				}
				return result;
			}
		}

		public override ContactFolders ContactFolders
		{
			get
			{
				ContactFolders result;
				using (base.CheckDisposed("ContactFolders::get"))
				{
					if (this.contactFolders == null)
					{
						this.contactFolders = ContactFolders.Load(XSOFactory.Default, this);
					}
					result = this.contactFolders;
				}
				return result;
			}
		}

		public bool ShouldFilterPrivateItems
		{
			get
			{
				bool result;
				using (base.CheckDisposed("ShouldFilterPrivateItems::get"))
				{
					result = this.filterPrivateItems;
				}
				return result;
			}
		}

		internal MasterCategoryList InternalGetMasterCategoryList()
		{
			MasterCategoryList result;
			using (base.CreateSessionGuard("InternalGetMasterCategoryList"))
			{
				if (this.masterCategoryList == null)
				{
					this.masterCategoryList = new MasterCategoryList(this);
				}
				result = this.masterCategoryList;
			}
			return result;
		}

		internal bool InternalIsConfigurationFolder(StoreObjectId id)
		{
			bool result;
			using (base.CreateSessionGuard("InternalIsConfigurationFolder"))
			{
				if (this.isDefaultFolderManagerBeingInitialized)
				{
					result = id.Equals(StoreObjectId.FromProviderSpecificId(base.Mailbox.MapiStore.GetNonIpmSubtreeFolderEntryId(), StoreObjectType.Folder));
				}
				else
				{
					result = (this.IsDefaultFolderType(id) == DefaultFolderType.Configuration);
				}
			}
			return result;
		}

		internal override bool OnBeforeItemChange(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, CallbackContext callbackContext)
		{
			bool result;
			using (base.CreateSessionGuard("OnBeforeItemChange"))
			{
				base.OnBeforeItemChange(operation, session, itemId, item, callbackContext);
				if (operation == ItemChangeOperation.Submit)
				{
					this.CheckIfItemNeedsRecipientsGroupExpansion(item);
				}
				if (item != null)
				{
					this.CheckForImplicitMarkAsNotClutter(operation, item.PropertyBag);
				}
				if (this.ActivitySession != null)
				{
					this.ActivitySession.CaptureActivityBeforeItemChange(operation, itemId, item);
				}
				if (this.copyOnWriteNotification != null)
				{
					result = this.copyOnWriteNotification.OnBeforeItemChange(operation, session, itemId, item, callbackContext);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal override void OnAfterItemChange(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, ConflictResolutionResult result, CallbackContext callbackContext)
		{
			using (base.CreateSessionGuard("OnAfterItemChange"))
			{
				base.OnAfterItemChange(operation, session, itemId, item, result, callbackContext);
				if (this.copyOnWriteNotification != null)
				{
					this.copyOnWriteNotification.OnAfterItemChange(operation, session, itemId, item, result, callbackContext);
				}
			}
		}

		internal override bool OnBeforeItemSave(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, CallbackContext callbackContext)
		{
			bool result;
			using (base.CreateSessionGuard("OnBeforeItemSave"))
			{
				base.OnBeforeItemSave(operation, session, itemId, item, callbackContext);
				if (this.copyOnWriteNotification != null)
				{
					result = this.copyOnWriteNotification.OnBeforeItemSave(operation, session, itemId, item, callbackContext);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal override void OnAfterItemSave(ItemChangeOperation operation, StoreSession session, StoreId itemId, CoreItem item, ConflictResolutionResult result, CallbackContext callbackContext)
		{
			using (base.CreateSessionGuard("OnAfterItemSave"))
			{
				base.OnAfterItemSave(operation, session, itemId, item, result, callbackContext);
				if (this.copyOnWriteNotification != null)
				{
					this.copyOnWriteNotification.OnAfterItemSave(operation, session, itemId, item, result, callbackContext);
				}
			}
		}

		internal override bool OnBeforeFolderChange(FolderChangeOperation operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, CallbackContext callbackContext)
		{
			bool result;
			using (base.CreateSessionGuard("OnBeforeFolderChange"))
			{
				base.OnBeforeFolderChange(operation, flags, sourceSession, destinationSession, sourceFolderId, destinationFolderId, itemIds, callbackContext);
				if (this.copyOnWriteNotification != null)
				{
					result = this.copyOnWriteNotification.OnBeforeFolderChange(operation, flags, sourceSession, destinationSession, sourceFolderId, destinationFolderId, itemIds, callbackContext);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal override void OnAfterFolderChange(FolderChangeOperation operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, GroupOperationResult result, CallbackContext callbackContext)
		{
			using (base.CreateSessionGuard("OnAfterFolderChange"))
			{
				base.OnAfterFolderChange(operation, flags, sourceSession, destinationSession, sourceFolderId, destinationFolderId, itemIds, result, callbackContext);
				if (result != null && result.OperationResult != OperationResult.Failed && (destinationSession == null || sourceSession == destinationSession))
				{
					if (this.ActivitySession != null)
					{
						this.ActivitySession.CaptureActivityAfterFolderChange(operation, flags, result.ObjectIds, result.ResultObjectIds, sourceFolderId, destinationFolderId);
					}
					this.CaptureMarkAsClutterOrNotClutter(operation, flags, result, sourceFolderId, destinationFolderId);
				}
				if (this.copyOnWriteNotification != null)
				{
					this.copyOnWriteNotification.OnAfterFolderChange(operation, flags, sourceSession, destinationSession, sourceFolderId, destinationFolderId, itemIds, result, callbackContext);
				}
			}
		}

		internal override void OnBeforeFolderBind(StoreObjectId folderId, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(folderId, "folderId");
			using (base.CreateSessionGuard("OnBeforeFolderBind"))
			{
				base.OnBeforeFolderBind(folderId, callbackContext);
				if (this.copyOnWriteNotification != null)
				{
					this.copyOnWriteNotification.OnBeforeFolderBind(this, folderId, callbackContext);
				}
			}
		}

		internal override void OnAfterFolderBind(StoreObjectId folderId, CoreFolder folder, bool success, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(folderId, "folderId");
			using (base.CreateSessionGuard("OnAfterFolderBind"))
			{
				base.OnAfterFolderBind(folderId, folder, success, callbackContext);
				if (this.copyOnWriteNotification != null)
				{
					this.copyOnWriteNotification.OnAfterFolderBind(this, folderId, folder, success, callbackContext);
				}
			}
		}

		internal override GroupOperationResult GetCallbackResults()
		{
			GroupOperationResult result;
			using (base.CreateSessionGuard("GetCallbackResults"))
			{
				if (this.copyOnWriteNotification != null)
				{
					result = this.copyOnWriteNotification.GetCallbackResults();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal void BypassAuditsFolderAccessChecking(Action action)
		{
			Util.ThrowOnNullArgument(action, "action");
			using (base.CreateSessionGuard("BypassAuditsFolderAccessChecking"))
			{
				bool flag = !this.bypassAuditsFolderAccessChecking;
				this.bypassAuditsFolderAccessChecking = true;
				try
				{
					action();
				}
				finally
				{
					if (flag)
					{
						this.bypassAuditsFolderAccessChecking = false;
					}
				}
			}
		}

		internal void BypassAuditing(Action action)
		{
			Util.ThrowOnNullArgument(action, "action");
			using (base.CreateSessionGuard("BypassAuditing"))
			{
				bool flag = !this.bypassAuditing;
				this.bypassAuditing = true;
				try
				{
					action();
				}
				finally
				{
					if (flag)
					{
						this.bypassAuditing = false;
					}
				}
			}
		}

		internal void DoNothingIfBypassAuditing(Action action)
		{
			Util.ThrowOnNullArgument(action, "action");
			using (base.CreateSessionGuard("DoNothingIfBypassAuditing"))
			{
				if (!this.bypassAuditing)
				{
					action();
				}
			}
		}

		internal void CheckForImplicitMarkAsNotClutter(ItemChangeOperation operation, ICorePropertyBag propertyBag)
		{
			using (base.CreateSessionGuard("CheckForImplicitMarkAsNotClutter"))
			{
				ArgumentValidator.ThrowIfNull("propertyBag", propertyBag);
				if (operation == ItemChangeOperation.Update)
				{
					bool valueOrDefault = propertyBag.GetValueOrDefault<bool>(ItemSchema.IsClutter, false);
					StoreObjectId valueOrDefault2 = propertyBag.GetValueOrDefault<StoreObjectId>(StoreObjectSchema.ParentItemId, null);
					bool flag = valueOrDefault2 != null && valueOrDefault2.Equals(this.GetDefaultFolderId(DefaultFolderType.Clutter));
					if (valueOrDefault || flag)
					{
						bool flag2 = propertyBag.IsPropertyDirty(InternalSchema.FlagStatus) && propertyBag.GetValueOrDefault<FlagStatus>(InternalSchema.FlagStatus, FlagStatus.NotFlagged) == FlagStatus.Flagged;
						bool flag3 = false;
						if (propertyBag.IsPropertyDirty(InternalSchema.LastVerbExecuted))
						{
							LastAction valueOrDefault3 = propertyBag.GetValueOrDefault<LastAction>(InternalSchema.LastVerbExecuted, LastAction.Open);
							if (valueOrDefault3 == LastAction.ReplyToSender || valueOrDefault3 == LastAction.ReplyToAll || valueOrDefault3 == LastAction.Forward || (valueOrDefault3 >= LastAction.VotingOptionMin && valueOrDefault3 <= LastAction.VotingOptionMax))
							{
								flag3 = true;
							}
						}
						if (flag2 || flag3)
						{
							propertyBag[InternalSchema.InferenceProcessingNeeded] = true;
							InferenceProcessingActions inferenceProcessingActions = (InferenceProcessingActions)propertyBag.GetValueOrDefault<long>(InternalSchema.InferenceProcessingActions, 0L);
							inferenceProcessingActions |= InferenceProcessingActions.ProcessImplicitMarkAsNotClutter;
							propertyBag[InternalSchema.InferenceProcessingActions] = (long)inferenceProcessingActions;
						}
					}
				}
			}
		}

		private void InternalInitializeDefaultFolders(IList<DefaultFolderType> foldersToInit, OpenMailboxSessionFlags openFlags)
		{
			if (ExTraceGlobals.SessionTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<string, OpenMailboxSessionFlags>((long)this.GetHashCode(), "MailboxSession::InternalInitializeDefaultFolders. Initializing default folders {0} with flags {1}", string.Join<DefaultFolderType>(", ", foldersToInit.ToArray<DefaultFolderType>()), openFlags);
			}
			this.isDefaultFolderManagerBeingInitialized = true;
			this.defaultFolderManager = DefaultFolderManager.Create(this, openFlags, foldersToInit);
			this.isDefaultFolderManagerBeingInitialized = false;
		}

		private void InternalLocalizeInitialDefaultFolders(OpenMailboxSessionFlags openFlags)
		{
			CultureInfo mailboxLocale = this.MailboxOwner.PreferredCultures.DefaultIfEmpty(this.InternalPreferedCulture).First<CultureInfo>();
			DefaultFolderManager defaultFolderManager = DefaultFolderManager.Create(this, openFlags, MailboxSession.DefaultFoldersToLocalizeOnFirstLogon);
			Exception[] array;
			defaultFolderManager.Localize(out array);
			if (array != null && array.Length > 0)
			{
				ExTraceGlobals.SessionTracer.TraceError<string, Exception>(0L, "MailboxSession::InternalLocalizeInitialDefaultFolders. Failed to localize default folders. Mailbox = {0}, exception = {1}.", this.MailboxOwnerLegacyDN, array[0]);
			}
			this.SetMailboxLocale(mailboxLocale);
		}

		public void SetMailboxLocale()
		{
			this.SetMailboxLocale(this.InternalPreferedCulture);
		}

		public void SetMailboxLocale(CultureInfo cultureInfo)
		{
			base.Mailbox[InternalSchema.IsMailboxLocalized] = true;
			base.Mailbox[InternalSchema.LocaleId] = 0;
			base.Mailbox[InternalSchema.LocaleId] = LocaleMap.GetLcidFromCulture(cultureInfo);
			base.Mailbox.Save();
			base.Mailbox.Load();
		}

		private void CheckIfItemNeedsRecipientsGroupExpansion(CoreItem item)
		{
			if (item != null && item.Recipients != null)
			{
				COWSettings cowsettings = this.COWSettings;
				if (cowsettings == null)
				{
					cowsettings = new COWSettings(this);
				}
				if (cowsettings.LegalHoldEnabled())
				{
					bool flag = false;
					foreach (CoreRecipient coreRecipient in item.Recipients)
					{
						object obj = coreRecipient.Participant.TryGetProperty(ParticipantSchema.IsDistributionList);
						if (!PropertyError.IsPropertyError(obj) && (bool)obj)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						item.PropertyBag[InternalSchema.NeedGroupExpansion] = true;
					}
				}
			}
		}

		public bool IsUnified
		{
			get
			{
				bool result;
				using (base.CheckDisposed("IsUnified::get"))
				{
					result = this.isUnified;
				}
				return result;
			}
			set
			{
				using (base.CheckDisposed("IsUnified::set"))
				{
					this.isUnified = value;
				}
			}
		}

		public bool IsAuditConfigFromUCCPolicyEnabled
		{
			get
			{
				bool value;
				using (base.CheckDisposed("IsAuditConfigFromUCCPolicyEnabled::get"))
				{
					if (this.isAuditConfigFromUCCPolicyEnabled == null)
					{
						VariantConfigurationSnapshot configuration = this.MailboxOwner.GetConfiguration();
						this.isAuditConfigFromUCCPolicyEnabled = new bool?(configuration != null && configuration.Ipaed.AuditConfigFromUCCPolicy.Enabled);
					}
					value = this.isAuditConfigFromUCCPolicyEnabled.Value;
				}
				return value;
			}
		}

		private static bool IsOwnerLogon(LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegatedUser)
		{
			return logonType == LogonType.Owner || (logonType == LogonType.BestAccess && (delegatedUser == null || string.IsNullOrEmpty(delegatedUser.LegacyDn) || string.Equals(delegatedUser.LegacyDn, owner.LegacyDn, StringComparison.OrdinalIgnoreCase)));
		}

		public bool TryToSyncSiteMailboxNow()
		{
			using (this.CheckObjectState("TryToSyncSiteMailboxNow"))
			{
				if (this.siteMailboxSynchronizerReference != null)
				{
					return this.siteMailboxSynchronizerReference.TryToSyncNow();
				}
			}
			return false;
		}

		private static void TriggerSiteMailboxSyncIfNeeded(StorageTransientException e, IExchangePrincipal mailbox, string clientInfoString)
		{
			MapiExceptionLogonFailed mapiExceptionLogonFailed = e.InnerException as MapiExceptionLogonFailed;
			if (mailbox.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox && mapiExceptionLogonFailed != null && !string.IsNullOrEmpty(clientInfoString) && clientInfoString.StartsWith("Client=OWA", StringComparison.OrdinalIgnoreCase))
			{
				using (SiteMailboxSynchronizerReference siteMailboxSynchronizer = SiteMailboxSynchronizerManager.Instance.GetSiteMailboxSynchronizer(mailbox, Utils.GetSyncClientString("Failed_OWA_Logon")))
				{
					siteMailboxSynchronizer.TryToSyncNow();
				}
			}
		}

		private static MailboxSession ConfigurableOpen(IExchangePrincipal mailbox, MailboxAccessInfo accessInfo, CultureInfo cultureInfo, string clientInfoString, LogonType logonType, PropertyDefinition[] mailboxProperties, MailboxSession.InitializationFlags initFlags, IList<DefaultFolderType> foldersToInit, IBudget budget)
		{
			return MailboxSession.ConfigurableOpen(mailbox, accessInfo, cultureInfo, clientInfoString, logonType, mailboxProperties, initFlags, foldersToInit, budget, false, null, UnifiedGroupMemberType.Unknown);
		}

		private static MailboxSession ConfigurableOpen(IExchangePrincipal mailbox, MailboxAccessInfo accessInfo, CultureInfo cultureInfo, string clientInfoString, LogonType logonType, PropertyDefinition[] mailboxProperties, MailboxSession.InitializationFlags initFlags, IList<DefaultFolderType> foldersToInit, IBudget budget, MailboxSessionSharableDataManager sharedDataManager)
		{
			return MailboxSession.ConfigurableOpen(mailbox, accessInfo, cultureInfo, clientInfoString, logonType, mailboxProperties, initFlags, foldersToInit, budget, false, sharedDataManager, UnifiedGroupMemberType.Unknown);
		}

		private static MailboxSession ConfigurableOpen(IExchangePrincipal mailbox, MailboxAccessInfo accessInfo, CultureInfo cultureInfo, string clientInfoString, LogonType logonType, PropertyDefinition[] mailboxProperties, MailboxSession.InitializationFlags initFlags, IList<DefaultFolderType> foldersToInit, IBudget budget, bool unifiedSession, MailboxSessionSharableDataManager sharedDataManager, UnifiedGroupMemberType memberType)
		{
			Util.ThrowOnNullArgument(mailbox, "mailbox");
			Util.ThrowOnNullArgument(accessInfo, "accessInfo");
			if (mailboxProperties != null && mailboxProperties.Length == 0)
			{
				throw new ArgumentException("mailboxProperties must be null or a non-zero length PropertyDefinition[]", "mailboxProperties");
			}
			DelegateLogonUser delegateLogonUser;
			if (accessInfo.AccessingUserAdEntry != null)
			{
				delegateLogonUser = new DelegateLogonUser(accessInfo.AccessingUserAdEntry);
			}
			else if (accessInfo.AccessingUserDn != null)
			{
				delegateLogonUser = new DelegateLogonUser(accessInfo.AccessingUserDn);
			}
			else
			{
				delegateLogonUser = new DelegateLogonUser(null);
			}
			OpenMailboxSessionFlags flags;
			MailboxSession.InternalBuildOpenMailboxSessionFlags(initFlags, logonType, foldersToInit, out flags);
			object obj = null;
			if (accessInfo.AuthenticatedUserPrincipal != null)
			{
				obj = accessInfo.AuthenticatedUserPrincipal.Identity;
			}
			else if (accessInfo.AuthenticatedUserContext != null)
			{
				if (accessInfo.AuthenticatedUserContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid))
				{
					obj = new ClientIdentityInfo(accessInfo.AuthenticatedUserContext.ClientContextHandle.DangerousGetHandle(), accessInfo.AuthenticatedUserContext.UserSid, new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null));
				}
				else
				{
					obj = StoreSession.FromAuthZContext(mailbox.MailboxInfo.OrganizationId.ToADSessionSettings(), accessInfo.AuthenticatedUserContext.ClientContextHandle);
				}
			}
			else if (accessInfo.AuthenticatedUserHandle != null)
			{
				obj = StoreSession.FromAuthZContext(mailbox.MailboxInfo.OrganizationId.ToADSessionSettings(), accessInfo.AuthenticatedUserHandle);
			}
			else if (accessInfo.ClientIdentityInfo != null)
			{
				obj = accessInfo.ClientIdentityInfo;
			}
			if (obj == null)
			{
				throw new ObjectNotFoundException(ServerStrings.UserCannotBeFoundFromContext(Marshal.GetLastWin32Error()));
			}
			return MailboxSession.CreateMailboxSession(logonType, mailbox, delegateLogonUser, obj, flags, cultureInfo, clientInfoString, mailboxProperties, foldersToInit, accessInfo.AuxiliaryIdentity, budget, unifiedSession, sharedDataManager, memberType);
		}

		public static MailboxSession Open(IExchangePrincipal mailboxOwner, WindowsPrincipal authenticatedUser, CultureInfo cultureInfo, string clientInfoString)
		{
			return MailboxSession.Open(mailboxOwner, authenticatedUser, cultureInfo, clientInfoString, true);
		}

		public static MailboxSession Open(IExchangePrincipal mailboxOwner, WindowsPrincipal authenticatedUser, CultureInfo cultureInfo, string clientInfoString, bool wantCachedConnection)
		{
			if (authenticatedUser == null)
			{
				throw new ArgumentNullException("authenticatedUser");
			}
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(authenticatedUser);
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Owner, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, string accessingUserDn, WindowsPrincipal authenticatedUser, CultureInfo cultureInfo, string clientInfoString)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			if (authenticatedUser == null)
			{
				throw new ArgumentNullException("authenticatedUser");
			}
			MailboxAccessInfo accessInfo;
			if (string.IsNullOrEmpty(accessingUserDn))
			{
				accessInfo = new MailboxAccessInfo(authenticatedUser);
			}
			else
			{
				accessInfo = new MailboxAccessInfo(accessingUserDn, authenticatedUser);
			}
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.BestAccess, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, string accessingUserDn, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, GenericIdentity auxiliaryIdentity)
		{
			return MailboxSession.OpenWithBestAccess(mailboxOwner, accessingUserDn, clientSecurityContext, cultureInfo, clientInfoString, auxiliaryIdentity, false);
		}

		private static MailboxAccessInfo GetMailboxAccessInfo(string accessingUserDn, ClientSecurityContext clientSecurityContext, GenericIdentity auxiliaryIdentity)
		{
			if (string.IsNullOrEmpty(accessingUserDn))
			{
				return new MailboxAccessInfo(clientSecurityContext, auxiliaryIdentity);
			}
			return new MailboxAccessInfo(accessingUserDn, clientSecurityContext, auxiliaryIdentity);
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, string accessingUserDn, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, GenericIdentity auxiliaryIdentity, bool unifiedSession)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			UnifiedGroupMemberType memberType = UnifiedGroupMemberType.Unknown;
			if (mailboxOwner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
			{
				AccessingUserInfo accessUserInfo = new AccessingUserInfo(accessingUserDn, null, mailboxOwner.MailboxInfo.OrganizationId, null);
				memberType = MailboxSession.GetUserMembershipType(mailboxOwner, accessUserInfo, clientSecurityContext, clientInfoString);
			}
			return MailboxSession.ConfigurableOpen(mailboxOwner, MailboxSession.GetMailboxAccessInfo(accessingUserDn, clientSecurityContext, auxiliaryIdentity), cultureInfo, clientInfoString, LogonType.BestAccess, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders, null, unifiedSession, null, memberType);
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, IADOrgPerson accessingUser, WindowsPrincipal authenticatedUser, CultureInfo cultureInfo, string clientInfoString)
		{
			if (accessingUser == null)
			{
				throw new ArgumentNullException("accessingUser");
			}
			MailboxSession mailboxSession = MailboxSession.OpenWithBestAccess(mailboxOwner, accessingUser.LegacyExchangeDN, authenticatedUser, cultureInfo, clientInfoString);
			if (mailboxSession.LogonType == LogonType.Delegated)
			{
				mailboxSession.delegateUser = accessingUser;
			}
			return mailboxSession;
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, IADOrgPerson accessingUser, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString)
		{
			return MailboxSession.OpenWithBestAccess(mailboxOwner, accessingUser, clientSecurityContext, cultureInfo, clientInfoString, false);
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, IADOrgPerson accessingUser, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, bool unifiedSession)
		{
			return MailboxSession.OpenWithBestAccess(mailboxOwner, accessingUser, clientSecurityContext, cultureInfo, clientInfoString, null, unifiedSession);
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, IADOrgPerson accessingUser, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, GenericIdentity auxiliaryIdentity)
		{
			return MailboxSession.OpenWithBestAccess(mailboxOwner, accessingUser, clientSecurityContext, cultureInfo, clientInfoString, auxiliaryIdentity, false);
		}

		public static MailboxSession OpenWithBestAccess(IExchangePrincipal mailboxOwner, IADOrgPerson accessingUser, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, GenericIdentity auxiliaryIdentity, bool unifiedSession)
		{
			if (accessingUser == null)
			{
				throw new ArgumentNullException("accessingUser");
			}
			UnifiedGroupMemberType memberType = UnifiedGroupMemberType.Unknown;
			if (mailboxOwner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
			{
				AccessingUserInfo accessUserInfo = new AccessingUserInfo(accessingUser.LegacyExchangeDN, accessingUser.ExternalDirectoryObjectId, mailboxOwner.MailboxInfo.OrganizationId, accessingUser.Id);
				memberType = MailboxSession.GetUserMembershipType(mailboxOwner, accessUserInfo, clientSecurityContext, clientInfoString);
			}
			MailboxSession mailboxSession = MailboxSession.ConfigurableOpen(mailboxOwner, MailboxSession.GetMailboxAccessInfo(accessingUser.LegacyExchangeDN, clientSecurityContext, auxiliaryIdentity), cultureInfo, clientInfoString, LogonType.BestAccess, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders, null, unifiedSession, null, memberType);
			if (mailboxSession.LogonType == LogonType.Delegated)
			{
				mailboxSession.delegateUser = accessingUser;
			}
			return mailboxSession;
		}

		public static MailboxSession OpenAsDelegate(IExchangePrincipal mailboxOwner, IADOrgPerson delegateUser, WindowsPrincipal authenticatedUser, CultureInfo cultureInfo, string clientInfoString)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			if (delegateUser == null)
			{
				throw new ArgumentNullException("delegateUser");
			}
			if (authenticatedUser == null)
			{
				throw new ArgumentNullException("authenticatedUser");
			}
			if (string.IsNullOrEmpty(delegateUser.LegacyExchangeDN))
			{
				ExTraceGlobals.SessionTracer.TraceError<string, string, string>(0L, "MailboxSession::OpenAsDelegate. delegateUser's LegacyDn is Null or Empty. mailboxOwner = {0}, delegateUser.DisplayName = {1}, delegateUser.LegacyExchangeDN = {2}.", mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName, delegateUser.LegacyExchangeDN);
				throw new AccessDeniedException(ServerStrings.ExMailboxAccessDenied(mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName));
			}
			MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(delegateUser, authenticatedUser);
			MailboxSession mailboxSession = MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Delegated, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
			if (mailboxSession.CanActAsOwner)
			{
				ExTraceGlobals.SessionTracer.TraceError<string>(0L, "MailboxSession::OpenAsDelegate. The user cannot act as a delegate. mailboxOwner.DisplayName = {0}.", mailboxOwner.MailboxInfo.DisplayName);
				mailboxSession.Dispose();
				throw new MailboxMustBeAccessedAsOwnerException(ServerStrings.ExMailboxMustBeAccessedAsOwner(mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName));
			}
			return mailboxSession;
		}

		public static MailboxSession OpenAsDelegate(IExchangePrincipal mailboxOwner, IADOrgPerson delegateUser, AuthzContextHandle authenticatedUserHandle, CultureInfo cultureInfo, string clientInfoString)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			if (delegateUser == null)
			{
				throw new ArgumentNullException("delegateUser");
			}
			if (authenticatedUserHandle == null || authenticatedUserHandle.IsInvalid)
			{
				throw new ArgumentNullException("authenticatedUserHandle");
			}
			if (string.IsNullOrEmpty(delegateUser.LegacyExchangeDN))
			{
				ExTraceGlobals.SessionTracer.TraceError<string, string, string>(0L, "MailboxSession::OpenAsDelegate. Delegated user's legacyDn is Empty or Null. mailboxOwner = {0}, delegateUser.DisplayName = {1}, delegateUser.legacyDn = {2}.", mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName, delegateUser.LegacyExchangeDN);
				throw new AccessDeniedException(ServerStrings.ExMailboxAccessDenied(mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName));
			}
			MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(delegateUser, authenticatedUserHandle);
			MailboxSession mailboxSession = MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Delegated, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
			if (mailboxSession.CanActAsOwner)
			{
				ExTraceGlobals.SessionTracer.TraceError<string, string>(0L, "MailboxSession::OpenAsDelegate. Delegated user cannot act as the owner. mailboxOwner = {0}, delegateUser.DisplayName = {1}.", mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName);
				mailboxSession.Dispose();
				throw new MailboxMustBeAccessedAsOwnerException(ServerStrings.ExMailboxMustBeAccessedAsOwner(mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName));
			}
			return mailboxSession;
		}

		public static MailboxSession OpenAsDelegate(IExchangePrincipal mailboxOwner, IADOrgPerson delegateUser, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			if (delegateUser == null)
			{
				throw new ArgumentNullException("delegateUser");
			}
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			if (string.IsNullOrEmpty(delegateUser.LegacyExchangeDN))
			{
				ExTraceGlobals.SessionTracer.TraceError<string, string, string>(0L, "MailboxSession::OpenAsDelegate. Delegated user's legacyDn is Empty or Null. mailboxOwner = {0}, delegateUser.DisplayName = {1}, delegateUser.legacyDn = {2}.", mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName, delegateUser.LegacyExchangeDN);
				throw new AccessDeniedException(ServerStrings.ExMailboxAccessDenied(mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName));
			}
			MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(delegateUser, clientSecurityContext);
			MailboxSession mailboxSession = MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Delegated, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders);
			if (mailboxSession.CanActAsOwner)
			{
				mailboxSession.Dispose();
				throw new MailboxMustBeAccessedAsOwnerException(ServerStrings.ExMailboxMustBeAccessedAsOwner(mailboxOwner.MailboxInfo.DisplayName, delegateUser.DisplayName));
			}
			return mailboxSession;
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString)
		{
			return MailboxSession.OpenAsAdmin(mailboxOwner, cultureInfo, clientInfoString, null);
		}

		public static MailboxSession OpenAsAdminWithBudget(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, IBudget budget)
		{
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()), null);
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Admin, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders, budget);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, GenericIdentity auxiliaryIdentity)
		{
			return MailboxSession.OpenAsAdmin(mailboxOwner, cultureInfo, clientInfoString, false, false, auxiliaryIdentity, false);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, GenericIdentity auxiliaryIdentity, bool nonInteractiveSession)
		{
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()), auxiliaryIdentity);
			MailboxSession.InitializationFlags initializationFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties;
			if (nonInteractiveSession)
			{
				initializationFlags |= MailboxSession.InitializationFlags.NonInteractiveSession;
			}
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.Admin, null, initializationFlags, MailboxSession.AllDefaultFolders);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc)
		{
			return MailboxSession.OpenAsAdmin(mailboxOwner, cultureInfo, clientInfoString, useLocalRpc, false);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb)
		{
			return MailboxSession.OpenAsAdmin(mailboxOwner, cultureInfo, clientInfoString, useLocalRpc, ignoreHomeMdb, false);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb, bool readOnly)
		{
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
			return MailboxSession.OpenAsAdmin(mailboxOwner, accessInfo, LogonType.Admin, cultureInfo, clientInfoString, useLocalRpc, ignoreHomeMdb, false, false, readOnly);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb, GenericIdentity auxiliaryIdentity, bool allowAdminLocalization = false)
		{
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()), auxiliaryIdentity);
			return MailboxSession.OpenAsAdmin(mailboxOwner, accessInfo, LogonType.Admin, cultureInfo, clientInfoString, useLocalRpc, ignoreHomeMdb, false, allowAdminLocalization, false);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, string accessingUserLegacyDn, WindowsPrincipal accessingWindowsPrincipal, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb)
		{
			return MailboxSession.OpenAsAdmin(mailboxOwner, accessingUserLegacyDn, accessingWindowsPrincipal, cultureInfo, clientInfoString, useLocalRpc, ignoreHomeMdb, false);
		}

		public static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, string accessingUserLegacyDn, WindowsPrincipal accessingWindowsPrincipal, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb, bool useRecoveryDatabase)
		{
			MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(accessingUserLegacyDn, accessingWindowsPrincipal);
			return MailboxSession.OpenAsAdmin(mailboxOwner, accessInfo, LogonType.DelegatedAdmin, cultureInfo, clientInfoString, useLocalRpc, ignoreHomeMdb, useRecoveryDatabase, false, false);
		}

		private static MailboxSession OpenAsAdmin(IExchangePrincipal mailboxOwner, MailboxAccessInfo accessInfo, LogonType logonType, CultureInfo cultureInfo, string clientInfoString, bool useLocalRpc, bool ignoreHomeMdb, bool recoveryDatabase, bool allowAdminLocalization = false, bool readOnly = false)
		{
			MailboxSession.InitializationFlags initializationFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties;
			if (useLocalRpc)
			{
				initializationFlags |= MailboxSession.InitializationFlags.RequestLocalRpc;
			}
			if (ignoreHomeMdb)
			{
				initializationFlags |= MailboxSession.InitializationFlags.OverrideHomeMdb;
			}
			if (recoveryDatabase)
			{
				initializationFlags |= MailboxSession.InitializationFlags.UseRecoveryDatabase;
				initializationFlags &= ~MailboxSession.InitializationFlags.CopyOnWrite;
			}
			if (allowAdminLocalization)
			{
				initializationFlags |= MailboxSession.InitializationFlags.AllowAdminLocalization;
			}
			if (readOnly)
			{
				initializationFlags |= MailboxSession.InitializationFlags.ReadOnly;
			}
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, logonType, null, initializationFlags, MailboxSession.AllDefaultFolders);
		}

		public static MailboxSession OpenAsSystemService(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString)
		{
			return MailboxSession.OpenAsSystemService(mailboxOwner, cultureInfo, clientInfoString, false);
		}

		public static MailboxSession OpenAsSystemService(IExchangePrincipal mailboxOwner, CultureInfo cultureInfo, string clientInfoString, bool readOnly)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
			MailboxSession.InitializationFlags initializationFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties;
			if (readOnly)
			{
				initializationFlags |= MailboxSession.InitializationFlags.ReadOnly;
			}
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, cultureInfo, clientInfoString, LogonType.SystemService, null, initializationFlags, MailboxSession.AllDefaultFolders);
		}

		public static MailboxSession OpenAsMrs(IExchangePrincipal mailboxOwner, MailboxSession.InitializationFlags extraInitializationFlags, string clientInfoString)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			return MailboxSession.ConfigurableOpen(mailboxOwner, new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent())), CultureInfo.InvariantCulture, clientInfoString, LogonType.SystemService, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties | extraInitializationFlags, MailboxSession.AllDefaultFolders);
		}

		public static MailboxSession OpenAsTransport(IExchangePrincipal mailboxOwner, string clientInfoString)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			if (clientInfoString == null)
			{
				throw new ArgumentNullException("clientInfoString");
			}
			if (clientInfoString.Length == 0)
			{
				throw new ArgumentException("clientInfoString has zero length", "clientInfoString");
			}
			MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, CultureInfo.InvariantCulture, clientInfoString, LogonType.Transport, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AlwaysInitDefaultFolders);
		}

		public static MailboxSession OpenAsTransport(IExchangePrincipal mailboxOwner, OpenTransportSessionFlags flags)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			EnumValidator.ThrowIfInvalid<OpenTransportSessionFlags>(flags, "flags");
			MailboxSession.CheckNoRemoteExchangePrincipal(mailboxOwner);
			MailboxSession.InitializationFlags initializationFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties;
			switch (flags)
			{
			case OpenTransportSessionFlags.OpenForQuotaMessageDelivery:
				initializationFlags |= MailboxSession.InitializationFlags.QuotaMessageDelivery;
				break;
			case OpenTransportSessionFlags.OpenForNormalMessageDelivery:
				initializationFlags |= MailboxSession.InitializationFlags.NormalMessageDelivery;
				break;
			case OpenTransportSessionFlags.OpenForSpecialMessageDelivery:
				initializationFlags |= MailboxSession.InitializationFlags.SpecialMessageDelivery;
				break;
			}
			MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, CultureInfo.InvariantCulture, "Client=Hub Transport", LogonType.Transport, null, initializationFlags, MailboxSession.AlwaysInitDefaultFolders);
		}

		public MailboxSession CloneWithBestAccess(IExchangePrincipal mailboxOwner, IADOrgPerson accessingUser, ClientSecurityContext clientSecurityContext, string clientInfoString, GenericIdentity auxiliaryIdentity, bool unifiedSession)
		{
			ArgumentValidator.ThrowIfNull("mailboxOwner", mailboxOwner);
			ArgumentValidator.ThrowIfNull("accessingUser", accessingUser);
			UnifiedGroupMemberType memberType = UnifiedGroupMemberType.Unknown;
			if (mailboxOwner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
			{
				AccessingUserInfo accessUserInfo = new AccessingUserInfo(accessingUser.LegacyExchangeDN, accessingUser.ExternalDirectoryObjectId, mailboxOwner.MailboxInfo.OrganizationId, accessingUser.Id);
				memberType = MailboxSession.GetUserMembershipType(mailboxOwner, accessUserInfo, clientSecurityContext, clientInfoString);
			}
			return this.CloneWithBestAccess(mailboxOwner, accessingUser.LegacyExchangeDN, clientSecurityContext, clientInfoString, auxiliaryIdentity, unifiedSession, memberType);
		}

		public MailboxSession CloneWithBestAccess(IExchangePrincipal mailboxOwner, string accessingUserDn, ClientSecurityContext clientSecurityContext, string clientInfoString, GenericIdentity auxiliaryIdentity, bool unifiedSession, UnifiedGroupMemberType memberType)
		{
			if (mailboxOwner == null)
			{
				throw new ArgumentNullException("mailboxOwner");
			}
			if (!object.Equals(this.mailboxOwner.ObjectId, mailboxOwner.ObjectId))
			{
				throw new ArgumentException("mailboxOwner not same");
			}
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			MailboxAccessInfo accessInfo;
			if (string.IsNullOrEmpty(accessingUserDn))
			{
				accessInfo = new MailboxAccessInfo(clientSecurityContext, auxiliaryIdentity);
			}
			else
			{
				accessInfo = new MailboxAccessInfo(accessingUserDn, clientSecurityContext, auxiliaryIdentity);
			}
			return MailboxSession.ConfigurableOpen(mailboxOwner, accessInfo, this.SessionCultureInfo, clientInfoString, LogonType.BestAccess, null, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties, MailboxSession.AllDefaultFolders, null, unifiedSession, this.sharedDataManager, memberType);
		}

		public void ReconstructExchangePrincipal()
		{
			DirectoryHelper.DoAdCallAndTranslateExceptions(delegate
			{
				ADSessionSettings adSettings;
				if (base.PersistableTenantPartitionHint != null)
				{
					adSettings = ADSessionSettings.FromTenantPartitionHint(TenantPartitionHint.FromPersistablePartitionHint(base.PersistableTenantPartitionHint));
				}
				else
				{
					adSettings = ADSessionSettings.FromRootOrgScopeSet();
				}
				Guid mailboxGuid = this.MailboxGuid;
				this.mailboxOwner = ExchangePrincipal.FromMailboxGuid(adSettings, this.MailboxGuid, null);
			}, "ReconstructExchangePrincipal");
		}

		private bool TryGetServiceUserLegacyDn(out string serviceUserLegacyDn)
		{
			serviceUserLegacyDn = null;
			if (!this.MailboxOwner.MailboxInfo.IsRemote)
			{
				serviceUserLegacyDn = Server.GetSystemAttendantLegacyDN(LegacyDN.Parse(this.MailboxOwner.MailboxInfo.Location.ServerLegacyDn)).ToString();
				return true;
			}
			return false;
		}

		public Rules InboxRules
		{
			get
			{
				Rules result;
				using (this.CheckObjectState("InboxRules::get"))
				{
					base.CheckCapabilities(base.Capabilities.CanHaveRules, "CanHaveRules");
					if (this.inboxRules == null)
					{
						using (DisposeGuard disposeGuard = default(DisposeGuard))
						{
							Folder folder = Folder.Bind(this, DefaultFolderType.Inbox);
							disposeGuard.Add<Folder>(folder);
							this.inboxRules = new Rules(folder);
							disposeGuard.Success();
							goto IL_8C;
						}
					}
					if (this.inboxRules.ServerRules == null)
					{
						Rules rules = this.inboxRules;
						this.inboxRules = new Rules(rules.Folder);
					}
					IL_8C:
					result = this.inboxRules;
				}
				return result;
			}
		}

		public Stream GetUnsearchableItems()
		{
			if (!this.IsRemote)
			{
				return new UnsearchableItemsStream(this);
			}
			return base.Mailbox.OpenPropertyStream(InternalSchema.UnsearchableItemsStream, PropertyOpenMode.ReadOnly);
		}

		public Rules AllInboxRules
		{
			get
			{
				Rules result;
				using (this.CheckObjectState("AllInboxRules::get"))
				{
					base.CheckCapabilities(base.Capabilities.CanHaveRules, "CanHaveRules");
					if (this.allInboxRules == null)
					{
						using (DisposeGuard disposeGuard = default(DisposeGuard))
						{
							Folder folder = Folder.Bind(this, DefaultFolderType.Inbox);
							disposeGuard.Add<Folder>(folder);
							this.allInboxRules = new Rules(folder, true);
							disposeGuard.Success();
						}
					}
					result = this.allInboxRules;
				}
				return result;
			}
		}

		public JunkEmailRule JunkEmailRule
		{
			get
			{
				return this.GetJunkEmailRule(false);
			}
		}

		public JunkEmailRule FilteredJunkEmailRule
		{
			get
			{
				return this.GetJunkEmailRule(true);
			}
		}

		private JunkEmailRule GetJunkEmailRule(bool filterJunkEmailRule)
		{
			JunkEmailRule result;
			using (this.CheckObjectState("JunkEmailRule::get"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveJunkEmailRule, "CanHaveJunkEmailRule");
				result = JunkEmailRule.Create(this, filterJunkEmailRule);
			}
			return result;
		}

		public JunkEmailRule.JunkEmailStatus GetJunkEmailRuleStatus()
		{
			JunkEmailRule.JunkEmailStatus result;
			using (this.CheckObjectState("GetJunkEmailRuleStatus"))
			{
				base.CheckCapabilities(base.Capabilities.CanHaveJunkEmailRule, "CanHaveJunkEmailRule");
				if (base.Mailbox == null || base.Mailbox.MapiStore == null)
				{
					string str = null;
					if (base.Mailbox == null)
					{
						str = ",Mailbox = null";
					}
					else if (base.Mailbox.MapiStore == null)
					{
						str = ",MapiStore = null";
					}
					throw new ConnectionFailedPermanentException(new LocalizedString(ServerStrings.ExStoreSessionDisconnected + str));
				}
				JunkEmailRule.JunkEmailStatus junkEmailStatus = JunkEmailRule.JunkEmailStatus.None;
				MapiFolder mapiFolder = null;
				bool flag = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiFolder = base.Mailbox.MapiStore.GetInboxFolder();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				using (mapiFolder)
				{
					MapiTable mapiTable = null;
					bool flag2 = false;
					try
					{
						if (this != null)
						{
							this.BeginMapiCall();
							this.BeginServerHealthCall();
							flag2 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						mapiTable = mapiFolder.GetAssociatedContentsTable();
					}
					catch (MapiPermanentException ex3)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex3, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
							ex3
						});
					}
					catch (MapiRetryableException ex4)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex4, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
							ex4
						});
					}
					finally
					{
						try
						{
							if (this != null)
							{
								this.EndMapiCall();
								if (flag2)
								{
									this.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
					using (mapiTable)
					{
						int num = 26080;
						PropTag propTag = PropTagHelper.PropTagFromIdAndType(num + 12, PropType.String);
						PropTag propTag2 = PropTagHelper.PropTagFromIdAndType(num + 9, PropType.Int);
						PropTag[] columns = new PropTag[]
						{
							propTag,
							propTag2
						};
						bool flag3 = false;
						try
						{
							if (this != null)
							{
								this.BeginMapiCall();
								this.BeginServerHealthCall();
								flag3 = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							mapiTable.Restrict(new Restriction.ContentRestriction(PropTag.MessageClass, "IPM.ExtendedRule.Message", ContentFlags.IgnoreCase));
							mapiTable.SetColumns(columns);
							mapiTable.SeekRow(BookMark.Beginning, 0);
						}
						catch (MapiPermanentException ex5)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex5, this, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
								ex5
							});
						}
						catch (MapiRetryableException ex6)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex6, this, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
								ex6
							});
						}
						finally
						{
							try
							{
								if (this != null)
								{
									this.EndMapiCall();
									if (flag3)
									{
										this.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
						for (;;)
						{
							PropValue[][] array = null;
							bool flag4 = false;
							try
							{
								if (this != null)
								{
									this.BeginMapiCall();
									this.BeginServerHealthCall();
									flag4 = true;
								}
								if (StorageGlobals.MapiTestHookBeforeCall != null)
								{
									StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
								}
								array = mapiTable.QueryRows(10);
							}
							catch (MapiPermanentException ex7)
							{
								throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex7, this, this, "{0}. MapiException = {1}.", new object[]
								{
									string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
									ex7
								});
							}
							catch (MapiRetryableException ex8)
							{
								throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetContentsTable, ex8, this, this, "{0}. MapiException = {1}.", new object[]
								{
									string.Format("MailboxSession::GetJunkEmailRuleStatus.", new object[0]),
									ex8
								});
							}
							finally
							{
								try
								{
									if (this != null)
									{
										this.EndMapiCall();
										if (flag4)
										{
											this.EndServerHealthCall();
										}
									}
								}
								finally
								{
									if (StorageGlobals.MapiTestHookAfterCall != null)
									{
										StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
									}
								}
							}
							if (array.Length == 0)
							{
								goto IL_50A;
							}
							foreach (PropValue[] array3 in array)
							{
								if (array3[0].GetString() == "Junk E-mail Rule")
								{
									goto Block_39;
								}
							}
						}
						Block_39:
						junkEmailStatus |= JunkEmailRule.JunkEmailStatus.IsPresent;
						PropValue[] array3;
						RuleStateFlags @int = (RuleStateFlags)array3[1].GetInt(0);
						if ((@int & RuleStateFlags.Enabled) == RuleStateFlags.Enabled)
						{
							junkEmailStatus |= JunkEmailRule.JunkEmailStatus.IsEnabled;
						}
						if ((@int & RuleStateFlags.Error) == RuleStateFlags.Error)
						{
							junkEmailStatus |= JunkEmailRule.JunkEmailStatus.IsError;
						}
						if ((@int & RuleStateFlags.RuleParseError) == RuleStateFlags.RuleParseError)
						{
							junkEmailStatus |= JunkEmailRule.JunkEmailStatus.IsError;
						}
						return junkEmailStatus;
						IL_50A:;
					}
				}
				result = junkEmailStatus;
			}
			return result;
		}

		public override bool CheckSubmissionQuota(int recipientCount)
		{
			bool result;
			using (base.CheckDisposed("CheckSubmissionQuota"))
			{
				if (this.IsRemote)
				{
					throw new NotSupportedException("CheckSubmissionQuota not supported for remote sessions.");
				}
				if (recipientCount == 0)
				{
					result = true;
				}
				else if (this.mailboxOwner.MailboxInfo.Configuration.ThrottlingPolicy.IsNullOrEmpty() && base.LogonType == LogonType.Transport)
				{
					result = true;
				}
				else if (this.MailboxOwner.RecipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox)
				{
					result = true;
				}
				else
				{
					result = MailboxSession.checkSubmissionQuotaDelegate(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.mailboxOwner.MailboxInfo.Location.ServerVersion, this.mailboxOwner.MailboxInfo.MailboxGuid, recipientCount, this.mailboxOwner.MailboxInfo.Configuration.ThrottlingPolicy, this.mailboxOwner.MailboxInfo.OrganizationId, base.ClientInfoString);
				}
			}
			return result;
		}

		public RoutingTypeOptionsData GetOptionsData(string routingType)
		{
			RoutingTypeOptionsData optionsData;
			using (this.CheckObjectState("GetOptionsData"))
			{
				optionsData = OptionsDataBuilder.Instance.GetOptionsData(this, routingType);
			}
			return optionsData;
		}

		public PolicyTagList GetPolicyTagList(RetentionActionType type)
		{
			PolicyTagList result;
			using (base.CheckDisposed("GetPolicyTagList"))
			{
				if (type != (RetentionActionType)0)
				{
					EnumValidator.ThrowIfInvalid<RetentionActionType>(type, "RetentionActionType");
				}
				PolicyTagList policyTagList = null;
				if (this.policyTagListDictionary == null)
				{
					this.policyTagListDictionary = new Dictionary<RetentionActionType, PolicyTagList>();
				}
				else
				{
					this.policyTagListDictionary.TryGetValue(type, out policyTagList);
				}
				if (!this.policyTagListDictionary.ContainsKey(type))
				{
					policyTagList = PolicyTagList.GetPolicyTagListFromMailboxSession(type, this);
					this.policyTagListDictionary[type] = policyTagList;
				}
				result = policyTagList;
			}
			return result;
		}

		public override ADSessionSettings GetADSessionSettings()
		{
			ADSessionSettings result;
			using (base.CheckDisposed("GetADSessionSettings"))
			{
				ADSessionSettings adsessionSettings;
				if (this.InternalMailboxOwner != null)
				{
					adsessionSettings = this.MailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings();
				}
				else
				{
					adsessionSettings = OrganizationId.ForestWideOrgId.ToADSessionSettings();
				}
				adsessionSettings.AccountingObject = base.AccountingObject;
				result = adsessionSettings;
			}
			return result;
		}

		public void AuditMailboxAccess(IAuditEvent auditEvent, bool isAsynchronous)
		{
			MailboxSession.<>c__DisplayClass3a CS$<>8__locals1 = new MailboxSession.<>c__DisplayClass3a();
			CS$<>8__locals1.auditEvent = auditEvent;
			CS$<>8__locals1.isAsynchronous = isAsynchronous;
			CS$<>8__locals1.<>4__this = this;
			Util.ThrowOnNullArgument(CS$<>8__locals1.auditEvent, "auditEvent");
			using (MailboxAuditOpticsLogData logData = new MailboxAuditOpticsLogData())
			{
				logData.RecordId = CS$<>8__locals1.auditEvent.RecordId;
				logData.Tenant = CS$<>8__locals1.auditEvent.OrganizationId;
				logData.Mailbox = CS$<>8__locals1.auditEvent.MailboxGuid.ToString();
				logData.Operation = CS$<>8__locals1.auditEvent.OperationName;
				logData.LogonType = CS$<>8__locals1.auditEvent.LogonTypeName;
				logData.OperationSucceeded = CS$<>8__locals1.auditEvent.OperationSucceeded;
				logData.ExternalAccess = CS$<>8__locals1.auditEvent.ExternalAccess;
				logData.Asynchronous = CS$<>8__locals1.isAsynchronous;
				using (base.CreateSessionGuard("AuditMailboxAccess"))
				{
					if (this.defaultFolderManager != null)
					{
						this.BypassAuditsFolderAccessChecking(delegate
						{
							CS$<>8__locals1.<>4__this.DoNothingIfBypassAuditing(delegate
							{
								CS$<>8__locals1.<>4__this.BypassAuditing(delegate
								{
									Stopwatch stopwatch = Stopwatch.StartNew();
									int num = CS$<>8__locals1.isAsynchronous ? 1 : 3;
									bool flag = false;
									IAuditLogRecord logRecord = CS$<>8__locals1.auditEvent.GetLogRecord();
									Exception ex = null;
									int i = 0;
									IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
									if (currentActivityScope != null)
									{
										logData.ActionContext = string.Format("{0}.{1}.{2}.{3}", new object[]
										{
											currentActivityScope.Component,
											currentActivityScope.Feature,
											currentActivityScope.Action,
											currentActivityScope.ClientInfo
										});
										logData.ClientRequestId = currentActivityScope.ClientRequestId;
									}
									logData.ActionContext = string.Format("{0}.{1}", logData.ActionContext, (CS$<>8__locals1.<>4__this.RemoteClientSessionInfo != null) ? CS$<>8__locals1.<>4__this.RemoteClientSessionInfo.ClientInfoString : CS$<>8__locals1.<>4__this.ClientInfoString);
									try
									{
										CS$<>8__locals1.<>4__this.copyOnWriteNotification.CheckAndCreateAuditsFolder(CS$<>8__locals1.<>4__this);
										while (i < num)
										{
											ex = null;
											try
											{
												logData.RecordSize = CS$<>8__locals1.<>4__this.GetAuditLog(logRecord.CreationTime).WriteAuditRecord(logRecord);
												stopwatch.Stop();
												COWSession.PerfCounters.TotalAuditSave.Increment();
												COWSession.PerfCounters.TotalAuditSaveTime.IncrementBy(stopwatch.ElapsedMilliseconds);
												logData.LoggingTime = stopwatch.ElapsedMilliseconds;
												flag = true;
												if (ExTraceGlobals.SessionTracer.IsTraceEnabled(TraceType.DebugTrace))
												{
													ExTraceGlobals.SessionTracer.TraceDebug<string, string>((long)CS$<>8__locals1.<>4__this.GetHashCode(), "[MailboxSession::AuditMailboxAccess] audit log has been committed successfully to mailbox {0}: {1}", CS$<>8__locals1.<>4__this.mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), AuditLogParseSerialize.GetAsString(logRecord));
												}
												logData.AuditSucceeded = true;
												break;
											}
											catch (IOException ex2)
											{
												ex = ex2;
												if (CS$<>8__locals1.isAsynchronous)
												{
													throw;
												}
											}
											catch (TransientException ex3)
											{
												ex = ex3;
												if (CS$<>8__locals1.isAsynchronous)
												{
													throw;
												}
											}
											catch (LocalizedException ex4)
											{
												ex = ex4;
												if (CS$<>8__locals1.isAsynchronous)
												{
													throw;
												}
												break;
											}
											catch (Exception ex5)
											{
												ex = ex5;
												throw;
											}
											finally
											{
												if (ex != null)
												{
													ExTraceGlobals.SessionTracer.TraceError<string, Guid, Exception>((long)CS$<>8__locals1.<>4__this.GetHashCode(), "Error occurred while saving audit information for mailbox '{0}' {1}. Exception details:/r/n{2}", CS$<>8__locals1.<>4__this.mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), CS$<>8__locals1.<>4__this.MailboxGuid, ex);
												}
											}
											i++;
										}
									}
									catch (TransientException ex6)
									{
										ExTraceGlobals.SessionTracer.TraceError<string, Guid, TransientException>((long)CS$<>8__locals1.<>4__this.GetHashCode(), "Error occurred while CheckAndCreateAuditsFolder for mailbox '{0}' {1}. Exception details:/r/n{2}", CS$<>8__locals1.<>4__this.mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), CS$<>8__locals1.<>4__this.MailboxGuid, ex6);
										ex = ex6;
										if (CS$<>8__locals1.isAsynchronous)
										{
											throw;
										}
									}
									finally
									{
										if (!flag)
										{
											string text = CS$<>8__locals1.<>4__this.mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString() ?? string.Empty;
											string text2 = string.Empty;
											string text3 = AuditLogParseSerialize.GetAsString(logRecord);
											if (ex != null)
											{
												text2 = ex.ToString();
											}
											ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3292933437U, ref text3);
											if (text.Length > 30000)
											{
												text = text.Substring(0, 30000);
												text2 = string.Empty;
												text3 = string.Empty;
											}
											else if (text.Length + text2.Length > 30000)
											{
												text2 = text2.Substring(0, 30000 - text.Length);
												text3 = string.Empty;
											}
											else if (text.Length + text2.Length + text3.Length > 30000)
											{
												text3 = text3.Substring(0, 30000 - text.Length - text2.Length);
											}
											ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorSavingMailboxAudit, CS$<>8__locals1.<>4__this.mailboxOwner.ObjectId.ToString(), new object[]
											{
												text,
												CS$<>8__locals1.<>4__this.MailboxGuid,
												i,
												text2,
												text3
											});
											if (stopwatch.IsRunning)
											{
												stopwatch.Stop();
											}
											logData.AuditSucceeded = false;
											logData.LoggingError = ex;
											logData.LoggingTime = stopwatch.ElapsedMilliseconds;
										}
									}
								});
							});
						});
					}
				}
			}
		}

		private IAuditLog GetAuditLog(DateTime timestamp)
		{
			if (AuditFeatureManager.IsPartitionedMailboxLogEnabled(this.mailboxOwner))
			{
				if (this.auditLog == null || this.auditLog.EstimatedLogEndTime < timestamp)
				{
					AuditLogCollection auditLogCollection = new AuditLogCollection(this, this.GetAuditsFolderId(), ExTraceGlobals.SessionTracer, (IAuditLogRecord record, MessageItem message) => AuditLogParseSerialize.SerializeMailboxAuditRecord(record, message));
					if (!auditLogCollection.FindLog(timestamp, true, out this.auditLog))
					{
						this.auditLog = new AuditLog(this, this.GetAuditsFolderId(), DateTime.MinValue, DateTime.MaxValue, 0, (IAuditLogRecord record, MessageItem message) => AuditLogParseSerialize.SerializeMailboxAuditRecord(record, message));
					}
				}
			}
			else if (this.auditLog == null)
			{
				this.auditLog = new AuditLog(this, this.GetAuditsFolderId(), DateTime.MinValue, DateTime.MaxValue, 0, (IAuditLogRecord record, MessageItem message) => AuditLogParseSerialize.SerializeMailboxAuditRecord(record, message));
			}
			return this.auditLog;
		}

		private static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (SslConfiguration.AllowExternalUntrustedCerts)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<string, SslPolicyErrors>((long)sender.GetHashCode(), "MailboxSession::CertificateErrorHandler. Allowed SSL certificate {0} with error {1}", certificate.Subject, sslPolicyErrors);
				return true;
			}
			return false;
		}

		private bool IgnoreActiveManagerSiteBoundary()
		{
			return base.LogonType == LogonType.Transport;
		}

		internal static MailboxSession.CheckSubmissionQuotaDelegate CheckSubmissionQuotaImpl
		{
			get
			{
				return MailboxSession.checkSubmissionQuotaDelegate;
			}
			set
			{
				MailboxSession.checkSubmissionQuotaDelegate = value;
			}
		}

		private static void RegisterCallback()
		{
			if (MailboxSession.registeredCertificateErrorHandler)
			{
				return;
			}
			lock (MailboxSession.lockRegisterCertificateErrorHandler)
			{
				if (!MailboxSession.registeredCertificateErrorHandler)
				{
					CertificateValidationManager.RegisterCallback("XRopXsoClient", new RemoteCertificateValidationCallback(MailboxSession.CertificateErrorHandler));
					MailboxSession.registeredCertificateErrorHandler = true;
				}
			}
		}

		private void Initialize(MapiStore linkedStore, LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegateUser, object identity, OpenMailboxSessionFlags flags, GenericIdentity auxiliaryIdentity)
		{
			this.Initialize(linkedStore, logonType, owner, delegateUser, identity, flags, auxiliaryIdentity, false);
		}

		private void Initialize(MapiStore linkedStore, LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegateUser, object identity, OpenMailboxSessionFlags flags, GenericIdentity auxiliaryIdentity, bool unifiedSession)
		{
			ExTraceGlobals.SessionTracer.Information((long)this.GetHashCode(), "MailboxSession::Initialize.");
			if (owner == null)
			{
				ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "MailboxSession::Initialize. The in parameter {0} should not be null.", "owner");
				throw new ArgumentNullException("owner");
			}
			if (identity == null)
			{
				ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "MailboxSession::Initialize. The in parameter {0} should not be null.", "identity");
				throw new ArgumentNullException("identity");
			}
			EnumValidator<OpenMailboxSessionFlags>.ThrowIfInvalid(flags, "flags");
			this.delegateUser = delegateUser.ADOrgPerson;
			this.mailboxOwner = owner;
			base.LogonType = logonType;
			Guid mailboxGuid = this.MailboxOwner.MailboxInfo.MailboxGuid;
			this.IsUnified = unifiedSession;
			this.identity = identity;
			this.auxiliaryIdentity = auxiliaryIdentity;
			this.useNamedProperties = flags.HasFlag(OpenMailboxSessionFlags.UseNamedProperties);
			this.nonInteractiveSession = flags.HasFlag(OpenMailboxSessionFlags.NonInteractiveSession);
			base.IsMoveUser = flags.HasFlag(OpenMailboxSessionFlags.MoveUser);
			base.IsXForestMove = flags.HasFlag(OpenMailboxSessionFlags.XForestMove);
			base.IsOlcMoveDestination = flags.HasFlag(OpenMailboxSessionFlags.OlcSync);
			base.StoreFlag |= OpenStoreFlag.NoMail;
			if (flags.HasFlag(OpenMailboxSessionFlags.OverrideHomeMdb))
			{
				base.StoreFlag |= OpenStoreFlag.OverrideHomeMdb;
			}
			if (flags.HasFlag(OpenMailboxSessionFlags.DisconnectedMailbox))
			{
				base.StoreFlag |= OpenStoreFlag.DisconnectedMailbox;
			}
			if (flags.HasFlag(OpenMailboxSessionFlags.XForestMove))
			{
				base.StoreFlag |= OpenStoreFlag.XForestMove;
			}
			if (flags.HasFlag(OpenMailboxSessionFlags.ReadOnly))
			{
				base.Capabilities = base.Capabilities.CloneAndExtendCapabilities(SessionCapabilitiesFlags.ReadOnly);
			}
			if (!this.mailboxOwner.MailboxInfo.MailboxDatabase.IsNullOrEmpty() && this.mailboxOwner.MailboxInfo.MailboxGuid != Guid.Empty && !this.mailboxOwner.MailboxInfo.IsRemote)
			{
				base.StoreFlag |= OpenStoreFlag.MailboxGuid;
			}
			if (flags.HasFlag(OpenMailboxSessionFlags.UseRecoveryDatabase))
			{
				if (!base.StoreFlag.HasFlag(OpenStoreFlag.MailboxGuid))
				{
					throw new ArgumentException("must be logging in with GUIDs, not legDN", "owner");
				}
				base.StoreFlag |= (OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership | OpenStoreFlag.OverrideHomeMdb | OpenStoreFlag.FailIfNoMailbox | OpenStoreFlag.NoLocalization | OpenStoreFlag.RestoreDatabase);
			}
			if (flags.HasFlag(OpenMailboxSessionFlags.RequestLocalRpcConnection))
			{
				MailboxSession.CheckNoRemoteExchangePrincipal(this.MailboxOwner);
				this.connectFlag |= ConnectFlag.LocalRpcOnly;
			}
			if (this.MailboxOwner.MailboxInfo.Location.ServerVersion < Server.E15MinVersion)
			{
				this.connectFlag |= ConnectFlag.IsPreExchange15;
			}
			if (this.MailboxOwner.RecipientTypeDetails == RecipientTypeDetails.MonitoringMailbox)
			{
				this.connectFlag |= ConnectFlag.MonitoringMailbox;
			}
			if (flags.HasFlag(OpenMailboxSessionFlags.RequestExchangeRpcServer) || StoreSession.TestRequestExchangeRpcServer)
			{
				this.connectFlag |= ConnectFlag.ConnectToExchangeRpcServerOnly;
			}
			else if (!this.MailboxOwner.MailboxInfo.IsRemote && this.MailboxOwner.MailboxInfo.Location.IsLegacyServer())
			{
				this.connectFlag |= ConnectFlag.AllowLegacyStore;
			}
			if (base.LogonType == LogonType.BestAccess)
			{
				if (string.IsNullOrEmpty(delegateUser.LegacyDn))
				{
					this.userLegacyDn = this.MailboxOwner.LegacyDn;
				}
				else
				{
					this.userLegacyDn = delegateUser.LegacyDn;
				}
				this.connectFlag |= ConnectFlag.UseDelegatedAuthPrivilege;
				base.StoreFlag |= OpenStoreFlag.TakeOwnership;
			}
			else if (base.LogonType == LogonType.Owner)
			{
				this.userLegacyDn = this.MailboxOwner.LegacyDn;
				this.connectFlag |= ConnectFlag.UseDelegatedAuthPrivilege;
				base.StoreFlag |= OpenStoreFlag.TakeOwnership;
			}
			else if (base.LogonType == LogonType.Delegated)
			{
				this.userLegacyDn = delegateUser.LegacyDn;
				this.connectFlag |= ConnectFlag.UseDelegatedAuthPrivilege;
				base.StoreFlag |= OpenStoreFlag.TakeOwnership;
			}
			else if (base.LogonType == LogonType.Admin || base.LogonType == LogonType.SystemService)
			{
				this.connectFlag |= ConnectFlag.UseAdminPrivilege;
				base.StoreFlag |= (OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership);
				if ((flags & OpenMailboxSessionFlags.AllowAdminLocalization) != OpenMailboxSessionFlags.AllowAdminLocalization)
				{
					base.StoreFlag |= OpenStoreFlag.NoLocalization;
				}
				this.TryGetServiceUserLegacyDn(out this.userLegacyDn);
			}
			else if (base.LogonType == LogonType.DelegatedAdmin)
			{
				this.connectFlag |= ConnectFlag.UseDelegatedAuthPrivilege;
				base.StoreFlag |= (OpenStoreFlag.UseAdminPrivilege | OpenStoreFlag.TakeOwnership);
				if ((flags & OpenMailboxSessionFlags.AllowAdminLocalization) != OpenMailboxSessionFlags.AllowAdminLocalization)
				{
					base.StoreFlag |= OpenStoreFlag.NoLocalization;
				}
				this.userLegacyDn = delegateUser.LegacyDn;
			}
			else if (base.LogonType == LogonType.Transport)
			{
				this.connectFlag |= ConnectFlag.UseTransportPrivilege;
				base.StoreFlag |= OpenStoreFlag.NoLocalization;
				if ((flags & OpenMailboxSessionFlags.OpenForQuotaMessageDelivery) == OpenMailboxSessionFlags.OpenForQuotaMessageDelivery)
				{
					base.StoreFlag |= OpenStoreFlag.DeliverQuotaMessage;
				}
				if ((flags & OpenMailboxSessionFlags.OpenForNormalMessageDelivery) == OpenMailboxSessionFlags.OpenForNormalMessageDelivery)
				{
					base.StoreFlag |= OpenStoreFlag.DeliverNormalMessage;
				}
				if ((flags & OpenMailboxSessionFlags.OpenForSpecialMessageDelivery) == OpenMailboxSessionFlags.OpenForSpecialMessageDelivery)
				{
					base.StoreFlag |= OpenStoreFlag.DeliverSpecialMessage;
				}
				this.TryGetServiceUserLegacyDn(out this.userLegacyDn);
			}
			ExTraceGlobals.SessionTracer.Information<string, string>((long)this.GetHashCode(), "MailboxSession::Initialize. server = {0}, user = {1}.", this.mailboxOwner.MailboxInfo.IsRemote ? string.Empty : this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn);
			MapiStore mapiStore = null;
			bool flag = false;
			try
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "MailboxSession::Initialize. server = {0}, user = {1}, connectFlag = {2}, storeFlag = {3}, culture = {4}, clientInfoString = {5}, flags = {6}.", new object[]
				{
					this.mailboxOwner.MailboxInfo.IsRemote ? string.Empty : this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
					this.userLegacyDn,
					this.connectFlag,
					base.StoreFlag,
					this.InternalPreferedCulture,
					base.ClientInfoString,
					(int)flags
				});
				this.connectFlag |= ConnectFlag.LowMemoryFootprint;
				mapiStore = this.ForceOpen(linkedStore, unifiedSession);
				base.IsConnected = true;
				if (base.LogonType == LogonType.BestAccess)
				{
					if (this.CanActAsOwner)
					{
						base.LogonType = LogonType.Owner;
					}
					else
					{
						if (owner.MailboxInfo.IsArchive)
						{
							throw new AccessDeniedException(ServerStrings.ExMailboxAccessDenied(this.mailboxOwner.MailboxInfo.DisplayName, "Archive mailbox:" + this.mailboxOwner.MailboxInfo.MailboxGuid.ToString()));
						}
						if (owner.MailboxInfo.IsAggregated)
						{
							throw new AccessDeniedException(ServerStrings.ExMailboxAccessDenied(this.mailboxOwner.MailboxInfo.DisplayName, "Aggregated mailbox:" + this.mailboxOwner.MailboxInfo.MailboxGuid));
						}
						base.LogonType = LogonType.Delegated;
						base.StoreFlag &= ~OpenStoreFlag.TakeOwnership;
					}
				}
				if (base.LogonType != LogonType.Admin || base.ClientInfoString == null || !MailboxSession.AllowedClientsForPublicFolderMailbox.IsMatch(base.ClientInfoString))
				{
					object obj = base.Mailbox.TryGetProperty(MailboxSchema.MailboxType);
					if (obj is int && StoreSession.IsPublicFolderMailbox((int)obj))
					{
						throw new AccessDeniedException(ServerStrings.OperationNotSupportedOnPublicFolderMailbox);
					}
				}
				if ((flags & OpenMailboxSessionFlags.InitDefaultFolders) == OpenMailboxSessionFlags.InitDefaultFolders && Util.Contains(this.foldersToInit, DefaultFolderType.Reminders) && (flags & OpenMailboxSessionFlags.InitUserConfigurationManager) != OpenMailboxSessionFlags.InitUserConfigurationManager)
				{
					throw new InvalidOperationException("Must have UserConfigurationManager to init Reminders folder");
				}
				if ((flags & OpenMailboxSessionFlags.InitUserConfigurationManager) == OpenMailboxSessionFlags.InitUserConfigurationManager)
				{
					this.userConfigurationManager = new UserConfigurationManager(this);
				}
				if ((flags & OpenMailboxSessionFlags.InitDefaultFolders) == OpenMailboxSessionFlags.InitDefaultFolders)
				{
					this.InternalInitializeDefaultFolders(this.foldersToInit, flags);
					if ((base.StoreFlag & OpenStoreFlag.NoLocalization) != OpenStoreFlag.NoLocalization && !Util.IsSpecialLcid(this.InternalPreferedCulture.LCID) && base.LogonType != LogonType.Delegated && !this.IsMailboxLocalized)
					{
						this.InternalLocalizeInitialDefaultFolders(flags);
					}
				}
				if ((flags & OpenMailboxSessionFlags.InitCopyOnWrite) == OpenMailboxSessionFlags.InitCopyOnWrite)
				{
					if ((flags & OpenMailboxSessionFlags.UseRecoveryDatabase) != OpenMailboxSessionFlags.None)
					{
						throw new ArgumentException("No CopyOnWrite allowed for Recovery DB logons", "flags");
					}
					if (!owner.MailboxInfo.IsRemote)
					{
						this.copyOnWriteNotification = COWSession.Create(this);
					}
				}
				if ((flags & OpenMailboxSessionFlags.InitCheckPrivateItemsAccess) == OpenMailboxSessionFlags.InitCheckPrivateItemsAccess && base.LogonType == LogonType.Delegated)
				{
					this.CheckPrivateItemsAccessPermission(delegateUser.LegacyDn);
				}
				this.activitySession = Microsoft.Exchange.Data.Storage.ActivitySession.Create(this);
				if (Utils.IsTeamMailbox(this))
				{
					try
					{
						TeamMailboxNotificationHelper.SendWelcomeMessageIfNeeded(this);
					}
					catch (StorageTransientException ex)
					{
						ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), string.Format("MailboxSession::Initialize. Failed to send welcome message for site mailbox because of {0} with detail: {1}", ex.GetType(), ex.Message));
					}
					catch (StoragePermanentException ex2)
					{
						ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), string.Format("MailboxSession::Initialize. Failed to send welcome message for site mailbox because of {0} with detail: {1}", ex2.GetType(), ex2.Message));
					}
					if (!string.IsNullOrEmpty(base.ClientInfoString) && (base.ClientInfoString.StartsWith("Client=OWA", StringComparison.OrdinalIgnoreCase) || base.ClientInfoString.StartsWith("Client=MSExchangeRPC", StringComparison.OrdinalIgnoreCase)))
					{
						string client;
						if (base.ClientInfoString.StartsWith("Client=OWA", StringComparison.OrdinalIgnoreCase))
						{
							client = "Client=OWA";
						}
						else
						{
							client = "Client=MSExchangeRPC";
						}
						this.siteMailboxSynchronizerReference = SiteMailboxSynchronizerManager.Instance.GetSiteMailboxSynchronizer(this.mailboxOwner, client);
					}
				}
				try
				{
					ExtendedRuleConditionConstraint.InitExtendedRuleSizeLimitIfNeeded(this);
				}
				catch (StoragePermanentException ex3)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), string.Format("MailboxSession::Initialize. Failed to initialize extended rule size limit because of {0} with detail: {1}", ex3.GetType(), ex3.Message));
				}
				catch (StorageTransientException ex4)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), string.Format("MailboxSession::Initialize. Failed to initialize extended rule size limit because of {0} with detail: {1}", ex4.GetType(), ex4.Message));
				}
				OrganizationContentConversionProperties organizationContentConversionProperties;
				if (StoreSession.directoryAccessor.TryGetOrganizationContentConversionProperties(this.OrganizationId, out organizationContentConversionProperties))
				{
					base.PreferredInternetCodePageForShiftJis = organizationContentConversionProperties.PreferredInternetCodePageForShiftJis;
					base.RequiredCoverage = organizationContentConversionProperties.RequiredCharsetCoverage;
				}
				if ((flags & OpenMailboxSessionFlags.InitDeadSessionChecking) == OpenMailboxSessionFlags.InitDeadSessionChecking)
				{
					base.StartDeadSessionChecking();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (this.copyOnWriteNotification != null)
					{
						this.copyOnWriteNotification.Dispose();
					}
					this.copyOnWriteNotification = null;
					base.IsConnected = false;
					base.SetMailboxStoreObject(null);
					if (mapiStore != null)
					{
						mapiStore.Dispose();
						mapiStore = null;
					}
				}
			}
		}

		internal static MailboxSession CreateDummyInstance()
		{
			IGenericADUser adUser = new GenericADUser
			{
				MailboxDatabase = new ADObjectId(Guid.NewGuid()),
				LegacyDn = " ",
				OrganizationId = null,
				DisplayName = " ",
				PrimarySmtpAddress = new SmtpAddress("foo@contoso.com"),
				MailboxGuid = Guid.NewGuid(),
				GrantSendOnBehalfTo = new ADMultiValuedProperty<ADObjectId>(),
				Languages = Array<CultureInfo>.Empty,
				RecipientType = RecipientType.UserMailbox,
				RecipientTypeDetails = RecipientTypeDetails.None,
				ObjectId = new ADObjectId(Guid.NewGuid()),
				MasterAccountSid = null,
				AggregatedMailboxGuids = Array<Guid>.Empty
			};
			return new MailboxSession
			{
				mailboxOwner = new UserPrincipalBuilder(adUser).Build(),
				identity = WindowsIdentity.GetCurrent()
			};
		}

		private MailboxSession()
		{
		}

		private static void InternalValidateServerVersion(IExchangePrincipal owner)
		{
			if (owner.MailboxInfo.Location.ServerVersion != 0)
			{
				ServerVersion serverVersion = new ServerVersion(owner.MailboxInfo.Location.ServerVersion);
				if (serverVersion.Major < StoreSession.CurrentServerMajorVersion)
				{
					throw new NotSupportedWithServerVersionException(owner.MailboxInfo.PrimarySmtpAddress.ToString(), serverVersion.Major, StoreSession.CurrentServerMajorVersion);
				}
			}
		}

		private static void InternalValidateDatacenterAccess(LogonType logonType, IExchangePrincipal owner, DelegateLogonUser delegatedUser)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.CheckLicense.Enabled)
			{
				if ((logonType == LogonType.Owner || logonType == LogonType.BestAccess || logonType == LogonType.Admin || logonType == LogonType.Delegated || logonType == LogonType.DelegatedAdmin) && StoreSession.directoryAccessor.IsTenantAccessBlocked(owner.MailboxInfo.OrganizationId))
				{
					throw new TenantAccessBlockedException(ServerStrings.ExTenantAccessBlocked(owner.MailboxInfo.OrganizationId.ToString()));
				}
				if (MailboxSession.IsOwnerLogon(logonType, owner, delegatedUser) && !MailboxSession.AllowMailboxLogon(owner))
				{
					throw new InvalidLicenseException(ServerStrings.ExInvalidLicense(owner.MailboxInfo.DisplayName));
				}
			}
		}

		private static bool AllowMailboxLogon(IExchangePrincipal exchangePrincipal)
		{
			bool flag = StoreSession.directoryAccessor.IsLicensingEnforcedInOrg(exchangePrincipal.MailboxInfo.OrganizationId);
			return exchangePrincipal.RecipientTypeDetails != RecipientTypeDetails.UserMailbox || !flag || (!StoreSession.directoryAccessor.IsTenantAccessBlocked(exchangePrincipal.MailboxInfo.OrganizationId) && CapabilityHelper.AllowMailboxLogon(exchangePrincipal.MailboxInfo.Configuration.SkuCapability, exchangePrincipal.MailboxInfo.Configuration.SkuAssigned, exchangePrincipal.MailboxInfo.WhenMailboxCreated));
		}

		private static UnifiedGroupMemberType GetUserMembershipType(IExchangePrincipal groupMailbox, AccessingUserInfo accessUserInfo, ClientSecurityContext clientSecurityContext, string clientInfoString)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("clientInfoString", clientInfoString);
			GroupMailboxAuthorizationHandler groupMailboxAuthorizationHandler = new GroupMailboxAuthorizationHandler(groupMailbox, accessUserInfo, clientInfoString, clientSecurityContext, groupMailbox.GetConfiguration());
			UnifiedGroupMemberType membershipType = groupMailboxAuthorizationHandler.GetMembershipType();
			if (membershipType == UnifiedGroupMemberType.None && groupMailbox.ModernGroupType == ModernGroupObjectType.Private)
			{
				throw new AccessDeniedException(ServerStrings.NotAuthorizedtoAccessGroupMailbox(accessUserInfo.LegacyExchangeDN, groupMailbox.MailboxInfo.PrimarySmtpAddress.ToString()));
			}
			return membershipType;
		}

		private MapiStore InternalGetRemoteConnection()
		{
			bool flag = false;
			MapiStore mapiStore = null;
			Client client = null;
			try
			{
				ConnectFlag connectFlag = (this.connectFlag & ~ConnectFlag.UseDelegatedAuthPrivilege) | ConnectFlag.ConnectToExchangeRpcServerOnly;
				if (base.LogonType == LogonType.SystemService)
				{
					connectFlag |= ConnectFlag.RemoteSystemService;
				}
				Uri remoteEndpoint = base.RemoteMailboxProperties.GetRemoteEndpoint(this.mailboxOwner.MailboxInfo.RemoteIdentity);
				if (this.mailboxOwner.MailboxInfo.RemoteIdentity == null || remoteEndpoint == null)
				{
					throw new MailboxOfflineException(ServerStrings.CannotAccessRemoteMailbox);
				}
				string text = "SMTP:" + this.mailboxOwner.MailboxInfo.RemoteIdentity.ToString();
				ExternalAuthentication current = ExternalAuthentication.GetCurrent();
				if (!current.Enabled)
				{
					string text2 = this.mailboxOwner.MailboxInfo.RemoteIdentity.Value.ToString();
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ExternalAuthDisabledMailboxSession, text2, new object[]
					{
						text2
					});
					ExTraceGlobals.XtcTracer.TraceError<string>(0L, "External authentification is disabled, remote mailbox/archive access for user {0} will be disabled.", text2);
					throw new MailboxOfflineException(ServerStrings.CannotAccessRemoteMailbox);
				}
				MailboxSession.RegisterCallback();
				string text3 = string.Format("{0}/{1}", remoteEndpoint, CertificateValidationManager.GenerateComponentIdQueryString("XRopXsoClient"));
				Uri internetWebProxy = null;
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && null != localServer.InternetWebProxy)
				{
					ExTraceGlobals.SessionTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Using configured InternetWebProxy {0}", localServer.InternetWebProxy);
					internetWebProxy = localServer.InternetWebProxy;
				}
				FederatedClientCredentials federatedCredentialsForDelegation = base.RemoteMailboxProperties.GetFederatedCredentialsForDelegation(current);
				client = new Client(federatedCredentialsForDelegation, new Uri(text3), internetWebProxy, this.mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), !this.nonInteractiveSession);
				bool flag2 = false;
				try
				{
					if (this != null)
					{
						this.BeginMapiCall();
						this.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiStore = MapiStore.OpenRemoteMailbox(text3, (base.LogonType == LogonType.Admin || base.LogonType == LogonType.SystemService) ? null : text, text, connectFlag, base.StoreFlag, this.InternalPreferedCulture, client, text, base.ClientInfoString, ClientSessionInfo.WrapInfoForRemoteServer(this));
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.mailboxOwner.MailboxInfo.RemoteIdentity.ToString()), ex, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::ForceOpen. Endpoint = {0}, Identity = {1}, User = {2}.", remoteEndpoint, this.mailboxOwner.MailboxInfo.RemoteIdentity, this.userLegacyDn),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.mailboxOwner.MailboxInfo.RemoteIdentity.ToString()), ex2, this, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MailboxSession::ForceOpen. Endpoint = {0}, Identity = {1}, User = {2}.", remoteEndpoint, this.mailboxOwner.MailboxInfo.RemoteIdentity, this.userLegacyDn),
						ex2
					});
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag2)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (mapiStore != null)
					{
						mapiStore.Dispose();
						mapiStore = null;
					}
					if (client != null)
					{
						client.Dispose();
						client = null;
					}
				}
			}
			return mapiStore;
		}

		private MapiStore InternalGetAggregatedMailboxConnection(MapiStore linkedStore, ClientIdentityInfo clientIdentity, byte[] partitionHint)
		{
			return this.InternalGetAggregatedMailboxConnection(linkedStore, () => MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), null, null, null, this.connectFlag, this.StoreFlag, this.InternalPreferedCulture, clientIdentity, this.ClientInfoString, partitionHint), partitionHint);
		}

		private MapiStore InternalGetAggregatedMailboxConnection(MapiStore linkedStore, WindowsIdentity windowsIdentity, byte[] partitionHint)
		{
			return this.InternalGetAggregatedMailboxConnection(linkedStore, () => MapiStore.OpenMailbox(this.mailboxOwner.MailboxInfo.Location.ServerFqdn, this.userLegacyDn, this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), null, null, null, this.connectFlag, this.StoreFlag, this.InternalPreferedCulture, windowsIdentity, this.ClientInfoString, partitionHint), partitionHint);
		}

		private MapiStore InternalGetAggregatedMailboxConnection(MapiStore linkedStore, Func<MapiStore> openMailbox, byte[] partitionHint)
		{
			MapiStore result = null;
			base.StoreFlag |= OpenStoreFlag.MailboxGuid;
			ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "MailboxSession::ForceOpen. Use user's MailboxGuid. MailboxGuid = {0}.", this.InternalMailboxOwner);
			if (linkedStore == null)
			{
				bool flag = false;
				try
				{
					try
					{
						if (this != null)
						{
							this.BeginMapiCall();
							this.BeginServerHealthCall();
							flag = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						result = openMailbox();
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, alt mbx Guid = {3}.", new object[]
							{
								this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
								this.userLegacyDn,
								this.mailboxOwner.LegacyDn,
								this.mailboxOwner.MailboxInfo.MailboxGuid
							}),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex2, this, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, alt mbx Guid = {3}.", new object[]
							{
								this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
								this.userLegacyDn,
								this.mailboxOwner.LegacyDn,
								this.mailboxOwner.MailboxInfo.MailboxGuid
							}),
							ex2
						});
					}
					return result;
				}
				finally
				{
					try
					{
						if (this != null)
						{
							this.EndMapiCall();
							if (flag)
							{
								this.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			bool flag2 = false;
			try
			{
				if (this != null)
				{
					this.BeginMapiCall();
					this.BeginServerHealthCall();
					flag2 = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = linkedStore.OpenAlternateMailbox(this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.GetDatabaseGuid(), base.StoreFlag, this.InternalPreferedCulture, base.ClientInfoString, partitionHint);
			}
			catch (MapiPermanentException ex3)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex3, this, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, alt mbx Guid = {3}.", new object[]
					{
						this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
						this.userLegacyDn,
						this.mailboxOwner.LegacyDn,
						this.mailboxOwner.MailboxInfo.MailboxGuid
					}),
					ex3
				});
			}
			catch (MapiRetryableException ex4)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotOpenMailbox(this.userLegacyDn), ex4, this, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MailboxSession::ForceOpen. server = {0}, user = {1}, owner = {2}, alt mbx Guid = {3}.", new object[]
					{
						this.mailboxOwner.MailboxInfo.Location.ServerFqdn,
						this.userLegacyDn,
						this.mailboxOwner.LegacyDn,
						this.mailboxOwner.MailboxInfo.MailboxGuid
					}),
					ex4
				});
			}
			finally
			{
				try
				{
					if (this != null)
					{
						this.EndMapiCall();
						if (flag2)
						{
							this.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		private bool ShouldThrowWrongServerException(IExchangePrincipal mailboxOwner)
		{
			return !this.IgnoreActiveManagerSiteBoundary() && !mailboxOwner.IsCrossSiteAccessAllowed && mailboxOwner.MailboxInfo.Location != null && !mailboxOwner.MailboxInfo.Location.IsLocal();
		}

		private void InternalDisposeServerObjects()
		{
			if (this.siteMailboxSynchronizerReference != null)
			{
				this.siteMailboxSynchronizerReference.Dispose();
				this.siteMailboxSynchronizerReference = null;
			}
			if (this.inboxRules != null)
			{
				this.inboxRules.Folder.Dispose();
				this.inboxRules = null;
			}
		}

		private void CaptureMarkAsClutterOrNotClutter(FolderChangeOperation operation, FolderChangeOperationFlags flags, GroupOperationResult result, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId)
		{
			UserMoveActionHandler userMoveActionHandler = null;
			if (UserMoveActionHandler.TryCreate(this, operation, flags, result, sourceFolderId, destinationFolderId, out userMoveActionHandler))
			{
				userMoveActionHandler.HandleUserMoves();
			}
		}

		private const string FreeBusyMessage = "LocalFreebusy";

		private const int RecipientCacheSize = 1024;

		private const MailboxSession.InitializationFlags OwnerFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties;

		private const MailboxSession.InitializationFlags TransportFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties;

		private const MailboxSession.InitializationFlags AdminFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.UseNamedProperties;

		private const MailboxSession.InitializationFlags SystemServiceFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties;

		private const MailboxSession.InitializationFlags MrsFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties;

		private const MailboxSession.InitializationFlags DelegateFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.CopyOnWrite | MailboxSession.InitializationFlags.DeadSessionChecking | MailboxSession.InitializationFlags.CheckPrivateItemsAccess | MailboxSession.InitializationFlags.UseNamedProperties;

		private const int ErrorSavingMailboxAuditLengthThreshold = 30000;

		private const string RemoteMailboxComponentId = "XRopXsoClient";

		private const string GroupMailboxOperation = "MungeUserToken";

		private static readonly Regex AllowedClientsForPublicFolderMailbox = new Regex(string.Format("\\A({0}|{1});Action=(SetMailboxFolderPermissionBase|Get-MailboxFolderPermission|Test-MapiConnectivity)", "Client=Management", "Client=Monitoring"), RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

		private static DefaultFolderType[] allDefaultFolders = (DefaultFolderType[])Enum.GetValues(typeof(DefaultFolderType));

		private static readonly PropertyDefinition[] IdDefinition = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] DelegateDefinitions = new PropertyDefinition[]
		{
			InternalSchema.DelegateEntryIds2,
			InternalSchema.DelegateFlags
		};

		private static readonly PropertyDefinition[] DeferredActionMessagesDefinitions = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.OriginalMessageSvrEId
		};

		private static readonly QueryFilter FreeBusyQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Subject, "LocalFreebusy");

		private static readonly StorePropertyDefinition[] ItemSchemaIdStoreDefinition = new StorePropertyDefinition[]
		{
			ItemSchema.Id
		};

		public static ReadOnlyCollection<DefaultFolderType> AllDefaultFolders = new ReadOnlyCollection<DefaultFolderType>(MailboxSession.allDefaultFolders);

		private static CultureInfo productDefaultCulture = new CultureInfo("en-US");

		private IADOrgPerson delegateUser;

		private bool canDispose;

		private MailboxSession masterMailboxSession;

		private DelegateSessionManager delegateSessionManager;

		private IExchangePrincipal mailboxOwner;

		private ExTimeZone exTimeZone = ExTimeZone.UtcTimeZone;

		private MasterCategoryList masterCategoryList;

		private PropertyDefinition[] mailboxProperties;

		private static PropertyDefinition[] mailboxItemCountsAndSizesProperties = new PropertyDefinition[]
		{
			InternalSchema.ItemCount,
			MailboxSchema.QuotaUsedExtended,
			InternalSchema.DeletedMsgCount,
			MailboxSchema.DumpsterQuotaUsedExtended
		};

		private IList<DefaultFolderType> foldersToInit;

		private bool useNamedProperties;

		private bool filterPrivateItems;

		private bool disableFilterPrivateItems;

		private UserConfigurationManager userConfigurationManager;

		private DefaultFolderManager defaultFolderManager;

		private bool isDefaultFolderManagerBeingInitialized;

		private CultureInfo preferedCultureInfoCache;

		private COWSession copyOnWriteNotification;

		private bool bypassAuditsFolderAccessChecking;

		private bool bypassAuditing;

		private static readonly object lockRegisterCertificateErrorHandler = new object();

		private IActivitySession activitySession;

		private ContactFolders contactFolders;

		private MailboxSessionSharableDataManager sharedDataManager;

		internal Hookable<IActivitySession> activitySessionHook;

		public static ReadOnlyCollection<DefaultFolderType> FreeDefaultFolders = new ReadOnlyCollection<DefaultFolderType>(new DefaultFolderType[]
		{
			DefaultFolderType.Configuration,
			DefaultFolderType.Root,
			DefaultFolderType.Inbox,
			DefaultFolderType.LegacySpoolerQueue
		});

		public static ReadOnlyCollection<DefaultFolderType> AlwaysInitDefaultFolders = new ReadOnlyCollection<DefaultFolderType>(new DefaultFolderType[]
		{
			DefaultFolderType.Root,
			DefaultFolderType.Configuration,
			DefaultFolderType.Inbox,
			DefaultFolderType.Outbox,
			DefaultFolderType.SentItems,
			DefaultFolderType.DeletedItems,
			DefaultFolderType.CommonViews,
			DefaultFolderType.SearchFolders,
			DefaultFolderType.DeferredActionFolder,
			DefaultFolderType.LegacySchedule,
			DefaultFolderType.LegacyShortcuts,
			DefaultFolderType.LegacyViews,
			DefaultFolderType.System,
			DefaultFolderType.AdminAuditLogs,
			DefaultFolderType.Audits,
			DefaultFolderType.Clutter
		});

		private static ReadOnlyCollection<DefaultFolderType> DefaultFoldersToLocalizeOnFirstLogon = new ReadOnlyCollection<DefaultFolderType>(new DefaultFolderType[]
		{
			DefaultFolderType.Root,
			DefaultFolderType.Inbox,
			DefaultFolderType.Outbox,
			DefaultFolderType.SentItems,
			DefaultFolderType.DeletedItems,
			DefaultFolderType.Calendar,
			DefaultFolderType.Contacts,
			DefaultFolderType.Drafts,
			DefaultFolderType.Journal,
			DefaultFolderType.Notes,
			DefaultFolderType.Tasks
		});

		private bool isUnified;

		private bool? isAuditConfigFromUCCPolicyEnabled;

		private static MailboxSession.CheckSubmissionQuotaDelegate checkSubmissionQuotaDelegate = (string mailboxServer, int mailboxServerVersion, Guid mailboxGuid, int recipientCount, ADObjectId throttlingPolicyId, OrganizationId organizationId, string clientInfo) => MailboxThrottle.Instance.ObtainUserSubmissionTokens(mailboxServer, mailboxServerVersion, mailboxGuid, recipientCount, throttlingPolicyId, organizationId, clientInfo);

		private static bool registeredCertificateErrorHandler = false;

		private Rules inboxRules;

		private Rules allInboxRules;

		private Dictionary<RetentionActionType, PolicyTagList> policyTagListDictionary;

		private bool nonInteractiveSession;

		private SiteMailboxSynchronizerReference siteMailboxSynchronizerReference;

		private IAuditLog auditLog;

		[Flags]
		public enum InitializationFlags
		{
			None = 0,
			DefaultFolders = 1,
			UserConfigurationManager = 2,
			CopyOnWrite = 4,
			DeadSessionChecking = 8,
			CheckPrivateItemsAccess = 16,
			RequestLocalRpc = 32,
			OverrideHomeMdb = 64,
			QuotaMessageDelivery = 128,
			NormalMessageDelivery = 256,
			SpecialMessageDelivery = 512,
			SuppressFolderIdPrefetch = 1024,
			UseNamedProperties = 2048,
			DeferDefaultFolderIdInitialization = 4096,
			UseRecoveryDatabase = 8192,
			NonInteractiveSession = 16384,
			DisconnectedMailbox = 32768,
			XForestMove = 65536,
			MoveUser = 131072,
			IgnoreForcedFolderInit = 262144,
			AllowAdminLocalization = 524288,
			ReadOnly = 1048576,
			OlcSync = 2097152
		}

		private delegate void InitializeMailboxSessionFailure();

		public struct MailboxItemCountsAndSizes
		{
			public int? ItemCount;

			public long? TotalItemSize;

			public int? DeletedItemCount;

			public long? TotalDeletedItemSize;
		}

		internal delegate bool CheckSubmissionQuotaDelegate(string mailboxServer, int mailboxServerVersion, Guid mailboxGuid, int recipientCount, ADObjectId throttlingPolicyId, OrganizationId organizationId, string clientInfo);
	}
}
