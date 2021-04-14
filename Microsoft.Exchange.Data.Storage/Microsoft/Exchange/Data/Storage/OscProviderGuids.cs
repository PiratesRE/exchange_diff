using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class OscProviderGuids
	{
		public static readonly Guid Facebook = new Guid("EFDABF9A-7D2C-4BE6-A266-872EC6FC63E7");

		public static readonly Guid LinkedIn = new Guid("8E8005E8-B772-4FC0-81B8-C55E18DE4FA4");

		public static readonly Guid WindowsLive = new Guid("D2B01241-99F4-416B-ADA1-D7330F70D171");

		public static readonly Guid SharePoint = new Guid("1C306CB1-771E-4B4B-A902-86E897877F5B");

		public static readonly Guid Xing = new Guid("8EC4B978-EBCF-46CC-931C-A1CCF3FEBA22");
	}
}
