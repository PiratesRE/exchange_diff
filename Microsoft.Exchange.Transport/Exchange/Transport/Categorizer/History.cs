using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class History : IEquatable<History>, IComparable<History>, IHistoryFacade
	{
		private History(List<HistoryRecord> records, RecipientP2Type recipientP2Type)
		{
			this.records = records;
			this.recipientP2Type = recipientP2Type;
		}

		public List<HistoryRecord> Records
		{
			get
			{
				return this.records;
			}
		}

		public RecipientP2Type RecipientType
		{
			get
			{
				return this.recipientP2Type;
			}
		}

		List<IHistoryRecordFacade> IHistoryFacade.Records
		{
			get
			{
				if (this.records == null)
				{
					return null;
				}
				return this.records.ConvertAll<IHistoryRecordFacade>((HistoryRecord record) => record);
			}
		}

		public static bool operator ==(History a, History b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.recipientP2Type != b.recipientP2Type)
			{
				return false;
			}
			if (a.records == null || b.records == null)
			{
				return false;
			}
			if (a.records.Count != b.records.Count)
			{
				return false;
			}
			for (int i = 0; i < a.records.Count; i++)
			{
				if (!a.records[i].Equals(b.records[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator !=(History a, History b)
		{
			return !(a == b);
		}

		public static History Derive(History parent, HistoryType type, RoutingAddress address, RecipientP2Type parentP2Type)
		{
			if (parent != null && parent.recipientP2Type != parentP2Type)
			{
				throw new InvalidOperationException("Parent history must match recipientP2Type passed in during Derive");
			}
			List<HistoryRecord> list = new List<HistoryRecord>();
			if (parent != null && parent.records != null && parent.records.Count != 0)
			{
				list.AddRange(parent.records);
			}
			HistoryRecord item = new HistoryRecord(type, address);
			list.Add(item);
			return new History(list, parentP2Type);
		}

		private static bool CheckValidRecipientP2Type(int p2Type)
		{
			return p2Type >= 0 && p2Type <= 3;
		}

		private static History ReadFrom(IExtendedPropertyCollection extendedProperties)
		{
			ReadOnlyCollection<string> readOnlyCollection;
			if (!extendedProperties.TryGetListValue<string>("Microsoft.Exchange.Transport.History", out readOnlyCollection))
			{
				readOnlyCollection = null;
			}
			RecipientP2Type recipientP2Type = RecipientP2Type.Unknown;
			int num;
			if (extendedProperties.TryGetValue<int>("Microsoft.Exchange.Transport.RecipientP2Type", out num) && History.CheckValidRecipientP2Type(num))
			{
				recipientP2Type = (RecipientP2Type)num;
			}
			if (recipientP2Type == RecipientP2Type.Unknown && (readOnlyCollection == null || readOnlyCollection.Count == 0))
			{
				return null;
			}
			History result;
			try
			{
				List<HistoryRecord> list = History.ParseSerializedHistory(readOnlyCollection);
				result = new History(list, recipientP2Type);
			}
			catch (FormatException)
			{
				ExTraceGlobals.ResolverTracer.TraceError(0L, "Could not parse recipient history from extended properties");
				result = null;
			}
			return result;
		}

		internal static History ReadFrom(TransportMailItem mailItem)
		{
			History history = History.ReadFrom(mailItem.ExtendedProperties);
			if (history != null)
			{
				return history;
			}
			return History.ReadFrom(mailItem.Message.MimeDocument.RootPart.Headers);
		}

		internal static History ReadFrom(MailRecipient recipient)
		{
			return History.ReadFrom(recipient.ExtendedProperties);
		}

		public static History ReadFrom(HeaderList headers)
		{
			RecipientP2Type recipientP2Type = RecipientP2Type.Unknown;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-Recipient-P2-Type");
			if (header != null)
			{
				try
				{
					recipientP2Type = History.ParseRecipientP2Type(header.Value.Trim());
				}
				catch (ExchangeDataException)
				{
					ExTraceGlobals.ResolverTracer.TraceError(0L, "Invalid MIME for RecipientP2Type header, treating as if no history");
					return null;
				}
				catch (FormatException)
				{
					ExTraceGlobals.ResolverTracer.TraceError(0L, "Corrupt recipient-type header, treating as if recipient type was \"Unknown\"");
				}
			}
			Header[] array = headers.FindAll("X-MS-Exchange-Organization-History");
			List<HistoryRecord> list = null;
			List<string> list2 = new List<string>();
			try
			{
				foreach (Header header2 in array)
				{
					list2.Add(header2.Value);
				}
			}
			catch (ExchangeDataException)
			{
				return null;
			}
			try
			{
				list = History.ParseSerializedHistory(list2);
			}
			catch (FormatException)
			{
				return null;
			}
			if (list == null && recipientP2Type == RecipientP2Type.Unknown)
			{
				return null;
			}
			return new History(list, recipientP2Type);
		}

		internal static IHistoryFacade ReadFromMailItemByAgent(ITransportMailItemFacade mailItem)
		{
			return History.ReadFrom((TransportMailItem)mailItem);
		}

		internal static IHistoryFacade ReadFromRecipientByAgent(IMailRecipientFacade recipient)
		{
			return History.ReadFrom((MailRecipient)recipient);
		}

		private static List<HistoryRecord> ParseSerializedHistory(IList<string> serializedHistory)
		{
			if (serializedHistory == null || serializedHistory.Count == 0)
			{
				return null;
			}
			List<HistoryRecord> list = new List<HistoryRecord>(serializedHistory.Count);
			foreach (string serializedRecord in serializedHistory)
			{
				HistoryRecord item = HistoryRecord.Parse(serializedRecord);
				list.Add(item);
			}
			return list;
		}

		public static void Clear(TransportMailItem mailItem)
		{
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			History history = History.ReadFrom(headers);
			if (history != null)
			{
				history.WriteTo(mailItem.ExtendedProperties);
			}
			headers.RemoveAll("X-MS-Exchange-Organization-History");
		}

		public void WriteTo(MailRecipient recipient)
		{
			this.WriteTo(recipient.ExtendedProperties);
		}

		public void WriteTo(IExtendedPropertyCollection extendedProperties)
		{
			List<string> serializedHistory;
			if (this.cachedSerializedHistory != null)
			{
				serializedHistory = this.cachedSerializedHistory;
			}
			else
			{
				serializedHistory = this.GetSerializedHistory();
			}
			extendedProperties.SetValue<List<string>>("Microsoft.Exchange.Transport.History", serializedHistory);
			extendedProperties.SetValue<int>("Microsoft.Exchange.Transport.RecipientP2Type", (int)this.recipientP2Type);
		}

		public void WriteTo(HeaderList headers)
		{
			MimeNode mimeNode = null;
			MimeNode mimeNode2 = null;
			foreach (Header header in headers)
			{
				bool flag = string.Equals(header.Name, "X-MS-Exchange-Organization-History", StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					if (mimeNode == null)
					{
						mimeNode = header;
					}
					mimeNode2 = header;
				}
				else if (mimeNode != null)
				{
					break;
				}
			}
			if (mimeNode2 == null)
			{
				mimeNode2 = headers.LastChild;
			}
			if (this.records != null && this.records.Count != 0)
			{
				foreach (HistoryRecord historyRecord in this.records)
				{
					Header newChild = new AsciiTextHeader("X-MS-Exchange-Organization-History", historyRecord.ToString());
					mimeNode2 = headers.InsertAfter(newChild, mimeNode2);
				}
			}
			if (this.recipientP2Type != RecipientP2Type.Unknown && headers.FindFirst("X-MS-Exchange-Organization-Recipient-P2-Type") == null)
			{
				Header newChild2 = new AsciiTextHeader("X-MS-Exchange-Organization-Recipient-P2-Type", History.RecipientP2TypeToString(this.recipientP2Type));
				headers.AppendChild(newChild2);
			}
		}

		private static string RecipientP2TypeToString(RecipientP2Type p2Type)
		{
			return History.recipientP2TypeNames[(int)p2Type];
		}

		private static RecipientP2Type ParseRecipientP2Type(string serializedP2Type)
		{
			for (int i = 0; i < History.recipientP2TypeNames.Length; i++)
			{
				if (serializedP2Type.Equals(History.recipientP2TypeNames[i], StringComparison.OrdinalIgnoreCase))
				{
					return (RecipientP2Type)i;
				}
			}
			ExTraceGlobals.ResolverTracer.TraceError<string>(0L, "Could not parse {0} as a RecipientP2Type", serializedP2Type);
			throw new FormatException("Unrecognized recipient P2 type");
		}

		public bool Contains(RoutingAddress searchAddress, out bool reportable)
		{
			reportable = true;
			if (this.records == null || this.records.Count == 0)
			{
				return false;
			}
			foreach (HistoryRecord historyRecord in this.records)
			{
				reportable = (reportable && historyRecord.Type == HistoryType.Forwarded);
				if (historyRecord.Address == searchAddress)
				{
					return true;
				}
			}
			return false;
		}

		public bool Equals(History other)
		{
			return this == other;
		}

		public int CompareTo(History other)
		{
			int num = 0;
			RecipientP2Type recipientP2Type = (other == null) ? RecipientP2Type.Unknown : other.recipientP2Type;
			if (this.recipientP2Type != recipientP2Type)
			{
				return this.recipientP2Type - recipientP2Type;
			}
			if (other != null && other.records != null)
			{
				num = other.records.Count;
			}
			int num2 = (this.records == null) ? 0 : this.records.Count;
			int num3 = (num2 < num) ? num2 : num;
			for (int i = 0; i < num3; i++)
			{
				int num4 = this.records[i].CompareTo(other.records[i]);
				if (num4 != 0)
				{
					return num4;
				}
			}
			return num2 - num;
		}

		public override bool Equals(object obj)
		{
			return this == obj as History;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void CacheSerializedHistory()
		{
			this.cachedSerializedHistory = this.GetSerializedHistory();
		}

		public void ClearSerializedHistory()
		{
			this.cachedSerializedHistory = null;
		}

		private List<string> GetSerializedHistory()
		{
			List<string> list = null;
			if (this.records != null && this.records.Count > 0)
			{
				list = new List<string>();
				foreach (HistoryRecord historyRecord in this.records)
				{
					string item = historyRecord.ToString();
					list.Add(item);
				}
			}
			return list;
		}

		private const string HistoryHeader = "X-MS-Exchange-Organization-History";

		private const string HistoryProperty = "Microsoft.Exchange.Transport.History";

		public const string RecipientP2TypeProperty = "Microsoft.Exchange.Transport.RecipientP2Type";

		private const string RecipientP2TypeHeader = "X-MS-Exchange-Organization-Recipient-P2-Type";

		private static string[] recipientP2TypeNames = new string[]
		{
			RecipientP2Type.Unknown.ToString(),
			RecipientP2Type.To.ToString(),
			RecipientP2Type.Cc.ToString(),
			RecipientP2Type.Bcc.ToString()
		};

		private RecipientP2Type recipientP2Type;

		private List<HistoryRecord> records;

		private List<string> cachedSerializedHistory;
	}
}
