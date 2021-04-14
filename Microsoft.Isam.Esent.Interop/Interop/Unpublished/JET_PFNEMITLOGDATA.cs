using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public delegate JET_err JET_PFNEMITLOGDATA(JET_INSTANCE instance, JET_EMITDATACTX pEmitLogDataCtx, byte[] pvLogData, int cbLogData, object callbackCtx);
}
