using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Search.Query
{
	public class QueryHistoryInputDictionary : IEnumerable<QueryHistoryInputDictionaryEntry>, IEnumerable
	{
		public void InitializeFrom(Stream stream)
		{
			this.ParseQueryHistoryInput(stream);
		}

		public void SerializeTo(Stream stream)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(stream))
			{
				binaryWriter.Write(1);
				lock (this.collectionLock)
				{
					binaryWriter.Write(this.sortedEntriesByQuery.Count);
					foreach (QueryHistoryInputDictionaryEntry queryHistoryInputDictionaryEntry in this.sortedEntriesByQuery.Values)
					{
						queryHistoryInputDictionaryEntry.SerializeTo(binaryWriter);
					}
				}
				stream.SetLength(stream.Position);
			}
		}

		public void Merge(string query)
		{
			QueryHistoryInputDictionaryEntry entry = new QueryHistoryInputDictionaryEntry(query);
			this.Merge(entry);
		}

		public void Merge(QueryHistoryInputDictionaryEntry entry)
		{
			lock (this.collectionLock)
			{
				QueryHistoryInputDictionaryEntry queryHistoryInputDictionaryEntry;
				if (this.sortedEntriesByQuery.TryGetValue(entry.Query, out queryHistoryInputDictionaryEntry))
				{
					queryHistoryInputDictionaryEntry.Rank = Math.Min(queryHistoryInputDictionaryEntry.Rank + 0.001, 1.0);
					queryHistoryInputDictionaryEntry.LastUsed = entry.LastUsed;
				}
				else
				{
					while (this.sortedEntriesByQuery.Count >= 1000)
					{
						string key = string.Empty;
						long num = DateTime.MaxValue.Ticks;
						foreach (QueryHistoryInputDictionaryEntry queryHistoryInputDictionaryEntry2 in this.sortedEntriesByQuery.Values)
						{
							if (queryHistoryInputDictionaryEntry2.LastUsed < num)
							{
								num = queryHistoryInputDictionaryEntry2.LastUsed;
								key = queryHistoryInputDictionaryEntry2.Query;
							}
						}
						this.sortedEntriesByQuery.Remove(key);
					}
					this.sortedEntriesByQuery.Add(entry.Query, entry);
				}
			}
		}

		public bool Remove(string query)
		{
			bool result;
			lock (this.collectionLock)
			{
				result = this.sortedEntriesByQuery.Remove(query);
			}
			return result;
		}

		public void Clear()
		{
			lock (this.collectionLock)
			{
				this.sortedEntriesByQuery.Clear();
			}
		}

		public IEnumerator<QueryHistoryInputDictionaryEntry> GetEnumerator()
		{
			return this.sortedEntriesByQuery.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void ParseQueryHistoryInput(Stream inputStream)
		{
			if (inputStream == null)
			{
				return;
			}
			if (inputStream.Length > 0L)
			{
				using (BinaryReader binaryReader = new BinaryReader(inputStream))
				{
					int num;
					try
					{
						num = binaryReader.ReadInt32();
					}
					catch (EndOfStreamException)
					{
						return;
					}
					if (num >= 1)
					{
						int num2;
						try
						{
							num2 = binaryReader.ReadInt32();
						}
						catch (EndOfStreamException)
						{
							return;
						}
						lock (this.collectionLock)
						{
							this.sortedEntriesByQuery.Clear();
							for (int i = 0; i < num2; i++)
							{
								QueryHistoryInputDictionaryEntry queryHistoryInputDictionaryEntry = new QueryHistoryInputDictionaryEntry();
								if (!queryHistoryInputDictionaryEntry.DeserializeFrom(binaryReader))
								{
									break;
								}
								this.sortedEntriesByQuery.Add(queryHistoryInputDictionaryEntry.Query, queryHistoryInputDictionaryEntry);
							}
						}
					}
				}
			}
		}

		public const string Name = "Search.QueryHistoryInput";

		public const int CurrentSupportedVersion = 1;

		public const int MaximumTrackedQueries = 1000;

		public const double Increment = 0.001;

		private readonly SortedDictionary<string, QueryHistoryInputDictionaryEntry> sortedEntriesByQuery = new SortedDictionary<string, QueryHistoryInputDictionaryEntry>();

		private object collectionLock = new object();
	}
}
