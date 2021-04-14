using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ExchangeServer", DefaultParameterSetName = "Identity")]
	public sealed class GetExchangeServer : GetSystemConfigurationObjectTask<ServerIdParameter, Server>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Domain")]
		public Fqdn Domain
		{
			get
			{
				return (Fqdn)base.Fields["Domain"];
			}
			set
			{
				base.Fields["Domain"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Status
		{
			get
			{
				return (SwitchParameter)(base.Fields["Status"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.Domain != null)
				{
					return new TextFilter(ServerSchema.NetworkAddress, "." + this.Domain, MatchOptions.Suffix, MatchFlags.IgnoreCase);
				}
				return null;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (!string.IsNullOrEmpty(this.Domain))
			{
				ADForest localForest = ADForest.GetLocalForest();
				if (localForest.FindDomainByFqdn(this.Domain) == null)
				{
					base.ThrowTerminatingError(new DomainNotFoundException(this.Domain.ToString()), ErrorCategory.InvalidArgument, this.Domain);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			if (this.Domain == null || (this.Domain != null && ((Server)dataObject).Domain.Equals(this.Domain, StringComparison.InvariantCultureIgnoreCase)))
			{
				ExchangeServer exchangeServer = new ExchangeServer((Server)dataObject);
				if (this.Status && exchangeServer.IsProvisionedServer)
				{
					this.WriteWarning(Strings.StatusSpecifiedForProvisionedServer);
				}
				if (this.Status && !exchangeServer.IsReadOnly && !((Server)dataObject).IsProvisionedServer)
				{
					if (string.IsNullOrEmpty(exchangeServer.Fqdn))
					{
						this.WriteWarning(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Server).Name, exchangeServer.Identity.ToString(), ServerSchema.Fqdn.Name));
					}
					else
					{
						Exception ex = null;
						string[] array;
						string[] array2;
						string staticConfigDomainController;
						string[] array3;
						bool? errorReportingEnabled;
						GetExchangeServer.GetConfigurationFromRegistry(exchangeServer.Fqdn, out array, out array2, out staticConfigDomainController, out array3, out errorReportingEnabled, out ex);
						if (ex != null)
						{
							this.WriteWarning(Strings.ErrorAccessingRegistryRaisesException(exchangeServer.Fqdn, ex.Message));
						}
						exchangeServer.StaticDomainControllers = array;
						exchangeServer.StaticGlobalCatalogs = array2;
						exchangeServer.StaticConfigDomainController = staticConfigDomainController;
						exchangeServer.StaticExcludedDomainControllers = array3;
						exchangeServer.ErrorReportingEnabled = errorReportingEnabled;
						if (exchangeServer.IsExchange2007OrLater)
						{
							try
							{
								exchangeServer.RefreshDsAccessData();
							}
							catch (ADTransientException ex2)
							{
								this.WriteWarning(Strings.ErrorADTopologyServiceNotAvailable(exchangeServer.Fqdn, ex2.Message));
							}
						}
						exchangeServer.ResetChangeTracking();
					}
				}
				base.WriteResult(exchangeServer);
			}
			TaskLogger.LogExit();
		}

		internal static void SetConfigurationToRegistry(string computerName, IEnumerable<string> domainControllerNames, IEnumerable<string> globalCatalogNames, string configDomainControllername, IEnumerable<string> excludedDomaincontrollerNames, bool? errorReporting, out Exception caughtException)
		{
			if (string.IsNullOrEmpty(computerName))
			{
				throw new ArgumentNullException("computerName");
			}
			if (domainControllerNames == null)
			{
				throw new ArgumentNullException("domainControllerNames");
			}
			if (globalCatalogNames == null)
			{
				throw new ArgumentNullException("globalCatalogNames");
			}
			if (configDomainControllername == null)
			{
				throw new ArgumentNullException("configDomainControllername");
			}
			if (excludedDomaincontrollerNames == null)
			{
				throw new ArgumentNullException("excludedDomaincontrollerNames");
			}
			caughtException = null;
			try
			{
				using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
				{
					RegistryKey registryKey2 = registryKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Profiles\\Default", true);
					if (registryKey2 == null)
					{
						registryKey2 = registryKey.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Profiles\\Default");
					}
					using (registryKey2)
					{
						string[] subKeyNames = registryKey2.GetSubKeyNames();
						if (subKeyNames != null)
						{
							foreach (string subkey in subKeyNames)
							{
								registryKey2.DeleteSubKeyTree(subkey);
							}
						}
						int num = 0;
						foreach (string value in domainControllerNames)
						{
							num++;
							string subkey2 = "UserDC" + num.ToString();
							using (RegistryKey registryKey4 = registryKey2.CreateSubKey(subkey2))
							{
								registryKey4.SetValue("HostName", value, RegistryValueKind.String);
								registryKey4.SetValue("IsGC", 0U, RegistryValueKind.DWord);
							}
						}
						num = 0;
						foreach (string value2 in globalCatalogNames)
						{
							num++;
							string subkey2 = "UserGC" + num.ToString();
							using (RegistryKey registryKey5 = registryKey2.CreateSubKey(subkey2))
							{
								registryKey5.SetValue("HostName", value2, RegistryValueKind.String);
								registryKey5.SetValue("IsGC", 1U, RegistryValueKind.DWord);
							}
						}
						List<string> list = new List<string>(excludedDomaincontrollerNames);
						if (0 < list.Count)
						{
							registryKey2.SetValue("ExcludedDCs", list.ToArray(), RegistryValueKind.MultiString);
						}
						else
						{
							registryKey2.DeleteValue("ExcludedDCs", false);
						}
					}
					RegistryKey registryKey6 = null;
					try
					{
						registryKey6 = registryKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Instance0", true);
						if (registryKey6 == null)
						{
							registryKey6 = registryKey.CreateSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Instance0");
						}
						using (registryKey6)
						{
							if (string.IsNullOrEmpty(configDomainControllername))
							{
								registryKey6.DeleteValue("ConfigDCHostName", false);
							}
							else
							{
								registryKey6.SetValue("ConfigDCHostName", configDomainControllername, RegistryValueKind.String);
							}
						}
					}
					finally
					{
						if (registryKey6 != null)
						{
							registryKey6.Close();
						}
					}
					RegistryKey registryKey8 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15", true);
					if (registryKey8 == null)
					{
						registryKey8 = registryKey.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15");
					}
					using (registryKey8)
					{
						registryKey8.SetValue("DisableErrorReporting", (errorReporting == null || errorReporting.Value) ? 0 : 1, RegistryValueKind.DWord);
					}
				}
			}
			catch (SecurityException ex)
			{
				caughtException = ex;
			}
			catch (IOException ex2)
			{
				caughtException = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				caughtException = ex3;
			}
		}

		internal static void GetConfigurationFromRegistry(string computerName, out string[] domainControllerNames, out string[] globalCatalogNames, out string configDomainControllername, out string[] excludedDomaincontrollerNames, out bool? errorReporting, out Exception caughtException)
		{
			if (string.IsNullOrEmpty(computerName))
			{
				throw new ArgumentNullException("computerName");
			}
			caughtException = null;
			configDomainControllername = string.Empty;
			excludedDomaincontrollerNames = new string[0];
			errorReporting = null;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			try
			{
				using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Profiles\\Default"))
					{
						if (registryKey2 != null)
						{
							string[] subKeyNames = registryKey2.GetSubKeyNames();
							foreach (string name in subKeyNames)
							{
								using (RegistryKey registryKey3 = registryKey2.OpenSubKey(name))
								{
									string text = registryKey3.GetValue("HostName", string.Empty) as string;
									int num = (int)registryKey3.GetValue("IsGC", 0);
									if (!string.IsNullOrEmpty(text))
									{
										if (num == 0)
										{
											list.Add(text);
										}
										else
										{
											list2.Add(text);
										}
									}
								}
							}
							excludedDomaincontrollerNames = (registryKey2.GetValue("ExcludedDCs", new string[0]) as string[]);
						}
					}
					using (RegistryKey registryKey4 = registryKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Instance0"))
					{
						if (registryKey4 != null)
						{
							configDomainControllername = (registryKey4.GetValue("ConfigDCHostName", string.Empty) as string);
						}
					}
					using (RegistryKey registryKey5 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15"))
					{
						if (registryKey5 != null)
						{
							object value = registryKey5.GetValue("DisableErrorReporting");
							if (value == null || registryKey5.GetValueKind("DisableErrorReporting") != RegistryValueKind.DWord)
							{
								errorReporting = new bool?(true);
							}
							else
							{
								errorReporting = new bool?((int)value == 0);
							}
						}
						else
						{
							errorReporting = new bool?(true);
						}
					}
				}
			}
			catch (SecurityException ex)
			{
				caughtException = ex;
			}
			catch (IOException ex2)
			{
				caughtException = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				caughtException = ex3;
			}
			domainControllerNames = list.ToArray();
			globalCatalogNames = list2.ToArray();
		}

		internal const string ErrorReportingEnabledKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		internal const string ProfilesDefaultKeyName = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Profiles\\Default";

		internal const string StaticConfigDCKeyName = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Instance0";
	}
}
