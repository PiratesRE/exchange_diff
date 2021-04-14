using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal static class MessageClass
	{
		public static bool IsExternalOofTemplate(string messageClass)
		{
			return ObjectClass.IsOfClass(messageClass, "IPM.Note.Rules.ExternalOofTemplate.Microsoft");
		}

		public static bool IsInternalOofTemplate(string messageClass)
		{
			return !MessageClass.IsExternalOofTemplate(messageClass) && ObjectClass.IsOfClass(messageClass, "IPM.Note.Rules.OofTemplate.Microsoft");
		}

		public static bool IsAppointment(string messageClass)
		{
			return ObjectClass.IsOfClass(messageClass, "IPM.Appointment");
		}

		public const string ExternalOofTemplate = "IPM.Note.Rules.ExternalOofTemplate.Microsoft";

		public const string InternalOofTemplate = "IPM.Note.Rules.OofTemplate.Microsoft";

		public const string Appointment = "IPM.Appointment";

		public const string UserUserOofSettings = "IPM.Microsoft.OOF.UserOofSettings";

		public const string ContactsEmailAddresses = "IPM.Microsoft.ContactsEmailAddresses";

		public const string OofAssistantControl = "IPM.Microsoft.OOF.Control";

		public const string OofSchedule = "IPM.Microsoft.OOF.Schedule";

		public const string OofLog = "IPM.Microsoft.OOF.Log";

		public const string RbaLog = "IPM.Microsoft.RBA.Log";

		public const string MfnLog = "IPM.Microsoft.MFN.Log";

		public const string CalendarAssistantLog = "IPM.Microsoft.CA.Log";

		public const string MrmLog = "IPM.Microsoft.MRM.Log";
	}
}
