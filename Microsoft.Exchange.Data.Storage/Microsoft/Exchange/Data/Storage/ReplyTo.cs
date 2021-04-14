using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ReplyTo : ParticipantList
	{
		internal ReplyTo(PropertyBag propertyBag) : this(propertyBag, false)
		{
		}

		internal ReplyTo(PropertyBag propertyBag, bool suppressCorruptDataException) : base(propertyBag, InternalSchema.MapiReplyToBlob, InternalSchema.MapiReplyToNames, null, suppressCorruptDataException)
		{
		}

		internal static ReplyTo CreateInstance(IStorePropertyBag storePropertyBag)
		{
			if (storePropertyBag.GetValueOrDefault<bool>(MessageItemSchema.ReplyToNamesExists, false) && storePropertyBag.GetValueOrDefault<bool>(MessageItemSchema.ReplyToBlobExists, false))
			{
				string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(InternalSchema.MapiReplyToNames, null);
				byte[] valueOrDefault2 = storePropertyBag.GetValueOrDefault<byte[]>(InternalSchema.MapiReplyToBlob, null);
				if (valueOrDefault != null && valueOrDefault2 != null)
				{
					PropertyBag propertyBag = new MemoryPropertyBag();
					propertyBag[InternalSchema.MapiReplyToNames] = valueOrDefault;
					propertyBag[InternalSchema.MapiReplyToBlob] = valueOrDefault2;
					return new ReplyTo(propertyBag);
				}
			}
			return null;
		}

		protected override void InsertItem(int index, Participant participant)
		{
			if (base.Count >= 128)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "ReplyTo exceeds the maximum number of participants");
				throw new LimitExceededException(ServerStrings.ExReplyToTooManyRecipients(128));
			}
			base.InsertItem(index, participant);
		}

		public const int MaxReplyToRecipients = 128;
	}
}
