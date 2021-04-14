using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionIsGroupMailbox : ServicePermanentException
	{
		static CalendarExceptionIsGroupMailbox()
		{
			CalendarExceptionIsGroupMailbox.errorMappings.Add(typeof(AcceptItemType).Name, CoreResources.IDs.ErrorCalendarIsGroupMailboxForAccept);
			CalendarExceptionIsGroupMailbox.errorMappings.Add(typeof(DeclineItemType).Name, CoreResources.IDs.ErrorCalendarIsGroupMailboxForDecline);
			CalendarExceptionIsGroupMailbox.errorMappings.Add(typeof(TentativelyAcceptItemType).Name, (CoreResources.IDs)3187786876U);
			CalendarExceptionIsGroupMailbox.errorMappings.Add(typeof(SuppressReadReceiptType).Name, CoreResources.IDs.ErrorCalendarIsGroupMailboxForSuppressReadReceipt);
		}

		public CalendarExceptionIsGroupMailbox(string operation) : base(CalendarExceptionIsGroupMailbox.errorMappings[operation])
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}

		private static Dictionary<string, Enum> errorMappings = new Dictionary<string, Enum>();
	}
}
