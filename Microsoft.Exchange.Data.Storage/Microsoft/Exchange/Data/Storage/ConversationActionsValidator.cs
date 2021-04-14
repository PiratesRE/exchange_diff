using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversationActionsValidator : DefaultFolderValidator
	{
		internal ConversationActionsValidator() : base(new IValidator[]
		{
			new MatchIsHidden(true),
			new MatchMapiFolderType(FolderType.Generic),
			new MatchContainerClass("IPF.Configuration")
		})
		{
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			folder[FolderSchema.IsHidden] = true;
			folder[StoreObjectSchema.ContainerClass] = "IPF.Configuration";
		}
	}
}
