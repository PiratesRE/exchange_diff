using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LargeItemLimitAlreadyExceededPermanentException : MailboxReplicationPermanentException
	{
		public LargeItemLimitAlreadyExceededPermanentException(string name, int encountered, string newlimit) : base(Strings.ErrorLargeItemLimitAlreadyExceeded(name, encountered, newlimit))
		{
			this.name = name;
			this.encountered = encountered;
			this.newlimit = newlimit;
		}

		public LargeItemLimitAlreadyExceededPermanentException(string name, int encountered, string newlimit, Exception innerException) : base(Strings.ErrorLargeItemLimitAlreadyExceeded(name, encountered, newlimit), innerException)
		{
			this.name = name;
			this.encountered = encountered;
			this.newlimit = newlimit;
		}

		protected LargeItemLimitAlreadyExceededPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
