using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class PeopleRecipientObject : RecipientObjectResolverRow
	{
		public PeopleRecipientObject(ADRawEntry entry) : base(entry)
		{
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)base.ADRawEntry[ADRecipientSchema.LegacyExchangeDN];
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return (RecipientType)base.ADRawEntry[ADRecipientSchema.RecipientType];
			}
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(RecipientObjectResolverRow.Properties)
		{
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.RecipientType
		}.ToArray();
	}
}
