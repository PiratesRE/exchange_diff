using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class StoreParticipantOrigin : ParticipantOrigin
	{
		public StoreParticipantOrigin(StoreId originItemId) : this(originItemId, EmailAddressIndex.None)
		{
		}

		public StoreParticipantOrigin(StoreId originItemId, EmailAddressIndex emailAddressIndex)
		{
			EnumValidator.ThrowIfInvalid<EmailAddressIndex>(emailAddressIndex);
			if (originItemId == null)
			{
				throw new ArgumentNullException("originItemId");
			}
			this.originItemId = StoreId.GetStoreObjectId(originItemId);
			this.emailAddressIndex = emailAddressIndex;
		}

		public EmailAddressIndex EmailAddressIndex
		{
			get
			{
				return this.emailAddressIndex;
			}
		}

		public StoreObjectId OriginItemId
		{
			get
			{
				return this.originItemId;
			}
		}

		public override string ToString()
		{
			return string.Format("Store ({0}, {1})", this.originItemId, this.emailAddressIndex);
		}

		internal override IEnumerable<PropValue> GetProperties()
		{
			List<PropValue> list = new List<PropValue>();
			if (this.emailAddressIndex == EmailAddressIndex.None)
			{
				list.Add(new PropValue(ParticipantSchema.OriginItemId, this.originItemId));
			}
			list.Add(new PropValue(ParticipantSchema.DisplayType, (this.emailAddressIndex == EmailAddressIndex.None) ? LegacyRecipientDisplayType.PersonalDistributionList : LegacyRecipientDisplayType.MailUser));
			return list;
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			return ParticipantValidationStatus.NoError;
		}

		private readonly StoreObjectId originItemId;

		private readonly EmailAddressIndex emailAddressIndex;
	}
}
