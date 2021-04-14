using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MsaSettings : ItemPropertiesBase
	{
		public override void Apply(MrsPSHandler psHandler, MailboxSession mailboxSession)
		{
			using (MonadCommand command = psHandler.GetCommand(MrsCmdlet.SetConsumerMailbox))
			{
				int num = 1;
				command.Parameters.AddWithValue("Identity", ConsumerMailboxIdParameter.Parse(mailboxSession.MailboxGuid.ToString()));
				if (this.LastName != null)
				{
					command.Parameters.AddWithValue("LastName", this.LastName);
				}
				if (this.FirstName != null)
				{
					command.Parameters.AddWithValue("FirstName", this.FirstName);
				}
				if (this.BirthdayInt != 0)
				{
					command.Parameters.AddWithValue("Birthdate", this.BirthdayInt);
					command.Parameters.AddWithValue("BirthdayPrecision", this.BirthdayPrecision);
				}
				if (command.Parameters.Count > num)
				{
					command.Execute();
				}
			}
		}

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string MailDomain { get; set; }

		[DataMember]
		public string LanguageCode { get; set; }

		[DataMember]
		public string CountryCode { get; set; }

		[DataMember]
		public int GeoIdInt { get; set; }

		[DataMember]
		public ushort GenderCodeUInt16 { get; set; }

		[DataMember]
		public ushort OccupationCodeUInt16 { get; set; }

		[DataMember]
		public string ZipCode { get; set; }

		[DataMember]
		public string TimeZone { get; set; }

		[DataMember]
		public string LcidString { get; set; }

		[DataMember]
		public int ProfileVersionInt { get; set; }

		[DataMember]
		public int MiscFlagsInt { get; set; }

		[DataMember]
		public int FlagsInt { get; set; }

		[DataMember]
		public int Accessibilty { get; set; }

		[DataMember]
		public ushort BirthdayPrecision { get; set; }

		[DataMember]
		public int BirthdayInt { get; set; }

		[DataMember]
		public int Age { get; set; }
	}
}
