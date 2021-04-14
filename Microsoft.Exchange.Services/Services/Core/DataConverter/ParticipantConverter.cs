using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ParticipantConverter : BaseConverter
	{
		public static EmailAddressWrapper ConvertParticipantToEmailAddressWrapper(IParticipant xsoEmailAddress, IdConverterWithCommandSettings idConverterWithCommandSettings)
		{
			if (xsoEmailAddress == null)
			{
				return null;
			}
			EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper
			{
				Name = xsoEmailAddress.DisplayName,
				EmailAddress = xsoEmailAddress.EmailAddress,
				RoutingType = xsoEmailAddress.RoutingType,
				MailboxType = MailboxHelper.GetMailboxType(xsoEmailAddress.Origin, xsoEmailAddress.RoutingType).ToString(),
				OriginalDisplayName = xsoEmailAddress.OriginalDisplayName
			};
			StoreParticipantOrigin storeParticipantOrigin = xsoEmailAddress.Origin as StoreParticipantOrigin;
			if (storeParticipantOrigin != null)
			{
				ConcatenatedIdAndChangeKey concatenatedId = idConverterWithCommandSettings.GetConcatenatedId(storeParticipantOrigin.OriginItemId);
				emailAddressWrapper.ItemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
				emailAddressWrapper.EmailAddressIndex = storeParticipantOrigin.EmailAddressIndex.ToString();
			}
			return emailAddressWrapper;
		}

		public override object ConvertToObject(string propertyString)
		{
			return null;
		}

		public override string ConvertToString(object propertyValue)
		{
			return string.Empty;
		}

		public override object ConvertToServiceObjectValue(object propertyValue, IdConverterWithCommandSettings idConverterWithCommandSettings)
		{
			Participant xsoEmailAddress = propertyValue as Participant;
			return ParticipantConverter.ConvertParticipantToEmailAddressWrapper(xsoEmailAddress, idConverterWithCommandSettings);
		}
	}
}
