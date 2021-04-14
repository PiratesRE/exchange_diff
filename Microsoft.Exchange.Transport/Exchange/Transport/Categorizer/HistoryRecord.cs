using System;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class HistoryRecord : IEquatable<HistoryRecord>, IComparable<HistoryRecord>, IHistoryRecordFacade
	{
		internal HistoryRecord(HistoryType type, RoutingAddress address)
		{
			this.type = type;
			this.address = address;
		}

		public HistoryType Type
		{
			get
			{
				return this.type;
			}
		}

		public RoutingAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public override string ToString()
		{
			if (this.serializedString != null)
			{
				return this.serializedString;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(HistoryRecord.historyTypeStrings[(int)this.type]);
			stringBuilder.Append(": ");
			stringBuilder.Append(this.address.ToString());
			this.serializedString = stringBuilder.ToString();
			return this.serializedString;
		}

		internal static HistoryRecord Parse(string serializedRecord)
		{
			int num = serializedRecord.IndexOf(':');
			if (num == -1)
			{
				throw new FormatException("History record missing ':' delimiter");
			}
			HistoryType historyType = HistoryRecord.ParseType(serializedRecord.Substring(0, num));
			string text = serializedRecord.Substring(num + 1).Trim();
			RoutingAddress routingAddress = new RoutingAddress(text);
			if (!routingAddress.IsValid)
			{
				throw new FormatException("Address is invalid");
			}
			return new HistoryRecord(historyType, routingAddress)
			{
				serializedString = serializedRecord
			};
		}

		private static HistoryType ParseType(string serializedType)
		{
			for (int i = 0; i < HistoryRecord.historyTypeStrings.Length; i++)
			{
				if (serializedType.Equals(HistoryRecord.historyTypeStrings[i]))
				{
					return (HistoryType)i;
				}
			}
			throw new FormatException("Unrecognized HistoryType");
		}

		public bool Equals(HistoryRecord other)
		{
			return this.type == other.type && this.address == other.address;
		}

		public int CompareTo(HistoryRecord other)
		{
			if (this.type != other.type)
			{
				return this.type - other.type;
			}
			return string.Compare(this.address.ToString(), other.address.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		private static string[] historyTypeStrings = new string[]
		{
			HistoryType.Expanded.ToString(),
			HistoryType.Forwarded.ToString(),
			HistoryType.DeliveredAndForwarded.ToString()
		};

		private HistoryType type;

		private RoutingAddress address;

		private string serializedString;
	}
}
