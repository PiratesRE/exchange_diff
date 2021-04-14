using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class ArchiveSupportedAnchorMailbox : UserBasedAnchorMailbox
	{
		protected ArchiveSupportedAnchorMailbox(AnchorSource anchorSource, object sourceObject, IRequestContext requestContext) : base(anchorSource, sourceObject, requestContext)
		{
		}

		public bool? IsArchive { get; set; }

		protected override ADPropertyDefinition[] PropertySet
		{
			get
			{
				if (this.IsArchive != null && this.IsArchive.Value)
				{
					return ArchiveSupportedAnchorMailbox.ArchiveMailboxADRawEntryPropertySet;
				}
				return base.PropertySet;
			}
		}

		protected override ADPropertyDefinition DatabaseProperty
		{
			get
			{
				if (this.IsArchive != null && this.IsArchive.Value)
				{
					return ADUserSchema.ArchiveDatabase;
				}
				return base.DatabaseProperty;
			}
		}

		protected override string ToCacheKey()
		{
			string text = base.ToCacheKey();
			if (this.IsArchive != null && this.IsArchive.Value)
			{
				text += "_Archive";
			}
			return text;
		}

		protected static readonly ADPropertyDefinition[] ArchiveMailboxADRawEntryPropertySet = new ADPropertyDefinition[]
		{
			ADObjectSchema.OrganizationId,
			ADUserSchema.ArchiveDatabase,
			ADRecipientSchema.PrimarySmtpAddress
		};
	}
}
