using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxSearchIsInProgressException : LocalizedException
	{
		public MailboxSearchIsInProgressException(string name) : base(Strings.MailboxSearchIsInProgress(name))
		{
			this.name = name;
		}

		public MailboxSearchIsInProgressException(string name, Exception innerException) : base(Strings.MailboxSearchIsInProgress(name), innerException)
		{
			this.name = name;
		}

		protected MailboxSearchIsInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
