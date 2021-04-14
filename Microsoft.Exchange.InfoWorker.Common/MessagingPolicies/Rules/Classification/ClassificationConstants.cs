using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Classification
{
	internal static class ClassificationConstants
	{
		public const string ADCollectionName = "ClassificationDefinitions";

		public const string ADPackagePartName = "config";

		public const string VersionElementName = "Version";

		public static readonly List<string> DataClassificationElementNames = new List<string>
		{
			"Entity",
			"Affinity"
		};
	}
}
