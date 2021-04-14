using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors
{
	internal static class AttendeeAccessors
	{
		public static readonly IStoragePropertyAccessor<Attendee, ExDateTime> ReplyTime = new DefaultStoragePropertyAccessor<Attendee, ExDateTime>(RecipientSchema.RecipientTrackStatusTime, false);
	}
}
