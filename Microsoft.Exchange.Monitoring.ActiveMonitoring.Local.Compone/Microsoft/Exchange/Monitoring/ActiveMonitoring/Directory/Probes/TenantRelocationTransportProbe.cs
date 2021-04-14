using System;
using System.Net.Mail;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class TenantRelocationTransportProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			string subject = "MonitoringMailSubject" + Guid.NewGuid().ToString();
			this.SendMail(subject, cancellationToken);
			this.CheckMail(subject, cancellationToken);
		}

		private void SendMail(string subject, CancellationToken token)
		{
			string text = base.Definition.Attributes["SmtpServers"];
			string[] array = text.Split(new char[]
			{
				';'
			});
			string value = base.Definition.Attributes["SmtpPort"];
			int port = Convert.ToInt32(value);
			string from = base.Definition.Attributes["SenderEmailAddress"];
			string value2 = base.Definition.Attributes["SendMessageTimeout"];
			int num = num = Convert.ToInt32(value2);
			bool flag = false;
			int num2 = 0;
			Exception innerException = null;
			while (!flag && num2 < num * 2)
			{
				SmtpClient smtpClient = null;
				try
				{
					string host = array[TenantRelocationTransportProbe.random.Next() % array.Length];
					smtpClient = new SmtpClient(host);
					smtpClient.Port = port;
					smtpClient.Send(new MailMessage(from, base.Definition.Account, subject, string.Empty)
					{
						Priority = MailPriority.Normal
					});
					flag = true;
				}
				catch (Exception ex)
				{
					num2++;
					innerException = ex;
					this.Sleep(TimeSpan.FromMilliseconds(500.0), token);
				}
				finally
				{
					if (smtpClient != null)
					{
						smtpClient.Dispose();
					}
				}
			}
			if (!flag)
			{
				throw new Exception("Failed to send test messsage after retry 5 times", innerException);
			}
		}

		private void CheckMail(string subject, CancellationToken token)
		{
			string value = base.Definition.Attributes["WaitMessageTimeout"];
			int num = Convert.ToInt32(value);
			using (MailboxSession mailboxSession = SearchStoreHelper.GetMailboxSession(base.Definition.Account, true, "Monitoring"))
			{
				VersionedId versionedId = null;
				using (Folder inboxFolder = SearchStoreHelper.GetInboxFolder(mailboxSession))
				{
					for (int i = 0; i <= num; i++)
					{
						ExDateTime exDateTime;
						versionedId = SearchStoreHelper.GetMessageBySubject(inboxFolder, subject, out exDateTime);
						if (versionedId != null)
						{
							base.Result.StateAttribute1 = "Email found at " + exDateTime;
							break;
						}
						this.Sleep(TimeSpan.FromMilliseconds(1000.0), token);
					}
				}
				if (versionedId == null)
				{
					throw new Exception("Message not found");
				}
			}
		}

		private void Sleep(TimeSpan duration, CancellationToken cancellationToken)
		{
			if (cancellationToken.WaitHandle != null)
			{
				cancellationToken.WaitHandle.WaitOne(duration);
				cancellationToken.ThrowIfCancellationRequested();
				return;
			}
			Thread.Sleep(duration);
		}

		private static Random random = new Random();
	}
}
