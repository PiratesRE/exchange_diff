using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplicationCheckResultToStringCaseNotHandled : LocalizedException
	{
		public ReplicationCheckResultToStringCaseNotHandled(ReplicationCheckResultEnum result) : base(Strings.ReplicationCheckResultToStringCaseNotHandled(result))
		{
			this.result = result;
		}

		public ReplicationCheckResultToStringCaseNotHandled(ReplicationCheckResultEnum result, Exception innerException) : base(Strings.ReplicationCheckResultToStringCaseNotHandled(result), innerException)
		{
			this.result = result;
		}

		protected ReplicationCheckResultToStringCaseNotHandled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.result = (ReplicationCheckResultEnum)info.GetValue("result", typeof(ReplicationCheckResultEnum));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("result", this.result);
		}

		public ReplicationCheckResultEnum Result
		{
			get
			{
				return this.result;
			}
		}

		private readonly ReplicationCheckResultEnum result;
	}
}
