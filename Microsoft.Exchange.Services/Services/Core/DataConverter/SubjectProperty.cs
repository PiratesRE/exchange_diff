using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class SubjectProperty : SimpleProperty
	{
		protected SubjectProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static SubjectProperty CreateCommand(CommandContext commandContext)
		{
			return new SubjectProperty(commandContext);
		}

		public override void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			CalendarItemBase calendarItemBase = commandSettings.StoreObject as CalendarItemBase;
			if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) || calendarItemBase == null || !(calendarItemBase.Session is MailboxSession))
			{
				base.ToServiceObject();
				return;
			}
			if (calendarItemBase.Sensitivity != Sensitivity.Normal && ((MailboxSession)calendarItemBase.Session).ShouldFilterPrivateItems)
			{
				this.WriteServiceProperty(ClientStrings.PrivateAppointmentSubject.ToString(calendarItemBase.Session.PreferedCulture), serviceObject, propertyInformation);
				return;
			}
			base.ToServiceObject();
		}
	}
}
