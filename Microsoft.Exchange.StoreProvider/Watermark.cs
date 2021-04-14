using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class Watermark
	{
		internal Watermark(Guid mailboxGuid, long counter)
		{
			this.mailboxGuid = mailboxGuid;
			this.consumerGuid = Guid.Empty;
			this.eventCounter = counter;
		}

		internal unsafe Watermark(IntPtr watermark)
		{
			WatermarkNative* ptr = (WatermarkNative*)watermark.ToPointer();
			this.mailboxGuid = ptr->mailboxGuid;
			this.consumerGuid = ptr->consumerGuid;
			this.eventCounter = ptr->llEventCounter;
		}

		public static Watermark CreateLowestMark(Guid mailboxGuid)
		{
			return new Watermark(mailboxGuid, 0L);
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public Guid ConsumerGuid
		{
			get
			{
				return this.consumerGuid;
			}
		}

		public long EventCounter
		{
			get
			{
				return this.eventCounter;
			}
		}

		public static Watermark GetMailboxWatermark(Guid mailboxGuid, long counter)
		{
			if (mailboxGuid == Guid.Empty)
			{
				throw MapiExceptionHelper.ArgumentException("mailboxGuid", "cannot be an empty GUID.");
			}
			return new Watermark(mailboxGuid, counter);
		}

		public static Watermark GetDatabaseWatermark(long counter)
		{
			return new Watermark(Guid.Empty, counter);
		}

		internal static Watermark[] Unmarshal(SafeExMemoryHandle ptrWatermarksNative, int countWatermarks)
		{
			Watermark[] array = new Watermark[countWatermarks];
			if (countWatermarks > 0)
			{
				IntPtr intPtr = ptrWatermarksNative.DangerousGetHandle();
				for (int i = 0; i < countWatermarks; i++)
				{
					array[i] = new Watermark(intPtr);
					intPtr = (IntPtr)((long)intPtr + (long)WatermarkNative.SizeOf);
				}
			}
			return array;
		}

		private Guid mailboxGuid;

		private Guid consumerGuid;

		private long eventCounter;
	}
}
