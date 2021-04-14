using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UsersNotInSameOrganizationPermanentException : MailboxReplicationPermanentException
	{
		public UsersNotInSameOrganizationPermanentException(string src, string tgt) : base(Strings.ErrorUsersNotInSameOrganization(src, tgt))
		{
			this.src = src;
			this.tgt = tgt;
		}

		public UsersNotInSameOrganizationPermanentException(string src, string tgt, Exception innerException) : base(Strings.ErrorUsersNotInSameOrganization(src, tgt), innerException)
		{
			this.src = src;
			this.tgt = tgt;
		}

		protected UsersNotInSameOrganizationPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.src = (string)info.GetValue("src", typeof(string));
			this.tgt = (string)info.GetValue("tgt", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("src", this.src);
			info.AddValue("tgt", this.tgt);
		}

		public string Src
		{
			get
			{
				return this.src;
			}
		}

		public string Tgt
		{
			get
			{
				return this.tgt;
			}
		}

		private readonly string src;

		private readonly string tgt;
	}
}
