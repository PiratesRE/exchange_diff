using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ServerMonitoringOverrideBase : Task
	{
		internal string ServerName
		{
			get
			{
				return this.Server.ToString();
			}
		}

		internal RegistryKey RegistryKeyHive
		{
			get
			{
				return this.registryKeyHive;
			}
			set
			{
				this.registryKeyHive = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.registryKeyHive == null)
				{
					this.registryKeyHive = this.OpenHive(this.Server.Fqdn);
				}
			}
			catch (IOException ex)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(this.ServerName, ex.ToString()), ErrorCategory.ObjectNotFound, null);
			}
			catch (SecurityException ex2)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(this.ServerName, ex2.ToString()), ExchangeErrorCategory.Authorization, null);
			}
			catch (UnauthorizedAccessException ex3)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(this.ServerName, ex3.ToString()), ExchangeErrorCategory.Authorization, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		internal RegistryKey OpenHive(string serverName)
		{
			if (string.IsNullOrWhiteSpace(serverName))
			{
				throw new ArgumentNullException("serverName");
			}
			return RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, serverName);
		}

		internal bool ValidateMonitoringItemExist(string healthsetName, MonitoringItemTypeEnum itemType)
		{
			List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> monitoringItemsForHealthSet = this.GetMonitoringItemsForHealthSet(healthsetName);
			if (monitoringItemsForHealthSet != null)
			{
				foreach (RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity rpcMonitorItemIdentity in monitoringItemsForHealthSet)
				{
					if (string.Compare(itemType.ToString(), rpcMonitorItemIdentity.ItemType, true) == 0 && string.Compare(rpcMonitorItemIdentity.Name, this.helper.MonitoringItemName, true) == 0 && (string.IsNullOrWhiteSpace(this.helper.TargetResource) || string.Compare(rpcMonitorItemIdentity.TargetResource, this.helper.TargetResource, true) == 0))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal string GetConvertedString(string stringToReplace, char oldChar, char newChar)
		{
			return stringToReplace.Replace(oldChar, newChar);
		}

		internal string GetPropertyName(string registryKeyName)
		{
			string[] array = MonitoringOverrideHelpers.SplitMonitoringItemIdentity(registryKeyName, '~');
			if (array != null && array.Length >= 3)
			{
				return array[2];
			}
			return string.Empty;
		}

		internal string GenerateMonitoringItemIdentityString(string fullString)
		{
			string[] array = MonitoringOverrideHelpers.SplitMonitoringItemIdentity(fullString, '~');
			if (array != null)
			{
				if (array.Length == 4)
				{
					return string.Format("{0}{1}{2}{1}{3}", new object[]
					{
						array[0],
						'\\',
						array[1],
						array[3]
					});
				}
				if (array.Length == 3)
				{
					return string.Format("{0}{1}{2}", array[0], '\\', array[1]);
				}
			}
			return fullString;
		}

		internal string GenerateOverrideString(string monitoringItemIdentity, string propertyName)
		{
			if (string.IsNullOrWhiteSpace(this.helper.TargetResource))
			{
				return string.Format("{0}{1}{2}{1}{3}", new object[]
				{
					this.helper.HealthSet,
					'~',
					this.helper.MonitoringItemName,
					propertyName
				});
			}
			return string.Format("{0}{1}{2}{1}{3}{1}{4}", new object[]
			{
				this.helper.HealthSet,
				'~',
				this.helper.MonitoringItemName,
				propertyName,
				this.helper.TargetResource
			});
		}

		internal void ValidateGlobalLocalConflict(string identity, string[] registryKeys, string propertyName, MonitoringItemTypeEnum monitoringItemType)
		{
			if (registryKeys != null)
			{
				foreach (string text in registryKeys)
				{
					if (string.Compare(text, identity, true) == 0)
					{
						base.WriteError(new PropertyAlreadyHasAnOverrideException(propertyName, this.helper.MonitoringItemIdentity, monitoringItemType.ToString()), ErrorCategory.ResourceExists, null);
					}
					else
					{
						string[] array = MonitoringOverrideHelpers.SplitMonitoringItemIdentity(text, '~');
						if (array.Length > 1 && string.Compare(array[0], this.helper.HealthSet, true) == 0 && string.Compare(array[1], this.helper.MonitoringItemName, true) == 0)
						{
							if (array.Length == 4 && !string.IsNullOrWhiteSpace(this.helper.TargetResource))
							{
								return;
							}
							if (array.Length == 4 && string.IsNullOrWhiteSpace(this.helper.TargetResource))
							{
								base.WriteError(new MonitoringItemAlreadyHasLocalOverrideException(this.helper.MonitoringItemIdentity, monitoringItemType.ToString(), this.GenerateMonitoringItemIdentityString(text)), ErrorCategory.ResourceExists, null);
							}
							else if (array.Length == 3 && !string.IsNullOrWhiteSpace(this.helper.TargetResource))
							{
								base.WriteError(new MonitoringItemAlreadyHasGlobalOverrideException(this.helper.MonitoringItemIdentity, monitoringItemType.ToString(), this.GenerateMonitoringItemIdentityString(text)), ErrorCategory.ResourceExists, null);
							}
						}
					}
				}
			}
		}

		private List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> GetMonitoringItemsForHealthSet(string healthSetName)
		{
			List<RpcGetMonitoringItemIdentity.RpcMonitorItemIdentity> result = null;
			try
			{
				result = RpcGetMonitoringItemIdentity.Invoke(this.Server.Fqdn, healthSetName, 900000);
			}
			catch (ActiveMonitoringServerException)
			{
			}
			catch (ActiveMonitoringServerTransientException)
			{
			}
			return result;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is InvalidVersionException || exception is InvalidIdentityException || exception is InvalidDurationException || exception is ExAssertException || DataAccessHelper.IsDataAccessKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			if (this.registryKeyHive != null)
			{
				this.registryKeyHive.Close();
				this.registryKeyHive = null;
			}
		}

		protected override void InternalStopProcessing()
		{
			base.InternalStopProcessing();
			if (this.registryKeyHive != null)
			{
				this.registryKeyHive.Close();
				this.registryKeyHive = null;
			}
		}

		private RegistryKey registryKeyHive;

		protected MonitoringOverrideHelpers helper = new MonitoringOverrideHelpers();

		internal static readonly string OverridesBaseRegistryKey = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\ActiveMonitoring\\Overrides", "v15");
	}
}
