using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Mdb;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal static class MdbModelUtils
	{
		internal static TModelItem GetModelItem<TModelItem, TInferenceModelDataBinder>(TInferenceModelDataBinder dataBinder) where TModelItem : InferenceBaseModelItem where TInferenceModelDataBinder : IInferenceModelDataBinder<TModelItem>
		{
			TModelItem tmodelItem = XsoUtil.MapXsoExceptions<TModelItem>(() => dataBinder.GetModelData());
			ExAssert.RetailAssert(tmodelItem != null, "Data binder returned a null Inference model item");
			return tmodelItem;
		}

		internal static void WriteModelItem<TModelItem, TInferenceModelDataBinder>(TInferenceModelDataBinder dataBinder, TModelItem modelItem) where TModelItem : InferenceBaseModelItem where TInferenceModelDataBinder : IInferenceModelDataBinder<TModelItem>
		{
			XsoUtil.MapXsoExceptions(delegate()
			{
				dataBinder.SaveModelData(modelItem);
			});
		}
	}
}
