using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCommon.Outdialing
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1751675783U, "NumberNotInStandardFormatNoRecipient");
			Strings.stringIDs.Add(3195800463U, "SkippingTargetDialPlan");
			Strings.stringIDs.Add(1369735291U, "CanonicalizationFailed");
		}

		public static LocalizedString NumberNotInStandardFormatNoRecipient
		{
			get
			{
				return new LocalizedString("NumberNotInStandardFormatNoRecipient", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPlayOnPhoneNumber(string phoneNumber)
		{
			return new LocalizedString("InvalidPlayOnPhoneNumber", Strings.ResourceManager, new object[]
			{
				phoneNumber
			});
		}

		public static LocalizedString DialPlanPropertyNotSet(string propertyName, string dialPlan)
		{
			return new LocalizedString("DialPlanPropertyNotSet", Strings.ResourceManager, new object[]
			{
				propertyName,
				dialPlan
			});
		}

		public static LocalizedString SkippingTargetDialPlan
		{
			get
			{
				return new LocalizedString("SkippingTargetDialPlan", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessCheckFailed(string phoneNumber)
		{
			return new LocalizedString("AccessCheckFailed", Strings.ResourceManager, new object[]
			{
				phoneNumber
			});
		}

		public static LocalizedString InvalidRecipientPhoneLength(string recipient, string dialPlan)
		{
			return new LocalizedString("InvalidRecipientPhoneLength", Strings.ResourceManager, new object[]
			{
				recipient,
				dialPlan
			});
		}

		public static LocalizedString NumberNotInStandardFormat(string recipient)
		{
			return new LocalizedString("NumberNotInStandardFormat", Strings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString CanonicalizationFailed
		{
			get
			{
				return new LocalizedString("CanonicalizationFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanonicalizationResult(string phoneNumber)
		{
			return new LocalizedString("CanonicalizationResult", Strings.ResourceManager, new object[]
			{
				phoneNumber
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(3);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.UM.UMCommon.Outdialing.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NumberNotInStandardFormatNoRecipient = 1751675783U,
			SkippingTargetDialPlan = 3195800463U,
			CanonicalizationFailed = 1369735291U
		}

		private enum ParamIDs
		{
			InvalidPlayOnPhoneNumber,
			DialPlanPropertyNotSet,
			AccessCheckFailed,
			InvalidRecipientPhoneLength,
			NumberNotInStandardFormat,
			CanonicalizationResult
		}
	}
}
