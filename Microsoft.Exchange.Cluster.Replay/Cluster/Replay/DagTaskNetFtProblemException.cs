using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskNetFtProblemException : DagTaskServerException
	{
		public DagTaskNetFtProblemException(int specificErrorCode) : base(ReplayStrings.DagTaskNetFtProblem(specificErrorCode))
		{
			this.specificErrorCode = specificErrorCode;
		}

		public DagTaskNetFtProblemException(int specificErrorCode, Exception innerException) : base(ReplayStrings.DagTaskNetFtProblem(specificErrorCode), innerException)
		{
			this.specificErrorCode = specificErrorCode;
		}

		protected DagTaskNetFtProblemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.specificErrorCode = (int)info.GetValue("specificErrorCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("specificErrorCode", this.specificErrorCode);
		}

		public int SpecificErrorCode
		{
			get
			{
				return this.specificErrorCode;
			}
		}

		private readonly int specificErrorCode;
	}
}
