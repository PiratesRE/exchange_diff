using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class PopTransaction : ProtocolTransaction
	{
		internal PopTransaction(ADUser user, string serverName, string password, int port) : base(user, serverName, password, port)
		{
			this.messagesToDelete = new Queue<string>();
			this.messageIdQueue = new Queue<string>();
		}

		internal override void Execute()
		{
			this.currentState = PopTransaction.State.Disconnected;
			base.LatencyTimer = Stopwatch.StartNew();
			if (!base.IsServiceRunning("MSExchangePOP3", base.CasServer))
			{
				base.Verbose(Strings.ServiceNotRunning("MSExchangePOP3"));
				base.UpdateTransactionResults(CasTransactionResultEnum.Failure, Strings.ServiceNotRunning("MSExchangePOP3"));
				base.CompleteTransaction();
				return;
			}
			this.popClient = new PopClient(base.CasServer, base.Port, base.ConnectionType, base.TrustAnySslCertificate);
			this.popClient.ExceptionReporter = new DataCommunicator.ExceptionReporterDelegate(this.ExceptionReporter);
			base.Verbose(Strings.PopImapConnecting);
			this.currentState = PopTransaction.State.Connecting;
			this.popClient.Connect(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
		}

		private static Queue<string> PutIDsInQueue(string response)
		{
			string pattern = "\r\n";
			string empty = string.Empty;
			char[] separator = new char[]
			{
				' '
			};
			string[] array = Regex.Split(response, pattern);
			if (array == null || array.Length < 3)
			{
				return new Queue<string>();
			}
			Queue<string> queue = new Queue<string>(array.Length - 2);
			for (int i = 1; i < array.Length - 1; i++)
			{
				string[] array2 = array[i].Split(separator);
				if (array2 != null && array2.Length > 1)
				{
					queue.Enqueue(array2[0]);
				}
			}
			return queue;
		}

		private void GetTheResponse(string response, object extraData)
		{
			switch (this.currentState)
			{
			case PopTransaction.State.SettingUpSecureStreamForTls:
			case PopTransaction.State.GettingMessageData:
			case PopTransaction.State.DeletingOlderMessages:
			case PopTransaction.State.LoggingOff:
			case PopTransaction.State.Disconnecting:
				break;
			default:
				if (!PopClient.IsOkResponse(response))
				{
					this.ReportErrorAndQuit(response);
					return;
				}
				break;
			}
			switch (this.currentState)
			{
			case PopTransaction.State.Disconnected:
			case PopTransaction.State.Disconnecting:
				return;
			case PopTransaction.State.Connecting:
				if (base.ConnectionType == ProtocolConnectionType.Tls)
				{
					this.currentState = PopTransaction.State.NegotiatingTls;
					this.popClient.Stls(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
					return;
				}
				break;
			case PopTransaction.State.Connected:
			case PopTransaction.State.SettingUpSecureStreamForTls:
				break;
			case PopTransaction.State.NegotiatingTls:
				this.currentState = PopTransaction.State.SettingUpSecureStreamForTls;
				this.popClient.SetUpSecureStreamForTls(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
				return;
			case PopTransaction.State.ReadyToLogIn:
				if (base.LightMode)
				{
					this.ReportSuccessAndLogOff();
					return;
				}
				base.Verbose(Strings.PopImapLoggingOn);
				this.currentState = PopTransaction.State.SendingUserCommand;
				if (!string.IsNullOrEmpty(base.User.UserPrincipalName))
				{
					this.popClient.UserCommand(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), base.User.UserPrincipalName);
					return;
				}
				this.popClient.UserCommand(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), base.User.SamAccountName);
				return;
			case PopTransaction.State.SendingUserCommand:
				this.currentState = PopTransaction.State.SendingPassCommand;
				this.popClient.PassCommand(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), base.Password);
				return;
			case PopTransaction.State.SendingPassCommand:
				this.popClient.HasLoggedIn = true;
				this.currentState = PopTransaction.State.GettingMessageIds;
				base.Verbose(Strings.PopGettingMessageIDs);
				this.popClient.List(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
				return;
			case PopTransaction.State.GettingMessageIds:
				this.messageIdQueue = PopTransaction.PutIDsInQueue(response);
				if (this.messageIdQueue.Count == 0)
				{
					throw new ArgumentOutOfRangeException("this.messageIDQueue", Strings.PopImapNoMessagesToDelete);
				}
				this.messagesToDelete.Clear();
				this.currentState = PopTransaction.State.GettingMessageData;
				this.popClient.GetMessage(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), this.messageIdQueue.Peek());
				return;
			case PopTransaction.State.GettingMessageData:
			{
				string item = this.messageIdQueue.Dequeue();
				if (PopClient.IsOkResponse(response))
				{
					bool flag = false;
					string text = ProtocolClient.GetSubjectOfMessage(response).Trim();
					if (string.IsNullOrEmpty(this.testMessageID) && base.MailSubject.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						this.testMessageID = item;
					}
					if (!flag && text.StartsWith("Test-POPConnectivity-", StringComparison.OrdinalIgnoreCase))
					{
						ExDateTime dt = ExDateTime.Parse(ProtocolClient.GetDateOfMessage(response), CultureInfo.InvariantCulture);
						if ((ExDateTime.Now - dt).Days >= 1)
						{
							this.messagesToDelete.Enqueue(item);
						}
					}
				}
				if (this.messageIdQueue.Count > 0)
				{
					this.popClient.GetMessage(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), this.messageIdQueue.Peek());
					return;
				}
				this.currentState = PopTransaction.State.DeletingTestMessage;
				this.DeleteTestMessage();
				return;
			}
			case PopTransaction.State.DeletingTestMessage:
				base.Verbose(Strings.PopImapDeleteOldMsgs);
				if (this.messagesToDelete.Count == 0)
				{
					base.Verbose(Strings.PopImapNoMessagesToDelete);
					this.ReportSuccessAndLogOff();
					return;
				}
				this.currentState = PopTransaction.State.DeletingOlderMessages;
				this.popClient.DeleteMessage(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), this.messagesToDelete.Dequeue());
				return;
			case PopTransaction.State.DeletingOlderMessages:
				if (this.messagesToDelete.Count > 0)
				{
					this.popClient.DeleteMessage(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), this.messagesToDelete.Dequeue());
					return;
				}
				this.ReportSuccessAndLogOff();
				return;
			case PopTransaction.State.LoggingOff:
				this.popClient.HasLoggedIn = false;
				this.currentState = PopTransaction.State.Disconnecting;
				this.Disconnect();
				return;
			default:
				throw new ArgumentOutOfRangeException(Strings.PopImapErrorUnexpectedValue(this.currentState.ToString()));
			}
			this.currentState = PopTransaction.State.ReadyToLogIn;
			this.popClient.Capa(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
		}

		private void ExceptionReporter(Exception exception)
		{
			base.UpdateTransactionResults(CasTransactionResultEnum.Failure, exception);
			this.Disconnect();
		}

		private void LogOffAndDisconnectAfterError()
		{
			if (this.popClient == null)
			{
				this.currentState = PopTransaction.State.Disconnected;
				base.CompleteTransaction();
				return;
			}
			if (this.popClient.HasLoggedIn)
			{
				this.LogOff();
				return;
			}
			this.Disconnect();
		}

		private void Disconnect()
		{
			base.Verbose(Strings.PopImapDisconnecting);
			this.popClient.Dispose();
			this.popClient = null;
			this.currentState = PopTransaction.State.Disconnected;
			base.CompleteTransaction();
		}

		private void DeleteTestMessage()
		{
			this.currentState = PopTransaction.State.DeletingTestMessage;
			if (string.IsNullOrEmpty(this.testMessageID))
			{
				base.UpdateTransactionResults(CasTransactionResultEnum.Failure, Strings.ErrorMessageNotFound(base.MailSubject));
				this.LogOffAndDisconnectAfterError();
				return;
			}
			base.Verbose(Strings.PopImapDeleteMsg(int.Parse(this.testMessageID, CultureInfo.InvariantCulture)));
			this.popClient.DeleteMessage(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse), this.testMessageID);
		}

		private void ReportSuccessAndLogOff()
		{
			base.LatencyTimer.Stop();
			base.UpdateTransactionResults(CasTransactionResultEnum.Success, null);
			this.LogOff();
		}

		private void LogOff()
		{
			this.currentState = PopTransaction.State.LoggingOff;
			base.Verbose(Strings.PopImapLoggingOff);
			this.popClient.LogOff(new DataCommunicator.GetCommandResponseDelegate(this.GetTheResponse));
		}

		private void ReportErrorAndQuit(string response)
		{
			if (response.Contains("Command is not valid in this state."))
			{
				response = string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", new object[]
				{
					response,
					Strings.CheckServerConfiguration
				});
			}
			base.UpdateTransactionResults(CasTransactionResultEnum.Failure, Strings.ErrorOfProtocolCommand("POP", response));
			this.LogOffAndDisconnectAfterError();
		}

		private PopClient popClient;

		private Queue<string> messageIdQueue;

		private Queue<string> messagesToDelete;

		private string testMessageID;

		private PopTransaction.State currentState;

		protected enum State
		{
			Disconnected,
			Connecting,
			Connected,
			NegotiatingTls,
			SettingUpSecureStreamForTls,
			ReadyToLogIn,
			SendingUserCommand,
			SendingPassCommand,
			GettingMessageIds,
			GettingMessageData,
			DeletingTestMessage,
			DeletingOlderMessages,
			LoggingOff,
			Disconnecting
		}
	}
}
