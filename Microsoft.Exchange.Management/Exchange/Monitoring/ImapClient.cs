using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring
{
	internal class ImapClient : ProtocolClient
	{
		internal ImapClient(string hostName, int portNumber, ProtocolConnectionType connectionMode, bool trustAnySSLCertificate) : base(hostName, portNumber, connectionMode, trustAnySSLCertificate)
		{
		}

		internal void SelectFolder(DataCommunicator.GetCommandResponseDelegate callback, string folder)
		{
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				string.Format(CultureInfo.InvariantCulture, "SELECT \"{0}\"\r\n", new object[]
				{
					folder
				})
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void FindMessagesBeforeTodayContainingSubject(DataCommunicator.GetCommandResponseDelegate callback, string subject)
		{
			this.GenerateCommandTag();
			string text = ExDateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
			string text2 = string.Format(CultureInfo.InvariantCulture, "SEARCH BEFORE \"{0}\" SUBJECT \"{1}\"\r\n", new object[]
			{
				text,
				subject
			});
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				text2
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void FindMessageIDsBySubject(string subjectKey, DataCommunicator.GetCommandResponseDelegate callback)
		{
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				string.Format(CultureInfo.InvariantCulture, "SEARCH SUBJECT \"{0}\"\r\n", new object[]
				{
					subjectKey
				})
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void StoreMessages(DataCommunicator.GetCommandResponseDelegate callback, string[] messageIDs)
		{
			StringBuilder stringBuilder = new StringBuilder(messageIDs[0]);
			for (int i = 1; i < messageIDs.Length; i++)
			{
				stringBuilder.AppendFormat(",{0}", messageIDs[i]);
			}
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				string.Format(CultureInfo.InvariantCulture, "STORE {0} +FLAGS (\\Deleted)\r\n", new object[]
				{
					stringBuilder.ToString()
				})
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void ExpungeMessages(DataCommunicator.GetCommandResponseDelegate callback)
		{
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				"EXPUNGE\r\n"
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void LogOn(DataCommunicator.GetCommandResponseDelegate callback, string userID, string password)
		{
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				string.Format(CultureInfo.InvariantCulture, "LOGIN {0} {1}\r\n", new object[]
				{
					userID,
					password
				})
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(base.SingleLineResponseCallback), callback, null);
		}

		internal void LogOff(DataCommunicator.GetCommandResponseDelegate callback)
		{
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				"LOGOUT\r\n"
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void Capability(DataCommunicator.GetCommandResponseDelegate callback)
		{
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				"CAPABILITY\r\n"
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal bool IsOkResponse(string response)
		{
			if (string.IsNullOrEmpty(response))
			{
				return false;
			}
			string[] array = response.Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			string text = "* ";
			if (array.Length == 1 && array[0].StartsWith(text + "OK", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			int num = -1;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].StartsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return false;
			}
			string value = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				"OK"
			});
			return array[num].StartsWith(value, StringComparison.OrdinalIgnoreCase);
		}

		internal void StartTls(DataCommunicator.GetCommandResponseDelegate callback)
		{
			this.GenerateCommandTag();
			string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.currentTag,
				"STARTTLS\r\n"
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(base.SingleLineResponseCallback), callback, null);
		}

		private string GenerateCommandTag()
		{
			StringBuilder stringBuilder = new StringBuilder("a");
			for (int i = 0; i < 3; i++)
			{
				char value = (char)(65 + this.random.Next(26));
				stringBuilder.Append(value);
			}
			this.currentTag = stringBuilder.ToString();
			return stringBuilder.ToString();
		}

		private void MultiLineResponseCallback(IAsyncResult asyncResult)
		{
			DataCommunicator.State state = (DataCommunicator.State)asyncResult.AsyncState;
			try
			{
				int num = state.DataStream.EndRead(asyncResult);
				if (num > 0)
				{
					state.AppendReceivedData(num);
					if (!this.LastLineReceived(state.Response))
					{
						state.BeginRead();
						return;
					}
				}
			}
			catch (InvalidOperationException exception)
			{
				if (base.Communicator.HasTimedOut)
				{
					base.Communicator.HandleException(DataCommunicator.CreateTimeoutException());
					return;
				}
				base.Communicator.HandleException(exception);
			}
			catch (IOException exception2)
			{
				base.Communicator.HandleException(exception2);
				return;
			}
			base.Communicator.StopTimer();
			state.LaunchResponseDelegate();
		}

		private bool LastLineReceived(string response)
		{
			int num = response.LastIndexOf("\r\n", StringComparison.Ordinal);
			if (num < 0)
			{
				return false;
			}
			if (response.StartsWith("+", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			num = response.LastIndexOf("\r\n", num - 1, StringComparison.Ordinal);
			string text;
			if (num < 0)
			{
				text = response;
			}
			else
			{
				num += 2;
				text = response.Substring(num, response.Length - num);
			}
			return text.StartsWith("+", StringComparison.OrdinalIgnoreCase) || text.StartsWith(this.currentTag, StringComparison.OrdinalIgnoreCase) || text.StartsWith("* BYE", StringComparison.OrdinalIgnoreCase);
		}

		internal const string InvalidImapState = "Command received in Invalid state.";

		protected const string TlsCommandFormat = "STARTTLS\r\n";

		private const string DateFormat = "dd-MMM-yyyy";

		private const string CapabilityCommand = "CAPABILITY\r\n";

		private const string InboxFolder = "INBOX";

		private const string SelectFolderCommandFormat = "SELECT \"{0}\"\r\n";

		private const string SearchSubjectCommandFormat = "SEARCH SUBJECT \"{0}\"\r\n";

		private const string HeaderCommandFormat = "FETCH {0} rfc822.header\r\n";

		private const string SearchDateCommandFormat = "SEARCH BEFORE \"{0}\"\r\n";

		private const string LoginCommandFormat = "LOGIN {0} {1}\r\n";

		private const string LogoutCommandFormat = "LOGOUT\r\n";

		private const string ImapCommandFormat = "{0} {1}";

		private const string SearchSubjectDateCommandFormat = "SEARCH BEFORE \"{0}\" SUBJECT \"{1}\"\r\n";

		private const string MarkForDeletionCommandFormat = "STORE {0} +FLAGS (\\Deleted)\r\n";

		private const string ExpungeCommandFormat = "EXPUNGE\r\n";

		private const string OkResponse = "OK";

		private const string NoResponse = "NO";

		private const string BadResponse = "BAD";

		private const int TagLength = 3;

		private Random random = new Random();

		private string currentTag;
	}
}
