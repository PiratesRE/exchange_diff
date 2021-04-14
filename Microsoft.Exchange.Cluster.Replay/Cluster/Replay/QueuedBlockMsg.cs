using System;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class QueuedBlockMsg
	{
		public JET_EMITDATACTX EmitContext { get; private set; }

		public byte[] LogDataBuf { get; private set; }

		public int LogDataStartOffset { get; private set; }

		public int LogDataLength { get; private set; }

		public int CompressedLogDataLength { get; private set; }

		public QueuedBlockMsg NextMsg { get; set; }

		public bool WasProcessed { get; set; }

		public long ReadDurationInTics { get; set; }

		public long RequestAckCounter { get; set; }

		public DateTime MessageUtc { get; set; }

		public IOBuffer IOBuffer { get; set; }

		public QueuedBlockMsg(JET_EMITDATACTX emitCtx, byte[] logDataBuf, int logDataStartOffset, int compressedLogDataLength)
		{
			this.EmitContext = emitCtx;
			this.LogDataBuf = logDataBuf;
			this.LogDataStartOffset = logDataStartOffset;
			this.LogDataLength = (int)emitCtx.cbLogData;
			this.CompressedLogDataLength = compressedLogDataLength;
		}

		public int GetMessageSize()
		{
			return this.LogDataLength + 50;
		}
	}
}
