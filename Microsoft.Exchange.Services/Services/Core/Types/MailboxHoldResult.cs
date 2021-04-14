using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "MailboxHoldResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "MailboxHoldResult", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MailboxHoldResult
	{
		public MailboxHoldResult()
		{
		}

		internal MailboxHoldResult(string holdId, string query, MailboxHoldStatus[] statuses)
		{
			this.holdId = holdId;
			this.query = query;
			this.statuses = statuses;
		}

		[DataMember(Name = "HoldId", IsRequired = true)]
		[XmlElement("HoldId")]
		public string HoldId
		{
			get
			{
				return this.holdId;
			}
			set
			{
				this.holdId = value;
			}
		}

		[XmlElement("Query")]
		[DataMember(Name = "Query", IsRequired = false)]
		public string Query
		{
			get
			{
				return this.query;
			}
			set
			{
				this.query = value;
			}
		}

		[DataMember(Name = "Statuses", IsRequired = false)]
		[XmlArray(ElementName = "MailboxHoldStatuses", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "MailboxHoldStatus", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(MailboxHoldStatus))]
		public MailboxHoldStatus[] Statuses
		{
			get
			{
				return this.statuses;
			}
			set
			{
				this.statuses = value;
			}
		}

		private string holdId;

		private string query;

		private MailboxHoldStatus[] statuses;
	}
}
