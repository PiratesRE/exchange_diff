using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class SharingMessageInitiator
	{
		[XmlElement]
		public string Name { get; set; }

		[XmlElement]
		public string SmtpAddress { get; set; }

		[XmlElement(IsNullable = false)]
		public string EntryId { get; set; }

		internal ValidationResults Validate()
		{
			if (string.IsNullOrEmpty(this.SmtpAddress))
			{
				return new ValidationResults(ValidationResult.Failure, "SmtpAddress is required");
			}
			if (!Microsoft.Exchange.Data.SmtpAddress.IsValidSmtpAddress(this.SmtpAddress))
			{
				return new ValidationResults(ValidationResult.Failure, "SmtpAddress is not valid value");
			}
			return ValidationResults.Success;
		}
	}
}
