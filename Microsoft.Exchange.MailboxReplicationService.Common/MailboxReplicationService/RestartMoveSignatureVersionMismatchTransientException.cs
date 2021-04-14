using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RestartMoveSignatureVersionMismatchTransientException : MailboxReplicationTransientException
	{
		public RestartMoveSignatureVersionMismatchTransientException(LocalizedString mbxId, uint originalVersion, uint currentVersion) : base(MrsStrings.ReportRestartingMoveBecauseMailboxSignatureVersionIsDifferent(mbxId, originalVersion, currentVersion))
		{
			this.mbxId = mbxId;
			this.originalVersion = originalVersion;
			this.currentVersion = currentVersion;
		}

		public RestartMoveSignatureVersionMismatchTransientException(LocalizedString mbxId, uint originalVersion, uint currentVersion, Exception innerException) : base(MrsStrings.ReportRestartingMoveBecauseMailboxSignatureVersionIsDifferent(mbxId, originalVersion, currentVersion), innerException)
		{
			this.mbxId = mbxId;
			this.originalVersion = originalVersion;
			this.currentVersion = currentVersion;
		}

		protected RestartMoveSignatureVersionMismatchTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbxId = (LocalizedString)info.GetValue("mbxId", typeof(LocalizedString));
			this.originalVersion = (uint)info.GetValue("originalVersion", typeof(uint));
			this.currentVersion = (uint)info.GetValue("currentVersion", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbxId", this.mbxId);
			info.AddValue("originalVersion", this.originalVersion);
			info.AddValue("currentVersion", this.currentVersion);
		}

		public LocalizedString MbxId
		{
			get
			{
				return this.mbxId;
			}
		}

		public uint OriginalVersion
		{
			get
			{
				return this.originalVersion;
			}
		}

		public uint CurrentVersion
		{
			get
			{
				return this.currentVersion;
			}
		}

		private readonly LocalizedString mbxId;

		private readonly uint originalVersion;

		private readonly uint currentVersion;
	}
}
