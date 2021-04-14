using System;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal class SettingsBase
	{
		public SettingsBase(XmlNode request, XmlNode response, ProtocolLogger protocolLogger)
		{
			this.request = request;
			this.response = response;
			this.protocolLogger = protocolLogger;
		}

		public XmlNode Request
		{
			get
			{
				return this.request;
			}
		}

		public XmlNode Response
		{
			get
			{
				return this.response;
			}
		}

		public ProtocolLogger ProtocolLogger
		{
			get
			{
				return this.protocolLogger;
			}
		}

		public virtual void Execute()
		{
			this.response = null;
		}

		private XmlNode request;

		private XmlNode response;

		private ProtocolLogger protocolLogger;

		protected enum ErrorCode
		{
			Success = 1,
			ProtocolError,
			AccessDenied,
			ServerUnavailable,
			InvalidArguments,
			ConflictingArguments,
			DeniedByPolicy
		}
	}
}
