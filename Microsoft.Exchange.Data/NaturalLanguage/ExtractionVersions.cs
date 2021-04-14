using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ExtractionVersions
	{
		public static readonly Version Office15 = new Version(15, 0, 0, 0);

		public static readonly Version CurrentVersion = ExtractionVersions.Office15;
	}
}
