using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class RelocationConstraint : XMLSerializableBase, IComparable<RelocationConstraint>
	{
		public RelocationConstraint() : this(null, DateTime.MinValue)
		{
		}

		public RelocationConstraint(RelocationConstraintType constraintType, DateTime expirationDate) : this(constraintType.ToString(), expirationDate)
		{
		}

		public RelocationConstraint(string name, DateTime expirationDate)
		{
			this.Name = name;
			this.ExpirationDate = expirationDate;
		}

		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "ExpirationDate")]
		public DateTime ExpirationDate { get; set; }

		int IComparable<RelocationConstraint>.CompareTo(RelocationConstraint other)
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
			return DateTime.Compare(this.ExpirationDate, other.ExpirationDate);
		}

		public override string ToString()
		{
			return string.Format("{0}: Expires {1}", this.Name, this.ExpirationDate);
		}
	}
}
