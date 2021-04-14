using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OneOffParticipantOrigin : ParticipantOrigin
	{
		public override string ToString()
		{
			return "OneOff";
		}

		internal override IEnumerable<PropValue> GetProperties()
		{
			return null;
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			return ParticipantValidationStatus.NoError;
		}
	}
}
