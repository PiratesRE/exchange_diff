using System;
using System.Linq;
using System.Management;
using System.Net;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network
{
	internal class WmiDnsClient
	{
		public WmiDnsClient(string serverFqdn)
		{
			WmiDnsClient.ValidateArgumentNotNullOrWhiteSpace(serverFqdn, "serverFqdn");
			if (!serverFqdn.Contains("."))
			{
				throw new ArgumentException("The server name must be a fully-qualified domain name.", "serverFqdn");
			}
			this.serverFqdn = serverFqdn;
			try
			{
				this.scope = new ManagementScope(new ManagementPath
				{
					Server = serverFqdn,
					NamespacePath = "root\\MicrosoftDNS"
				});
				this.scope.Connect();
			}
			catch (SystemException innerException)
			{
				throw new ApplicationException(string.Format("Failed to connect to DNS service on machine '{0}'.", serverFqdn), innerException);
			}
		}

		public IPAddress[] GetForwarders()
		{
			IPAddress[] result;
			using (ManagementObject server = this.GetServer())
			{
				result = WmiDnsClient.ConvertToIpAddresses((string[])server["Forwarders"]);
			}
			return result;
		}

		public void SetForwarders(IPAddress[] forwarders)
		{
			using (ManagementObject server = this.GetServer())
			{
				server["Forwarders"] = WmiDnsClient.ConvertFromIpAddresses(forwarders);
				server.Put();
			}
		}

		private static string[] ConvertFromIpAddresses(IPAddress[] ipAddresses)
		{
			if (ipAddresses == null)
			{
				return null;
			}
			return (from ip in ipAddresses
			select ip.ToString()).ToArray<string>();
		}

		private static IPAddress[] ConvertToIpAddresses(string[] stringAddresses)
		{
			if (stringAddresses == null || stringAddresses.Length == 0)
			{
				return null;
			}
			return (from s in stringAddresses
			select IPAddress.Parse(s)).ToArray<IPAddress>();
		}

		private static void ValidateArgumentNotNull(object argument, string name)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		private static void ValidateArgumentNotNullOrEmpty(string argument, string name)
		{
			if (string.IsNullOrEmpty(argument))
			{
				throw new ArgumentNullException(name, "The value is null or empty.");
			}
		}

		private static void ValidateArgumentNotNullOrWhiteSpace(string argument, string name)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				throw new ArgumentNullException(name, "The value is null or white-space.");
			}
		}

		private ManagementObject GetObject(ManagementPath path)
		{
			ManagementObject result;
			try
			{
				ManagementObject managementObject = new ManagementObject
				{
					Scope = this.scope,
					Path = path
				};
				managementObject.Get();
				result = managementObject;
			}
			catch (ManagementException)
			{
				result = null;
			}
			return result;
		}

		private ManagementObject GetServer()
		{
			WmiDnsClient.ManagementPathComposable managementPathComposable = new WmiDnsClient.ManagementPathComposable();
			managementPathComposable.ClassName = "MicrosoftDNS_Server";
			managementPathComposable.AppendKey("Name", this.serverFqdn);
			ManagementObject @object = this.GetObject(managementPathComposable);
			if (@object == null)
			{
				throw new ManagementException(string.Format("Unable to acquire the DNS Server instance '{0}'.", managementPathComposable));
			}
			return @object;
		}

		private readonly ManagementScope scope;

		private readonly string serverFqdn;

		private class ManagementPathComposable : ManagementPath
		{
			public ManagementPathComposable()
			{
			}

			public ManagementPathComposable(string path) : base(path)
			{
			}

			public void AppendKey(string key, string value)
			{
				WmiDnsClient.ValidateArgumentNotNull(value, "value");
				value = "\"" + value.Replace("\"", "\\\"") + "\"";
				this.AppendKeyCore(key, value);
			}

			public void AppendKey(string key, uint value)
			{
				this.AppendKeyCore(key, value.ToString());
			}

			private void AppendKeyCore(string key, string value)
			{
				WmiDnsClient.ValidateArgumentNotNullOrEmpty(key, "key");
				if (!base.IsInstance && !base.IsClass)
				{
					throw new InvalidOperationException("The ClassName property must be set before calling this method.");
				}
				string text = base.RelativePath;
				if (base.IsSingleton)
				{
					text = text.Remove(text.Length - 2);
				}
				base.RelativePath = string.Concat(new string[]
				{
					text,
					(text == base.ClassName) ? "." : ",",
					key,
					"=",
					value
				});
			}
		}
	}
}
