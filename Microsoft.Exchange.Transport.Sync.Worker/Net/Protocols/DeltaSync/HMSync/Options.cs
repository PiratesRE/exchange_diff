using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[XmlRoot(ElementName = "Options", Namespace = "HMSYNC:", IsNullable = false)]
	[Serializable]
	public class Options
	{
		[XmlIgnore]
		public byte Conflict
		{
			get
			{
				return this.internalConflict;
			}
			set
			{
				this.internalConflict = value;
				this.internalConflictSpecified = true;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Conflict", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "HMSYNC:")]
		public byte internalConflict;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalConflictSpecified;
	}
}
