using System;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface IGroupingModelTrainingContext
	{
		int ModelVersion { get; set; }
	}
}
