using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class CorePropertyBagAdaptor : IPropertyBag
	{
		public CorePropertyBagAdaptor(CoreObjectProperties coreObjectProperties, ICorePropertyBag corePropertyBag, ICoreObject propertyMappingReference, ClientSideProperties clientSideProperties, PropertyConverter propertyConverter, DownloadBodyOption downloadBodyOption, Encoding string8Encoding, bool wantUnicode, bool isUpload, bool isFastTransferCopyProperties)
		{
			this.corePropertyBag = corePropertyBag;
			this.propertyMappingReference = propertyMappingReference;
			this.session = new SessionAdaptor(propertyMappingReference.Session);
			this.clientSideProperties = clientSideProperties;
			this.string8Encoding = string8Encoding;
			this.wantUnicode = wantUnicode;
			this.fixupMapping = ((isUpload || isFastTransferCopyProperties) ? FixupMapping.FastTransferCopyProperties : FixupMapping.FastTransfer);
			this.excludeList = CorePropertyBagAdaptor.CalculateBodyPropertiesExcludeList(corePropertyBag, downloadBodyOption, !this.IsMoveUser && !isUpload && !isFastTransferCopyProperties);
			this.coreObjectProperties = coreObjectProperties;
			this.propertyConverter = propertyConverter;
			if (this.BodyHelper != null)
			{
				this.conversionList = CorePropertyBagAdaptor.allBodyIncludes;
				return;
			}
			this.conversionList = Array<PropertyTag>.Empty;
		}

		private BodyHelper BodyHelper
		{
			get
			{
				BestBodyCoreObjectProperties bestBodyCoreObjectProperties = this.coreObjectProperties as BestBodyCoreObjectProperties;
				if (bestBodyCoreObjectProperties != null)
				{
					return bestBodyCoreObjectProperties.BodyHelper;
				}
				return null;
			}
		}

		public ISession Session
		{
			get
			{
				return this.session;
			}
		}

		public AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag)
		{
			if (this.BodyHelper != null)
			{
				this.BodyHelper.InitiatePropertyEvaluation();
			}
			PropertyDefinition propertyDefinition = this.GetPropertyDefinition(propertyTag);
			return this.GetAnnotatedProperty(propertyDefinition, propertyTag);
		}

		public IEnumerable<AnnotatedPropertyValue> GetAnnotatedProperties()
		{
			bool useUnicodeType = true;
			ICollection<PropertyDefinition> propertyDefinitions = this.coreObjectProperties.AllFoundProperties.Union(this.GetPropertyDefinitions(this.conversionList));
			ICollection<PropertyTag> propertyTags = MEDSPropertyTranslator.PropertyTagsFromPropertyDefinitions<PropertyDefinition>(this.propertyMappingReference.Session, propertyDefinitions, useUnicodeType);
			if (this.BodyHelper != null)
			{
				this.BodyHelper.InitiatePropertyEvaluation();
			}
			using (IEnumerator<PropertyTag> propertyTagEnumerator = propertyTags.GetEnumerator())
			{
				foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
				{
					propertyTagEnumerator.MoveNext();
					PropertyTag propertyTag = propertyTagEnumerator.Current;
					if (this.IncludeDownloadProperty(propertyTag))
					{
						yield return this.GetAnnotatedProperty(propertyDefinition, propertyTag);
					}
				}
			}
			yield break;
		}

		public virtual void SetProperty(PropertyValue propertyValue)
		{
			if (propertyValue.IsError)
			{
				throw new RopExecutionException(string.Format("Can't set errors for properties; propertyValue={0}.", propertyValue), (ErrorCode)2147746050U);
			}
			if (this.IgnoreUploadProperty(propertyValue.PropertyTag))
			{
				return;
			}
			PropertyConverter.ConvertFromExportToOurServerId(this.propertyMappingReference.Session, ref propertyValue);
			this.propertyConverter.ConvertPropertyValueFromClient(this.propertyMappingReference.Session, this.coreObjectProperties, ref propertyValue);
			EntryIdConverter.ConvertFromClient(this.string8Encoding, ref propertyValue);
			PropertyDefinition propertyDefinition;
			try
			{
				propertyDefinition = this.GetPropertyDefinition(propertyValue.PropertyTag);
			}
			catch (RopExecutionException)
			{
				if (this.propertyMappingReference.Session != null && this.propertyMappingReference.Session.OperationContext.IsMoveUser && propertyValue.PropertyTag.IsNamedProperty)
				{
					return;
				}
				throw;
			}
			this.coreObjectProperties.SetProperty(propertyDefinition, MEDSPropertyTranslator.TranslatePropertyValue(this.propertyMappingReference.Session, propertyValue));
		}

		public void Delete(PropertyTag property)
		{
			if (this.IgnoreUploadProperty(property))
			{
				return;
			}
			this.coreObjectProperties.DeleteProperty(this.GetPropertyDefinition(property));
		}

		public System.IO.Stream GetPropertyStream(PropertyTag property)
		{
			if (this.IgnoreDownloadProperty(property))
			{
				return null;
			}
			System.IO.Stream result;
			try
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					System.IO.Stream stream = null;
					if (this.BodyHelper != null && property.IsBodyProperty())
					{
						stream = this.BodyHelper.GetConversionStream(property);
						if (stream != null)
						{
							disposeGuard.Add<System.IO.Stream>(stream);
						}
					}
					if (stream == null)
					{
						stream = this.coreObjectProperties.OpenStream(this.GetPropertyDefinition(property), PropertyOpenMode.ReadOnly);
						disposeGuard.Add<System.IO.Stream>(stream);
						long length = stream.Length;
					}
					disposeGuard.Success();
					result = stream;
				}
			}
			catch (ObjectNotFoundException)
			{
				result = null;
			}
			return result;
		}

		public virtual System.IO.Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			if (this.IgnoreUploadProperty(property))
			{
				return null;
			}
			if (this.propertyConverter.ClientPropertyTagsThatRequireConversion.Contains(property) || EntryIdConverter.NeedsConversion(property))
			{
				return new MemoryPropertyBag.WriteMemoryStream(this, property, dataSizeEstimate);
			}
			return this.coreObjectProperties.OpenStream(this.GetPropertyDefinition(property), PropertyOpenMode.Create);
		}

		private static PropertyId[] CalculateBodyPropertiesExcludeList(ICorePropertyBag propertyBag, DownloadBodyOption downloadBodyOption, bool excludePreview)
		{
			EnumValidator.AssertValid<DownloadBodyOption>(downloadBodyOption);
			switch (downloadBodyOption)
			{
			case DownloadBodyOption.RtfOnly:
				if (!excludePreview)
				{
					return CorePropertyBagAdaptor.rtfCompressedExcludes;
				}
				return CorePropertyBagAdaptor.rtfCompressedExcludesWithPreview;
			case DownloadBodyOption.BestBodyOnly:
				return CorePropertyBagAdaptor.CalculateBestBodyPropertyExcludeList(propertyBag, excludePreview);
			case DownloadBodyOption.AllBodyProperties:
				if (!excludePreview)
				{
					return CorePropertyBagAdaptor.defaultExcludes;
				}
				return CorePropertyBagAdaptor.defaultExcludesWithPreview;
			default:
				throw new ArgumentOutOfRangeException("downloadBodyOption");
			}
		}

		private static PropertyId[] CalculateBestBodyPropertyExcludeList(ICorePropertyBag propertyBag, bool excludePreview)
		{
			switch (propertyBag.GetValueOrDefault<NativeBodyInfo>(CoreItemSchema.NativeBodyInfo))
			{
			case NativeBodyInfo.PlainTextBody:
				if (!excludePreview)
				{
					return CorePropertyBagAdaptor.plainTextExcludes;
				}
				return CorePropertyBagAdaptor.plainTextExcludesWithPreview;
			case NativeBodyInfo.RtfCompressedBody:
				if (!excludePreview)
				{
					return CorePropertyBagAdaptor.rtfCompressedExcludes;
				}
				return CorePropertyBagAdaptor.rtfCompressedExcludesWithPreview;
			case NativeBodyInfo.HtmlBody:
				if (!excludePreview)
				{
					return CorePropertyBagAdaptor.htmlExcludes;
				}
				return CorePropertyBagAdaptor.htmlExcludesWithPreview;
			case NativeBodyInfo.ClearSignedBody:
				if (!excludePreview)
				{
					return CorePropertyBagAdaptor.allBodyExcludes;
				}
				return CorePropertyBagAdaptor.allBodyExcludesWithPreview;
			default:
				if (!excludePreview)
				{
					return CorePropertyBagAdaptor.defaultExcludes;
				}
				return CorePropertyBagAdaptor.defaultExcludesWithPreview;
			}
		}

		private PropertyDefinition GetPropertyDefinition(PropertyTag property)
		{
			return this.GetPropertyDefinitions(new PropertyTag[]
			{
				property
			})[0];
		}

		private PropertyDefinition[] GetPropertyDefinitions(PropertyTag[] properties)
		{
			return MEDSPropertyTranslator.GetPropertyDefinitionsIgnoreTypeChecking(this.propertyMappingReference.Session, this.propertyMappingReference.PropertyBag, properties);
		}

		private AnnotatedPropertyValue GetAnnotatedProperty(PropertyDefinition propertyDefinition, PropertyTag propertyTag)
		{
			PropertyValue propertyValue;
			if (!IdConverters.GetClientId(this.propertyMappingReference.Session, this.corePropertyBag, propertyTag, out propertyValue))
			{
				object xsoPropertyValue = this.coreObjectProperties.TryGetProperty(propertyDefinition);
				Feature.Stubbed(212759, "Support String8 for Restrictions in FastTransfer");
				bool useUnicodeForRestrictions = true;
				propertyValue = MEDSPropertyTranslator.TranslatePropertyValue(this.propertyMappingReference.Session, propertyTag, xsoPropertyValue, useUnicodeForRestrictions);
				if (this.BodyHelper != null && BodyHelper.IsFixupNeeded(propertyValue.PropertyTag))
				{
					this.BodyHelper.FixupProperty(ref propertyValue, this.fixupMapping);
				}
				this.propertyConverter.ConvertPropertyValueToClient(this.propertyMappingReference.Session, this.coreObjectProperties, ref propertyValue, null);
				PropertyConverter.ConvertFromOurToExportServerId(this.propertyMappingReference.Session, ref propertyValue);
				EntryIdConverter.ConvertToClient(this.wantUnicode, this.string8Encoding, ref propertyValue);
				if (!propertyValue.IsError)
				{
					propertyTag = propertyValue.PropertyTag;
				}
			}
			NamedProperty namedProperty = null;
			if (propertyTag.IsNamedProperty)
			{
				NamedPropertyDefinition namedPropertyDefinition = (NamedPropertyDefinition)propertyDefinition;
				NamedPropertyDefinition.NamedPropertyKey key = namedPropertyDefinition.GetKey();
				namedProperty = key.ToNamedProperty();
			}
			return new AnnotatedPropertyValue(propertyTag, propertyValue, namedProperty);
		}

		private bool IncludeDownloadProperty(PropertyTag propertyTag)
		{
			if (this.excludeList != null)
			{
				foreach (PropertyId propertyId in this.excludeList)
				{
					if (propertyTag.PropertyId == propertyId)
					{
						return false;
					}
				}
			}
			return !this.IgnoreDownloadProperty(propertyTag);
		}

		protected bool IsMoveUser
		{
			get
			{
				return this.propertyMappingReference != null && this.propertyMappingReference.Session != null && this.propertyMappingReference.Session.IsMoveUser;
			}
		}

		protected Encoding String8Encoding
		{
			get
			{
				return this.string8Encoding;
			}
		}

		protected virtual bool IgnoreDownloadProperty(PropertyTag property)
		{
			return !this.clientSideProperties.ShouldBeReturnedIfRequested(property.PropertyId);
		}

		protected virtual bool IgnoreUploadProperty(PropertyTag property)
		{
			return !this.clientSideProperties.ShouldBeReturnedIfRequested(property.PropertyId);
		}

		private static readonly PropertyId[] defaultExcludes = new PropertyId[]
		{
			PropertyId.NativeBodyInfo
		};

		private static readonly PropertyId[] plainTextExcludes = new PropertyId[]
		{
			PropertyId.NativeBodyInfo,
			PropertyId.RtfCompressed,
			PropertyId.Html
		};

		private static readonly PropertyId[] rtfCompressedExcludes = new PropertyId[]
		{
			PropertyId.NativeBodyInfo,
			PropertyId.Body,
			PropertyId.Html
		};

		private static readonly PropertyId[] htmlExcludes = new PropertyId[]
		{
			PropertyId.NativeBodyInfo,
			PropertyId.Body,
			PropertyId.RtfCompressed
		};

		private static readonly PropertyId[] allBodyExcludes = new PropertyId[]
		{
			PropertyId.NativeBodyInfo,
			PropertyId.Body,
			PropertyId.RtfCompressed,
			PropertyId.Html
		};

		private static readonly PropertyId[] previewExclude = new PropertyId[]
		{
			PropertyId.Preview
		};

		private static readonly PropertyId[] defaultExcludesWithPreview = CorePropertyBagAdaptor.defaultExcludes.Concat(CorePropertyBagAdaptor.previewExclude).ToArray<PropertyId>();

		private static readonly PropertyId[] plainTextExcludesWithPreview = CorePropertyBagAdaptor.plainTextExcludes.Concat(CorePropertyBagAdaptor.previewExclude).ToArray<PropertyId>();

		private static readonly PropertyId[] rtfCompressedExcludesWithPreview = CorePropertyBagAdaptor.rtfCompressedExcludes.Concat(CorePropertyBagAdaptor.previewExclude).ToArray<PropertyId>();

		private static readonly PropertyId[] htmlExcludesWithPreview = CorePropertyBagAdaptor.htmlExcludes.Concat(CorePropertyBagAdaptor.previewExclude).ToArray<PropertyId>();

		private static readonly PropertyId[] allBodyExcludesWithPreview = CorePropertyBagAdaptor.allBodyExcludes.Concat(CorePropertyBagAdaptor.previewExclude).ToArray<PropertyId>();

		private static readonly PropertyTag[] allBodyIncludes = new PropertyTag[]
		{
			PropertyTag.Body,
			PropertyTag.Html,
			PropertyTag.RtfCompressed,
			PropertyTag.RtfInSync
		};

		private static readonly PropertyTag[] rtfOnlyIncludes = new PropertyTag[]
		{
			PropertyTag.RtfCompressed,
			PropertyTag.RtfInSync
		};

		private readonly ICorePropertyBag corePropertyBag;

		private readonly ICoreObject propertyMappingReference;

		private readonly ISession session;

		private readonly PropertyId[] excludeList;

		private readonly PropertyTag[] conversionList;

		private readonly ClientSideProperties clientSideProperties;

		private readonly PropertyConverter propertyConverter;

		private readonly Encoding string8Encoding;

		private readonly bool wantUnicode;

		private readonly FixupMapping fixupMapping;

		private readonly CoreObjectProperties coreObjectProperties;
	}
}
