using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FolderBasedCalendarItemStateDefinition : SinglePropertyValueBasedCalendarItemStateDefinition<byte[]>
	{
		public FolderBasedCalendarItemStateDefinition(byte[] targetFolderId) : base(InternalSchema.OriginalFolderId, targetFolderId, ArrayComparer<byte>.Comparer)
		{
		}

		public override string SchemaKey
		{
			get
			{
				return "{4BFFBA09-A6D3-4144-A2B5-0FD7B57C3FFD}";
			}
		}

		public override StorePropertyDefinition[] RequiredProperties
		{
			get
			{
				return FolderBasedCalendarItemStateDefinition.requiredProperties;
			}
		}

		private static readonly StorePropertyDefinition[] requiredProperties = new StorePropertyDefinition[]
		{
			CalendarItemBaseSchema.ClientIntent,
			InternalSchema.OriginalFolderId
		};
	}
}
