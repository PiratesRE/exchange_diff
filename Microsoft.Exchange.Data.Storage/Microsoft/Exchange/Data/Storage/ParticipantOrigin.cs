using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ParticipantOrigin
	{
		internal abstract IEnumerable<PropValue> GetProperties();

		internal abstract ParticipantValidationStatus Validate(Participant participant);
	}
}
