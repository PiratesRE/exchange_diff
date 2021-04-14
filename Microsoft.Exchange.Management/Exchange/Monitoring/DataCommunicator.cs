using System;
using System.Globalization;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class DataCommunicator : IDisposable
	{
		internal DataCommunicator(string hostName, int portNumber, ProtocolConnectionType connectionMode, bool trustAnySSLCertificate)
		{
			if (string.IsNullOrEmpty(hostName))
			{
				throw new ArgumentNullException("hostName");
			}
			if (portNumber <= 0)
			{
				throw new ArgumentNullException("portNumber");
			}
			this.serverName = hostName;
			this.port = portNumber;
			this.connectionType = connectionMode;
			this.trustAnyCertificate = trustAnySSLCertificate;
		}

		internal bool HasConnected
		{
			get
			{
				return this.tcpClient != null && this.tcpClient.Connected;
			}
		}

		internal bool HasTimedOut
		{
			get
			{
				return this.hasTimedOut;
			}
		}

		internal Stream DataStream
		{
			get
			{
				return this.dataStream;
			}
		}

		internal DataCommunicator.ExceptionReporterDelegate ExceptionReporter
		{
			get
			{
				return this.exceptionReporter;
			}
			set
			{
				this.exceptionReporter = value;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static TimeoutException CreateTimeoutException()
		{
			return new TimeoutException(Strings.PopImapErrorIOTimeout(60));
		}

		internal void ConnectAsync(AsyncCallback readCallback, DataCommunicator.GetCommandResponseDelegate responseDelegate, object callerArguments)
		{
			DataCommunicator.State state = new DataCommunicator.State();
			state.ResponseDelegate = responseDelegate;
			state.ReadDataCallback = readCallback;
			state.CallerArguments = callerArguments;
			try
			{
				this.tcpClient = new TcpClient();
			}
			catch (SocketException exception)
			{
				this.HandleException(exception);
			}
			this.StartTimer();
			this.tcpClient.BeginConnect(this.serverName, this.port, new AsyncCallback(this.ConnectAsyncCallback), state);
		}

		internal void SendCommandAsync(string command, AsyncCallback readCallback, DataCommunicator.GetCommandResponseDelegate responseDelegate, object callerArguments)
		{
			if (this.dataStream == null)
			{
				return;
			}
			DataCommunicator.State state = new DataCommunicator.State();
			state.ReadDataCallback = readCallback;
			state.ResponseDelegate = responseDelegate;
			state.CallerArguments = callerArguments;
			state.DataStream = this.dataStream;
			try
			{
				this.StartTimer();
				byte[] bytes = Encoding.ASCII.GetBytes(command);
				this.dataStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.SendDataCallback), state);
			}
			catch (IOException exception)
			{
				this.HandleException(exception);
			}
			catch (InvalidOperationException exception2)
			{
				this.HandleException(exception2);
			}
		}

		internal void Close()
		{
			this.StopTimer();
			this.CloseStream();
			if (this.tcpClient != null)
			{
				this.tcpClient.Close();
				this.tcpClient = null;
			}
		}

		internal void InitializeSecureStreamAsync(DataCommunicator.GetCommandResponseDelegate responseDelegate)
		{
			if (this.dataStream == null)
			{
				return;
			}
			this.InitializeSecureStreamAsync(new DataCommunicator.State
			{
				ResponseDelegate = responseDelegate,
				DataStream = this.DataStream
			});
		}

		internal void ReadResponseAsync(AsyncCallback readCallback, DataCommunicator.GetCommandResponseDelegate responseDelegate, object callerArguments)
		{
			if (this.dataStream == null)
			{
				return;
			}
			DataCommunicator.State state = new DataCommunicator.State();
			state.ReadDataCallback = readCallback;
			state.ResponseDelegate = responseDelegate;
			state.CallerArguments = callerArguments;
			state.DataStream = this.dataStream;
			this.StartTimer();
			this.ReadResponseAsync(state);
		}

		internal void StartTimer()
		{
			this.StopTimer();
			this.timeOutTimer = new Timer(new TimerCallback(this.TimeoutEventHandler), null, 60000, -1);
		}

		internal void StopTimer()
		{
			if (this.timeOutTimer != null)
			{
				this.timeOutTimer.Dispose();
				this.timeOutTimer = null;
			}
		}

		internal void HandleException(Exception exception)
		{
			this.Close();
			if (this.ExceptionReporter == null)
			{
				throw new NullReferenceException("Handled exception reporter not found.", exception);
			}
			if (this.hasTimedOut)
			{
				this.exceptionReporter(DataCommunicator.CreateTimeoutException());
				return;
			}
			this.exceptionReporter(exception);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.Close();
			}
			this.disposed = true;
		}

		private void ConnectAsyncCallback(IAsyncResult asyncResult)
		{
			try
			{
				this.tcpClient.EndConnect(asyncResult);
				DataCommunicator.State state = (DataCommunicator.State)asyncResult.AsyncState;
				this.dataStream = this.tcpClient.GetStream();
				state.DataStream = this.dataStream;
				if (this.connectionType == ProtocolConnectionType.Ssl)
				{
					this.InitializeSecureStreamAsync(state);
				}
				else
				{
					this.ReadResponseAsync(state);
				}
			}
			catch (InvalidOperationException exception)
			{
				this.HandleException(exception);
			}
			catch (SocketException exception2)
			{
				this.HandleException(exception2);
			}
			catch (IOException exception3)
			{
				this.HandleException(exception3);
			}
		}

		private void SendDataCallback(IAsyncResult asyncResult)
		{
			DataCommunicator.State state = (DataCommunicator.State)asyncResult.AsyncState;
			try
			{
				state.DataStream.EndWrite(asyncResult);
				if (this.dataStream != null)
				{
					this.ReadResponseAsync(state);
				}
			}
			catch (InvalidOperationException exception)
			{
				this.HandleException(exception);
			}
			catch (IOException exception2)
			{
				this.HandleException(exception2);
			}
		}

		private void ReadResponseAsync(DataCommunicator.State state)
		{
			try
			{
				state.BeginRead();
			}
			catch (InvalidOperationException exception)
			{
				this.HandleException(exception);
			}
			catch (IOException exception2)
			{
				this.HandleException(exception2);
			}
		}

		private void InitializeSecureStreamAsync(DataCommunicator.State state)
		{
			if (this.dataStream == null)
			{
				return;
			}
			try
			{
				SslStream sslStream = new SslStream(this.dataStream, false, new RemoteCertificateValidationCallback(this.ValidateServerCertificate), null);
				state.DataStream = sslStream;
				this.dataStream = sslStream;
				this.StartTimer();
				sslStream.BeginAuthenticateAsClient(this.serverName, new AsyncCallback(this.InitializeSecureStreamAsyncCallback), state);
			}
			catch (InvalidOperationException exception)
			{
				this.HandleException(exception);
			}
			catch (IOException ex)
			{
				this.HandleException(new ProtocolException(Strings.ErrorAuthenticationFailed(ex.Message) + Strings.InitializeServerResponse(string.IsNullOrEmpty(state.Response) ? string.Empty : state.Response), ex));
			}
			catch (AuthenticationException innerException)
			{
				this.HandleException(new ProtocolException(Strings.ErrorAuthenticationFailed(this.certificateError) + Strings.InitializeServerResponse(string.IsNullOrEmpty(state.Response) ? string.Empty : state.Response), innerException));
			}
		}

		private void InitializeSecureStreamAsyncCallback(IAsyncResult asyncResult)
		{
			DataCommunicator.State state = (DataCommunicator.State)asyncResult.AsyncState;
			try
			{
				SslStream sslStream = (SslStream)state.DataStream;
				sslStream.EndAuthenticateAsClient(asyncResult);
				if (this.connectionType == ProtocolConnectionType.Ssl)
				{
					state.BeginRead();
				}
				else
				{
					state.LaunchResponseDelegate();
				}
			}
			catch (IOException ex)
			{
				this.HandleException(new ProtocolException(Strings.ErrorAuthenticationFailed(ex.Message) + Strings.InitializeServerResponse(string.IsNullOrEmpty(state.Response) ? string.Empty : state.Response), ex));
			}
			catch (InvalidOperationException innerException)
			{
				this.HandleException(new ProtocolException(Strings.ErrorAuthentication + Strings.InitializeServerResponse(string.IsNullOrEmpty(state.Response) ? string.Empty : state.Response), innerException));
			}
			catch (AuthenticationException innerException2)
			{
				this.HandleException(new ProtocolException(Strings.ErrorAuthenticationFailed(this.certificateError) + Strings.InitializeServerResponse(string.IsNullOrEmpty(state.Response) ? string.Empty : state.Response), innerException2));
			}
		}

		private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None || this.trustAnyCertificate)
			{
				return true;
			}
			if (certificate != null)
			{
				this.certificateError = string.Format(CultureInfo.InvariantCulture, " {0}\r\n", new object[]
				{
					certificate.Subject
				});
			}
			this.certificateError = string.Format(CultureInfo.InvariantCulture, "Name:{0} SslPolicyErrors:{1}", new object[]
			{
				this.certificateError,
				sslPolicyErrors.ToString()
			});
			if (chain != null)
			{
				foreach (X509ChainStatus x509ChainStatus in chain.ChainStatus)
				{
					this.certificateError = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4}", new object[]
					{
						this.certificateError,
						Environment.NewLine,
						x509ChainStatus.Status.ToString(),
						Environment.NewLine,
						x509ChainStatus.StatusInformation
					});
				}
			}
			return false;
		}

		private void TimeoutEventHandler(object stateInfo)
		{
			this.StopTimer();
			this.hasTimedOut = true;
			this.CloseStream();
		}

		private void CloseStream()
		{
			if (this.dataStream != null)
			{
				this.dataStream.Close();
				this.dataStream = null;
			}
		}

		private const string EndOfLine = "\r\n";

		private const int READTIMEOUT = 60000;

		private TcpClient tcpClient;

		private ProtocolConnectionType connectionType;

		private readonly string serverName;

		private readonly int port;

		private readonly bool trustAnyCertificate;

		private bool disposed;

		private Stream dataStream;

		private string certificateError;

		private Timer timeOutTimer;

		private bool hasTimedOut;

		private DataCommunicator.ExceptionReporterDelegate exceptionReporter;

		internal delegate void GetCommandResponseDelegate(string response, object callerArguments);

		internal delegate void ExceptionReporterDelegate(Exception exception);

		internal class State
		{
			internal State()
			{
				this.response = new StringBuilder(1024);
				this.buffer = new byte[1024];
			}

			internal Stream DataStream
			{
				get
				{
					return this.stream;
				}
				set
				{
					this.stream = value;
				}
			}

			internal string Response
			{
				get
				{
					return this.response.ToString();
				}
				set
				{
					this.response = new StringBuilder(value);
				}
			}

			internal byte[] Buffer
			{
				get
				{
					return this.buffer;
				}
			}

			internal DataCommunicator.GetCommandResponseDelegate ResponseDelegate
			{
				get
				{
					return this.responseDelegate;
				}
				set
				{
					this.responseDelegate = value;
				}
			}

			internal object CallerArguments
			{
				get
				{
					return this.callerArguments;
				}
				set
				{
					this.callerArguments = value;
				}
			}

			internal AsyncCallback ReadDataCallback
			{
				get
				{
					return this.readCallback;
				}
				set
				{
					this.readCallback = value;
				}
			}

			internal void AppendReceivedData(int length)
			{
				if (length != 0)
				{
					this.response.Append(Encoding.ASCII.GetString(this.buffer, 0, length));
				}
			}

			internal void LaunchResponseDelegate()
			{
				this.responseDelegate(this.response.ToString(), this.callerArguments);
			}

			internal void BeginRead()
			{
				this.stream.BeginRead(this.buffer, 0, 1024, this.readCallback, this);
			}

			internal const int BUFFSIZE = 1024;

			private Stream stream;

			private StringBuilder response;

			private byte[] buffer;

			private DataCommunicator.GetCommandResponseDelegate responseDelegate;

			private object callerArguments;

			private AsyncCallback readCallback;
		}
	}
}
