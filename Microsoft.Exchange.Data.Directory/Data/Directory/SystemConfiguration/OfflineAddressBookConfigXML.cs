using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "OfflineAddressBookConfigXML")]
	public class OfflineAddressBookConfigXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "ManifestVersion")]
		public OfflineAddressBookManifestVersion ManifestVersion
		{
			get
			{
				return this.manifestVersion;
			}
			set
			{
				this.manifestVersion = value;
			}
		}

		[XmlElement(ElementName = "LastFailedTime")]
		public DateTime? LastFailedTime
		{
			get
			{
				return this.lastFailedTime;
			}
			set
			{
				this.lastFailedTime = value;
			}
		}

		[XmlElement(ElementName = "LastGeneratingData")]
		public OfflineAddressBookLastGeneratingData LastGeneratingData
		{
			get
			{
				return this.lastGeneratingData;
			}
			set
			{
				this.lastGeneratingData = value;
			}
		}

		private OfflineAddressBookManifestVersion manifestVersion;

		private DateTime? lastFailedTime;

		private OfflineAddressBookLastGeneratingData lastGeneratingData;
	}
}
