using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class EasConnectionSettings
	{
		internal EasConnectionSettings(EasEndpointSettings easEndpointSettings, EasConnectionParameters connectionParameters, EasAuthenticationParameters authenticationParameters, EasDeviceParameters deviceParameters)
		{
			this.EasEndpointSettings = easEndpointSettings;
			this.ConnectionParameters = connectionParameters;
			this.AuthenticationParameters = authenticationParameters;
			this.DeviceParameters = deviceParameters;
		}

		internal ILog Log
		{
			get
			{
				return this.ConnectionParameters.Log;
			}
		}

		internal EasProtocolVersion EasProtocolVersion
		{
			get
			{
				return this.ConnectionParameters.EasProtocolVersion;
			}
		}

		internal string ClientLanguage
		{
			get
			{
				return this.ConnectionParameters.ClientLanguage;
			}
		}

		internal bool RequestCompression
		{
			get
			{
				return this.ConnectionParameters.RequestCompression;
			}
		}

		internal bool AcceptMultipart
		{
			get
			{
				return this.ConnectionParameters.AcceptMultipart;
			}
		}

		internal string DeviceId
		{
			get
			{
				return this.DeviceParameters.DeviceId;
			}
		}

		internal string DeviceType
		{
			get
			{
				return this.DeviceParameters.DeviceType;
			}
		}

		internal string UserAgent
		{
			get
			{
				return this.DeviceParameters.UserAgent;
			}
		}

		internal EasExtensionCapabilities ExtensionCapabilities { get; set; }

		internal EasEndpointSettings EasEndpointSettings { get; set; }

		private EasConnectionParameters ConnectionParameters { get; set; }

		private EasDeviceParameters DeviceParameters { get; set; }

		private EasAuthenticationParameters AuthenticationParameters { get; set; }
	}
}
