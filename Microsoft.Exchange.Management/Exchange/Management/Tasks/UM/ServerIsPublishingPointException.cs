using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerIsPublishingPointException : LocalizedException
	{
		public ServerIsPublishingPointException(string dialPlan) : base(Strings.ServerIsPublishingPointException(dialPlan))
		{
			this.dialPlan = dialPlan;
		}

		public ServerIsPublishingPointException(string dialPlan, Exception innerException) : base(Strings.ServerIsPublishingPointException(dialPlan), innerException)
		{
			this.dialPlan = dialPlan;
		}

		protected ServerIsPublishingPointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dialPlan = (string)info.GetValue("dialPlan", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dialPlan", this.dialPlan);
		}

		public string DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		private readonly string dialPlan;
	}
}
