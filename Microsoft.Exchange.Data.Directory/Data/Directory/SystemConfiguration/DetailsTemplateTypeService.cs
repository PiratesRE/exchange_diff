using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DetailsTemplateTypeService
	{
		public string TemplateType { get; private set; }

		public MAPIPropertiesDictionary MAPIPropertiesDictionary { get; private set; }

		public DetailsTemplateTypeService(string templateType, MAPIPropertiesDictionary propertiesDictionary)
		{
			this.TemplateType = templateType;
			this.MAPIPropertiesDictionary = propertiesDictionary;
		}
	}
}
