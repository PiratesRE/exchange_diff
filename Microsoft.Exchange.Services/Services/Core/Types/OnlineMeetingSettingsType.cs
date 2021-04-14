using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "OnlineMeetingSettingsType")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "OnlineMeetingSettingsType")]
	[Serializable]
	public class OnlineMeetingSettingsType
	{
		[IgnoreDataMember]
		[XmlAttribute("LobbyBypass")]
		public LobbyBypass LobbyBypass { get; set; }

		[DataMember(Name = "LobbyBypass", IsRequired = true, Order = 1)]
		[XmlIgnore]
		public string LobbyBypassString
		{
			get
			{
				return EnumUtilities.ToString<LobbyBypass>(this.LobbyBypass);
			}
			set
			{
				this.LobbyBypass = EnumUtilities.Parse<LobbyBypass>(value);
			}
		}

		[XmlAttribute("AccessLevel")]
		[IgnoreDataMember]
		public OnlineMeetingAccessLevel AccessLevel { get; set; }

		[DataMember(Name = "AccessLevel", IsRequired = true, Order = 2)]
		[XmlIgnore]
		public string AccessLevelString
		{
			get
			{
				return EnumUtilities.ToString<OnlineMeetingAccessLevel>(this.AccessLevel);
			}
			set
			{
				this.AccessLevel = EnumUtilities.Parse<OnlineMeetingAccessLevel>(value);
			}
		}

		[IgnoreDataMember]
		[XmlAttribute("Presenters")]
		public Presenters Presenters { get; set; }

		[DataMember(Name = "Presenters", IsRequired = true, Order = 3)]
		[XmlIgnore]
		public string PresentersString
		{
			get
			{
				return EnumUtilities.ToString<Presenters>(this.Presenters);
			}
			set
			{
				this.Presenters = EnumUtilities.Parse<Presenters>(value);
			}
		}
	}
}
