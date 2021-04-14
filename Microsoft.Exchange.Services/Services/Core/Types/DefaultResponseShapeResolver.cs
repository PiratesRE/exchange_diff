using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class DefaultResponseShapeResolver : IResponseShapeResolver
	{
		public T GetResponseShape<T>(string shapeName, T clientResponseShape, IFeaturesManager featuresManager = null) where T : ResponseShape
		{
			return clientResponseShape;
		}
	}
}
