using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal class RoutedQueueBase : DataRow
	{
		internal RoutedQueueBase(long id, NextHopSolutionKey key) : base(Components.MessagingDatabase.Database.QueueTable)
		{
			this.Id = id;
			this.NextHopConnector = key.NextHopConnector;
			this.NextHopType = key.NextHopType;
			this.SetNextHopDomainAndTlsDomain(key.NextHopDomain, key.NextHopTlsDomain);
			this.State = 0;
		}

		private RoutedQueueBase() : base(Components.MessagingDatabase.Database.QueueTable)
		{
		}

		public long Id
		{
			get
			{
				return ((ColumnCache<long>)base.Columns[0]).Value;
			}
			private set
			{
				((ColumnCache<long>)base.Columns[0]).Value = value;
			}
		}

		public Guid NextHopConnector
		{
			get
			{
				return ((ColumnCache<Guid>)base.Columns[1]).Value;
			}
			private set
			{
				((ColumnCache<Guid>)base.Columns[1]).Value = value;
			}
		}

		public string NextHopDomain
		{
			get
			{
				string result;
				string text;
				this.SeperateDomainAndTls(((ColumnCache<string>)base.Columns[2]).Value, out result, out text);
				return result;
			}
		}

		public string NextHopTlsDomain
		{
			get
			{
				string text;
				string result;
				this.SeperateDomainAndTls(((ColumnCache<string>)base.Columns[2]).Value, out text, out result);
				return result;
			}
		}

		private void SetNextHopDomainAndTlsDomain(string nextHopDomain, string tlsDomain)
		{
			string value = string.Format("{0}{1}{2}", nextHopDomain, "/", tlsDomain);
			((ColumnCache<string>)base.Columns[2]).Value = value;
		}

		private void SeperateDomainAndTls(string storedValue, out string nextHopDomain, out string nextHopTlsDomain)
		{
			if (string.IsNullOrEmpty(storedValue))
			{
				nextHopDomain = string.Empty;
				nextHopTlsDomain = string.Empty;
				return;
			}
			int num = storedValue.IndexOf("/");
			if (num < 0)
			{
				nextHopDomain = storedValue;
				nextHopTlsDomain = string.Empty;
				return;
			}
			nextHopDomain = storedValue.Substring(0, num);
			nextHopTlsDomain = storedValue.Substring(num + 1);
		}

		public NextHopType NextHopType
		{
			get
			{
				return new NextHopType(((ColumnCache<int>)base.Columns[4]).Value);
			}
			private set
			{
				((ColumnCache<int>)base.Columns[4]).Value = value.ToInt32();
			}
		}

		public bool Suspended
		{
			get
			{
				return (this.State & 1) != 0;
			}
			set
			{
				this.State = ((value ? 1 : 0) | (this.State & -2));
			}
		}

		private int State
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[3]).Value;
			}
			set
			{
				ColumnCache<int> columnCache = (ColumnCache<int>)base.Columns[3];
				if (!columnCache.HasValue || columnCache.Value != value)
				{
					((ColumnCache<int>)base.Columns[3]).Value = value;
				}
			}
		}

		public static RoutedQueueBase LoadFromRow(DataTableCursor cursor)
		{
			RoutedQueueBase routedQueueBase = new RoutedQueueBase();
			routedQueueBase.LoadFromCurrentRow(cursor);
			return routedQueueBase;
		}

		public new void Commit()
		{
			base.Commit(TransactionCommitMode.Lazy);
		}

		private const string NextHopDomainAndTlsDomainSeperator = "/";

		private enum StateBits
		{
			Active,
			Suspended
		}
	}
}
