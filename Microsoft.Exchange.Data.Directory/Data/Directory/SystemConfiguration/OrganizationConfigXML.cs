using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "OrgConfig")]
	[Serializable]
	public sealed class OrganizationConfigXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "DefaultMovePriority")]
		public int DefaultMovePriority { get; set; }

		[XmlElement(ElementName = "UpgradeConstraints")]
		public UpgradeConstraintArray UpgradeConstraints { get; set; }

		[XmlElement(ElementName = "UpgradeMessage")]
		public string UpgradeMessage { get; set; }

		[XmlElement(ElementName = "UpgradeDetails")]
		public string UpgradeDetails { get; set; }

		[XmlElement(ElementName = "PreviousMailboxRelease")]
		public string PreviousMailboxRelease { get; set; }

		[XmlElement(ElementName = "PilotMailboxRelease")]
		public string PilotMailboxRelease { get; set; }

		[XmlElement(ElementName = "U14MbxC")]
		public int? UpgradeE14MbxCountForCurrentStage { get; set; }

		[XmlElement(ElementName = "U14RequestC")]
		public int? UpgradeE14RequestCountForCurrentStage { get; set; }

		[XmlElement(ElementName = "UE14CUTime")]
		public DateTime? UpgradeLastE14CountsUpdateTime { get; set; }

		[XmlIgnore]
		public UpgradeStage? UpgradeStage { get; set; }

		[XmlElement(ElementName = "UStageInt")]
		public int? UpgradeStageInt
		{
			get
			{
				UpgradeStage? upgradeStage = this.UpgradeStage;
				if (upgradeStage == null)
				{
					return null;
				}
				return new int?((int)upgradeStage.GetValueOrDefault());
			}
			set
			{
				int? num = value;
				this.UpgradeStage = ((num != null) ? new UpgradeStage?((UpgradeStage)num.GetValueOrDefault()) : null);
			}
		}

		[XmlElement(ElementName = "UStageTime")]
		public DateTime? UpgradeStageTimeStamp { get; set; }

		[XmlElement(ElementName = "UpgradeUnitsOverride")]
		public int? UpgradeUnitsOverride { get; set; }

		[XmlElement(ElementName = "ReleaseTrack")]
		public ReleaseTrack? ReleaseTrack { get; set; }

		[XmlElement(ElementName = "UpgradeConstraintsDisabled")]
		public bool? UpgradeConstraintsDisabled { get; set; }

		[XmlElement(ElementName = "RelocationConstraints")]
		public RelocationConstraintArray RelocationConstraints { get; set; }

		[XmlElement(ElementName = "LastSuccessfulRelocationSyncStart")]
		public DateTime? LastSuccessfulRelocationSyncStart { get; set; }

		public bool ShouldSerializeUpgradeE14MbxCountForCurrentStage()
		{
			return this.UpgradeE14MbxCountForCurrentStage != null;
		}

		public bool ShouldSerializeUpgradeE14RequestCountForCurrentStage()
		{
			return this.UpgradeE14RequestCountForCurrentStage != null;
		}

		public bool ShouldSerializeUpgradeLastE14CountsUpdateTime()
		{
			return this.UpgradeLastE14CountsUpdateTime != null;
		}

		public bool ShouldSerializeUpgradeStageInt()
		{
			return this.UpgradeStageInt != null;
		}

		public bool ShouldSerializeUpgradeStageTimeStamp()
		{
			return this.UpgradeStageTimeStamp != null;
		}

		public bool ShouldSerializeUpgradeUnitsOverride()
		{
			return this.UpgradeUnitsOverride != null;
		}

		public bool ShouldSerializeUpgradeConstraintsDisabled()
		{
			return this.UpgradeConstraintsDisabled != null;
		}
	}
}
