using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxPlanResolverRow : AdObjectResolverRow
	{
		public MailboxPlanResolverRow(ADRawEntry aDRawEntry) : base(aDRawEntry)
		{
		}

		public override string DisplayName
		{
			get
			{
				string text = (string)base.ADRawEntry[ADRecipientSchema.DisplayName];
				if (string.IsNullOrEmpty(text))
				{
					text = base.DisplayName;
				}
				return text;
			}
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(AdObjectResolverRow.Properties)
		{
			ADRecipientSchema.DisplayName
		}.ToArray();
	}
}
