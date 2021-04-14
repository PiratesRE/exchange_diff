using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.ExSmtpClient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class SmtpClientHelper
	{
		public static void Submit(SendAction sendAction, string host, int port, NetworkCredential credentials)
		{
			if (string.IsNullOrEmpty(host) || port == 0)
			{
				throw new SmtpServerInfoMissingException();
			}
			using (SmtpTalk smtpTalk = new SmtpTalk(SmtpClientHelper.DebugOutput))
			{
				smtpTalk.Connect(host, port);
				smtpTalk.Ehlo();
				smtpTalk.StartTls(false);
				smtpTalk.Ehlo();
				smtpTalk.Authenticate(credentials, SmtpSspiMechanism.Login);
				smtpTalk.MailFrom("<" + credentials.UserName + ">", null);
				foreach (string str in sendAction.Recipients)
				{
					smtpTalk.RcptTo("<" + str + ">", null);
				}
				byte[] data = sendAction.Data;
				using (MemoryStream memoryStream = new MemoryStream(data.Length))
				{
					memoryStream.Write(data, 0, data.Length);
					memoryStream.Position = 0L;
					smtpTalk.Chunking(memoryStream);
				}
				smtpTalk.Quit();
			}
		}

		private static readonly SmtpClientHelper.SmtpClientDebugOutput DebugOutput = new SmtpClientHelper.SmtpClientDebugOutput();

		private class SmtpClientDebugOutput : ISmtpClientDebugOutput
		{
			void ISmtpClientDebugOutput.Output(Trace tracer, object context, string message, params object[] args)
			{
				if (!string.IsNullOrEmpty(message))
				{
					MrsTracer.Common.Debug(message, args);
				}
			}
		}
	}
}
