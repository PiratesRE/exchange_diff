using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotMovePrimaryAndArchiveToOpposingMrsVersions : MailboxReplicationPermanentException
	{
		public CannotMovePrimaryAndArchiveToOpposingMrsVersions(string srcPrimary, string targetPrimary, string srcArchive, string targetArchive) : base(Strings.ErrorCannotMovePrimaryAndArchiveInDifferentDirections(srcPrimary, targetPrimary, srcArchive, targetArchive))
		{
			this.srcPrimary = srcPrimary;
			this.targetPrimary = targetPrimary;
			this.srcArchive = srcArchive;
			this.targetArchive = targetArchive;
		}

		public CannotMovePrimaryAndArchiveToOpposingMrsVersions(string srcPrimary, string targetPrimary, string srcArchive, string targetArchive, Exception innerException) : base(Strings.ErrorCannotMovePrimaryAndArchiveInDifferentDirections(srcPrimary, targetPrimary, srcArchive, targetArchive), innerException)
		{
			this.srcPrimary = srcPrimary;
			this.targetPrimary = targetPrimary;
			this.srcArchive = srcArchive;
			this.targetArchive = targetArchive;
		}

		protected CannotMovePrimaryAndArchiveToOpposingMrsVersions(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.srcPrimary = (string)info.GetValue("srcPrimary", typeof(string));
			this.targetPrimary = (string)info.GetValue("targetPrimary", typeof(string));
			this.srcArchive = (string)info.GetValue("srcArchive", typeof(string));
			this.targetArchive = (string)info.GetValue("targetArchive", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("srcPrimary", this.srcPrimary);
			info.AddValue("targetPrimary", this.targetPrimary);
			info.AddValue("srcArchive", this.srcArchive);
			info.AddValue("targetArchive", this.targetArchive);
		}

		public string SrcPrimary
		{
			get
			{
				return this.srcPrimary;
			}
		}

		public string TargetPrimary
		{
			get
			{
				return this.targetPrimary;
			}
		}

		public string SrcArchive
		{
			get
			{
				return this.srcArchive;
			}
		}

		public string TargetArchive
		{
			get
			{
				return this.targetArchive;
			}
		}

		private readonly string srcPrimary;

		private readonly string targetPrimary;

		private readonly string srcArchive;

		private readonly string targetArchive;
	}
}
