using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ClientControlFlags : uint
	{
		None = 0U,
		EnablePerfSendToServer = 1U,
		EnablePerfSendToMailbox = 2U,
		EnableCompression = 4U,
		EnableHttpTunneling = 8U,
		EnablePerfSendGcData = 16U
	}
}
