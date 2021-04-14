using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ExRoutingTypeDriver : RoutingTypeDriver
	{
		internal override IEqualityComparer<IParticipant> AddressEqualityComparer
		{
			get
			{
				return RoutingTypeDriver.OrdinalCaseInsensitiveAddressEqualityComparerImpl.Default;
			}
		}

		internal static List<PropValue> TryParseExchangeLegacyDN(string inputString)
		{
			if (!ExRoutingTypeDriver.IsValidExAddress(inputString))
			{
				return null;
			}
			return Participant.ListCoreProperties(null, inputString, "EX");
		}

		internal override bool? IsRoutable(string routingType, StoreSession session)
		{
			return new bool?(base.IsRoutable(routingType, session) ?? true);
		}

		internal override bool IsRoutingTypeSupported(string routingType)
		{
			return routingType == "EX";
		}

		internal override void Normalize(PropertyBag participantPropertyBag)
		{
			string valueOrDefault = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.DisplayName);
			string valueOrDefault2 = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
			string valueOrDefault3 = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
			if (valueOrDefault == null)
			{
				if (valueOrDefault3 != null)
				{
					participantPropertyBag[ParticipantSchema.DisplayName] = valueOrDefault3;
				}
				else if (valueOrDefault2 != null && ExRoutingTypeDriver.IsValidExAddress(valueOrDefault2))
				{
					participantPropertyBag.SetOrDeleteProperty(ParticipantSchema.DisplayName, Util.SubstringBetween(valueOrDefault2, "=", null, SubstringOptions.Backward));
				}
			}
			if (PropertyError.IsPropertyNotFound(participantPropertyBag.TryGetProperty(ParticipantSchema.EmailAddressForDisplay)))
			{
				participantPropertyBag.SetOrDeleteProperty(ParticipantSchema.EmailAddressForDisplay, valueOrDefault3);
			}
			participantPropertyBag.SetOrDeleteProperty(ParticipantSchema.LegacyExchangeDN, valueOrDefault2);
			participantPropertyBag.SetOrDeleteProperty(ParticipantSchema.SendRichInfo, true);
			base.Normalize(participantPropertyBag);
		}

		internal override bool TryDetectRoutingType(PropertyBag participantPropertyBag, out string routingType)
		{
			string valueOrDefault = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
			if (valueOrDefault != null && ExRoutingTypeDriver.IsValidExAddress(valueOrDefault))
			{
				routingType = "EX";
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
			if (!ExRoutingTypeDriver.IsValidExAddress(participant.EmailAddress))
			{
				return ParticipantValidationStatus.InvalidAddressFormat;
			}
			return ParticipantValidationStatus.NoError;
		}

		private static bool IsValidExAddress(string address)
		{
			LegacyDN legacyDN;
			return LegacyDN.TryParse(address, out legacyDN);
		}

		private const string LegDnNamePartDelimiter = "=";
	}
}
