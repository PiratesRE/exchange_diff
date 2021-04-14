using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class BandRandomEntitySelector : EntitySelector
	{
		protected BandRandomEntitySelector(Band band, LoadContainer sourceContainer, string constraintSetIdentity)
		{
			AnchorUtil.ThrowOnNullArgument(band, "band");
			AnchorUtil.ThrowOnNullArgument(sourceContainer, "sourceContainer");
			this.band = band;
			this.SourceEntities = (from x in sourceContainer.Children
			where x.DirectoryObjectIdentity.Name == constraintSetIdentity
			select x).Cast<LoadContainer>().SelectMany((LoadContainer x) => x.Children).Where(new Func<LoadEntity, bool>(this.IsAcceptedEntity));
		}

		public override bool IsEmpty
		{
			get
			{
				return !this.SourceEntities.Any<LoadEntity>();
			}
		}

		private protected IEnumerable<LoadEntity> SourceEntities { protected get; private set; }

		protected virtual bool IsAcceptedEntity(LoadEntity entity)
		{
			DirectoryMailbox directoryMailbox = entity.DirectoryObject as DirectoryMailbox;
			return directoryMailbox != null && this.band.ContainsMailbox(directoryMailbox);
		}

		private readonly Band band;
	}
}
