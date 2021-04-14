using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.UnifiedContent
{
	public class UnifiedContentSerializer
	{
		internal UnifiedContentSerializer(Stream stream, SharedStream sharedStream, List<IExtractedContent> contentList = null)
		{
			this.sharedStream = sharedStream;
			this.outputStream = stream;
			this.contentList = (contentList ?? new List<IExtractedContent>());
			using (SharedContentWriter sharedContentWriter = new SharedContentWriter(stream))
			{
				this.WriteHeader(sharedContentWriter, sharedStream.SharedName);
			}
		}

		internal List<IExtractedContent> ContentCollection
		{
			get
			{
				return this.contentList;
			}
		}

		internal List<IRawContent> RawContentCollection
		{
			get
			{
				return this.contentList.Cast<IRawContent>().ToList<IRawContent>();
			}
		}

		public void Commit()
		{
			this.contentList = this.uncommittedContent;
			this.uncommittedContent = new List<IExtractedContent>();
			this.sharedStream.Flush();
			foreach (UnifiedContentSerializer.HeaderInfo headerInfo in this.headerInfoList)
			{
				this.WriteStreamHeader(headerInfo.Id, headerInfo.Content);
				headerInfo.Content.NeedsUpdate = true;
			}
			this.headerInfoList.Clear();
			this.outputStream.Flush();
		}

		public void WriteProperty(UnifiedContentSerializer.PropertyId id, string data)
		{
			long value = 8L + SharedContentWriter.ComputeLength(data);
			using (SharedContentWriter sharedContentWriter = new SharedContentWriter(this.outputStream))
			{
				sharedContentWriter.Write(value);
				sharedContentWriter.Write(UnifiedContentSerializer.EntryId.Property);
				sharedContentWriter.Write(id);
				sharedContentWriter.Write(data);
			}
		}

		internal SharedContent AddStream(UnifiedContentSerializer.EntryId id, Stream stream, string name)
		{
			int count = this.uncommittedContent.Count;
			SharedContent sharedContent = (SharedContent)this.LookupStream(count, stream);
			if (sharedContent != null)
			{
				this.AddStreamHeader(id, sharedContent);
				return sharedContent;
			}
			return this.WriteStream(id, stream, name);
		}

		internal SharedContent WriteStream(UnifiedContentSerializer.EntryId id, Stream stream, string name)
		{
			SharedContent sharedContent = SharedContent.Create(this.sharedStream, stream, name);
			sharedContent.FileName = name;
			this.AddStreamHeader(id, sharedContent);
			return sharedContent;
		}

		internal SharedContent AddNewStream(UnifiedContentSerializer.EntryId id, Stream stream, string name)
		{
			int count = this.uncommittedContent.Count;
			if ((SharedContent)this.LookupStream(count, stream) == null)
			{
				return this.WriteStream(id, stream, name);
			}
			return null;
		}

		internal List<IExtractedContent> LookupStream(string name, Stream stream)
		{
			uint hash = Crc32.ComputeHash(stream, 0L);
			IEnumerable<IExtractedContent> source = from c in this.contentList
			where c.FileName == name && !c.IsModified(hash)
			select c;
			return source.ToList<IExtractedContent>();
		}

		private IExtractedContent LookupStream(int index, Stream stream)
		{
			if (this.contentList.Count > index)
			{
				IExtractedContent extractedContent = this.contentList[index];
				if (extractedContent != null && !extractedContent.IsModified(stream))
				{
					return extractedContent;
				}
			}
			return null;
		}

		private void AddStreamHeader(UnifiedContentSerializer.EntryId id, SharedContent sharedContent)
		{
			this.headerInfoList.Add(new UnifiedContentSerializer.HeaderInfo
			{
				Id = id,
				Content = sharedContent
			});
			this.uncommittedContent.Add(sharedContent);
		}

		private void WriteStreamHeader(UnifiedContentSerializer.EntryId id, SharedContent sharedContent)
		{
			using (SharedContentWriter sharedContentWriter = new SharedContentWriter(this.outputStream))
			{
				long num = 0L;
				long position = this.outputStream.Position;
				sharedContentWriter.Write(num);
				sharedContentWriter.Write(id);
				if (sharedContent.FileName != null)
				{
					sharedContentWriter.Write(sharedContent.FileName);
				}
				sharedContentWriter.Write(sharedContent.RawDataEntryPosition);
				sharedContentWriter.Write(sharedContent.Properties.Count);
				foreach (KeyValuePair<string, object> keyValuePair in sharedContent.Properties)
				{
					sharedContentWriter.Write(keyValuePair.Key);
					if (keyValuePair.Value == null)
					{
						sharedContentWriter.Write(0U);
					}
					else
					{
						Type type = keyValuePair.Value.GetType();
						if (type == typeof(string))
						{
							sharedContentWriter.Write(Encoding.Unicode.GetBytes((string)keyValuePair.Value));
						}
						else if (type == typeof(byte[]))
						{
							sharedContentWriter.Write((byte[])keyValuePair.Value);
						}
						else
						{
							ulong value = Convert.ToUInt64(keyValuePair.Value);
							sharedContentWriter.Write(BitConverter.GetBytes(value));
						}
					}
				}
				num = this.outputStream.Position - position - 8L;
				this.outputStream.Position = position;
				sharedContentWriter.Write(num);
				this.outputStream.Position += num;
			}
		}

		private void WriteHeader(SharedContentWriter writer, string shareName)
		{
			byte[] bytes = Encoding.ASCII.GetBytes("PPMAIL01");
			writer.Write(bytes, 0, bytes.Length);
			long value = 4L + SharedContentWriter.ComputeLength(shareName);
			writer.Write(value);
			writer.Write(UnifiedContentSerializer.EntryId.PpeHeader);
			writer.Write(shareName);
		}

		private readonly SharedStream sharedStream;

		private readonly Stream outputStream;

		private readonly List<UnifiedContentSerializer.HeaderInfo> headerInfoList = new List<UnifiedContentSerializer.HeaderInfo>();

		private List<IExtractedContent> contentList;

		private List<IExtractedContent> uncommittedContent = new List<IExtractedContent>();

		public enum EntryId
		{
			PpeHeader,
			Property,
			Body,
			Attachment,
			File,
			Stream
		}

		public enum PropertyId
		{
			Subject = 1
		}

		internal struct HeaderInfo
		{
			public HeaderInfo(UnifiedContentSerializer.EntryId id, SharedContent content)
			{
				this.Id = id;
				this.Content = content;
			}

			public UnifiedContentSerializer.EntryId Id;

			public SharedContent Content;
		}
	}
}
