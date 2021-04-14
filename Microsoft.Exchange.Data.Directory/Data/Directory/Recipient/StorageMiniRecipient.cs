using System;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class StorageMiniRecipient : MiniRecipient, IFederatedIdentityParameters
	{
		internal StorageMiniRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		public StorageMiniRecipient()
		{
		}

		public string Alias
		{
			get
			{
				return (string)this[StorageMiniRecipientSchema.Alias];
			}
		}

		public SmtpDomain ArchiveDomain
		{
			get
			{
				return (SmtpDomain)this[StorageMiniRecipientSchema.ArchiveDomain];
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return (ArchiveStatusFlags)this[StorageMiniRecipientSchema.ArchiveStatus];
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return base.Id;
			}
		}

		public string ImmutableId
		{
			get
			{
				return (string)this[StorageMiniRecipientSchema.ImmutableId];
			}
		}

		public string ImmutableIdPartial
		{
			get
			{
				return ADRecipient.ConvertOnPremisesObjectIdToString(this[StorageMiniRecipientSchema.RawOnPremisesObjectId]);
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return StorageMiniRecipient.schema;
			}
		}

		internal FederatedIdentity GetFederatedIdentity()
		{
			return FederatedIdentityHelper.GetFederatedIdentity(this);
		}

		private static readonly StorageMiniRecipientSchema schema = ObjectSchema.GetInstance<StorageMiniRecipientSchema>();
	}
}
