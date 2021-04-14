using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(JsonFaultResponse))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OnlineMeetingType
	{
		[DataMember]
		public ItemId ItemId { get; set; }

		[DataMember(IsRequired = false)]
		public string ConferenceId { get; set; }

		[DataMember]
		public string HelpUrl { get; set; }

		[DataMember]
		public string LegalUrl { get; set; }

		[DataMember]
		public string CustomFooterText { get; set; }

		[DataMember]
		public string ExternalDirectoryUri { get; set; }

		[DataMember]
		public string InternalDirectoryUri { get; set; }

		[DataMember]
		public string LogoUrl { get; set; }

		[DataMember]
		public string WebUrl { get; set; }

		[DataMember(IsRequired = false)]
		public DialInNumberType[] Numbers { get; set; }

		[DataMember(IsRequired = false)]
		public AcpInformationType AcpInformation { get; set; }

		public LobbyBypass LobbyBypass
		{
			get
			{
				return this.lobbyBypass;
			}
			set
			{
				this.lobbyBypass = value;
			}
		}

		[DataMember(Name = "LobbyBypass")]
		public string LobbyBypassString
		{
			get
			{
				return this.lobbyBypass.ToString();
			}
			set
			{
				this.lobbyBypass = (LobbyBypass)Enum.Parse(typeof(LobbyBypass), value);
			}
		}

		public OnlineMeetingAccessLevel AccessLevel
		{
			get
			{
				return this.accessLevel;
			}
			set
			{
				this.accessLevel = value;
			}
		}

		[DataMember(Name = "AccessLevel")]
		public string AccessLevelString
		{
			get
			{
				return this.accessLevel.ToString();
			}
			set
			{
				this.accessLevel = (OnlineMeetingAccessLevel)Enum.Parse(typeof(OnlineMeetingAccessLevel), value);
			}
		}

		public Presenters Presenters
		{
			get
			{
				return this.presenters;
			}
			set
			{
				this.presenters = value;
			}
		}

		[DataMember(Name = "Presenters")]
		public string PresentersString
		{
			get
			{
				return this.presenters.ToString();
			}
			set
			{
				this.presenters = (Presenters)Enum.Parse(typeof(Presenters), value);
			}
		}

		[DataMember]
		public string DiagnosticInfo { get; set; }

		public static OnlineMeetingType CreateFailedOnlineMeetingType(string diagnosticInfo)
		{
			return new OnlineMeetingType
			{
				DiagnosticInfo = diagnosticInfo
			};
		}

		private LobbyBypass lobbyBypass;

		private OnlineMeetingAccessLevel accessLevel;

		private Presenters presenters;
	}
}
