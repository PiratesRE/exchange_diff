using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[XmlType(TypeName = "Responses", Namespace = "AirSync:")]
	[Serializable]
	public class Responses
	{
		[XmlIgnore]
		public ResponsesChangeCollection ChangeCollection
		{
			get
			{
				if (this.internalChangeCollection == null)
				{
					this.internalChangeCollection = new ResponsesChangeCollection();
				}
				return this.internalChangeCollection;
			}
			set
			{
				this.internalChangeCollection = value;
			}
		}

		[XmlIgnore]
		public ResponsesDeleteCollection DeleteCollection
		{
			get
			{
				if (this.internalDeleteCollection == null)
				{
					this.internalDeleteCollection = new ResponsesDeleteCollection();
				}
				return this.internalDeleteCollection;
			}
			set
			{
				this.internalDeleteCollection = value;
			}
		}

		[XmlIgnore]
		public ResponsesAddCollection AddCollection
		{
			get
			{
				if (this.internalAddCollection == null)
				{
					this.internalAddCollection = new ResponsesAddCollection();
				}
				return this.internalAddCollection;
			}
			set
			{
				this.internalAddCollection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ResponsesChange), ElementName = "Change", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		public ResponsesChangeCollection internalChangeCollection;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ResponsesDelete), ElementName = "Delete", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		public ResponsesDeleteCollection internalDeleteCollection;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ResponsesAdd), ElementName = "Add", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		public ResponsesAddCollection internalAddCollection;
	}
}
