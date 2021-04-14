using System;
using System.Linq;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmLastKnownGoodConfig
	{
		internal AmRole Role { get; private set; }

		internal AmServerName AuthoritativeServer { get; private set; }

		internal AmServerName[] Members { get; private set; }

		private AmLastKnownGoodConfig()
		{
			this.Role = AmRole.Unknown;
		}

		internal static AmLastKnownGoodConfig ConstructLastKnownGoodConfigFromPersistentState()
		{
			AmLastKnownGoodConfig amLastKnownGoodConfig = null;
			string fromRegistry = AmLastKnownGoodConfig.GetFromRegistry();
			amLastKnownGoodConfig = new AmLastKnownGoodConfig();
			if (!string.IsNullOrEmpty(fromRegistry))
			{
				try
				{
					AmLastKnownConfigSerializable amLastKnownConfigSerializable = (AmLastKnownConfigSerializable)SerializationUtil.XmlToObject(fromRegistry, typeof(AmLastKnownConfigSerializable));
					amLastKnownGoodConfig.Role = (AmRole)amLastKnownConfigSerializable.Role;
					amLastKnownGoodConfig.AuthoritativeServer = new AmServerName(amLastKnownConfigSerializable.AuthoritativeServer);
					amLastKnownGoodConfig.Members = (from serverNameFqdn in amLastKnownConfigSerializable.Members
					select new AmServerName(serverNameFqdn)).ToArray<AmServerName>();
					amLastKnownGoodConfig.m_prevObjectXml = fromRegistry;
					string text = string.Empty;
					if (amLastKnownGoodConfig.Members != null)
					{
						string[] value = (from server in amLastKnownGoodConfig.Members
						select server.NetbiosName).ToArray<string>();
						text = string.Join(",", value);
					}
					ReplayCrimsonEvents.LastKnownGoodConfigInitialized.Log<AmRole, AmServerName, string>(amLastKnownGoodConfig.Role, amLastKnownGoodConfig.AuthoritativeServer, text);
				}
				catch (Exception ex)
				{
					ReplayCrimsonEvents.LastKnownGoodConfigSerializationError.Log<string, string>("Deserialize", ex.ToString());
				}
			}
			return amLastKnownGoodConfig;
		}

		internal void Update(AmConfig cfg)
		{
			lock (this.m_locker)
			{
				this.Role = cfg.Role;
				if (cfg.IsPamOrSam)
				{
					this.Members = cfg.DagConfig.MemberServers;
					this.AuthoritativeServer = cfg.DagConfig.CurrentPAM;
				}
				else
				{
					this.Members = new AmServerName[]
					{
						AmServerName.LocalComputerName
					};
					this.AuthoritativeServer = AmServerName.LocalComputerName;
				}
				this.Persist();
			}
		}

		private void Persist()
		{
			AmLastKnownConfigSerializable amLastKnownConfigSerializable = new AmLastKnownConfigSerializable();
			amLastKnownConfigSerializable.Role = (int)this.Role;
			amLastKnownConfigSerializable.AuthoritativeServer = this.AuthoritativeServer.Fqdn;
			amLastKnownConfigSerializable.Members = (from server in this.Members
			select server.Fqdn).ToArray<string>();
			string text = string.Empty;
			try
			{
				text = SerializationUtil.ObjectToXml(amLastKnownConfigSerializable);
				if (!SharedHelper.StringIEquals(text, this.m_prevObjectXml))
				{
					this.SaveToRegistry(text);
					this.m_prevObjectXml = text;
				}
			}
			catch (Exception ex)
			{
				ReplayCrimsonEvents.LastKnownGoodConfigSerializationError.Log<string, string>("Serialize", ex.ToString());
			}
		}

		private void SaveToRegistry(string objectXml)
		{
			Exception ex;
			IRegistryKey registryKey = Dependencies.RegistryKeyProvider.TryOpenKey(SharedHelper.AmRegKeyRoot, ref ex);
			if (ex != null)
			{
				throw ex;
			}
			if (registryKey != null)
			{
				using (registryKey)
				{
					registryKey.SetValue("LastKnownGoodConfig", objectXml, RegistryValueKind.String);
				}
			}
		}

		private static string GetFromRegistry()
		{
			Exception ex;
			IRegistryKey registryKey = Dependencies.RegistryKeyProvider.TryOpenKey(SharedHelper.AmRegKeyRoot, ref ex);
			if (ex != null)
			{
				throw ex;
			}
			string result = string.Empty;
			if (registryKey != null)
			{
				using (registryKey)
				{
					result = (string)registryKey.GetValue("LastKnownGoodConfig", string.Empty);
				}
			}
			return result;
		}

		private object m_locker = new object();

		private string m_prevObjectXml;
	}
}
