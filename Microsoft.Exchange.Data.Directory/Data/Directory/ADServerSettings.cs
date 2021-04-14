using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal abstract class ADServerSettings : ICloneable, IEquatable<ADServerSettings>
	{
		public ADServerSettings()
		{
			this.syncRoot = new object();
			this.preferredGlobalCatalog = new Dictionary<string, Fqdn>(StringComparer.OrdinalIgnoreCase);
			this.configurationDomainController = new Dictionary<string, Fqdn>(StringComparer.OrdinalIgnoreCase);
			this.serverPerDomain = new Dictionary<ADObjectId, Fqdn>(ADObjectIdEqualityComparer.Instance);
			this.cachedPreferredDCList = new MultiValuedProperty<Fqdn>();
			this.lastUsedDc = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.serversDown = new List<Fqdn>();
			this.sdListLock = new object();
			this.writeOriginatingChangeTimestamp = ADGlobalConfigSettings.WriteOriginatingChangeTimestamp;
			this.writeShadowProperties = ADGlobalConfigSettings.WriteShadowProperties;
			this.disableGls = false;
		}

		internal virtual ADObjectId RecipientViewRoot { get; set; }

		internal virtual bool ViewEntireForest
		{
			get
			{
				return this.RecipientViewRoot == null;
			}
			set
			{
				if (this.RecipientViewRoot == null != value)
				{
					ADObjectId recipientViewRoot = null;
					if (!value)
					{
						recipientViewRoot = TopologyProvider.GetInstance().GetDomainNamingContextForLocalForest();
					}
					this.RecipientViewRoot = recipientViewRoot;
				}
			}
		}

		internal bool WriteOriginatingChangeTimestamp
		{
			get
			{
				return this.writeShadowProperties;
			}
			set
			{
				this.writeOriginatingChangeTimestamp = value;
			}
		}

		internal bool WriteShadowProperties
		{
			get
			{
				return this.writeShadowProperties;
			}
			set
			{
				this.writeShadowProperties = value;
			}
		}

		internal bool DisableGls
		{
			get
			{
				return this.disableGls;
			}
			set
			{
				this.disableGls = value;
			}
		}

		internal bool ForceADInTemplateScope { get; set; }

		internal virtual bool IsUpdatableByADSession
		{
			get
			{
				return false;
			}
		}

		protected abstract bool EnforceIsUpdatableByADSession { get; }

		internal string Token { get; set; }

		internal static string GetPartitionFqdnFromADServerFqdn(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			return fqdn.Substring(fqdn.IndexOf(".") + 1);
		}

		internal static ADServerInfo GetServerInfoFromFqdn(string fqdn, ConnectionType connectionType)
		{
			PooledLdapConnection pooledLdapConnection = null;
			string empty = string.Empty;
			ADServerInfo adserverInfo;
			try
			{
				string partitionFqdn = Globals.IsMicrosoftHostedOnly ? ADServerSettings.GetPartitionFqdnFromADServerFqdn(fqdn) : TopologyProvider.LocalForestFqdn;
				pooledLdapConnection = ConnectionPoolManager.GetConnection(connectionType, partitionFqdn, null, fqdn, (connectionType == ConnectionType.GlobalCatalog) ? TopologyProvider.GetInstance().DefaultGCPort : TopologyProvider.GetInstance().DefaultDCPort);
				string writableNC = pooledLdapConnection.ADServerInfo.WritableNC;
				if (!pooledLdapConnection.SessionOptions.HostName.Equals(fqdn, StringComparison.OrdinalIgnoreCase))
				{
					throw new ADOperationException(DirectoryStrings.ErrorInvalidServerFqdn(fqdn, pooledLdapConnection.SessionOptions.HostName));
				}
				adserverInfo = pooledLdapConnection.ADServerInfo;
			}
			finally
			{
				if (pooledLdapConnection != null)
				{
					pooledLdapConnection.ReturnToPool();
				}
			}
			return adserverInfo;
		}

		internal static void ThrowIfServerNameDoesntMatchPartitionId(string serverName, string partitionFqdn)
		{
			if (!ADServerSettings.IsServerNamePartitionSameAsPartitionId(serverName, partitionFqdn))
			{
				throw new DomainControllerFromWrongDomainException(DirectoryStrings.ExceptionSeverNotInPartition(serverName, partitionFqdn));
			}
		}

		[Conditional("DEBUG")]
		internal static void AssertIfServerNameDoesntMatchPartitionId(string serverName, string partitionFqdn, string additionalInfo)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				return;
			}
			string.IsNullOrEmpty(partitionFqdn);
		}

		internal static bool IsServerNamePartitionSameAsPartitionId(string serverName, string partitionFqdn)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentNullException("serverName");
			}
			if (string.IsNullOrEmpty(partitionFqdn))
			{
				throw new ArgumentNullException("partitionFqdn");
			}
			IPAddress ipaddress;
			return !Globals.IsMicrosoftHostedOnly || !serverName.Contains(".") || IPAddress.TryParse(serverName, out ipaddress) || partitionFqdn.Trim().EndsWith(ADServerSettings.GetPartitionFqdnFromADServerFqdn(serverName).Trim(), StringComparison.OrdinalIgnoreCase);
		}

		internal virtual bool IsFailoverRequired()
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				return false;
			}
			foreach (Fqdn fqdn in this.serversDown)
			{
				lock (this.syncRoot)
				{
					IEnumerable<Fqdn>[] array = new IEnumerable<Fqdn>[]
					{
						this.configurationDomainController.Values,
						this.preferredGlobalCatalog.Values,
						this.PreferredDomainControllers
					};
					foreach (IEnumerable<Fqdn> enumerable in array)
					{
						foreach (Fqdn fqdn2 in enumerable)
						{
							string text = fqdn2;
							if (text.Equals(fqdn))
							{
								return true;
							}
						}
					}
				}
				if (ADRunspaceServerSettingsProvider.GetInstance().IsServerKnown(fqdn))
				{
					return true;
				}
			}
			return false;
		}

		internal virtual void MarkDcDown(Fqdn fqdn)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("MarkDcDown is not supported by " + base.GetType().Name);
			}
			if (this.serversDown.Contains(fqdn))
			{
				return;
			}
			lock (this.sdListLock)
			{
				if (!this.serversDown.Contains(fqdn))
				{
					this.serversDown = new List<Fqdn>(this.serversDown)
					{
						fqdn
					};
				}
			}
		}

		internal virtual void MarkDcUp(Fqdn fqdn)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("MarkDcUp is not supported by " + base.GetType().Name);
			}
			if (!this.serversDown.Contains(fqdn))
			{
				return;
			}
			lock (this.sdListLock)
			{
				if (this.serversDown.Contains(fqdn))
				{
					List<Fqdn> list = new List<Fqdn>(this.serversDown);
					list.Remove(fqdn);
					this.serversDown = list;
				}
			}
		}

		internal virtual bool TryFailover(out ADServerSettings newServerSettings, out string failToFailOverReason, bool forestWideAffinityRequested = false)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("TryFailover is not supported on " + base.GetType().Name);
			}
			bool result = true;
			failToFailOverReason = null;
			ADServerSettings adserverSettings = (ADServerSettings)this.Clone();
			ADRunspaceServerSettingsProvider instance = ADRunspaceServerSettingsProvider.GetInstance();
			string token = adserverSettings.Token;
			foreach (Fqdn fqdn in adserverSettings.serversDown)
			{
				adserverSettings.RemovePreferredDC(fqdn);
				try
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, Fqdn> keyValuePair in adserverSettings.preferredGlobalCatalog)
					{
						if (string.Equals(fqdn, keyValuePair.Value, StringComparison.OrdinalIgnoreCase))
						{
							string key = keyValuePair.Key;
							list.Add(key);
						}
					}
					foreach (string partitionFqdn in list)
					{
						ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug<string>((long)instance.GetHashCode(), "GetGcOnlyRunspaceServerSettings for token {0}", token ?? "<null>");
						bool flag;
						ADServerInfo gcFromToken = instance.GetGcFromToken(partitionFqdn, token, out flag, forestWideAffinityRequested);
						adserverSettings.SetPreferredGlobalCatalog(partitionFqdn, gcFromToken);
						if (adserverSettings.Token != null)
						{
							adserverSettings.SetConfigurationDomainController(partitionFqdn, gcFromToken);
						}
						if (adserverSettings.Token != null || flag)
						{
							adserverSettings.AddPreferredDC(gcFromToken);
						}
					}
				}
				catch (ADTransientException ex)
				{
					failToFailOverReason = ex.ToString();
					result = false;
				}
				catch (ADExternalException ex2)
				{
					failToFailOverReason = ex2.ToString();
					result = false;
				}
				if (token == null)
				{
					List<string> list2 = new List<string>();
					foreach (KeyValuePair<string, Fqdn> keyValuePair2 in adserverSettings.configurationDomainController)
					{
						if (string.Equals(fqdn, keyValuePair2.Value, StringComparison.OrdinalIgnoreCase))
						{
							string key2 = keyValuePair2.Key;
							list2.Add(key2);
						}
					}
					foreach (string text in list2)
					{
						ADServerInfo configDc = instance.GetConfigDc(text);
						if (configDc != null)
						{
							adserverSettings.SetConfigurationDomainController(text, configDc);
						}
					}
				}
			}
			adserverSettings.serversDown.Clear();
			newServerSettings = adserverSettings;
			return result;
		}

		internal IDictionary<string, Fqdn> PreferredGlobalCatalogs
		{
			get
			{
				IDictionary<string, Fqdn> result;
				lock (this.syncRoot)
				{
					result = new Dictionary<string, Fqdn>(this.preferredGlobalCatalog, StringComparer.OrdinalIgnoreCase);
				}
				return result;
			}
		}

		internal virtual Fqdn PreferredGlobalCatalog(string partitionFqdn)
		{
			Fqdn result;
			lock (this.syncRoot)
			{
				Fqdn fqdn;
				this.preferredGlobalCatalog.TryGetValue(partitionFqdn, out fqdn);
				result = fqdn;
			}
			return result;
		}

		internal virtual void SetPreferredGlobalCatalog(string partitionFqdn, ADServerInfo serverInfo)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("SetPreferredGlobalCatalog passing ADServerInfo is not supported on " + base.GetType().Name);
			}
			lock (this.syncRoot)
			{
				this.preferredGlobalCatalog[partitionFqdn] = new Fqdn(serverInfo.Fqdn);
			}
		}

		internal virtual void SetPreferredGlobalCatalog(string partitionFqdn, Fqdn fqdn)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("SetPreferredGlobalCatalog passing Fqdn is not supported on " + base.GetType().Name);
			}
			ADServerSettings.GetServerInfoFromFqdn(fqdn, ConnectionType.GlobalCatalog);
			lock (this.syncRoot)
			{
				this.preferredGlobalCatalog[partitionFqdn] = fqdn;
			}
		}

		internal IDictionary<string, Fqdn> ConfigurationDomainControllers
		{
			get
			{
				IDictionary<string, Fqdn> result;
				lock (this.syncRoot)
				{
					result = new Dictionary<string, Fqdn>(this.configurationDomainController, StringComparer.OrdinalIgnoreCase);
				}
				return result;
			}
		}

		internal virtual Fqdn ConfigurationDomainController(string partitionFqdn)
		{
			Fqdn result;
			lock (this.syncRoot)
			{
				Fqdn fqdn;
				this.configurationDomainController.TryGetValue(partitionFqdn, out fqdn);
				result = fqdn;
			}
			return result;
		}

		internal virtual void SetConfigurationDomainController(string partitionFqdn, ADServerInfo serverInfo)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("SetConfigurationDomainController passing ADServerInfo is not supported on " + base.GetType().Name);
			}
			lock (this.syncRoot)
			{
				this.configurationDomainController[partitionFqdn] = new Fqdn(serverInfo.Fqdn);
			}
		}

		internal virtual void SetConfigurationDomainController(string partitionFqdn, Fqdn fqdn)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("SetConfigurationDomainController passing Fqdn is not supported on " + base.GetType().Name);
			}
			ADServerSettings.GetServerInfoFromFqdn(fqdn, ConnectionType.DomainController);
			lock (this.syncRoot)
			{
				this.configurationDomainController[partitionFqdn] = fqdn;
			}
		}

		internal virtual MultiValuedProperty<Fqdn> PreferredDomainControllers
		{
			get
			{
				return this.cachedPreferredDCList;
			}
		}

		internal virtual string GetPreferredDC(ADObjectId domain)
		{
			Fqdn fqdn;
			if (this.serverPerDomain.TryGetValue(domain, out fqdn))
			{
				return fqdn;
			}
			return null;
		}

		internal virtual void AddPreferredDC(ADServerInfo serverInfo)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("AddPreferredDC passing ADServerInfo is not supported on " + base.GetType().Name);
			}
			this.InternalAddPreferredDC(serverInfo);
		}

		internal virtual void AddPreferredDC(Fqdn fqdn)
		{
			if (this.EnforceIsUpdatableByADSession && !this.IsUpdatableByADSession)
			{
				throw new NotSupportedException("AddPreferredDC passing Fqdn is not supported on " + base.GetType().Name);
			}
			ADServerInfo serverInfoFromFqdn = ADServerSettings.GetServerInfoFromFqdn(fqdn, ConnectionType.DomainController);
			this.InternalAddPreferredDC(serverInfoFromFqdn);
		}

		internal string LastUsedDc(string partitionFqdn)
		{
			string result;
			lock (this.syncRoot)
			{
				string text;
				this.lastUsedDc.TryGetValue(partitionFqdn, out text);
				result = text;
			}
			return result;
		}

		internal void SetLastUsedDc(string partitionFqdn, string serverName)
		{
			lock (this.syncRoot)
			{
				this.lastUsedDc[partitionFqdn] = serverName;
			}
		}

		protected virtual void CopyTo(object targetObj)
		{
			ArgumentValidator.ThrowIfNull("targetObj", targetObj);
			ArgumentValidator.ThrowIfTypeInvalid<ADServerSettings>("targetObj", targetObj);
			ADServerSettings adserverSettings = (ADServerSettings)targetObj;
			adserverSettings.Token = this.Token;
			lock (this.syncRoot)
			{
				adserverSettings.preferredGlobalCatalog = new Dictionary<string, Fqdn>(this.preferredGlobalCatalog, StringComparer.OrdinalIgnoreCase);
				adserverSettings.configurationDomainController = new Dictionary<string, Fqdn>(this.configurationDomainController, StringComparer.OrdinalIgnoreCase);
			}
			adserverSettings.RecipientViewRoot = this.RecipientViewRoot;
			adserverSettings.serverPerDomain = new Dictionary<ADObjectId, Fqdn>(this.serverPerDomain, ADObjectIdEqualityComparer.Instance);
			adserverSettings.cachedPreferredDCList = new MultiValuedProperty<Fqdn>(adserverSettings.serverPerDomain.Values);
			adserverSettings.serversDown = new List<Fqdn>(this.serversDown);
			adserverSettings.writeOriginatingChangeTimestamp = this.writeOriginatingChangeTimestamp;
			adserverSettings.writeShadowProperties = this.writeShadowProperties;
			adserverSettings.disableGls = this.disableGls;
			adserverSettings.ForceADInTemplateScope = this.ForceADInTemplateScope;
		}

		public abstract object Clone();

		public bool Equals(ADServerSettings other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.writeOriginatingChangeTimestamp != other.writeOriginatingChangeTimestamp || this.writeShadowProperties != other.writeShadowProperties || this.disableGls != other.disableGls || this.ForceADInTemplateScope != other.ForceADInTemplateScope || !ADObjectId.Equals(this.RecipientViewRoot, other.RecipientViewRoot) || !string.Equals(this.Token, other.Token, StringComparison.OrdinalIgnoreCase) || !this.serverPerDomain.Equals(other.serverPerDomain))
			{
				return false;
			}
			bool result;
			lock (this.syncRoot)
			{
				lock (other.syncRoot)
				{
					result = (this.preferredGlobalCatalog.Equals(other.preferredGlobalCatalog) && this.configurationDomainController.Equals(other.configurationDomainController));
				}
			}
			return result;
		}

		protected void InternalAddPreferredDC(ADServerInfo serverInfo)
		{
			if (serverInfo == null)
			{
				throw new ArgumentNullException("serverInfo");
			}
			if (string.IsNullOrEmpty(serverInfo.WritableNC))
			{
				throw new ArgumentException("serverInfo.WritableNC should not be null or empty");
			}
			ADObjectId key = new ADObjectId(serverInfo.WritableNC);
			if (this.serverPerDomain.ContainsKey(key))
			{
				return;
			}
			Fqdn value = new Fqdn(serverInfo.Fqdn);
			lock (this.syncRoot)
			{
				if (!this.serverPerDomain.ContainsKey(key))
				{
					this.serverPerDomain = new Dictionary<ADObjectId, Fqdn>(this.serverPerDomain)
					{
						{
							key,
							value
						}
					};
					this.cachedPreferredDCList = new MultiValuedProperty<Fqdn>(this.serverPerDomain.Values);
				}
			}
		}

		private void RemovePreferredDC(string fqdn)
		{
			Fqdn value = new Fqdn(fqdn);
			if (!this.serverPerDomain.ContainsValue(value))
			{
				return;
			}
			lock (this.syncRoot)
			{
				ADObjectId adobjectId = null;
				foreach (KeyValuePair<ADObjectId, Fqdn> keyValuePair in this.serverPerDomain)
				{
					if (string.Equals(keyValuePair.Value, fqdn, StringComparison.OrdinalIgnoreCase))
					{
						adobjectId = keyValuePair.Key;
						break;
					}
				}
				if (adobjectId != null)
				{
					Dictionary<ADObjectId, Fqdn> dictionary = new Dictionary<ADObjectId, Fqdn>(this.serverPerDomain);
					dictionary.Remove(adobjectId);
					this.serverPerDomain = dictionary;
					this.cachedPreferredDCList = new MultiValuedProperty<Fqdn>(this.serverPerDomain.Values);
				}
			}
		}

		private object syncRoot;

		private Dictionary<string, Fqdn> preferredGlobalCatalog;

		private Dictionary<string, Fqdn> configurationDomainController;

		private Dictionary<ADObjectId, Fqdn> serverPerDomain;

		private MultiValuedProperty<Fqdn> cachedPreferredDCList;

		private Dictionary<string, string> lastUsedDc;

		private List<Fqdn> serversDown;

		private object sdListLock;

		private bool writeOriginatingChangeTimestamp;

		private bool writeShadowProperties;

		private bool disableGls;
	}
}
