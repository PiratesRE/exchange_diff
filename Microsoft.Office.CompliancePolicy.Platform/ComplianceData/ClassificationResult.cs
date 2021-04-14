using System;

namespace Microsoft.Office.CompliancePolicy.ComplianceData
{
	public sealed class ClassificationResult
	{
		public ClassificationResult(Guid classificationId, int count, int confidence)
		{
			if (classificationId == Guid.Empty)
			{
				throw new ArgumentException(string.Format("classificationId is empty", new object[0]));
			}
			this.ClassificationId = classificationId;
			this.Count = count;
			this.Confidence = confidence;
		}

		public Guid ClassificationId { get; private set; }

		public int Count { get; private set; }

		public int Confidence { get; private set; }
	}
}
