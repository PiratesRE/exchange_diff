using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ConfigureGroupMailbox
	{
		public ConfigureGroupMailbox(IRecipientSession adSession, ADUser group, ADUser executingUser, MailboxSession mailboxSession)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfInvalidValue<IRecipientSession>("adSession", adSession, (IRecipientSession session) => !session.ReadOnly);
			ArgumentValidator.ThrowIfNull("group", group);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfInvalidValue<MailboxSession>("mailboxSession", mailboxSession, (MailboxSession session) => session.MailboxOwner.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox);
			this.adSession = adSession;
			this.group = group;
			this.executingUser = executingUser;
			this.mailboxSession = mailboxSession;
			this.mailboxStoreTypeProvider = new MailboxStoreTypeProvider(this.group)
			{
				MailboxSession = this.mailboxSession
			};
		}

		private IExchangePrincipal MailboxPrincipal
		{
			get
			{
				return this.mailboxSession.MailboxOwner;
			}
		}

		public static MailboxSession CreateMailboxSessionForConfiguration(ExchangePrincipal groupPrincipal, string domainController)
		{
			MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(groupPrincipal, CultureInfo.InvariantCulture, "Client=WebServices;Action=ConfigureGroupMailbox");
			mailboxSession.SetADRecipientSessionFactory((bool isReadonly, ConsistencyMode consistencyMode) => DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, isReadonly, consistencyMode, null, groupPrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 127, "CreateMailboxSessionForConfiguration", "f:\\15.00.1497\\sources\\dev\\UnifiedGroups\\src\\UnifiedGroups\\GroupMailboxAccessLayer\\Commands\\ConfigureGroupMailbox.cs"));
			return mailboxSession;
		}

		public GroupMailboxConfigurationReport Execute(GroupMailboxConfigurationAction forceActionMask)
		{
			this.report = new GroupMailboxConfigurationReport();
			if (this.group.IsGroupMailboxConfigured && forceActionMask == GroupMailboxConfigurationAction.None)
			{
				return this.report;
			}
			Predicate<GroupMailboxConfigurationAction> condition = (GroupMailboxConfigurationAction action) => !this.group.IsGroupMailboxConfigured || forceActionMask.Contains(action);
			this.ExecuteIf(condition, GroupMailboxConfigurationAction.SetRegionalSettings, new Action(this.SetRegionalConfiguration));
			this.ExecuteIf(condition, GroupMailboxConfigurationAction.CreateDefaultFolders, new Action(this.CreateDefaultFolders));
			this.ExecuteIf((GroupMailboxConfigurationAction actionType) => !this.group.IsGroupMailboxConfigured, GroupMailboxConfigurationAction.SetInitialFolderPermissions, new Action(this.SetFolderPermissions));
			this.ExecuteIf(condition, GroupMailboxConfigurationAction.ConfigureCalendar, new Action(this.ConfigureCalendar));
			this.ExecuteIf((GroupMailboxConfigurationAction actionType) => !this.group.IsGroupMailboxConfigured, GroupMailboxConfigurationAction.GenerateGroupPhoto, new Action(this.UploadPhoto));
			this.ExecuteIf(condition, GroupMailboxConfigurationAction.SendWelcomeMessage, new Action(this.SendWelcomeMessage));
			this.MarkGroupConfigured();
			return this.report;
		}

		private void UploadPhoto()
		{
			try
			{
				if (GroupMailboxDefaultPhotoUploader.IsFlightEnabled(this.mailboxSession))
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						GroupMailboxDefaultPhotoUploader groupMailboxDefaultPhotoUploader = new GroupMailboxDefaultPhotoUploader(this.adSession, this.mailboxSession, this.group);
						this.group.ThumbnailPhoto = groupMailboxDefaultPhotoUploader.Upload();
					}, (Exception e) => GrayException.IsSystemGrayException(e));
				}
			}
			catch (LocalizedException arg)
			{
				this.TraceAndReportWarning(new LocalizedString(string.Format("Unable to upload photo for group {0} Error {1}", this.group.PrimarySmtpAddress, arg)));
			}
		}

		private void SetRegionalConfiguration()
		{
			MailboxRegionalConfiguration mailboxRegionalConfiguration = new MailboxRegionalConfiguration
			{
				TimeZone = ExTimeZoneValue.Parse("Pacific Standard Time"),
				Principal = this.MailboxPrincipal
			};
			mailboxRegionalConfiguration.Save(this.mailboxStoreTypeProvider);
		}

		private void ConfigureCalendar()
		{
			MailboxCalendarConfiguration mailboxCalendarConfiguration = new MailboxCalendarConfiguration
			{
				Principal = this.MailboxPrincipal,
				RemindersEnabled = false,
				ReminderSoundEnabled = false
			};
			mailboxCalendarConfiguration.Save(this.mailboxStoreTypeProvider);
			this.TraceDebug("Save settings to disable calendar reminder", new object[0]);
			using (CalendarConfigurationDataProvider calendarConfigurationDataProvider = new CalendarConfigurationDataProvider(this.mailboxSession))
			{
				CalendarConfiguration instance = new CalendarConfiguration
				{
					MailboxOwnerId = this.group.Id,
					RemoveForwardedMeetingNotifications = true,
					RemoveOldMeetingMessages = true
				};
				calendarConfigurationDataProvider.Save(instance);
				this.TraceDebug("Save settings to disable calendar forward notification.", new object[0]);
			}
		}

		private void CreateDefaultFolders()
		{
			int num = 0;
			foreach (DefaultFolderType defaultFolderType in this.mailboxSession.DefaultFolders)
			{
				if (!GroupMailboxPermissionHandler.IsFolderToBeIgnored(defaultFolderType))
				{
					this.TraceDebug("Getting DefaultFolderId, DefaultFolderType={0}", new object[]
					{
						defaultFolderType
					});
					if (this.mailboxSession.GetDefaultFolderId(defaultFolderType) == null)
					{
						this.TraceDebug("Creating Folder {0}", new object[]
						{
							defaultFolderType
						});
						this.mailboxSession.CreateDefaultFolder(defaultFolderType);
						num++;
					}
				}
			}
			this.report.FoldersCreatedCount = num;
		}

		private void SetFolderPermissions()
		{
			ExternalUser externalUser = ExternalUser.CreateExternalUserForGroupMailbox(this.MailboxPrincipal.MailboxInfo.DisplayName, "Member@local", this.MailboxPrincipal.MailboxInfo.MailboxGuid, SecurityIdentity.GroupMailboxMemberType.Member);
			ExternalUser externalUser2 = ExternalUser.CreateExternalUserForGroupMailbox(this.MailboxPrincipal.MailboxInfo.DisplayName, "Owner@local", this.MailboxPrincipal.MailboxInfo.MailboxGuid, SecurityIdentity.GroupMailboxMemberType.Owner);
			using (ExternalUserCollection externalUsers = this.mailboxSession.GetExternalUsers())
			{
				if (!externalUsers.Contains(externalUser))
				{
					externalUsers.Add(externalUser);
				}
				if (!externalUsers.Contains(externalUser2))
				{
					externalUsers.Add(externalUser2);
				}
				externalUsers.Save();
				if (!externalUsers.Contains(externalUser))
				{
					throw new GroupMailboxFailedToAddExternalUserException(Strings.ErrorUnableToAddExternalUser(externalUser.Name));
				}
				if (!externalUsers.Contains(externalUser2))
				{
					throw new GroupMailboxFailedToAddExternalUserException(Strings.ErrorUnableToAddExternalUser(externalUser2.Name));
				}
				this.TraceDebug("Added external member user {0} to external user collection", new object[]
				{
					externalUser.Name
				});
				this.TraceDebug("Added external owner user {0} to external user collection", new object[]
				{
					externalUser2.Name
				});
			}
			PermissionSecurityPrincipal userSecurityPrincipal = new PermissionSecurityPrincipal(externalUser);
			PermissionSecurityPrincipal userSecurityPrincipal2 = new PermissionSecurityPrincipal(externalUser2);
			int num = 0;
			List<PermissionEntry> list = new List<PermissionEntry>(3);
			var array = new <>f__AnonymousType0<DefaultFolderType, MemberRights, MemberRights>[]
			{
				new
				{
					Folder = DefaultFolderType.MailboxAssociation,
					OwnerPermission = GroupMailboxPermissionHandler.MailboxAssociationPermission,
					MemberPermission = GroupMailboxPermissionHandler.MailboxAssociationPermission
				},
				new
				{
					Folder = DefaultFolderType.SearchFolders,
					OwnerPermission = (GroupMailboxPermissionHandler.SearchFolderPermission | GroupMailboxPermissionHandler.OwnerSpecificPermission),
					MemberPermission = GroupMailboxPermissionHandler.SearchFolderPermission
				},
				new
				{
					Folder = DefaultFolderType.Calendar,
					OwnerPermission = GroupMailboxPermissionHandler.CalendarFolderPermission,
					MemberPermission = GroupMailboxPermissionHandler.CalendarFolderPermission
				}
			};
			list.Add(new PermissionEntry(userSecurityPrincipal2, GroupMailboxPermissionHandler.ConfigurationFolderPermission));
			int num2;
			GroupMailboxPermissionHandler.AssignMemberRight(this.mailboxSession, list, DefaultFolderType.Configuration, out num2);
			num += num2;
			var array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				var <>f__AnonymousType = array2[i];
				list.Clear();
				list.Add(new PermissionEntry(userSecurityPrincipal2, <>f__AnonymousType.OwnerPermission));
				list.Add(new PermissionEntry(userSecurityPrincipal, <>f__AnonymousType.MemberPermission));
				if (!GroupMailboxPermissionHandler.AssignMemberRight(this.mailboxSession, list, <>f__AnonymousType.Folder, out num2))
				{
					throw new GroupMailboxFailedToConfigureMailboxException(Strings.ErrorUnableToConfigureMailbox(<>f__AnonymousType.Folder.ToString(), this.MailboxPrincipal.MailboxInfo.DisplayName));
				}
				num += num2;
			}
			this.report.FoldersPrivilegedCount = num;
			this.mailboxSession.Mailbox[MailboxSchema.GroupMailboxPermissionsVersion] = GroupMailboxPermissionHandler.GroupMailboxPermissionVersion;
			this.mailboxSession.Mailbox.Save();
			this.mailboxSession.Mailbox.Load();
		}

		private void SendWelcomeMessage()
		{
			try
			{
				GroupWarmingMessageComposer groupWarmingMessageComposer = new GroupWarmingMessageComposer(this.group, this.executingUser);
				using (MessageItem messageItem = MessageItem.Create(this.mailboxSession, this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)))
				{
					groupWarmingMessageComposer.WriteToMessage(messageItem);
					messageItem.Save(SaveMode.NoConflictResolution);
				}
			}
			catch (LocalizedException ex)
			{
				this.TraceAndReportWarning(Strings.WarningUnableToSendWelcomeMessage(ex.Message));
			}
		}

		private void MarkGroupConfigured()
		{
			if (this.group.IsGroupMailboxConfigured)
			{
				return;
			}
			this.group.IsGroupMailboxConfigured = true;
			this.adSession.Save(this.group);
			this.TraceDebug("Set group's IsGroupMailboxConfigured property to true. ExternalDirectoryObjectId={0}", new object[]
			{
				this.group.ExternalDirectoryObjectId
			});
		}

		private void ExecuteIf(Predicate<GroupMailboxConfigurationAction> condition, GroupMailboxConfigurationAction actionType, Action action)
		{
			if (condition(actionType))
			{
				using (new GroupMailboxConfigurationActionStopwatch(this.report, actionType))
				{
					action();
				}
			}
		}

		private void TraceDebug(string message, params object[] args)
		{
			ConfigureGroupMailbox.Tracer.TraceDebug((long)this.GetHashCode(), message, args);
		}

		private void TraceAndReportWarning(LocalizedString message)
		{
			ConfigureGroupMailbox.Tracer.TraceWarning((long)this.GetHashCode(), message);
			this.report.Warnings.Add(message);
		}

		private const string DefaultTimeZone = "Pacific Standard Time";

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		private readonly IRecipientSession adSession;

		private readonly ADUser group;

		private readonly ADUser executingUser;

		private readonly MailboxSession mailboxSession;

		private readonly MailboxStoreTypeProvider mailboxStoreTypeProvider;

		private GroupMailboxConfigurationReport report;
	}
}
