using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PagedTransmitter : IDataExport
	{
		public PagedTransmitter(string data, bool useCompression)
		{
			this.Initialize(CommonUtils.PackString(data, useCompression));
		}

		public PagedTransmitter(byte[] data, bool useCompression)
		{
			this.Initialize(useCompression ? CommonUtils.CompressData(data) : data);
		}

		DataExportBatch IDataExport.ExportData()
		{
			if (this.curIndex == -1)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			DataExportBatch dataExportBatch = new DataExportBatch();
			if (this.dataToSend == null || this.curIndex + this.chunkSize >= this.dataToSend.Length)
			{
				dataExportBatch.Opcode = 211;
				if (this.dataToSend == null || this.curIndex == 0)
				{
					dataExportBatch.Data = this.dataToSend;
				}
				else
				{
					int num = this.dataToSend.Length - this.curIndex;
					dataExportBatch.Data = new byte[num];
					Array.Copy(this.dataToSend, this.curIndex, dataExportBatch.Data, 0, num);
				}
				dataExportBatch.IsLastBatch = true;
				this.curIndex = -1;
			}
			else
			{
				dataExportBatch.Opcode = 210;
				dataExportBatch.Data = new byte[this.chunkSize];
				Array.Copy(this.dataToSend, this.curIndex, dataExportBatch.Data, 0, this.chunkSize);
				this.curIndex += this.chunkSize;
			}
			return dataExportBatch;
		}

		void IDataExport.CancelExport()
		{
			this.curIndex = -1;
		}

		private void Initialize(byte[] dataToSend)
		{
			this.dataToSend = dataToSend;
			this.chunkSize = 1000000;
			this.curIndex = 0;
		}

		private const int DefaultChunkSize = 1000000;

		private byte[] dataToSend;

		private int curIndex;

		private int chunkSize;
	}
}
