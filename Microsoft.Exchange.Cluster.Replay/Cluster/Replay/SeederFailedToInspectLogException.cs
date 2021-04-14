using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederFailedToInspectLogException : SeederServerException
	{
		public SeederFailedToInspectLogException(string logfileName, string inspectionError) : base(ReplayStrings.SeederFailedToInspectLogException(logfileName, inspectionError))
		{
			this.logfileName = logfileName;
			this.inspectionError = inspectionError;
		}

		public SeederFailedToInspectLogException(string logfileName, string inspectionError, Exception innerException) : base(ReplayStrings.SeederFailedToInspectLogException(logfileName, inspectionError), innerException)
		{
			this.logfileName = logfileName;
			this.inspectionError = inspectionError;
		}

		protected SeederFailedToInspectLogException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.logfileName = (string)info.GetValue("logfileName", typeof(string));
			this.inspectionError = (string)info.GetValue("inspectionError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("logfileName", this.logfileName);
			info.AddValue("inspectionError", this.inspectionError);
		}

		public string LogfileName
		{
			get
			{
				return this.logfileName;
			}
		}

		public string InspectionError
		{
			get
			{
				return this.inspectionError;
			}
		}

		private readonly string logfileName;

		private readonly string inspectionError;
	}
}
