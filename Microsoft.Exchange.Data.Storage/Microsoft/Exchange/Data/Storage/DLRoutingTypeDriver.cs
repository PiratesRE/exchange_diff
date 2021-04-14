using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DLRoutingTypeDriver : RoutingTypeDriver
	{
		internal override IEqualityComparer<IParticipant> AddressEqualityComparer
		{
			get
			{
				return DLRoutingTypeDriver.AddressEqualityComparerImpl.Default;
			}
		}

		internal override bool? IsRoutable(string routingType, StoreSession session)
		{
			return new bool?(true);
		}

		internal override bool IsRoutingTypeSupported(string routingType)
		{
			return routingType == "MAPIPDL";
		}

		internal override void Normalize(PropertyBag participantPropertyBag)
		{
			participantPropertyBag[ParticipantSchema.DisplayType] = 5;
			base.Normalize(participantPropertyBag);
		}

		internal override bool TryDetectRoutingType(PropertyBag participantPropertyBag, out string routingType)
		{
			routingType = null;
			return false;
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			if (participant.EmailAddress != null)
			{
				return ParticipantValidationStatus.AddressAndRoutingTypeMismatch;
			}
			if (PropertyError.IsPropertyNotFound(participant.TryGetProperty(ParticipantSchema.OriginItemId)))
			{
				return ParticipantValidationStatus.AddressAndOriginMismatch;
			}
			return ParticipantValidationStatus.NoError;
		}

		private sealed class AddressEqualityComparerImpl : IEqualityComparer<IParticipant>
		{
			public bool Equals(IParticipant x, IParticipant y)
			{
				object obj = x.GetValueOrDefault<object>(ParticipantSchema.OriginItemId) as StoreObjectId;
				if (obj == null)
				{
					return x.Equals(y);
				}
				return obj.Equals(y.GetValueOrDefault<object>(ParticipantSchema.OriginItemId));
			}

			public int GetHashCode(IParticipant x)
			{
				object obj = x.GetValueOrDefault<object>(ParticipantSchema.OriginItemId) as StoreObjectId;
				if (obj == null)
				{
					return x.GetHashCode();
				}
				return obj.GetHashCode();
			}

			public static DLRoutingTypeDriver.AddressEqualityComparerImpl Default = new DLRoutingTypeDriver.AddressEqualityComparerImpl();
		}
	}
}
