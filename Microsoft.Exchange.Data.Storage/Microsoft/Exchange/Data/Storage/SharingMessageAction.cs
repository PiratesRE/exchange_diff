using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class SharingMessageAction
	{
		[XmlElement(IsNullable = false)]
		public string Title { get; set; }

		[XmlArrayItem(ElementName = "Provider")]
		[XmlArray]
		public SharingMessageProvider[] Providers { get; set; }

		internal ValidationResults Validate(SharingMessageKind sharingMessageKind)
		{
			if (this.Providers == null || this.Providers.Length == 0)
			{
				return new ValidationResults(ValidationResult.Failure, "There should be at least one provider");
			}
			int num = 0;
			int num2 = 0;
			foreach (SharingMessageProvider sharingMessageProvider in this.Providers)
			{
				ValidationResults validationResults = sharingMessageProvider.Validate(sharingMessageKind);
				switch (validationResults.Result)
				{
				case ValidationResult.Success:
					if (sharingMessageProvider.IsExchangeInternalProvider)
					{
						num++;
						if (num > 1)
						{
							return new ValidationResults(ValidationResult.Failure, "There should be at most one Exchange internal provider");
						}
					}
					else if (sharingMessageProvider.IsExchangeExternalProvider)
					{
						num2++;
						if (num2 > 1)
						{
							return new ValidationResults(ValidationResult.Failure, "There should be at most one Exchange internal provider");
						}
					}
					break;
				case ValidationResult.Failure:
					return validationResults;
				}
			}
			return ValidationResults.Success;
		}
	}
}
