using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class CoreItemPropertyBag : IPropertyBag
	{
		public CoreItemPropertyBag(IPropertyBag propertyBag, bool sendEntryId)
		{
			this.propertyBag = propertyBag;
			this.addedProperties = (sendEntryId ? new PropertyTag[]
			{
				PropertyTag.OriginalMessageEntryId
			} : Array<PropertyTag>.Empty);
		}

		public AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag)
		{
			if (propertyTag == PropertyTag.OriginalMessageEntryId)
			{
				return new AnnotatedPropertyValue(propertyTag, this.GetAnnotatedProperty(PropertyTag.EntryId).PropertyValue.CloneAs(propertyTag.PropertyId), null);
			}
			return this.propertyBag.GetAnnotatedProperty(propertyTag);
		}

		public IEnumerable<AnnotatedPropertyValue> GetAnnotatedProperties()
		{
			return this.addedProperties.Select(new Func<PropertyTag, AnnotatedPropertyValue>(this.GetAnnotatedProperty)).Concat(this.propertyBag.GetAnnotatedProperties().Where(new Func<AnnotatedPropertyValue, bool>(this.NotAddedProperty))).Where(new Func<AnnotatedPropertyValue, bool>(this.IncludePropertyOnDownload));
		}

		public void SetProperty(PropertyValue propertyValue)
		{
			if (this.IncludePropertyOnUpload(propertyValue.PropertyTag))
			{
				this.propertyBag.SetProperty(propertyValue);
			}
		}

		public void Delete(PropertyTag property)
		{
			if (this.IncludePropertyOnUpload(property))
			{
				this.propertyBag.Delete(property);
			}
		}

		public Stream GetPropertyStream(PropertyTag property)
		{
			if (!this.IncludePropertyOnDownload(property) || !this.NotAddedProperty(property))
			{
				return null;
			}
			return this.propertyBag.GetPropertyStream(property);
		}

		public Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			if (!this.IncludePropertyOnUpload(property))
			{
				return null;
			}
			return this.propertyBag.SetPropertyStream(property, dataSizeEstimate);
		}

		public ISession Session
		{
			get
			{
				return this.propertyBag.Session;
			}
		}

		private bool IncludePropertyOnDownload(AnnotatedPropertyValue propertyValue)
		{
			return this.IncludePropertyOnDownload(propertyValue.PropertyTag);
		}

		private bool IncludePropertyOnDownload(PropertyTag property)
		{
			return property.PropertyId != PropertyTag.EntryId.PropertyId;
		}

		private bool IncludePropertyOnUpload(PropertyTag property)
		{
			return !CoreItemPropertyBag.StoreComputedMessageProperties.Contains(property, PropertyTag.PropertyIdComparer);
		}

		private bool NotAddedProperty(PropertyTag propertyTag)
		{
			foreach (PropertyTag propertyTag2 in this.addedProperties)
			{
				if (propertyTag2.PropertyId == propertyTag.PropertyId)
				{
					return false;
				}
			}
			return true;
		}

		private bool NotAddedProperty(AnnotatedPropertyValue propertyValue)
		{
			return this.NotAddedProperty(propertyValue.PropertyTag);
		}

		private static readonly PropertyTag[] StoreComputedMessageProperties = new PropertyTag[]
		{
			PropertyTag.RuleFolderEntryId
		};

		private readonly PropertyTag[] addedProperties;

		private readonly IPropertyBag propertyBag;
	}
}
