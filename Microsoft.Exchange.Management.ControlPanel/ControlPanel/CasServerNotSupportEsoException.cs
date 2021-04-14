using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CasServerNotSupportEsoException : LocalizedException
	{
		public CasServerNotSupportEsoException(string userName, string currentSite, string mailboxSite) : base(Strings.CasServerNotSupportEso(userName, currentSite, mailboxSite))
		{
			this.userName = userName;
			this.currentSite = currentSite;
			this.mailboxSite = mailboxSite;
		}

		public CasServerNotSupportEsoException(string userName, string currentSite, string mailboxSite, Exception innerException) : base(Strings.CasServerNotSupportEso(userName, currentSite, mailboxSite), innerException)
		{
			this.userName = userName;
			this.currentSite = currentSite;
			this.mailboxSite = mailboxSite;
		}

		protected CasServerNotSupportEsoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
			this.currentSite = (string)info.GetValue("currentSite", typeof(string));
			this.mailboxSite = (string)info.GetValue("mailboxSite", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
			info.AddValue("currentSite", this.currentSite);
			info.AddValue("mailboxSite", this.mailboxSite);
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

		private readonly string userName;

		private readonly string currentSite;

		private readonly string mailboxSite;
	}
}
