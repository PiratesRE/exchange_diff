using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class QueryableExchangeId
	{
		public QueryableExchangeId(ExchangeId exchangeId)
		{
			this.exchangeId = exchangeId;
		}

		public ushort Replid
		{
			get
			{
				return this.exchangeId.Replid;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.exchangeId.Guid;
			}
		}

		public ulong Counter
		{
			get
			{
				return this.exchangeId.Counter;
			}
		}

		public byte[] Globcnt
		{
			get
			{
				return this.exchangeId.Globcnt;
			}
		}

		public byte[] Binary8ByteFormat
		{
			get
			{
				return this.exchangeId.To8ByteArray();
			}
		}

		public byte[] Binary9ByteFormat
		{
			get
			{
				return this.exchangeId.To9ByteArray();
			}
		}

		public byte[] Binary22ByteFormat
		{
			get
			{
				return this.exchangeId.To22ByteArray();
			}
		}

		public byte[] Binary24ByteFormat
		{
			get
			{
				return this.exchangeId.To24ByteArray();
			}
		}

		public byte[] Binary26ByteFormat
		{
			get
			{
				return this.exchangeId.To26ByteArray();
			}
		}

		public long LongFormat
		{
			get
			{
				return this.exchangeId.ToLong();
			}
		}

		public string ExmonStringFormat
		{
			get
			{
				return string.Format("{0:x}-{1:x}", this.exchangeId.Replid, this.exchangeId.Counter);
			}
		}

		public string TraceStringFormat
		{
			get
			{
				return this.exchangeId.ToString();
			}
		}

		public bool IsReplidKnown
		{
			get
			{
				return this.exchangeId.IsReplidKnown;
			}
		}

		public bool IsNull
		{
			get
			{
				return this.exchangeId.IsNull;
			}
		}

		public bool IsZero
		{
			get
			{
				return this.exchangeId.IsZero;
			}
		}

		public bool IsNullOrZero
		{
			get
			{
				return this.exchangeId.IsNullOrZero;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.exchangeId.IsValid;
			}
		}

		private readonly ExchangeId exchangeId;
	}
}
