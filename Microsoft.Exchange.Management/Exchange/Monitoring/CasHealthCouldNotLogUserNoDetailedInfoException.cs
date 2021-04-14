using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthCouldNotLogUserNoDetailedInfoException : LocalizedException
	{
		public CasHealthCouldNotLogUserNoDetailedInfoException(string userName, string mailboxServerName, string scriptName) : base(Strings.CasHealthCouldNotLogUserNoDetailedInfo(userName, mailboxServerName, scriptName))
		{
			this.userName = userName;
			this.mailboxServerName = mailboxServerName;
			this.scriptName = scriptName;
		}

		public CasHealthCouldNotLogUserNoDetailedInfoException(string userName, string mailboxServerName, string scriptName, Exception innerException) : base(Strings.CasHealthCouldNotLogUserNoDetailedInfo(userName, mailboxServerName, scriptName), innerException)
		{
			this.userName = userName;
			this.mailboxServerName = mailboxServerName;
			this.scriptName = scriptName;
		}

		protected CasHealthCouldNotLogUserNoDetailedInfoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
			this.mailboxServerName = (string)info.GetValue("mailboxServerName", typeof(string));
			this.scriptName = (string)info.GetValue("scriptName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
			info.AddValue("mailboxServerName", this.mailboxServerName);
			info.AddValue("scriptName", this.scriptName);
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		public string MailboxServerName
		{
			get
			{
				return this.mailboxServerName;
			}
		}

		public string ScriptName
		{
			get
			{
				return this.scriptName;
			}
		}

		private readonly string userName;

		private readonly string mailboxServerName;

		private readonly string scriptName;
	}
}
