using System;

namespace Microsoft.Exchange.Security.Authorization
{
	[Flags]
	internal enum Permission : uint
	{
		None = 0U,
		SMTPSubmit = 1U,
		SMTPSubmitForMLS = 2U,
		SMTPAcceptAnyRecipient = 4U,
		SMTPAcceptAuthenticationFlag = 8U,
		SMTPAcceptAnySender = 16U,
		SMTPAcceptAuthoritativeDomainSender = 32U,
		BypassAntiSpam = 64U,
		BypassMessageSizeLimit = 128U,
		SMTPSendEXCH50 = 256U,
		SMTPAcceptEXCH50 = 512U,
		AcceptRoutingHeaders = 1024U,
		AcceptForestHeaders = 2048U,
		AcceptOrganizationHeaders = 4096U,
		SendRoutingHeaders = 8192U,
		SendForestHeaders = 16384U,
		SendOrganizationHeaders = 32768U,
		SendAs = 65536U,
		SMTPSendXShadow = 131072U,
		SMTPAcceptXShadow = 262144U,
		SMTPAcceptXProxyFrom = 524288U,
		SMTPAcceptXSessionParams = 1048576U,
		SMTPAcceptXMessageContextADRecipientCache = 2097152U,
		SMTPAcceptXMessageContextExtendedProperties = 4194304U,
		SMTPAcceptXMessageContextFastIndex = 8388608U,
		SMTPAcceptXAttr = 16777216U,
		SMTPAcceptXSysProbe = 33554432U,
		All = 67108863U
	}
}
