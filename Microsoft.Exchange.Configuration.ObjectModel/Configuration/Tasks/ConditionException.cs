using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class ConditionException : LocalizedException
	{
		public ConditionException(Condition faultingCondition) : base(Strings.ExceptionCondition(faultingCondition.Role.FailureDescription, faultingCondition))
		{
			this.faultingCondition = faultingCondition;
		}

		protected ConditionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.faultingCondition = (Condition)info.GetValue("faultingCondition", typeof(Condition));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("faultingCondition", this.faultingCondition);
		}

		public Condition FaultingCondition
		{
			get
			{
				return this.faultingCondition;
			}
		}

		private Condition faultingCondition;
	}
}
