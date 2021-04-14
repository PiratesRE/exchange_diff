using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncForeignPrincipalSchema : SyncObjectSchema
	{
		public override DirectoryObjectClass DirectoryObjectClass
		{
			get
			{
				return DirectoryObjectClass.ForeignPrincipal;
			}
		}

		public static SyncPropertyDefinition DisplayName = new SyncPropertyDefinition(ADRecipientSchema.DisplayName, "DisplayName", typeof(DirectoryPropertyStringSingleLength1To256), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Description = new SyncPropertyDefinition("Description", "Description", typeof(string), typeof(DirectoryPropertyStringSingleLength1To1024), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.SyncPropertySetVersion3, string.Empty);

		public static SyncPropertyDefinition LinkedPartnerGroupId = new SyncPropertyDefinition(ADGroupSchema.LinkedPartnerGroupId, "ForeignPrincipalId", typeof(DirectoryPropertyGuidSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition LinkedPartnerOrganizationId = new SyncPropertyDefinition(ADGroupSchema.LinkedPartnerOrganizationId, "ForeignContextId", typeof(DirectoryPropertyGuidSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);
	}
}
