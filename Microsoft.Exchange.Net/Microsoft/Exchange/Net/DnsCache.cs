using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal class DnsCache
	{
		public DnsCache() : this(10000)
		{
		}

		public DnsCache(int cacheSize)
		{
			this.cacheSize = cacheSize;
			this.data = new Dictionary<DnsQuery, DnsResult>(cacheSize);
		}

		public int MaxCacheSize
		{
			get
			{
				return this.cacheSize;
			}
		}

		public int Count
		{
			get
			{
				int count;
				try
				{
					this.syncRoot.EnterReadLock();
					count = this.data.Count;
				}
				finally
				{
					try
					{
						this.syncRoot.ExitReadLock();
					}
					catch (ApplicationException)
					{
					}
				}
				return count;
			}
		}

		public static DnsCache CreateFromSystem()
		{
			return DnsCache.CreateFromFile(DnsCache.HostsFile);
		}

		public static DnsCache CreateFromFile(string path)
		{
			DnsCache dnsCache = new DnsCache();
			dnsCache.UpdateFromFile(path);
			return dnsCache;
		}

		public DnsResult Find(DnsQuery query)
		{
			try
			{
				this.syncRoot.EnterReadLock();
				DnsResult dnsResult;
				if (this.data.TryGetValue(query, out dnsResult))
				{
					dnsResult.UpdateLastAccess();
					return dnsResult;
				}
			}
			finally
			{
				try
				{
					this.syncRoot.ExitReadLock();
				}
				catch (ApplicationException)
				{
				}
			}
			return null;
		}

		public void Add(DnsQuery query, DnsResult results)
		{
			try
			{
				this.syncRoot.EnterWriteLock();
				this.data[query] = results;
				if (this.data.Count >= this.MaxCacheSize + this.hostsFileEntries)
				{
					this.ExpireCache(10);
				}
			}
			finally
			{
				try
				{
					this.syncRoot.ExitWriteLock();
				}
				catch (ApplicationException)
				{
				}
			}
		}

		public void FlushCache()
		{
			try
			{
				this.syncRoot.EnterWriteLock();
				this.ExpireCache(100);
			}
			finally
			{
				try
				{
					this.syncRoot.ExitWriteLock();
				}
				catch (ApplicationException)
				{
				}
			}
			this.quadATimeOuts.Clear();
		}

		public void UpdateFromFile(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			this.pathToHostsFile = path;
			string directoryName = Path.GetDirectoryName(path);
			string fileName = Path.GetFileName(path);
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(directoryName, fileName);
			fileSystemWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime);
			fileSystemWatcher.Deleted += this.UpdateCache;
			fileSystemWatcher.Changed += this.UpdateCache;
			fileSystemWatcher.Created += this.UpdateCache;
			fileSystemWatcher.Renamed += new RenamedEventHandler(this.UpdateCache);
			fileSystemWatcher.EnableRaisingEvents = true;
			fileSystemWatcher = Interlocked.Exchange<FileSystemWatcher>(ref this.fileSystemWatcher, fileSystemWatcher);
			if (fileSystemWatcher != null)
			{
				fileSystemWatcher.EnableRaisingEvents = false;
				fileSystemWatcher.Dispose();
			}
			this.UpdateCache(null, null);
		}

		public void Close()
		{
			FileSystemWatcher fileSystemWatcher = Interlocked.Exchange<FileSystemWatcher>(ref this.fileSystemWatcher, null);
			if (fileSystemWatcher != null)
			{
				fileSystemWatcher.EnableRaisingEvents = false;
				fileSystemWatcher.Dispose();
			}
		}

		private void UpdateCache(object sender, FileSystemEventArgs eventArgs)
		{
			int num = 3;
			bool flag;
			do
			{
				flag = false;
				num--;
				try
				{
					this.syncRoot.EnterWriteLock();
					this.hostsFileEntries = 0;
					this.data.Clear();
					this.LoadPermanentEntries();
					Stream stream = new FileStream(this.pathToHostsFile, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete, 4096, FileOptions.SequentialScan);
					using (StreamReader streamReader = new StreamReader(stream, Encoding.ASCII, false))
					{
						this.LoadFromStream(streamReader);
						this.hostsFileEntries = this.data.Count;
					}
				}
				catch (FileNotFoundException arg)
				{
					ExTraceGlobals.DNSTracer.TraceError<string, FileNotFoundException>(0L, "DNS address cache failed to find file \"{0}\". {1}", this.pathToHostsFile, arg);
				}
				catch (DirectoryNotFoundException arg2)
				{
					ExTraceGlobals.DNSTracer.TraceError<string, DirectoryNotFoundException>(0L, "DNS address cache failed to find directory \"{0}\". {1}", this.pathToHostsFile, arg2);
				}
				catch (IOException arg3)
				{
					ExTraceGlobals.DNSTracer.TraceError<string, IOException>(0L, "DNS address cache failed to read file \"{0}\". {1}", this.pathToHostsFile, arg3);
					flag = true;
				}
				catch (UnauthorizedAccessException arg4)
				{
					ExTraceGlobals.DNSTracer.TraceError<string, UnauthorizedAccessException>(0L, "DNS address cache was not authorized to access file \"{0}\". {1}", this.pathToHostsFile, arg4);
				}
				finally
				{
					try
					{
						this.syncRoot.ExitWriteLock();
					}
					catch (ApplicationException)
					{
					}
				}
				if (num > 0 && flag)
				{
					Thread.Sleep(1000);
				}
			}
			while (num > 0 && flag);
			if (flag)
			{
				ExTraceGlobals.DNSTracer.TraceError<string>(0L, "DNS address cache failed to read file \"{0}\". Retry Count exceeded.", this.pathToHostsFile);
			}
		}

		private void ExpireCache(int purgePercentage)
		{
			ExTraceGlobals.DNSTracer.TraceDebug((long)this.GetHashCode(), "Expiring cache");
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan t = TimeSpan.FromDays(365.0);
			List<DnsCache.DnsExpireEntry> list = new List<DnsCache.DnsExpireEntry>(this.data.Count);
			foreach (KeyValuePair<DnsQuery, DnsResult> keyValuePair in this.data)
			{
				if (!keyValuePair.Value.IsPermanentEntry)
				{
					DateTime dateTime = keyValuePair.Value.LastAccess;
					if (keyValuePair.Value.Expires < utcNow)
					{
						dateTime -= t;
					}
					DnsCache.DnsExpireEntry dnsExpireEntry = new DnsCache.DnsExpireEntry(keyValuePair.Key, dateTime);
					int num = list.BinarySearch(dnsExpireEntry, dnsExpireEntry);
					if (num < 0)
					{
						num = ~num;
					}
					list.Insert(num, dnsExpireEntry);
				}
			}
			ExTraceGlobals.DNSTracer.TraceDebug<int>((long)this.GetHashCode(), "Expiration list contains {0} candidates", list.Count);
			int num2 = this.data.Count * purgePercentage / 100;
			foreach (DnsCache.DnsExpireEntry dnsExpireEntry2 in list)
			{
				if (num2-- <= 0)
				{
					break;
				}
				ExTraceGlobals.DNSTracer.TraceDebug<DnsQuery, DateTime>((long)this.GetHashCode(), "Purging {0}, lastAccess {1}", dnsExpireEntry2.Query, dnsExpireEntry2.LastAccessDate);
				this.data.Remove(dnsExpireEntry2.Query);
			}
		}

		private void LoadFromStream(StreamReader sr)
		{
			string text;
			while ((text = sr.ReadLine()) != null)
			{
				text = text.TrimStart(null);
				if (!string.IsNullOrEmpty(text) && text[0] != '#')
				{
					int num = text.IndexOf('#');
					if (num != -1)
					{
						text = text.Substring(0, num);
					}
					string[] array = text.Split(DnsCache.Dividers, StringSplitOptions.RemoveEmptyEntries);
					IPAddress address;
					if (array.Length >= 2 && IPAddress.TryParse(array[0], out address))
					{
						string text2 = Dns.TrimTrailingDot(array[1]);
						this.AddAddressRecord(address, text2);
						this.AddPtrRecord(address, text2);
						for (int i = 2; i < array.Length; i++)
						{
							this.AddCnameRecord(Dns.TrimTrailingDot(array[i]), text2);
						}
					}
				}
			}
		}

		private void LoadPermanentEntries()
		{
			DnsQuery key = new DnsQuery(DnsRecordType.MX, "localhost");
			DnsResult value = new DnsResult(DnsStatus.InfoDomainNonexistent, IPAddress.None, DnsResult.NoExpiration);
			this.data[key] = value;
		}

		private void AddAddressRecord(IPAddress address, string host)
		{
			DnsQuery dnsQuery;
			DnsRecord dnsRecord;
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				dnsQuery = new DnsQuery(DnsRecordType.A, host);
				dnsRecord = new DnsARecord(dnsQuery.Question, address);
			}
			else
			{
				dnsQuery = new DnsQuery(DnsRecordType.AAAA, host);
				dnsRecord = new DnsAAAARecord(dnsQuery.Question, address);
			}
			dnsRecord.Section = DnsResponseSection.Answer;
			dnsRecord.TimeToLive = TimeSpan.MaxValue;
			DnsResult dnsResult;
			DnsRecordList dnsRecordList;
			if (this.data.TryGetValue(dnsQuery, out dnsResult))
			{
				dnsRecordList = dnsResult.List;
				using (IEnumerator<DnsRecord> enumerator = DnsRecordList.EnumerateAddresses(dnsRecordList).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DnsRecord dnsRecord2 = enumerator.Current;
						DnsAddressRecord dnsAddressRecord = (DnsAddressRecord)dnsRecord2;
						if (address.Equals(dnsAddressRecord.IPAddress))
						{
							return;
						}
					}
					goto IL_C9;
				}
			}
			dnsRecordList = new DnsRecordList();
			dnsResult = new DnsResult(DnsStatus.Success, IPAddress.None, dnsRecordList, DnsResult.NoExpiration);
			this.data[dnsQuery] = dnsResult;
			IL_C9:
			dnsRecordList.Add(dnsRecord);
		}

		private void AddPtrRecord(IPAddress address, string host)
		{
			DnsQuery dnsQuery = new DnsQuery(DnsRecordType.PTR, PtrRequest.ConstructPTRQuery(address));
			DnsResult dnsResult;
			DnsRecordList dnsRecordList;
			if (this.data.TryGetValue(dnsQuery, out dnsResult))
			{
				dnsRecordList = dnsResult.List;
				using (IEnumerator<DnsRecord> enumerator = dnsRecordList.EnumerateAnswers(DnsRecordType.PTR).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DnsRecord dnsRecord = enumerator.Current;
						DnsPtrRecord dnsPtrRecord = (DnsPtrRecord)dnsRecord;
						if (dnsPtrRecord.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
					}
					goto IL_90;
				}
			}
			dnsRecordList = new DnsRecordList();
			dnsResult = new DnsResult(DnsStatus.Success, IPAddress.None, dnsRecordList, DnsResult.NoExpiration);
			this.data[dnsQuery] = dnsResult;
			IL_90:
			dnsRecordList.Add(new DnsPtrRecord(dnsQuery.Question, host)
			{
				Section = DnsResponseSection.Answer,
				TimeToLive = TimeSpan.MaxValue
			});
		}

		private void AddCnameRecord(string host, string alias)
		{
			DnsQuery dnsQuery = new DnsQuery(DnsRecordType.CNAME, host);
			DnsResult dnsResult;
			DnsRecordList dnsRecordList;
			if (this.data.TryGetValue(dnsQuery, out dnsResult))
			{
				dnsRecordList = dnsResult.List;
				using (IEnumerator<DnsRecord> enumerator = dnsRecordList.EnumerateAnswers(DnsRecordType.PTR).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DnsRecord dnsRecord = enumerator.Current;
						DnsPtrRecord dnsPtrRecord = (DnsPtrRecord)dnsRecord;
						if (dnsPtrRecord.Host.Equals(alias, StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
					}
					goto IL_8A;
				}
			}
			dnsRecordList = new DnsRecordList();
			dnsResult = new DnsResult(DnsStatus.Success, IPAddress.None, dnsRecordList, DnsResult.NoExpiration);
			this.data[dnsQuery] = dnsResult;
			IL_8A:
			dnsRecordList.Add(new DnsCNameRecord(dnsQuery.Question, alias)
			{
				Section = DnsResponseSection.Answer,
				TimeToLive = TimeSpan.MaxValue
			});
		}

		public void AddAaaaQueryTimeOut(string domain)
		{
			this.quadATimeOuts.AddOrUpdate(domain, 0, (string k, int v) => v + 1);
		}

		public void RemoveAaaaTimeOutEntry(string domainName)
		{
			int num;
			this.quadATimeOuts.TryRemove(domainName, out num);
		}

		public int GetAaaaTimeOutEntry(string domainName)
		{
			int result;
			if (this.quadATimeOuts.TryGetValue(domainName, out result))
			{
				return result;
			}
			return -1;
		}

		public void AddDiagnosticInfoTo(XElement dnsCacheElement)
		{
			dnsCacheElement.SetAttributeValue("CacheSize", this.cacheSize);
			XElement xelement = new XElement("Cache");
			XElement xelement2 = new XElement("TimeOuts");
			dnsCacheElement.Add(xelement);
			dnsCacheElement.Add(xelement2);
			xelement.SetAttributeValue("Count", this.data.Count);
			foreach (KeyValuePair<DnsQuery, DnsResult> keyValuePair in this.data)
			{
				XElement xelement3 = new XElement("Entry");
				XElement content = new XElement("Query", keyValuePair.Key);
				XElement content2 = new XElement("Result", keyValuePair.Value);
				xelement3.Add(content);
				xelement3.Add(content2);
				xelement.Add(xelement3);
			}
			xelement2.SetAttributeValue("Count", this.quadATimeOuts.Count);
			foreach (KeyValuePair<string, int> keyValuePair2 in this.quadATimeOuts)
			{
				XElement xelement4 = new XElement("Entry");
				XElement content3 = new XElement("domain", keyValuePair2.Key);
				XElement content4 = new XElement("timeoutCount", keyValuePair2.Value);
				xelement4.Add(content3);
				xelement4.Add(content4);
				xelement2.Add(xelement4);
			}
		}

		private const int DefaultCacheSize = 10000;

		private const int PurgePercentage = 10;

		public const int QuadATimeoutNotFound = -1;

		private static readonly string HostsFile = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\HOSTS");

		private static readonly char[] Dividers = new char[]
		{
			' ',
			'\t'
		};

		private Dictionary<DnsQuery, DnsResult> data;

		private int cacheSize;

		private int hostsFileEntries;

		private ReaderWriterLockSlim syncRoot = new ReaderWriterLockSlim();

		private string pathToHostsFile;

		private FileSystemWatcher fileSystemWatcher;

		private ConcurrentDictionary<string, int> quadATimeOuts = new ConcurrentDictionary<string, int>();

		private class DnsExpireEntry : IComparer<DnsCache.DnsExpireEntry>
		{
			internal DnsExpireEntry(DnsQuery query, DateTime lastAccessDate)
			{
				this.query = query;
				this.lastAccessDate = lastAccessDate;
			}

			internal DnsQuery Query
			{
				get
				{
					return this.query;
				}
			}

			internal DateTime LastAccessDate
			{
				get
				{
					return this.lastAccessDate;
				}
			}

			public int Compare(DnsCache.DnsExpireEntry x, DnsCache.DnsExpireEntry y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				if (x.lastAccessDate < y.lastAccessDate)
				{
					return -1;
				}
				if (!(x.lastAccessDate > y.lastAccessDate))
				{
					return 0;
				}
				return 1;
			}

			private DnsQuery query;

			private DateTime lastAccessDate;
		}
	}
}
