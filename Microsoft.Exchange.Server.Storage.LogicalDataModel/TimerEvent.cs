using System;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal class TimerEvent
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

		internal int DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		internal Property Prop
		{
			get
			{
				return this.prop;
			}
		}

		internal static bool IsValidTimerDateTime(DateTime? eventTime)
		{
			return eventTime != null && !(eventTime.Value >= TimerEventHandler.DateTimeMax);
		}

		internal static byte[] SerializeExtraData(int documentId, Property prop)
		{
			uint[] array = new uint[2];
			object[] array2 = new object[2];
			array[0] = PropTag.Message.DocumentId.PropTag;
			array2[0] = documentId;
			array[1] = prop.Tag.PropTag;
			array2[1] = prop.Value;
			return PropertyBlob.BuildBlob(array, array2);
		}

		internal static bool DeserializeExtraData(byte[] blob, out int documentId, out Property prop)
		{
			documentId = 0;
			prop = new Property(StorePropTag.Invalid, null);
			if (blob == null)
			{
				ExTraceGlobals.EventsTracer.TraceError(58140L, "No extra data in the property blob");
				return false;
			}
			PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(blob, 0);
			if (blobReader.PropertyCount < 2)
			{
				ExTraceGlobals.EventsTracer.TraceError(33564L, "No property data in the property blob");
				return false;
			}
			int index;
			int index2;
			if (blobReader.GetPropertyTag(0) == PropTag.Message.DocumentId.PropTag)
			{
				index = 0;
				index2 = 1;
			}
			else
			{
				if (blobReader.GetPropertyTag(1) != PropTag.Message.DocumentId.PropTag)
				{
					ExTraceGlobals.EventsTracer.TraceError(49948L, "Unexpected property in the property blob");
					return false;
				}
				index = 1;
				index2 = 0;
			}
			StorePropTag tag = StorePropTag.CreateWithoutInfo(blobReader.GetPropertyTag(index2), ObjectType.Message);
			if (tag.PropType != PropertyType.SysTime)
			{
				ExTraceGlobals.EventsTracer.TraceError(48316L, "Unexpected property type in the property blob");
				return false;
			}
			documentId = (int)blobReader.GetPropertyValue(index);
			prop = new Property(tag, blobReader.GetPropertyValue(index2));
			return true;
		}

		internal TimerEvent(DateTime eventTime, int mailboxNumber, int documentId, Property prop)
		{
			this.eventTime = eventTime;
			this.mailboxNumber = mailboxNumber;
			this.documentId = documentId;
			this.prop = prop;
		}

		private readonly DateTime eventTime;

		private readonly int mailboxNumber;

		private readonly int documentId;

		private readonly Property prop;

		internal enum EventType
		{
			None,
			TimerEvent
		}
	}
}
