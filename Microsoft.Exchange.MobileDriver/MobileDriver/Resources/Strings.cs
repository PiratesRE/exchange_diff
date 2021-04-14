using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.TextMessaging.MobileDriver.Resources
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(684812396U, "ConstNull");
			Strings.stringIDs.Add(2765603176U, "ErrorEmailMessageNotFound");
			Strings.stringIDs.Add(268030502U, "ErrorNotSupportMultimediaMessage");
			Strings.stringIDs.Add(298068888U, "ErrorEmptyCalNotifContent");
			Strings.stringIDs.Add(893814531U, "ErrorTooManyParts");
			Strings.stringIDs.Add(3337871027U, "ErrorAvaliableServiceNotFound");
			Strings.stringIDs.Add(406232425U, "ErrorEmailNotificationDeadLoop");
			Strings.stringIDs.Add(597584490U, "ErrorNeutralCodingScheme");
			Strings.stringIDs.Add(1045457712U, "ConstNa");
			Strings.stringIDs.Add(3260461220U, "ErrorInvalidPhoneNumber");
			Strings.stringIDs.Add(234345771U, "ErrorTooManyRecipients");
			Strings.stringIDs.Add(4114712759U, "calNotifAllDayEventsDesc");
		}

		public static LocalizedString ConstNull
		{
			get
			{
				return new LocalizedString("ConstNull", "Ex4302BE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotAcknowledged(string number, string ecpLink)
		{
			return new LocalizedString("ErrorNotAcknowledged", "Ex0B9AAF", false, true, Strings.ResourceManager, new object[]
			{
				number,
				ecpLink
			});
		}

		public static LocalizedString ErrorNoP2pDeliveryPoint(string ecpLink)
		{
			return new LocalizedString("ErrorNoP2pDeliveryPoint", "ExDBC203", false, true, Strings.ResourceManager, new object[]
			{
				ecpLink
			});
		}

		public static LocalizedString ErrorEmailMessageNotFound
		{
			get
			{
				return new LocalizedString("ErrorEmailMessageNotFound", "ExCA033E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString notifCountOfEventsDesc(string number)
		{
			return new LocalizedString("notifCountOfEventsDesc", "Ex2AE6E1", false, true, Strings.ResourceManager, new object[]
			{
				number
			});
		}

		public static LocalizedString ErrorUnableDeliverForEas(string number, string error)
		{
			return new LocalizedString("ErrorUnableDeliverForEas", "Ex6744C1", false, true, Strings.ResourceManager, new object[]
			{
				number,
				error
			});
		}

		public static LocalizedString ErrorNotSupportMultimediaMessage
		{
			get
			{
				return new LocalizedString("ErrorNotSupportMultimediaMessage", "Ex3CE24B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmptyCalNotifContent
		{
			get
			{
				return new LocalizedString("ErrorEmptyCalNotifContent", "Ex7E26A4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoM2pDeliveryPointForEmailAlert(string ecpLink)
		{
			return new LocalizedString("ErrorNoM2pDeliveryPointForEmailAlert", "Ex35983F", false, true, Strings.ResourceManager, new object[]
			{
				ecpLink
			});
		}

		public static LocalizedString ErrorNoProviderForTextMessage(string textMessagingSlabLink)
		{
			return new LocalizedString("ErrorNoProviderForTextMessage", "ExD7F292", false, true, Strings.ResourceManager, new object[]
			{
				textMessagingSlabLink
			});
		}

		public static LocalizedString ErrorTooManyParts
		{
			get
			{
				return new LocalizedString("ErrorTooManyParts", "Ex4324C5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorObjectNotFound(string identity)
		{
			return new LocalizedString("ErrorObjectNotFound", "ExBD415B", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorCannotParseSettings(string error)
		{
			return new LocalizedString("ErrorCannotParseSettings", "Ex362A58", false, true, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ErrorAvaliableServiceNotFound
		{
			get
			{
				return new LocalizedString("ErrorAvaliableServiceNotFound", "Ex812156", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCantBeCoded(string codingScheme, string text)
		{
			return new LocalizedString("ErrorCantBeCoded", "Ex0EE29C", false, true, Strings.ResourceManager, new object[]
			{
				codingScheme,
				text
			});
		}

		public static LocalizedString ErrorEmailNotificationDeadLoop
		{
			get
			{
				return new LocalizedString("ErrorEmailNotificationDeadLoop", "ExC71CAD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNeutralCodingScheme
		{
			get
			{
				return new LocalizedString("ErrorNeutralCodingScheme", "ExCC2AFE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoProviderForNotification(string textMessagingSlabLink, string notificationSetupWizardLink)
		{
			return new LocalizedString("ErrorNoProviderForNotification", "ExE077BF", false, true, Strings.ResourceManager, new object[]
			{
				textMessagingSlabLink,
				notificationSetupWizardLink
			});
		}

		public static LocalizedString ErrorInvalidCalNotifContent(string content, string error)
		{
			return new LocalizedString("ErrorInvalidCalNotifContent", "Ex581BCF", false, true, Strings.ResourceManager, new object[]
			{
				content,
				error
			});
		}

		public static LocalizedString ErrorUnableDeliverForSmtpToSmsGateway(string recipient)
		{
			return new LocalizedString("ErrorUnableDeliverForSmtpToSmsGateway", "Ex4FB268", false, true, Strings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString ErrorServiceUnsupported(string type)
		{
			return new LocalizedString("ErrorServiceUnsupported", "Ex412370", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ErrorNotSupportM2pWhenEasEnabled(string ecpLink)
		{
			return new LocalizedString("ErrorNotSupportM2pWhenEasEnabled", "Ex07D127", false, true, Strings.ResourceManager, new object[]
			{
				ecpLink
			});
		}

		public static LocalizedString ErrorInvalidMobileSessionMode(string method, string mode)
		{
			return new LocalizedString("ErrorInvalidMobileSessionMode", "Ex96906E", false, true, Strings.ResourceManager, new object[]
			{
				method,
				mode
			});
		}

		public static LocalizedString ErrorUnknownCalNotifType(string type)
		{
			return new LocalizedString("ErrorUnknownCalNotifType", "Ex945030", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ConstNa
		{
			get
			{
				return new LocalizedString("ConstNa", "ExBF1BBB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidState(string name, string value)
		{
			return new LocalizedString("ErrorInvalidState", "Ex8D2802", false, true, Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString ErrorInvalidPhoneNumber
		{
			get
			{
				return new LocalizedString("ErrorInvalidPhoneNumber", "Ex2A46FF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyRecipients
		{
			get
			{
				return new LocalizedString("ErrorTooManyRecipients", "ExDA4CEE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString calNotifAllDayEventsDesc
		{
			get
			{
				return new LocalizedString("calNotifAllDayEventsDesc", "Ex9576F3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoM2pDeliveryPoint(string ecpLink)
		{
			return new LocalizedString("ErrorNoM2pDeliveryPoint", "Ex0DBFE1", false, true, Strings.ResourceManager, new object[]
			{
				ecpLink
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(12);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.TextMessaging.MobileDriver.Resources.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ConstNull = 684812396U,
			ErrorEmailMessageNotFound = 2765603176U,
			ErrorNotSupportMultimediaMessage = 268030502U,
			ErrorEmptyCalNotifContent = 298068888U,
			ErrorTooManyParts = 893814531U,
			ErrorAvaliableServiceNotFound = 3337871027U,
			ErrorEmailNotificationDeadLoop = 406232425U,
			ErrorNeutralCodingScheme = 597584490U,
			ConstNa = 1045457712U,
			ErrorInvalidPhoneNumber = 3260461220U,
			ErrorTooManyRecipients = 234345771U,
			calNotifAllDayEventsDesc = 4114712759U
		}

		private enum ParamIDs
		{
			ErrorNotAcknowledged,
			ErrorNoP2pDeliveryPoint,
			notifCountOfEventsDesc,
			ErrorUnableDeliverForEas,
			ErrorNoM2pDeliveryPointForEmailAlert,
			ErrorNoProviderForTextMessage,
			ErrorObjectNotFound,
			ErrorCannotParseSettings,
			ErrorCantBeCoded,
			ErrorNoProviderForNotification,
			ErrorInvalidCalNotifContent,
			ErrorUnableDeliverForSmtpToSmsGateway,
			ErrorServiceUnsupported,
			ErrorNotSupportM2pWhenEasEnabled,
			ErrorInvalidMobileSessionMode,
			ErrorUnknownCalNotifType,
			ErrorInvalidState,
			ErrorNoM2pDeliveryPoint
		}
	}
}
