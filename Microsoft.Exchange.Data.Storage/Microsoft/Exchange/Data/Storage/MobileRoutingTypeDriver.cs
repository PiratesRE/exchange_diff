using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MobileRoutingTypeDriver : RoutingTypeDriver
	{
		internal override IEqualityComparer<IParticipant> AddressEqualityComparer
		{
			get
			{
				return RoutingTypeDriver.OrdinalCaseInsensitiveAddressEqualityComparerImpl.Default;
			}
		}

		internal override bool IsRoutingTypeSupported(string routingType)
		{
			return routingType == "MOBILE";
		}

		internal override bool? IsRoutable(string routingType, StoreSession session)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null)
			{
				return new bool?(base.IsRoutable(routingType, session).Value && mailboxSession.MailboxOwner.MailboxInfo.Configuration.IsPersonToPersonMessagingEnabled);
			}
			return null;
		}

		internal static List<PropValue> TryParseMobilePhoneNumber(string inputString)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				return null;
			}
			int num;
			int num2;
			string number;
			string text;
			if ((num = inputString.LastIndexOfAny(MobileRoutingTypeDriver.separatorsRightBracket)) == -1 || (num2 = inputString.LastIndexOfAny(MobileRoutingTypeDriver.separatorsLeftBracket)) == -1)
			{
				number = inputString;
				text = inputString;
			}
			else
			{
				if (num2 >= num || ((inputString[num2] != '[' || inputString[num] != ']') && (inputString[num2] != '<' || inputString[num] != '>')))
				{
					return null;
				}
				text = inputString.Substring(0, num2);
				number = inputString.Substring(num2 + 1, num - num2 - 1);
			}
			E164Number e164Number;
			if (!E164Number.TryParse(number, out e164Number))
			{
				return null;
			}
			int num3 = text.Length;
			int num4 = -1;
			int i = 0;
			while (i < text.Length)
			{
				if (!char.IsWhiteSpace(text[i]))
				{
					if ('"' == text[i])
					{
						num3 = i;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			int j = text.Length - 1;
			while (j >= num3)
			{
				if (!char.IsWhiteSpace(text[j]))
				{
					if ('"' == text[j])
					{
						num4 = j;
						break;
					}
					break;
				}
				else
				{
					j--;
				}
			}
			if (num3 < num4)
			{
				text = text.Substring(num3 + 1, num4 - num3 - 1);
			}
			else
			{
				text = text.Trim();
			}
			return Participant.ListCoreProperties(text, e164Number.Number, "MOBILE");
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			if (participant.EmailAddress == null)
			{
				return ParticipantValidationStatus.AddressRequiredForRoutingType;
			}
			if (!E164Number.IsValidE164Number(participant.EmailAddress))
			{
				return ParticipantValidationStatus.InvalidAddressFormat;
			}
			return ParticipantValidationStatus.NoError;
		}

		internal override void Normalize(PropertyBag participantPropertyBag)
		{
			string valueOrDefault = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
			E164Number e164Number;
			if (!string.IsNullOrEmpty(valueOrDefault) && E164Number.TryParse(valueOrDefault, out e164Number))
			{
				participantPropertyBag.SetOrDeleteProperty(ParticipantSchema.EmailAddress, e164Number.Number);
			}
			base.Normalize(participantPropertyBag);
		}

		private static readonly char[] separatorsLeftBracket = new char[]
		{
			'[',
			'<'
		};

		private static readonly char[] separatorsRightBracket = new char[]
		{
			']',
			'>'
		};
	}
}
