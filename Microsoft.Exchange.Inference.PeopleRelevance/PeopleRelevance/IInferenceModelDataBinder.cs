using System;
using Microsoft.Exchange.Inference.Common;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal interface IInferenceModelDataBinder<TInferenceModelItem> where TInferenceModelItem : InferenceBaseModelItem
	{
		TInferenceModelItem GetModelData();

		long SaveModelData(TInferenceModelItem data);
	}
}
