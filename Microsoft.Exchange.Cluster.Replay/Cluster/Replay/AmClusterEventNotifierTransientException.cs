using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmClusterEventNotifierTransientException : AmTransientException
	{
		public AmClusterEventNotifierTransientException(int errCode) : base(ReplayStrings.AmClusterEventNotifierTransientException(errCode))
		{
			this.errCode = errCode;
		}

		public AmClusterEventNotifierTransientException(int errCode, Exception innerException) : base(ReplayStrings.AmClusterEventNotifierTransientException(errCode), innerException)
		{
			this.errCode = errCode;
		}

		protected AmClusterEventNotifierTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errCode = (int)info.GetValue("errCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errCode", this.errCode);
		}

		public int ErrCode
		{
			get
			{
				return this.errCode;
			}
		}

		private readonly int errCode;
	}
}
