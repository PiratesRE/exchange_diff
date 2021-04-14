using System;

namespace Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar
{
	public static class ExTraceGlobals
	{
		public static Trace CreateBirthdayEventForContactTracer
		{
			get
			{
				if (ExTraceGlobals.createBirthdayEventForContactTracer == null)
				{
					ExTraceGlobals.createBirthdayEventForContactTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.createBirthdayEventForContactTracer;
			}
		}

		public static Trace BirthdayCalendarReferenceTracer
		{
			get
			{
				if (ExTraceGlobals.birthdayCalendarReferenceTracer == null)
				{
					ExTraceGlobals.birthdayCalendarReferenceTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.birthdayCalendarReferenceTracer;
			}
		}

		public static Trace BirthdayAssistantBusinessLogicTracer
		{
			get
			{
				if (ExTraceGlobals.birthdayAssistantBusinessLogicTracer == null)
				{
					ExTraceGlobals.birthdayAssistantBusinessLogicTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.birthdayAssistantBusinessLogicTracer;
			}
		}

		public static Trace EnableBirthdayCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.enableBirthdayCalendarTracer == null)
				{
					ExTraceGlobals.enableBirthdayCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.enableBirthdayCalendarTracer;
			}
		}

		public static Trace BirthdayCalendarsTracer
		{
			get
			{
				if (ExTraceGlobals.birthdayCalendarsTracer == null)
				{
					ExTraceGlobals.birthdayCalendarsTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.birthdayCalendarsTracer;
			}
		}

		public static Trace BirthdayEventDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.birthdayEventDataProviderTracer == null)
				{
					ExTraceGlobals.birthdayEventDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.birthdayEventDataProviderTracer;
			}
		}

		public static Trace BirthdayContactDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.birthdayContactDataProviderTracer == null)
				{
					ExTraceGlobals.birthdayContactDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.birthdayContactDataProviderTracer;
			}
		}

		public static Trace UpdateBirthdaysForLinkedContactsTracer
		{
			get
			{
				if (ExTraceGlobals.updateBirthdaysForLinkedContactsTracer == null)
				{
					ExTraceGlobals.updateBirthdaysForLinkedContactsTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.updateBirthdaysForLinkedContactsTracer;
			}
		}

		public static Trace UpdateBirthdayEventForContactTracer
		{
			get
			{
				if (ExTraceGlobals.updateBirthdayEventForContactTracer == null)
				{
					ExTraceGlobals.updateBirthdayEventForContactTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.updateBirthdayEventForContactTracer;
			}
		}

		public static Trace DeleteBirthdayEventForContactTracer
		{
			get
			{
				if (ExTraceGlobals.deleteBirthdayEventForContactTracer == null)
				{
					ExTraceGlobals.deleteBirthdayEventForContactTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.deleteBirthdayEventForContactTracer;
			}
		}

		public static Trace GetBirthdayCalendarViewTracer
		{
			get
			{
				if (ExTraceGlobals.getBirthdayCalendarViewTracer == null)
				{
					ExTraceGlobals.getBirthdayCalendarViewTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.getBirthdayCalendarViewTracer;
			}
		}

		private static Guid componentGuid = new Guid("F89B9EF1-6D5A-48BF-85C8-445007D8B947");

		private static Trace createBirthdayEventForContactTracer = null;

		private static Trace birthdayCalendarReferenceTracer = null;

		private static Trace birthdayAssistantBusinessLogicTracer = null;

		private static Trace enableBirthdayCalendarTracer = null;

		private static Trace birthdayCalendarsTracer = null;

		private static Trace birthdayEventDataProviderTracer = null;

		private static Trace birthdayContactDataProviderTracer = null;

		private static Trace updateBirthdaysForLinkedContactsTracer = null;

		private static Trace updateBirthdayEventForContactTracer = null;

		private static Trace deleteBirthdayEventForContactTracer = null;

		private static Trace getBirthdayCalendarViewTracer = null;
	}
}
