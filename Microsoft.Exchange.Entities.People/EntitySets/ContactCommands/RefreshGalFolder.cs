using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.Entities.People.Converters;
using Microsoft.Exchange.Entities.People.DataProviders;
using Microsoft.Exchange.Entities.People.EntitySets.ResponseTypes;
using Microsoft.Exchange.Entities.People.Utilities;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.People.EntitySets.ContactCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RefreshGalFolder : Command<RefreshGALFolderResponseEntity>, IStorageEntitySetScope<IMailboxSession>
	{
		public RefreshGalFolder(IMailboxSession mailboxSession, IRecipientSession recipientSession, ITracer tracer, IPerformanceDataLogger perfLogger, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("perfLogger", perfLogger);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			this.tracer = tracer;
			this.perfLogger = perfLogger;
			this.tracingId = this.GetHashCode();
			this.StoreSession = mailboxSession;
			this.RecipientSession = recipientSession;
			this.XsoFactory = xsoFactory;
			this.IdConverter = IdConverter.Instance;
			this.recipientCacheContactsDataProvider = new ContactDataProvider(this, this.Trace);
			this.recipientCacheContactsDataProvider.FolderInScope = DefaultFolderType.RecipientCache;
			this.recipientCacheContactsDataProvider.ContactProperties = GALContactsFolderSchema.ContactPropertyDefinitions;
			this.contactDataProvider = new ADContactDataProvider(recipientSession, this.Trace);
			this.exceptionList = new List<Exception>();
		}

		public IMailboxSession StoreSession { get; private set; }

		public IXSOFactory XsoFactory { get; private set; }

		public IdConverter IdConverter { get; private set; }

		public IRecipientSession RecipientSession { get; private set; }

		protected override ITracer Trace
		{
			get
			{
				return this.tracer;
			}
		}

		public new RefreshGALFolderResponseEntity Execute(CommandContext context)
		{
			return base.Execute(context);
		}

		protected override RefreshGALFolderResponseEntity OnExecute()
		{
			using (new StopwatchPerformanceTracker("RefreshGALFolder", this.perfLogger))
			{
				this.InPlaceUpdateRecipientCacheFolder();
			}
			if (this.exceptionList.Count > 0)
			{
				throw new AggregateException(this.exceptionList);
			}
			return new RefreshGALFolderResponseEntity();
		}

		private void InPlaceUpdateRecipientCacheFolder()
		{
			IEnumerable<IStorePropertyBag> allContacts = this.recipientCacheContactsDataProvider.GetAllContacts(RefreshGalFolder.ContactSortColumns);
			if (allContacts == null || !allContacts.Any<IStorePropertyBag>())
			{
				this.Trace.TraceDebug((long)this.tracingId, "RefreshGALContactsFolderCommand.InitialLoad: No contact found from Recipient cache folder, nothing to load in GAL folder, exiting");
				return;
			}
			IEnumerable<IStorePropertyBag> contactsFromAd = this.GetContactsFromAd(allContacts);
			if (contactsFromAd == null)
			{
				this.Trace.TraceDebug((long)this.tracingId, "RefreshGALContactsFolderCommand.InitialLoad: No contact found from Directory - no data to load, exiting");
				return;
			}
			this.PopulatePersonInfo(contactsFromAd, allContacts);
			this.DiffAndUpdateFolder(allContacts, contactsFromAd);
		}

		private void PopulatePersonInfo(IEnumerable<IStorePropertyBag> collectionToLoadInformation, IEnumerable<IStorePropertyBag> collectionWithInformation)
		{
			ListDiffCalculator<IStorePropertyBag, PropertyDefinition> listDiffCalculator = new ListDiffCalculator<IStorePropertyBag, PropertyDefinition>(StoreContactEmailAddressComparer.Instance, new ContactDeltaCalculator(RefreshGalFolder.RecipientCacheSpecificProperties));
			DiffResult<IStorePropertyBag, PropertyDefinition> diffResult = listDiffCalculator.DiffUnSortedLists(collectionToLoadInformation.ToList<IStorePropertyBag>(), collectionWithInformation.ToList<IStorePropertyBag>());
			if (diffResult.UpdateList.Count == 0)
			{
				return;
			}
			foreach (IStorePropertyBag storePropertyBag in diffResult.UpdateList.Keys)
			{
				foreach (Tuple<PropertyDefinition, object> tuple in diffResult.UpdateList[storePropertyBag])
				{
					storePropertyBag[tuple.Item1] = tuple.Item2;
				}
			}
		}

		private void AddContacts(ICollection<IStorePropertyBag> addCollection)
		{
			if (addCollection.Count == 0)
			{
				this.Trace.TraceDebug((long)this.GetHashCode(), "RefreshGalContactsFolderCommand.AddContacts - addList.Count is 0, nothing to add.");
				return;
			}
			foreach (IStorePropertyBag storePropertyBag in addCollection)
			{
				try
				{
					ConflictResolutionResult conflictResolutionResult = this.recipientCacheContactsDataProvider.AddContact(storePropertyBag);
					if (conflictResolutionResult.SaveStatus != SaveResult.Success)
					{
						this.Trace.TraceError<IStorePropertyBag, ConflictResolutionResult>((long)this.GetHashCode(), "Adding contact failed for contact {0}. ConflictResolutionResult = {1}.", storePropertyBag, conflictResolutionResult);
						this.AddToExceptionList(new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(storePropertyBag), conflictResolutionResult));
					}
				}
				catch (TransientException ex)
				{
					this.Trace.TraceError<IStorePropertyBag, TransientException>((long)this.GetHashCode(), "Adding contact failed for contact {0}. Exception = {1}.", storePropertyBag, ex);
					this.AddToExceptionList(ex);
				}
				catch (StoragePermanentException ex2)
				{
					this.Trace.TraceError<IStorePropertyBag, StoragePermanentException>((long)this.GetHashCode(), "Adding contact failed for contact {0}. Exception = {1}.", storePropertyBag, ex2);
					this.AddToExceptionList(ex2);
				}
			}
		}

		private void UpdateContacts(Dictionary<IStorePropertyBag, ICollection<Tuple<PropertyDefinition, object>>> updateList)
		{
			if (updateList.Count == 0)
			{
				this.Trace.TraceDebug((long)this.GetHashCode(), "RefreshGalContactsFolderCommand.UpdateContacts - updateList.Count is 0, nothing to updated.");
				return;
			}
			foreach (IStorePropertyBag storePropertyBag in updateList.Keys)
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				if (valueOrDefault == null)
				{
					this.Trace.TraceDebug<IStorePropertyBag>((long)this.GetHashCode(), "RefreshGalContactsFolderCommand.UpdateContacts - VersionedId is null for this contact {0}, thus skipping it.", storePropertyBag);
				}
				else
				{
					try
					{
						ConflictResolutionResult conflictResolutionResult = this.recipientCacheContactsDataProvider.UpdateContact(valueOrDefault, updateList[storePropertyBag]);
						if (conflictResolutionResult.SaveStatus != SaveResult.Success)
						{
							this.Trace.TraceError<IStorePropertyBag, ConflictResolutionResult>((long)this.GetHashCode(), "Updating contact failed for contact {0}. ConflictResolutionResult = {1}.", storePropertyBag, conflictResolutionResult);
							this.AddToExceptionList(new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(storePropertyBag), conflictResolutionResult));
						}
					}
					catch (TransientException ex)
					{
						this.Trace.TraceError<IStorePropertyBag, TransientException>((long)this.GetHashCode(), "Updating contact failed for contact {0}. Exception = {1}.", storePropertyBag, ex);
						this.AddToExceptionList(ex);
					}
					catch (StoragePermanentException ex2)
					{
						this.Trace.TraceError<IStorePropertyBag, StoragePermanentException>((long)this.GetHashCode(), "Updating contact failed for contact {0}. Exception = {1}.", storePropertyBag, ex2);
						this.AddToExceptionList(ex2);
					}
				}
			}
		}

		private IEnumerable<IStorePropertyBag> GetContactsFromAd(IEnumerable<IStorePropertyBag> contacts)
		{
			ProxyAddress[] proxyAddresses = this.GetProxyAddresses(contacts);
			ProxyAddress[] array = null;
			if (proxyAddresses.Length > 1000)
			{
				this.tracer.TraceDebug<int, int>((long)this.GetHashCode(), "RefreshGalContactsFolderCommand.GetContactsFromAd: Number of proxy addresses {0} are more than {1}. Sending for only {1} entries to AD.", proxyAddresses.Length, 1000);
				array = new ProxyAddress[1000];
				Array.Copy(proxyAddresses, array, 1000);
			}
			IEnumerable<Result<ADRawEntry>> batchADObjects = this.contactDataProvider.GetBatchADObjects(array ?? proxyAddresses);
			if (batchADObjects.Count<Result<ADRawEntry>>() == 0)
			{
				this.Trace.TraceDebug<ProxyAddress[]>((long)this.tracingId, "RefreshGALContactsFolderCommand.GetContactsFromAd: No AD object found for the given set of proxyAddresses {0}", proxyAddresses);
				return Enumerable.Empty<IStorePropertyBag>();
			}
			ADObjectToIStorePropertyConverter adobjectToIStorePropertyConverter = new ADObjectToIStorePropertyConverter(this.Trace, GALContactsFolderSchema.ContactPropertyDefinitions);
			return adobjectToIStorePropertyConverter.ConvertAdObjectsToIStorePropertyBags(batchADObjects);
		}

		private ProxyAddress[] GetProxyAddresses(IEnumerable<IStorePropertyBag> contacts)
		{
			HashSet<ProxyAddress> hashSet = new HashSet<ProxyAddress>();
			foreach (IStorePropertyBag storePropertyBag in contacts)
			{
				string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, null);
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					ProxyAddress item;
					if (ProxyAddress.TryParse(valueOrDefault, out item))
					{
						hashSet.Add(item);
					}
					else
					{
						this.Trace.TraceDebug<string>((long)this.tracingId, "RefreshGALContactsFolderCommand.GetProxyAddresses: emailAddress - {0} failed in ProxyAddress.TryParse method, ignoring this email address", valueOrDefault);
					}
				}
			}
			ProxyAddress[] array = new ProxyAddress[hashSet.Count];
			hashSet.CopyTo(array);
			return array;
		}

		private void DiffAndUpdateFolder(IEnumerable<IStorePropertyBag> sourceContacts, IEnumerable<IStorePropertyBag> targetContacts)
		{
			ArgumentValidator.ThrowIfNull("sourceContacts", sourceContacts);
			ArgumentValidator.ThrowIfNull("targetContacts", targetContacts);
			ListDiffCalculator<IStorePropertyBag, PropertyDefinition> listDiffCalculator = new ListDiffCalculator<IStorePropertyBag, PropertyDefinition>(StoreContactPersonIdComparer.Instance, new ContactDeltaCalculator(GALContactsFolderSchema.ContactPropertyDefinitions));
			DiffResult<IStorePropertyBag, PropertyDefinition> diffResult = listDiffCalculator.DiffUnSortedLists(sourceContacts.ToList<IStorePropertyBag>(), targetContacts.ToList<IStorePropertyBag>());
			this.AddContacts(diffResult.AddList);
			this.UpdateContacts(diffResult.UpdateList);
		}

		private void AddToExceptionList(Exception exception)
		{
			this.exceptionList.Add(exception);
		}

		private const int MaxEntriesForADQuery = 1000;

		private static readonly PropertyDefinition[] RecipientCacheSpecificProperties = new PropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.PersonId,
			ContactSchema.RelevanceScore
		};

		private static readonly SortBy[] ContactSortColumns = new SortBy[]
		{
			new SortBy(ContactSchema.PersonId, SortOrder.Ascending)
		};

		private readonly IPerformanceDataLogger perfLogger = NullPerformanceDataLogger.Instance;

		private readonly int tracingId;

		private readonly ITracer tracer;

		private readonly List<Exception> exceptionList;

		private readonly ContactDataProvider recipientCacheContactsDataProvider;

		private readonly ADContactDataProvider contactDataProvider;
	}
}
