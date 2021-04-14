using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PeopleIKnowMetadata
	{
		public int RelevanceScore { get; set; }
	}
}
