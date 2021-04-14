using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AutomaticLink : ContactLink
	{
		public AutomaticLink(MailboxInfoForLinking mailboxInfo, IExtensibleLogger logger, IContactLinkingPerformanceTracker performanceTracker, IDirectoryPersonSearcher directoryPersonSearcher, IContactStoreForContactLinking contactStoreForContactLinking) : base(mailboxInfo, logger, performanceTracker)
		{
			Util.ThrowOnNullArgument(directoryPersonSearcher, "directoryPersonSearcher");
			Util.ThrowOnNullArgument(contactStoreForContactLinking, "contactStoreForContactLinking");
			Util.ThrowOnNullArgument(performanceTracker, "performanceTracker");
			this.directoryPersonSearcher = directoryPersonSearcher;
			this.contactStoreForContactLinking = contactStoreForContactLinking;
		}

		public static void DisableAutomaticLinkingForItem(Item item)
		{
			item.PropertyBag.Context.AutomaticContactLinkingAction = AutomaticContactLinkingAction.Ignore;
		}

		public static void ForceAutomaticLinkingForItem(Item item)
		{
			item.PropertyBag.Context.AutomaticContactLinkingAction = AutomaticContactLinkingAction.Force;
		}

		public void LinkAllExistingContacts()
		{
			this.LinkAllExistingContactsTimeBound(ExDateTime.MaxValue);
		}

		internal static void PerformContactLinkingAfterMigration(StoreSession storeSession)
		{
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				ContactLink.Tracer.TraceDebug(0L, "AutomaticLink::PerformContactLinkingAfterMigration. Skiping session as it is not a MailboxSession.");
				return;
			}
			ContactLink.Tracer.TraceDebug<MailboxSession>(0L, "StoreSession::PerformContactLinking. Starting linking for {0}.", mailboxSession);
			ExDateTime exDateTime = ExDateTime.Now.Add(AutomaticLink.AllocatedTimeSlotForLinkingOnMigration.Value);
			MailboxInfoForLinking mailboxInfoForLinking = MailboxInfoForLinking.CreateFromMailboxSession(mailboxSession);
			ContactLinkingPerformanceTracker performanceTracker = new ContactLinkingPerformanceTracker(mailboxSession);
			DirectoryPersonSearcher directoryPersonSearcher = new DirectoryPersonSearcher(mailboxSession.MailboxOwner);
			ContactStoreForBulkContactLinking contactStoreForBulkContactLinking = new ContactStoreForBulkContactLinking(mailboxSession, performanceTracker);
			ContactLinkingLogger contactLinkingLogger = new ContactLinkingLogger("PerformContactLinkingAfterMigration", mailboxInfoForLinking);
			bool flag = false;
			contactLinkingLogger.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.MigrationStart>
			{
				{
					ContactLinkingLogSchema.MigrationStart.DueTime,
					(DateTime)exDateTime
				}
			});
			try
			{
				AutomaticLink.RefreshContactLinkingDefaultFolderIds(mailboxSession);
				AutomaticLink automaticLink = new AutomaticLink(mailboxInfoForLinking, contactLinkingLogger, performanceTracker, directoryPersonSearcher, contactStoreForBulkContactLinking);
				automaticLink.LinkAllExistingContactsTimeBound(exDateTime);
				flag = true;
			}
			catch (DefaultFolderNameClashException e)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e);
			}
			catch (AccessDeniedException e2)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e2);
			}
			catch (ConnectionFailedTransientException e3)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e3);
			}
			catch (FolderSaveTransientException e4)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e4);
			}
			catch (MailboxUnavailableException e5)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e5);
			}
			catch (ObjectNotFoundException e6)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e6);
			}
			catch (StorageTransientException e7)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e7);
			}
			catch (StoragePermanentException e8)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e8);
			}
			catch (Exception e9)
			{
				AutomaticLink.TraceAndLogExceptionDuringLinkingPostMigration(mailboxInfoForLinking, contactLinkingLogger, e9);
				throw;
			}
			finally
			{
				contactLinkingLogger.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.MigrationEnd>
				{
					{
						ContactLinkingLogSchema.MigrationEnd.Success,
						flag
					}
				});
				ContactLink.Tracer.TraceDebug<MailboxInfoForLinking, bool>(0L, "AutomaticLink::PerformContactLinkingAfterMigration. Done processing {0}. Contact linking completed successfully: {1}", mailboxInfoForLinking, flag);
			}
		}

		internal void LinkAllExistingContactsTimeBound(ExDateTime dueTime)
		{
			this.processingDueTime = new ExDateTime?(dueTime);
			base.PerformanceTracker.Start();
			try
			{
				if (!this.CanContinueProcessingContacts())
				{
					throw new InvalidOperationException("Can't start contact linking operation as conditions to continue processing are not met.");
				}
				HashSet<ContactInfoForLinking> hashSet = new HashSet<ContactInfoForLinking>();
				IEnumerable<ContactInfoForLinking> enumerable = this.contactStoreForContactLinking.GetAllContacts().Take(AutomaticLink.MaximumNumberOfContactsToProcess.Value);
				foreach (ContactInfoForLinking contactBeingSaved in enumerable)
				{
					if (!this.CanContinueProcessingContacts())
					{
						break;
					}
					base.PerformanceTracker.IncrementContactsProcessed();
					hashSet.UnionWith(this.Link(contactBeingSaved, enumerable));
				}
				base.Commit(hashSet);
			}
			finally
			{
				base.PerformanceTracker.Stop();
				base.LogEvent(base.PerformanceTracker.GetLogEvent());
			}
		}

		internal void LinkNewOrUpdatedContactBeforeSave(ICoreItem contactBeingSaved, Func<ContactInfoForLinking, IContactStoreForContactLinking, IEnumerable<ContactInfoForLinking>> otherContactsDelegate)
		{
			base.PerformanceTracker.Start();
			try
			{
				Util.ThrowOnNullArgument(otherContactsDelegate, "otherContactsDelegate");
				if (!this.CanContinueProcessingContacts())
				{
					throw new InvalidOperationException("Can't start contact linking operation as conditions to continue processing are not met.");
				}
				if (this.IsValidContactBeingSaved(contactBeingSaved))
				{
					ContactInfoForLinking contactInfoForLinking = ContactInfoForLinkingFromCoreObject.Create(contactBeingSaved);
					base.Commit(this.Link(contactInfoForLinking, otherContactsDelegate(contactInfoForLinking, this.contactStoreForContactLinking)));
				}
			}
			catch (Exception value)
			{
				ContactLink.Tracer.TraceDebug<MailboxInfoForLinking>(0L, "AutomaticLink::LinkNewOrUpdatedContactBeforeSave. Unhandled exception processing {0}.", base.MailboxInfo);
				base.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.Error>
				{
					{
						ContactLinkingLogSchema.Error.Context,
						"Unhandled Exception"
					},
					{
						ContactLinkingLogSchema.Error.Exception,
						value
					}
				});
				throw;
			}
			finally
			{
				base.PerformanceTracker.Stop();
				base.LogEvent(base.PerformanceTracker.GetLogEvent());
			}
		}

		internal IEnumerable<ContactInfoForLinking> Link(ContactInfoForLinking contactBeingSaved, IEnumerable<ContactInfoForLinking> otherContacts)
		{
			PersonId personId = contactBeingSaved.PersonId;
			HashSet<ContactInfoForLinking> hashSet = new HashSet<ContactInfoForLinking>(this.contactStoreForContactLinking.GetPersonContacts(personId));
			int num = (hashSet.Count > 0) ? hashSet.Count : 1;
			int maxLinkCount = AutomaticLink.MaximumNumberOfContactsPerPerson.Value - num;
			bool flag = false;
			HashSet<ContactInfoForLinking> hashSet2 = this.LinkWithContacts(contactBeingSaved, otherContacts, maxLinkCount, out flag);
			if (hashSet2.Count > 0 || flag)
			{
				hashSet2.Add(contactBeingSaved);
				hashSet2.UnionWith(hashSet);
				this.UpdatePersonPropertiesInAllContacts(contactBeingSaved, hashSet2);
			}
			return hashSet2;
		}

		private static void RefreshContactLinkingDefaultFolderIds(MailboxSession mailboxSession)
		{
			foreach (DefaultFolderType folderType in ContactsSearchFolderCriteria.MyContactsExtended.ScopeDefaultFolderTypes)
			{
				AutomaticLink.RefreshDefaultFolderWithRetry(mailboxSession, folderType);
			}
		}

		private static void RefreshDefaultFolderWithRetry(MailboxSession mailboxSession, DefaultFolderType folderType)
		{
			for (int i = 0; i < 2; i++)
			{
				try
				{
					mailboxSession.RefreshDefaultFolder(folderType);
					return;
				}
				catch (StorageTransientException arg)
				{
					ContactLink.Tracer.TraceError<StorageTransientException>(0L, "AutomaticLink.RefreshDefaultFolderWithRetry: retry due MailboxSession.RefreshDefaultFolder() failed with exception: {0} .", arg);
				}
			}
			mailboxSession.RefreshDefaultFolder(folderType);
		}

		private static void MergeGALLinkState(ContactInfoForLinking contactBeingSaved, ContactInfoForLinking otherContact)
		{
			switch (contactBeingSaved.GALLinkState)
			{
			case GALLinkState.NotLinked:
				contactBeingSaved.UpdateGALLinkFrom(otherContact);
				return;
			case GALLinkState.Linked:
			case GALLinkState.NotAllowed:
				otherContact.UpdateGALLinkFrom(contactBeingSaved);
				return;
			default:
				return;
			}
		}

		private static bool ShouldLink(ContactLinkingOperation operation)
		{
			switch (operation)
			{
			case ContactLinkingOperation.AutoLinkViaEmailAddress:
			case ContactLinkingOperation.AutoLinkViaIMAddress:
				break;
			default:
				if (operation != ContactLinkingOperation.AutoLinkViaGalLinkId)
				{
					return false;
				}
				break;
			}
			return true;
		}

		private static void TraceAndLogExceptionDuringLinkingPostMigration(MailboxInfoForLinking mailbox, ContactLinkingLogger logger, Exception e)
		{
			ContactLink.Tracer.TraceError<MailboxInfoForLinking, Exception>(0L, "AutomaticLink::PerformContactLinkingAfterMigration:  failed to link contacts post-migration of mailbox {0}.  Exception: {1}", mailbox, e);
			logger.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.Error>
			{
				{
					ContactLinkingLogSchema.Error.Context,
					"Failed to link contacts post-migration"
				},
				{
					ContactLinkingLogSchema.Error.Exception,
					e
				}
			});
		}

		private void UpdatePersonPropertiesInAllContacts(ContactInfoForLinking contactBeingSaved, ICollection<ContactInfoForLinking> matchingContacts)
		{
			if (matchingContacts.Count > 0)
			{
				foreach (ContactInfoForLinking contactInfoForLinking in matchingContacts)
				{
					if (!contactInfoForLinking.Linked)
					{
						ContactLink.Tracer.TraceWarning<ContactInfoForLinking>((long)this.GetHashCode(), "Matching contact had linked property set to false. Contact Information: {0}", contactInfoForLinking);
						contactInfoForLinking.Linked = true;
					}
					this.UpdatePersonaId(contactInfoForLinking, contactBeingSaved.PersonId);
					contactInfoForLinking.LinkRejectHistory = contactBeingSaved.LinkRejectHistory;
					contactInfoForLinking.UpdateGALLinkFrom(contactBeingSaved);
				}
			}
		}

		private bool TryFindDirectoryMatch(ContactInfoForLinking contactBeingSaved, out ContactInfoForLinkingFromDirectory contactFromDirectory)
		{
			contactFromDirectory = null;
			if (contactBeingSaved.IsDL)
			{
				ContactLink.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutomaticLink.TryFindDirectoryMatch: skipping linking {0} with directory because it is a PDL.", contactBeingSaved.DisplayName);
				return false;
			}
			if (contactBeingSaved.GALLinkState != GALLinkState.NotLinked)
			{
				ContactLink.Tracer.TraceDebug<string, GALLinkState>((long)this.GetHashCode(), "AutomaticLink.TryFindDirectoryMatch: skipping linking {0} with directory because GALLinkState={1}", contactBeingSaved.DisplayName, contactBeingSaved.GALLinkState);
				return false;
			}
			if (!this.directoryPersonSearcher.TryFind(contactBeingSaved, out contactFromDirectory))
			{
				ContactLink.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutomaticLink.TryFindDirectoryMatch: did not find person in directory to match with contact {0}", contactBeingSaved.DisplayName);
				return false;
			}
			ContactLink.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutomaticLink.TryFindDirectoryMatch: Found person in directory to match with contact {0}", contactBeingSaved.DisplayName);
			return true;
		}

		private HashSet<ContactInfoForLinking> LinkWithContacts(ContactInfoForLinking contactBeingSaved, IEnumerable<ContactInfoForLinking> otherContacts, int maxLinkCount, out bool linkedToDirectory)
		{
			linkedToDirectory = false;
			HashSet<ContactInfoForLinking> hashSet = new HashSet<ContactInfoForLinking>(ContactInfoForLinkingComparerByItemId.Instance);
			PersonId personId = contactBeingSaved.PersonId;
			HashSet<PersonId> hashSet2 = new HashSet<PersonId>();
			ContactInfoForLinkingFromDirectory contactInfoForLinkingFromDirectory = null;
			if (contactBeingSaved.GALLinkState != GALLinkState.NotAllowed && contactBeingSaved.GALLinkID == null)
			{
				this.TryFindDirectoryMatch(contactBeingSaved, out contactInfoForLinkingFromDirectory);
			}
			foreach (ContactInfoForLinking contactInfoForLinking in otherContacts)
			{
				if (!this.CanContinueProcessingContacts())
				{
					break;
				}
				if (!contactInfoForLinking.PersonId.Equals(personId) && !ContactInfoForLinkingComparerByItemId.Instance.Equals(contactInfoForLinking, contactBeingSaved) && !hashSet2.Contains(contactInfoForLinking.PersonId))
				{
					ContactLinkingOperation contactLinkingOperation = AutomaticLinkCriteria.CanLink(contactBeingSaved, contactInfoForLinking);
					if (contactInfoForLinkingFromDirectory != null && contactInfoForLinking.GALLinkState == GALLinkState.Linked && contactInfoForLinking.GALLinkID == contactInfoForLinkingFromDirectory.GALLinkID)
					{
						if (contactLinkingOperation == ContactLinkingOperation.None)
						{
							contactLinkingOperation = ContactLinkingOperation.AutoLinkViaGalLinkId;
						}
						contactInfoForLinkingFromDirectory = null;
					}
					if (AutomaticLink.ShouldLink(contactLinkingOperation))
					{
						hashSet2.Add(contactInfoForLinking.PersonId);
						HashSet<ContactInfoForLinking> hashSet3 = new HashSet<ContactInfoForLinking>(this.contactStoreForContactLinking.GetPersonContacts(contactInfoForLinking.PersonId));
						if (hashSet3.Count + hashSet.Count <= maxLinkCount)
						{
							hashSet.UnionWith(hashSet3);
							this.LinkTwoContacts(contactLinkingOperation, contactBeingSaved, contactInfoForLinking);
							if (hashSet.Count == maxLinkCount)
							{
								string text = string.Format(CultureInfo.InvariantCulture, "Stopping linking iteration for PersonID {0}  because we have reached maximum number of contacts for the persona.", new object[]
								{
									contactBeingSaved.PersonId
								});
								ContactLink.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutomaticLinking.LinkWithContacts: {0}", text);
								base.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.Warning>
								{
									{
										ContactLinkingLogSchema.Warning.Context,
										text
									}
								});
								break;
							}
						}
						else
						{
							ContactLink.Tracer.TraceDebug<PersonId, VersionedId>((long)this.GetHashCode(), "AutomaticLinking.LinkWithContacts: Skipping link with PersonID {0} for item {1} because we would be over the maximum number of contacts for the persona.", contactInfoForLinking.PersonId, contactBeingSaved.ItemId);
							this.LogSkippedContactLinking(contactBeingSaved, hashSet.Count, maxLinkCount, contactInfoForLinking, hashSet3.Count);
						}
					}
				}
			}
			if (contactInfoForLinkingFromDirectory != null && contactBeingSaved.GALLinkState != GALLinkState.Linked)
			{
				this.LogGALLinkOperation(contactBeingSaved, contactInfoForLinkingFromDirectory);
				contactBeingSaved.SetGALLink(contactInfoForLinkingFromDirectory);
				linkedToDirectory = true;
			}
			return hashSet;
		}

		private bool CanContinueProcessingContacts()
		{
			if (this.processingDueTime != null)
			{
				bool flag = this.processingDueTime.Value < ExDateTime.UtcNow;
				if (flag)
				{
					ContactLink.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "AutomaticLink::CanContinueProcessingContacts. Operation aborted due to time out. Mailbox: {0}", base.MailboxInfo.MailboxGuid);
				}
				return !flag;
			}
			return true;
		}

		private bool IsValidContactBeingSaved(ICoreItem contactBeingSaved)
		{
			Util.ThrowOnNullArgument(contactBeingSaved, "contactBeingSaved");
			StoreId parentFolder = contactBeingSaved.PropertyBag.GetValueOrDefault<StoreId>(StoreObjectSchema.ParentItemId, null);
			if (parentFolder == null)
			{
				ContactLink.Tracer.TraceDebug<StoreId>((long)this.GetHashCode(), "AutomaticLinking.IsValidContactBeingSaved: ignoring item because we are unable to retrieve ParentItemId from the contact.", parentFolder);
				return false;
			}
			if (!Array.Exists<StoreId>(this.contactStoreForContactLinking.FolderScope, (StoreId folderId) => parentFolder.Equals(folderId)))
			{
				ContactLink.Tracer.TraceDebug<StoreId>((long)this.GetHashCode(), "AutomaticLinking.IsValidContactBeingSaved: ignoring item because its folder is not under MyContactsExtended: {0}.", parentFolder);
				return false;
			}
			return true;
		}

		private void LinkTwoContacts(ContactLinkingOperation operation, ContactInfoForLinking contact1, ContactInfoForLinking contact2)
		{
			if (!contact1.Linked && contact2.Linked)
			{
				this.LogLinkOperation(operation, contact1, contact2);
				this.UpdatePersonaId(contact1, contact2.PersonId);
				contact1.Linked = true;
			}
			else if (contact1.Linked && !contact2.Linked)
			{
				this.LogLinkOperation(operation, contact2, contact1);
				this.UpdatePersonaId(contact2, contact1.PersonId);
				contact2.Linked = true;
			}
			else if (contact1.Linked && contact2.Linked)
			{
				this.LogLinkOperation(operation, contact2, contact1);
				this.UpdatePersonaId(contact2, contact1.PersonId);
			}
			else
			{
				if (!contact2.IsNew && contact1.IsNew)
				{
					this.LogLinkOperation(operation, contact1, contact2);
					this.UpdatePersonaId(contact1, contact2.PersonId);
				}
				else
				{
					this.LogLinkOperation(operation, contact2, contact1);
					this.UpdatePersonaId(contact2, contact1.PersonId);
				}
				contact1.Linked = true;
				contact2.Linked = true;
			}
			contact1.LinkRejectHistory.UnionWith(contact2.LinkRejectHistory);
			contact2.LinkRejectHistory = contact1.LinkRejectHistory;
			AutomaticLink.MergeGALLinkState(contact1, contact2);
		}

		private void UpdatePersonaId(ContactInfoForLinking contactToUpdate, PersonId newPersonId)
		{
			PersonId personId = contactToUpdate.PersonId;
			contactToUpdate.PersonId = newPersonId;
			this.contactStoreForContactLinking.ContactRemovedFromPerson(personId, contactToUpdate);
			this.contactStoreForContactLinking.ContactAddedToPerson(newPersonId, contactToUpdate);
		}

		private void LogLinkOperation(ContactLinkingOperation operation, ContactInfoForLinking contactToUpdate, ContactInfoForLinking otherContact)
		{
			base.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.ContactLinking>
			{
				{
					ContactLinkingLogSchema.ContactLinking.LinkOperation,
					operation
				},
				{
					ContactLinkingLogSchema.ContactLinking.LinkingPersonId,
					contactToUpdate.PersonId
				},
				{
					ContactLinkingLogSchema.ContactLinking.LinkingItemId,
					contactToUpdate.ItemId
				},
				{
					ContactLinkingLogSchema.ContactLinking.LinkToPersonId,
					otherContact.PersonId
				},
				{
					ContactLinkingLogSchema.ContactLinking.LinkToItemId,
					otherContact.ItemId
				}
			});
		}

		private void LogGALLinkOperation(ContactInfoForLinking contactToUpdate, ContactInfoForLinkingFromDirectory galContact)
		{
			base.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.ContactLinking>
			{
				{
					ContactLinkingLogSchema.ContactLinking.LinkOperation,
					ContactLinkingOperation.AutoLinkViaEmailOrImAddressInDirectoryPerson
				},
				{
					ContactLinkingLogSchema.ContactLinking.LinkingPersonId,
					contactToUpdate.PersonId
				},
				{
					ContactLinkingLogSchema.ContactLinking.LinkingItemId,
					contactToUpdate.ItemId
				},
				{
					ContactLinkingLogSchema.ContactLinking.LinkToPersonId,
					galContact.GALLinkID
				}
			});
		}

		private void LogSkippedContactLinking(ContactInfoForLinking contactBeingSaved, int currentCountOfContactsAdded, int maximumNumberOfContactsToAdd, ContactInfoForLinking matchedContact, int otherPersonaContactCount)
		{
			base.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.SkippedContactLink>
			{
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkingPersonId,
					contactBeingSaved.PersonId
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkingItemId,
					contactBeingSaved.ItemId
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkToPersonId,
					matchedContact.PersonId
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkToItemId,
					matchedContact.ItemId
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkToPersonContactCount,
					otherPersonaContactCount
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.CurrentCountOfContactsAdded,
					currentCountOfContactsAdded
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.MaximumContactsAllowedToAdd,
					maximumNumberOfContactsToAdd
				}
			});
		}

		private void LogSkippedContactLinkingWithDirectory(ContactInfoForLinking contactBeingSaved, int currentCountOfContactsAdded, int maximumNumberOfContactsToAdd)
		{
			base.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.SkippedContactLink>
			{
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkingPersonId,
					contactBeingSaved.PersonId
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkingItemId,
					contactBeingSaved.ItemId
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.LinkToPersonId,
					"N/A - Directory"
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.CurrentCountOfContactsAdded,
					currentCountOfContactsAdded
				},
				{
					ContactLinkingLogSchema.SkippedContactLink.MaximumContactsAllowedToAdd,
					maximumNumberOfContactsToAdd
				}
			});
		}

		private const int RetriesForRefreshDefaultFolder = 2;

		internal static readonly IntAppSettingsEntry MaximumNumberOfContactsPerPerson = new IntAppSettingsEntry("MaximumNumberOfContactsPerPerson", 30, ExTraceGlobals.ContactLinkingTracer);

		internal static readonly IntAppSettingsEntry MaximumNumberOfContactsToProcess = new IntAppSettingsEntry("MaximumNumberOfContactsToProcessesForLinking", 5000, ExTraceGlobals.ContactLinkingTracer);

		private static readonly TimeSpanAppSettingsEntry AllocatedTimeSlotForLinkingOnMigration = new TimeSpanAppSettingsEntry("AllocatedTimeSlotForLinkingOnMigration", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(5.0), ExTraceGlobals.ContactLinkingTracer);

		private readonly IDirectoryPersonSearcher directoryPersonSearcher;

		private readonly IContactStoreForContactLinking contactStoreForContactLinking;

		private ExDateTime? processingDueTime;
	}
}
