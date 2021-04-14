using System;
using System.IO;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal class SafetyNetInfoCache
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DumpsterTracer;
			}
		}

		public SafetyNetInfoCache(string dbGuidStr, string dbName)
		{
			this.m_dbGuidStr = dbGuidStr;
			this.m_stateSafetyNetInfo = new SafetyNetRegKey(dbGuidStr, dbName);
		}

		public bool IsRedeliveryRequired(bool readThrough, bool fThrow)
		{
			SafetyNetInfoHashTable safetyNetInfosReadThrough;
			if (readThrough)
			{
				safetyNetInfosReadThrough = this.GetSafetyNetInfosReadThrough();
			}
			else
			{
				Exception safetyNetInfosCached = this.GetSafetyNetInfosCached(out safetyNetInfosReadThrough);
				if (safetyNetInfosCached != null)
				{
					if (fThrow)
					{
						throw safetyNetInfosCached;
					}
					SafetyNetInfoCache.Tracer.TraceError<string>((long)this.GetHashCode(), "IsRedeliveryRequired() for DB '{0}' returning true as a safeguard since cluster DB read failed.", this.m_dbGuidStr);
					return true;
				}
			}
			return safetyNetInfosReadThrough.RedeliveryRequired;
		}

		public string RedeliveryServers
		{
			get
			{
				string result = string.Empty;
				SafetyNetInfoHashTable safetyNetInfoHashTable;
				if (this.GetSafetyNetInfosCached(out safetyNetInfoHashTable) == null)
				{
					result = safetyNetInfoHashTable.RedeliveryServers;
				}
				return result;
			}
		}

		public DateTime RedeliveryStartTime
		{
			get
			{
				DateTime result = DateTime.MinValue;
				SafetyNetInfoHashTable safetyNetInfoHashTable;
				if (this.GetSafetyNetInfosCached(out safetyNetInfoHashTable) == null)
				{
					result = safetyNetInfoHashTable.RedeliveryStartTime;
				}
				return result;
			}
		}

		public DateTime RedeliveryEndTime
		{
			get
			{
				DateTime result = DateTime.MinValue;
				SafetyNetInfoHashTable safetyNetInfoHashTable;
				if (this.GetSafetyNetInfosCached(out safetyNetInfoHashTable) == null)
				{
					result = safetyNetInfoHashTable.RedeliveryEndTime;
				}
				return result;
			}
		}

		public Exception GetSafetyNetInfosCached(out SafetyNetInfoHashTable safetyNetInfos)
		{
			SafetyNetInfoCache.InnerTable innerTable = this.m_currentTable;
			DateTime utcNow = DateTime.UtcNow;
			Exception ex = null;
			safetyNetInfos = null;
			if (this.IsReadThroughNeeded(innerTable))
			{
				lock (this)
				{
					innerTable = this.m_currentTable;
					if (this.IsReadThroughNeeded(innerTable))
					{
						try
						{
							innerTable = this.ReadNewTable();
							this.m_currentTable = innerTable;
							this.m_lastTableLoadException = null;
						}
						catch (DumpsterRedeliveryException ex2)
						{
							ex = ex2;
						}
						catch (IOException ex3)
						{
							ex = ex3;
						}
						catch (ClusterException ex4)
						{
							ex = ex4;
						}
						if (ex != null)
						{
							this.m_lastTableLoadException = ex;
							SafetyNetInfoCache.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "GetSafetyNetInfosCached() for '{0}' failed to load the new table: {1}", this.m_dbGuidStr, this.m_lastTableLoadException);
						}
					}
				}
			}
			if (innerTable != null)
			{
				safetyNetInfos = innerTable.Table;
			}
			return ex;
		}

		public SafetyNetInfoHashTable GetSafetyNetInfosReadThrough()
		{
			SafetyNetInfoHashTable table;
			lock (this)
			{
				this.m_currentTable = this.ReadNewTable();
				table = this.m_currentTable.Table;
			}
			return table;
		}

		public void Reset(SafetyNetInfo safetyNetInfo)
		{
			lock (this)
			{
				safetyNetInfo.RedeliveryRequired = false;
				this.Update(safetyNetInfo);
			}
		}

		public void Update(SafetyNetInfo safetyNetInfo)
		{
			lock (this)
			{
				if (safetyNetInfo.IsModified())
				{
					this.m_stateSafetyNetInfo.WriteRequestInfo(safetyNetInfo);
					safetyNetInfo.ClearModified();
					this.m_currentTable = null;
				}
				else
				{
					SafetyNetInfoCache.Tracer.TraceDebug<string, SafetyNetInfo>((long)this.GetHashCode(), "Update() for DB '{0}' skipping ClusDB update since redelivery request has not been modified. SafetyNetInfo = {1}", this.m_dbGuidStr, safetyNetInfo);
				}
			}
		}

		public void Delete(SafetyNetInfo safetyNetInfo)
		{
			lock (this)
			{
				this.m_stateSafetyNetInfo.DeleteRequest(safetyNetInfo);
				this.m_currentTable = null;
			}
		}

		private bool IsReadThroughNeeded(SafetyNetInfoCache.InnerTable cachedTable)
		{
			return cachedTable == null || cachedTable.CreateTimeUtc < DateTime.UtcNow - SafetyNetInfoCache.CacheTTL;
		}

		private SafetyNetInfoCache.InnerTable ReadNewTable()
		{
			SafetyNetInfoCache.InnerTable innerTable = new SafetyNetInfoCache.InnerTable();
			innerTable.Table = new SafetyNetInfoHashTable();
			foreach (SafetyNetRequestKey safetyNetRequestKey in this.m_stateSafetyNetInfo.ReadRequestKeys())
			{
				SafetyNetInfo prevInfo = null;
				if (this.m_currentTable != null)
				{
					this.m_currentTable.Table.TryGetValue(safetyNetRequestKey, out prevInfo);
				}
				SafetyNetInfo safetyNetInfo = this.m_stateSafetyNetInfo.ReadRequestInfo(safetyNetRequestKey, prevInfo);
				if (safetyNetInfo != null)
				{
					innerTable.Table.Add(safetyNetRequestKey, safetyNetInfo);
				}
			}
			innerTable.CreateTimeUtc = DateTime.UtcNow;
			return innerTable;
		}

		private static readonly TimeSpan CacheTTL = TimeSpan.FromSeconds((double)RegistryParameters.DumpsterInfoCacheTTLInSec);

		private readonly string m_dbGuidStr;

		private SafetyNetRegKey m_stateSafetyNetInfo;

		private SafetyNetInfoCache.InnerTable m_currentTable;

		private Exception m_lastTableLoadException;

		private class InnerTable
		{
			public SafetyNetInfoHashTable Table;

			public DateTime CreateTimeUtc;
		}
	}
}
