using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[TextMessagingSettingsRoot]
	[Serializable]
	public class TextMessagingSettingsVersion1Point0 : TextMessagingSettingsBase
	{
		public TextMessagingSettingsVersion1Point0() : base(new Version(1, 0))
		{
		}

		public TextMessagingSettingsVersion1Point0(MachineToPersonMessagingPolicies m2pMessagingPolicies, IEnumerable<DeliveryPoint> deliveryPoints) : base(new Version(1, 0))
		{
			this.MachineToPersonMessagingPolicies = m2pMessagingPolicies;
			if (deliveryPoints != null)
			{
				this.DeliveryPoints = new List<DeliveryPoint>(deliveryPoints);
			}
		}

		[XmlElement("MachineToPersonMessagingPolicies")]
		public MachineToPersonMessagingPolicies MachineToPersonMessagingPolicies
		{
			get
			{
				return AccessorTemplates.DefaultConstructionPropertyGetter<MachineToPersonMessagingPolicies>(ref this.m2pMessagingPolicies);
			}
			set
			{
				this.m2pMessagingPolicies = value;
			}
		}

		[XmlElement("DeliveryPoint")]
		public List<DeliveryPoint> DeliveryPoints
		{
			get
			{
				return AccessorTemplates.ListPropertyGetter<DeliveryPoint>(ref this.deliveryPoints);
			}
			set
			{
				AccessorTemplates.ListPropertySetter<DeliveryPoint>(ref this.deliveryPoints, value);
			}
		}

		[XmlIgnore]
		public IList<DeliveryPoint> PersonToPersonPreferences
		{
			get
			{
				return DeliveryPoint.GetPersonToPersonPreferences(this.DeliveryPoints);
			}
		}

		[XmlIgnore]
		public IList<DeliveryPoint> MachineToPersonPreferences
		{
			get
			{
				return DeliveryPoint.GetMachineToPersonPreferences(this.DeliveryPoints);
			}
		}

		private MachineToPersonMessagingPolicies m2pMessagingPolicies;

		private List<DeliveryPoint> deliveryPoints;
	}
}
