using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class FolderPropertyBagAdaptor : CorePropertyBagAdaptor
	{
		public FolderPropertyBagAdaptor(ICorePropertyBag corePropertyBag, ICoreObject propertyMappingReference, Encoding string8Encoding, bool wantUnicode, bool isUpload, bool shouldInterceptPropertyChanges) : base(new CoreObjectProperties(corePropertyBag), corePropertyBag, propertyMappingReference, ClientSideProperties.FolderInstance, PropertyConverter.Folder, DownloadBodyOption.AllBodyProperties, string8Encoding, wantUnicode, isUpload, false)
		{
			this.shouldInterceptPropertyChanges = shouldInterceptPropertyChanges;
			if (this.shouldInterceptPropertyChanges)
			{
				this.interceptedPropertyBag = new MemoryPropertyBag(base.Session);
			}
		}

		public override void SetProperty(PropertyValue propertyValue)
		{
			if (base.IsMoveUser && propertyValue.PropertyTag == PropertyTag.PackedNamedProps)
			{
				MDBEFCollection mdbefcollection = MDBEFCollection.CreateFrom(propertyValue.Value as byte[], base.String8Encoding);
				using (IEnumerator<AnnotatedPropertyValue> enumerator = mdbefcollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AnnotatedPropertyValue annotatedPropertyValue = enumerator.Current;
						PropertyTag propertyTag = annotatedPropertyValue.PropertyTag;
						base.Session.TryResolveFromNamedProperty(annotatedPropertyValue.NamedProperty, ref propertyTag);
						base.SetProperty(new PropertyValue(propertyTag, annotatedPropertyValue.PropertyValue.Value));
					}
					goto IL_9A;
				}
			}
			base.SetProperty(propertyValue);
			IL_9A:
			if (this.shouldInterceptPropertyChanges && !this.IgnoreUploadProperty(propertyValue.PropertyTag))
			{
				this.interceptedPropertyBag.SetProperty(propertyValue);
			}
		}

		public override System.IO.Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			if (this.IgnoreUploadProperty(property))
			{
				return null;
			}
			return new MemoryPropertyBag.WriteMemoryStream(this, property, dataSizeEstimate);
		}

		public IEnumerable<AnnotatedPropertyValue> GetInterceptedProperties()
		{
			if (!this.shouldInterceptPropertyChanges)
			{
				return null;
			}
			return this.interceptedPropertyBag.GetAnnotatedProperties();
		}

		private readonly bool shouldInterceptPropertyChanges;

		private MemoryPropertyBag interceptedPropertyBag;
	}
}
