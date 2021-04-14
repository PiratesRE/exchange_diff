using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "UserConfig")]
	[Serializable]
	public sealed class UserConfigXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "UpgradeDetails")]
		public string UpgradeDetails { get; set; }

		[XmlElement(ElementName = "UpgradeMessage")]
		public string UpgradeMessage { get; set; }

		[XmlElement(ElementName = "UpgradeStage")]
		public UpgradeStage? UpgradeStage { get; set; }

		[XmlElement(ElementName = "MailboxProvisioningConstraints")]
		public MailboxProvisioningConstraints MailboxProvisioningConstraints { get; set; }

		[XmlElement(ElementName = "UpgradeStageTimeStamp")]
		public DateTime? UpgradeStageTimeStamp { get; set; }

		[XmlElement(ElementName = "RelocationLastWriteTime")]
		public DateTime RelocationLastWriteTime { get; set; }

		[XmlElement(ElementName = "ReleaseTrack")]
		public ReleaseTrack? ReleaseTrack { get; set; }

		[XmlElement(ElementName = "RelocationShadowPropMetaData")]
		public PropertyMetaData[] RelocationShadowPropMetaData { get; set; }

		public bool ShouldSerializeUpgradeStage()
		{
			return this.UpgradeStage != null;
		}

		public bool ShouldSerializeReleaseTrack()
		{
			return this.ReleaseTrack != null;
		}

		public bool ShouldSerializeUpgradeStageTimeStamp()
		{
			return this.UpgradeStageTimeStamp != null;
		}
	}
}
