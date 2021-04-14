using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract]
	[XmlType(TypeName = "AggregatedAccountType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(ConnectionSettingsInfo))]
	[XmlInclude(typeof(ConnectionSettingsInfo))]
	[Serializable]
	public class AggregatedAccountType
	{
		public AggregatedAccountType()
		{
		}

		internal AggregatedAccountType(Guid mailboxGuid, Guid subscriptionGuid, string emailAddress, string userName, ConnectionSettingsInfo connectionSettings)
		{
			this.MailboxGuid = mailboxGuid;
			this.SubscriptionGuid = subscriptionGuid;
			this.EmailAddress = emailAddress;
			this.UserName = userName;
			this.ConnectionSettings = connectionSettings;
		}

		[XmlElement("MailboxGuid")]
		[DataMember]
		public Guid MailboxGuid { get; set; }

		[DataMember]
		[XmlElement("SubscriptionGuid")]
		public Guid SubscriptionGuid { get; set; }

		[DataMember]
		[XmlElement("EmailAddress")]
		public string EmailAddress { get; set; }

		[XmlElement("UserName")]
		[DataMember]
		public string UserName { get; set; }

		[XmlElement("ConnectionSettings")]
		[DataMember]
		public ConnectionSettingsInfo ConnectionSettings { get; set; }
	}
}
