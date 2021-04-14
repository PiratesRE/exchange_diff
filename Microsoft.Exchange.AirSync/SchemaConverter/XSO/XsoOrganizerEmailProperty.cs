using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoOrganizerEmailProperty : XsoStringProperty
	{
		public XsoOrganizerEmailProperty() : base(CalendarItemBaseSchema.OrganizerEmailAddress)
		{
		}

		public override string StringData
		{
			get
			{
				CalendarItemBase calendarItemBase = base.XsoItem as CalendarItemBase;
				if (null == calendarItemBase.Organizer)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "value for Organizer is null.");
					return string.Empty;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "Loading value for Organizer .");
				return EmailAddressConverter.LookupEmailAddressString(calendarItemBase.Organizer, calendarItemBase.Session.MailboxOwner);
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
		}
	}
}
