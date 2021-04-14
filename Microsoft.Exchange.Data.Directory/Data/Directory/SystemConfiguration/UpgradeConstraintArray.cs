using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class UpgradeConstraintArray : XMLSerializableBase
	{
		public UpgradeConstraintArray() : this(null)
		{
		}

		public UpgradeConstraintArray(UpgradeConstraint[] array)
		{
			this.UpgradeConstraints = array;
		}

		[XmlArray("UpgradeConstraints")]
		[XmlArrayItem("UpgradeConstraint")]
		public UpgradeConstraint[] UpgradeConstraints { get; set; }
	}
}
