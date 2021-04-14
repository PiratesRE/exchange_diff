using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UnrecognizedParticipantEntryId : ParticipantEntryId
	{
		internal UnrecognizedParticipantEntryId(byte[] entryId)
		{
			this.entryId = entryId;
		}

		public override string ToString()
		{
			return string.Format("Unrecognized: {0} bytes", this.entryId.Length);
		}

		internal override IEnumerable<PropValue> GetParticipantProperties()
		{
			return Array<PropValue>.Empty;
		}

		protected override void Serialize(ParticipantEntryId.Writer writer)
		{
			writer.Write(this.entryId);
		}

		private readonly byte[] entryId;
	}
}
