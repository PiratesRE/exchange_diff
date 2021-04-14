using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMServerNotFoundDialPlanException : UMServerNotFoundException
	{
		public UMServerNotFoundDialPlanException(string dialPlan) : base(Strings.UMServerNotFoundDialPlanException(dialPlan))
		{
			this.dialPlan = dialPlan;
		}

		public UMServerNotFoundDialPlanException(string dialPlan, Exception innerException) : base(Strings.UMServerNotFoundDialPlanException(dialPlan), innerException)
		{
			this.dialPlan = dialPlan;
		}

		protected UMServerNotFoundDialPlanException(SerializationInfo info, StreamingContext context) : base(info, context)
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
