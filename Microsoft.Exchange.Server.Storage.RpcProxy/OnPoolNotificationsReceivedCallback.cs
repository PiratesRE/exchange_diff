using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal delegate void OnPoolNotificationsReceivedCallback(Guid instanceId, int generation, ErrorCode errorCode, uint[] sessionHandles);
}
