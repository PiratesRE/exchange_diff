using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "OfflineAddressBookLastGeneratingData")]
	public class OfflineAddressBookLastGeneratingData : XMLSerializableBase
	{
		[XmlElement(ElementName = "MailboxGuid")]
		public Guid? MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
			set
			{
				this.mailboxGuid = value;
			}
		}

		[XmlElement(ElementName = "DatabaseGuid")]
		public Guid? DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
			set
			{
				this.databaseGuid = value;
			}
		}

		[XmlElement(ElementName = "ServerFqdn")]
		public string ServerFqdn
		{
			get
			{
				return this.serverFqdn;
			}
			set
			{
				this.serverFqdn = value;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"MailboxGuid: ",
				(this.mailboxGuid != null) ? this.mailboxGuid.ToString() : "",
				"; DatabaseGuid: ",
				(this.databaseGuid != null) ? this.databaseGuid.ToString() : "",
				"; Server: ",
				this.serverFqdn
			});
		}

		private Guid? mailboxGuid;

		private Guid? databaseGuid;

		private string serverFqdn;
	}
}
