using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[XmlRoot(ElementName = "ItemOperations", Namespace = "ItemOperations:", IsNullable = false)]
	[Serializable]
	public class ItemOperations
	{
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
		public ItemOperationsFault Fault
		{
			get
			{
				if (this.internalFault == null)
				{
					this.internalFault = new ItemOperationsFault();
				}
				return this.internalFault;
			}
			set
			{
				this.internalFault = value;
			}
		}

		[XmlIgnore]
		public ItemOperationsAuthPolicy AuthPolicy
		{
			get
			{
				if (this.internalAuthPolicy == null)
				{
					this.internalAuthPolicy = new ItemOperationsAuthPolicy();
				}
				return this.internalAuthPolicy;
			}
			set
			{
				this.internalAuthPolicy = value;
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

		[XmlElement(ElementName = "Status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalStatus;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalStatusSpecified;

		[XmlElement(Type = typeof(ItemOperationsFault), ElementName = "Fault", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ItemOperationsFault internalFault;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ItemOperationsAuthPolicy), ElementName = "AuthPolicy", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public ItemOperationsAuthPolicy internalAuthPolicy;

		[XmlElement(Type = typeof(Responses), ElementName = "Responses", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Responses internalResponses;
	}
}
