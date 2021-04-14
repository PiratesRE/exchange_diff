using System;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services
{
	public class UnifiedGroupsHelper
	{
		internal static Persona UnifiedGroupParticipantToPersona(UnifiedGroupParticipant participant)
		{
			Persona persona = new Persona();
			persona.DisplayName = participant.DisplayName;
			persona.Alias = participant.Alias;
			persona.ADObjectId = participant.Id.ObjectGuid;
			persona.PersonaId = IdConverter.PersonaIdFromADObjectId(persona.ADObjectId);
			persona.Title = participant.Title;
			persona.ImAddress = participant.SipAddress;
			persona.EmailAddress = new EmailAddressWrapper
			{
				Name = (persona.DisplayName ?? string.Empty),
				EmailAddress = participant.PrimarySmtpAddressToString,
				RoutingType = "SMTP",
				MailboxType = MailboxHelper.MailboxTypeType.Mailbox.ToString()
			};
			return persona;
		}
	}
}
