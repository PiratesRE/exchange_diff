using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal static class ODataPathExtensions
	{
		public static string GetIdKey(this ODataPathSegment segment)
		{
			ArgumentValidator.ThrowIfNull("segment", segment);
			ArgumentValidator.ThrowIfTypeInvalid<KeySegment>("segment", segment);
			KeySegment keySegment = (KeySegment)segment;
			string key = keySegment.Keys.Single((KeyValuePair<string, object> x) => x.Key.Equals("Id")).Value.ToString();
			return UrlUtilities.TrimKey(key);
		}

		public static string GetSingletonName(this ODataPathSegment segment)
		{
			ArgumentValidator.ThrowIfNull("segment", segment);
			ArgumentValidator.ThrowIfTypeInvalid<SingletonSegment>("segment", segment);
			SingletonSegment singletonSegment = (SingletonSegment)segment;
			return singletonSegment.Singleton.Name;
		}

		public static string GetPropertyName(this ODataPathSegment segment)
		{
			ArgumentValidator.ThrowIfNull("segment", segment);
			ArgumentValidator.ThrowIfTypeInvalid<NavigationPropertySegment>("segment", segment);
			NavigationPropertySegment navigationPropertySegment = (NavigationPropertySegment)segment;
			return navigationPropertySegment.NavigationProperty.Name;
		}

		public static string GetEntitySetName(this ODataPathSegment segment)
		{
			ArgumentValidator.ThrowIfNull("segment", segment);
			ArgumentValidator.ThrowIfTypeInvalid<EntitySetSegment>("segment", segment);
			EntitySetSegment entitySetSegment = (EntitySetSegment)segment;
			return entitySetSegment.EntitySet.Name;
		}

		public static string GetEntitySetOrPropertyName(this ODataPathSegment segment)
		{
			ArgumentValidator.ThrowIfNull("segment", segment);
			if (segment is EntitySetSegment)
			{
				return segment.GetEntitySetName();
			}
			return segment.GetPropertyName();
		}

		public static string GetActionName(this ODataPathSegment segment)
		{
			ArgumentValidator.ThrowIfNull("segment", segment);
			ArgumentValidator.ThrowIfTypeInvalid<OperationSegment>("segment", segment);
			OperationSegment operationSegment = (OperationSegment)segment;
			return operationSegment.Operations.FirstOrDefault<IEdmOperation>().Name;
		}
	}
}
