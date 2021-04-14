using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.SignalApiEx
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Unpacker : IUnpacker
	{
		public Item ReadItem(BinaryReader reader)
		{
			switch (reader.ReadByte())
			{
			case 0:
				return new Item(reader);
			case 1:
				return new BinaryItem(reader, this);
			case 2:
				return new ExchangeItem(reader, this);
			default:
				throw new ArgumentException("Unexpected item type");
			}
		}

		public void SetContent(Item item, Guid attachContentId)
		{
			BinaryItem binaryItem = item as BinaryItem;
			if (binaryItem != null)
			{
				byte[] data;
				if (this.itemAttach.TryGetValue(attachContentId, out data))
				{
					binaryItem.Data = data;
					return;
				}
				throw new KeyNotFoundException(string.Format("Attachment with key {0} for item {1} not found", attachContentId, item.Identifier));
			}
			else
			{
				ExchangeItem exchangeItem = item as ExchangeItem;
				if (exchangeItem == null)
				{
					throw new ArgumentException("Unexpected item type");
				}
				MessageItem item2;
				if (this.messageItemAttach.TryGetValue(attachContentId, out item2))
				{
					exchangeItem.Item = item2;
					return;
				}
				throw new KeyNotFoundException(string.Format("Attachment with key {0} for item {1} not found", attachContentId, item.Identifier));
			}
		}

		public Signal Unpack(MessageItem mailMessage)
		{
			this.itemAttach = new Dictionary<Guid, byte[]>();
			this.messageItemAttach = new Dictionary<Guid, MessageItem>();
			foreach (AttachmentHandle handle in mailMessage.AttachmentCollection.GetHandles())
			{
				using (Attachment attachment = mailMessage.AttachmentCollection.Open(handle))
				{
					StreamAttachment streamAttachment = attachment as StreamAttachment;
					if (streamAttachment != null)
					{
						byte[] value = Unpacker.ReadBytes(streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly));
						this.itemAttach.Add(new Guid(streamAttachment.FileName), value);
					}
					else
					{
						ItemAttachment itemAttachment = attachment as ItemAttachment;
						if (itemAttachment != null)
						{
							MessageItem itemAsMessage = itemAttachment.GetItemAsMessage();
							string contentId = itemAttachment.ContentId;
							if (contentId != null)
							{
								this.messageItemAttach.Add(new Guid(contentId), itemAsMessage);
							}
						}
					}
				}
			}
			return this.ReadSignal(mailMessage);
		}

		private static byte[] ReadBytes(Stream stream)
		{
			byte[] result;
			try
			{
				byte[] array = new byte[8096];
				using (MemoryStream memoryStream = new MemoryStream())
				{
					int count;
					while ((count = stream.Read(array, 0, array.Length)) > 0)
					{
						memoryStream.Write(array, 0, count);
					}
					result = memoryStream.ToArray();
				}
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
			return result;
		}

		private Signal ReadSignal(MessageItem mailMessage)
		{
			Signal result;
			using (TextReader textReader = mailMessage.Body.OpenTextReader(BodyFormat.TextPlain))
			{
				string s = textReader.ReadToEnd();
				using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(s)))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						AbstractSignalFeeder.ApiVersion apiVersion = binaryReader.ReadInt32();
						if (apiVersion != 1)
						{
							throw new ArgumentException("Unknown signal collection version " + apiVersion);
						}
						result = new SignalEx(binaryReader, this);
					}
				}
			}
			return result;
		}

		private const int BufferSize = 8096;

		private Dictionary<Guid, byte[]> itemAttach;

		private Dictionary<Guid, MessageItem> messageItemAttach;
	}
}
