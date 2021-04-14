using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ProxyCantFindCasServerException : ProxyException
	{
		public ProxyCantFindCasServerException(string userName, string currentSite, string mailboxSite, string decisionLog) : base(Strings.ProxyCantFindCasServer(userName, currentSite, mailboxSite, decisionLog))
		{
			this.userName = userName;
			this.currentSite = currentSite;
			this.mailboxSite = mailboxSite;
			this.decisionLog = decisionLog;
		}

		public ProxyCantFindCasServerException(string userName, string currentSite, string mailboxSite, string decisionLog, Exception innerException) : base(Strings.ProxyCantFindCasServer(userName, currentSite, mailboxSite, decisionLog), innerException)
		{
			this.userName = userName;
			this.currentSite = currentSite;
			this.mailboxSite = mailboxSite;
			this.decisionLog = decisionLog;
		}

		protected ProxyCantFindCasServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
			this.currentSite = (string)info.GetValue("currentSite", typeof(string));
			this.mailboxSite = (string)info.GetValue("mailboxSite", typeof(string));
			this.decisionLog = (string)info.GetValue("decisionLog", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
			info.AddValue("currentSite", this.currentSite);
			info.AddValue("mailboxSite", this.mailboxSite);
			info.AddValue("decisionLog", this.decisionLog);
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		public string CurrentSite
		{
			get
			{
				return this.currentSite;
			}
		}

		public string MailboxSite
		{
			get
			{
				return this.mailboxSite;
			}
		}

		public string DecisionLog
		{
			get
			{
				return this.decisionLog;
			}
		}

		private readonly string userName;

		private readonly string currentSite;

		private readonly string mailboxSite;

		private readonly string decisionLog;
	}
}
