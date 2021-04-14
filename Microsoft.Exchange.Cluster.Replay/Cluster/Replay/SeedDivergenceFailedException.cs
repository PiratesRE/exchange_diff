using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeedDivergenceFailedException : SeederServerException
	{
		public SeedDivergenceFailedException(string targetCopyName, string divergenceFileName, string errorMsg) : base(ReplayStrings.SeedDivergenceFailedException(targetCopyName, divergenceFileName, errorMsg))
		{
			this.targetCopyName = targetCopyName;
			this.divergenceFileName = divergenceFileName;
			this.errorMsg = errorMsg;
		}

		public SeedDivergenceFailedException(string targetCopyName, string divergenceFileName, string errorMsg, Exception innerException) : base(ReplayStrings.SeedDivergenceFailedException(targetCopyName, divergenceFileName, errorMsg), innerException)
		{
			this.targetCopyName = targetCopyName;
			this.divergenceFileName = divergenceFileName;
			this.errorMsg = errorMsg;
		}

		protected SeedDivergenceFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.targetCopyName = (string)info.GetValue("targetCopyName", typeof(string));
			this.divergenceFileName = (string)info.GetValue("divergenceFileName", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("targetCopyName", this.targetCopyName);
			info.AddValue("divergenceFileName", this.divergenceFileName);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string TargetCopyName
		{
			get
			{
				return this.targetCopyName;
			}
		}

		public string DivergenceFileName
		{
			get
			{
				return this.divergenceFileName;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string targetCopyName;

		private readonly string divergenceFileName;

		private readonly string errorMsg;
	}
}
