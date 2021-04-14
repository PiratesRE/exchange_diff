using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Office.CompliancePolicy.Validators
{
	internal class SharepointValidationResult
	{
		public SharepointSource SharepointSource { get; internal set; }

		public bool IsValid { get; internal set; }

		public bool IsTopLevelSiteCollection { get; internal set; }

		public LocalizedString ValidationText { get; internal set; }
	}
}
