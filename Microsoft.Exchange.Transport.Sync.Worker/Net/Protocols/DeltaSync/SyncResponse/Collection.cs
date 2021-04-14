using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[XmlType(TypeName = "Collection", Namespace = "AirSync:")]
	[Serializable]
	public class Collection
	{
		[XmlIgnore]
		public string Class
		{
			get
			{
				return this.internalClass;
			}
			set
			{
				this.internalClass = value;
			}
		}

		[XmlIgnore]
		public string SyncKey
		{
			get
			{
				return this.internalSyncKey;
			}
			set
			{
				this.internalSyncKey = value;
			}
		}

		[XmlIgnore]
		public int Status
		{
			get
			{
				return this.internalStatus;
			}
			set
			{
				this.internalStatus = value;
				this.internalStatusSpecified = true;
			}
		}

		[XmlIgnore]
		public Commands Commands
		{
			get
			{
				if (this.internalCommands == null)
				{
					this.internalCommands = new Commands();
				}
				return this.internalCommands;
			}
			set
			{
				this.internalCommands = value;
			}
		}

		[XmlIgnore]
		public Responses Responses
		{
			get
			{
				if (this.internalResponses == null)
				{
					this.internalResponses = new Responses();
				}
				return this.internalResponses;
			}
			set
			{
				this.internalResponses = value;
			}
		}

		[XmlIgnore]
		public MoreAvailable MoreAvailable
		{
			get
			{
				if (this.internalMoreAvailable == null)
				{
					this.internalMoreAvailable = new MoreAvailable();
				}
				return this.internalMoreAvailable;
			}
			set
			{
				this.internalMoreAvailable = value;
			}
		}

		[XmlElement(ElementName = "Class", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalClass;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "SyncKey", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "AirSync:")]
		public string internalSyncKey;

		[XmlElement(ElementName = "Status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalStatus;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalStatusSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Commands), ElementName = "Commands", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		public Commands internalCommands;

		[XmlElement(Type = typeof(Responses), ElementName = "Responses", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Responses internalResponses;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(MoreAvailable), ElementName = "MoreAvailable", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		public MoreAvailable internalMoreAvailable;
	}
}
