using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal abstract class ParticipantWrapper<TOriginal> : IParticipantWrapper
	{
		protected ParticipantWrapper(TOriginal original)
		{
			this.Original = original;
		}

		public TOriginal Original { get; private set; }

		public abstract Participant Participant { get; }

		public static implicit operator Participant(ParticipantWrapper<TOriginal> wrapper)
		{
			return wrapper.Participant;
		}
	}
}
