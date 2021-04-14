using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class Pop3Client : TcpClient, IPop3Client, IDisposable
	{
		public Pop3Client(CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
		}

		public void Connect(string server, int port, string username, string password, bool enableSsl, int readTimeout)
		{
			if (this.isConnected)
			{
				return;
			}
			try
			{
				if (!base.Connected)
				{
					Task task = base.ConnectAsync(server, port);
					task.Wait(this.cancellationToken);
				}
				if (enableSsl)
				{
					SslStream sslStream = new SslStream(base.GetStream(), false);
					Task task2 = sslStream.AuthenticateAsClientAsync(server);
					task2.Wait(this.cancellationToken);
					this.stream = sslStream;
				}
				else
				{
					this.stream = base.GetStream();
				}
				this.stream.ReadTimeout = readTimeout * 1000;
				this.reader = new StreamReader(this.stream, Encoding.ASCII);
				string text = this.Response();
				if (!this.ResponseOk(text))
				{
					throw new Pop3Exception(string.Format("Pop3 server response is not OK. Response: {0}", text), text, "Connect");
				}
			}
			catch (Exception ex)
			{
				if (ex is Pop3Exception)
				{
					throw;
				}
				if (ex is OperationCanceledException)
				{
					throw;
				}
				throw new Pop3Exception(string.Format("Pop3 server connection failure. Server: {0}, Port: {1}, EnableSSL: {2}, Timeout: {3}.", new object[]
				{
					server,
					port,
					enableSsl,
					readTimeout
				}), ex);
			}
			this.USER(username);
			this.PASS(password);
			this.isConnected = true;
		}

		public void Disconnect()
		{
			if (base.Connected)
			{
				try
				{
					this.SendCommand("QUIT");
				}
				finally
				{
					this.isConnected = false;
				}
			}
		}

		public void USER(string username)
		{
			this.SendCommand(string.Format("USER {0}", username));
		}

		public void PASS(string password)
		{
			this.SendCommand(string.Format("{0} {1}", "PASS", password));
		}

		public List<Pop3Message> List()
		{
			this.SendCommand("LIST");
			List<Pop3Message> list = new List<Pop3Message>();
			string text;
			while (this.GetNextResponseLine(out text))
			{
				string[] array = text.Split(new char[]
				{
					' '
				});
				if (array.Length == 2)
				{
					int num;
					int.TryParse(array[0], out num);
					int num2;
					int.TryParse(array[1], out num2);
					if (num != 0 && num2 != 0)
					{
						list.Add(new Pop3Message
						{
							Number = (long)num,
							Size = (long)num2
						});
					}
				}
			}
			return list;
		}

		public void RetrieveHeader(Pop3Message message)
		{
			this.SendCommand(string.Format("TOP {0} 0", message.Number));
			message.HeaderComponents = this.GetNextResponseLines();
		}

		public void Retrieve(Pop3Message message)
		{
			this.SendCommand(string.Format("RETR {0}", message.Number));
			message.Components = this.GetNextResponseLines();
			int num = 0;
			for (int i = 0; i < message.Components.Count; i++)
			{
				if (string.IsNullOrEmpty(message.Components[i]))
				{
					num = i;
					break;
				}
			}
			for (int j = num + 1; j < message.Components.Count; j++)
			{
				message.BodyComponents.Add(message.Components[j]);
			}
		}

		public void Retrieve(List<Pop3Message> messages)
		{
			foreach (Pop3Message message in messages)
			{
				this.Retrieve(message);
			}
		}

		public void Delete(Pop3Message message)
		{
			this.SendCommand(string.Format("DELE {0}", message.Number));
		}

		public new void Dispose()
		{
			this.Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.disposed = true;
				if (this.isConnected)
				{
					this.Disconnect();
				}
				base.Close();
				base.Dispose(true);
				if (this.stream != null)
				{
					this.stream.Dispose();
				}
				if (this.reader != null)
				{
					this.reader.Dispose();
				}
				GC.SuppressFinalize(this);
			}
			this.disposed = true;
		}

		private void SendCommand(string command)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(command + "\r\n");
			Task task = this.stream.WriteAsync(bytes, 0, bytes.Length);
			task.Wait(this.cancellationToken);
			string text = this.Response();
			if (!this.ResponseOk(text))
			{
				string message = string.Format("Pop3 server response is not OK. Command: {0}, Response: {1}", command.Contains("PASS") ? "PASS <Omitted>" : command, text);
				throw new Pop3Exception(message, text, command);
			}
		}

		private string Response()
		{
			Task<string> task = this.reader.ReadLineAsync();
			task.Wait(this.cancellationToken);
			return task.Result;
		}

		private bool ResponseOk(string response)
		{
			return !string.IsNullOrEmpty(response) && response.Length >= 3 && response.Substring(0, 3) == "+OK";
		}

		private bool GetNextResponseLine(out string response)
		{
			response = this.Response();
			if (response == ".")
			{
				return false;
			}
			if (response.StartsWith("."))
			{
				response = response.Substring(1);
			}
			return true;
		}

		private List<string> GetNextResponseLines()
		{
			List<string> list = new List<string>();
			string item;
			while (this.GetNextResponseLine(out item))
			{
				list.Add(item);
			}
			return list;
		}

		private const string CRLF = "\r\n";

		private const string PASSCMD = "PASS";

		private readonly CancellationToken cancellationToken;

		private bool isConnected;

		private Stream stream;

		private StreamReader reader;

		private bool disposed;
	}
}
