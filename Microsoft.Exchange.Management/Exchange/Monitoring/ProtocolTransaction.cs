using System;
using System.Collections;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class ProtocolTransaction
	{
		internal ProtocolTransaction()
		{
			this.stopwatch = new Stopwatch();
		}

		internal ProtocolTransaction(ADUser user, string serverName, string password, int port)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentNullException("server");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}
			this.user = user;
			this.casServer = serverName;
			this.password = password;
			this.port = port;
			this.stopwatch = new Stopwatch();
		}

		internal ProtocolConnectionType ConnectionType
		{
			get
			{
				return this.connectionType;
			}
			set
			{
				this.connectionType = value;
			}
		}

		internal bool TrustAnySslCertificate
		{
			get
			{
				return this.trustAnySslCertificate;
			}
			set
			{
				this.trustAnySslCertificate = value;
			}
		}

		internal string MailSubject
		{
			get
			{
				return this.mailSubject;
			}
			set
			{
				this.mailSubject = value;
			}
		}

		internal CasTransactionOutcome TransactionResult
		{
			get
			{
				return this.transactionResult;
			}
			set
			{
				this.transactionResult = value;
			}
		}

		internal Queue InstanceQueue
		{
			get
			{
				return this.instance.Outcomes;
			}
		}

		internal TestCasConnectivity.TestCasConnectivityRunInstance Instance
		{
			get
			{
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}

		internal bool LightMode
		{
			get
			{
				return this.instance.LightMode;
			}
		}

		protected ADUser User
		{
			get
			{
				return this.user;
			}
		}

		protected string CasServer
		{
			get
			{
				return this.casServer;
			}
		}

		protected string Password
		{
			get
			{
				return this.password;
			}
		}

		protected int Port
		{
			get
			{
				return this.port;
			}
		}

		protected Stopwatch LatencyTimer
		{
			get
			{
				return this.stopwatch;
			}
			set
			{
				this.stopwatch = value;
			}
		}

		internal abstract void Execute();

		protected void Verbose(object message)
		{
			this.instance.Outcomes.Enqueue(message);
		}

		protected bool IsServiceRunning(string serviceName, string server)
		{
			using (ServiceController serviceController = new ServiceController(serviceName, server))
			{
				if (serviceController.Status != ServiceControllerStatus.Running)
				{
					this.transactionResult.Update(CasTransactionResultEnum.Failure, TimeSpan.Zero, Strings.ServiceNotRunning(serviceName));
					return false;
				}
			}
			return true;
		}

		protected void UpdateTransactionResults(CasTransactionResultEnum status, object messageObject)
		{
			string additionalInformation;
			if (messageObject == null)
			{
				additionalInformation = string.Empty;
			}
			else if (messageObject is Exception)
			{
				additionalInformation = ProtocolTransaction.FormatExceptionOutput((Exception)messageObject);
			}
			else
			{
				additionalInformation = messageObject.ToString();
			}
			this.transactionResult.Update(status, this.stopwatch.Elapsed, additionalInformation);
		}

		protected void CompleteTransaction()
		{
			this.stopwatch.Stop();
			this.instance.Outcomes.Enqueue(this.transactionResult);
			this.instance.Result.Outcomes.Add(this.transactionResult);
			this.instance.Result.Complete();
		}

		private static string FormatExceptionOutput(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				stringBuilder.AppendFormat("{0}: {1}", ex.GetType(), (ex.Message != null) ? ex.Message : string.Empty);
				if (ex.InnerException != null)
				{
					stringBuilder.Append(" ---> ");
				}
			}
			return stringBuilder.ToString();
		}

		protected const string POPService = "MSExchangePOP3";

		protected const string IMAPService = "MSExchangeIMAP4";

		protected const string IISService = "IISADMIN";

		private const string DateClause = "\r\nDate:";

		private const string SubjectClause = "\r\nSubject:";

		private const char RChar = '\r';

		private string mailSubject;

		private readonly int port;

		private ADUser user;

		private readonly string password;

		private readonly string casServer;

		private ProtocolConnectionType connectionType;

		private bool trustAnySslCertificate;

		private CasTransactionOutcome transactionResult;

		private TestCasConnectivity.TestCasConnectivityRunInstance instance;

		private Stopwatch stopwatch;
	}
}
