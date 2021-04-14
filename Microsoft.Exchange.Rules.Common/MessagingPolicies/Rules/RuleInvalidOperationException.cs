using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RuleInvalidOperationException : LocalizedException
	{
		public RuleInvalidOperationException(string details) : base(RulesStrings.RuleInvalidOperationDescription(details))
		{
			this.details = details;
		}

		public RuleInvalidOperationException(string details, Exception innerException) : base(RulesStrings.RuleInvalidOperationDescription(details), innerException)
		{
			this.details = details;
		}

		protected RuleInvalidOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("details", this.details);
		}

		public string Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly string details;
	}
}
