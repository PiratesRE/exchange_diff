using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class RelocationConstraintArray : XMLSerializableBase
	{
		public RelocationConstraintArray() : this(null)
		{
		}

		public RelocationConstraintArray(RelocationConstraint[] array)
		{
			this.RelocationConstraints = array;
		}

		[XmlArrayItem("RelocationConstraint")]
		[XmlArray("RelocationConstraintsCollection")]
		public RelocationConstraint[] RelocationConstraints { get; set; }
	}
}
