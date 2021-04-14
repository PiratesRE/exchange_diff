using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthCouldNotLogUserException : LocalizedException
	{
		public CasHealthCouldNotLogUserException(string userName, string mailboxServerName, string scriptName, string errorString) : base(Strings.CasHealthCouldNotLogUser(userName, mailboxServerName, scriptName, errorString))
		{
			this.userName = userName;
			this.mailboxServerName = mailboxServerName;
			this.scriptName = scriptName;
			this.errorString = errorString;
		}

		public CasHealthCouldNotLogUserException(string userName, string mailboxServerName, string scriptName, string errorString, Exception innerException) : base(Strings.CasHealthCouldNotLogUser(userName, mailboxServerName, scriptName, errorString), innerException)
		{
			this.userName = userName;
			this.mailboxServerName = mailboxServerName;
			this.scriptName = scriptName;
			this.errorString = errorString;
		}

		protected CasHealthCouldNotLogUserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
			this.mailboxServerName = (string)info.GetValue("mailboxServerName", typeof(string));
			this.scriptName = (string)info.GetValue("scriptName", typeof(string));
			this.errorString = (string)info.GetValue("errorString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
			info.AddValue("mailboxServerName", this.mailboxServerName);
			info.AddValue("scriptName", this.scriptName);
			info.AddValue("errorString", this.errorString);
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

		public string ErrorString
		{
			get
			{
				return this.errorString;
			}
		}

		private readonly string userName;

		private readonly string mailboxServerName;

		private readonly string scriptName;

		private readonly string errorString;
	}
}
