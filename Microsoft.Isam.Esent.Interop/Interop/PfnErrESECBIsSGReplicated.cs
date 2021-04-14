using System;

namespace Microsoft.Isam.Esent.Interop
{
	public delegate JET_err PfnErrESECBIsSGReplicated(ESEBACK_CONTEXT pContext, JET_INSTANCE jetinst, out bool pfReplicated, out Guid wszSGGuid, out LOGSHIP_INFO[] prgInfo);
}
