using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	internal class JobCache : IDiagnosable
	{
		internal JobCache(AnchorContext anchorContext)
		{
			this.Context = anchorContext;
			this.sharedDataLock = new object();
			this.cacheEntries = new Dictionary<ADObjectId, CacheEntryBase>(32);
			this.cacheUpdated = new AutoResetEvent(false);
		}

		internal AutoResetEvent CacheUpdated
		{
			get
			{
				return this.cacheUpdated;
			}
		}

		private AnchorContext Context { get; set; }

		public string GetDiagnosticComponentName()
		{
			return "JobCache";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xml = null;
			AnchorUtil.RunOperationWithCulture(CultureInfo.InvariantCulture, delegate
			{
				xml = this.InternalGetDiagnosticInfo(parameters.Argument);
			});
			return xml;
		}

		internal bool TryAdd(ADUser user, bool wakeCache)
		{
			AnchorUtil.ThrowOnNullArgument(user, "user");
			return this.TryUpdate(this.Context.CreateCacheEntry(user), wakeCache);
		}

		internal bool TryUpdate(CacheEntryBase cacheEntry, bool wakeCache)
		{
			AnchorUtil.ThrowOnNullArgument(cacheEntry, "cacheEntry");
			bool flag = cacheEntry.Validate();
			if (this.Contains(cacheEntry.ObjectId))
			{
				if (!flag)
				{
					lock (this.sharedDataLock)
					{
						this.Context.Logger.Log(MigrationEventType.Warning, "Removing CacheEntry {0} because it's invalid ...", new object[]
						{
							cacheEntry
						});
						this.cacheEntries.Remove(cacheEntry.ObjectId);
						goto IL_9E;
					}
				}
				this.Context.Logger.Log(MigrationEventType.Verbose, "CacheEntry {0} already exists in cache, skipping add", new object[]
				{
					cacheEntry
				});
				return true;
			}
			IL_9E:
			if (!flag)
			{
				return false;
			}
			lock (this.sharedDataLock)
			{
				this.cacheEntries[cacheEntry.ObjectId] = cacheEntry;
			}
			if (wakeCache && this.Context.Config.GetConfig<bool>("ShouldWakeOnCacheUpdate"))
			{
				this.Context.Logger.Log(MigrationEventType.Information, "triggering cache update", new object[0]);
				this.cacheUpdated.Set();
			}
			return true;
		}

		internal void Remove(CacheEntryBase cacheEntry)
		{
			AnchorUtil.ThrowOnNullArgument(cacheEntry, "cacheEntry");
			this.Context.Logger.Log(MigrationEventType.Warning, "Deactivating CacheEntry {0} ...", new object[]
			{
				cacheEntry
			});
			cacheEntry.Deactivate();
			lock (this.sharedDataLock)
			{
				this.cacheEntries.Remove(cacheEntry.ObjectId);
			}
		}

		internal void Clear()
		{
			this.Context.Logger.Log(MigrationEventType.Warning, "Clearing cache", new object[0]);
			lock (this.sharedDataLock)
			{
				this.cacheEntries.Clear();
			}
		}

		internal List<CacheEntryBase> Get()
		{
			List<CacheEntryBase> list;
			lock (this.sharedDataLock)
			{
				list = new List<CacheEntryBase>(this.cacheEntries.Count);
				foreach (KeyValuePair<ADObjectId, CacheEntryBase> keyValuePair in this.cacheEntries)
				{
					list.Add(keyValuePair.Value);
				}
			}
			return list;
		}

		internal bool Contains(ADObjectId objectId)
		{
			bool result;
			lock (this.sharedDataLock)
			{
				result = this.cacheEntries.ContainsKey(objectId);
			}
			return result;
		}

		private XElement InternalGetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			ICollection<CacheEntryBase> collection = this.Get();
			XElement xelement2 = new XElement("CacheEntryCollection", new XElement("count", collection.Count));
			xelement.Add(xelement2);
			foreach (CacheEntryBase cacheEntryBase in collection)
			{
				XElement diagnosticInfo = cacheEntryBase.GetDiagnosticInfo(argument);
				xelement2.Add(diagnosticInfo);
			}
			return xelement;
		}

		private bool TryGet(ADObjectId objectId, out CacheEntryBase cacheEntry)
		{
			bool result;
			lock (this.sharedDataLock)
			{
				result = this.cacheEntries.TryGetValue(objectId, out cacheEntry);
			}
			return result;
		}

		public const int ExpectedNumMbxsPerDatabase = 2;

		public const int ExpectedNumDBsPerServer = 16;

		private readonly AutoResetEvent cacheUpdated;

		private readonly object sharedDataLock;

		private Dictionary<ADObjectId, CacheEntryBase> cacheEntries;
	}
}
