using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "DAGConfig")]
	[Serializable]
	public sealed class DatabaseAvailabilityGroupConfigXml : XMLSerializableBase
	{
		[XmlIgnore]
		public int? MailboxLoadBalanceRelativeCapacity { get; set; }

		[XmlAttribute("MLBRC")]
		public string MailboxLoadBalanceRelativeCapacityRaw
		{
			get
			{
				return XMLSerializableBase.GetNullableSerializationValue<int>(this.MailboxLoadBalanceRelativeCapacity);
			}
			set
			{
				this.MailboxLoadBalanceRelativeCapacity = XMLSerializableBase.GetNullableAttribute<int>(value, new XMLSerializableBase.TryParseDelegate<int>(int.TryParse));
			}
		}

		[XmlIgnore]
		public int? MailboxLoadBalanceOverloadThreshold { get; set; }

		[XmlAttribute("MLBOT")]
		public string MailboxLoadBalanceOverloadThresholdRaw
		{
			get
			{
				return XMLSerializableBase.GetNullableSerializationValue<int>(this.MailboxLoadBalanceOverloadThreshold);
			}
			set
			{
				this.MailboxLoadBalanceOverloadThreshold = XMLSerializableBase.GetNullableAttribute<int>(value, new XMLSerializableBase.TryParseDelegate<int>(int.TryParse));
			}
		}

		[XmlIgnore]
		public int? MailboxLoadBalanceMinimumBalancingThreshold { get; set; }

		[XmlAttribute("MLBMT")]
		public string MailboxLoadBalanceMinimumBalancingThresholdRaw
		{
			get
			{
				return XMLSerializableBase.GetNullableSerializationValue<int>(this.MailboxLoadBalanceMinimumBalancingThreshold);
			}
			set
			{
				this.MailboxLoadBalanceMinimumBalancingThreshold = XMLSerializableBase.GetNullableAttribute<int>(value, new XMLSerializableBase.TryParseDelegate<int>(int.TryParse));
			}
		}

		[XmlIgnore]
		public ByteQuantifiedSize? MailboxLoadBalanceMaximumEdbFileSize { get; set; }

		[XmlAttribute("MLBMFS")]
		public string MailboxLoadBalanceMaximumEdbFileSizeBytes
		{
			get
			{
				if (this.MailboxLoadBalanceMaximumEdbFileSize != null)
				{
					return this.MailboxLoadBalanceMaximumEdbFileSize.Value.ToBytes().ToString(CultureInfo.InvariantCulture);
				}
				return null;
			}
			set
			{
				this.MailboxLoadBalanceMaximumEdbFileSize = (string.IsNullOrWhiteSpace(value) ? null : new ByteQuantifiedSize?(ByteQuantifiedSize.FromBytes(ulong.Parse(value))));
			}
		}

		[XmlAttribute("MLBE")]
		public bool MailboxLoadBalanceEnabled { get; set; }
	}
}
