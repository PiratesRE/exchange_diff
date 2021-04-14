using System;
using System.Net;
using System.Xml;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal interface IProvisionCommandHost
	{
		XmlNode XmlRequest { get; }

		XmlDocument XmlResponse { get; }

		ProtocolLogger ProtocolLogger { get; }

		uint? HeaderPolicyKey { get; }

		int ProtocolVersion { get; }

		IPolicyData PolicyData { get; }

		IGlobalInfo GlobalInfo { get; }

		void SetErrorResponse(HttpStatusCode httpStatusCode, StatusCode easStatusCode);

		void SendRemoteWipeConfirmationMessage(ExDateTime wipeAckTime);

		void ResetMobileServiceSelector();

		void ProcessDeviceInformationSettings(XmlNode inboundDeviceInformationNode, XmlNode provisionResponseNode);
	}
}
