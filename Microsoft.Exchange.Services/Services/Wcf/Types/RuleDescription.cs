using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RuleDescription
	{
		public RuleDescription()
		{
		}

		public RuleDescription(RuleDescription ruleDescription)
		{
			this.ActionDescriptions = ruleDescription.ActionDescriptions.ToArray();
			this.ActivationDescription = ruleDescription.ActivationDescription;
			this.ConditionDescriptions = ruleDescription.ConditionDescriptions.ToArray();
			this.ExceptionDescriptions = ruleDescription.ExceptionDescriptions.ToArray();
			this.ExpiryDescription = ruleDescription.ExpiryDescription;
			this.RuleDescriptionActivation = ruleDescription.RuleDescriptionActivation;
			this.RuleDescriptionExceptIf = ruleDescription.RuleDescriptionExceptIf;
			this.RuleDescriptionExpiry = ruleDescription.RuleDescriptionExpiry;
			this.RuleDescriptionIf = ruleDescription.RuleDescriptionIf;
			this.RuleDescriptionTakeActions = ruleDescription.RuleDescriptionTakeActions;
		}

		[DataMember]
		public string[] ActionDescriptions { get; private set; }

		[DataMember]
		public string ActivationDescription { get; private set; }

		[DataMember]
		public string[] ConditionDescriptions { get; private set; }

		[DataMember]
		public string[] ExceptionDescriptions { get; private set; }

		[DataMember]
		public string ExpiryDescription { get; private set; }

		[DataMember]
		public string RuleDescriptionActivation { get; private set; }

		[DataMember]
		public string RuleDescriptionExceptIf { get; private set; }

		[DataMember]
		public string RuleDescriptionExpiry { get; private set; }

		[DataMember]
		public string RuleDescriptionIf { get; private set; }

		[DataMember]
		public string RuleDescriptionTakeActions { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("{ActionDescriptions = ");
			stringBuilder.Append(this.CreateStringList(this.ActionDescriptions));
			stringBuilder.Append(", ActivationDescription = \"");
			stringBuilder.Append(this.ActivationDescription);
			stringBuilder.Append("\", ConditionDescriptions = ");
			stringBuilder.Append(this.CreateStringList(this.ConditionDescriptions));
			stringBuilder.Append(", ExceptionDescriptions = \"");
			stringBuilder.Append(this.CreateStringList(this.ExceptionDescriptions));
			stringBuilder.Append(", ExpiryDescription = \"");
			stringBuilder.Append(this.ExpiryDescription);
			stringBuilder.Append("\", RuleDescriptionActivation = \"");
			stringBuilder.Append(this.RuleDescriptionActivation);
			stringBuilder.Append("\", RuleDescriptionExceptIf = \"");
			stringBuilder.Append(this.RuleDescriptionExceptIf);
			stringBuilder.Append("\", RuleDescriptionExpiry = \"");
			stringBuilder.Append(this.RuleDescriptionExpiry);
			stringBuilder.Append("\", RuleDescriptionIf = \"");
			stringBuilder.Append(this.RuleDescriptionIf);
			stringBuilder.Append("\", RuleDescriptionTakeActions = \"");
			stringBuilder.Append(this.RuleDescriptionTakeActions);
			stringBuilder.Append("\"}");
			return stringBuilder.ToString();
		}

		private string CreateStringList(IEnumerable<string> values)
		{
			if (values == null || !values.Any<string>())
			{
				return "{}";
			}
			return "{" + string.Join(",", from e in values
			select "\"" + (e ?? string.Empty) + "\"") + "}";
		}
	}
}
