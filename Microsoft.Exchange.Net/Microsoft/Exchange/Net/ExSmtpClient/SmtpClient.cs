using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SmtpClient : DisposeTrackableBase
	{
		internal SmtpClient(string host, int port, ISmtpClientDebugOutput smtpClientDebugOutput)
		{
			if (string.IsNullOrEmpty(host))
			{
				throw new ArgumentNullException("host");
			}
			if (smtpClientDebugOutput == null)
			{
				throw new ArgumentNullException("smtpClientDebugOutput");
			}
			this.serverName = host;
			this.serverPort = port;
			this.smtpClientDebugOutput = smtpClientDebugOutput;
			this.smtpTalk = new SmtpTalk(this.smtpClientDebugOutput);
		}

		internal string From
		{
			get
			{
				base.CheckDisposed();
				return this.from;
			}
			set
			{
				base.CheckDisposed();
				this.from = value;
			}
		}

		internal IList<KeyValuePair<string, string>> FromParameters
		{
			get
			{
				IList<KeyValuePair<string, string>> result;
				if ((result = this.fromParameters) == null)
				{
					result = (this.fromParameters = new List<KeyValuePair<string, string>>());
				}
				return result;
			}
		}

		internal string[] To
		{
			get
			{
				base.CheckDisposed();
				return (string[])this.recips.Clone();
			}
			set
			{
				base.CheckDisposed();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.recips = (string[])value.Clone();
			}
		}

		internal bool NDRRequired
		{
			get
			{
				base.CheckDisposed();
				return this.ndrRequired;
			}
			set
			{
				base.CheckDisposed();
				this.ndrRequired = value;
			}
		}

		internal MemoryStream DataStream
		{
			get
			{
				base.CheckDisposed();
				return this.dataStream;
			}
			set
			{
				base.CheckDisposed();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.dataStream = value;
			}
		}

		private SmtpTalk Talk
		{
			get
			{
				return this.smtpTalk;
			}
		}

		internal void AuthCredentials(NetworkCredential credentials)
		{
			base.CheckDisposed();
			this.authCredentials = credentials;
		}

		internal void Submit()
		{
			base.CheckDisposed();
			if (this.dataStream == null)
			{
				throw new ArgumentNullException("DataStream");
			}
			if (this.From == null)
			{
				throw new ArgumentNullException("From");
			}
			if (this.To == null)
			{
				throw new ArgumentNullException("To");
			}
			this.Talk.Connect(this.serverName, this.serverPort);
			this.Talk.Ehlo();
			this.Talk.StartTls(true);
			this.Talk.Ehlo();
			this.Talk.Authenticate(this.authCredentials, SmtpSspiMechanism.Kerberos);
			this.Talk.MailFrom(this.from, this.fromParameters);
			for (int i = 0; i < this.recips.Length; i++)
			{
				this.Talk.RcptTo(this.recips[i], new bool?(this.ndrRequired));
			}
			this.dataStream.Position = 0L;
			this.Talk.Chunking(this.dataStream);
			try
			{
				this.Talk.Quit();
			}
			catch (SocketException ex)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, 0, "Failed to QUIT. '{0}'", new object[]
				{
					ex
				});
			}
			catch (IOException ex2)
			{
				this.smtpClientDebugOutput.Output(ExTraceGlobals.SmtpClientTracer, 0, "Failed to QUIT '{0}'", new object[]
				{
					ex2
				});
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.dataStream != null)
				{
					this.dataStream.Flush();
					this.dataStream.Dispose();
					this.dataStream = null;
				}
				if (this.tempfile != null)
				{
					this.tempfile.Delete();
					this.tempfile = null;
				}
				this.smtpTalk.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SmtpClient>(this);
		}

		private SmtpTalk smtpTalk;

		private string serverName;

		private int serverPort = 25;

		private NetworkCredential authCredentials;

		private string[] recips;

		private string from;

		private bool ndrRequired;

		private MemoryStream dataStream;

		private FileInfo tempfile;

		private ISmtpClientDebugOutput smtpClientDebugOutput;

		private IList<KeyValuePair<string, string>> fromParameters;
	}
}
