using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADRecipientCache<TEntry> : IADRecipientCache, IEnumerable<KeyValuePair<ProxyAddress, Result<ADRawEntry>>>, IEnumerable, IADRecipientCache<TEntry> where TEntry : ADRawEntry, new()
	{
		public ADRecipientCache(ADPropertyDefinition[] properties) : this(properties, 0)
		{
		}

		public ADRecipientCache(ADPropertyDefinition[] properties, int capacity)
		{
			this.adSessionLock = new object();
			this.orgId = OrganizationId.ForestWideOrgId;
			this.dictionaryLock = new object();
			base..ctor();
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] == null)
				{
					throw new ArgumentException("Null property definition is not allowed!", "properties");
				}
			}
			if (typeof(TEntry) != typeof(ADRawEntry) && typeof(TEntry) != typeof(TransportMiniRecipient))
			{
				throw new ArgumentException("This class can only be used with ADRawEntry or TransportMiniRecipient generic parameter values.");
			}
			TEntry tentry = Activator.CreateInstance<TEntry>();
			this.isFullADRecipientObject = (tentry is ADRecipient);
			this.dictionary = new Dictionary<ProxyAddress, Result<TEntry>>(capacity);
			this.properties = properties;
			this.propertyCollection = new ReadOnlyCollection<ADPropertyDefinition>(this.properties);
		}

		public ADRecipientCache(IRecipientSession adSession, ADPropertyDefinition[] properties, int capacity) : this(properties, capacity)
		{
			this.adSession = adSession;
			if (adSession != null)
			{
				this.orgId = null;
			}
		}

		public ADRecipientCache(IRecipientSession adSession, ADPropertyDefinition[] properties, int capacity, OrganizationId orgId) : this(properties, capacity)
		{
			if (adSession == null)
			{
				throw new ArgumentNullException("adSession");
			}
			this.adSession = adSession;
			this.orgId = orgId;
		}

		public ADRecipientCache(ADPropertyDefinition[] properties, int capacity, OrganizationId orgId) : this(properties, capacity)
		{
			if (!OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				this.orgId = orgId;
			}
		}

		protected ADRecipientCache()
		{
			this.adSessionLock = new object();
			this.orgId = OrganizationId.ForestWideOrgId;
			this.dictionaryLock = new object();
			base..ctor();
		}

		public virtual OrganizationId OrganizationId
		{
			get
			{
				return this.orgId;
			}
		}

		public virtual IRecipientSession ADSession
		{
			get
			{
				if (this.adSession == null && this.orgId != null)
				{
					lock (this.adSessionLock)
					{
						if (this.adSession == null)
						{
							this.adSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.orgId), 532, "ADSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADRecipientCache.cs");
						}
					}
				}
				if (this.adSession != null)
				{
					return this.adSession;
				}
				if (ADRecipientCache<ADRawEntry>.defaultADSession == null)
				{
					ADRecipientCache<TEntry>.defaultADSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId), 548, "ADSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADRecipientCache.cs");
				}
				return ADRecipientCache<TEntry>.defaultADSession;
			}
		}

		public virtual ReadOnlyCollection<ADPropertyDefinition> CachedADProperties
		{
			get
			{
				return this.propertyCollection;
			}
		}

		public virtual int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public virtual ICollection<ProxyAddress> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public virtual ICollection<ProxyAddress> ClonedKeys
		{
			get
			{
				ICollection<ProxyAddress> result;
				lock (this.dictionaryLock)
				{
					result = this.dictionary.Keys.ToArray<ProxyAddress>();
				}
				return result;
			}
		}

		public virtual ICollection<Result<TEntry>> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		IEnumerable<Result<ADRawEntry>> IADRecipientCache.Values
		{
			get
			{
				foreach (Result<TEntry> value in this.dictionary.Values)
				{
					Result<TEntry> result = value;
					ADRawEntry data = result.Data;
					Result<TEntry> result2 = value;
					yield return new Result<ADRawEntry>(data, result2.Error);
				}
				yield break;
			}
		}

		private static MSExchangeADRecipientCacheInstance PerfCounters
		{
			get
			{
				return ADRecipientCache<TEntry>.perfCounters;
			}
		}

		public static void InitializePerfCounters(string instanceName)
		{
			ADRecipientCache<TEntry>.perfCounters = MSExchangeADRecipientCache.GetInstance(instanceName);
			ADRecipientCache<TEntry>.numberOfQueriesPercentileCounter = new AggregatingPercentileCounter(1L, 100L);
		}

		public void Clear()
		{
			lock (this.dictionaryLock)
			{
				this.dictionary = new Dictionary<ProxyAddress, Result<TEntry>>();
			}
		}

		public virtual bool ContainsKey(ProxyAddress proxyAddress)
		{
			bool result;
			lock (this.dictionaryLock)
			{
				result = this.dictionary.ContainsKey(proxyAddress);
			}
			return result;
		}

		public virtual bool CopyEntryFrom(IADRecipientCache<TEntry> cacheToCopyFrom, string smtpAddress)
		{
			return SmtpAddress.IsValidSmtpAddress(smtpAddress) && this.CopyEntryFrom(cacheToCopyFrom, new SmtpProxyAddress(smtpAddress, true));
		}

		public virtual bool CopyEntryFrom(IADRecipientCache<TEntry> cacheToCopyFrom, ProxyAddress proxyAddress)
		{
			if (cacheToCopyFrom == null)
			{
				throw new ArgumentNullException("cacheToCopyFrom");
			}
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			if (!this.CachedADProperties.SequenceEqual(cacheToCopyFrom.CachedADProperties))
			{
				return false;
			}
			Result<TEntry> result;
			if (cacheToCopyFrom.TryGetValue(proxyAddress, out result))
			{
				this.AddCacheEntry(proxyAddress, new Result<TEntry>(result.Data, result.Error), false);
				return true;
			}
			return false;
		}

		public virtual Result<TEntry> FindAndCacheRecipient(ProxyAddress proxyAddress)
		{
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			Result<TEntry> result;
			bool flag2;
			lock (this.dictionaryLock)
			{
				flag2 = this.dictionary.TryGetValue(proxyAddress, out result);
			}
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.AggregateHits_Base.Increment();
			}
			if (!flag2)
			{
				if (ADRecipientCache<TEntry>.PerfCounters != null)
				{
					ADRecipientCache<TEntry>.PerfCounters.AggregateMisses.Increment();
				}
				result = this.LookUpRecipientInAD(proxyAddress, this.properties);
				lock (this.dictionaryLock)
				{
					Result<TEntry> result2;
					if (this.dictionary.TryGetValue(proxyAddress, out result2))
					{
						if (ADRecipientCache<TEntry>.PerfCounters != null)
						{
							ADRecipientCache<TEntry>.PerfCounters.RepeatedQueryForTheSameRecipient.Increment();
						}
						return result2;
					}
					this.AddCacheEntry(proxyAddress, result, false, true);
				}
				return result;
			}
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.AggregateHits.Increment();
			}
			return result;
		}

		public virtual Result<TEntry> FindAndCacheRecipient(ADObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.IndividualAddressLookupsTotal.Increment();
				ADRecipientCache<TEntry>.PerfCounters.RequestsPendingTotal.Increment();
				ADRecipientCache<TEntry>.PerfCounters.AggregateHits_Base.Increment();
				ADRecipientCache<TEntry>.PerfCounters.AggregateMisses.Increment();
				ADRecipientCache<TEntry>.PerfCounters.AggregateLookupsTotal.Increment();
				this.IncrementQueriesPerCacheCounter();
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			Result<TEntry> result;
			try
			{
				TEntry entry = default(TEntry);
				ADNotificationAdapter.RunADOperation(delegate()
				{
					if (typeof(TEntry) == typeof(TransportMiniRecipient))
					{
						entry = (this.ADSession.ReadMiniRecipient<TransportMiniRecipient>(objectId, this.properties) as TEntry);
						return;
					}
					if (this.isFullADRecipientObject)
					{
						entry = (TEntry)((object)this.ADSession.Read(objectId));
						return;
					}
					entry = (TEntry)((object)this.ADSession.ReadADRawEntry(objectId, this.properties));
				});
				if (entry == null)
				{
					result = new Result<TEntry>(default(TEntry), ProviderError.NotFound);
				}
				else
				{
					result = new Result<TEntry>(entry, null);
				}
			}
			catch (DataValidationException ex)
			{
				ComponentTrace<ADRecipientCacheTags>.TraceError<DataValidationException>(0, -1L, "DataValidationException: {0}", ex);
				result = new Result<TEntry>(default(TEntry), ex.Error);
			}
			finally
			{
				stopwatch.Stop();
				if (ADRecipientCache<TEntry>.PerfCounters != null)
				{
					ADRecipientCache<TEntry>.PerfCounters.AverageLookupQueryLatency.IncrementBy(stopwatch.ElapsedMilliseconds);
				}
				ADRecipientCache<TEntry>.DecrementPendingRequestsCounter();
			}
			if (result.Data != null)
			{
				ProxyAddress primarySmtpAddress = ADRecipientCache<TEntry>.GetPrimarySmtpAddress(result.Data);
				if (primarySmtpAddress != null)
				{
					this.AddCacheEntry(primarySmtpAddress, result);
				}
			}
			return result;
		}

		public virtual IList<Result<TEntry>> FindAndCacheRecipients(IList<ProxyAddress> proxyAddressList)
		{
			if (proxyAddressList == null)
			{
				throw new ArgumentNullException("proxyAddressList");
			}
			List<Result<TEntry>> list = new List<Result<TEntry>>(proxyAddressList.Count);
			List<ProxyAddress> proxies = new List<ProxyAddress>(proxyAddressList.Count);
			List<int> list2 = new List<int>(proxyAddressList.Count);
			lock (this.dictionaryLock)
			{
				int num = 0;
				foreach (ProxyAddress proxyAddress in proxyAddressList)
				{
					Result<TEntry> item = new Result<TEntry>(default(TEntry), null);
					if (null == proxyAddress || this.dictionary.TryGetValue(proxyAddress, out item))
					{
						list.Add(item);
					}
					else
					{
						list.Add(item);
						proxies.Add(proxyAddress);
						list2.Add(num);
					}
					num++;
				}
			}
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.AggregateHits_Base.Increment();
				if (list2.Count == 0)
				{
					ADRecipientCache<TEntry>.PerfCounters.AggregateHits.Increment();
				}
				else
				{
					int num2 = list2.Count / ADRecipientCache<TEntry>.BatchSize;
					if (list2.Count % ADRecipientCache<TEntry>.BatchSize != 0)
					{
						num2++;
					}
					ADRecipientCache<TEntry>.PerfCounters.AggregateMisses.IncrementBy((long)num2);
				}
			}
			if (proxies.Count == 0)
			{
				return list;
			}
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.BatchedAddressLookupsTotal.Increment();
				ADRecipientCache<TEntry>.PerfCounters.RequestsPendingTotal.Increment();
				ADRecipientCache<TEntry>.PerfCounters.AggregateLookupsTotal.Increment();
				this.IncrementQueriesPerCacheCounter();
			}
			Result<TEntry>[] rawResults = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				int i;
				ADNotificationAdapter.RunADOperation(delegate()
				{
					if (typeof(TEntry) == typeof(ADRawEntry))
					{
						Result<ADRawEntry>[] source = this.ADSession.FindByProxyAddresses(proxies.ToArray(), this.properties);
						rawResults = (from i in source
						select new Result<TEntry>((TEntry)((object)i.Data), i.Error)).ToArray<Result<TEntry>>();
						return;
					}
					if (typeof(TEntry) == typeof(TransportMiniRecipient))
					{
						Result<TransportMiniRecipient>[] source2 = this.ADSession.FindByProxyAddresses<TransportMiniRecipient>(proxies.ToArray());
						rawResults = (from i in source2
						select new Result<TEntry>(i.Data as TEntry, i.Error)).ToArray<Result<TEntry>>();
						return;
					}
					throw new NotSupportedException();
				});
			}
			finally
			{
				stopwatch.Stop();
				if (ADRecipientCache<TEntry>.PerfCounters != null)
				{
					ADRecipientCache<TEntry>.PerfCounters.AverageLookupQueryLatency.IncrementBy(stopwatch.ElapsedMilliseconds);
				}
				ADRecipientCache<TEntry>.DecrementPendingRequestsCounter();
			}
			for (int i = 0; i < rawResults.Length; i++)
			{
				if (rawResults[i].Data != null)
				{
					this.PopulateCalculatedProperties(rawResults[i].Data);
				}
				list[list2[i]] = rawResults[i];
			}
			bool flag2 = false;
			lock (this.dictionaryLock)
			{
				for (int j = 0; j < proxies.Count; j++)
				{
					Result<TEntry> value;
					if (proxies[j] != null && this.dictionary.TryGetValue(proxies[j], out value))
					{
						list[list2[j]] = value;
						flag2 = true;
					}
					else
					{
						this.AddCacheEntry(proxies[j], list[list2[j]], false, false);
					}
				}
			}
			if (flag2 && ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.RepeatedQueryForTheSameRecipient.Increment();
			}
			return list;
		}

		public Dictionary<ProxyAddress, Result<TEntry>>.Enumerator GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		public virtual bool Remove(ProxyAddress proxyAddress)
		{
			bool result2;
			lock (this.dictionaryLock)
			{
				Result<TEntry> result;
				if (this.dictionary.TryGetValue(proxyAddress, out result))
				{
					this.dictionary.Remove(proxyAddress);
					if (result.Data == null)
					{
						result2 = true;
					}
					else
					{
						if (ADRecipientCache<TEntry>.IsExAddress(proxyAddress))
						{
							this.dictionary.Remove(ADRecipientCache<TEntry>.GetPrimarySmtpAddress(result.Data));
						}
						else if (ADRecipientCache<TEntry>.IsSmtpAddress(proxyAddress))
						{
							this.dictionary.Remove(ProxyAddress.Parse(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, ADRecipientCache<TEntry>.GetLegacyExchangeDN(result.Data)));
						}
						result2 = true;
					}
				}
				else
				{
					result2 = false;
				}
			}
			return result2;
		}

		public virtual bool TryGetValue(ProxyAddress proxyAddress, out Result<TEntry> result)
		{
			bool result2;
			lock (this.dictionaryLock)
			{
				result2 = this.dictionary.TryGetValue(proxyAddress, out result);
			}
			return result2;
		}

		public virtual void AddCacheEntry(ProxyAddress proxyAddress, Result<TEntry> result)
		{
			this.AddCacheEntry(proxyAddress, result, true, true);
		}

		public virtual IEnumerable<TRecipient> ExpandGroup<TRecipient>(IADDistributionList group) where TRecipient : MiniRecipient, new()
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.ExpandGroupRequestsTotal.Increment();
				ADRecipientCache<TEntry>.PerfCounters.RequestsPendingTotal.Increment();
			}
			try
			{
				foreach (TRecipient entry in group.Expand<TRecipient>(1000, this.properties))
				{
					yield return entry;
				}
			}
			finally
			{
				ADRecipientCache<TEntry>.DecrementPendingRequestsCounter();
			}
			yield break;
		}

		public virtual Result<TEntry> ReadSecurityDescriptor(ProxyAddress proxyAddress)
		{
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			Result<TEntry> result = this.FindAndCacheRecipient(proxyAddress);
			if (result.Data != null)
			{
				IDirectorySession adsession = this.ADSession;
				TEntry data = result.Data;
				RawSecurityDescriptor rawSecurityDescriptor = adsession.ReadSecurityDescriptor(data.Id);
				lock (result.Data)
				{
					result.Data.propertyBag.SetField(ADObjectSchema.NTSecurityDescriptor, SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor));
				}
			}
			return result;
		}

		public virtual void DropSecurityDescriptor(ProxyAddress proxyAddress)
		{
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			Result<TEntry> result;
			if (this.TryGetValue(proxyAddress, out result))
			{
				lock (result.Data)
				{
					result.Data.propertyBag.SetField(ADObjectSchema.NTSecurityDescriptor, null);
				}
			}
		}

		public virtual Result<TEntry> ReloadRecipient(ProxyAddress proxyAddress, IEnumerable<ADPropertyDefinition> extraProperties)
		{
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			if (extraProperties == null)
			{
				throw new ArgumentNullException("extraProperties");
			}
			Result<TEntry> result = this.FindAndCacheRecipient(proxyAddress);
			if (result.Data != null)
			{
				IDirectorySession adsession = this.ADSession;
				TEntry data = result.Data;
				ADRawEntry adrawEntry = adsession.ReadADRawEntry(data.Id, extraProperties);
				lock (result.Data)
				{
					foreach (ADPropertyDefinition adpropertyDefinition in extraProperties)
					{
						result.Data.propertyBag.SetField(adpropertyDefinition, adrawEntry[adpropertyDefinition]);
					}
				}
			}
			return result;
		}

		Result<ADRawEntry> IADRecipientCache.FindAndCacheRecipient(ADObjectId objectId)
		{
			Result<TEntry> result = this.FindAndCacheRecipient(objectId);
			return new Result<ADRawEntry>(result.Data, result.Error);
		}

		Result<ADRawEntry> IADRecipientCache.FindAndCacheRecipient(ProxyAddress proxyAddress)
		{
			Result<TEntry> result = this.FindAndCacheRecipient(proxyAddress);
			return new Result<ADRawEntry>(result.Data, result.Error);
		}

		IList<Result<ADRawEntry>> IADRecipientCache.FindAndCacheRecipients(IList<ProxyAddress> proxyAddressList)
		{
			IList<Result<TEntry>> source = this.FindAndCacheRecipients(proxyAddressList);
			return (from i in source
			select new Result<ADRawEntry>(i.Data, i.Error)).ToList<Result<ADRawEntry>>();
		}

		void IADRecipientCache.AddCacheEntry(ProxyAddress proxyAddress, Result<ADRawEntry> result)
		{
			this.AddCacheEntry(proxyAddress, new Result<TEntry>((TEntry)((object)result.Data), result.Error));
		}

		bool IADRecipientCache.CopyEntryFrom(IADRecipientCache cacheToCopyFrom, string smtpAddress)
		{
			return SmtpAddress.IsValidSmtpAddress(smtpAddress) && ((IADRecipientCache)this).CopyEntryFrom(cacheToCopyFrom, new SmtpProxyAddress(smtpAddress, true));
		}

		bool IADRecipientCache.CopyEntryFrom(IADRecipientCache cacheToCopyFrom, ProxyAddress proxyAddress)
		{
			if (cacheToCopyFrom == null)
			{
				throw new ArgumentNullException("cacheToCopyFrom");
			}
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			if (!this.CachedADProperties.SequenceEqual(cacheToCopyFrom.CachedADProperties))
			{
				return false;
			}
			Result<ADRawEntry> result;
			if (cacheToCopyFrom.TryGetValue(proxyAddress, out result))
			{
				this.AddCacheEntry(proxyAddress, new Result<TEntry>((TEntry)((object)result.Data), result.Error), false);
				return true;
			}
			return false;
		}

		Result<ADRawEntry> IADRecipientCache.ReloadRecipient(ProxyAddress proxyAddress, IEnumerable<ADPropertyDefinition> extraProperties)
		{
			Result<TEntry> result = this.ReloadRecipient(proxyAddress, extraProperties);
			return new Result<ADRawEntry>(result.Data, result.Error);
		}

		Result<ADRawEntry> IADRecipientCache.ReadSecurityDescriptor(ProxyAddress proxyAddress)
		{
			Result<TEntry> result = this.ReadSecurityDescriptor(proxyAddress);
			return new Result<ADRawEntry>(result.Data, result.Error);
		}

		bool IADRecipientCache.TryGetValue(ProxyAddress proxyAddress, out Result<ADRawEntry> result)
		{
			Result<TEntry> result2;
			if (this.TryGetValue(proxyAddress, out result2))
			{
				result = new Result<ADRawEntry>(result2.Data, result2.Error);
				return true;
			}
			result = new Result<ADRawEntry>(null, null);
			return false;
		}

		private static bool IsSmtpAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.Smtp);
		}

		private static bool IsExAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.LegacyDN);
		}

		private static ProxyAddress GetPrimarySmtpAddress(TEntry entry)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)entry[ADRecipientSchema.EmailAddresses];
			return proxyAddressCollection.FindPrimary(ProxyAddressPrefix.Smtp);
		}

		private static string GetLegacyExchangeDN(TEntry entry)
		{
			return (string)entry[ADRecipientSchema.LegacyExchangeDN];
		}

		private static void DecrementPendingRequestsCounter()
		{
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.RequestsPendingTotal.Decrement();
			}
		}

		private Result<TEntry> LookUpRecipientInAD(ProxyAddress proxyAddress, ADPropertyDefinition[] properties)
		{
			if (ADRecipientCache<TEntry>.PerfCounters != null)
			{
				ADRecipientCache<TEntry>.PerfCounters.IndividualAddressLookupsTotal.Increment();
				ADRecipientCache<TEntry>.PerfCounters.RequestsPendingTotal.Increment();
				ADRecipientCache<TEntry>.PerfCounters.AggregateLookupsTotal.Increment();
				this.IncrementQueriesPerCacheCounter();
			}
			ComponentTrace<ADRecipientCacheTags>.TraceDebug<ProxyAddress>(0, -1L, "Lookup recipient {0}", proxyAddress);
			TEntry entry = default(TEntry);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					if (typeof(TEntry) == typeof(ADRawEntry))
					{
						entry = (TEntry)((object)this.ADSession.FindByProxyAddress(proxyAddress, properties));
						return;
					}
					if (typeof(TEntry) == typeof(TransportMiniRecipient))
					{
						entry = (this.ADSession.FindByProxyAddress<TransportMiniRecipient>(proxyAddress) as TEntry);
						return;
					}
					throw new NotSupportedException();
				});
				if (entry == null)
				{
					return new Result<TEntry>(default(TEntry), ProviderError.NotFound);
				}
			}
			catch (DataValidationException ex)
			{
				ComponentTrace<ADRecipientCacheTags>.TraceError<DataValidationException>(0, -1L, "DataValidationException: {0}", ex);
				return new Result<TEntry>(default(TEntry), ex.Error);
			}
			finally
			{
				stopwatch.Stop();
				if (ADRecipientCache<TEntry>.PerfCounters != null)
				{
					ADRecipientCache<TEntry>.PerfCounters.AverageLookupQueryLatency.IncrementBy(stopwatch.ElapsedMilliseconds);
				}
				ADRecipientCache<TEntry>.DecrementPendingRequestsCounter();
			}
			return new Result<TEntry>(entry, null);
		}

		private void PopulateCalculatedProperties(ADRawEntry entry)
		{
			MiniRecipient miniRecipient = entry as MiniRecipient;
			bool flag = false;
			foreach (ADPropertyDefinition adpropertyDefinition in this.CachedADProperties)
			{
				if (adpropertyDefinition.IsCalculated)
				{
					if (miniRecipient == null || miniRecipient.HasSupportingProperties(adpropertyDefinition))
					{
						object obj = entry[adpropertyDefinition];
					}
					else
					{
						flag = true;
						ComponentTrace<ADRecipientCacheTags>.TraceInformation<string>(0, (long)this.GetHashCode(), "After lookup, supporting properties are missing for the calculated property: {0}.", adpropertyDefinition.Name);
					}
				}
			}
			if (flag)
			{
				ComponentTrace<ADRecipientCacheTags>.TraceWarning<string>(0, (long)this.GetHashCode(), "Supporting properties were missing for the type: {0}.", entry.GetType().Name);
			}
		}

		private void AddCacheEntry(ProxyAddress proxyAddress, Result<TEntry> result, bool populateCalculatedProperties)
		{
			this.AddCacheEntry(proxyAddress, result, true, populateCalculatedProperties);
		}

		private void AddCacheEntry(ProxyAddress proxyAddress, Result<TEntry> result, bool isLockRequired, bool populateCalculatedProperties)
		{
			TEntry data = result.Data;
			if (populateCalculatedProperties && data != null)
			{
				this.PopulateCalculatedProperties(data);
			}
			this.SetEntry(proxyAddress, result, isLockRequired);
			if (data == null)
			{
				return;
			}
			ProxyAddress primarySmtpAddress = ADRecipientCache<TEntry>.GetPrimarySmtpAddress(data);
			if (ADRecipientCache<TEntry>.IsSmtpAddress(proxyAddress))
			{
				ProxyAddress proxyAddress2 = ProxyAddress.Parse(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, ADRecipientCache<TEntry>.GetLegacyExchangeDN(data));
				this.SetEntry(proxyAddress2, result, isLockRequired);
				if (null != primarySmtpAddress && primarySmtpAddress != proxyAddress)
				{
					this.SetEntry(primarySmtpAddress, result, isLockRequired);
					return;
				}
			}
			else if (primarySmtpAddress != null)
			{
				this.SetEntry(primarySmtpAddress, result, isLockRequired);
			}
		}

		private void SetEntry(ProxyAddress proxyAddress, Result<TEntry> result, bool isLockRequired)
		{
			if (isLockRequired)
			{
				lock (this.dictionaryLock)
				{
					this.AddRecipientCacheEntry(result, proxyAddress);
					return;
				}
			}
			this.AddRecipientCacheEntry(result, proxyAddress);
		}

		private void AddRecipientCacheEntry(Result<TEntry> result, ProxyAddress proxyAddress)
		{
			if (this.dictionary.ContainsKey(proxyAddress))
			{
				return;
			}
			this.dictionary[proxyAddress] = result;
		}

		private void IncrementQueriesPerCacheCounter()
		{
			ADRecipientCache<TEntry>.numberOfQueriesPercentileCounter.IncrementValue(ref this.numberOfLookups, 1L);
			ADRecipientCache<TEntry>.PerfCounters.NumberOfQueriesPerRecipientCache50Percentile.RawValue = ADRecipientCache<TEntry>.numberOfQueriesPercentileCounter.PercentileQuery(50.0);
			ADRecipientCache<TEntry>.PerfCounters.NumberOfQueriesPerRecipientCache80Percentile.RawValue = ADRecipientCache<TEntry>.numberOfQueriesPercentileCounter.PercentileQuery(80.0);
			ADRecipientCache<TEntry>.PerfCounters.NumberOfQueriesPerRecipientCache95Percentile.RawValue = ADRecipientCache<TEntry>.numberOfQueriesPercentileCounter.PercentileQuery(95.0);
			ADRecipientCache<TEntry>.PerfCounters.NumberOfQueriesPerRecipientCache99Percentile.RawValue = ADRecipientCache<TEntry>.numberOfQueriesPercentileCounter.PercentileQuery(99.0);
		}

		IEnumerator<KeyValuePair<ProxyAddress, Result<ADRawEntry>>> IEnumerable<KeyValuePair<ProxyAddress, Result<ADRawEntry>>>.GetEnumerator()
		{
			foreach (KeyValuePair<ProxyAddress, Result<TEntry>> pair in this.dictionary)
			{
				KeyValuePair<ProxyAddress, Result<TEntry>> keyValuePair = pair;
				ProxyAddress key = keyValuePair.Key;
				KeyValuePair<ProxyAddress, Result<TEntry>> keyValuePair2 = pair;
				ADRawEntry data = keyValuePair2.Value.Data;
				KeyValuePair<ProxyAddress, Result<TEntry>> keyValuePair3 = pair;
				yield return new KeyValuePair<ProxyAddress, Result<ADRawEntry>>(key, new Result<ADRawEntry>(data, keyValuePair3.Value.Error));
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private const int PageSize = 1000;

		public static readonly int BatchSize = ADRecipientObjectSession.ReadMultipleMaxBatchSize;

		private readonly ADPropertyDefinition[] properties;

		private readonly ReadOnlyCollection<ADPropertyDefinition> propertyCollection;

		private IRecipientSession adSession;

		private readonly object adSessionLock;

		private readonly OrganizationId orgId;

		private static MSExchangeADRecipientCacheInstance perfCounters = null;

		private static AggregatingPercentileCounter numberOfQueriesPercentileCounter = null;

		private static IRecipientSession defaultADSession;

		private Dictionary<ProxyAddress, Result<TEntry>> dictionary;

		private readonly object dictionaryLock;

		private long numberOfLookups;

		private readonly bool isFullADRecipientObject;
	}
}
