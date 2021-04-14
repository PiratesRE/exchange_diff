using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal static class ParticipantsAddressesConverter
	{
		public static EmailAddressWrapper[] ToAddresses(IList<Participant> participants)
		{
			if (participants == null || participants.Count == 0)
			{
				return null;
			}
			EmailAddressWrapper[] array = new EmailAddressWrapper[participants.Count];
			for (int i = 0; i < participants.Count; i++)
			{
				EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
				Participant participant = participants[i];
				if (null != participant)
				{
					emailAddressWrapper.EmailAddress = participant.EmailAddress;
					emailAddressWrapper.Name = participant.DisplayName;
					emailAddressWrapper.RoutingType = participant.RoutingType;
					emailAddressWrapper.OriginalDisplayName = participant.OriginalDisplayName;
				}
				array[i] = emailAddressWrapper;
			}
			return array;
		}

		public static Participant[] ToParticipants(IList<EmailAddressWrapper> emailAddressList)
		{
			if (emailAddressList == null || emailAddressList.Count == 0)
			{
				return null;
			}
			Participant[] array = new Participant[emailAddressList.Count];
			for (int i = 0; i < emailAddressList.Count; i++)
			{
				EmailAddressWrapper emailAddressWrapper = emailAddressList[i];
				if (emailAddressWrapper != null)
				{
					if (string.IsNullOrEmpty(emailAddressWrapper.RoutingType))
					{
						emailAddressWrapper.RoutingType = "SMTP";
					}
					array[i] = new Participant(emailAddressWrapper.Name, emailAddressWrapper.EmailAddress, emailAddressWrapper.RoutingType, emailAddressWrapper.OriginalDisplayName);
				}
			}
			return array;
		}
	}
}
