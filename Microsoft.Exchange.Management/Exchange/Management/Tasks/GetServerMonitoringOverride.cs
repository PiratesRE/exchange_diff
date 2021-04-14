using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ServerMonitoringOverride")]
	public sealed class GetServerMonitoringOverride : ServerMonitoringOverrideBase
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (base.RegistryKeyHive != null)
				{
					foreach (object obj in Enum.GetValues(typeof(MonitoringItemTypeEnum)))
					{
						MonitoringItemTypeEnum monitoringItemTypeEnum = (MonitoringItemTypeEnum)obj;
						string text = monitoringItemTypeEnum.ToString();
						using (RegistryKey registryKey = base.RegistryKeyHive.OpenSubKey(string.Format("{0}\\{1}", ServerMonitoringOverrideBase.OverridesBaseRegistryKey, text)))
						{
							if (registryKey != null)
							{
								string[] subKeyNames = registryKey.GetSubKeyNames();
								foreach (string text2 in subKeyNames)
								{
									using (RegistryKey registryKey2 = registryKey.OpenSubKey(text2))
									{
										string propertyValue = (string)registryKey2.GetValue("PropertyValue", string.Empty);
										string expirationTime = (string)registryKey2.GetValue("ExpirationTime", string.Empty);
										string applyVersion = (string)registryKey2.GetValue("ApplyVersion", string.Empty);
										string createdBy = (string)registryKey2.GetValue("CreatedBy", string.Empty);
										string createdTime = (string)registryKey2.GetValue("TimeUpdated", string.Empty);
										string[] array2 = MonitoringOverrideHelpers.SplitMonitoringItemIdentity(text2, '~');
										base.WriteObject(new MonitoringOverrideObject((array2.Length >= 1) ? array2[0] : string.Empty, (array2.Length >= 2) ? array2[1] : string.Empty, (array2.Length >= 4) ? array2[3] : string.Empty, text, base.GetPropertyName(text2), propertyValue, expirationTime, applyVersion, createdBy, createdTime));
									}
								}
							}
						}
					}
				}
			}
			catch (IOException ex)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(base.ServerName, ex.ToString()), ErrorCategory.ObjectNotFound, null);
			}
			catch (SecurityException ex2)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(base.ServerName, ex2.ToString()), ExchangeErrorCategory.Authorization, null);
			}
			catch (UnauthorizedAccessException ex3)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(base.ServerName, ex3.ToString()), ExchangeErrorCategory.Authorization, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}
	}
}
