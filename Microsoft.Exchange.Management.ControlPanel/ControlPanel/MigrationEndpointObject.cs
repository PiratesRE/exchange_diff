using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public sealed class MigrationEndpointObject
	{
		public MigrationEndpointObject(MigrationEndpoint endpoint)
		{
			this.Name = endpoint.Identity.Id;
			this.Identity = new Identity(endpoint.Identity);
			this.RemoteServer = endpoint.RemoteServer;
			this.Port = endpoint.Port;
			this.ExchangeServer = endpoint.ExchangeServer;
			this.RpcProxyServer = endpoint.RpcProxyServer;
			this.EndpointType = endpoint.EndpointType.ToString();
			this.MaxConcurrentMigrations = endpoint.MaxConcurrentMigrations.ToString();
			this.UserName = endpoint.Username;
			this.MailboxPermission = endpoint.MailboxPermission.ToString();
			if (endpoint.Authentication != null)
			{
				this.Authentication = endpoint.Authentication.ToString();
			}
		}

		[DataMember]
		public string Name { get; private set; }

		[DataMember]
		public Identity Identity { get; private set; }

		[DataMember]
		public string RemoteServer { get; private set; }

		[DataMember]
		public string ExchangeServer { get; private set; }

		[DataMember]
		public string RpcProxyServer { get; private set; }

		[DataMember]
		public int? Port { get; private set; }

		[DataMember]
		public string EndpointType { get; private set; }

		[DataMember]
		public string UserName { get; private set; }

		[DataMember]
		public string MaxConcurrentMigrations { get; private set; }

		[DataMember]
		public string MailboxPermission { get; private set; }

		[DataMember]
		public string Authentication { get; private set; }

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != base.GetType())
			{
				return false;
			}
			MigrationEndpointObject migrationEndpointObject = obj as MigrationEndpointObject;
			return migrationEndpointObject == this || (string.Equals(this.Name, migrationEndpointObject.Name) && string.Equals(this.RemoteServer, migrationEndpointObject.RemoteServer) && string.Equals(this.ExchangeServer, migrationEndpointObject.ExchangeServer) && string.Equals(this.RpcProxyServer, migrationEndpointObject.RpcProxyServer) && string.Equals(this.UserName, migrationEndpointObject.UserName) && this.Port == migrationEndpointObject.Port && this.EndpointType == migrationEndpointObject.EndpointType && this.MaxConcurrentMigrations == migrationEndpointObject.MaxConcurrentMigrations && string.Equals(this.MailboxPermission, migrationEndpointObject.MailboxPermission) && string.Equals(this.Authentication, migrationEndpointObject.Authentication));
		}
	}
}
