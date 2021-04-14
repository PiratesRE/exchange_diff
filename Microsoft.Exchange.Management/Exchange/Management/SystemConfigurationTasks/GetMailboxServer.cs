using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MailboxServer", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxServer : GetSystemConfigurationObjectTask<MailboxServerIdParameter, Server>
	{
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
				return new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL);
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			MailboxServer mailboxServer = new MailboxServer((Server)dataObject);
			string fqdn = ((Server)dataObject).Fqdn;
			if (this.Status && ((Server)dataObject).IsProvisionedServer)
			{
				this.WriteWarning(Strings.StatusSpecifiedForProvisionedServer);
			}
			if (this.Status && !mailboxServer.IsReadOnly && !((Server)dataObject).IsProvisionedServer)
			{
				if (string.IsNullOrEmpty(fqdn))
				{
					this.WriteWarning(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Server).Name, mailboxServer.Identity.ToString(), ServerSchema.Fqdn.Name));
				}
				else
				{
					Exception ex = null;
					CultureInfo[] array;
					GetMailboxServer.GetConfigurationFromRegistry(fqdn, out array, out ex);
					if (ex != null)
					{
						this.WriteWarning(Strings.ErrorAccessingRegistryRaisesException(fqdn, ex.Message));
					}
					mailboxServer.Locale = array;
					mailboxServer.ResetChangeTracking();
				}
			}
			base.WriteResult(mailboxServer);
			TaskLogger.LogExit();
		}

		internal static void GetConfigurationFromRegistry(string computerName, out CultureInfo[] locale, out Exception caughtException)
		{
			if (string.IsNullOrEmpty(computerName))
			{
				throw new ArgumentNullException("computerName");
			}
			caughtException = null;
			List<CultureInfo> list = new List<CultureInfo>();
			try
			{
				using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language"))
					{
						if (registryKey2 != null)
						{
							string[] valueNames = registryKey2.GetValueNames();
							if (valueNames != null)
							{
								foreach (string text in valueNames)
								{
									int culture = (int)registryKey2.GetValue(text, 0);
									try
									{
										CultureInfo cultureInfo = new CultureInfo(culture);
										if (cultureInfo.ThreeLetterWindowsLanguageName.Equals(text, StringComparison.InvariantCultureIgnoreCase))
										{
											list.Add(cultureInfo);
										}
									}
									catch (ArgumentException)
									{
										TaskLogger.Trace("There is An inlvad data in the rigistry: SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language", new object[0]);
									}
								}
							}
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
			locale = list.ToArray();
		}

		internal static void SetConfigurationFromRegistry(string computerName, IEnumerable<CultureInfo> locale, out Exception caughtException)
		{
			if (string.IsNullOrEmpty(computerName))
			{
				throw new ArgumentNullException("computerName");
			}
			if (locale == null)
			{
				throw new ArgumentNullException("locale");
			}
			caughtException = null;
			try
			{
				using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language"))
					{
						if (registryKey2 != null)
						{
							registryKey.DeleteSubKeyTree("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language");
						}
					}
					IEnumerator<CultureInfo> enumerator = locale.GetEnumerator();
					if (enumerator.MoveNext())
					{
						using (RegistryKey registryKey3 = registryKey.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language"))
						{
							do
							{
								CultureInfo cultureInfo = enumerator.Current;
								registryKey3.SetValue(cultureInfo.ThreeLetterWindowsLanguageName, cultureInfo.LCID, RegistryValueKind.DWord);
							}
							while (enumerator.MoveNext());
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
		}

		internal const string LanguageKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language";
	}
}
