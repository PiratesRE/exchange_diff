using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AttachmentPropertyRestriction : PropertyRestriction
	{
		public AttachmentPropertyRestriction()
		{
			this.BlockAfterLink.Add(AttachmentSchema.DisplayName);
			this.BlockAfterLink.Add(AttachmentSchema.AttachLongPathName);
			this.BlockAfterLink.Add(AttachmentSchema.AttachMethod);
			this.BlockAfterLink.Add(AttachmentSchema.AttachFileName);
		}

		public static AttachmentPropertyRestriction Instance = new AttachmentPropertyRestriction();
	}
}
