using System;

namespace Microsoft.Exchange.Assistants
{
	internal interface IDemandJobNotification
	{
		void OnBeforeDemandJob(Guid mailboxGuid, Guid databaseGuid);
	}
}
