using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PagedReceiver : DisposeTrackableBase, IDataImport, IDisposable
	{
		public PagedReceiver(TransmissionDoneStringDelegate stringDone, bool useCompression) : this(stringDone, null, useCompression)
		{
		}

		public PagedReceiver(TransmissionDoneBlobDelegate blobDone, bool useCompression) : this(null, blobDone, useCompression)
		{
		}

		private PagedReceiver(TransmissionDoneStringDelegate stringDone, TransmissionDoneBlobDelegate blobDone, bool useCompression)
		{
			this.stringDoneDelegate = stringDone;
			this.blobDoneDelegate = blobDone;
			this.useCompression = useCompression;
			this.chunks = new List<byte[]>();
			this.totalDataSize = 0;
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			PagedDataMessage pagedDataMessage = message as PagedDataMessage;
			if (pagedDataMessage == null)
			{
				throw new InputDataIsInvalidPermanentException();
			}
			if (pagedDataMessage.Buffer != null)
			{
				this.chunks.Add(pagedDataMessage.Buffer);
				this.totalDataSize += pagedDataMessage.Buffer.Length;
			}
			if (pagedDataMessage.IsLastChunk)
			{
				byte[] array;
				if (this.chunks.Count == 0)
				{
					array = null;
				}
				else if (this.chunks.Count == 1)
				{
					array = this.chunks[0];
				}
				else
				{
					array = new byte[this.totalDataSize];
					int num = 0;
					foreach (byte[] array2 in this.chunks)
					{
						array2.CopyTo(array, num);
						num += array2.Length;
					}
				}
				if (this.blobDoneDelegate != null)
				{
					if (this.useCompression)
					{
						array = CommonUtils.DecompressData(array);
					}
					this.blobDoneDelegate(array);
					return;
				}
				string data = CommonUtils.UnpackString(array, this.useCompression);
				this.stringDoneDelegate(data);
			}
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			return null;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PagedReceiver>(this);
		}

		private TransmissionDoneStringDelegate stringDoneDelegate;

		private TransmissionDoneBlobDelegate blobDoneDelegate;

		private bool useCompression;

		private List<byte[]> chunks;

		private int totalDataSize;
	}
}
