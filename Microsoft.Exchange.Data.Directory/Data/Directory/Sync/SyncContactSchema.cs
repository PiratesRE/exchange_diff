using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncContactSchema : SyncOrgPersonSchema
	{
		public override DirectoryObjectClass DirectoryObjectClass
		{
			get
			{
				return DirectoryObjectClass.Contact;
			}
		}

		public static SyncPropertyDefinition Manager = SyncUserSchema.Manager;
	}
}
