using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDataProvider : MailboxStoreDataProvider
	{
		private MailboxDataStore OpenMailboxStore()
		{
			return new MailboxDataStore(base.ADUser);
		}

		public static ADUser GetDiscoveryMailbox(IRecipientSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			IRecipientSession recipientSession = session;
			if (recipientSession.ConfigScope != ConfigScopes.TenantLocal)
			{
				recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(session.DomainController, true, ConsistencyMode.PartiallyConsistent, session.NetworkCredential, session.SessionSettings, 104, "GetDiscoveryMailbox", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Search\\MailboxSearch\\MailboxDataProvider.cs");
				recipientSession.UseGlobalCatalog = true;
				recipientSession.EnforceDefaultScope = session.EnforceDefaultScope;
			}
			ADRecipient[] array = recipientSession.Find(null, QueryScope.SubTree, MailboxDataProvider.DiscoverySystemMailboxFilter, null, 2);
			switch (array.Length)
			{
			case 0:
				throw new ObjectNotFoundException(ServerStrings.DiscoveryMailboxNotFound);
			case 1:
				return array[0] as ADUser;
			default:
				throw new NonUniqueRecipientException(array[0], new NonUniqueAddressError(ServerStrings.DiscoveryMailboxIsNotUnique(array[0].Id.ToString(), array[1].Id.ToString()), array[0].Id, "DiscoveryMailbox"));
			}
		}

		public static ADUser GetDiscoveryUserMailbox(IRecipientSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			IRecipientSession recipientSession = session;
			if (recipientSession.ConfigScope != ConfigScopes.TenantLocal)
			{
				recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(session.DomainController, true, ConsistencyMode.PartiallyConsistent, session.NetworkCredential, session.SessionSettings, 156, "GetDiscoveryUserMailbox", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Search\\MailboxSearch\\MailboxDataProvider.cs");
				recipientSession.UseGlobalCatalog = true;
				recipientSession.EnforceDefaultScope = session.EnforceDefaultScope;
			}
			ADPagedReader<ADRecipient> adpagedReader = recipientSession.FindPaged(null, QueryScope.SubTree, MailboxDataProvider.DiscoveryUserMailboxFilter, null, 10);
			foreach (ADRecipient adrecipient in adpagedReader)
			{
				ADUser aduser = (ADUser)adrecipient;
				ADUser aduser2 = aduser;
				if (ExchangePrincipal.FromADUser(recipientSession.SessionSettings, aduser2, RemotingOptions.AllowCrossSite).MailboxInfo.Location.ServerVersion >= Server.E14SP1MinVersion)
				{
					return aduser2;
				}
			}
			throw new ObjectNotFoundException(ServerStrings.UserDiscoveryMailboxNotFound);
		}

		public static void IncrementDiscoveryCopyItemsRatePerfCounter(int numberOfItems)
		{
			NamedPropMap.GetPerfCounters().DiscoveryCopyItemsRate.IncrementBy((long)numberOfItems);
		}

		public static void IncrementDiscoveryMailboxSearchQueuePerfCounters()
		{
			NamedPropMap.GetPerfCounters().DiscoveryMailboxSearchesQueued.Increment();
		}

		public static void DecrementDiscoveryMailboxSearchQueuePerfCounters()
		{
			NamedPropMap.GetPerfCounters().DiscoveryMailboxSearchesQueued.Decrement();
		}

		public static void IncrementDiscoveryMailboxSearchPerfCounters(int numberOfMailboxes)
		{
			NamedPropMap.GetPerfCounters().DiscoveryMailboxSearchesActive.Increment();
			NamedPropMap.GetPerfCounters().DiscoveryMailboxSearchSourceMailboxesActive.IncrementBy((long)numberOfMailboxes);
		}

		public static void DecrementDiscoveryMailboxSearchPerfCounters(int numberOfMailboxes)
		{
			NamedPropMap.GetPerfCounters().DiscoveryMailboxSearchesActive.Decrement();
			NamedPropMap.GetPerfCounters().DiscoveryMailboxSearchSourceMailboxesActive.IncrementBy((long)(-1 * numberOfMailboxes));
		}

		public IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
		}

		public MailboxDataProvider(ADUser adUser, IRecipientSession recipientSession) : base(adUser)
		{
			this.recipientSession = recipientSession;
		}

		public MailboxDataProvider(ADUser adUser) : this(adUser, null)
		{
		}

		public MailboxDataProvider(IRecipientSession recipientSession) : this(MailboxDataProvider.GetDiscoveryMailbox(recipientSession), recipientSession)
		{
		}

		private void Save(SearchObjectBase instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			MailboxDataProvider.Tracer.TraceDebug<SearchObjectId>((long)this.GetHashCode(), "Saving search object {0}", instance.Id);
			if (instance.ObjectState == ObjectState.Deleted)
			{
				throw new InvalidOperationException("Calling Save() on a deleted object is not permitted. Delete() should be used instead.");
			}
			if (instance.ObjectState == ObjectState.Unchanged)
			{
				return;
			}
			bool flag = false;
			if (instance.ObjectState == ObjectState.New && (instance.Id == null || instance.Id.IsEmpty))
			{
				instance.SetId(base.ADUser.Id, Guid.NewGuid());
				flag = true;
			}
			ValidationError[] array = instance.Validate();
			if (array.Length > 0)
			{
				throw new DataValidationException(array[0]);
			}
			using (MailboxDataStore mailboxDataStore = this.OpenMailboxStore())
			{
				if (flag)
				{
					while (mailboxDataStore.Exists(instance.Id))
					{
						instance.SetId(base.ADUser.Id, Guid.NewGuid());
					}
				}
				instance.OnSaving();
				mailboxDataStore.Save(instance);
			}
			instance.ResetChangeTracking(true);
			this.LogSaveEvent(instance);
		}

		private void LogSaveEvent(SearchObjectBase obj)
		{
			SearchEventLogger.PropertyLogData propertyLogData = new SearchEventLogger.PropertyLogData();
			if (base.ADUser.OrganizationId != null && base.ADUser.OrganizationId.ConfigurationUnit != null)
			{
				propertyLogData.AddOrganization(base.ADUser.OrganizationId.ConfigurationUnit.ToString());
			}
			switch (obj.ObjectType)
			{
			case ObjectType.SearchObject:
				break;
			case ObjectType.SearchStatus:
			{
				SearchStatus searchStatus = (SearchStatus)obj;
				propertyLogData.AddSearchStatus(searchStatus);
				SearchEventLogger.Instance.LogSearchStatusSavedEvent(propertyLogData);
				if (searchStatus.Errors == null)
				{
					return;
				}
				using (MultiValuedProperty<string>.Enumerator enumerator = searchStatus.Errors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string errorMsg = enumerator.Current;
						SearchEventLogger.Instance.LogSearchErrorEvent(obj.Id.ToString(), errorMsg);
					}
					return;
				}
				break;
			}
			default:
				return;
			}
			propertyLogData.AddSearchObject((SearchObject)obj);
			SearchEventLogger.Instance.LogSearchObjectSavedEvent(propertyLogData);
		}

		internal void Delete(SearchObjectBase instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			MailboxDataProvider.Tracer.TraceDebug<SearchObjectId>((long)this.GetHashCode(), "Deleting search object {0}", instance.Id);
			if (instance.ObjectState == ObjectState.Deleted)
			{
				throw new InvalidOperationException("The object has already been deleted");
			}
			using (MailboxDataStore mailboxDataStore = this.OpenMailboxStore())
			{
				mailboxDataStore.Delete(instance);
			}
			instance.MarkAsDeleted();
		}

		public bool Exists(SearchObjectId identity)
		{
			MailboxDataProvider.Tracer.TraceDebug<SearchObjectId>((long)this.GetHashCode(), "Querying existence of search object {0}", identity);
			bool result;
			using (MailboxDataStore mailboxDataStore = this.OpenMailboxStore())
			{
				result = mailboxDataStore.Exists(identity);
			}
			return result;
		}

		public bool Exists<T>(string name) where T : IConfigurable, new()
		{
			MailboxDataProvider.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Querying existence of search object {0}", name);
			bool result;
			using (MailboxDataStore mailboxDataStore = this.OpenMailboxStore())
			{
				result = mailboxDataStore.Exists<T>(name);
			}
			return result;
		}

		public override IConfigurable Read<T>(ObjectId identity)
		{
			MailboxDataProvider.Tracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "Reading search object with identity {0}", identity);
			IConfigurable result;
			using (MailboxDataStore mailboxDataStore = this.OpenMailboxStore())
			{
				result = mailboxDataStore.Read<T>(identity as SearchObjectId);
			}
			return result;
		}

		public override IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			MailboxDataProvider.Tracer.TraceDebug<QueryFilter>((long)this.GetHashCode(), "Finding search object that match filter '{0}'", filter);
			if (filter != null && !(filter is TextFilter))
			{
				throw new ArgumentException("filter");
			}
			IEnumerable<T> enumerable = null;
			using (MailboxDataStore mailboxDataStore = this.OpenMailboxStore())
			{
				enumerable = mailboxDataStore.FindPaged<T>(filter as TextFilter, 0);
			}
			LinkedList<IConfigurable> linkedList = new LinkedList<IConfigurable>();
			foreach (T t in enumerable)
			{
				IConfigurable value = t;
				linkedList.AddLast(value);
				if (linkedList.Count >= 1000)
				{
					break;
				}
			}
			IConfigurable[] array = new IConfigurable[linkedList.Count];
			linkedList.CopyTo(array, 0);
			return array;
		}

		public override IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			MailboxDataProvider.Tracer.TraceDebug<QueryFilter>((long)this.GetHashCode(), "Finding search object that match filter {0}", filter);
			if (filter != null && !(filter is TextFilter))
			{
				throw new ArgumentException("filter");
			}
			IEnumerable<T> result;
			using (MailboxDataStore mailboxDataStore = this.OpenMailboxStore())
			{
				result = mailboxDataStore.FindPaged<T>(filter as TextFilter, pageSize);
			}
			return result;
		}

		public override void Save(IConfigurable instance)
		{
			this.Save(instance as SearchObjectBase);
		}

		public override void Delete(IConfigurable instance)
		{
			this.Delete(instance as SearchObjectBase);
		}

		public const string Ediscovery = "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}";

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private readonly IRecipientSession recipientSession;

		internal static readonly QueryFilter DiscoverySystemMailboxFilter = new AndFilter(new QueryFilter[]
		{
			new TextFilter(ADObjectSchema.Name, "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}", MatchOptions.FullString, MatchFlags.IgnoreCase),
			new TextFilter(ADRecipientSchema.Alias, "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}", MatchOptions.FullString, MatchFlags.IgnoreCase),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox)
		});

		internal static readonly QueryFilter DiscoveryUserMailboxFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.DiscoveryMailbox);
	}
}
