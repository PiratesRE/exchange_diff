using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class UpgradeConstraint : XMLSerializableBase, IComparable<UpgradeConstraint>
	{
		public UpgradeConstraint() : this(null, null, DateTime.MinValue)
		{
		}

		public UpgradeConstraint(string name, string reason) : this(name, reason, DateTime.MinValue)
		{
		}

		public UpgradeConstraint(string name, string reason, DateTime expirationDate)
		{
			this.Name = name;
			this.Reason = reason;
			this.ExpirationDate = expirationDate;
		}

		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "Reason")]
		public string Reason { get; set; }

		[XmlAttribute(AttributeName = "ExpirationDate")]
		public DateTime ExpirationDate { get; set; }

		int IComparable<UpgradeConstraint>.CompareTo(UpgradeConstraint other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = StringComparer.OrdinalIgnoreCase.Compare(this.Name, other.Name);
			if (num != 0)
			{
				return num;
			}
			num = StringComparer.OrdinalIgnoreCase.Compare(this.Reason, other.Reason);
			if (num != 0)
			{
				return num;
			}
			return DateTime.Compare(this.ExpirationDate, other.ExpirationDate);
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", this.Name, this.Reason, this.ExpirationDate);
		}
	}
}
