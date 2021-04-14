using System;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class DeferredSendEvent
	{
		internal DateTime EventTime
		{
			get
			{
				return this.eventTime;
			}
		}

		internal int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		internal long FolderId
		{
			get
			{
				return this.fid;
			}
		}

		internal long MessageId
		{
			get
			{
				return this.mid;
			}
		}

		internal static byte[] SerializeExtraData(long folderId, long messageId)
		{
			uint[] array = new uint[2];
			object[] array2 = new object[2];
			array[0] = PropTag.Message.Fid.PropTag;
			array2[0] = folderId;
			array[1] = PropTag.Message.Mid.PropTag;
			array2[1] = messageId;
			return PropertyBlob.BuildBlob(array, array2);
		}

		internal static bool DeserializeExtraData(byte[] blob, out long folderId, out long messageId)
		{
			folderId = ExchangeId.Zero.ToLong();
			messageId = ExchangeId.Zero.ToLong();
			if (blob == null)
			{
				ExTraceGlobals.DeferredSendTracer.TraceError(44360L, "No extra data in the property blob");
				return false;
			}
			PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(blob, 0);
			for (int i = 0; i < blobReader.PropertyCount; i++)
			{
				uint propertyTag = blobReader.GetPropertyTag(i);
				object propertyValue = blobReader.GetPropertyValue(i);
				if (propertyTag == PropTag.Message.Fid.PropTag)
				{
					folderId = (long)propertyValue;
				}
				else if (propertyTag == PropTag.Message.Mid.PropTag)
				{
					messageId = (long)propertyValue;
				}
				else
				{
					ExTraceGlobals.DeferredSendTracer.TraceDebug(60744L, "Unncessary properties found in the property blog");
				}
			}
			return true;
		}

		internal DeferredSendEvent(DateTime eventTime, int mailboxNumber, long fid, long mid)
		{
			this.eventTime = eventTime;
			this.mailboxNumber = mailboxNumber;
			this.fid = fid;
			this.mid = mid;
		}

		private readonly DateTime eventTime;

		private readonly int mailboxNumber;

		private readonly long fid;

		private readonly long mid;
	}
}
