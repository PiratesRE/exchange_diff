using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class StorageMiniRecipientSchema : MiniRecipientSchema
	{
		public static readonly ADPropertyDefinition Alias = ADRecipientSchema.Alias;

		public static readonly ADPropertyDefinition ArchiveDomain = ADUserSchema.ArchiveDomain;

		public static readonly ADPropertyDefinition ArchiveStatus = ADUserSchema.ArchiveStatus;

		public static readonly ADPropertyDefinition ImmutableId = ADRecipientSchema.ImmutableId;

		public static readonly ADPropertyDefinition RawOnPremisesObjectId = ADRecipientSchema.RawOnPremisesObjectId;
	}
}
