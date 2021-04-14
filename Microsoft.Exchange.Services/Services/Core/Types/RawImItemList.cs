using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RawImItemList
	{
		public RawImGroup[] Groups { get; set; }

		public PersonId[] Personas { get; set; }
	}
}
