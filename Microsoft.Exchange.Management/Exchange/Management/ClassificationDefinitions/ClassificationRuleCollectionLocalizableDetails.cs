using System;
using System.Globalization;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	internal class ClassificationRuleCollectionLocalizableDetails
	{
		internal string PublisherName { get; set; }

		internal string Name { get; set; }

		internal string Description { get; set; }

		internal CultureInfo Culture { get; set; }
	}
}
