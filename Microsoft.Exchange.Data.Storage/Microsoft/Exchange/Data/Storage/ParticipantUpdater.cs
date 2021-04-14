using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ParticipantUpdater
	{
		public static int GetSMTPAddressesForParticipantsIfNecessary(Participant[] participants, RecipientCollection collection)
		{
			int num = 0;
			Participant.Job job = new Participant.Job(participants.Length);
			bool flag = false;
			foreach (Participant participant in participants)
			{
				Participant participant2 = (participant.RoutingType == "EX") ? participant : null;
				if (participant2 != null)
				{
					flag = true;
					num++;
				}
				job.Add(new Participant.JobItem(participant2));
			}
			if (!flag)
			{
				return num;
			}
			StoreSession session = collection.CoreItem.Session;
			ADSessionSettings adsessionSettings = Participant.BatchBuilder.GetADSessionSettings(session);
			Participant.BatchBuilder.Execute(job, new Participant.BatchBuilder[]
			{
				Participant.BatchBuilder.RequestAllProperties(),
				Participant.BatchBuilder.CopyPropertiesFromInput(),
				Participant.BatchBuilder.GetPropertiesFromAD(null, adsessionSettings, new PropertyDefinition[]
				{
					ParticipantSchema.SmtpAddress
				})
			});
			for (int j = 0; j < participants.Length; j++)
			{
				if (job[j].Result != null && job[j].Error == null)
				{
					Participant participant3 = job[j].Result.ToParticipant();
					if (participant3.ValidationStatus == ParticipantValidationStatus.NoError)
					{
						participants[j] = participant3;
					}
				}
			}
			return num;
		}
	}
}
