using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversionResult
	{
		internal ConversionResult()
		{
		}

		internal void AddSubResult(ConversionResult subResult)
		{
			this.itemWasModified |= subResult.itemWasModified;
		}

		public bool ItemWasModified
		{
			get
			{
				return this.itemWasModified;
			}
			internal set
			{
				this.itemWasModified = value;
			}
		}

		public long BodySize
		{
			get
			{
				return this.bodySize;
			}
			internal set
			{
				this.bodySize = value;
			}
		}

		public int RecipientCount
		{
			get
			{
				return this.recipientCount;
			}
			internal set
			{
				this.recipientCount = value;
			}
		}

		public int AttachmentCount
		{
			get
			{
				return this.attachmentCount;
			}
			internal set
			{
				this.attachmentCount = value;
			}
		}

		public long AccumulatedAttachmentSize
		{
			get
			{
				return this.accumulatedAttachmentSize;
			}
			internal set
			{
				this.accumulatedAttachmentSize = value;
			}
		}

		private long bodySize;

		private int recipientCount;

		private int attachmentCount;

		private long accumulatedAttachmentSize;

		private bool itemWasModified;
	}
}
