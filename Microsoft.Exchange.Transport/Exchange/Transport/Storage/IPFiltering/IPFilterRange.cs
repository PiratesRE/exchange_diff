using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Storage.IPFiltering
{
	internal class IPFilterRange : IPRange, IComparable<IPFilterRange>
	{
		public IPFilterRange(int id, IPvxAddress start, IPvxAddress end, DateTime expiry, int type) : base(start, end, (IPRange.Format)(type & 15))
		{
			this.identity = id;
			this.timeToLive = expiry;
			this.flags = (type & 4080);
		}

		internal IPFilterRange(IPvxAddress start) : base(start, start, IPRange.Format.SingleAddress)
		{
			this.identity = -1;
			this.timeToLive = DateTime.MaxValue;
		}

		internal IPFilterRange(int identity, IPvxAddress start, IPvxAddress end, DateTime expiry, int type, string comment) : this(identity, start, end, expiry, type)
		{
			this.comment = comment;
		}

		public int Identity
		{
			get
			{
				return this.identity;
			}
			internal set
			{
				this.identity = value;
			}
		}

		public DateTime ExpiresOn
		{
			get
			{
				return this.timeToLive;
			}
		}

		public string Comment
		{
			get
			{
				return this.comment;
			}
			internal set
			{
				this.comment = value;
			}
		}

		public PolicyType PolicyType
		{
			get
			{
				if ((this.flags & 240) != 16)
				{
					return PolicyType.Deny;
				}
				return PolicyType.Allow;
			}
		}

		public bool AdminCreated
		{
			get
			{
				return (this.flags & 3840) == 256;
			}
		}

		internal int TypeFlags
		{
			get
			{
				return (int)(base.RangeFormat | (IPRange.Format)this.flags);
			}
		}

		public static IPFilterRange FromRowWithComment(IPFilterRow row)
		{
			return new IPFilterRange(row.Identity, row.LowerBound, row.UpperBound, row.ExpiresOn, row.TypeFlags, row.Comment);
		}

		public static IPFilterRange FromRowWithoutComment(IPFilterRow row)
		{
			return new IPFilterRange(row.Identity, row.LowerBound, row.UpperBound, row.ExpiresOn, row.TypeFlags);
		}

		public bool IsExpired(DateTime now)
		{
			return this.timeToLive <= now;
		}

		int IComparable<IPFilterRange>.CompareTo(IPFilterRange x)
		{
			if (x == null)
			{
				return 1;
			}
			return base.LowerBound.CompareTo(x.LowerBound);
		}

		internal void PurgeComment()
		{
			this.comment = null;
		}

		internal const int RangeFormatMask = 15;

		internal const int AllowFlag = 16;

		internal const int DenyFlag = 32;

		internal const int AllowDenyMask = 240;

		internal const int AdminCreatedFlag = 256;

		internal const int ProgrammaticallyCreatedFlag = 512;

		internal const int CreationTypeMask = 3840;

		private int identity;

		private string comment;

		private DateTime timeToLive;

		private int flags;
	}
}
