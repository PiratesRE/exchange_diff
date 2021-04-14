using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "List", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class List
	{
		[XmlIgnore]
		public string name
		{
			get
			{
				return this.internalname;
			}
			set
			{
				this.internalname = value;
			}
		}

		[XmlIgnore]
		public StatusType Set
		{
			get
			{
				if (this.internalSet == null)
				{
					this.internalSet = new StatusType();
				}
				return this.internalSet;
			}
			set
			{
				this.internalSet = value;
			}
		}

		[XmlIgnore]
		public StatusType Add
		{
			get
			{
				if (this.internalAdd == null)
				{
					this.internalAdd = new StatusType();
				}
				return this.internalAdd;
			}
			set
			{
				this.internalAdd = value;
			}
		}

		[XmlIgnore]
		public StatusType Delete
		{
			get
			{
				if (this.internalDelete == null)
				{
					this.internalDelete = new StatusType();
				}
				return this.internalDelete;
			}
			set
			{
				this.internalDelete = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.Unqualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalname;

		[XmlElement(Type = typeof(StatusType), ElementName = "Set", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public StatusType internalSet;

		[XmlElement(Type = typeof(StatusType), ElementName = "Add", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public StatusType internalAdd;

		[XmlElement(Type = typeof(StatusType), ElementName = "Delete", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public StatusType internalDelete;
	}
}
