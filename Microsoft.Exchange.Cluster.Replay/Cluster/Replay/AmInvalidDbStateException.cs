using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmInvalidDbStateException : AmCommonException
	{
		public AmInvalidDbStateException(Guid databaseGuid, string stateStr) : base(ReplayStrings.AmInvalidDbState(databaseGuid, stateStr))
		{
			this.databaseGuid = databaseGuid;
			this.stateStr = stateStr;
		}

		public AmInvalidDbStateException(Guid databaseGuid, string stateStr, Exception innerException) : base(ReplayStrings.AmInvalidDbState(databaseGuid, stateStr), innerException)
		{
			this.databaseGuid = databaseGuid;
			this.stateStr = stateStr;
		}

		protected AmInvalidDbStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseGuid = (Guid)info.GetValue("databaseGuid", typeof(Guid));
			this.stateStr = (string)info.GetValue("stateStr", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseGuid", this.databaseGuid);
			info.AddValue("stateStr", this.stateStr);
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public string StateStr
		{
			get
			{
				return this.stateStr;
			}
		}

		private readonly Guid databaseGuid;

		private readonly string stateStr;
	}
}
