using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest
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
		public GetChanges GetChanges
		{
			get
			{
				if (this.internalGetChanges == null)
				{
					this.internalGetChanges = new GetChanges();
				}
				return this.internalGetChanges;
			}
			set
			{
				this.internalGetChanges = value;
			}
		}

		[XmlIgnore]
		public int WindowSize
		{
			get
			{
				return this.internalWindowSize;
			}
			set
			{
				this.internalWindowSize = value;
				this.internalWindowSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public string CollectionId
		{
			get
			{
				return this.internalCollectionId;
			}
			set
			{
				this.internalCollectionId = value;
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

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Class", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "AirSync:")]
		public string internalClass;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "SyncKey", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "AirSync:")]
		public string internalSyncKey;

		[XmlElement(Type = typeof(GetChanges), ElementName = "GetChanges", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public GetChanges internalGetChanges;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "WindowSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "AirSync:")]
		public int internalWindowSize;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalWindowSizeSpecified;

		[XmlElement(ElementName = "CollectionId", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalCollectionId;

		[XmlElement(Type = typeof(Commands), ElementName = "Commands", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Commands internalCommands;
	}
}
