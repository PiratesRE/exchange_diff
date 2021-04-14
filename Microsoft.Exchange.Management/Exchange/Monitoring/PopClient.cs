using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Exchange.Monitoring
{
	internal class PopClient : ProtocolClient
	{
		internal PopClient(string hostName, int portNumber, ProtocolConnectionType connectionMode, bool trustAnySSLCertificate) : base(hostName, portNumber, connectionMode, trustAnySSLCertificate)
		{
		}

		internal static bool IsOkResponse(string response)
		{
			return !string.IsNullOrEmpty(response) && response.StartsWith("+OK", StringComparison.OrdinalIgnoreCase);
		}

		internal void GetMessage(DataCommunicator.GetCommandResponseDelegate callback, string messageNumber)
		{
			string command = string.Format(CultureInfo.InvariantCulture, "RETR {0}\r\n", new object[]
			{
				messageNumber
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void List(DataCommunicator.GetCommandResponseDelegate callback)
		{
			string command = "LIST\r\n";
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void DeleteMessage(DataCommunicator.GetCommandResponseDelegate callback, string messageNumber)
		{
			string command = string.Format(CultureInfo.InvariantCulture, "DELE {0}\r\n", new object[]
			{
				messageNumber
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(base.SingleLineResponseCallback), callback, null);
		}

		internal void UserCommand(DataCommunicator.GetCommandResponseDelegate callback, string userID)
		{
			string command = string.Format(CultureInfo.InvariantCulture, "USER {0}\r\n", new object[]
			{
				userID
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(base.SingleLineResponseCallback), callback, null);
		}

		internal void PassCommand(DataCommunicator.GetCommandResponseDelegate callback, string password)
		{
			string command = string.Format(CultureInfo.InvariantCulture, "PASS {0}\r\n", new object[]
			{
				password
			});
			base.Communicator.SendCommandAsync(command, new AsyncCallback(base.SingleLineResponseCallback), callback, null);
		}

		internal void LogOff(DataCommunicator.GetCommandResponseDelegate callback)
		{
			base.Communicator.SendCommandAsync("QUIT\r\n", new AsyncCallback(base.SingleLineResponseCallback), callback, null);
		}

		internal void Stls(DataCommunicator.GetCommandResponseDelegate callback)
		{
			string command = string.Format(CultureInfo.InvariantCulture, "STLS\r\n", new object[0]);
			base.Communicator.SendCommandAsync(command, new AsyncCallback(base.SingleLineResponseCallback), callback, null);
		}

		internal void Capa(DataCommunicator.GetCommandResponseDelegate callback)
		{
			string command = string.Format(CultureInfo.InvariantCulture, "CAPA\r\n", new object[0]);
			base.Communicator.SendCommandAsync(command, new AsyncCallback(this.MultiLineResponseCallback), callback, null);
		}

		internal void MultiLineResponseCallback(IAsyncResult asyncResult)
		{
			DataCommunicator.State state = (DataCommunicator.State)asyncResult.AsyncState;
			try
			{
				int num = state.DataStream.EndRead(asyncResult);
				if (num > 0)
				{
					state.AppendReceivedData(num);
					if (!PopClient.IsOkResponse(state.Response))
					{
						if (!state.Response.EndsWith("\r\n", StringComparison.Ordinal))
						{
							state.ReadDataCallback = new AsyncCallback(base.SingleLineResponseCallback);
							state.BeginRead();
							return;
						}
					}
					else
					{
						bool flag = false;
						if (!state.Response.EndsWith("\r\n.\r\n", StringComparison.Ordinal))
						{
							flag = true;
						}
						if (flag)
						{
							state.BeginRead();
							return;
						}
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

		internal const string InvalidPopState = "Command is not valid in this state.";

		private const string MultiLineTerminator = "\r\n.\r\n";

		private const string RetriveMessageCommandFormat = "RETR {0}\r\n";

		private const string DeleteMessageCommandFormat = "DELE {0}\r\n";

		private const string QuitCommandFormat = "QUIT\r\n";

		private const string UserCommandFormat = "USER {0}\r\n";

		private const string PassCommandFormat = "PASS {0}\r\n";

		private const string ListCommandFormat = "LIST\r\n";

		private const string CapabilityCommand = "CAPA\r\n";

		private const string TlsCommand = "STLS\r\n";

		private const string OkResponse = "+OK";

		private const string ErrorResponse = "-ERR";
	}
}
