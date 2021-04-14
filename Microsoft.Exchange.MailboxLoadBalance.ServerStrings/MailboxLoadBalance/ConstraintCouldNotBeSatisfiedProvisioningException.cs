using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConstraintCouldNotBeSatisfiedProvisioningException : MailboxLoadBalancePermanentException
	{
		public ConstraintCouldNotBeSatisfiedProvisioningException(string constraintExpression) : base(MigrationWorkflowServiceStrings.ErrorConstraintCouldNotBeSatisfied(constraintExpression))
		{
			this.constraintExpression = constraintExpression;
		}

		public ConstraintCouldNotBeSatisfiedProvisioningException(string constraintExpression, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorConstraintCouldNotBeSatisfied(constraintExpression), innerException)
		{
			this.constraintExpression = constraintExpression;
		}

		protected ConstraintCouldNotBeSatisfiedProvisioningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.constraintExpression = (string)info.GetValue("constraintExpression", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("constraintExpression", this.constraintExpression);
		}

		public string ConstraintExpression
		{
			get
			{
				return this.constraintExpression;
			}
		}

		private readonly string constraintExpression;
	}
}
