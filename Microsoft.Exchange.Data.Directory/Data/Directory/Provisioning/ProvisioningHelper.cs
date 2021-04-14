using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	internal static class ProvisioningHelper
	{
		public static string GetProvisioningObjectTag(Type poType)
		{
			if (null == poType)
			{
				throw new ArgumentNullException("poType");
			}
			if (!ProvisioningHelper.poType2Tag.ContainsKey(poType))
			{
				lock (ProvisioningHelper.poType2Tag)
				{
					if (!ProvisioningHelper.poType2Tag.ContainsKey(poType))
					{
						object[] customAttributes = poType.GetCustomAttributes(typeof(ProvisioningObjectTagAttribute), false);
						if (customAttributes == null || customAttributes.Length == 0)
						{
							throw new ArgumentException(string.Format("The presentation object data type '{0}' does not associate with any provisiong tag. Please use the custom attribute ProvisioningObjectTagAttribute for the purpose.", poType));
						}
						ProvisioningHelper.poType2Tag.Add(poType, ((ProvisioningObjectTagAttribute)customAttributes[0]).Tag);
					}
				}
			}
			return ProvisioningHelper.poType2Tag[poType];
		}

		public static ADPropertyDefinition FromProvisionedADProperty(ADPropertyDefinition provisionedProperty, string name, string ldapDisplayName)
		{
			if (provisionedProperty == null)
			{
				throw new ArgumentNullException("provisionedProperty");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(ldapDisplayName))
			{
				throw new ArgumentNullException("ldapDisplayName");
			}
			if ((provisionedProperty.Flags & ADPropertyDefinitionFlags.ReadOnly) != ADPropertyDefinitionFlags.None)
			{
				throw new ArgumentException(string.Format("It does not make sense to create provisioning template for ReadOnly property: {0}.", provisionedProperty.Name));
			}
			ADPropertyDefinitionFlags flags = provisionedProperty.Flags & ~(ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.WriteOnce | ADPropertyDefinitionFlags.TaskPopulated);
			Type type = provisionedProperty.Type;
			PropertyDefinitionConstraint[] array = new PropertyDefinitionConstraint[provisionedProperty.AllConstraints.Count];
			provisionedProperty.AllConstraints.CopyTo(array, 0);
			object defaultValue = null;
			if (provisionedProperty.Type.IsValueType)
			{
				if (!provisionedProperty.Type.IsGenericType || (provisionedProperty.Type.GetGenericTypeDefinition() != typeof(Nullable<>) && provisionedProperty.Type.GetGenericTypeDefinition() != typeof(Unlimited<>)))
				{
					type = typeof(Nullable<>).MakeGenericType(new Type[]
					{
						provisionedProperty.Type
					});
					defaultValue = null;
				}
				else if (provisionedProperty.Type.IsGenericType && provisionedProperty.Type.GetGenericTypeDefinition() == typeof(Unlimited<>))
				{
					defaultValue = provisionedProperty.DefaultValue;
				}
			}
			else if (provisionedProperty.Type == typeof(string))
			{
				defaultValue = string.Empty;
			}
			return new ADPropertyDefinition(name, ExchangeObjectVersion.Exchange2010, type, provisionedProperty.FormatProvider, ldapDisplayName, flags, defaultValue, array, PropertyDefinitionConstraint.None, ProviderPropertyDefinition.None, null, null, null, null, null);
		}

		private static Dictionary<Type, string> poType2Tag = new Dictionary<Type, string>();

		internal static readonly Type[] AllSupportedRecipientTypes = new Type[]
		{
			typeof(Mailbox),
			typeof(MailUser),
			typeof(RemoteMailbox),
			typeof(DistributionGroup),
			typeof(MailContact),
			typeof(MailPublicFolder),
			typeof(ADPublicFolder),
			typeof(DynamicDistributionGroup),
			typeof(SyncMailbox),
			typeof(SyncMailUser),
			typeof(SyncMailContact),
			typeof(SyncDistributionGroup)
		};

		public static readonly Unlimited<ByteQuantifiedSize> DefaultArchiveQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(100UL));

		public static readonly Unlimited<ByteQuantifiedSize> DefaultArchiveWarningQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(90UL));

		public static readonly Unlimited<ByteQuantifiedSize> DefaultRecoverableItemsQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(30UL));

		public static readonly Unlimited<ByteQuantifiedSize> DefaultRecoverableItemsWarningQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(20UL));

		public static readonly Unlimited<ByteQuantifiedSize> DefaultCalendarLoggingQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(6UL));
	}
}
