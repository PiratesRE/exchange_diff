using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class ImapTransaction : ProtocolTransaction
	{
		internal ImapTransaction(ADUser user, string serverName, string password, int port) : base(user, serverName, password, port)
		{
		}

		internal override void Execute()
		{
			this.currentState = ImapTransaction.State.Disconnected;
			base.LatencyTimer = Stopwatch.StartNew();
			if (!base.IsServiceRunning("MSExchangeIMAP4", base.CasServer))
			{
				base.Verbose(Strings.ServiceNotRunning("MSExchangeIMAP4"));
				base.UpdateTransactionResults(CasTransactionResultEnum.Failure, Strings.ServiceNotRunning("MSExchangeIMAP4"));
				base.CompleteTransaction();
				return;
			}
			this.imapClient = new ImapClient(base.CasServer, base.Port, base.ConnectionType, base.TrustAnySslCertificate);
			this.imapClient.ExceptionReporter = new DataCommunicator.ExceptionReporterDelegate(this.ExceptionReporter);
			this.imapClient.HasLoggedIn = false;
			base.Verbose(Strings.PopImapConnecting);
			this.currentState = ImapTransaction.State.Connecting;
			this.imapClient.Connect(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
		}

		private static string[] ParseMessageIDs(string response)
		{
			string[] array = new string[0];
			string[] array2 = Regex.Split(response, "\r\n");
			if (array2 != null && array2.Length > 0)
			{
				string[] array3 = array2[0].Split(new char[]
				{
					' '
				});
				if (array3 != null && array3.Length > 2)
				{
					array = new string[array3.Length - 2];
					Array.Copy(array3, 2, array, 0, array3.Length - 2);
				}
			}
			return array;
		}

		private void GetTheResponse(string response, object extraArgs)
		{
			ImapTransaction.State state = this.currentState;
			if (state != ImapTransaction.State.SettingUpSecureStreamForTls)
			{
				switch (state)
				{
				default:
					if (!this.imapClient.IsOkResponse(response))
					{
						this.ReportErrorAndQuit(response);
						return;
					}
					break;
				case ImapTransaction.State.LoggingOff:
				case ImapTransaction.State.Disconnecting:
					break;
				}
			}
			switch (this.currentState)
			{
			case ImapTransaction.State.Disconnected:
			case ImapTransaction.State.Disconnecting:
				return;
			case ImapTransaction.State.Connecting:
				if (base.ConnectionType == ProtocolConnectionType.Tls)
				{
					this.currentState = ImapTransaction.State.NegotiatingTls;
					this.imapClient.StartTls(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
					return;
				}
				break;
			case ImapTransaction.State.Connected:
				break;
			case ImapTransaction.State.NegotiatingTls:
				this.currentState = ImapTransaction.State.SettingUpSecureStreamForTls;
				this.imapClient.SetUpSecureStreamForTls(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
				return;
			case ImapTransaction.State.SettingUpSecureStreamForTls:
			case ImapTransaction.State.ReadyToLogIn:
				if (base.LightMode)
				{
					this.ReportSuccessAndLogOff();
					return;
				}
				this.currentState = ImapTransaction.State.LoggingOn;
				base.Verbose(Strings.PopImapLoggingOn);
				if (!string.IsNullOrEmpty(base.User.UserPrincipalName))
				{
					this.imapClient.LogOn(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), base.User.UserPrincipalName, base.Password);
					return;
				}
				this.imapClient.LogOn(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), base.User.SamAccountName, base.Password);
				return;
			case ImapTransaction.State.LoggingOn:
			{
				this.imapClient.HasLoggedIn = true;
				this.currentState = ImapTransaction.State.SelectingFolder;
				string text = "INBOX";
				base.Verbose(Strings.ImapSelectingFolder(text));
				this.imapClient.SelectFolder(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), text);
				return;
			}
			case ImapTransaction.State.SelectingFolder:
				this.currentState = ImapTransaction.State.FindingTestMessage;
				base.Verbose(Strings.PopImapSearchForTestMsg(base.MailSubject));
				this.imapClient.FindMessageIDsBySubject(base.MailSubject, new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
				return;
			case ImapTransaction.State.FindingTestMessage:
			{
				int num = 0;
				string[] array = ImapTransaction.ParseMessageIDs(response);
				if (array != null && 1 == array.Length)
				{
					num = int.Parse(array[0], CultureInfo.InvariantCulture);
				}
				if (num <= 0)
				{
					base.UpdateTransactionResults(CasTransactionResultEnum.Failure, Strings.ErrorMessageNotFound(base.MailSubject));
					this.LogOffAndDisconnectAfterError();
					return;
				}
				this.currentState = ImapTransaction.State.DeletingTestMessage;
				base.Verbose(Strings.PopImapDeleteMsg(num));
				this.DeleteMessage(num);
				return;
			}
			case ImapTransaction.State.DeletingTestMessage:
				this.currentState = ImapTransaction.State.GettingOlderMessages;
				base.Verbose(Strings.PopImapDeleteOldMsgs);
				this.imapClient.FindMessagesBeforeTodayContainingSubject(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), "Test-ImapConnectivity-");
				return;
			case ImapTransaction.State.GettingOlderMessages:
			{
				string[] array2 = ImapTransaction.ParseMessageIDs(response);
				if (array2 != null && array2.Length > 0)
				{
					this.currentState = ImapTransaction.State.DeletingOlderMessages;
					this.DeleteMessages(array2);
					return;
				}
				base.Verbose(Strings.PopImapNoMessagesToDelete);
				goto IL_2C3;
			}
			case ImapTransaction.State.DeletingOlderMessages:
				goto IL_2C3;
			case ImapTransaction.State.LoggingOff:
				this.imapClient.HasLoggedIn = false;
				this.currentState = ImapTransaction.State.Disconnecting;
				this.Disconnect();
				return;
			default:
				throw new ArgumentOutOfRangeException(Strings.PopImapErrorUnexpectedValue(this.currentState.ToString()));
			}
			this.currentState = ImapTransaction.State.ReadyToLogIn;
			this.imapClient.Capability(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
			return;
			IL_2C3:
			this.ReportSuccessAndLogOff();
		}

		private void ExceptionReporter(Exception exception)
		{
			base.UpdateTransactionResults(CasTransactionResultEnum.Failure, exception);
			this.Disconnect();
		}

		private void LogOffAndDisconnectAfterError()
		{
			if (this.imapClient == null)
			{
				this.currentState = ImapTransaction.State.Disconnected;
				base.CompleteTransaction();
				return;
			}
			if (this.imapClient.HasLoggedIn)
			{
				this.LogOff();
				return;
			}
			this.Disconnect();
		}

		private void Disconnect()
		{
			base.Verbose(Strings.PopImapDisconnecting);
			this.imapClient.Dispose();
			this.imapClient = null;
			this.currentState = ImapTransaction.State.Disconnected;
			base.CompleteTransaction();
		}

		private void DeleteMessage(int messageID)
		{
			this.DeleteMessages(new string[]
			{
				messageID.ToString(CultureInfo.InvariantCulture)
			});
		}

		private void DeleteMessages(string[] messageIDs)
		{
			this.imapClient.StoreMessages(new DataCommunicator.GetCommandResponseDelegate(this.AfterStoreCommand), messageIDs);
		}

		private void AfterStoreCommand(string response, object extraArgs)
		{
			if (!this.imapClient.IsOkResponse(response))
			{
				this.ReportErrorAndQuit(response);
				return;
			}
			this.imapClient.ExpungeMessages(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
		}

		private void ReportSuccessAndLogOff()
		{
			base.LatencyTimer.Stop();
			base.UpdateTransactionResults(CasTransactionResultEnum.Success, null);
			this.LogOff();
		}

		private void LogOff()
		{
			this.currentState = ImapTransaction.State.LoggingOff;
			base.Verbose(Strings.PopImapLoggingOff);
			this.imapClient.LogOff(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
		}

		private void ReportErrorAndQuit(string response)
		{
			if (response.Contains("Command received in Invalid state."))
			{
				response = string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", new object[]
				{
					response,
					Strings.CheckServerConfiguration
				});
			}
			base.UpdateTransactionResults(CasTransactionResultEnum.Failure, Strings.ErrorOfProtocolCommand("IMAP", response));
			this.LogOffAndDisconnectAfterError();
		}

		private const string EndOfLine = "\r\n";

		private static readonly char[] space = new char[]
		{
			' '
		};

		private ImapClient imapClient;

		private ImapTransaction.State currentState;

		protected enum State
		{
			Disconnected,
			Connecting,
			Connected,
			NegotiatingTls,
			SettingUpSecureStreamForTls,
			ReadyToLogIn,
			LoggingOn,
			SelectingFolder,
			FindingTestMessage,
			DeletingTestMessage,
			GettingOlderMessages,
			DeletingOlderMessages,
			LoggingOff,
			Disconnecting
		}
	}
}
