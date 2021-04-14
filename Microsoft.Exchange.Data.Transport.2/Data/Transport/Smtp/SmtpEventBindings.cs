using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	internal class SmtpEventBindings
	{
		internal const string EventOnProcessAuthentication = "OnProcessAuthentication";

		internal const string EventOnProxyInboundMessage = "OnProxyInboundMessage";

		internal const string EventOnXSessionParamsCommand = "OnXSessionParamsCommand";

		public const string EventOnAuthCommand = "OnAuthCommand";

		public const string EventOnDataCommand = "OnDataCommand";

		public const string EventOnEhloCommand = "OnEhloCommand";

		public const string EventOnEndOfAuthentication = "OnEndOfAuthentication";

		public const string EventOnEndOfData = "OnEndOfData";

		public const string EventOnEndOfHeaders = "OnEndOfHeaders";

		public const string EventOnHeloCommand = "OnHeloCommand";

		public const string EventOnHelpCommand = "OnHelpCommand";

		public const string EventOnMailCommand = "OnMailCommand";

		public const string EventOnNoopCommand = "OnNoopCommand";

		public const string EventOnRcptCommand = "OnRcptCommand";

		public const string EventOnRcpt2Command = "OnRcpt2Command";

		public const string EventOnReject = "OnReject";

		public const string EventOnRsetCommand = "OnRsetCommand";

		public const string EventOnConnectEvent = "OnConnectEvent";

		public const string EventOnDisconnectEvent = "OnDisconnectEvent";

		public const string EventOnStartTlsCommand = "OnStartTlsCommand";

		internal static readonly string[] All = new string[]
		{
			"OnConnectEvent",
			"OnHeloCommand",
			"OnEhloCommand",
			"OnStartTlsCommand",
			"OnAuthCommand",
			"OnProcessAuthentication",
			"OnEndOfAuthentication",
			"OnXSessionParamsCommand",
			"OnMailCommand",
			"OnRcptCommand",
			"OnDataCommand",
			"OnEndOfHeaders",
			"OnProxyInboundMessage",
			"OnEndOfData",
			"OnHelpCommand",
			"OnNoopCommand",
			"OnReject",
			"OnRsetCommand",
			"OnDisconnectEvent"
		};
	}
}
