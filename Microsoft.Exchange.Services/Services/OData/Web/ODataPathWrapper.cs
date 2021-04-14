using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class ODataPathWrapper
	{
		public ODataPathWrapper(ODataPath odataPath)
		{
			ArgumentValidator.ThrowIfNull("odataPath", odataPath);
			this.ODataPath = odataPath;
			this.PathSegments = this.ODataPath.ToList<ODataPathSegment>();
			this.ResolveEntitySet();
		}

		public ODataPath ODataPath { get; private set; }

		public IEdmEntityType EntityType { get; private set; }

		public IEdmNavigationSource NavigationSource { get; private set; }

		public List<ODataPathSegment> PathSegments { get; private set; }

		public ODataPathSegment FirstSegment
		{
			get
			{
				return this.ODataPath.FirstSegment;
			}
		}

		public ODataPathSegment LastSegment
		{
			get
			{
				return this.ODataPath.LastSegment;
			}
		}

		public ODataPathSegment EntitySegment
		{
			get
			{
				return this.PathSegments[this.entitySegmentIndex];
			}
		}

		public ODataPathSegment ParentOfEntitySegment
		{
			get
			{
				return this.PathSegments[this.entitySegmentIndex - 1];
			}
		}

		public ODataPathSegment GrandParentOfEntitySegment
		{
			get
			{
				return this.PathSegments[this.entitySegmentIndex - 2];
			}
		}

		public string ResolveEntityKey()
		{
			if (this.EntitySegment is KeySegment)
			{
				return this.EntitySegment.GetIdKey();
			}
			if (this.EntitySegment is SingletonSegment)
			{
				return this.EntitySegment.GetSingletonName();
			}
			if (this.EntitySegment is NavigationPropertySegment)
			{
				return this.EntitySegment.GetPropertyName();
			}
			throw new InvalidOperationException(string.Format("Unknown ID representation from last segment {0}", this.EntitySegment));
		}

		private void ResolveEntitySet()
		{
			this.entitySegmentIndex = this.PathSegments.Count - 1;
			if (this.EntitySegment is CountSegment)
			{
				this.entitySegmentIndex--;
			}
			this.EntityType = this.ResolveEntityType(this.EntitySegment);
			EntitySetSegment entitySetSegment = this.EntitySegment as EntitySetSegment;
			if (entitySetSegment != null)
			{
				this.NavigationSource = entitySetSegment.EntitySet;
				return;
			}
			SingletonSegment singletonSegment = this.EntitySegment as SingletonSegment;
			if (singletonSegment != null)
			{
				this.NavigationSource = singletonSegment.Singleton;
				return;
			}
			NavigationPropertySegment navigationPropertySegment = this.EntitySegment as NavigationPropertySegment;
			if (navigationPropertySegment != null)
			{
				this.NavigationSource = navigationPropertySegment.NavigationSource;
				return;
			}
			KeySegment keySegment = this.EntitySegment as KeySegment;
			if (keySegment != null)
			{
				this.NavigationSource = keySegment.NavigationSource;
				return;
			}
			OperationSegment operationSegment = this.EntitySegment as OperationSegment;
			if (operationSegment != null)
			{
				this.NavigationSource = operationSegment.EntitySet;
				return;
			}
			throw new NotSupportedException(string.Format("Unexpected ODataPathSegment type {0}", this.EntitySegment));
		}

		private IEdmEntityType ResolveEntityType(ODataPathSegment entitySegment)
		{
			IEdmType edmType = entitySegment.EdmType;
			OperationSegment operationSegment = entitySegment as OperationSegment;
			if (operationSegment != null && edmType == null)
			{
				IEdmOperation edmOperation = operationSegment.Operations.First<IEdmOperation>();
				IEdmOperationParameter edmOperationParameter = edmOperation.Parameters.First<IEdmOperationParameter>();
				edmType = edmOperationParameter.Type.Definition;
			}
			IEdmCollectionType edmCollectionType = edmType as IEdmCollectionType;
			if (edmCollectionType != null)
			{
				edmType = (edmCollectionType.ElementType.Definition as IEdmEntityType);
			}
			IEdmEntityType edmEntityType = edmType as IEdmEntityType;
			if (edmEntityType != null)
			{
				return edmEntityType;
			}
			throw new NotSupportedException(string.Format("Unexpected IEdmType type {0}", edmType));
		}

		private int entitySegmentIndex;
	}
}
