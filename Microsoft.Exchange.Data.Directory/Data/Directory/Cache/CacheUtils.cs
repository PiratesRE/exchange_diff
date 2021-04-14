using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal static class CacheUtils
	{
		internal static void PopulateAndCheckObjectType<TObject>(DirectoryCacheRequest cacheRequest)
		{
			ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
			ObjectType objectType = cacheRequest.ObjectType;
			cacheRequest.ObjectType = CacheUtils.GetObjectTypeFor(typeof(TObject), true);
		}

		internal static ObjectType GetObjectTypeFor(Type type, bool throwIfUnknownType = true)
		{
			if (typeof(ExchangeConfigurationUnit) == type)
			{
				return ObjectType.ExchangeConfigurationUnit;
			}
			if (typeof(ADRecipient) == type || typeof(ADRecipient).IsAssignableFrom(type))
			{
				return ObjectType.Recipient;
			}
			if (typeof(ADRawEntry) == type)
			{
				return ObjectType.ADRawEntry;
			}
			if (typeof(AcceptedDomain) == type)
			{
				return ObjectType.AcceptedDomain;
			}
			if (typeof(FederatedOrganizationId) == type)
			{
				return ObjectType.FederatedOrganizationId;
			}
			if (typeof(MiniRecipient) == type)
			{
				return ObjectType.MiniRecipient;
			}
			if (typeof(OWAMiniRecipient) == type)
			{
				return ObjectType.OWAMiniRecipient;
			}
			if (typeof(TransportMiniRecipient) == type)
			{
				return ObjectType.TransportMiniRecipient;
			}
			if (typeof(LoadBalancingMiniRecipient) == type)
			{
				return ObjectType.LoadBalancingMiniRecipient;
			}
			if (typeof(FrontEndMiniRecipient) == type)
			{
				return ObjectType.FrontEndMiniRecipient;
			}
			if (typeof(ActiveSyncMiniRecipient) == type)
			{
				return ObjectType.ActiveSyncMiniRecipient;
			}
			if (typeof(StorageMiniRecipient) == type)
			{
				return ObjectType.StorageMiniRecipient;
			}
			if (typeof(MiniRecipientWithTokenGroups) == type)
			{
				return ObjectType.MiniRecipientWithTokenGroups;
			}
			if (throwIfUnknownType)
			{
				throw new NotSupportedException(string.Format("Type {0} is not supported for caching.", type.Name));
			}
			return ObjectType.Unknown;
		}

		internal static bool IsTenantScopedObject(ObjectType objectType)
		{
			if (objectType <= ObjectType.TransportMiniRecipient)
			{
				if (objectType <= ObjectType.FederatedOrganizationId)
				{
					switch (objectType)
					{
					case ObjectType.ExchangeConfigurationUnit:
					case ObjectType.AcceptedDomain:
						return false;
					case ObjectType.Recipient:
						break;
					case ObjectType.ExchangeConfigurationUnit | ObjectType.Recipient:
						goto IL_6C;
					default:
						if (objectType != ObjectType.FederatedOrganizationId)
						{
							goto IL_6C;
						}
						break;
					}
				}
				else if (objectType != ObjectType.MiniRecipient && objectType != ObjectType.TransportMiniRecipient)
				{
					goto IL_6C;
				}
			}
			else if (objectType <= ObjectType.ActiveSyncMiniRecipient)
			{
				if (objectType != ObjectType.OWAMiniRecipient && objectType != ObjectType.ActiveSyncMiniRecipient)
				{
					goto IL_6C;
				}
			}
			else
			{
				if (objectType == ObjectType.ADRawEntry)
				{
					return false;
				}
				if (objectType != ObjectType.StorageMiniRecipient && objectType != ObjectType.MiniRecipientWithTokenGroups)
				{
					goto IL_6C;
				}
			}
			return true;
			IL_6C:
			throw new NotSupportedException("ObjectType not supportted");
		}

		internal static bool IsPowerOf2(int value)
		{
			return value != 0 && (value & value - 1) == 0;
		}
	}
}
