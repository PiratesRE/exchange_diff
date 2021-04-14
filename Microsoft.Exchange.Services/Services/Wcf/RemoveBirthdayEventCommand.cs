using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RemoveBirthdayEventCommand : ServiceCommand<CalendarActionResponse>
	{
		public RemoveBirthdayEventCommand(CallContext callContext, Microsoft.Exchange.Services.Core.Types.ItemId contactId) : base(callContext)
		{
			this.contactId = contactId;
		}

		protected override CalendarActionResponse InternalExecute()
		{
			return new CalendarActionResponse();
		}

		private Microsoft.Exchange.Services.Core.Types.ItemId contactId;
	}
}
