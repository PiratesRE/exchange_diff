using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BufferedTransmitter : DisposableWrapper<IDataImport>, IDataImport, IDisposable
	{
		public BufferedTransmitter(IDataImport destination, int exportBufferSizeFromMrsKB, bool ownsDestination, bool useBuffering, bool useCompression) : base(destination, ownsDestination)
		{
			this.useBuffering = useBuffering;
			this.useCompression = useCompression;
			this.minBatchSize = this.GetExportBufferSize(exportBufferSizeFromMrsKB);
			this.dataBuffer = new MemoryStream();
			this.gzipStream = null;
			this.writer = null;
			this.ResetWriter();
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			if (!this.useBuffering)
			{
				base.WrappedObject.SendMessage(message);
				return;
			}
			DataMessageOpcode value;
			byte[] array;
			message.Serialize(this.useCompression, out value, out array);
			this.writer.Write((int)value);
			if (array != null)
			{
				this.writer.Write(array.Length);
				this.writer.Write(array);
				this.uncompressedSize += 8 + array.Length;
			}
			else
			{
				this.writer.Write(0);
				this.uncompressedSize += 8;
			}
			this.bufferCount++;
			if (this.gzipStream != null)
			{
				this.gzipStream.Flush();
			}
			if (this.dataBuffer.Length >= (long)this.minBatchSize)
			{
				this.FlushBuffers(false);
			}
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			if (!this.useBuffering)
			{
				return base.WrappedObject.SendMessageAndWaitForReply(message);
			}
			if (message is FlushMessage)
			{
				this.FlushBuffers(true);
				return null;
			}
			return base.WrappedObject.SendMessageAndWaitForReply(message);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (this.writer != null)
			{
				this.writer.Close();
				this.writer = null;
			}
			if (this.gzipStream != null)
			{
				this.gzipStream.Dispose();
				this.gzipStream = null;
			}
			if (this.dataBuffer != null)
			{
				this.dataBuffer.Dispose();
				this.dataBuffer = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		private void ResetWriter()
		{
			if (this.gzipStream != null)
			{
				this.gzipStream.Close();
				this.gzipStream = null;
			}
			this.dataBuffer.SetLength(0L);
			this.dataBuffer.Capacity = this.minBatchSize + this.minBatchSize / 10;
			this.dataBuffer.Position = 0L;
			this.uncompressedSize = 0;
			this.bufferCount = 0;
			Stream output;
			if (this.useCompression)
			{
				this.gzipStream = new GZipStream(this.dataBuffer, CompressionMode.Compress, true);
				output = this.gzipStream;
			}
			else
			{
				output = this.dataBuffer;
			}
			this.writer = new BinaryWriter(output);
		}

		private void FlushBuffers(bool doDestinationFlush)
		{
			if (this.gzipStream != null)
			{
				this.gzipStream.Dispose();
				this.gzipStream = null;
			}
			this.dataBuffer.Flush();
			byte[] data;
			if (this.dataBuffer.Length > 0L)
			{
				data = this.dataBuffer.ToArray();
				MrsTracer.ProxyClient.Debug("Flushing {0} bytes.", new object[]
				{
					this.dataBuffer.Length
				});
			}
			else
			{
				data = null;
			}
			this.ResetWriter();
			base.WrappedObject.SendMessage(new BufferBatchMessage(data, doDestinationFlush));
		}

		private int GetExportBufferSize(int exportBufferSizeFromMrsKB)
		{
			int num = ConfigBase<MRSConfigSchema>.GetConfig<int>("ExportBufferSizeOverrideKB");
			if (num == 0)
			{
				if (exportBufferSizeFromMrsKB > 0)
				{
					num = exportBufferSizeFromMrsKB;
				}
				else
				{
					num = ConfigBase<MRSConfigSchema>.GetConfig<int>("ExportBufferSizeKB");
				}
			}
			return num * 1024;
		}

		private readonly int minBatchSize;

		private bool useCompression;

		private bool useBuffering;

		private MemoryStream dataBuffer;

		private GZipStream gzipStream;

		private BinaryWriter writer;

		private int uncompressedSize;

		private int bufferCount;
	}
}
