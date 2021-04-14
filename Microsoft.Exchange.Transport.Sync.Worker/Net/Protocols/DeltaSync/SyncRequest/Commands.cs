using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest
{
	[XmlType(TypeName = "Commands", Namespace = "AirSync:")]
	[Serializable]
	public class Commands
	{
		[XmlIgnore]
		public ChangeCollection ChangeCollection
		{
			get
			{
				if (this.internalChangeCollection == null)
				{
					this.internalChangeCollection = new ChangeCollection();
				}
				return this.internalChangeCollection;
			}
			set
			{
				this.internalChangeCollection = value;
			}
		}

		[XmlIgnore]
		public DeleteCollection DeleteCollection
		{
			get
			{
				if (this.internalDeleteCollection == null)
				{
					this.internalDeleteCollection = new DeleteCollection();
				}
				return this.internalDeleteCollection;
			}
			set
			{
				this.internalDeleteCollection = value;
			}
		}

		[XmlIgnore]
		public AddCollection AddCollection
		{
			get
			{
				if (this.internalAddCollection == null)
				{
					this.internalAddCollection = new AddCollection();
				}
				return this.internalAddCollection;
			}
			set
			{
				this.internalAddCollection = value;
			}
		}

		[XmlElement(Type = typeof(Change), ElementName = "Change", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ChangeCollection internalChangeCollection;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Delete), ElementName = "Delete", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		public DeleteCollection internalDeleteCollection;

		[XmlElement(Type = typeof(Add), ElementName = "Add", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public AddCollection internalAddCollection;
	}
}
