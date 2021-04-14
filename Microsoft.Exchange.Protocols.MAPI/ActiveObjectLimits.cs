using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class ActiveObjectLimits
	{
		public static long EffectiveLimitation(ClientType clientType)
		{
			switch (clientType)
			{
			case ClientType.System:
			case ClientType.Administrator:
				return ConfigurationSchema.PerAdminSessionLimit.Value;
			case ClientType.User:
				break;
			default:
				if (clientType != ClientType.MoMT)
				{
					return ConfigurationSchema.PerOtherSessionLimit.Value;
				}
				break;
			}
			return ConfigurationSchema.PerUserSessionLimit.Value;
		}

		public static long EffectiveLimitation(MapiObjectTrackedType trackedType)
		{
			ConfigurationSchema<long> configurationSchema = null;
			if (ActiveObjectLimits.perSessionLimitationMap.TryGetValue(trackedType, out configurationSchema) && configurationSchema != null)
			{
				return configurationSchema.Value;
			}
			return -1L;
		}

		public static long EffectiveLimitation(MapiServiceType serviceType)
		{
			return ConfigurationSchema.PerServiceSessionLimit.Value;
		}

		internal static void Initialize()
		{
			ActiveObjectLimits.perSessionLimitationMap = new Dictionary<MapiObjectTrackedType, ConfigurationSchema<long>>();
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.Folder, ConfigurationSchema.PerSessionFolderLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.Message, ConfigurationSchema.PerSessionMessageLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.Attachment, ConfigurationSchema.PerSessionAttachmentLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.Stream, ConfigurationSchema.PerSessionStreamLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.Notify, ConfigurationSchema.PerSessionNotifyLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.FolderView, ConfigurationSchema.PerSessionFolderViewLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.MessageView, ConfigurationSchema.PerSessionMessageViewLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.AttachmentView, ConfigurationSchema.PerSessionAttachmentViewLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.PermissionView, ConfigurationSchema.PerSessionACLViewLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.FastTransferSource, ConfigurationSchema.PerSessionFxSrcLimit);
			ActiveObjectLimits.perSessionLimitationMap.Add(MapiObjectTrackedType.FastTransferDestination, ConfigurationSchema.PerSessionFxDstLimit);
		}

		internal static void Terminate()
		{
		}

		private static Dictionary<MapiObjectTrackedType, ConfigurationSchema<long>> perSessionLimitationMap;
	}
}
