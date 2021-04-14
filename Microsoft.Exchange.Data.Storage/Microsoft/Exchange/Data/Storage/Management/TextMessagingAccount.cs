using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.VersionedXml;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class TextMessagingAccount : VersionedXmlConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return TextMessagingAccount.schema;
			}
		}

		public TextMessagingAccount()
		{
			((SimplePropertyBag)this.propertyBag).SetObjectIdentityPropertyDefinition(VersionedXmlConfigurationObjectSchema.Identity);
		}

		public override ObjectId Identity
		{
			get
			{
				return (ADObjectId)this[VersionedXmlConfigurationObjectSchema.Identity];
			}
		}

		[Parameter]
		public RegionInfo CountryRegionId
		{
			get
			{
				return (RegionInfo)this[TextMessagingAccountSchema.CountryRegionId];
			}
			set
			{
				this[TextMessagingAccountSchema.CountryRegionId] = value;
			}
		}

		[Parameter]
		public int MobileOperatorId
		{
			get
			{
				return (int)this[TextMessagingAccountSchema.MobileOperatorId];
			}
			set
			{
				this[TextMessagingAccountSchema.MobileOperatorId] = value;
			}
		}

		[Parameter]
		public E164Number NotificationPhoneNumber
		{
			get
			{
				return (E164Number)this[TextMessagingAccountSchema.NotificationPhoneNumber];
			}
			set
			{
				this[TextMessagingAccountSchema.NotificationPhoneNumber] = value;
			}
		}

		public bool NotificationPhoneNumberVerified
		{
			get
			{
				return (bool)this[TextMessagingAccountSchema.NotificationPhoneNumberVerified];
			}
		}

		public bool EasEnabled
		{
			get
			{
				return (bool)this[TextMessagingAccountSchema.EasEnabled];
			}
		}

		public E164Number EasPhoneNumber
		{
			get
			{
				return (E164Number)this[TextMessagingAccountSchema.EasPhoneNumber];
			}
		}

		public string EasDeviceProtocol
		{
			get
			{
				return (string)this[TextMessagingAccountSchema.EasDeviceProtocol];
			}
		}

		public string EasDeviceType
		{
			get
			{
				return (string)this[TextMessagingAccountSchema.EasDeviceType];
			}
		}

		public string EasDeviceId
		{
			get
			{
				return (string)this[TextMessagingAccountSchema.EasDeviceId];
			}
		}

		public string EasDeviceName
		{
			get
			{
				return (string)this[TextMessagingAccountSchema.EasDeviceName];
			}
		}

		internal TextMessagingSettingsVersion1Point0 TextMessagingSettings
		{
			get
			{
				return (TextMessagingSettingsVersion1Point0)this[TextMessagingAccountSchema.TextMessagingSettings];
			}
			set
			{
				this[TextMessagingAccountSchema.TextMessagingSettings] = value;
			}
		}

		internal static object TextMessagingSettingsGetter(IPropertyBag propertyBag)
		{
			if (propertyBag[TextMessagingAccountSchema.RawTextMessagingSettings] == null)
			{
				bool flag = ((PropertyBag)propertyBag).IsChanged(TextMessagingAccountSchema.RawTextMessagingSettings);
				propertyBag[TextMessagingAccountSchema.RawTextMessagingSettings] = new TextMessagingSettingsVersion1Point0(null, new DeliveryPoint[]
				{
					new DeliveryPoint(0, DeliveryPointType.ExchangeActiveSync, null, null, null, null, null, -1, -1),
					new DeliveryPoint(1, DeliveryPointType.SmtpToSmsGateway, null, null, null, null, null, -1, 1)
				});
				if (!flag)
				{
					((PropertyBag)propertyBag).ResetChangeTracking(TextMessagingAccountSchema.RawTextMessagingSettings);
				}
			}
			return (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.RawTextMessagingSettings];
		}

		internal static void TextMessagingSettingsSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[TextMessagingAccountSchema.RawTextMessagingSettings] = CloneHelper.SerializeObj(value);
		}

		internal static object CountryRegionIdGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			IList<PossibleRecipient> effectivePossibleRecipients = textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.EffectivePossibleRecipients;
			if (effectivePossibleRecipients.Count == 0)
			{
				return null;
			}
			string region = effectivePossibleRecipients[0].Region;
			if (!string.IsNullOrEmpty(region))
			{
				return new RegionInfo(region);
			}
			return null;
		}

		internal static void CountryRegionIdSetter(object value, IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 settings = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			string region = (value == null) ? null : ((RegionInfo)value).TwoLetterISORegionName;
			TextMessagingAccount.GetWritablePossibleRecipient(settings).Region = region;
		}

		internal static object MobileOperatorIdGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			IList<PossibleRecipient> effectivePossibleRecipients = textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.EffectivePossibleRecipients;
			if (effectivePossibleRecipients.Count == 0)
			{
				return -1;
			}
			int num = -1;
			if (!int.TryParse(effectivePossibleRecipients[0].Carrier, out num))
			{
				num = -1;
			}
			return num;
		}

		internal static void MobileOperatorIdSetter(object value, IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 settings = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			string carrier = (-1 == (int)value) ? null : ((int)value).ToString("00000");
			TextMessagingAccount.GetWritablePossibleRecipient(settings).Carrier = carrier;
		}

		internal static object NotificationPhoneNumberGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			IList<PossibleRecipient> effectivePossibleRecipients = textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.EffectivePossibleRecipients;
			if (effectivePossibleRecipients.Count == 0)
			{
				return null;
			}
			return effectivePossibleRecipients[0].PhoneNumber;
		}

		internal static void NotificationPhoneNumberSetter(object value, IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			if (value == null)
			{
				PossibleRecipient.MarkEffective(textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.PossibleRecipients, false);
				return;
			}
			bool flag = false;
			PossibleRecipient writablePossibleRecipient = TextMessagingAccount.GetWritablePossibleRecipient(textMessagingSettingsVersion1Point, out flag);
			if (flag)
			{
				writablePossibleRecipient.PhoneNumber = (E164Number)value;
			}
			else
			{
				PossibleRecipient mathed = PossibleRecipient.GetMathed(textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.PossibleRecipients, (E164Number)value, false);
				PossibleRecipient.MarkEffective(textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.PossibleRecipients, false);
				if (mathed == null)
				{
					textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.PossibleRecipients.Add(new PossibleRecipient(true, DateTime.UtcNow, writablePossibleRecipient.Region, writablePossibleRecipient.Carrier, (E164Number)value, false, null, null, null));
				}
				else
				{
					mathed.Region = writablePossibleRecipient.Region;
					mathed.Carrier = writablePossibleRecipient.Carrier;
					mathed.MarkEffective(true);
				}
			}
			PossibleRecipient.PurgeNonEffectiveBefore(textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.PossibleRecipients, DateTime.UtcNow - TimeSpan.FromDays(7.0), 10);
		}

		internal static object NotificationPhoneNumberVerifiedGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			IList<PossibleRecipient> effectivePossibleRecipients = textMessagingSettingsVersion1Point.MachineToPersonMessagingPolicies.EffectivePossibleRecipients;
			return effectivePossibleRecipients.Count != 0 && effectivePossibleRecipients[0].Acknowledged;
		}

		internal static object EasEnabledGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			return textMessagingSettingsVersion1Point.DeliveryPoints[0].Ready && (-1 != textMessagingSettingsVersion1Point.DeliveryPoints[0].P2pMessagingPriority || -1 != textMessagingSettingsVersion1Point.DeliveryPoints[0].M2pMessagingPriority);
		}

		internal static object EasPhoneNumberGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			return textMessagingSettingsVersion1Point.DeliveryPoints[0].PhoneNumber;
		}

		internal static object EasDeviceProtocolGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			return textMessagingSettingsVersion1Point.DeliveryPoints[0].Protocol;
		}

		internal static object EasDeviceTypeGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			return textMessagingSettingsVersion1Point.DeliveryPoints[0].DeviceType;
		}

		internal static object EasDeviceIdGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			return textMessagingSettingsVersion1Point.DeliveryPoints[0].DeviceId;
		}

		internal static object EasDeviceFriendlyNameGetter(IPropertyBag propertyBag)
		{
			TextMessagingSettingsVersion1Point0 textMessagingSettingsVersion1Point = (TextMessagingSettingsVersion1Point0)propertyBag[TextMessagingAccountSchema.TextMessagingSettings];
			return textMessagingSettingsVersion1Point.DeliveryPoints[0].DeviceFriendlyName;
		}

		private static PossibleRecipient GetWritablePossibleRecipient(TextMessagingSettingsVersion1Point0 settings)
		{
			bool flag = false;
			return TextMessagingAccount.GetWritablePossibleRecipient(settings, out flag);
		}

		private static PossibleRecipient GetWritablePossibleRecipient(TextMessagingSettingsVersion1Point0 settings, out bool created)
		{
			created = false;
			IList<PossibleRecipient> list = settings.MachineToPersonMessagingPolicies.EffectivePossibleRecipients;
			if (list.Count == 0)
			{
				list = settings.MachineToPersonMessagingPolicies.PossibleRecipients;
			}
			if (list.Count == 0)
			{
				list.Add(new PossibleRecipient(true, DateTime.UtcNow, null, null, null, false, null, null, null));
				created = true;
			}
			return list[0];
		}

		private void UpdateOnceOrignalEasEnabled()
		{
			if (this.originalEasEnabled != null)
			{
				return;
			}
			this.originalEasEnabled = new bool?(this.EasEnabled);
		}

		internal bool OrignalEasEnabled
		{
			get
			{
				bool? flag = this.originalEasEnabled;
				if (flag == null)
				{
					return this.EasEnabled;
				}
				return flag.GetValueOrDefault();
			}
		}

		internal void SetEasEnabled(E164Number number, string protocol, string deviceType, string deviceId, string deviceFriendlyName)
		{
			if (null == number)
			{
				throw new ArgumentNullException("number");
			}
			this.UpdateOnceOrignalEasEnabled();
			this.TextMessagingSettings.DeliveryPoints[0].P2pMessagingPriority = 0;
			this.TextMessagingSettings.DeliveryPoints[0].M2pMessagingPriority = -1;
			this.TextMessagingSettings.DeliveryPoints[1].P2pMessagingPriority = -1;
			this.TextMessagingSettings.DeliveryPoints[1].M2pMessagingPriority = 1;
			this.TextMessagingSettings.DeliveryPoints[0].PhoneNumber = number;
			this.TextMessagingSettings.DeliveryPoints[0].Protocol = protocol;
			this.TextMessagingSettings.DeliveryPoints[0].DeviceType = deviceType;
			this.TextMessagingSettings.DeliveryPoints[0].DeviceId = deviceId;
			this.TextMessagingSettings.DeliveryPoints[0].DeviceFriendlyName = deviceFriendlyName;
		}

		internal void SetEasDisabled()
		{
			this.UpdateOnceOrignalEasEnabled();
			this.TextMessagingSettings.DeliveryPoints[0].P2pMessagingPriority = -1;
			this.TextMessagingSettings.DeliveryPoints[0].M2pMessagingPriority = -1;
			this.TextMessagingSettings.DeliveryPoints[1].P2pMessagingPriority = -1;
			this.TextMessagingSettings.DeliveryPoints[1].M2pMessagingPriority = 1;
			this.TextMessagingSettings.DeliveryPoints[0].PhoneNumber = null;
			this.TextMessagingSettings.DeliveryPoints[0].Protocol = null;
			this.TextMessagingSettings.DeliveryPoints[0].DeviceType = null;
			this.TextMessagingSettings.DeliveryPoints[0].DeviceId = null;
			this.TextMessagingSettings.DeliveryPoints[0].DeviceFriendlyName = null;
		}

		internal CultureInfo NotificationPreferredCulture { get; set; }

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return base.ToString();
		}

		internal override string UserConfigurationName
		{
			get
			{
				return "TextMessaging.001";
			}
		}

		internal override ProviderPropertyDefinition RawVersionedXmlPropertyDefinition
		{
			get
			{
				return TextMessagingAccountSchema.RawTextMessagingSettings;
			}
		}

		internal const int DefaultEasP2pPriority = 0;

		internal const int DefaultEasM2pPriority = -1;

		internal const int DefaultSmtp2SmsGatewayP2pPriority = -1;

		internal const int DefaultSmtp2SmsGatewayM2pPriority = 1;

		internal const int EasDeliveryPointIndex = 0;

		internal const int Smtp2SmsGatewayDeliveryPointIndex = 1;

		internal const string ConfigurationName = "TextMessaging.001";

		private static XsoMailboxConfigurationObjectSchema schema = ObjectSchema.GetInstance<TextMessagingAccountSchema>();

		private bool? originalEasEnabled;
	}
}
