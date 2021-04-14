using System;

namespace Microsoft.Exchange.Rpc.ProcessAccess
{
	internal unsafe delegate void RunProcessCommandDelegate(void* hBinding, int inBytesLen, byte* pInBytes, int* pOutBytesLen, byte** ppOutBytes);
}
