using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMTypes
{
	[XmlType(TypeName = "StringWithVersionType", Namespace = "HMTYPES:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class StringWithVersionType
	{
		[XmlIgnore]
		public int version
		{
			get
			{
				return this.internalversion;
			}
			set
			{
				this.internalversion = value;
				this.internalversionSpecified = true;
			}
		}

		[XmlIgnore]
		public string Value
		{
			get
			{
				return this.internalValue;
			}
			set
			{
				this.internalValue = value;
			}
		}

		[XmlAttribute(AttributeName = "version", Form = XmlSchemaForm.Unqualified, DataType = "int", Namespace = "HMTYPES:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalversion;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalversionSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlText(DataType = "string")]
		public string internalValue;
	}
}
