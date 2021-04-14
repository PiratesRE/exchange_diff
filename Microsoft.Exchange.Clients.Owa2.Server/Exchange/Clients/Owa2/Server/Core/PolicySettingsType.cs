using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PolicySettingsType
	{
		public OutboundCharsetOptions OutboundCharset
		{
			get
			{
				return this.outboundCharset;
			}
			set
			{
				this.outboundCharset = value;
			}
		}

		[DataMember(Name = "OutboundCharset")]
		public string OutboundCharsetString
		{
			get
			{
				return this.outboundCharset.ToString();
			}
			set
			{
				this.outboundCharset = (OutboundCharsetOptions)Enum.Parse(typeof(OutboundCharsetOptions), value);
			}
		}

		[DataMember]
		public bool UseGB18030
		{
			get
			{
				return this.useGB18030;
			}
			set
			{
				this.useGB18030 = value;
			}
		}

		[DataMember]
		public bool UseISO885915
		{
			get
			{
				return this.useISO885915;
			}
			set
			{
				this.useISO885915 = value;
			}
		}

		public InstantMessagingTypeOptions InstantMessagingType
		{
			get
			{
				return this.instantMessagingType;
			}
			set
			{
				this.instantMessagingType = value;
			}
		}

		[DataMember(Name = "InstantMessagingType")]
		public string InstantMessagingTypeString
		{
			get
			{
				return this.instantMessagingType.ToString();
			}
			set
			{
				this.instantMessagingType = (InstantMessagingTypeOptions)Enum.Parse(typeof(InstantMessagingTypeOptions), value);
			}
		}

		[DataMember]
		public string DefaultTheme
		{
			get
			{
				return this.defaultTheme;
			}
			set
			{
				this.defaultTheme = value;
			}
		}

		[DataMember(Name = "PlacesEnabled")]
		public bool PlacesEnabled
		{
			get
			{
				return this.placesEnabled;
			}
			set
			{
				this.placesEnabled = value;
			}
		}

		[DataMember(Name = "WeatherEnabled")]
		public bool WeatherEnabled
		{
			get
			{
				return this.weatherEnabled;
			}
			set
			{
				this.weatherEnabled = value;
			}
		}

		[DataMember(Name = "AllowCopyContactsToDeviceAddressBook")]
		public bool AllowCopyContactsToDeviceAddressBook
		{
			get
			{
				return this.allowCopyContactsToDeviceAddressBook;
			}
			set
			{
				this.allowCopyContactsToDeviceAddressBook = value;
			}
		}

		public AllowOfflineOnEnum AllowOfflineOn
		{
			get
			{
				return this.allowOfflineOn;
			}
			set
			{
				this.allowOfflineOn = value;
			}
		}

		[DataMember(Name = "AllowOfflineOn")]
		public string AllowOfflineOnString
		{
			get
			{
				return this.allowOfflineOn.ToString();
			}
			set
			{
				this.allowOfflineOn = (AllowOfflineOnEnum)Enum.Parse(typeof(AllowOfflineOnEnum), value);
			}
		}

		[DataMember]
		public string MySiteUrl { get; set; }

		private OutboundCharsetOptions outboundCharset;

		private bool useGB18030;

		private bool useISO885915;

		private InstantMessagingTypeOptions instantMessagingType;

		private string defaultTheme;

		private bool placesEnabled;

		private bool weatherEnabled;

		private bool allowCopyContactsToDeviceAddressBook;

		private AllowOfflineOnEnum allowOfflineOn;
	}
}
