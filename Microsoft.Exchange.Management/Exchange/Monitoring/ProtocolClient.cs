using System;
using System.IO;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class ProtocolClient : IDisposable
	{
		internal ProtocolClient(string hostName, int portNumber, ProtocolConnectionType connectionMode, bool trustAnySSLCertificate)
		{
			this.server = hostName;
			this.connectionType = connectionMode;
			this.trustAnyCertificate = trustAnySSLCertificate;
			this.communicator = new DataCommunicator(hostName, portNumber, connectionMode, trustAnySSLCertificate);
		}

		internal bool TrustAnySSLCertificate
		{
			get
			{
				return this.trustAnyCertificate;
			}
		}

		internal bool HasConnected
		{
			get
			{
				return this.communicator != null && this.communicator.HasConnected;
			}
		}

		internal bool HasLoggedIn
		{
			get
			{
				return this.loggedIn;
			}
			set
			{
				this.loggedIn = value;
			}
		}

		internal ProtocolConnectionType ConnectionType
		{
			get
			{
				return this.connectionType;
			}
		}

		internal string Server
		{
			get
			{
				return this.server;
			}
		}

		internal DataCommunicator.ExceptionReporterDelegate ExceptionReporter
		{
			get
			{
				return this.communicator.ExceptionReporter;
			}
			set
			{
				this.communicator.ExceptionReporter = value;
			}
		}

		protected DataCommunicator Communicator
		{
			get
			{
				return this.communicator;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static string GetSubjectOfMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentNullException("message");
			}
			string result = string.Empty;
			if (message.IndexOf("\r\nSubject:", StringComparison.OrdinalIgnoreCase) > -1)
			{
				message = message.Remove(0, message.IndexOf("\r\nSubject:", StringComparison.OrdinalIgnoreCase) + 10);
				message = message.Remove(message.IndexOf('\r'), message.Length - message.IndexOf('\r'));
				result = message;
			}
			return result;
		}

		internal static string GetDateOfMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentNullException("message");
			}
			string result = string.Empty;
			if (message.IndexOf("\r\nDate:", StringComparison.OrdinalIgnoreCase) > -1)
			{
				message = message.Remove(0, message.IndexOf("\r\nDate:", StringComparison.OrdinalIgnoreCase) + 7);
				message = message.Remove(message.IndexOf('\r'), message.Length - message.IndexOf('\r'));
				result = message;
			}
			return result;
		}

		internal virtual void Connect(DataCommunicator.GetCommandResponseDelegate responseDelegate)
		{
			this.communicator.ConnectAsync(new AsyncCallback(this.SingleLineResponseCallback), responseDelegate, null);
		}

		internal void SetUpSecureStreamForTls(DataCommunicator.GetCommandResponseDelegate callback)
		{
			this.Communicator.InitializeSecureStreamAsync(callback);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.loggedIn = false;
				if (this.communicator != null)
				{
					this.communicator.Close();
					this.communicator.Dispose();
					this.communicator = null;
				}
				this.disposed = true;
			}
		}

		protected void SingleLineResponseCallback(IAsyncResult asyncResult)
		{
			DataCommunicator.State state = (DataCommunicator.State)asyncResult.AsyncState;
			try
			{
				int num = state.DataStream.EndRead(asyncResult);
				if (num > 0)
				{
					state.AppendReceivedData(num);
					if (!state.Response.EndsWith("\r\n", StringComparison.Ordinal))
					{
						state.BeginRead();
					}
				}
				this.Communicator.StopTimer();
				state.LaunchResponseDelegate();
			}
			catch (InvalidOperationException exception)
			{
				if (this.Communicator.HasTimedOut)
				{
					this.Communicator.HandleException(DataCommunicator.CreateTimeoutException());
				}
				else
				{
					this.Communicator.HandleException(exception);
				}
			}
			catch (IOException exception2)
			{
				this.Communicator.HandleException(exception2);
			}
		}

		protected const string EndOfLine = "\r\n";

		private const string DateClause = "\r\nDate:";

		private const string SubjectClause = "\r\nSubject:";

		private const char RChar = '\r';

		private DataCommunicator communicator;

		private readonly string server;

		private readonly bool trustAnyCertificate;

		private ProtocolConnectionType connectionType;

		private bool disposed;

		private bool loggedIn;
	}
}
