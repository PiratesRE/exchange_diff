using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3962995036U, "InvalidSerializedToken");
			Strings.stringIDs.Add(2766800417U, "InvalidListOfAppIds");
			Strings.stringIDs.Add(3216875091U, "InvalidRecipientsList");
			Strings.stringIDs.Add(4087543273U, "InvalidMnPayloadContent");
			Strings.stringIDs.Add(333453583U, "InvalidNotificationIdentifier");
			Strings.stringIDs.Add(816696259U, "InvalidPayload");
			Strings.stringIDs.Add(757184198U, "InvalidPlatform");
			Strings.stringIDs.Add(2677654617U, "InvalidAppId");
			Strings.stringIDs.Add(162180343U, "InvalidRecipientDeviceId");
			Strings.stringIDs.Add(2790684056U, "InvalidRecipient");
			Strings.stringIDs.Add(1125823870U, "InvalidRecipientAppId");
			Strings.stringIDs.Add(1327410019U, "InvalidWorkloadId");
			Strings.stringIDs.Add(3937582550U, "OutlookInvalidPayloadData");
		}

		public static LocalizedString InvalidSerializedToken
		{
			get
			{
				return new LocalizedString("InvalidSerializedToken", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidListOfAppIds
		{
			get
			{
				return new LocalizedString("InvalidListOfAppIds", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTenantId(string tenantId)
		{
			return new LocalizedString("InvalidTenantId", Strings.ResourceManager, new object[]
			{
				tenantId
			});
		}

		public static LocalizedString InvalidRecipientsList
		{
			get
			{
				return new LocalizedString("InvalidRecipientsList", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationServerExceptionMessage(string messageBody)
		{
			return new LocalizedString("PushNotificationServerExceptionMessage", Strings.ResourceManager, new object[]
			{
				messageBody
			});
		}

		public static LocalizedString ExceptionMessageTimeoutCall(string target, string message)
		{
			return new LocalizedString("ExceptionMessageTimeoutCall", Strings.ResourceManager, new object[]
			{
				target,
				message
			});
		}

		public static LocalizedString InvalidMnPayloadContent
		{
			get
			{
				return new LocalizedString("InvalidMnPayloadContent", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNotificationIdentifier
		{
			get
			{
				return new LocalizedString("InvalidNotificationIdentifier", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPayload
		{
			get
			{
				return new LocalizedString("InvalidPayload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPlatform
		{
			get
			{
				return new LocalizedString("InvalidPlatform", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAppId
		{
			get
			{
				return new LocalizedString("InvalidAppId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionPushNotificationError(string server, string error)
		{
			return new LocalizedString("ExceptionPushNotificationError", Strings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString InvalidMnRecipientLastSubscriptionUpdate(string date)
		{
			return new LocalizedString("InvalidMnRecipientLastSubscriptionUpdate", Strings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString InvalidRecipientDeviceId
		{
			get
			{
				return new LocalizedString("InvalidRecipientDeviceId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRecipient
		{
			get
			{
				return new LocalizedString("InvalidRecipient", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRecipientAppId
		{
			get
			{
				return new LocalizedString("InvalidRecipientAppId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidWorkloadId
		{
			get
			{
				return new LocalizedString("InvalidWorkloadId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionEndpointNotFoundError(string server, string error)
		{
			return new LocalizedString("ExceptionEndpointNotFoundError", Strings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString InvalidTargetAppId(string notificationType)
		{
			return new LocalizedString("InvalidTargetAppId", Strings.ResourceManager, new object[]
			{
				notificationType
			});
		}

		public static LocalizedString OutlookInvalidPayloadData
		{
			get
			{
				return new LocalizedString("OutlookInvalidPayloadData", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(13);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.PushNotifications.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidSerializedToken = 3962995036U,
			InvalidListOfAppIds = 2766800417U,
			InvalidRecipientsList = 3216875091U,
			InvalidMnPayloadContent = 4087543273U,
			InvalidNotificationIdentifier = 333453583U,
			InvalidPayload = 816696259U,
			InvalidPlatform = 757184198U,
			InvalidAppId = 2677654617U,
			InvalidRecipientDeviceId = 162180343U,
			InvalidRecipient = 2790684056U,
			InvalidRecipientAppId = 1125823870U,
			InvalidWorkloadId = 1327410019U,
			OutlookInvalidPayloadData = 3937582550U
		}

		private enum ParamIDs
		{
			InvalidTenantId,
			PushNotificationServerExceptionMessage,
			ExceptionMessageTimeoutCall,
			ExceptionPushNotificationError,
			InvalidMnRecipientLastSubscriptionUpdate,
			ExceptionEndpointNotFoundError,
			InvalidTargetAppId
		}
	}
}
