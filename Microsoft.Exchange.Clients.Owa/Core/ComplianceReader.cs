using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ComplianceReader
	{
		public MessageClassificationReader MessageClassificationReader
		{
			get
			{
				return this.messageClassificationReader;
			}
		}

		public RmsTemplateReader RmsTemplateReader
		{
			get
			{
				return this.rmsTemplateReader;
			}
		}

		internal ComplianceReader(OrganizationId organizationId)
		{
			this.messageClassificationReader = new MessageClassificationReader(organizationId);
			this.rmsTemplateReader = new RmsTemplateReader(organizationId);
		}

		public ComplianceType GetComplianceType(Guid guid, CultureInfo locale)
		{
			ClassificationSummary classificationSummary = this.messageClassificationReader.LookupMessageClassification(guid, locale);
			if (classificationSummary != null)
			{
				return ComplianceType.MessageClassification;
			}
			RmsTemplate rmsTemplate = this.rmsTemplateReader.LookupRmsTemplate(guid);
			if (rmsTemplate != null)
			{
				return ComplianceType.RmsTemplate;
			}
			return ComplianceType.Unknown;
		}

		public bool IsComplianceFeatureEnabled(bool isIrmEnabled, bool isIrmProtected, CultureInfo locale)
		{
			return (isIrmEnabled && (this.rmsTemplateReader.IsInternalLicensingEnabled || (isIrmProtected && this.rmsTemplateReader.IsExternalLicensingEnabled))) || this.messageClassificationReader.GetClassificationsForLocale(locale).Count > 0;
		}

		public string GetDescription(Guid guid, CultureInfo locale)
		{
			switch (this.GetComplianceType(guid, locale))
			{
			case ComplianceType.MessageClassification:
				return this.messageClassificationReader.GetDescription(guid, locale, false);
			case ComplianceType.RmsTemplate:
				return this.rmsTemplateReader.GetDescription(guid, locale);
			default:
				return string.Empty;
			}
		}

		internal static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		private MessageClassificationReader messageClassificationReader;

		private RmsTemplateReader rmsTemplateReader;
	}
}
