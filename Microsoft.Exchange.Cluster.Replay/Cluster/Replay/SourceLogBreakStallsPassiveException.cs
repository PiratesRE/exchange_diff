using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceLogBreakStallsPassiveException : TransientException
	{
		public SourceLogBreakStallsPassiveException(string sourceServerName, string error) : base(ReplayStrings.SourceLogBreakStallsPassiveError(sourceServerName, error))
		{
			this.sourceServerName = sourceServerName;
			this.error = error;
		}

		public SourceLogBreakStallsPassiveException(string sourceServerName, string error, Exception innerException) : base(ReplayStrings.SourceLogBreakStallsPassiveError(sourceServerName, error), innerException)
		{
			this.sourceServerName = sourceServerName;
			this.error = error;
		}

		protected SourceLogBreakStallsPassiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sourceServerName = (string)info.GetValue("sourceServerName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sourceServerName", this.sourceServerName);
			info.AddValue("error", this.error);
		}

		public string SourceServerName
		{
			get
			{
				return this.sourceServerName;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string sourceServerName;

		private readonly string error;
	}
}
