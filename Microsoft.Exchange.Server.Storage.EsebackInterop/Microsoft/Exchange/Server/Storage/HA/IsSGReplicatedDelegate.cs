using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal unsafe delegate int IsSGReplicatedDelegate(_ESEBACK_CONTEXT* pContext, ulong jetinst, int* pfReplicated, uint cbSGGuid, ushort* wszSGGuid, uint* pcInfo, _LOGSHIP_INFO** prgInfo);
}
