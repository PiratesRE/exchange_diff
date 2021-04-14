using System;
using System.IO;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BufferedReceiver : DisposableWrapper<IDataImport>, IDataImport, IDisposable
	{
		public BufferedReceiver(IDataImport destination, bool ownsDestination, bool useBuffering, bool useCompression) : base(destination, ownsDestination)
		{
			this.useBuffering = useBuffering;
			this.useCompression = useCompression;
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			if (!this.useBuffering)
			{
				base.WrappedObject.SendMessage(message);
				return;
			}
			BufferBatchMessage bufferBatchMessage = message as BufferBatchMessage;
			if (bufferBatchMessage == null)
			{
				base.WrappedObject.SendMessage(message);
				return;
			}
			if (bufferBatchMessage.Buffer != null)
			{
				byte[] buffer;
				if (this.useCompression)
				{
					buffer = CommonUtils.DecompressData(bufferBatchMessage.Buffer);
				}
				else
				{
					buffer = bufferBatchMessage.Buffer;
				}
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						while (memoryStream.Position < memoryStream.Length)
						{
							int opcode = CommonUtils.ReadInt(binaryReader);
							byte[] data = CommonUtils.ReadBlob(binaryReader);
							IDataMessage message2 = DataMessageSerializer.Deserialize(opcode, data, this.useCompression);
							base.WrappedObject.SendMessage(message2);
						}
					}
				}
			}
			if (bufferBatchMessage.FlushAfterImport)
			{
				base.WrappedObject.SendMessageAndWaitForReply(FlushMessage.Instance);
			}
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			return base.WrappedObject.SendMessageAndWaitForReply(message);
		}

		private bool useBuffering;

		private bool useCompression;
	}
}
