using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IResponseShapeResolver
	{
		T GetResponseShape<T>(string shapeName, T clientResponseShape, IFeaturesManager featuresManager = null) where T : ResponseShape;
	}
}
