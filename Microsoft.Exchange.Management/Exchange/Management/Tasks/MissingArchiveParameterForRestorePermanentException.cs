using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MissingArchiveParameterForRestorePermanentException : MailboxReplicationPermanentException
	{
		public MissingArchiveParameterForRestorePermanentException(string identity) : base(Strings.ErrorCloudArchiveNeedsTargetIsArchiveSwitchForRestore(identity))
		{
			this.identity = identity;
		}

		public MissingArchiveParameterForRestorePermanentException(string identity, Exception innerException) : base(Strings.ErrorCloudArchiveNeedsTargetIsArchiveSwitchForRestore(identity), innerException)
		{
			this.identity = identity;
		}

		protected MissingArchiveParameterForRestorePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
