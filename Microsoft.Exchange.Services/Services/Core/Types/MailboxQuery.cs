using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "MailboxQuery", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "MailboxQueryType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MailboxQuery
	{
		public MailboxQuery()
		{
		}

		public MailboxQuery(string query, MailboxSearchScope[] mailboxSearchScopes)
		{
			this.query = query;
			this.mailboxSearchScopes = mailboxSearchScopes;
		}

		[XmlElement("Query")]
		[DataMember(Name = "Query", IsRequired = true)]
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

		[DataMember(Name = "MailboxSearchScopes", IsRequired = true)]
		[XmlArray(ElementName = "MailboxSearchScopes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "MailboxSearchScope", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(MailboxSearchScope))]
		public MailboxSearchScope[] MailboxSearchScopes
		{
			get
			{
				return this.mailboxSearchScopes;
			}
			set
			{
				this.mailboxSearchScopes = value;
			}
		}

		private string query;

		private MailboxSearchScope[] mailboxSearchScopes;
	}
}
