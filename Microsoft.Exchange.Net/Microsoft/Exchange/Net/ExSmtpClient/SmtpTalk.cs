using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SmtpTalk : DisposeTrackableBase
	{
		internal SmtpTalk(ISmtpClientDebugOutput smtpClientDebugOutput)
		{
			this.smtpClientDebugOutput = smtpClientDebugOutput;
		}

		private Stream TcpStream
		{
			get
			{
				return this.tcpStream;
			}
			set
			{
				this.tcpStream = value;
			}
		}

		private TcpClient TcpClient
		{
			get
			{
				return this.tcpClient;
			}
			set
			{
				this.tcpClient = value;
			}
		}

		private bool Connected
		{
			get
			{
				return this.tcpClient != null && this.TcpClient.Connected;
			}
		}

		public string EhloResponseText
		{
			get
			{
				return this.ehloResponseText;
			}
		}

		internal void Connect(string server, int port)
		{
			base.CheckDisposed();
			if (this.Connected)
			{
				throw new AlreadyConnectedToSMTPServerException(this.serverName);
			}
			this.clientFqdn = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
			this.serverName = server;
			IPHostEntry hostEntry = Dns.GetHostEntry(server);
			IPAddress[] addressList = hostEntry.AddressList;
			IPAddress[] array = addressList;
			int i = 0;
			while (i < array.Length)
			{
				IPAddress ipaddress = array[i];
				IPEndPoint remoteEP = new IPEndPoint(ipaddress, port);
				this.TcpClient = new TcpClient();
				this.TcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
				this.TcpClient.SendTimeout = SmtpConstants.NetworkingTimeout;
				this.TcpClient.ReceiveTimeout = SmtpConstants.NetworkingTimeout;
				try
				{
					this.TcpClient.Connect(remoteEP);
				}
				catch (SocketException)
				{
					this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Could not connect to {0} at {1}::{2}, will try other IP bindings", new object[]
					{
						server,
						ipaddress.ToString(),
						port.ToString(CultureInfo.InvariantCulture)
					});
					goto IL_184;
				}
				goto IL_F2;
				IL_184:
				i++;
				continue;
				IL_F2:
				this.TcpStream = this.TcpClient.GetStream();
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), string.Concat(new string[]
				{
					"Connected to ",
					server,
					" at ",
					ipaddress.ToString(),
					"::",
					port.ToString(CultureInfo.InvariantCulture)
				}), new object[0]);
				SmtpTalk.ServerResponseInfo serverResponse = this.GetServerResponse();
				this.CheckResponse(serverResponse, 220);
				return;
			}
			throw new FailedToConnectToSMTPServerException(server);
		}

		internal void Ehlo()
		{
			base.CheckDisposed();
			SmtpTalk.ServerResponseInfo serverResponseInfo = this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.EhloCommand + this.clientFqdn), SmtpCommandType.Ehlo, 250);
			this.ehloResponseText = serverResponseInfo.StatusString;
			this.advertisedFqdn = serverResponseInfo.StatusString.Substring(4).Split(new char[]
			{
				' '
			})[0];
			if (!SmtpClientDomainUtility.IsValidDomain(this.advertisedFqdn))
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "The SMTP server returned '{0}' as FQDN. This value is invalid so bailing out.", new object[]
				{
					this.advertisedFqdn
				});
				throw new AuthFailureException();
			}
		}

		internal void StartTls(bool useAnonymousTls = true)
		{
			base.CheckDisposed();
			if (useAnonymousTls)
			{
				this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.XAnonymousTlsCommand), SmtpCommandType.XAnonymousTls, 220);
			}
			else
			{
				this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.StartTlsCommand), SmtpCommandType.STARTTLS, 220);
			}
			SmtpSslStream smtpSslStream = new SmtpSslStream(this.TcpClient.GetStream(), this.smtpClientDebugOutput);
			smtpSslStream.Handshake();
			this.TcpStream = smtpSslStream;
		}

		internal void Authenticate(NetworkCredential networkCredential, SmtpSspiMechanism authType = SmtpSspiMechanism.Kerberos)
		{
			base.CheckDisposed();
			SmtpSslStream smtpSslStream = this.TcpStream as SmtpSslStream;
			if (smtpSslStream == null)
			{
				throw new MustBeTlsForAuthException();
			}
			using (SmtpAuth smtpAuth = new SmtpAuth(this.smtpClientDebugOutput))
			{
				if (authType != SmtpSspiMechanism.Login)
				{
					if (authType != SmtpSspiMechanism.Kerberos)
					{
						this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), authType.ToString() + " is not an expected SMTP authentication mechanism", new object[0]);
						throw new UnsupportedAuthMechanismException(authType.ToString());
					}
					string targetSPN = string.Format(CultureInfo.InvariantCulture, SmtpConstants.TargetSpn, new object[]
					{
						this.advertisedFqdn
					});
					string text = smtpAuth.HandleOutboundAuth(null, targetSPN, smtpSslStream.CertificatePublicKey, smtpSslStream.SessionKey, true);
					SmtpTalk.ServerResponseInfo serverResponseInfo = this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.AuthCommand + text), SmtpCommandType.Custom, 235);
					string text2 = ((string)serverResponseInfo.StatusString.Split(new char[]
					{
						' '
					}).GetValue(1)).Trim();
					smtpAuth.HandleOutboundAuth(text2, targetSPN, smtpSslStream.CertificatePublicKey, smtpSslStream.SessionKey, false);
				}
				else
				{
					string text = smtpAuth.GetInitialBlob(networkCredential, authType);
					SmtpTalk.ServerResponseInfo serverResponseInfo = this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.AuthLoginCommand + text), SmtpCommandType.Custom, 334);
					string text2 = ((string)serverResponseInfo.StatusString.Split(new char[]
					{
						' '
					}).GetValue(1)).Trim();
					text = smtpAuth.GetNextBlob(Encoding.Default.GetBytes(text2), networkCredential, SmtpSspiMechanism.Login);
					this.Command(SmtpUtils.ProperlyTerminatedCommand(text), SmtpCommandType.Custom, 235);
				}
			}
		}

		internal void MailFrom(string sender, IEnumerable<KeyValuePair<string, string>> parameters = null)
		{
			base.CheckDisposed();
			StringBuilder stringBuilder = new StringBuilder(SmtpConstants.MailFromCommand + sender);
			if (parameters != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in parameters)
				{
					stringBuilder.AppendFormat(" {0}={1}", keyValuePair.Key, SmtpUtils.ToXtextString(keyValuePair.Value, false));
				}
			}
			string protocolText = SmtpUtils.ProperlyTerminatedCommand(stringBuilder.ToString());
			this.Command(protocolText, SmtpCommandType.Mail, 250);
		}

		internal void RcptTo(string recipient, bool? ndrForFailure = null)
		{
			base.CheckDisposed();
			string str = string.Empty;
			if (ndrForFailure != null)
			{
				str = (ndrForFailure.Value ? SmtpConstants.NDRForFailure : SmtpConstants.NoNDR);
			}
			this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.RcptToCommand + recipient + str), SmtpCommandType.Recipient, 250);
		}

		internal void Chunking(MemoryStream stream)
		{
			base.CheckDisposed();
			string s = string.Format(CultureInfo.InvariantCulture, SmtpConstants.BdatCommand, new object[]
			{
				stream.Length
			});
			SmtpChunk[] array = new SmtpChunk[2];
			array[0].Data = Encoding.Default.GetBytes(s);
			array[0].Length = Encoding.Default.GetByteCount(s);
			array[1].Data = stream.GetBuffer();
			array[1].Length = (int)stream.Length;
			this.Command(array, SmtpCommandType.BDAT, 250);
		}

		internal void Quit()
		{
			base.CheckDisposed();
			this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.QuitCommand), SmtpCommandType.Quit, 221);
		}

		internal void DisableDelayedAck()
		{
			base.CheckDisposed();
			this.Command(SmtpUtils.ProperlyTerminatedCommand(SmtpConstants.XSHADOW + this.clientFqdn), SmtpCommandType.XSHADOW, 250);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Connected)
			{
				this.TcpStream.Flush();
				this.TcpStream.Dispose();
				this.TcpStream = null;
				this.TcpClient.Close();
				this.TcpClient = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SmtpTalk>(this);
		}

		private void Send(byte[] data, int length)
		{
			this.TcpStream.Write(data, 0, length);
		}

		private SmtpTalk.ServerResponseInfo GetServerResponse()
		{
			string currentResponse = string.Empty;
			SmtpTalk.ServerResponseInfo serverResponseInfo;
			do
			{
				serverResponseInfo = this.Receive(currentResponse);
				this.ProcessServerResponse(serverResponseInfo);
				currentResponse = serverResponseInfo.StatusString;
			}
			while (!serverResponseInfo.CommandResponseCompleted);
			return serverResponseInfo;
		}

		private SmtpTalk.ServerResponseInfo Receive(string currentResponse)
		{
			byte[] array = new byte[SmtpConstants.BufferSize];
			StringBuilder stringBuilder = new StringBuilder(currentResponse);
			int count = this.TcpStream.Read(array, 0, array.Length);
			stringBuilder.Append(Encoding.Default.GetString(array, 0, count));
			SmtpTalk.ServerResponseInfo serverResponseInfo = new SmtpTalk.ServerResponseInfo();
			serverResponseInfo.StatusStream = new MemoryStream();
			serverResponseInfo.StatusStream.Write(Encoding.Default.GetBytes(stringBuilder.ToString()), 0, stringBuilder.Length);
			return serverResponseInfo;
		}

		private void ProcessServerResponse(SmtpTalk.ServerResponseInfo response)
		{
			string @string = Encoding.Default.GetString(response.StatusStream.ToArray());
			if (@string.Length == 0)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Received 0 bytes as a response => the connection was closed.", new object[0]);
				response.CommandResponseCompleted = true;
				return;
			}
			if (@string.Length < 5)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "The SMTP server's response (" + @string + ") is too short to be a complete line.  Will read more from the server.", new object[0]);
				response.CommandResponseCompleted = false;
				return;
			}
			response.Status = SmtpUtils.StatusFromResponseString(@string);
			bool flag = @string.Length >= 4 && @string[3] == '-';
			int num = @string.LastIndexOf(SmtpConstants.CrLf, StringComparison.Ordinal);
			if (num != @string.Length - SmtpConstants.CrLf.Length)
			{
				response.CommandResponseCompleted = false;
				return;
			}
			if (flag)
			{
				int i = 0;
				while (i < @string.Length)
				{
					int num2 = @string.IndexOf(SmtpConstants.CrLf, i, StringComparison.Ordinal) - i + SmtpConstants.CrLf.Length;
					if (num2 < 5)
					{
						this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "This line of the SMTP server's multiline response (" + @string.Substring(i, num2) + ") is too short to be a complete line.  Will read more from the server.", new object[0]);
						response.CommandResponseCompleted = false;
						return;
					}
					int num3 = SmtpUtils.StatusFromResponseString(@string.Substring(i));
					if (response.Status != num3)
					{
						this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), string.Concat(new string[]
						{
							"This line of the SMTP server's multiline response (",
							@string.Substring(i, num2),
							") does not begin with the same three-digit status code as the first line(",
							response.Status.ToString(CultureInfo.InvariantCulture),
							"), in violation of the SMTP protocol."
						}), new object[0]);
						throw new InvalidSmtpServerResponseException(@string);
					}
					char c = @string[i + 3];
					bool flag2;
					if (c != ' ')
					{
						if (c != '-')
						{
							this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "The charcter after the status code (" + @string[i + 3].ToString() + ") is neither a ' ' or a '-', in violation of the SMTP protocol.", new object[0]);
							throw new InvalidSmtpServerResponseException(@string);
						}
						flag2 = false;
					}
					else
					{
						flag2 = true;
					}
					i += num2;
					if (-1 == @string.IndexOf(SmtpConstants.CrLf, i, StringComparison.Ordinal))
					{
						if (flag2)
						{
							response.CommandResponseCompleted = true;
						}
						else
						{
							this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "The last line of the multiline response does not contain a ' ' after the status code. The multiline response must not be complete, will read more from the server.", new object[0]);
							response.CommandResponseCompleted = false;
						}
					}
				}
				return;
			}
			response.CommandResponseCompleted = true;
		}

		private void CheckResponse(SmtpTalk.ServerResponseInfo response, int expectedCode)
		{
			if (response.Status != expectedCode)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), string.Format(CultureInfo.InvariantCulture, "The SMTP server returned '{0}' instead of the expected '{1}' status", new object[]
				{
					response.Status,
					expectedCode
				}), new object[0]);
				throw new UnexpectedSmtpServerResponseException(expectedCode, response.Status, response.StatusString);
			}
		}

		private SmtpTalk.ServerResponseInfo Command(string protocolText, SmtpCommandType command, int expectedCode)
		{
			SmtpChunk[] array = new SmtpChunk[1];
			array[0].Data = Encoding.Default.GetBytes(protocolText);
			array[0].Length = Encoding.Default.GetByteCount(protocolText);
			return this.Command(array, command, expectedCode);
		}

		private SmtpTalk.ServerResponseInfo Command(SmtpChunk[] chunks, SmtpCommandType command, int expectedCode)
		{
			this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, this.GetHashCode(), "Processing command: {0}", new object[]
			{
				command
			});
			if (!this.Connected)
			{
				throw new NotConnectedToSMTPServerException(this.serverName);
			}
			foreach (SmtpChunk smtpChunk in chunks)
			{
				this.Send(smtpChunk.Data, smtpChunk.Length);
			}
			SmtpTalk.ServerResponseInfo serverResponse = this.GetServerResponse();
			this.CheckResponse(serverResponse, expectedCode);
			return serverResponse;
		}

		private string clientFqdn = string.Empty;

		private string advertisedFqdn = string.Empty;

		private string serverName = string.Empty;

		private TcpClient tcpClient;

		private Stream tcpStream;

		private ISmtpClientDebugOutput smtpClientDebugOutput;

		private string ehloResponseText = string.Empty;

		[Serializable]
		internal class ServerResponseInfo
		{
			internal int Status
			{
				get
				{
					return this.status;
				}
				set
				{
					this.status = value;
				}
			}

			internal bool CommandResponseCompleted
			{
				get
				{
					return this.commandResponseCompleted;
				}
				set
				{
					this.commandResponseCompleted = value;
				}
			}

			internal MemoryStream StatusStream
			{
				get
				{
					return this.statusStream;
				}
				set
				{
					this.statusStream = value;
				}
			}

			internal string StatusString
			{
				get
				{
					return Encoding.Default.GetString(this.statusStream.ToArray());
				}
			}

			private MemoryStream statusStream = new MemoryStream();

			private int status = -1;

			private bool commandResponseCompleted;
		}
	}
}
