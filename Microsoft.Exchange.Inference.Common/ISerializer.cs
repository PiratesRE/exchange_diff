using System;
using System.IO;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface ISerializer<TModelItem> where TModelItem : InferenceBaseModelItem
	{
		void Serialize(Stream stream, TModelItem modelItem);

		TModelItem Deserialize(Stream stream);
	}
}
