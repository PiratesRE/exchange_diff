using System;
using System.IO;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class SourceInformation
	{
		public SourceInformation(string name, string id, string sourceFilter, Uri serviceEndpoint, string legacyExchangeDN)
		{
			this.Configuration = new SourceInformation.SourceConfiguration(name, id, sourceFilter, serviceEndpoint, legacyExchangeDN);
			this.Status = new SourceInformation.SourceStatus();
		}

		public ISourceDataProvider ServiceClient { get; set; }

		public SourceInformation.SourceConfiguration Configuration { get; private set; }

		public SourceInformation.SourceStatus Status { get; set; }

		[Serializable]
		public class SourceConfiguration
		{
			public SourceConfiguration(string name, string id, string sourceFilter, Uri serviceEndpoint, string legacyExchangeDN)
			{
				this.Name = name;
				this.Id = id;
				this.SourceFilter = sourceFilter;
				this.ServiceEndpoint = serviceEndpoint;
				this.LegacyExchangeDN = legacyExchangeDN;
			}

			public string Name { get; private set; }

			public string Id { get; private set; }

			public string SourceFilter { get; private set; }

			public Uri ServiceEndpoint { get; internal set; }

			public string LegacyExchangeDN { get; private set; }

			internal string SearchName { get; set; }
		}

		[Serializable]
		public class SourceStatus : ISourceStatus
		{
			public SourceStatus()
			{
				this.ItemCount = -1;
				this.UnsearchableItemCount = -1;
			}

			public int ProcessedItemCount { get; set; }

			public int ItemCount { get; set; }

			public long TotalSize { get; set; }

			public int ProcessedUnsearchableItemCount { get; set; }

			public int UnsearchableItemCount { get; set; }

			public long DuplicateItemCount { get; set; }

			public long UnsearchableDuplicateItemCount { get; set; }

			public long ErrorItemCount { get; set; }

			public bool IsSearchCompleted(bool includeSearchableItems, bool includeUnsearchableItems)
			{
				return (this.ItemCount >= 0 || !includeSearchableItems) && (this.UnsearchableItemCount >= 0 || !includeUnsearchableItems);
			}

			public bool IsExportCompleted(bool includeSearchableItems, bool includeUnsearchableItems)
			{
				return ((this.ItemCount >= 0 && this.ItemCount <= this.ProcessedItemCount) || !includeSearchableItems) && ((this.UnsearchableItemCount >= 0 && this.UnsearchableItemCount <= this.ProcessedUnsearchableItemCount) || !includeUnsearchableItems);
			}

			public void SaveToStream(Stream stream)
			{
				stream.Write(BitConverter.GetBytes(this.ProcessedItemCount), 0, 4);
				stream.Write(BitConverter.GetBytes(this.ItemCount), 0, 4);
				stream.Write(BitConverter.GetBytes(this.TotalSize), 0, 8);
				stream.Write(BitConverter.GetBytes(this.ProcessedUnsearchableItemCount), 0, 4);
				stream.Write(BitConverter.GetBytes(this.UnsearchableItemCount), 0, 4);
				stream.Write(BitConverter.GetBytes(this.DuplicateItemCount), 0, 8);
				stream.Write(BitConverter.GetBytes(this.UnsearchableDuplicateItemCount), 0, 8);
				stream.Write(BitConverter.GetBytes(this.ErrorItemCount), 0, 8);
			}

			public void LoadFromStream(Stream stream)
			{
				byte[] array = new byte[8];
				SourceInformation.SourceStatus.SafeRead(stream, array, 4);
				this.ProcessedItemCount = BitConverter.ToInt32(array, 0);
				SourceInformation.SourceStatus.SafeRead(stream, array, 4);
				this.ItemCount = BitConverter.ToInt32(array, 0);
				SourceInformation.SourceStatus.SafeRead(stream, array, 8);
				this.TotalSize = BitConverter.ToInt64(array, 0);
				SourceInformation.SourceStatus.SafeRead(stream, array, 4);
				this.ProcessedUnsearchableItemCount = BitConverter.ToInt32(array, 0);
				SourceInformation.SourceStatus.SafeRead(stream, array, 4);
				this.UnsearchableItemCount = BitConverter.ToInt32(array, 0);
				SourceInformation.SourceStatus.SafeRead(stream, array, 8);
				this.DuplicateItemCount = BitConverter.ToInt64(array, 0);
				SourceInformation.SourceStatus.SafeRead(stream, array, 8);
				this.UnsearchableDuplicateItemCount = BitConverter.ToInt64(array, 0);
				SourceInformation.SourceStatus.SafeRead(stream, array, 8);
				this.ErrorItemCount = BitConverter.ToInt64(array, 0);
			}

			private static void SafeRead(Stream stream, byte[] buffer, int length)
			{
				int num = stream.Read(buffer, 0, length);
				if (num != length)
				{
					throw new ExportException(ExportErrorType.CorruptedStatus);
				}
			}
		}
	}
}
