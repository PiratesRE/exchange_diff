using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Management.SystemProbeTasks
{
	[Cmdlet("Send", "SystemProbeEmail")]
	public sealed class SendSystemProbeEmail : Task
	{
		[Parameter(Mandatory = false)]
		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Attachments
		{
			get
			{
				return this.attachments.ToArray();
			}
			set
			{
				this.attachments = new List<string>(value);
			}
		}

		[Parameter(Mandatory = true)]
		public string SmtpServer
		{
			get
			{
				return this.smtpServer;
			}
			set
			{
				this.smtpServer = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SmtpUser
		{
			get
			{
				return this.smtpUser;
			}
			set
			{
				this.smtpUser = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SmtpPassword
		{
			get
			{
				return this.smtpPassword;
			}
			set
			{
				this.smtpPassword = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string[] To
		{
			get
			{
				return this.to.ToArray();
			}
			set
			{
				this.to = new List<string>(value);
			}
		}

		[Parameter(Mandatory = false)]
		public string[] CC
		{
			get
			{
				return this.cc.ToArray();
			}
			set
			{
				this.cc = new List<string>(value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool Html
		{
			get
			{
				return this.html;
			}
			set
			{
				this.html = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ProbeGuid
		{
			get
			{
				return this.probeGuid;
			}
			set
			{
				this.probeGuid = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseSsl
		{
			get
			{
				return this.useSsl;
			}
			set
			{
				this.useSsl = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Port
		{
			get
			{
				int? num = this.port;
				if (num == null)
				{
					return 25;
				}
				return num.GetValueOrDefault();
			}
			set
			{
				this.port = new int?(value);
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UseXheader
		{
			get
			{
				return this.useXheader;
			}
			set
			{
				this.useXheader = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TestContext
		{
			get
			{
				return this.testContext;
			}
			set
			{
				this.testContext = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.probeGuid == Guid.Empty)
			{
				this.probeGuid = Guid.NewGuid();
			}
			SystemProbeMailProperties systemProbeMailProperties = new SystemProbeMailProperties();
			systemProbeMailProperties.Guid = this.ProbeGuid;
			try
			{
				SmtpTalk smtpTalk = new SmtpTalk(new SendSystemProbeEmail.SmtpClientDebugOutput(this));
				smtpTalk.Connect(this.smtpServer, this.port ?? 25);
				smtpTalk.Ehlo();
				SmtpResponse ehloResponse;
				if (SmtpResponse.TryParse(smtpTalk.EhloResponseText, out ehloResponse))
				{
					this.ehloOptions.ParseResponse(ehloResponse, new IPAddress(0L));
				}
				if (this.UseSsl)
				{
					smtpTalk.StartTls(true);
					smtpTalk.Ehlo();
				}
				if (!string.IsNullOrEmpty(this.smtpUser))
				{
					smtpTalk.Authenticate(new NetworkCredential(this.smtpUser, this.smtpPassword), SmtpSspiMechanism.Kerberos);
				}
				else if (this.UseSsl)
				{
					smtpTalk.Authenticate(CredentialCache.DefaultNetworkCredentials, SmtpSspiMechanism.Kerberos);
				}
				if (this.ehloOptions.XSysProbe)
				{
					base.WriteVerbose(Strings.SendingGuidInMailFrom);
					smtpTalk.MailFrom(string.Format(CultureInfo.InvariantCulture, "{0} xsysprobeid={1}", new object[]
					{
						this.from,
						this.probeGuid.ToString()
					}), null);
				}
				else
				{
					smtpTalk.MailFrom(this.from, null);
				}
				this.to.ForEach(delegate(string to)
				{
					smtpTalk.RcptTo(to, new bool?(false));
				});
				string text = Path.Combine(Path.GetTempPath(), this.probeGuid.ToString());
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				List<string> list = new List<string>(Directory.GetFiles(text, "*.eml"));
				list.ForEach(delegate(string f)
				{
					File.Delete(f);
				});
				MailMessage message = this.CreateMailMessage();
				using (SmtpClient smtpClient = new SmtpClient(this.smtpServer))
				{
					smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
					smtpClient.PickupDirectoryLocation = text;
					smtpClient.Send(message);
				}
				list = new List<string>(Directory.GetFiles(text, "*.eml"));
				if (list.Count != 1)
				{
					throw new Exception("Unexpected number of files in private pickup folder");
				}
				using (FileStream fileStream = File.OpenRead(list[0]))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						fileStream.CopyTo(memoryStream);
						memoryStream.Position = 0L;
						smtpTalk.Chunking(memoryStream);
					}
				}
				if (this.testContext)
				{
					systemProbeMailProperties.MailMessage = list[0];
				}
				else
				{
					try
					{
						Directory.Delete(text, true);
					}
					catch (IOException ex)
					{
						base.WriteWarning(ex.Message);
					}
				}
				try
				{
					smtpTalk.Quit();
				}
				catch (SocketException)
				{
				}
				catch (IOException)
				{
				}
			}
			catch (MustBeTlsForAuthException ex2)
			{
				base.WriteWarning(ex2.Message);
			}
			catch (UnexpectedSmtpServerResponseException ex3)
			{
				base.WriteWarning(ex3.Message);
			}
			base.WriteObject(systemProbeMailProperties);
		}

		private MailMessage CreateMailMessage()
		{
			MailMessage message = new MailMessage();
			message.From = new MailAddress(this.from);
			message.Subject = this.subject;
			message.Body = this.body;
			message.IsBodyHtml = this.html;
			if (!this.ehloOptions.XSysProbe || this.useXheader)
			{
				base.WriteVerbose(new LocalizedString(string.Format(CultureInfo.CurrentCulture, "Sending system probe guid in {0} header", new object[]
				{
					"X-FFOSystemProbe"
				})));
				string value = SystemProbeId.EncryptProbeGuid(this.probeGuid, DateTime.UtcNow);
				message.Headers.Add("X-FFOSystemProbe", value);
			}
			this.to.ForEach(delegate(string s)
			{
				message.To.Add(s);
			});
			this.cc.ForEach(delegate(string s)
			{
				message.CC.Add(s);
			});
			this.attachments.ForEach(delegate(string s)
			{
				message.Attachments.Add(this.CreateAttachment(s));
			});
			return message;
		}

		private Attachment CreateAttachment(string file)
		{
			Attachment attachment = new Attachment(file, "application/octet-stream");
			ContentDisposition contentDisposition = attachment.ContentDisposition;
			contentDisposition.CreationDate = File.GetCreationTime(file);
			contentDisposition.ModificationDate = File.GetLastWriteTime(file);
			contentDisposition.ReadDate = File.GetLastAccessTime(file);
			return attachment;
		}

		private string subject;

		private string body;

		private List<string> attachments = new List<string>();

		private string smtpServer;

		private string smtpUser;

		private string smtpPassword;

		private string from;

		private List<string> to = new List<string>();

		private List<string> cc = new List<string>();

		private bool html;

		private Guid probeGuid;

		private bool useSsl;

		private int? port;

		private EhloOptions ehloOptions = new EhloOptions();

		private bool useXheader;

		private bool testContext;

		private class SmtpClientDebugOutput : ISmtpClientDebugOutput
		{
			public SmtpClientDebugOutput(PSCmdlet host)
			{
				this.host = host;
			}

			public void Output(Trace tracer, object context, string message, params object[] args)
			{
				tracer.TraceInformation(0, 0L, message, args);
				this.host.WriteVerbose(string.Format(CultureInfo.CurrentCulture, message, args));
			}

			private PSCmdlet host;
		}
	}
}
