using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class ProtocolConnectivity : TestCasConnectivity
	{
		[Alias(new string[]
		{
			"Identity"
		})]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public ServerIdParameter ClientAccessServer
		{
			get
			{
				return this.clientAccessServer;
			}
			set
			{
				this.clientAccessServer = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateNotNullOrEmpty]
		public PSCredential MailboxCredential
		{
			get
			{
				return this.mailboxCredential;
			}
			set
			{
				this.mailboxCredential = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ProtocolConnectionType ConnectionType
		{
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter TrustAnySSLCertificate
		{
			get
			{
				return this.trustAllCertificates;
			}
			set
			{
				this.trustAllCertificates = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public int PortClientAccessServer
		{
			get
			{
				return this.port;
			}
			set
			{
				this.port = value;
			}
		}

		[ValidateRange(1, 120)]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public int PerConnectionTimeout
		{
			get
			{
				return (int)(base.Fields["PerConnectionTimeout"] ?? 24);
			}
			set
			{
				base.Fields["PerConnectionTimeout"] = value;
			}
		}

		protected bool SendTestMessage
		{
			get
			{
				return this.sendTestMessage;
			}
			set
			{
				this.sendTestMessage = value;
			}
		}

		protected string CurrentProtocol
		{
			get
			{
				return this.currrentProtocol;
			}
			set
			{
				this.currrentProtocol = value;
			}
		}

		protected string MailSubject
		{
			get
			{
				return this.mailSubject;
			}
		}

		protected ADUser User
		{
			get
			{
				return this.user;
			}
		}

		protected string MailboxFqdn
		{
			get
			{
				if (!string.IsNullOrEmpty(this.mailboxFqdn))
				{
					return this.mailboxFqdn;
				}
				return string.Empty;
			}
		}

		protected string CmdletMonitoringEventSource
		{
			get
			{
				return "MSExchange Monitoring " + this.CmdletNoun;
			}
		}

		protected string CmdletNoun
		{
			get
			{
				return this.cmdletNoun;
			}
			set
			{
				this.cmdletNoun = value;
			}
		}

		protected string MailSubjectPrefix
		{
			get
			{
				return this.mailSubjectPrefix;
			}
			set
			{
				this.mailSubjectPrefix = value;
			}
		}

		protected override string TransactionSuccessEventMessage
		{
			get
			{
				return Strings.ProtocolTransactionsSucceeded(this.currrentProtocol);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string text = Strings.ProtocolConfrimationMessage(this.CurrentProtocol);
				text = text + Strings.SelectedConnectionType(this.connection.ToString()) + "\r\n\r\n";
				if (this.TrustAnySSLCertificate)
				{
					text = text + Strings.ProtolcolWarnTrustAllCertificates.ToString() + "\r\n\r\n";
				}
				if (this.MailboxCredential != null)
				{
					try
					{
						NetworkCredential networkCredential = this.MailboxCredential.GetNetworkCredential();
						text = text + Strings.CasHealthWarnUserCredentials(networkCredential.Domain + "\\" + networkCredential.UserName).ToString() + "\r\n\r\n";
					}
					catch (PSArgumentException)
					{
						text = text + Strings.CasHealthWarnUserCredentials(this.MailboxCredential.UserName).ToString() + "\r\n\r\n";
					}
				}
				return new LocalizedString(text);
			}
		}

		internal static string ShortErrorMessageFromException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			string result = string.Empty;
			result = exception.Message;
			if (exception.InnerException != null)
			{
				result = Strings.ProtocolTransactionShortErrorMsgFromExceptionWithInnerException(exception.GetType().ToString(), exception.Message, exception.InnerException.GetType().ToString(), exception.InnerException.Message);
			}
			else
			{
				result = Strings.ProtocolTransactionShortErrorMsgFromException(exception.GetType().ToString(), exception.Message);
			}
			return result;
		}

		internal static LocalizedException SetUserInformation(TestCasConnectivity.TestCasConnectivityRunInstance instance, IConfigDataProvider session, ref ADUser user)
		{
			if (session == null)
			{
				throw new LocalizedException(Strings.ErrorNullParameter("session"));
			}
			if (instance == null)
			{
				throw new LocalizedException(Strings.ErrorNullParameter("instance"));
			}
			IRecipientSession recipientSession = session as IRecipientSession;
			if (recipientSession == null)
			{
				throw new LocalizedException(Strings.ErrorNullParameter("recipientSession"));
			}
			if (!string.IsNullOrEmpty(instance.credentials.Domain) && instance.credentials.UserName.IndexOf('@') == -1)
			{
				WindowsIdentity windowsIdentity = null;
				string text = instance.credentials.UserName + "@" + instance.credentials.Domain;
				try
				{
					windowsIdentity = new WindowsIdentity(text);
				}
				catch (SecurityException ex)
				{
					return new CasHealthUserNotFoundException(text, ex.Message);
				}
				using (windowsIdentity)
				{
					user = (recipientSession.FindBySid(windowsIdentity.User) as ADUser);
					if (user == null)
					{
						return new AdUserNotFoundException(text, string.Empty);
					}
					goto IL_174;
				}
			}
			user = null;
			RecipientIdParameter recipientIdParameter = RecipientIdParameter.Parse(instance.credentials.UserName);
			IEnumerable<ADUser> objects = recipientIdParameter.GetObjects<ADUser>(null, recipientSession);
			if (objects != null)
			{
				IEnumerator<ADUser> enumerator = objects.GetEnumerator();
				if (enumerator != null && enumerator.MoveNext())
				{
					user = enumerator.Current;
				}
				if (enumerator.MoveNext())
				{
					user = null;
					return new AdUserNotUniqueException(instance.credentials.UserName);
				}
			}
			if (user == null)
			{
				return new AdUserNotFoundException(instance.credentials.UserName, string.Empty);
			}
			IL_174:
			return null;
		}

		internal static LocalizedException SetExchangePrincipalInformation(TestCasConnectivity.TestCasConnectivityRunInstance instance, ADUser user)
		{
			if (instance == null)
			{
				return new LocalizedException(Strings.ErrorNullParameter("instance"));
			}
			if (user == null)
			{
				return new LocalizedException(Strings.ErrorNullParameter("user"));
			}
			if (instance.exchangePrincipal == null)
			{
				try
				{
					if (user is ADSystemMailbox)
					{
						throw new NotSupportedException("not supported for ADSystemMailbox");
					}
					instance.exchangePrincipal = ExchangePrincipal.FromADUser(user, null);
				}
				catch (ObjectNotFoundException result)
				{
					return result;
				}
				catch (InvalidCastException innerException)
				{
					return new LocalizedException(Strings.ErrorInvalidCasting, innerException);
				}
				catch (LocalizedException result2)
				{
					return result2;
				}
			}
			return null;
		}

		protected override string TransactionFailuresEventMessage(string detailedInformation)
		{
			return Strings.ProtocolTransactionsFailed(this.currrentProtocol, detailedInformation);
		}

		protected override string TransactionWarningsEventMessage(string detailedInformation)
		{
			return Strings.ProtocolTransactionWarnings(this.currrentProtocol, detailedInformation);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (this.casToTest == null && this.clientAccessServer == null)
				{
					CasHealthMustSpecifyCasException exception = new CasHealthMustSpecifyCasException();
					base.WriteMonitoringEvent(2020, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(exception));
				}
			}
			catch (ADTransientException exception2)
			{
				base.CasConnectivityWriteError(exception2, ErrorCategory.ObjectNotFound, null);
			}
			finally
			{
				if (base.HasErrors && base.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 617, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Common\\ProtocolConnectivity.cs");
			if (!this.recipientSession.IsReadConnectionAvailable())
			{
				this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 625, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Common\\ProtocolConnectivity.cs");
			}
			this.recipientSession.UseGlobalCatalog = true;
			this.recipientSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds(15.0));
			this.systemConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 634, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Common\\ProtocolConnectivity.cs");
			this.systemConfigurationSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds(15.0));
		}

		protected override List<CasTransactionOutcome> ExecuteTests(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			CasTransactionOutcome casTransactionOutcome = null;
			if (!this.SetUpConnectivityTests(instance, out casTransactionOutcome))
			{
				instance.Outcomes.Enqueue(casTransactionOutcome);
				instance.Result.Outcomes.Add(casTransactionOutcome);
				instance.Result.Complete();
				return null;
			}
			this.ExecuteTestConnectivity(instance);
			return null;
		}

		protected override List<TestCasConnectivity.TestCasConnectivityRunInstance> PopulateInfoPerCas(TestCasConnectivity.TestCasConnectivityRunInstance instance, List<CasTransactionOutcome> outcomeList)
		{
			List<TestCasConnectivity.TestCasConnectivityRunInstance> list = new List<TestCasConnectivity.TestCasConnectivityRunInstance>();
			if (string.IsNullOrEmpty(instance.CasFqdn) || this.casToTest == null)
			{
				CasHealthMustSpecifyCasException exception = new CasHealthMustSpecifyCasException();
				base.CasConnectivityWriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			else
			{
				list.Add(instance);
			}
			return list;
		}

		protected virtual void ExecuteTestConnectivity(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			throw new NotImplementedException("ExecuteTestConnectivity");
		}

		protected virtual string MonitoringLatencyPerformanceCounter()
		{
			return string.Empty;
		}

		protected Exception CheckDatabaseStatus(string databaseName, string mailboxServerName, ref bool databaseResult)
		{
			databaseResult = false;
			LocalizedString localizedString = default(LocalizedString);
			MailboxDatabase mailboxDatabase = null;
			try
			{
				if (string.IsNullOrEmpty(mailboxServerName))
				{
					throw new ArgumentNullException("mailboxServerName");
				}
				if (string.IsNullOrEmpty(databaseName))
				{
					throw new ArgumentNullException("databaseName");
				}
				Server server = base.FindServerByHostName(mailboxServerName);
				if (server == null)
				{
					return new CasHealthMailboxServerNotFoundException(mailboxServerName);
				}
				MailboxDatabase[] mailboxDatabases = server.GetMailboxDatabases();
				foreach (MailboxDatabase mailboxDatabase2 in mailboxDatabases)
				{
					if (string.Compare(databaseName, mailboxDatabase2.Name, true, CultureInfo.InvariantCulture) == 0)
					{
						mailboxDatabase = mailboxDatabase2;
						break;
					}
				}
			}
			catch (ArgumentException result)
			{
				return result;
			}
			catch (ADTransientException result2)
			{
				return result2;
			}
			if (mailboxDatabase == null)
			{
				localizedString = new LocalizedString(Strings.ErrorMailboxDatabaseNotFound(databaseName));
				return new LocalizedException(localizedString);
			}
			if (mailboxDatabase.Recovery)
			{
				string databaseId = mailboxDatabase.ServerName + "\\\\" + mailboxDatabase.Name;
				RecoveryMailboxDatabaseNotMonitoredException exception = new RecoveryMailboxDatabaseNotMonitoredException(databaseId);
				base.WriteMonitoringEvent(2009, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(exception));
			}
			else
			{
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", mailboxDatabase.ServerName, null, null, null))
				{
					MdbStatus[] array2 = exRpcAdmin.ListMdbStatus(new Guid[]
					{
						mailboxDatabase.Guid
					});
					if (array2.Length != 0)
					{
						databaseResult = ((array2[0].Status & MdbStatusFlags.Online) == MdbStatusFlags.Online);
						if (!databaseResult)
						{
							base.WriteMonitoringEvent(2023, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(new LocalizedException(Strings.ErrorDatabaseOffline)));
						}
					}
				}
			}
			return null;
		}

		protected override List<CasTransactionOutcome> BuildPerformanceOutcomes(TestCasConnectivity.TestCasConnectivityRunInstance instance, string mbxFqdn)
		{
			return new List<CasTransactionOutcome>
			{
				new CasTransactionOutcome(instance.CasFqdn, Strings.TestProtocolConnectivity(this.CurrentProtocol), Strings.ProtocolConnectivityScenario(this.CurrentProtocol), this.MonitoringLatencyPerformanceCounter(), base.LocalSiteName, true, instance.credentials.UserName)
			};
		}

		private bool SetUpConnectivityTests(TestCasConnectivity.TestCasConnectivityRunInstance instance, out CasTransactionOutcome outcome)
		{
			MailboxSession mailboxSession = null;
			outcome = new CasTransactionOutcome(base.CasFqdn, Strings.ValidatingTestCasConnectivityRunInstance, Strings.ValidatingTestCasConnectivityRunInstance, this.MonitoringLatencyPerformanceCounter(), base.LocalSiteName, true, string.Empty);
			if (instance == null)
			{
				base.WriteMonitoringEvent(2020, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(new ArgumentNullException("instance")));
				outcome.Update(CasTransactionResultEnum.Failure);
				return false;
			}
			this.userName = instance.credentials.UserName;
			if (!this.SetUpConnTestValidateAndMakeADUser(instance, out outcome))
			{
				return false;
			}
			if (!this.SetUpConnTestSetExchangePrincipal(instance, out outcome))
			{
				return false;
			}
			if (instance.credentials.Password == null)
			{
				base.WriteMonitoringEvent(2020, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(new ArgumentNullException("password")));
				outcome.Update(CasTransactionResultEnum.Failure);
				return false;
			}
			if (instance.LightMode)
			{
				return true;
			}
			try
			{
				if (!this.SetUpConnTestSetCreateMailboxSession(instance, out mailboxSession, out outcome))
				{
					return false;
				}
				if (!this.SetUpConnTestSendTestMessage(instance, mailboxSession, out outcome))
				{
					return false;
				}
			}
			finally
			{
				if (mailboxSession != null)
				{
					mailboxSession.Dispose();
					mailboxSession = null;
				}
			}
			return true;
		}

		private bool SetUpConnTestValidateAndMakeADUser(TestCasConnectivity.TestCasConnectivityRunInstance instance, out CasTransactionOutcome outcome)
		{
			outcome = new CasTransactionOutcome(base.CasFqdn, Strings.ValidatingUserObject, Strings.ValidatingUserObjectDescription, this.MonitoringLatencyPerformanceCounter(), base.LocalSiteName, false, this.userName);
			LocalizedException ex = ProtocolConnectivity.SetUserInformation(instance, this.recipientSession, ref this.user);
			if (ex != null)
			{
				base.WriteMonitoringEvent(2001, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(ex));
				outcome.Update(CasTransactionResultEnum.Failure);
				return false;
			}
			if (this.user != null && !this.user.IsMailboxEnabled)
			{
				UserIsNotMailBoxEnabledException exception = new UserIsNotMailBoxEnabledException(instance.credentials.UserName);
				base.WriteMonitoringEvent(2003, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(exception));
				outcome.Update(CasTransactionResultEnum.Failure);
				return false;
			}
			outcome.Update(CasTransactionResultEnum.Success);
			return true;
		}

		private bool SetUpConnTestSetExchangePrincipal(TestCasConnectivity.TestCasConnectivityRunInstance instance, out CasTransactionOutcome outcome)
		{
			outcome = null;
			if (instance.exchangePrincipal == null)
			{
				outcome = new CasTransactionOutcome(base.CasFqdn, Strings.CreateExchangePrincipalObject, Strings.CreateExchangePrincipalObject, this.MonitoringLatencyPerformanceCounter(), base.LocalSiteName, false, this.userName);
				LocalizedException ex = ProtocolConnectivity.SetExchangePrincipalInformation(instance, this.User);
				if (ex != null)
				{
					base.WriteMonitoringEvent(2021, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(ex));
					outcome.Update(CasTransactionResultEnum.Failure);
					return false;
				}
				outcome.Update(CasTransactionResultEnum.Success);
			}
			return true;
		}

		private bool SetUpConnTestSetCreateMailboxSession(TestCasConnectivity.TestCasConnectivityRunInstance instance, out MailboxSession mailboxSession, out CasTransactionOutcome outcome)
		{
			this.mailboxFqdn = instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn;
			string text = string.Empty;
			mailboxSession = null;
			outcome = null;
			try
			{
				outcome = this.BuildOutcome(Strings.CreateMailboxSession, Strings.CreateMailboxSessionDetail(this.User.PrimarySmtpAddress.ToString()), instance);
				mailboxSession = MailboxSession.OpenAsAdmin(instance.exchangePrincipal, CultureInfo.InvariantCulture, "Client=Monitoring;Action=System Management");
			}
			catch (StorageTransientException exception)
			{
				text = ProtocolConnectivity.ShortErrorMessageFromException(exception);
				new MailboxCreationException(this.User.PrimarySmtpAddress.ToString(), text);
				outcome.Update(CasTransactionResultEnum.Failure, Strings.ErrorMailboxCreationFailure(this.User.PrimarySmtpAddress.ToString(), text));
				return false;
			}
			catch (StoragePermanentException exception2)
			{
				text = ProtocolConnectivity.ShortErrorMessageFromException(exception2);
				new MailboxCreationException(this.User.PrimarySmtpAddress.ToString(), text);
				outcome.Update(CasTransactionResultEnum.Failure, Strings.ErrorMailboxCreationFailure(this.User.PrimarySmtpAddress.ToString(), text));
				return false;
			}
			catch (ArgumentException exception3)
			{
				text = ProtocolConnectivity.ShortErrorMessageFromException(exception3);
				LocalizedException exception4 = new LocalizedException(new LocalizedString(text));
				base.WriteMonitoringEvent(2013, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(exception4));
				outcome.Update(CasTransactionResultEnum.Failure);
				return false;
			}
			outcome.Update(CasTransactionResultEnum.Success);
			return true;
		}

		private bool SetUpConnTestSendTestMessage(TestCasConnectivity.TestCasConnectivityRunInstance instance, MailboxSession mailboxSession, out CasTransactionOutcome outcome)
		{
			outcome = null;
			if (this.SendTestMessage)
			{
				outcome = this.BuildOutcome(Strings.DatabaseStatus, Strings.CheckDatabaseStatus(this.user.Database.Name), instance);
				bool flag = false;
				Exception ex = this.CheckDatabaseStatus(this.user.Database.Name, this.MailboxFqdn, ref flag);
				if (!flag)
				{
					if (ex == null)
					{
						goto IL_16C;
					}
				}
				try
				{
					outcome = this.BuildOutcome(Strings.SendMessage, Strings.SendMessage, instance);
					using (MessageItem messageItem = MessageItem.Create(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)))
					{
						this.mailSubject = this.MailSubjectPrefix + Guid.NewGuid().ToString();
						messageItem.Subject = this.MailSubject;
						using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextPlain))
						{
							textWriter.Write("Body of -");
							textWriter.Write(this.MailSubject);
						}
						messageItem.IsReadReceiptRequested = false;
						messageItem.Save(SaveMode.NoConflictResolution);
						instance.Outcomes.Enqueue(Strings.TestMessageSent(this.MailSubject, this.user.PrimarySmtpAddress.ToString()));
					}
					return true;
				}
				catch (LocalizedException exception)
				{
					base.WriteMonitoringEvent(2008, this.CmdletMonitoringEventSource, EventTypeEnumeration.Error, base.ShortErrorMsgFromException(exception));
					outcome.Update(CasTransactionResultEnum.Failure);
					return false;
				}
				IL_16C:
				if (ex != null)
				{
					outcome.Update(CasTransactionResultEnum.Failure, ex.Message);
				}
				else
				{
					outcome.Update(CasTransactionResultEnum.Failure);
				}
				return false;
			}
			return true;
		}

		protected const string CrlfText = "\r\n\r\n";

		protected const double LatencyPerformanceIncaseOfError = -1.0;

		private const int DefaultConnectionCacheSize = 50;

		private const int DefaultADOperationsTimeoutInSeconds = 15;

		private const int DefaultConnectionTimeoutInSeconds = 24;

		private string mailboxFqdn;

		private string cmdletNoun = "ProtocolConnectivity";

		private string mailSubjectPrefix = "Connectivity";

		private string mailSubject;

		private string userName;

		private ADUser user;

		private int port;

		private ProtocolConnectionType connection;

		private string currrentProtocol;

		private bool sendTestMessage;

		private IRecipientSession recipientSession;

		private IConfigurationSession systemConfigurationSession;

		internal new static class EventId
		{
			public const int TransactionSucceeded = 2000;

			public const int UserNotFound = 2001;

			public const int SecurityErrorFound = 2002;

			public const int MailboxNotEnabled = 2003;

			public const int OperationOnOldServer = 2004;

			public const int SessionObjectNotCreated = 2005;

			public const int NetworkCredentialIsNull = 2006;

			public const int NodeDoesNotOwnExchangeVirtualServer = 2007;

			public const int MessageNotSent = 2008;

			public const int RecoveryMailboxDatabaseNotMonitored = 2009;

			public const int UserNotUnique = 2010;

			public const int ServerIsNotCas = 2011;

			public const int CasServerNotFound = 2012;

			public const int ServiceNotRunning = 2013;

			public const int InvalidMonadTask = 2014;

			public const int MailboxDatabaseNotFound = 2015;

			public const int MailboxDatabaseNotUnique = 2016;

			public const int CasServerNotSpecified = 2017;

			public const int CasServerNotRpcEnabled = 2018;

			public const int TransactionFailed = 2019;

			public const int InvalidData = 2020;

			public const int ExchangePrincipalCreationFailed = 2021;

			public const int MailboxAndCasSeverInDiffrentDomain = 2022;

			public const int MailboxDatabaseOffline = 2023;
		}
	}
}
