using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[XmlType(TypeName = "ReportAsSpamTypeOptions", Namespace = "ItemOperations:")]
	[Serializable]
	public class ReportAsSpamTypeOptions
	{
		[XmlIgnore]
		public ReportAsSpamTypeOptionsReport Report
		{
			get
			{
				if (this.internalReport == null)
				{
					this.internalReport = new ReportAsSpamTypeOptionsReport();
				}
				return this.internalReport;
			}
			set
			{
				this.internalReport = value;
			}
		}

		[XmlIgnore]
		public BounceMessage BounceMessage
		{
			get
			{
				if (this.internalBounceMessage == null)
				{
					this.internalBounceMessage = new BounceMessage();
				}
				return this.internalBounceMessage;
			}
			set
			{
				this.internalBounceMessage = value;
			}
		}

		[XmlIgnore]
		public UnsubscribeFromMailingList UnsubscribeFromMailingList
		{
			get
			{
				if (this.internalUnsubscribeFromMailingList == null)
				{
					this.internalUnsubscribeFromMailingList = new UnsubscribeFromMailingList();
				}
				return this.internalUnsubscribeFromMailingList;
			}
			set
			{
				this.internalUnsubscribeFromMailingList = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ReportAsSpamTypeOptionsReport), ElementName = "Report", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public ReportAsSpamTypeOptionsReport internalReport;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(BounceMessage), ElementName = "BounceMessage", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public BounceMessage internalBounceMessage;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(UnsubscribeFromMailingList), ElementName = "UnsubscribeFromMailingList", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public UnsubscribeFromMailingList internalUnsubscribeFromMailingList;
	}
}
