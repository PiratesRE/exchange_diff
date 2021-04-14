using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest
{
	[XmlType(TypeName = "Add", Namespace = "AirSync:")]
	[Serializable]
	public class Add
	{
		[XmlIgnore]
		public string ClientId
		{
			get
			{
				return this.internalClientId;
			}
			set
			{
				this.internalClientId = value;
			}
		}

		[XmlIgnore]
		public string ServerId
		{
			get
			{
				return this.internalServerId;
			}
			set
			{
				this.internalServerId = value;
			}
		}

		[XmlIgnore]
		public ApplicationDataType ApplicationData
		{
			get
			{
				if (this.internalApplicationData == null)
				{
					this.internalApplicationData = new ApplicationDataType();
				}
				return this.internalApplicationData;
			}
			set
			{
				this.internalApplicationData = value;
			}
		}

		[XmlElement(ElementName = "ClientId", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalClientId;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "ServerId", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "AirSync:")]
		public string internalServerId;

		[XmlElement(Type = typeof(ApplicationDataType), ElementName = "ApplicationData", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "AirSync:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ApplicationDataType internalApplicationData;
	}
}
