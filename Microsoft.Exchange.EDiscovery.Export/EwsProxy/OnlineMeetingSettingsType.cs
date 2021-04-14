using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class OnlineMeetingSettingsType
	{
		public LobbyBypassType LobbyBypass
		{
			get
			{
				return this.lobbyBypassField;
			}
			set
			{
				this.lobbyBypassField = value;
			}
		}

		public OnlineMeetingAccessLevelType AccessLevel
		{
			get
			{
				return this.accessLevelField;
			}
			set
			{
				this.accessLevelField = value;
			}
		}

		public PresentersType Presenters
		{
			get
			{
				return this.presentersField;
			}
			set
			{
				this.presentersField = value;
			}
		}

		private LobbyBypassType lobbyBypassField;

		private OnlineMeetingAccessLevelType accessLevelField;

		private PresentersType presentersField;
	}
}
