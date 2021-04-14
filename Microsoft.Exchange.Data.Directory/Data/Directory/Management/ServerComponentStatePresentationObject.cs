using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class ServerComponentStatePresentationObject : ConfigurableObject
	{
		public ServerComponentStatePresentationObject() : base(new SimpleProviderPropertyBag())
		{
		}

		public ServerComponentStatePresentationObject(ADObjectId serverId, string serverFqdn, string componentName, MultiValuedProperty<string> componentStates) : base(new SimpleProviderPropertyBag())
		{
			this[SimpleProviderObjectSchema.Identity] = serverId;
			this.ServerFqdn = serverFqdn;
			this.Component = componentName;
			bool serverWideOfflineIsOffline;
			List<ServerComponentStates.ItemEntry> list;
			List<ServerComponentStates.ItemEntry> list2;
			this.State = ServerComponentStates.ReadEffectiveComponentState(this.ServerFqdn, componentStates, ServerComponentStateSources.All, this.Component, ServerComponentStateManager.GetDefaultState(this.Component), out serverWideOfflineIsOffline, out list, out list2);
			this.ServerWideOfflineIsOffline = serverWideOfflineIsOffline;
			if (list != null)
			{
				this.localStates = new List<ServerComponentStatePresentationObject.RequesterDetails>(list.Count);
				foreach (ServerComponentStates.ItemEntry info in list)
				{
					this.localStates.Add(new ServerComponentStatePresentationObject.RequesterDetails(info));
				}
			}
			if (list2 != null)
			{
				this.remoteStates = new List<ServerComponentStatePresentationObject.RequesterDetails>(list2.Count);
				foreach (ServerComponentStates.ItemEntry info2 in list2)
				{
					this.remoteStates.Add(new ServerComponentStatePresentationObject.RequesterDetails(info2));
				}
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ServerComponentStatePresentationObject.schema;
			}
		}

		public string ServerFqdn { get; set; }

		public string Component { get; set; }

		public ServiceState State { get; private set; }

		public bool ServerWideOfflineIsOffline { get; private set; }

		public List<ServerComponentStatePresentationObject.RequesterDetails> RemoteStates
		{
			get
			{
				return this.remoteStates;
			}
		}

		public List<ServerComponentStatePresentationObject.RequesterDetails> LocalStates
		{
			get
			{
				return this.localStates;
			}
		}

		private static ServerComponentStateSchema schema = ObjectSchema.GetInstance<ServerComponentStateSchema>();

		private List<ServerComponentStatePresentationObject.RequesterDetails> remoteStates;

		private List<ServerComponentStatePresentationObject.RequesterDetails> localStates;

		[Serializable]
		public class RequesterDetails
		{
			internal RequesterDetails(ServerComponentStates.ItemEntry info)
			{
				this.Requester = info.Requester;
				this.Component = info.Component;
				this.State = info.State;
				this.Timestamp = info.Timestamp;
			}

			public string Requester { get; private set; }

			public ServiceState State { get; private set; }

			public DateTime Timestamp { get; private set; }

			public string Component { get; private set; }
		}
	}
}
