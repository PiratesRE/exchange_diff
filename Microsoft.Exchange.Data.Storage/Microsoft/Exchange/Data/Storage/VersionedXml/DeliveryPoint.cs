using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class DeliveryPoint
	{
		internal static IList<DeliveryPoint> GetPersonToPersonPreferences(IList<DeliveryPoint> candidates)
		{
			List<DeliveryPoint> list = new List<DeliveryPoint>(candidates.Count);
			foreach (DeliveryPoint deliveryPoint in candidates)
			{
				if (deliveryPoint.Ready && -1 != deliveryPoint.P2pMessagingPriority)
				{
					list.Add(deliveryPoint);
				}
			}
			list.Sort((DeliveryPoint x, DeliveryPoint y) => x.P2pMessagingPriority.CompareTo(y.P2pMessagingPriority));
			return new ReadOnlyCollection<DeliveryPoint>(list);
		}

		internal static IList<DeliveryPoint> GetMachineToPersonPreferences(IList<DeliveryPoint> candidates)
		{
			List<DeliveryPoint> list = new List<DeliveryPoint>(candidates.Count);
			foreach (DeliveryPoint deliveryPoint in candidates)
			{
				if (deliveryPoint.Ready && -1 != deliveryPoint.M2pMessagingPriority)
				{
					list.Add(deliveryPoint);
				}
			}
			list.Sort((DeliveryPoint x, DeliveryPoint y) => x.M2pMessagingPriority.CompareTo(y.M2pMessagingPriority));
			return new ReadOnlyCollection<DeliveryPoint>(list);
		}

		public DeliveryPoint()
		{
		}

		public DeliveryPoint(byte identity, DeliveryPointType type, E164Number phonenumber, string protocol, string deviceType, string deviceId, string deviceFriendlyName, int p2pMessagingPriority, int m2pMessagingPriority)
		{
			this.Identity = identity;
			this.Type = type;
			this.PhoneNumber = phonenumber;
			this.Protocol = protocol;
			this.DeviceType = deviceType;
			this.DeviceId = deviceId;
			this.DeviceFriendlyName = deviceFriendlyName;
			this.P2pMessagingPriority = p2pMessagingPriority;
			this.M2pMessagingPriority = m2pMessagingPriority;
		}

		[XmlElement("Identity")]
		public byte Identity { get; set; }

		[XmlElement("Type")]
		public DeliveryPointType Type { get; set; }

		[XmlElement("PhoneNumber")]
		public E164Number PhoneNumber { get; set; }

		[XmlElement("Protocol")]
		public string Protocol { get; set; }

		[XmlElement("DeviceType")]
		public string DeviceType { get; set; }

		[XmlElement("DeviceId")]
		public string DeviceId { get; set; }

		[XmlElement("DeviceFriendlyName")]
		public string DeviceFriendlyName { get; set; }

		[XmlElement("P2pMessaginPriority")]
		public int P2pMessagingPriority { get; set; }

		[XmlElement("M2pMessagingPriority")]
		public int M2pMessagingPriority { get; set; }

		[XmlIgnore]
		public bool Ready
		{
			get
			{
				switch (this.Type)
				{
				case DeliveryPointType.Unknown:
					return false;
				case DeliveryPointType.ExchangeActiveSync:
					return null != this.PhoneNumber;
				case DeliveryPointType.SmtpToSmsGateway:
					return true;
				default:
					return false;
				}
			}
		}

		internal const int PriorityDisabled = -1;
	}
}
