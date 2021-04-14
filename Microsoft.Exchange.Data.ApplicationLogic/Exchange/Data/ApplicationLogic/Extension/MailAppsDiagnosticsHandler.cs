using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class MailAppsDiagnosticsHandler : ExchangeDiagnosableWrapper<MailAppsResult>
	{
		protected override string UsageText
		{
			get
			{
				return "This handler will return information related to mail apps." + Environment.NewLine;
			}
		}

		protected override string UsageSample
		{
			get
			{
				return string.Concat(new string[]
				{
					"To get raw org mail apps table, use the argument: \"cmd=get,org=<domain>\"",
					Environment.NewLine,
					"To get raw user mail apps table, use the argument: \"cmd=get,org=<domain>,usr=<user>\"",
					Environment.NewLine,
					"Example: Get-ExchangeDiagnosticInfo -Process MSExchangeServicesAppPool -Component MailApps -Argument \"cmd=get,usr=joe,org=msft.ccsctp.net\""
				});
			}
		}

		public static MailAppsDiagnosticsHandler GetInstance()
		{
			if (MailAppsDiagnosticsHandler.instance == null)
			{
				lock (MailAppsDiagnosticsHandler.lockObject)
				{
					if (MailAppsDiagnosticsHandler.instance == null)
					{
						MailAppsDiagnosticsHandler.instance = new MailAppsDiagnosticsHandler();
					}
				}
			}
			return MailAppsDiagnosticsHandler.instance;
		}

		private MailAppsDiagnosticsHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "MailApps";
			}
		}

		internal override MailAppsResult GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			MailAppsResult mailAppsResult = new MailAppsResult();
			string text = argument.Argument;
			if (!string.IsNullOrEmpty(text))
			{
				string value = null;
				string text2 = ",val=";
				int num = text.IndexOf(text2);
				if (num != -1)
				{
					value = text.Substring(num + text2.Length);
					text = argument.Argument.Substring(0, num);
				}
				MailAppsArgument mailAppsArgument = new MailAppsArgument(text);
				if (mailAppsArgument.HasArgument("org") && mailAppsArgument.HasArgument("cmd"))
				{
					bool flag = false;
					string argument2 = mailAppsArgument.GetArgument<string>("org");
					ExchangePrincipal exchangePrincipal;
					if (mailAppsArgument.HasArgument("usr"))
					{
						string argument3 = mailAppsArgument.GetArgument<string>("usr");
						ADSessionSettings adSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(argument2);
						exchangePrincipal = ExchangePrincipal.FromProxyAddress(adSettings, argument3 + "@" + argument2, RemotingOptions.AllowCrossSite);
					}
					else
					{
						ADUser orgMailbox = OrgExtensionTable.GetOrgMailbox(argument2);
						exchangePrincipal = ExchangePrincipal.FromADUser(orgMailbox, null);
						flag = true;
					}
					using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(exchangePrincipal, CultureInfo.CurrentCulture, "Client=WebServices"))
					{
						using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox), "ExtensionMasterTable", UserConfigurationTypes.XML, true, false))
						{
							using (Stream xmlStream = folderConfiguration.GetXmlStream())
							{
								if (string.Equals(mailAppsArgument.GetArgument<string>("cmd"), "set", StringComparison.OrdinalIgnoreCase) && num != -1)
								{
									xmlStream.SetLength(0L);
									bool flag2 = string.IsNullOrEmpty(value);
									if (!flag2)
									{
										using (StreamWriter streamWriter = new StreamWriter(xmlStream, Encoding.UTF8))
										{
											streamWriter.Write(value);
										}
									}
									folderConfiguration.Save();
									if (flag)
									{
										OrgEmptyMasterTableCache.Singleton.Update(exchangePrincipal.MailboxInfo.OrganizationId, flag2);
									}
									mailAppsResult.Message = "Raw value saved.";
								}
								else
								{
									using (StreamReader streamReader = new StreamReader(xmlStream, true))
									{
										mailAppsResult.RawMasterTable = streamReader.ReadToEnd();
										if (mailAppsArgument.HasArgument("len"))
										{
											int argument4 = mailAppsArgument.GetArgument<int>("len");
											if (argument4 > 0 && argument4 < mailAppsResult.RawMasterTable.Length)
											{
												mailAppsResult.RawMasterTable = mailAppsResult.RawMasterTable.Substring(0, argument4);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return mailAppsResult;
		}

		private static MailAppsDiagnosticsHandler instance;

		private static object lockObject = new object();
	}
}
