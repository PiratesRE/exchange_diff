using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IEhloOptions
	{
		int Flags { get; set; }

		string AdvertisedFQDN { get; set; }

		IPAddress AdvertisedIPAddress { get; set; }

		bool BinaryMime { get; set; }

		bool EightBitMime { get; set; }

		bool EnhancedStatusCodes { get; set; }

		bool Dsn { get; set; }

		SizeMode Size { get; set; }

		long MaxSize { get; set; }

		ICollection<string> AuthenticationMechanisms { get; }

		bool XAdrc { get; set; }

		bool XExprops { get; set; }

		Version XExpropsVersion { get; }

		bool XFastIndex { get; set; }

		bool StartTLS { get; set; }

		bool AnonymousTLS { get; set; }

		ICollection<string> ExchangeAuthArgs { get; }

		bool Pipelining { get; set; }

		bool Chunking { get; set; }

		bool XMsgId { get; set; }

		bool Xexch50 { get; set; }

		bool XLongAddr { get; set; }

		bool XOrar { get; set; }

		bool XRDst { get; set; }

		bool XShadow { get; set; }

		bool XShadowRequest { get; set; }

		bool XOorg { get; set; }

		bool XProxy { get; set; }

		bool XProxyFrom { get; set; }

		bool XProxyTo { get; set; }

		bool XRsetProxyTo { get; set; }

		bool XSessionMdbGuid { get; set; }

		bool XAttr { get; set; }

		bool XSysProbe { get; set; }

		bool XOrigFrom { get; set; }

		ICollection<string> ExtendedCommands { get; }

		bool SmtpUtf8 { get; set; }

		bool XSessionType { get; set; }

		bool AreAnyAuthMechanismsSupported();

		SmtpResponse CreateSmtpResponse(AdrcSmtpMessageContextBlob adrcSmtpMessageContextBlobInstance, ExtendedPropertiesSmtpMessageContextBlob extendedPropertiesSmtpMessageContextBlobInstance, FastIndexSmtpMessageContextBlob fastIndexSmtpMessageContextBlobInstance);

		void SetFlags(EhloOptionsFlags flagsToSet, bool value);

		void AddAuthenticationMechanism(string mechanism, bool enabled);

		void ParseResponse(SmtpResponse response, IPAddress remoteIPAddress);

		void ParseResponse(SmtpResponse ehloResponse, IPAddress remoteIPAddress, int linesToSkip);

		void ParseHeloResponse(SmtpResponse heloResponse);

		bool MatchForClientProxySession(IEhloOptions other, out string nonCriticalNonMatchingOptions, out string criticalNonMatchingOptions);

		bool MatchForInboundProxySession(IEhloOptions other, bool proxyingBdat, out string nonMatchingOptions, out string criticalNonMatchingOptions);
	}
}
