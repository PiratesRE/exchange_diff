using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class GroupFilter : RecipientFilter
	{
		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return new RecipientTypeDetails[]
				{
					RecipientTypeDetails.MailNonUniversalGroup,
					RecipientTypeDetails.MailUniversalDistributionGroup,
					RecipientTypeDetails.MailUniversalSecurityGroup
				};
			}
		}

		protected override void UpdateFilterProperty()
		{
			base.UpdateFilterProperty();
			string text = (string)base["Filter"];
			string text2 = null;
			if (!string.IsNullOrEmpty(this.AdditionalFilterText))
			{
				text2 = this.AdditionalFilterText;
			}
			if (!string.IsNullOrEmpty(text))
			{
				text2 = string.Format("({0}) -and {1}", text, text2);
			}
			base["Filter"] = text2;
		}

		protected abstract string AdditionalFilterText { get; }
	}
}
