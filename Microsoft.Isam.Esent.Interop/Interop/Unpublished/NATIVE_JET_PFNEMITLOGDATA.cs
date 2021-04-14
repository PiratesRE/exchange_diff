using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal delegate JET_err NATIVE_JET_PFNEMITLOGDATA(IntPtr instance, ref NATIVE_EMITDATACTX pEmitLogDataCtx, IntPtr pvLogData, uint cbLogData, IntPtr callbackCtx);
}
