using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal abstract class SynchronizedThreadMap<K>
	{
		public SynchronizedThreadMap(int capacity, Trace tracer, string keyDisplayName, int estimatedEntrySize, int threadLimit, SmtpResponse exceededResponse, bool shouldRemoveEntryOnZero) : this(capacity, null, tracer, keyDisplayName, estimatedEntrySize, threadLimit, exceededResponse, null, shouldRemoveEntryOnZero)
		{
		}

		public SynchronizedThreadMap(int capacity, Trace tracer, string keyDisplayName, int estimatedEntrySize, int threadLimit, string exceptionMessage, bool shouldRemoveEntryOnZero) : this(capacity, null, tracer, keyDisplayName, estimatedEntrySize, threadLimit, SmtpResponse.Empty, exceptionMessage, shouldRemoveEntryOnZero)
		{
		}

		public SynchronizedThreadMap(int capacity, IEqualityComparer<K> comparer, Trace tracer, string keyDisplayName, int estimatedEntrySize, int threadLimit, SmtpResponse exceededResponse, bool shouldRemoveEntryOnZero) : this(capacity, comparer, tracer, keyDisplayName, estimatedEntrySize, threadLimit, exceededResponse, null, shouldRemoveEntryOnZero)
		{
		}

		public SynchronizedThreadMap(int capacity, IEqualityComparer<K> comparer, Trace tracer, string keyDisplayName, int estimatedEntrySize, int threadLimit, string exceptionMessage, bool shouldRemoveEntryOnZero) : this(capacity, comparer, tracer, keyDisplayName, estimatedEntrySize, threadLimit, SmtpResponse.Empty, exceptionMessage, shouldRemoveEntryOnZero)
		{
		}

		public SynchronizedThreadMap(int capacity, IEqualityComparer<K> comparer, Trace tracer, string keyDisplayName, int estimatedEntrySize, int threadLimit, SmtpResponse exceededResponse, string exceptionMessage, bool shouldRemoveEntryOnZero)
		{
			this.map = new Dictionary<K, int>(capacity, comparer);
			this.SyncRoot = new object();
			this.Tracer = tracer;
			this.keyDisplayName = keyDisplayName;
			this.estimatedEntrySize = estimatedEntrySize;
			this.exceededResponse = exceededResponse;
			this.exceptionMessage = exceptionMessage;
			this.shouldRemoveEntryOnZero = shouldRemoveEntryOnZero;
			this.threadLimit = threadLimit;
		}

		protected SynchronizedThreadMap(int capacity, IEqualityComparer<K> comparer, Trace tracer, string keyDisplayName, int estimatedEntrySize, Dictionary<K, int> threadLimitsForDiagnostics, SmtpResponse exceededResponse, bool shouldRemoveEntryOnZero) : this(capacity, comparer, tracer, keyDisplayName, estimatedEntrySize, 0, exceededResponse, null, shouldRemoveEntryOnZero)
		{
			this.threadLimitsForDiagnostics = threadLimitsForDiagnostics;
		}

		public int ThreadLimit
		{
			get
			{
				return this.threadLimit;
			}
			set
			{
				this.threadLimit = value;
			}
		}

		protected Dictionary<K, int> ThreadMap
		{
			get
			{
				return this.map;
			}
		}

		private protected Trace Tracer { protected get; private set; }

		private protected object SyncRoot { protected get; private set; }

		public static int CalculateAdaptiveThreadLimit(Trace tracer, string entityType, int availableThreads, int perEntityThreadLimit, int entityCount)
		{
			TraceHelper.TracePass(tracer, 0L, "CalculateAdaptiveThreadLimit: perEntityThreadLimit: {0}, availableThreads: {1}, entityCount: {2}, entityType: {3}", new object[]
			{
				perEntityThreadLimit,
				availableThreads,
				entityCount,
				entityType
			});
			if (entityCount == 0)
			{
				return availableThreads;
			}
			int num = perEntityThreadLimit * entityCount;
			if (num < availableThreads)
			{
				perEntityThreadLimit = (int)Math.Ceiling((double)availableThreads / (double)entityCount);
				TraceHelper.TracePass(tracer, 0L, "CalculateAdaptiveThreadLimit: thread limit changed to {0} to ensure all available threads are used", new object[]
				{
					perEntityThreadLimit
				});
			}
			return perEntityThreadLimit;
		}

		public void CheckAndIncrement(K key, ulong sessionId, string mdb)
		{
			this.CheckThreadLimit(key, this.threadLimit, sessionId, mdb, true);
		}

		public bool TryCheckAndIncrement(K key, ulong sessionId, string mdb)
		{
			return this.TryCheckThreadLimit(key, this.threadLimit, sessionId, mdb, true);
		}

		public void Decrement(K key)
		{
			lock (this.SyncRoot)
			{
				int num;
				if (!this.map.TryGetValue(key, out num))
				{
					string message = string.Format(CultureInfo.InvariantCulture, "Trying to decrement a non-existent thread map entry {0}. Current map content is {1}", new object[]
					{
						key,
						this.Dump()
					});
					TraceHelper.TraceFail(this.Tracer, 0L, message);
					throw new InvalidOperationException(message);
				}
				if (1 > num)
				{
					string message2 = string.Format(CultureInfo.InvariantCulture, "Trying to decrement a thread map entry {0} that is already 0. Current map content is {1}", new object[]
					{
						key,
						this.Dump()
					});
					this.Tracer.TraceError(0L, message2);
					throw new InvalidOperationException(message2);
				}
				if (this.shouldRemoveEntryOnZero && 1 == num)
				{
					this.map.Remove(key);
					if (this.threadLimitsForDiagnostics != null)
					{
						this.threadLimitsForDiagnostics.Remove(key);
					}
					TraceHelper.TracePass(this.Tracer, 0L, "Removed thread map entry {0} since it's the last thread being used.", new object[]
					{
						key
					});
				}
				else
				{
					num = (this.map[key] = num - 1);
					TraceHelper.TracePass(this.Tracer, 0L, "Decremented thread map entry {0} to {1}.", new object[]
					{
						key,
						num
					});
				}
			}
		}

		public string Dump()
		{
			string result;
			lock (this.SyncRoot)
			{
				StringBuilder stringBuilder = new StringBuilder(this.estimatedEntrySize * this.map.Count);
				foreach (KeyValuePair<K, int> keyValuePair in this.map)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:{1},{2}", new object[]
					{
						keyValuePair.Key,
						keyValuePair.Value,
						Environment.NewLine
					});
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public virtual XElement GetDiagnosticInfo(XElement parentElement)
		{
			lock (this.SyncRoot)
			{
				parentElement.Add(new XElement("count", this.map.Count));
				if (this.threadLimitsForDiagnostics == null)
				{
					parentElement.Add(new XElement("limit", this.threadLimit));
				}
				if (this.map.Count > 0)
				{
					foreach (KeyValuePair<K, int> keyValuePair in this.map)
					{
						XElement xelement = new XElement("item");
						xelement.Add(new XElement("key", keyValuePair.Key));
						xelement.Add(new XElement("value", keyValuePair.Value));
						if (this.threadLimitsForDiagnostics != null)
						{
							int num;
							if (this.threadLimitsForDiagnostics.TryGetValue(keyValuePair.Key, out num))
							{
								xelement.Add(new XElement("limit", num));
							}
							else
							{
								xelement.Add(new XElement("limit", "not set"));
							}
						}
						parentElement.Add(xelement);
					}
				}
			}
			return parentElement;
		}

		protected void CheckThreadLimit(K key, int threadLimit, ulong sessionId, string mdb, bool shouldIncrement)
		{
			if (this.TryCheckThreadLimit(key, threadLimit, sessionId, mdb, shouldIncrement))
			{
				return;
			}
			if (this.exceptionMessage != null)
			{
				string message = string.Format(this.exceptionMessage, key, threadLimit);
				throw new ThreadLimitExceededException(message);
			}
			throw new ThreadLimitExceededException(this.exceededResponse);
		}

		protected bool TryCheckThreadLimit(K key, int threadLimit, ulong sessionId, string mdb, bool shouldIncrement)
		{
			bool result;
			lock (this.SyncRoot)
			{
				int num;
				if (!this.map.TryGetValue(key, out num))
				{
					num = 0;
				}
				if (this.threadLimitsForDiagnostics != null)
				{
					this.threadLimitsForDiagnostics[key] = threadLimit;
				}
				if (shouldIncrement)
				{
					num++;
				}
				if (num > threadLimit)
				{
					TraceHelper.TraceFail(this.Tracer, 0L, "{0} {1} has reached thread limit {2}.", new object[]
					{
						this.keyDisplayName,
						key,
						threadLimit
					});
					this.LogLimitExceeded(key, threadLimit, sessionId, mdb);
					result = false;
				}
				else
				{
					this.map[key] = num;
					TraceHelper.TracePass(this.Tracer, 0L, "{0} {1} has been incremented to {2}.", new object[]
					{
						this.keyDisplayName,
						key,
						num
					});
					result = true;
				}
			}
			return result;
		}

		protected virtual void LogLimitExceeded(K key, int threadLimit, ulong sessionId, string mdb)
		{
		}

		private Dictionary<K, int> map;

		private string keyDisplayName;

		private int estimatedEntrySize;

		private SmtpResponse exceededResponse;

		private string exceptionMessage;

		private bool shouldRemoveEntryOnZero;

		private int threadLimit;

		private Dictionary<K, int> threadLimitsForDiagnostics;
	}
}
