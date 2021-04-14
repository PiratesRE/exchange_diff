using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class ServerPickerClient
	{
		public virtual ADPropertyDefinition OverrideListPropertyDefinition
		{
			get
			{
				return null;
			}
		}

		public virtual PickerServer CreatePickerServer(Server server, PickerServerList pickerServerList)
		{
			return new PickerServer(server, pickerServerList);
		}

		public virtual void LogNoActiveServerEvent(string serverRole)
		{
		}

		public virtual void UpdateServersTotalPerfmon(int count)
		{
		}

		public virtual void UpdateServersInRetryPerfmon(int count)
		{
		}

		public virtual void UpdateServersPercentageActivePerfmon(int percentage)
		{
		}

		public virtual void LocalServerDiscovered(Server server)
		{
		}

		public virtual void ServerListUpdated(List<PickerServer> serverList)
		{
		}

		internal static readonly ServerPickerClient Default = new ServerPickerClient();
	}
}
