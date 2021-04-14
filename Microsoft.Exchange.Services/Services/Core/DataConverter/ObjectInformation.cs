using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ObjectInformation : XmlElementInformation
	{
		internal ObjectInformation(string localName, ExchangeVersion effectiveVersion, Type associatedType, StoreObjectType[] associatedStoreObjectTypes, Shape.CreateShapeCallback createShape, Shape.CreateShapeForPropertyBagCallback createShapeForPropertyBag, Shape.CreateShapeForStoreObjectCallback createShapeForStoreObject, ObjectInformation priorVersionObjectInformation) : base(localName, string.Empty, effectiveVersion)
		{
			this.associatedType = associatedType;
			this.associatedStoreObjectTypes = associatedStoreObjectTypes;
			this.createShape = createShape;
			this.createShapeForPropertyBag = createShapeForPropertyBag;
			this.createShapeForStoreObject = createShapeForStoreObject;
			this.priorVersionObjectInformation = priorVersionObjectInformation;
		}

		internal ObjectInformation(string localName, ExchangeVersion effectiveVersion, Type associatedType, StoreObjectType[] associatedStoreObjectTypes, Shape.CreateShapeCallback createShape, ObjectInformation priorVersionObjectInformation) : this(localName, effectiveVersion, associatedType, associatedStoreObjectTypes, createShape, null, null, priorVersionObjectInformation)
		{
		}

		internal Type AssociatedType
		{
			get
			{
				return this.associatedType;
			}
		}

		internal StoreObjectType[] AssociatedStoreObjectTypes
		{
			get
			{
				return this.associatedStoreObjectTypes;
			}
		}

		private ObjectInformation GetRequestVersionObjectInformation()
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ExchangeVersion, ExchangeVersion>(0L, "ObjectInformation.GetRequestVersionObjectInformation: ObjectInformation effectiveVersion = {0}, Request version = {1}", this.effectiveVersion, ExchangeVersion.Current);
			if (ExchangeVersion.Current.Supports(this.effectiveVersion))
			{
				return this;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "ObjectInformation.GetRequestVersionObjectInformation: ObjectInformation not supported in request version. Examine priorVersionObjectInformation.");
			return this.priorVersionObjectInformation.GetRequestVersionObjectInformation();
		}

		internal Shape CreateShape(bool forPropertyBag)
		{
			ObjectInformation requestVersionObjectInformation = this.GetRequestVersionObjectInformation();
			Shape result;
			if (forPropertyBag && requestVersionObjectInformation.createShapeForPropertyBag != null)
			{
				result = requestVersionObjectInformation.createShapeForPropertyBag();
			}
			else
			{
				result = requestVersionObjectInformation.createShape();
			}
			return result;
		}

		private Shape PrivateCreateShape(StoreObject storeObject)
		{
			ObjectInformation requestVersionObjectInformation = this.GetRequestVersionObjectInformation();
			Shape result;
			if (requestVersionObjectInformation.createShapeForStoreObject != null)
			{
				result = requestVersionObjectInformation.createShapeForStoreObject(storeObject);
			}
			else
			{
				result = requestVersionObjectInformation.createShape();
			}
			return result;
		}

		internal static Shape CreateShape(StoreObject storeObject)
		{
			ObjectInformation objectInformation = Schema.GetObjectInformation(storeObject);
			return objectInformation.PrivateCreateShape(storeObject);
		}

		private Type associatedType;

		private StoreObjectType[] associatedStoreObjectTypes;

		private Shape.CreateShapeCallback createShape;

		private Shape.CreateShapeForPropertyBagCallback createShapeForPropertyBag;

		private Shape.CreateShapeForStoreObjectCallback createShapeForStoreObject;

		private ObjectInformation priorVersionObjectInformation;

		internal static readonly ObjectInformation NoPriorVersionObjectInformation;
	}
}
