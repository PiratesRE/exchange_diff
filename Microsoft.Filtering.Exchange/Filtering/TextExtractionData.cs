using System;

namespace Microsoft.Filtering
{
	internal struct TextExtractionData
	{
		public int StreamId;

		public long StreamSize;

		public int ParentId;

		public string Types;

		public string ModuleUsed;

		public string SkippedModules;

		public string FailedModules;

		public string DisabledModules;

		public int TextExtractionResult;

		public string AdditionalInformation;
	}
}
