using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.LogUploader
{
	[DataContract]
	public class AuditUploaderConfigSchema : IExtensibleDataObject, IVerifiable
	{
		[DataMember(IsRequired = true)]
		public List<ParsingRule> ParsingSection { get; set; }

		[DataMember(IsRequired = true)]
		public List<FilteringRule> FilteringSection { get; set; }

		ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }

		public virtual void Initialize()
		{
			if (this.ParsingSection != null)
			{
				foreach (ParsingRule parsingRule in this.ParsingSection)
				{
					parsingRule.Initialize();
				}
			}
			if (this.FilteringSection != null)
			{
				foreach (FilteringRule filteringRule in this.FilteringSection)
				{
					filteringRule.Initialize();
				}
			}
		}

		public virtual void Validate()
		{
			if (this.ParsingSection != null)
			{
				foreach (ParsingRule parsingRule in this.ParsingSection)
				{
					parsingRule.Validate();
				}
			}
			if (this.FilteringSection != null)
			{
				foreach (FilteringRule filteringRule in this.FilteringSection)
				{
					filteringRule.Validate();
				}
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}
	}
}
