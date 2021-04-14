using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Monitoring;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Monitoring
{
	internal static class TestConnectivityCredentialsManager
	{
		public static SmtpAddress? GetEnterpriseAutomatedTaskUser(ADSite site, string domain)
		{
			if (string.IsNullOrEmpty(domain))
			{
				throw new ArgumentException("domain");
			}
			bool flag = TestConnectivityCredentialsManager.IsExchangeMultiTenant();
			if (flag)
			{
				return null;
			}
			if (site != null)
			{
				Guid guid = site.Guid;
				string local = "extest_" + site.Guid.ToString().Replace("-", string.Empty).Substring(0, 13);
				return new SmtpAddress?(new SmtpAddress(local, domain));
			}
			return null;
		}

		public static SmtpAddress? GetMultiTenantAutomatedTaskUser(Task taskConsumer, ITopologyConfigurationSession configurationSession, ADSite site)
		{
			return TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(taskConsumer, configurationSession, site, DatacenterUserType.LEGACY);
		}

		public static SmtpAddress? GetMultiTenantAutomatedTaskUser(Task taskConsumer, ITopologyConfigurationSession configurationSession, ADSite site, DatacenterUserType userType)
		{
			return TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(taskConsumer, userType);
		}

		public static SmtpAddress? GetMultiTenantAutomatedTaskUser(Task taskConsumer, DatacenterUserType userType)
		{
			if (!TestConnectivityCredentialsManager.IsExchangeMultiTenant())
			{
				TestConnectivityCredentialsManager.TraceError("Request for a MultiTenant test account was made in a non-multitenant environment");
				throw new InvalidTestAccountRequestException();
			}
			string cachedLocalTestMailbox = TestConnectivityCredentialsManager.GetCachedLocalTestMailbox(taskConsumer, userType);
			taskConsumer.WriteVerbose(new LocalizedString(string.Format("Test username returned is [{0}]", cachedLocalTestMailbox)));
			if (cachedLocalTestMailbox != null)
			{
				SmtpAddress value = SmtpAddress.Parse(cachedLocalTestMailbox);
				SmtpAddress smtpAddress;
				if (TestConnectivityCredentialsManager.IsUnderFaultInjection(out smtpAddress))
				{
					value = smtpAddress;
				}
				return new SmtpAddress?(value);
			}
			throw new CannotFindTestUserException();
		}

		public static string GetAutomatedTaskDataCenterDomain(ADSite site)
		{
			if (site == null || site.Name == null)
			{
				return null;
			}
			return string.Format("{0}.{1}", site.Name, "exchangemon.net");
		}

		public static string NewTestUserScriptName
		{
			get
			{
				return TestConnectivityCredentialsManager.IsExchangeMultiTenant() ? (Datacenter.IsPartnerHostedOnly(false) ? Strings.CasHealthTestNewUserHostingScriptName : Strings.CasHealthTestNewUserDataCenterScriptName) : Strings.CasHealthTestNewUserScriptName;
			}
		}

		public static string GetDomainUserNameFromCredentials(NetworkCredential credentials)
		{
			if (credentials == null)
			{
				return null;
			}
			if (credentials.UserName.Contains("@") || string.IsNullOrEmpty(credentials.Domain))
			{
				return credentials.UserName;
			}
			return string.Format("{0}\\{1}", credentials.Domain, credentials.UserName);
		}

		public static LocalizedException ResetAutomatedCredentialsAndVerify(ExchangePrincipal exchangePrincipal, NetworkCredential credentials, bool forceResetCredentials, out bool credentialsWereReset)
		{
			credentialsWereReset = false;
			if (TestConnectivityCredentialsManager.IsExchangeMultiTenant() && TestConnectivityCredentialsManager.IsStandardDatacenterTestUserAccount(exchangePrincipal))
			{
				return new CannotChangeStandardUserCredentialsException(Strings.CannotChangeStandardUserCredentials(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()));
			}
			TestConnectivityCredentialsManager.TraceInfo("Refreshing automated probing task information...");
			try
			{
				string text = (exchangePrincipal.MailboxInfo.Location.ServerFqdn == null) ? "" : exchangePrincipal.MailboxInfo.Location.ServerFqdn;
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, Thread.CurrentThread.CurrentCulture, "Client=Monitoring;Action=Test-CasConnectivity"))
				{
					using (Folder folder = TestConnectivityCredentialsManager.BindToTestCasConnectivityInfoFolder(mailboxSession))
					{
						using (MessageItem automatedTestCasConnectivityItem = TestConnectivityCredentialsManager.GetAutomatedTestCasConnectivityItem(folder, true))
						{
							ExDateTime now;
							TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(automatedTestCasConnectivityItem, exchangePrincipal, credentials, out now);
							string userPrincipalNameFromCredentials = TestConnectivityCredentialsManager.GetUserPrincipalNameFromCredentials(credentials);
							TestConnectivityCredentialsManager.TraceInfo("Calling LogonUser for user " + userPrincipalNameFromCredentials);
							Microsoft.Exchange.Diagnostics.Components.Monitoring.ExTraceGlobals.MonitoringTasksTracer.TraceDebug<string>(0L, "ResetAutomatedCredentialsAndVerify: calling LogonUser for User: {0}", userPrincipalNameFromCredentials);
							SafeUserTokenHandle safeUserTokenHandle;
							bool flag = NativeMethods.LogonUser(userPrincipalNameFromCredentials, null, credentials.Password, 8, 0, out safeUserTokenHandle);
							if (safeUserTokenHandle != null)
							{
								safeUserTokenHandle.Dispose();
							}
							bool flag2 = now <= ExDateTime.Now && now.AddDays(7.0) <= ExDateTime.Now;
							if (!flag || string.IsNullOrEmpty(credentials.Password) || flag2 || DirectoryHelper.GetPasswordExpirationDate(exchangePrincipal.ObjectId, mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid)) < ExDateTime.Now || forceResetCredentials)
							{
								string text2 = TestConnectivityCredentialsManager.PasswordGenerator.CreateNew();
								using (SecureString secureString = text2.ConvertToSecureString())
								{
									try
									{
										DirectoryHelper.ResetPassword(exchangePrincipal.ObjectId, secureString, mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent));
										now = ExDateTime.Now;
										credentialsWereReset = true;
									}
									catch (TargetInvocationException ex)
									{
										if (ex.InnerException is UnauthorizedAccessException)
										{
											return new InstructResetCredentialsException(TestConnectivityCredentialsManager.ShortErrorMsgFromException(ex));
										}
										throw;
									}
								}
								credentials.Password = text2.ToString();
								TestConnectivityCredentialsManager.BuildAutomatedTestCasConnectivityInfo(automatedTestCasConnectivityItem, credentials, now);
								automatedTestCasConnectivityItem.Save(SaveMode.NoConflictResolution);
								automatedTestCasConnectivityItem.Load();
								TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(automatedTestCasConnectivityItem, exchangePrincipal, credentials, out now);
								userPrincipalNameFromCredentials = TestConnectivityCredentialsManager.GetUserPrincipalNameFromCredentials(credentials);
								TestConnectivityCredentialsManager.TraceInfo("Calling LogonUser for user " + userPrincipalNameFromCredentials);
								Microsoft.Exchange.Diagnostics.Components.Monitoring.ExTraceGlobals.MonitoringTasksTracer.TraceDebug<string>(0L, "ResetAutomatedCredentialsAndVerify: calling LogonUser for User: {0}", userPrincipalNameFromCredentials);
								flag = NativeMethods.LogonUser(userPrincipalNameFromCredentials, null, credentials.Password, 8, 0, out safeUserTokenHandle);
								if (safeUserTokenHandle != null)
								{
									safeUserTokenHandle.Dispose();
								}
								if (flag)
								{
									TestConnectivityCredentialsManager.CredentialInfo value = new TestConnectivityCredentialsManager.CredentialInfo(credentials, now);
									lock (TestConnectivityCredentialsManager.cachedCredentialsLock)
									{
										TestConnectivityCredentialsManager.cachedCredentials[userPrincipalNameFromCredentials] = value;
									}
									return null;
								}
								return new CasHealthCouldNotLogUserException(TestConnectivityCredentialsManager.GetDomainUserNameFromCredentials(credentials), (text == null) ? "" : text, TestConnectivityCredentialsManager.NewTestUserScriptName, Marshal.GetLastWin32Error().ToString());
							}
						}
					}
				}
			}
			catch (LocalizedException exception)
			{
				return new CasHealthStorageErrorException(exchangePrincipal.MailboxInfo.Location.ServerFqdn, credentials.Domain, credentials.UserName, TestConnectivityCredentialsManager.ShortErrorMsgFromException(exception));
			}
			TestConnectivityCredentialsManager.TraceInfo("Automated probing task information refreshed!");
			return null;
		}

		public static LocalizedException SaveAutomatedCredentials(ExchangePrincipal exchangePrincipal, NetworkCredential credentials)
		{
			if (TestConnectivityCredentialsManager.IsExchangeMultiTenant() && TestConnectivityCredentialsManager.IsStandardDatacenterTestUserAccount(exchangePrincipal))
			{
				return new CannotChangeStandardUserCredentialsException(Strings.CannotChangeStandardUserCredentials(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()));
			}
			try
			{
				if (exchangePrincipal.MailboxInfo.Location.ServerFqdn != null)
				{
					string serverFqdn = exchangePrincipal.MailboxInfo.Location.ServerFqdn;
				}
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, Thread.CurrentThread.CurrentCulture, "Client=Monitoring;Action=Test-CasConnectivity"))
				{
					using (Folder folder = TestConnectivityCredentialsManager.BindToTestCasConnectivityInfoFolder(mailboxSession))
					{
						using (MessageItem automatedTestCasConnectivityItem = TestConnectivityCredentialsManager.GetAutomatedTestCasConnectivityItem(folder, true))
						{
							ExDateTime maxPasswordResetDateTime = TestConnectivityCredentialsManager.GetMaxPasswordResetDateTime();
							TestConnectivityCredentialsManager.BuildAutomatedTestCasConnectivityInfo(automatedTestCasConnectivityItem, credentials, maxPasswordResetDateTime);
							automatedTestCasConnectivityItem.Save(SaveMode.NoConflictResolution);
							TestConnectivityCredentialsManager.CredentialInfo value = new TestConnectivityCredentialsManager.CredentialInfo(credentials, maxPasswordResetDateTime);
							lock (TestConnectivityCredentialsManager.cachedCredentialsLock)
							{
								TestConnectivityCredentialsManager.cachedCredentials[exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()] = value;
							}
							using (SecureString secureString = credentials.Password.ConvertToSecureString())
							{
								DirectoryHelper.ResetPassword(exchangePrincipal.ObjectId, secureString, mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent));
							}
						}
					}
				}
			}
			catch (LocalizedException exception)
			{
				return new CasHealthStorageErrorException(exchangePrincipal.MailboxInfo.Location.ServerFqdn, credentials.Domain, credentials.UserName, TestConnectivityCredentialsManager.ShortErrorMsgFromException(exception));
			}
			TestConnectivityCredentialsManager.TraceInfo("Automated probing task information saved!");
			return null;
		}

		public static LocalizedException LoadAutomatedTestCasConnectivityInfo(ExchangePrincipal exchangePrincipal, NetworkCredential credentials)
		{
			ExDateTime exDateTime;
			return TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(exchangePrincipal, credentials, out exDateTime);
		}

		private static void TraceInfo(string info)
		{
			info = "TestConnectivityCredentialsManager: " + info;
			Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.TraceTracer.Information((long)TestConnectivityCredentialsManager.hashCode, info);
		}

		private static void TraceInfo(string fmt, params object[] p)
		{
			TestConnectivityCredentialsManager.TraceInfo(string.Format(fmt, p));
		}

		private static void TraceError(string info)
		{
			info = "TestConnectivityCredentialsManager: " + info;
			Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.TraceTracer.TraceError((long)TestConnectivityCredentialsManager.hashCode, info);
		}

		internal static bool IsUnderFaultInjection(out SmtpAddress injectedUser)
		{
			string text = null;
			Microsoft.Exchange.Diagnostics.Components.Monitoring.ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3320196413U, ref text);
			if (text != null)
			{
				text = text.Replace("__AT__", "@");
				injectedUser = new SmtpAddress(text);
				return true;
			}
			return false;
		}

		internal static bool IsExchangeMultiTenant()
		{
			bool result = false;
			try
			{
				result = Datacenter.IsMultiTenancyEnabled();
			}
			catch (CannotDetermineExchangeModeException ex)
			{
				TestConnectivityCredentialsManager.TraceInfo("TestCaseConnectivity exception: " + ex.Message);
			}
			return result;
		}

		private static ADRawEntry FindUser(string userSmtpAddress)
		{
			if (string.IsNullOrEmpty(userSmtpAddress))
			{
				return null;
			}
			ADRawEntry result;
			using (WindowsIdentity windowsIdentity = new WindowsIdentity(userSmtpAddress))
			{
				ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(TestConnectivityCredentialsManager.GetDomainFromSmtpAddress(userSmtpAddress));
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 889, "FindUser", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Cas\\TestConnectivityCredentialsManager.cs");
				SecurityIdentifier user = windowsIdentity.User;
				result = tenantOrRootOrgRecipientSession.FindBySid(windowsIdentity.User);
			}
			return result;
		}

		private static string GetDomainFromSmtpAddress(string smtpAddress)
		{
			if (string.IsNullOrEmpty(smtpAddress))
			{
				return null;
			}
			int num = smtpAddress.IndexOf('@');
			return smtpAddress.Substring(num + 1);
		}

		private static string GetMultiTenantTestUserNameFromDatabaseName(string mailboxDatabaseName, DatacenterUserType userType)
		{
			string forestNameForTestAccounts = TestConnectivityCredentialsManager.GetForestNameForTestAccounts();
			return string.Format("{0}@{1}.{2}.{3}", new object[]
			{
				mailboxDatabaseName,
				forestNameForTestAccounts,
				TestConnectivityCredentialsManager.GetTenantNamePrefixFromUserType(userType),
				"exchangemon.net"
			});
		}

		private static string GetForestNameForTestAccounts()
		{
			string result = string.Empty;
			ADForest localForest = ADForest.GetLocalForest();
			if (string.Equals(localForest.Fqdn, "prod.exchangelabs.com", StringComparison.InvariantCultureIgnoreCase))
			{
				result = "namprd01";
			}
			else
			{
				result = localForest.Fqdn.Split(new char[]
				{
					'.'
				})[0];
			}
			return result;
		}

		private static string GetTenantNamePrefixFromUserType(DatacenterUserType userType)
		{
			switch (userType)
			{
			case DatacenterUserType.EDU:
				return "edu";
			case DatacenterUserType.BPOS:
				return "o365";
			default:
				return string.Empty;
			}
		}

		private static bool IsStandardDatacenterTestUserAccount(ExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal == null || string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()))
			{
				return false;
			}
			Regex regex = new Regex(".*@.*[.].*[.]exchangemon.net", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			return regex.IsMatch(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
		}

		private static MessageItem GetAutomatedTestCasConnectivityItem(Folder folder, bool createIfNotThere)
		{
			QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Subject, "TestCasConnectivityInfo");
			object[][] array = null;
			PropertyDefinition[] dataColumns = new PropertyDefinition[]
			{
				ItemSchema.Id
			};
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, dataColumns))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
				{
					array = queryResult.GetRows(1);
				}
			}
			if (array != null && array.Length == 1)
			{
				return MessageItem.Bind(folder.Session, ((VersionedId)array[0][0]).ObjectId);
			}
			if (createIfNotThere)
			{
				return MessageItem.Create(folder.Session, folder.Id);
			}
			return null;
		}

		private static string GetUserPrincipalNameFromCredentials(NetworkCredential credentials)
		{
			if (credentials == null)
			{
				return null;
			}
			if (credentials.UserName.Contains("@") || string.IsNullOrEmpty(credentials.Domain))
			{
				return credentials.UserName;
			}
			return string.Format("{0}@{1}", credentials.UserName, credentials.Domain);
		}

		private static ExDateTime GetMinPasswordResetDateTime()
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime);
		}

		private static ExDateTime GetMaxPasswordResetDateTime()
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime);
		}

		private static LocalizedException LoadAutomatedTestCasConnectivityInfo(ExchangePrincipal exchangePrincipal, NetworkCredential credentials, out ExDateTime lastPasswordReset)
		{
			lastPasswordReset = TestConnectivityCredentialsManager.GetMinPasswordResetDateTime();
			try
			{
				if (TestConnectivityCredentialsManager.IsExchangeMultiTenant() && TestConnectivityCredentialsManager.IsStandardDatacenterTestUserAccount(exchangePrincipal))
				{
					TestConnectivityCredentialsManager.TraceInfo("Detected standard datacenter test-user account. Computing the credentials for the test account in forest {0}", new object[]
					{
						ADForest.GetLocalForest()
					});
					string forestNameForTestAccounts = TestConnectivityCredentialsManager.GetForestNameForTestAccounts();
					string text = (forestNameForTestAccounts.Length < 4) ? forestNameForTestAccounts : forestNameForTestAccounts.Substring(0, 3);
					text = text.ToUpper();
					credentials.Password = "J$p1ter" + text;
					TestConnectivityCredentialsManager.TraceInfo("Credentials successfully loaded for the test account");
					return null;
				}
				TestConnectivityCredentialsManager.TraceInfo("Loading automated probing task information...");
				string userPrincipalNameFromCredentials = TestConnectivityCredentialsManager.GetUserPrincipalNameFromCredentials(credentials);
				if (!string.IsNullOrEmpty(userPrincipalNameFromCredentials))
				{
					TestConnectivityCredentialsManager.CredentialInfo credentialInfo = null;
					lock (TestConnectivityCredentialsManager.cachedCredentialsLock)
					{
						TestConnectivityCredentialsManager.cachedCredentials.TryGetValue(userPrincipalNameFromCredentials, out credentialInfo);
					}
					if (credentialInfo != null)
					{
						SafeUserTokenHandle safeUserTokenHandle;
						bool flag2 = NativeMethods.LogonUser(userPrincipalNameFromCredentials, null, credentialInfo.Credential.Password, 8, 0, out safeUserTokenHandle);
						if (safeUserTokenHandle != null)
						{
							safeUserTokenHandle.Dispose();
						}
						if (flag2)
						{
							lastPasswordReset = credentialInfo.PasswordResetTime;
							credentials.Password = credentialInfo.Credential.Password;
							return null;
						}
					}
				}
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, Thread.CurrentThread.CurrentCulture, "Client=Monitoring;Action=Test-CasConnectivity"))
				{
					using (Folder folder = TestConnectivityCredentialsManager.BindToTestCasConnectivityInfoFolder(mailboxSession))
					{
						using (Item automatedTestCasConnectivityItem = TestConnectivityCredentialsManager.GetAutomatedTestCasConnectivityItem(folder, false))
						{
							if (automatedTestCasConnectivityItem == null)
							{
								return new CasHealthInstructResetCredentialsException();
							}
							if (TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(automatedTestCasConnectivityItem, exchangePrincipal, credentials, out lastPasswordReset))
							{
								TestConnectivityCredentialsManager.CredentialInfo value = new TestConnectivityCredentialsManager.CredentialInfo(credentials, lastPasswordReset);
								string userPrincipalNameFromCredentials2 = TestConnectivityCredentialsManager.GetUserPrincipalNameFromCredentials(credentials);
								if (string.IsNullOrEmpty(userPrincipalNameFromCredentials2))
								{
									throw new InvalidOperationException();
								}
								lock (TestConnectivityCredentialsManager.cachedCredentialsLock)
								{
									TestConnectivityCredentialsManager.cachedCredentials[userPrincipalNameFromCredentials2] = value;
								}
							}
						}
					}
				}
				TestConnectivityCredentialsManager.TraceInfo("Automated probing task information found and loaded!");
			}
			catch (LocalizedException exception)
			{
				return new CasHealthStorageErrorException(exchangePrincipal.MailboxInfo.Location.ServerFqdn, credentials.Domain, credentials.UserName, TestConnectivityCredentialsManager.ShortErrorMsgFromException(exception));
			}
			return null;
		}

		private static bool LoadAutomatedTestCasConnectivityInfo(Item item, ExchangePrincipal exchangePrincipal, NetworkCredential credentials, out ExDateTime lastPasswordReset)
		{
			lastPasswordReset = TestConnectivityCredentialsManager.GetMinPasswordResetDateTime();
			credentials.Password = string.Empty;
			string text;
			using (TextReader textReader = item.Body.OpenTextReader(BodyFormat.TextPlain))
			{
				text = textReader.ReadToEnd().Trim();
			}
			if (string.IsNullOrEmpty(text))
			{
				TestConnectivityCredentialsManager.TraceInfo("Got a null or empty Body from the MessageItem.");
				return false;
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			try
			{
				safeXmlDocument.LoadXml(text);
			}
			catch (XmlException)
			{
				return false;
			}
			return TestConnectivityCredentialsManager.ParseAutomatedTestCasConnectivityInfo(safeXmlDocument, credentials, out lastPasswordReset);
		}

		private static void BuildAutomatedTestCasConnectivityInfo(MessageItem item, NetworkCredential credentials, ExDateTime lastPasswordReset)
		{
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><TestCasHealthInfo><Domain /><UserName /><Password /><LastPasswordReset /></TestCasHealthInfo>";
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/TestCasHealthInfo/Domain");
			xmlNode.InnerText = credentials.Domain;
			xmlNode = xmlDocument.SelectSingleNode("/TestCasHealthInfo/UserName");
			xmlNode.InnerText = credentials.UserName;
			xmlNode = xmlDocument.SelectSingleNode("/TestCasHealthInfo/Password");
			xmlNode.InnerText = credentials.Password;
			xmlNode = xmlDocument.SelectSingleNode("/TestCasHealthInfo/LastPasswordReset");
			xmlNode.InnerText = lastPasswordReset.ToString();
			item.Subject = "TestCasConnectivityInfo";
			item.OpenAsReadWrite();
			using (TextWriter textWriter = item.Body.OpenTextWriter(BodyFormat.TextPlain))
			{
				textWriter.Write(xmlDocument.InnerXml);
			}
		}

		private static Folder BindToTestCasConnectivityInfoFolder(MailboxSession mailboxSession)
		{
			return Folder.Bind(mailboxSession, DefaultFolderType.Configuration);
		}

		private static bool ParseAutomatedTestCasConnectivityInfo(SafeXmlDocument xmlDoc, NetworkCredential credentials, out ExDateTime retLastPasswordReset)
		{
			retLastPasswordReset = TestConnectivityCredentialsManager.GetMinPasswordResetDateTime();
			if (xmlDoc == null)
			{
				return false;
			}
			XmlNode xmlNode = xmlDoc.SelectSingleNode("/TestCasHealthInfo/UserName");
			if (xmlNode == null)
			{
				return false;
			}
			if (credentials.UserName != xmlNode.InnerText)
			{
				return false;
			}
			xmlNode = xmlDoc.SelectSingleNode("/TestCasHealthInfo/Domain");
			if (xmlNode == null)
			{
				return false;
			}
			if (credentials.Domain != xmlNode.InnerText)
			{
				return false;
			}
			xmlNode = xmlDoc.SelectSingleNode("/TestCasHealthInfo/LastPasswordReset");
			if (xmlNode == null)
			{
				return false;
			}
			ExDateTime exDateTime;
			if (!ExDateTime.TryParse(xmlNode.InnerText, out exDateTime))
			{
				return false;
			}
			xmlNode = xmlDoc.SelectSingleNode("/TestCasHealthInfo/Password");
			if (xmlNode == null)
			{
				return false;
			}
			string innerText = xmlNode.InnerText;
			if ("" == xmlNode.Value)
			{
				return false;
			}
			credentials.Password = innerText;
			retLastPasswordReset = exDateTime;
			return true;
		}

		private static string ShortErrorMsgFromException(Exception exception)
		{
			string result = string.Empty;
			if (exception.InnerException != null && !(exception is StorageTransientException) && !(exception is StoragePermanentException))
			{
				result = Strings.CasHealthShortErrorMsgFromExceptionWithInnerException(exception.GetType().ToString(), exception.Message, exception.InnerException.GetType().ToString(), exception.InnerException.Message);
			}
			else
			{
				result = Strings.CasHealthShortErrorMsgFromException(exception.GetType().ToString(), exception.Message);
			}
			return result;
		}

		private static void LogException(Exception ex)
		{
			while (ex != null)
			{
				TestConnectivityCredentialsManager.TraceInfo(string.Format("Exception {0} thrown :\n{1}\n:{2}", ex.GetType(), ex.Message, ex.StackTrace));
				ex = ex.InnerException;
			}
		}

		private static List<string> CacheListOfLocalMailboxDatabases(Task taskConsumer, ITopologyConfigurationSession configurationSession, ADSite site)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, site.Id),
				new ExistsFilter(ActiveDirectoryServerSchema.HostedDatabaseCopies)
			});
			MiniServer[] array = configurationSession.FindMiniServer(null, QueryScope.SubTree, filter, null, 0, null);
			taskConsumer.WriteVerbose(new LocalizedString(string.Format("Found [{0}] mailbox servers in the local site [{1}]", array.Length, site.Name)));
			array.Shuffle<MiniServer>();
			List<string> list = new List<string>();
			MiniServer[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				MiniServer miniServer = array2[i];
				if (list.Count <= 25)
				{
					taskConsumer.WriteVerbose(new LocalizedString(string.Format("Reading database status from MBX server [{0}]", miniServer.Name)));
					RpcDatabaseCopyStatus2[] copyStatus;
					try
					{
						copyStatus = ReplayRpcClientHelper.GetCopyStatus(miniServer.Fqdn, RpcGetDatabaseCopyStatusFlags2.None, null);
					}
					catch (TaskServerTransientException)
					{
						goto IL_241;
					}
					catch (TaskServerException)
					{
						goto IL_241;
					}
					goto IL_D5;
					IL_241:
					i++;
					continue;
					IL_D5:
					int num = 0;
					copyStatus.Shuffle<RpcDatabaseCopyStatus2>();
					foreach (RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus in copyStatus)
					{
						if (num > 5)
						{
							break;
						}
						if (rpcDatabaseCopyStatus.IsActiveCopy())
						{
							string text = null;
							if (TestConnectivityCredentialsManager.cachedDatabaseGuidToNameMapping.ContainsKey(rpcDatabaseCopyStatus.DBGuid))
							{
								text = TestConnectivityCredentialsManager.cachedDatabaseGuidToNameMapping[rpcDatabaseCopyStatus.DBGuid];
							}
							else
							{
								text = rpcDatabaseCopyStatus.DBName;
								if (text == null)
								{
									try
									{
										IADDatabase iaddatabase = AmHelper.FindDatabaseByGuid(rpcDatabaseCopyStatus.DBGuid);
										text = iaddatabase.Name;
									}
									catch (AmDatabaseNotFoundException)
									{
										taskConsumer.WriteVerbose(new LocalizedString(string.Format("AmDatabaseNotFoundException trying to look up mailbox database [{0}]", rpcDatabaseCopyStatus.DBGuid)));
										goto IL_230;
									}
								}
								if (text == null)
								{
									taskConsumer.WriteVerbose(new LocalizedString(string.Format("Could not find the name of mailbox database [{0}]", rpcDatabaseCopyStatus.DBGuid)));
									goto IL_230;
								}
								TestConnectivityCredentialsManager.cachedDatabaseGuidToNameMapping.Add(rpcDatabaseCopyStatus.DBGuid, text);
							}
							taskConsumer.WriteVerbose(new LocalizedString(string.Format("Looking at mailbox database [{0}]", text)));
							if (!list.Contains(text, StringComparer.InvariantCultureIgnoreCase))
							{
								if (text.Contains(" "))
								{
									taskConsumer.WriteVerbose(new LocalizedString(string.Format("Database [{0}] can't host a test user - discarding.", text)));
								}
								else
								{
									taskConsumer.WriteVerbose(new LocalizedString(string.Format("Database [{0}] is mounted on a mailbox server in our site; adding to the list.", text)));
									list.Add(text);
									num++;
								}
							}
						}
						IL_230:;
					}
					goto IL_241;
				}
				break;
			}
			return list;
		}

		private static List<string> CacheListOfLocalTestMailboxes(IEnumerable<string> localMailboxDatabases, Task taskConsumer, ITopologyConfigurationSession configurationSession, ADSite site, DatacenterUserType userType)
		{
			List<string> list = new List<string>();
			foreach (string text in localMailboxDatabases)
			{
				string multiTenantTestUserNameFromDatabaseName = TestConnectivityCredentialsManager.GetMultiTenantTestUserNameFromDatabaseName(text, userType);
				taskConsumer.WriteVerbose(new LocalizedString(string.Format("Looking in AD for test mailbox [{0}] of type [{1}] in database [{2}].", multiTenantTestUserNameFromDatabaseName, userType, text)));
				Exception ex = null;
				try
				{
					if (TestConnectivityCredentialsManager.FindUser(multiTenantTestUserNameFromDatabaseName) == null)
					{
						taskConsumer.WriteVerbose(new LocalizedString(string.Format("Could not find test user [{0}] in AD", multiTenantTestUserNameFromDatabaseName)));
						continue;
					}
					taskConsumer.WriteVerbose(new LocalizedString(string.Format("Found test user [{0}] in AD!", multiTenantTestUserNameFromDatabaseName)));
				}
				catch (SecurityException ex2)
				{
					ex = ex2;
				}
				catch (UnauthorizedAccessException ex3)
				{
					ex = ex3;
				}
				catch (ArgumentException ex4)
				{
					ex = ex4;
				}
				catch (LocalizedException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					TestConnectivityCredentialsManager.LogException(ex);
				}
				else
				{
					list.Add(multiTenantTestUserNameFromDatabaseName);
				}
			}
			return list;
		}

		private static string CacheLegacyTestUserName(Task taskConsumer, ITopologyConfigurationSession configurationSession, ADSite site)
		{
			string automatedTaskDataCenterDomain = TestConnectivityCredentialsManager.GetAutomatedTaskDataCenterDomain(site);
			taskConsumer.WriteVerbose(new LocalizedString(string.Format("Exchange Domain is [{0}]", automatedTaskDataCenterDomain)));
			ADSessionSettings sessionSettings = ADSessionSettings.FromTenantCUName(automatedTaskDataCenterDomain);
			ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 1742, "CacheLegacyTestUserName", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Cas\\TestConnectivityCredentialsManager.cs");
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
				new AmbiguousNameResolutionFilter("extest_")
			});
			ADPagedReader<MiniRecipient> adpagedReader = tenantRecipientSession.FindPagedMiniRecipient<MiniRecipient>(null, QueryScope.OneLevel, filter, null, 0, null);
			MiniRecipient miniRecipient = null;
			if (adpagedReader != null)
			{
				foreach (MiniRecipient miniRecipient2 in adpagedReader)
				{
					if (miniRecipient2 != null && miniRecipient2.Name.StartsWith("extest_", StringComparison.InvariantCultureIgnoreCase))
					{
						miniRecipient = miniRecipient2;
						break;
					}
				}
			}
			if (miniRecipient == null)
			{
				return null;
			}
			return miniRecipient.UserPrincipalName;
		}

		private static void RebuildTestUserCaches(Task taskConsumer)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1802, "RebuildTestUserCaches", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Cas\\TestConnectivityCredentialsManager.cs");
			ADSite localSite = topologyConfigurationSession.GetLocalSite();
			List<string> localMailboxDatabases = TestConnectivityCredentialsManager.CacheListOfLocalMailboxDatabases(taskConsumer, topologyConfigurationSession, localSite);
			string text = TestConnectivityCredentialsManager.CacheLegacyTestUserName(taskConsumer, topologyConfigurationSession, localSite);
			List<string> list = TestConnectivityCredentialsManager.CacheListOfLocalTestMailboxes(localMailboxDatabases, taskConsumer, topologyConfigurationSession, localSite, DatacenterUserType.EDU);
			List<string> list2 = TestConnectivityCredentialsManager.CacheListOfLocalTestMailboxes(localMailboxDatabases, taskConsumer, topologyConfigurationSession, localSite, DatacenterUserType.BPOS);
			TestConnectivityCredentialsManager.cachedLocalLegacyMailbox = text;
			TestConnectivityCredentialsManager.cachedLocalBposMailboxes = list2;
			TestConnectivityCredentialsManager.cachedLocalEduMailboxes = list;
			TestConnectivityCredentialsManager.cachedMailboxLastUpdate = DateTime.UtcNow;
		}

		private static string GetCachedLocalTestMailbox(Task taskConsumer, DatacenterUserType userType)
		{
			string text = null;
			bool flag = false;
			string value = string.Format("GetCachedLocalTestMailbox() received a request for a test mailbox of type '{0}'.", userType.ToString());
			taskConsumer.WriteVerbose(new LocalizedString(value));
			lock (TestConnectivityCredentialsManager.cachedMailboxLock)
			{
				DateTime utcNow = DateTime.UtcNow;
				int num = 0;
				switch (userType)
				{
				case DatacenterUserType.LEGACY:
					if (!string.IsNullOrEmpty(TestConnectivityCredentialsManager.cachedLocalLegacyMailbox))
					{
						num = 1;
					}
					break;
				case DatacenterUserType.EDU:
					if (TestConnectivityCredentialsManager.cachedLocalEduMailboxes != null)
					{
						num = TestConnectivityCredentialsManager.cachedLocalEduMailboxes.Count;
					}
					break;
				case DatacenterUserType.BPOS:
					if (TestConnectivityCredentialsManager.cachedLocalBposMailboxes != null)
					{
						num = TestConnectivityCredentialsManager.cachedLocalBposMailboxes.Count;
					}
					break;
				}
				if (num == 0)
				{
					flag = true;
				}
				value = string.Format("Cache currently holds '{0}' mailboxes of type '{1}'.", num.ToString(), userType.ToString());
				taskConsumer.WriteVerbose(new LocalizedString(value));
				if (!flag)
				{
					int num2 = MonitoringHelper.ReadIntAppSettingKey("TestUserCacheRefreshMinutes", 5, 1440, 60);
					value = string.Format("Current UTC time is '{0}'. Last cache update was '{1}'. Cache refresh policy is every '{2}' minutes.", utcNow.ToString("R"), TestConnectivityCredentialsManager.cachedMailboxLastUpdate.ToString("R"), num2.ToString());
					taskConsumer.WriteVerbose(new LocalizedString(value));
					if (utcNow > TestConnectivityCredentialsManager.cachedMailboxLastUpdate.AddMinutes((double)num2))
					{
						flag = true;
					}
				}
				if (flag)
				{
					taskConsumer.WriteVerbose(new LocalizedString(string.Format("Building the cache now.", new object[0])));
					TestConnectivityCredentialsManager.RebuildTestUserCaches(taskConsumer);
				}
				else
				{
					taskConsumer.WriteVerbose(new LocalizedString(string.Format("The cache does not need to be refreshed.", new object[0])));
				}
				Random random = new Random();
				switch (userType)
				{
				case DatacenterUserType.LEGACY:
					text = TestConnectivityCredentialsManager.cachedLocalLegacyMailbox;
					break;
				case DatacenterUserType.EDU:
				{
					int count = TestConnectivityCredentialsManager.cachedLocalEduMailboxes.Count;
					if (count > 0)
					{
						int index = random.Next(count);
						text = TestConnectivityCredentialsManager.cachedLocalEduMailboxes[index];
					}
					break;
				}
				case DatacenterUserType.BPOS:
				{
					int count = TestConnectivityCredentialsManager.cachedLocalBposMailboxes.Count;
					if (count > 0)
					{
						int index = random.Next(count);
						text = TestConnectivityCredentialsManager.cachedLocalBposMailboxes[index];
					}
					break;
				}
				}
			}
			if (text != null)
			{
				value = string.Format("Returning test mailbox '{0}'.", text);
				taskConsumer.WriteVerbose(new LocalizedString(value));
				return text;
			}
			value = string.Format("Can't find a test mailbox of type '{0}'.", userType.ToString());
			taskConsumer.WriteVerbose(new LocalizedString(value));
			throw new CannotFindTestUserException();
		}

		internal const string MultiTenantTestDomain = "exchangemon.net";

		internal const string TestUserNamePrefix = "extest_";

		private const string InfoDocument = "<?xml version=\"1.0\" encoding=\"utf-8\"?><TestCasHealthInfo><Domain /><UserName /><Password /><LastPasswordReset /></TestCasHealthInfo>";

		private const string DomainXPath = "/TestCasHealthInfo/Domain";

		private const string UserNameXPath = "/TestCasHealthInfo/UserName";

		private const string PasswordXPath = "/TestCasHealthInfo/Password";

		private const string LastPasswordResetXPath = "/TestCasHealthInfo/LastPasswordReset";

		private const uint MonitoringTestConnectivityUser = 3320196413U;

		private const string ProbingInfoItemSubject = "TestCasConnectivityInfo";

		private static int hashCode = Assembly.GetExecutingAssembly().GetHashCode();

		private static object cachedMailboxLock = new object();

		private static Dictionary<Guid, string> cachedDatabaseGuidToNameMapping = new Dictionary<Guid, string>();

		private static List<string> cachedLocalEduMailboxes = null;

		private static List<string> cachedLocalBposMailboxes = null;

		private static string cachedLocalLegacyMailbox = null;

		private static DateTime cachedMailboxLastUpdate = DateTime.UtcNow.AddYears(-1);

		private static object cachedCredentialsLock = new object();

		private static Dictionary<string, TestConnectivityCredentialsManager.CredentialInfo> cachedCredentials = new Dictionary<string, TestConnectivityCredentialsManager.CredentialInfo>();

		private class CredentialInfo
		{
			internal NetworkCredential Credential { get; private set; }

			internal ExDateTime PasswordResetTime { get; private set; }

			internal CredentialInfo(NetworkCredential credential, ExDateTime resetTime)
			{
				this.Credential = credential;
				this.PasswordResetTime = resetTime;
			}
		}

		private class PasswordGenerator
		{
			public static string CreateNew()
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
				{
					byte[] array = new byte[4];
					for (int i = 0; i < 30; i++)
					{
						rngcryptoServiceProvider.GetBytes(array);
						stringBuilder.Append(TestConnectivityCredentialsManager.PasswordGenerator.PasswordChars[(int)(checked((IntPtr)(unchecked((ulong)BitConverter.ToUInt32(array, 0) % (ulong)((long)TestConnectivityCredentialsManager.PasswordGenerator.PasswordChars.Length)))))]);
					}
					rngcryptoServiceProvider.GetBytes(array);
					uint num = BitConverter.ToUInt32(array, 0) % 29U + 1U;
					int num2 = 0;
					while ((long)num2 < (long)((ulong)num))
					{
						rngcryptoServiceProvider.GetBytes(array);
						uint index = BitConverter.ToUInt32(array, 0) % 29U + 1U;
						rngcryptoServiceProvider.GetBytes(array);
						stringBuilder[(int)index] = TestConnectivityCredentialsManager.PasswordGenerator.LowerCaseLetters[(int)(checked((IntPtr)(unchecked((ulong)BitConverter.ToUInt32(array, 0) % (ulong)((long)TestConnectivityCredentialsManager.PasswordGenerator.LowerCaseLetters.Length)))))];
						num2++;
					}
					rngcryptoServiceProvider.GetBytes(array);
					num = BitConverter.ToUInt32(array, 0) % 29U + 1U;
					int num3 = 0;
					while ((long)num3 < (long)((ulong)num))
					{
						rngcryptoServiceProvider.GetBytes(array);
						uint index2 = BitConverter.ToUInt32(array, 0) % 29U + 1U;
						rngcryptoServiceProvider.GetBytes(array);
						stringBuilder[(int)index2] = TestConnectivityCredentialsManager.PasswordGenerator.UpperCaseLetters[(int)(checked((IntPtr)(unchecked((ulong)BitConverter.ToUInt32(array, 0) % (ulong)((long)TestConnectivityCredentialsManager.PasswordGenerator.UpperCaseLetters.Length)))))];
						num3++;
					}
					rngcryptoServiceProvider.GetBytes(array);
					num = BitConverter.ToUInt32(array, 0) % 29U + 1U;
					int num4 = 0;
					while ((long)num4 < (long)((ulong)num))
					{
						rngcryptoServiceProvider.GetBytes(array);
						uint index3 = BitConverter.ToUInt32(array, 0) % 29U + 1U;
						rngcryptoServiceProvider.GetBytes(array);
						stringBuilder[(int)index3] = TestConnectivityCredentialsManager.PasswordGenerator.SymbolChars[(int)(checked((IntPtr)(unchecked((ulong)BitConverter.ToUInt32(array, 0) % (ulong)((long)TestConnectivityCredentialsManager.PasswordGenerator.SymbolChars.Length)))))];
						num4++;
					}
					rngcryptoServiceProvider.GetBytes(array);
					num = BitConverter.ToUInt32(array, 0) % 29U + 1U;
					int num5 = 0;
					while ((long)num5 < (long)((ulong)num))
					{
						rngcryptoServiceProvider.GetBytes(array);
						uint index4 = BitConverter.ToUInt32(array, 0) % 29U + 1U;
						rngcryptoServiceProvider.GetBytes(array);
						stringBuilder[(int)index4] = TestConnectivityCredentialsManager.PasswordGenerator.NumberChars[(int)((UIntPtr)(BitConverter.ToUInt32(array, 0) % 10U))];
						num5++;
					}
				}
				return stringBuilder.ToString();
			}

			private const int PasswordLength = 30;

			private static char[] PasswordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?".ToCharArray();

			private static char[] UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

			private static char[] LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

			private static char[] NumberChars = "0123456789".ToCharArray();

			private static char[] SymbolChars = "`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?".ToCharArray();
		}
	}
}
