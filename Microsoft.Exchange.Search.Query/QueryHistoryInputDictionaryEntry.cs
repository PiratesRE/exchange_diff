using System;
using System.IO;

namespace Microsoft.Exchange.Search.Query
{
	public class QueryHistoryInputDictionaryEntry
	{
		public QueryHistoryInputDictionaryEntry()
		{
		}

		public QueryHistoryInputDictionaryEntry(string query)
		{
			this.Rank = 0.001;
			this.LastUsed = DateTime.UtcNow.Ticks;
			this.Query = query.Trim();
		}

		public string Query { get; set; }

		public double Rank { get; set; }

		public long LastUsed { get; set; }

		public void SerializeTo(BinaryWriter writer)
		{
			writer.Write(this.Query);
			writer.Write(this.Rank);
			writer.Write(this.LastUsed);
		}

		public bool DeserializeFrom(BinaryReader reader)
		{
			bool result = false;
			try
			{
				this.Query = reader.ReadString();
				this.Rank = reader.ReadDouble();
				this.LastUsed = reader.ReadInt64();
				result = true;
			}
			catch (EndOfStreamException)
			{
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			return this.Query.Equals(((QueryHistoryInputDictionaryEntry)obj).Query, StringComparison.InvariantCultureIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.Query.GetHashCode();
		}
	}
}
