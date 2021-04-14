using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class UnspecifiedRoutingTypeDriver : RoutingTypeDriver
	{
		internal override IEqualityComparer<IParticipant> AddressEqualityComparer
		{
			get
			{
				return UnspecifiedRoutingTypeDriver.AddressEqualityComparerImpl.Default;
			}
		}

		internal static List<PropValue> TryParse(string inputString)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				return null;
			}
			return Participant.ListCoreProperties(inputString, null, null, inputString);
		}

		internal override bool? IsRoutable(string routingType, StoreSession session)
		{
			return new bool?(false);
		}

		internal override bool IsRoutingTypeSupported(string routingType)
		{
			return routingType == null;
		}

		internal override void Normalize(PropertyBag participantPropertyBag)
		{
			string valueOrDefault = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.DisplayName);
			string valueOrDefault2 = participantPropertyBag.GetValueOrDefault<string>(ParticipantSchema.EmailAddress);
			if (valueOrDefault2 != null)
			{
				if (valueOrDefault == null)
				{
					participantPropertyBag[ParticipantSchema.DisplayName] = valueOrDefault2;
				}
				else
				{
					participantPropertyBag[ParticipantSchema.EmailAddressForDisplay] = valueOrDefault2;
				}
				participantPropertyBag.Delete(ParticipantSchema.EmailAddress);
			}
			participantPropertyBag.Delete(ParticipantSchema.DisplayTypeEx);
			participantPropertyBag.Delete(ParticipantSchema.DisplayType);
			base.Normalize(participantPropertyBag);
		}

		internal override bool TryDetectRoutingType(PropertyBag participantPropertyBag, out string routingType)
		{
			routingType = null;
			return true;
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			if (participant.DisplayName == null)
			{
				if (participant.GetValueOrDefault<string>(ParticipantSchema.EmailAddressForDisplay, null) == null)
				{
					return ParticipantValidationStatus.DisplayNameRequiredForRoutingType;
				}
			}
			else if (participant.EmailAddress != null)
			{
				return ParticipantValidationStatus.AddressAndRoutingTypeMismatch;
			}
			return ParticipantValidationStatus.NoError;
		}

		internal override string FormatAddress(Participant participant, AddressFormat addressFormat)
		{
			if (addressFormat == AddressFormat.OutlookFormat || addressFormat == AddressFormat.Rfc822Smtp)
			{
				return participant.DisplayName;
			}
			return base.FormatAddress(participant, addressFormat);
		}

		private static readonly PropertyDefinition[] meaningfulProperties = new PropertyDefinition[]
		{
			ParticipantSchema.DisplayName,
			ParticipantSchema.EmailAddress,
			ParticipantSchema.EmailAddressForDisplay
		};

		private sealed class AddressEqualityComparerImpl : IEqualityComparer<IParticipant>
		{
			public bool Equals(IParticipant x, IParticipant y)
			{
				if (x.DisplayName == null)
				{
					return x.Equals(y);
				}
				return StringComparer.Ordinal.Equals(x.DisplayName, y.DisplayName);
			}

			public int GetHashCode(IParticipant x)
			{
				if (x.DisplayName == null)
				{
					return x.GetHashCode();
				}
				return StringComparer.Ordinal.GetHashCode(x.DisplayName);
			}

			public static UnspecifiedRoutingTypeDriver.AddressEqualityComparerImpl Default = new UnspecifiedRoutingTypeDriver.AddressEqualityComparerImpl();
		}
	}
}
