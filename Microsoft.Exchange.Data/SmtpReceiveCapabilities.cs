using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum SmtpReceiveCapabilities
	{
		None = 0,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptOorgHeader)]
		AcceptOorgHeader = 1,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptOorgProtocol)]
		AcceptOorgProtocol = 2,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptProxyProtocol)]
		AcceptProxyProtocol = 4,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptProxyFromProtocol)]
		AcceptProxyFromProtocol = 8,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptProxyToProtocol)]
		AcceptProxyToProtocol = 16,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptXAttrProtocol)]
		AcceptXAttrProtocol = 32,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptOrgHeaders)]
		AcceptOrgHeaders = 64,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptXSysProbeProtocol)]
		AcceptXSysProbeProtocol = 128,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptCrossForestMail)]
		AcceptCrossForestMail = 256,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptCloudServicesMail)]
		AcceptCloudServicesMail = 512,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAllowSubmit)]
		AllowSubmit = 1024,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAcceptXOriginalFromProtocol)]
		AcceptXOriginalFromProtocol = 2048,
		[LocDescription(DataStrings.IDs.SmtpReceiveCapabilitiesAllowConsumerMail)]
		AllowConsumerMail = 4096
	}
}
