using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MigrationCsvFile : EncodedFile
	{
		[DataMember]
		public string FirstSmtpAddress { get; set; }

		[DataMember]
		public int MailboxCount { get; set; }
	}
}
