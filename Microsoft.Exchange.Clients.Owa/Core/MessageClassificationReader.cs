using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class MessageClassificationReader
	{
		internal MessageClassificationReader(OrganizationId organizationId)
		{
			this.organizationId = organizationId;
		}

		public List<ClassificationSummary> GetClassificationsForLocale(CultureInfo locale)
		{
			ComplianceReader.ThrowOnNullArgument(locale, "locale");
			List<ClassificationSummary> list = new List<ClassificationSummary>();
			foreach (ClassificationSummary classificationSummary in MessageClassificationReader.classificationConfig.GetClassifications(this.organizationId, locale))
			{
				if (classificationSummary.PermissionMenuVisible)
				{
					list.Add(classificationSummary);
				}
			}
			return list;
		}

		public ClassificationSummary LookupMessageClassification(Guid guid, CultureInfo locale)
		{
			ComplianceReader.ThrowOnNullArgument(locale, "locale");
			if (guid == Guid.Empty)
			{
				return null;
			}
			return MessageClassificationReader.classificationConfig.GetClassification(this.organizationId, guid, locale);
		}

		public string GetDescription(Guid guid, CultureInfo locale, bool checkForPermissionMenuVisible)
		{
			ClassificationSummary classificationSummary = this.LookupMessageClassification(guid, locale);
			if (checkForPermissionMenuVisible && !classificationSummary.PermissionMenuVisible)
			{
				return string.Empty;
			}
			return this.GetDescription(classificationSummary);
		}

		private string GetDescription(ClassificationSummary classification)
		{
			string result = string.Empty;
			if (classification != null)
			{
				if (classification.SenderDescription != null && !string.IsNullOrEmpty(classification.SenderDescription.Trim()))
				{
					result = (string.IsNullOrEmpty(classification.DisplayName) ? string.Empty : classification.DisplayName) + " - " + classification.SenderDescription;
				}
				else if (classification.DisplayName != null && !string.IsNullOrEmpty(classification.DisplayName.Trim()))
				{
					result = classification.DisplayName;
				}
			}
			return result;
		}

		private static readonly ClassificationConfig classificationConfig = new ClassificationConfig();

		private readonly OrganizationId organizationId;
	}
}
