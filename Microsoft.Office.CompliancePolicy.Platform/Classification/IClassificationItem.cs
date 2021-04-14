using System;
using System.IO;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	public interface IClassificationItem
	{
		Stream Content { get; }

		string ItemId { get; }

		void SetClassificationResults(ICAClassificationResultCollection results);
	}
}
