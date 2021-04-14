using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess
{
	internal class UMServerManager
	{
		internal static UMServerProxy GetServerByDialplan(ADObjectId dialPlanId)
		{
			Guid context = (dialPlanId != null) ? dialPlanId.ObjectGuid : Guid.Empty;
			UMServerProxy umserverProxy = (UMServerProxy)UMServerManager.PlayOnPhoneUMServerPicker.Instance.PickNextServer(context);
			if (umserverProxy == null)
			{
				throw new UMServerNotFoundDialPlanException(context.ToString());
			}
			return umserverProxy;
		}

		internal static UMServerProxy GetServer()
		{
			return UMServerManager.GetServerByDialplan(null);
		}

		internal static UMServerProxy GetServer(string fqdn)
		{
			return new UMServerProxy(fqdn);
		}

		internal static void ClearServerCache()
		{
			UMServerManager.PlayOnPhoneUMServerPicker.Instance.ClearServerCache();
		}

		internal static bool IsAuthorizedUMServer(UMServerProxy umServerProxy)
		{
			bool result = false;
			List<IVersionedRpcTarget> registeredUMServers = UMServerManager.PlayOnPhoneUMServerPicker.Instance.GetRegisteredUMServers();
			foreach (IVersionedRpcTarget versionedRpcTarget in registeredUMServers)
			{
				UMServerProxy umserverProxy = (UMServerProxy)versionedRpcTarget;
				if (umserverProxy != null && umserverProxy.Fqdn != null && umserverProxy.Fqdn.Equals(umServerProxy.Fqdn))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private class PlayOnPhoneUMServerPicker : UMServerRpcTargetPickerBase<IVersionedRpcTarget>
		{
			public void ClearServerCache()
			{
				base.RefreshServers();
			}

			protected override IVersionedRpcTarget CreateTarget(Server server)
			{
				return new UMServerProxy(server);
			}

			public static readonly UMServerManager.PlayOnPhoneUMServerPicker Instance = new UMServerManager.PlayOnPhoneUMServerPicker();
		}
	}
}
