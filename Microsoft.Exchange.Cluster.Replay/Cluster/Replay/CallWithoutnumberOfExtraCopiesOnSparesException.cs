using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CallWithoutnumberOfExtraCopiesOnSparesException : DatabaseCopyLayoutException
	{
		public CallWithoutnumberOfExtraCopiesOnSparesException(string errMsg) : base(ReplayStrings.CallWithoutnumberOfExtraCopiesOnSparesException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public CallWithoutnumberOfExtraCopiesOnSparesException(string errMsg, Exception innerException) : base(ReplayStrings.CallWithoutnumberOfExtraCopiesOnSparesException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected CallWithoutnumberOfExtraCopiesOnSparesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg", this.errMsg);
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string errMsg;
	}
}
