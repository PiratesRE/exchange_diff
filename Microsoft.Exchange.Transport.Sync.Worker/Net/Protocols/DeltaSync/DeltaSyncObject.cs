using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class DeltaSyncObject
	{
		internal DeltaSyncObject(Guid serverId)
		{
			this.serverId = serverId;
		}

		internal DeltaSyncObject(string clientId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("clientId", clientId);
			this.clientId = clientId;
			this.isClientObject = true;
		}

		internal bool IsClientObject
		{
			get
			{
				return this.isClientObject;
			}
		}

		internal string Id
		{
			get
			{
				if (!this.isClientObject)
				{
					return this.serverId.ToString();
				}
				return this.clientId;
			}
		}

		internal string ClientId
		{
			get
			{
				return this.clientId;
			}
		}

		internal Guid ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		internal DeltaSyncFolder Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		private Guid serverId;

		private string clientId;

		private bool isClientObject;

		private DeltaSyncFolder parent;
	}
}
