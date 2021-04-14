using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SmtpRoutingTypeDriver : RoutingTypeDriver
	{
		internal override IEqualityComparer<IParticipant> AddressEqualityComparer
		{
			get
			{
				return RoutingTypeDriver.OrdinalCaseInsensitiveAddressEqualityComparerImpl.Default;
			}
		}

		internal override bool? IsRoutable(string routingType, StoreSession session)
		{
			return new bool?(base.IsRoutable(routingType, session) ?? true);
		}

		internal override bool IsRoutingTypeSupported(string routingType)
		{
			return routingType == "SMTP";
		}

		internal override void Normalize(PropertyBag participantPropertyBag)
		{
			participantPropertyBag.SetOrDeleteProperty(ParticipantSchema.SmtpAddress, participantPropertyBag.TryGetProperty(ParticipantSchema.EmailAddress));
			base.Normalize(participantPropertyBag);
		}

		internal override bool TryDetectRoutingType(PropertyBag participantPropertyBag, out string routingType)
		{
			string valueOrDefault = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
			if (valueOrDefault != null && SmtpRoutingTypeDriver.IsValidSmtpAddress(valueOrDefault))
			{
				routingType = "SMTP";
				return true;
			}
			routingType = null;
			return false;
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			if (participant.EmailAddress == null)
			{
				return ParticipantValidationStatus.AddressRequiredForRoutingType;
			}
			if (!SmtpRoutingTypeDriver.IsValidSmtpAddress(participant.EmailAddress))
			{
				return ParticipantValidationStatus.InvalidAddressFormat;
			}
			return ParticipantValidationStatus.NoError;
		}

		internal override string FormatAddress(Participant participant, AddressFormat addressFormat)
		{
			if (addressFormat == AddressFormat.Rfc822Smtp)
			{
				return string.Format("\"{0}\" <{1}>", participant.DisplayName, participant.EmailAddress);
			}
			return base.FormatAddress(participant, addressFormat);
		}

		private static bool IsValidSmtpAddress(string address)
		{
			return SmtpAddress.IsValidSmtpAddress(address);
		}

		private const string RfcFormat = "\"{0}\" <{1}>";

		internal static IEqualityComparer<string> SmtpAddressEqualityComparer = RoutingTypeDriver.OrdinalCaseInsensitiveAddressEqualityComparerImpl.Default;
	}
}
