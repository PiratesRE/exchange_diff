using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum MimeItemType
	{
		MimeMessageGeneric,
		MimeMessageSmime,
		MimeMessageSmimeMultipartSigned,
		MimeMessageDsn,
		MimeMessageMdn,
		MimeMessageJournalTnef,
		MimeMessageJournalMsg,
		MimeMessageSecondaryJournal,
		MimeMessageCalendar,
		MimeMessageReplication
	}
}
