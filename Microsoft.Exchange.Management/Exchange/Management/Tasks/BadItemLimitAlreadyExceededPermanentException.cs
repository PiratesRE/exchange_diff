using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BadItemLimitAlreadyExceededPermanentException : MailboxReplicationPermanentException
	{
		public BadItemLimitAlreadyExceededPermanentException(string name, int encountered, string newlimit) : base(Strings.ErrorBadItemLimitAlreadyExceededNew(name, encountered, newlimit))
		{
			this.name = name;
			this.encountered = encountered;
			this.newlimit = newlimit;
		}

		public BadItemLimitAlreadyExceededPermanentException(string name, int encountered, string newlimit, Exception innerException) : base(Strings.ErrorBadItemLimitAlreadyExceededNew(name, encountered, newlimit), innerException)
		{
			this.name = name;
			this.encountered = encountered;
			this.newlimit = newlimit;
		}

		protected BadItemLimitAlreadyExceededPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.encountered = (int)info.GetValue("encountered", typeof(int));
			this.newlimit = (string)info.GetValue("newlimit", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("encountered", this.encountered);
			info.AddValue("newlimit", this.newlimit);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Encountered
		{
			get
			{
				return this.encountered;
			}
		}

		public string Newlimit
		{
			get
			{
				return this.newlimit;
			}
		}

		private readonly string name;

		private readonly int encountered;

		private readonly string newlimit;
	}
}
