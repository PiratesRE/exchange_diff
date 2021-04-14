using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[XmlType(TypeName = "ReportAsSpam", Namespace = "ItemOperations:")]
	[Serializable]
	public class ReportAsSpam
	{
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
		public SubResultType Report
		{
			get
			{
				if (this.internalReport == null)
				{
					this.internalReport = new SubResultType();
				}
				return this.internalReport;
			}
			set
			{
				this.internalReport = value;
			}
		}

		[XmlIgnore]
		public SubResultType BounceMessage
		{
			get
			{
				if (this.internalBounceMessage == null)
				{
					this.internalBounceMessage = new SubResultType();
				}
				return this.internalBounceMessage;
			}
			set
			{
				this.internalBounceMessage = value;
			}
		}

		[XmlIgnore]
		public SubResultType UnsubscribeFromMailingList
		{
			get
			{
				if (this.internalUnsubscribeFromMailingList == null)
				{
					this.internalUnsubscribeFromMailingList = new SubResultType();
				}
				return this.internalUnsubscribeFromMailingList;
			}
			set
			{
				this.internalUnsubscribeFromMailingList = value;
			}
		}

		[XmlElement(ElementName = "ServerId", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalServerId;

		[XmlElement(ElementName = "Status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalStatus;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalStatusSpecified;

		[XmlElement(Type = typeof(SubResultType), ElementName = "Report", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SubResultType internalReport;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(SubResultType), ElementName = "BounceMessage", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public SubResultType internalBounceMessage;

		[XmlElement(Type = typeof(SubResultType), ElementName = "UnsubscribeFromMailingList", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SubResultType internalUnsubscribeFromMailingList;
	}
}
