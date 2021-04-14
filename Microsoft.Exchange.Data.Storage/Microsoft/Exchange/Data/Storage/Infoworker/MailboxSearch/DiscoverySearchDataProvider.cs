using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DiscoverySearchDataProvider : TenantStoreDataProvider, IDiscoverySearchDataProvider
	{
		static DiscoverySearchDataProvider()
		{
			DiscoverySearchDataProvider.minimalProperties = (from SimplePropertyDefinition x in new MailboxDiscoverySearch().ObjectSchema.AllProperties
			where (x.PropertyDefinitionFlags & PropertyDefinitionFlags.ReturnOnBind) != PropertyDefinitionFlags.ReturnOnBind && (!(x is EwsStoreObjectPropertyDefinition) || ((EwsStoreObjectPropertyDefinition)x).StorePropertyDefinition != ItemSchema.Attachments)
			select x).ToArray<SimplePropertyDefinition>();
		}

		public DiscoverySearchDataProvider(OrganizationId organizationId) : base(organizationId)
		{
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return base.Mailbox.MailboxInfo.OrganizationId;
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				return base.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		public string DisplayName
		{
			get
			{
				return base.Mailbox.MailboxInfo.DisplayName;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return base.Mailbox.ObjectId.DistinguishedName;
			}
		}

		public string LegacyDistinguishedName
		{
			get
			{
				return base.Mailbox.LegacyDn;
			}
		}

		public Guid ObjectGuid
		{
			get
			{
				return base.Mailbox.ObjectId.ObjectGuid;
			}
		}

		public IEnumerable<T> GetAll<T>() where T : DiscoverySearchBase, new()
		{
			return base.InternalFindPaged<T>(null, null, false, this.sortBy, 1000, new ProviderPropertyDefinition[0]);
		}

		public MailboxDiscoverySearch FindByInPlaceHoldIdentity(string inPlaceHoldIdentity)
		{
			Util.ThrowOnNullOrEmptyArgument(inPlaceHoldIdentity, "inPlaceHoldIdentity");
			SearchFilter filter = new SearchFilter.IsEqualTo(MailboxDiscoverySearchSchema.InPlaceHoldIdentity.StorePropertyDefinition, inPlaceHoldIdentity);
			IEnumerable<MailboxDiscoverySearch> source = this.InternalFindPaged<MailboxDiscoverySearch>(filter, null, false, null, 1, new ProviderPropertyDefinition[0]);
			return source.FirstOrDefault<MailboxDiscoverySearch>();
		}

		public MailboxDiscoverySearch FindByLegacySearchObjectIdentity(string legacySearchObjectIdentity)
		{
			Util.ThrowOnNullOrEmptyArgument(legacySearchObjectIdentity, "legacySearchObjectIdentity");
			SearchFilter filter = new SearchFilter.IsEqualTo(MailboxDiscoverySearchSchema.LegacySearchObjectIdentity.StorePropertyDefinition, legacySearchObjectIdentity);
			IEnumerable<MailboxDiscoverySearch> source = this.InternalFindPaged<MailboxDiscoverySearch>(filter, null, false, null, 1, new ProviderPropertyDefinition[0]);
			return source.FirstOrDefault<MailboxDiscoverySearch>();
		}

		public T Find<T>(string searchId) where T : DiscoverySearchBase, new()
		{
			Util.ThrowOnNullOrEmptyArgument(searchId, "searchId");
			return this.FindByAlternativeId<T>(searchId);
		}

		public override T FindByAlternativeId<T>(string alternativeId)
		{
			if (string.IsNullOrEmpty(alternativeId))
			{
				throw new ArgumentNullException("alternativeId");
			}
			SearchFilter filter = new SearchFilter.IsEqualTo(EwsStoreObjectSchema.AlternativeId.StorePropertyDefinition, alternativeId);
			IEnumerable<T> source = this.InternalFindPaged<T>(filter, null, false, null, 1, new ProviderPropertyDefinition[0]);
			return source.FirstOrDefault<T>();
		}

		public void CreateOrUpdate<T>(T discoverySearch) where T : DiscoverySearchBase
		{
			Util.ThrowOnNullArgument(discoverySearch, "discoverySearch");
			this.Save(discoverySearch);
		}

		public void Delete<T>(string searchId) where T : DiscoverySearchBase, new()
		{
			Util.ThrowOnNullOrEmptyArgument(searchId, "searchId");
			T t = this.Find<T>(searchId);
			if (t == null)
			{
				throw new StoragePermanentException(ServerStrings.MailboxSearchObjectNotExist(searchId));
			}
			base.Delete(t);
		}

		protected override IEnumerable<T> InternalFindPaged<T>(SearchFilter filter, FolderId rootId, bool deepSearch, SortBy[] sortBy, int pageSize, params ProviderPropertyDefinition[] properties)
		{
			Stopwatch stopwatch = new Stopwatch();
			IEnumerable<T> result;
			try
			{
				if (filter == null && typeof(T) == typeof(MailboxDiscoverySearch))
				{
					properties = DiscoverySearchDataProvider.minimalProperties;
					deepSearch = false;
					pageSize = 1000;
				}
				stopwatch.Start();
				result = base.InternalFindPaged<T>(filter, rootId, deepSearch, sortBy, pageSize, properties).ToList<T>();
			}
			finally
			{
				stopwatch.Stop();
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			}
			return result;
		}

		protected override void OnExchangeServiceCreated(ExchangeService service)
		{
			service.UserAgent = string.Format("Exchange\\{0}\\EDiscovery\\EWS\\SID={1}&S=CFG&BI=0&R=0&RT={2}", DiscoverySearchDataProvider.version, Guid.NewGuid(), DateTime.UtcNow.Ticks);
			base.OnExchangeServiceCreated(service);
		}

		protected override FolderId GetDefaultFolder()
		{
			if (this.mailboxSearchFolderId == null)
			{
				this.mailboxSearchFolderId = base.GetOrCreateFolder("Discovery", new FolderId(10, new Mailbox(base.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString()))).Id;
			}
			return this.mailboxSearchFolderId;
		}

		internal const string MailboxSearchFolderName = "Discovery";

		private static ProviderPropertyDefinition[] minimalProperties = null;

		private static string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

		private readonly SortBy[] sortBy = new SortBy[]
		{
			new SortBy(MailboxDiscoverySearchSchema.LastModifiedTime, SortOrder.Descending)
		};

		private FolderId mailboxSearchFolderId;
	}
}
