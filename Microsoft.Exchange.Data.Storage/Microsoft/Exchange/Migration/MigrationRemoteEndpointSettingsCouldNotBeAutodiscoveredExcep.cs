using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationRemoteEndpointSettingsCouldNotBeAutodiscoveredException : MigrationPermanentException
	{
		public MigrationRemoteEndpointSettingsCouldNotBeAutodiscoveredException(string serverName) : base(Strings.CouldNotDetermineExchangeRemoteSettings(serverName))
		{
			this.serverName = serverName;
		}

		public MigrationRemoteEndpointSettingsCouldNotBeAutodiscoveredException(string serverName, Exception innerException) : base(Strings.CouldNotDetermineExchangeRemoteSettings(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected MigrationRemoteEndpointSettingsCouldNotBeAutodiscoveredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
