using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DomainUsersGroupNotFoundException : LocalizedException
	{
		public DomainUsersGroupNotFoundException(string sid) : base(Strings.DomainUsersGroupNotFoundException(sid))
		{
			this.sid = sid;
		}

		public DomainUsersGroupNotFoundException(string sid, Exception innerException) : base(Strings.DomainUsersGroupNotFoundException(sid), innerException)
		{
			this.sid = sid;
		}

		protected DomainUsersGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sid = (string)info.GetValue("sid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sid", this.sid);
		}

		public string Sid
		{
			get
			{
				return this.sid;
			}
		}

		private readonly string sid;
	}
}
