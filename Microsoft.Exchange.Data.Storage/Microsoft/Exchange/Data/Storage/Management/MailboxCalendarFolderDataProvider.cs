using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxCalendarFolderDataProvider : MailboxFolderDataProviderBase
	{
		public MailboxCalendarFolderDataProvider(ADSessionSettings adSessionSettings, ADUser mailboxOwner, string action) : base(adSessionSettings, mailboxOwner, action)
		{
		}

		internal MailboxCalendarFolderDataProvider()
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxCalendarFolderDataProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId == null)
			{
				throw new NotSupportedException("rootId must be specified");
			}
			if (!(rootId is MailboxFolderId))
			{
				throw new NotSupportedException("rootId: " + rootId.GetType().FullName);
			}
			if (!typeof(MailboxCalendarFolder).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			MailboxFolderId mailboxFolderId = (MailboxFolderId)rootId;
			StoreObjectId folderId = mailboxFolderId.StoreObjectIdValue ?? base.ResolveStoreObjectIdFromFolderPath(mailboxFolderId.MailboxFolderPath);
			if (folderId == null)
			{
				throw new CantFindCalendarFolderException(mailboxFolderId);
			}
			using (Folder folder = Folder.Bind(base.MailboxSession, folderId))
			{
				if (!StringComparer.OrdinalIgnoreCase.Equals(folder.ClassName, "IPF.Appointment"))
				{
					throw new CantFindCalendarFolderException(mailboxFolderId);
				}
			}
			MailboxCalendarFolder mailboxCalendarFolder = (MailboxCalendarFolder)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			mailboxCalendarFolder.MailboxFolderId = mailboxFolderId;
			UserConfigurationDictionaryHelper.Fill(mailboxCalendarFolder, MailboxCalendarFolder.CalendarFolderConfigurationProperties, (bool createIfNonexisting) => UserConfigurationHelper.GetPublishingConfiguration(this.MailboxSession, folderId, createIfNonexisting));
			yield return (T)((object)mailboxCalendarFolder);
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			MailboxCalendarFolder mailboxCalendarFolder = instance as MailboxCalendarFolder;
			if (mailboxCalendarFolder == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			if (mailboxCalendarFolder.PublishEnabled)
			{
				SharingPolicy sharingPolicy = DirectoryHelper.ReadSharingPolicy(base.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, base.MailboxSession.MailboxOwner.MailboxInfo.IsArchive, base.MailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
				if (sharingPolicy == null || !sharingPolicy.Enabled || !sharingPolicy.IsAllowedForAnonymousCalendarSharing())
				{
					throw new NotAllowedPublishingByPolicyException();
				}
				SharingPolicyAction allowedForAnonymousCalendarSharing = sharingPolicy.GetAllowedForAnonymousCalendarSharing();
				int maxAllowed = PolicyAllowedDetailLevel.GetMaxAllowed(allowedForAnonymousCalendarSharing);
				if (mailboxCalendarFolder.DetailLevel > (DetailLevelEnumType)maxAllowed)
				{
					throw new NotAllowedPublishingByPolicyException(mailboxCalendarFolder.DetailLevel, (DetailLevelEnumType)maxAllowed);
				}
			}
			MailboxFolderId mailboxFolderId = mailboxCalendarFolder.MailboxFolderId;
			StoreObjectId folderId = mailboxFolderId.StoreObjectIdValue ?? base.ResolveStoreObjectIdFromFolderPath(mailboxFolderId.MailboxFolderPath);
			if (folderId == null || folderId.ObjectType != StoreObjectType.CalendarFolder)
			{
				throw new CantFindCalendarFolderException(mailboxFolderId);
			}
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(base.MailboxSession, folderId))
			{
				ExtendedFolderFlags? valueAsNullable = calendarFolder.GetValueAsNullable<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags);
				if (valueAsNullable != null && (valueAsNullable.Value & ExtendedFolderFlags.PersonalShare) != (ExtendedFolderFlags)0)
				{
					throw new CannotShareFolderException(ServerStrings.CannotShareOtherPersonalFolder);
				}
				this.SaveSharingAnonymous(mailboxCalendarFolder, folderId);
				if (!mailboxCalendarFolder.PublishEnabled)
				{
					mailboxCalendarFolder.PublishedCalendarUrl = null;
					mailboxCalendarFolder.PublishedICalUrl = null;
				}
				UserConfigurationDictionaryHelper.Save(mailboxCalendarFolder, MailboxCalendarFolder.CalendarFolderConfigurationProperties, (bool createIfNonexisting) => UserConfigurationHelper.GetPublishingConfiguration(this.MailboxSession, folderId, createIfNonexisting));
				if (MailboxCalendarFolderDataProvider.UpdateExtendedFolderFlags(mailboxCalendarFolder, calendarFolder))
				{
					calendarFolder.Save();
				}
			}
		}

		private static bool UpdateExtendedFolderFlags(MailboxCalendarFolder mailboxCalendarFolder, Folder folder)
		{
			ExtendedFolderFlags? valueAsNullable = folder.GetValueAsNullable<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags);
			ExtendedFolderFlags extendedFolderFlags;
			if (mailboxCalendarFolder.PublishEnabled)
			{
				if (valueAsNullable != null && (valueAsNullable.Value & ExtendedFolderFlags.ExchangePublishedCalendar) == ExtendedFolderFlags.ExchangePublishedCalendar)
				{
					return false;
				}
				extendedFolderFlags = ((valueAsNullable != null) ? (valueAsNullable.Value | ExtendedFolderFlags.ExchangePublishedCalendar) : ExtendedFolderFlags.ExchangePublishedCalendar);
			}
			else
			{
				if (valueAsNullable == null || (valueAsNullable.Value & ExtendedFolderFlags.ExchangePublishedCalendar) == (ExtendedFolderFlags)0)
				{
					return false;
				}
				extendedFolderFlags = (valueAsNullable.Value & (ExtendedFolderFlags)2147483647);
			}
			folder[FolderSchema.ExtendedFolderFlags] = extendedFolderFlags;
			return true;
		}

		private void SaveSharingAnonymous(MailboxCalendarFolder mailboxCalendarFolder, StoreObjectId folderId)
		{
			Util.ThrowOnNullArgument(mailboxCalendarFolder, "mailboxCalendarFolder");
			Util.ThrowOnNullArgument(folderId, "folderId");
			IRecipientSession adrecipientSession = base.MailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent);
			ADRecipient adrecipient = adrecipientSession.Read(base.MailboxSession.MailboxOwner.ObjectId);
			if (adrecipient == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "This is not an ADUser so SharingAnonymousIdentities doesn't apply. Recipient = {0}.", adrecipient);
				return;
			}
			if (mailboxCalendarFolder.PublishEnabled)
			{
				PublishingUrl publishingUrl = PublishingUrl.Create(mailboxCalendarFolder.PublishedCalendarUrl);
				ExTraceGlobals.SharingTracer.TraceDebug<ADUser, string, StoreObjectId>((long)this.GetHashCode(), "Publish is enabled and trying to add or update SharingAnonymousIdentities. User={0}; Url Identity={1}; Folder={2}.", aduser, publishingUrl.Identity, folderId);
				aduser.SharingAnonymousIdentities.AddOrUpdate(publishingUrl.DataType.ExternalName, publishingUrl.Identity, folderId.ToBase64String());
			}
			else
			{
				PublishingUrl publishingUrl2 = PublishingUrl.Create(mailboxCalendarFolder.PublishedCalendarUrl);
				ExTraceGlobals.SharingTracer.TraceDebug<ADUser, string, StoreObjectId>((long)this.GetHashCode(), "Publish is NOT enabled and trying to remove SharingAnonymousIdentities. User={0}; Url Identity={1}; Folder={2}.", aduser, publishingUrl2.Identity, folderId);
				aduser.SharingAnonymousIdentities.Remove(publishingUrl2.Identity);
			}
			if (aduser.SharingAnonymousIdentities.Changed)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADUser>((long)this.GetHashCode(), "Save SharingAnonymousIdentities on user {0}.", aduser);
				adrecipientSession.Save(aduser);
			}
		}

		private const ExtendedFolderFlags PublishedCalendarFlags = ExtendedFolderFlags.ExchangePublishedCalendar;
	}
}
