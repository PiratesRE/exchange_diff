using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaResponseShapeResolver : IResponseShapeResolver
	{
		public OwaResponseShapeResolver()
		{
			this.responseShapesMap = WellKnownShapes.ResponseShapes;
		}

		public T GetResponseShape<T>(string shapeName, T clientResponseShape, IFeaturesManager featuresManager = null) where T : ResponseShape
		{
			if (string.IsNullOrEmpty(shapeName))
			{
				return clientResponseShape;
			}
			WellKnownShapeName key;
			ResponseShape responseShape;
			if (!Enum.TryParse<WellKnownShapeName>(shapeName, out key) || !this.responseShapesMap.TryGetValue(key, out responseShape))
			{
				return clientResponseShape;
			}
			if (clientResponseShape == null)
			{
				return (T)((object)responseShape);
			}
			OwaResponseShapeResolver.AddFlightedProperties(clientResponseShape, responseShape, featuresManager);
			return this.MergeResponseShapes<T>(clientResponseShape, (T)((object)responseShape));
		}

		private static void AddFlightedProperties(ResponseShape clientShape, ResponseShape namedShape, IFeaturesManager featuresManager)
		{
			if (namedShape.FlightedProperties == null || featuresManager == null)
			{
				return;
			}
			HashSet<PropertyPath> hashSet = new HashSet<PropertyPath>();
			foreach (string text in namedShape.FlightedProperties.Keys)
			{
				if (featuresManager.IsFeatureSupported(text))
				{
					hashSet.UnionWith(namedShape.FlightedProperties[text]);
				}
			}
			if (hashSet.Any<PropertyPath>())
			{
				if (clientShape.AdditionalProperties != null)
				{
					hashSet.UnionWith(clientShape.AdditionalProperties);
				}
				clientShape.AdditionalProperties = hashSet.ToArray<PropertyPath>();
			}
		}

		private T MergeResponseShapes<T>(T clientShape, T namedShape) where T : ResponseShape
		{
			PropertyPath[] additionalProperties = clientShape.AdditionalProperties;
			PropertyPath[] additionalProperties2 = namedShape.AdditionalProperties;
			if (additionalProperties == null || additionalProperties.Length == 0)
			{
				clientShape.AdditionalProperties = additionalProperties2;
			}
			else if (additionalProperties2 != null && additionalProperties2.Length > 0)
			{
				List<PropertyPath> list = new List<PropertyPath>(additionalProperties2.Length + additionalProperties.Length);
				list.AddRange(additionalProperties2);
				list.AddRange(additionalProperties);
				clientShape.AdditionalProperties = list.ToArray();
			}
			return clientShape;
		}

		private readonly Dictionary<WellKnownShapeName, ResponseShape> responseShapesMap;
	}
}
