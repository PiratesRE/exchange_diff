using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class PropertyServerObject : SessionServerObject
	{
		protected PropertyServerObject(ClientSideProperties clientSideProperties, PropertyConverter converter)
		{
			this.ClientSideProperties = clientSideProperties;
			this.PropertyConverter = converter;
		}

		protected PropertyServerObject(Logon logon, ClientSideProperties clientSideProperties, PropertyConverter converter) : base(logon)
		{
			this.ClientSideProperties = clientSideProperties;
			this.PropertyConverter = converter;
		}

		protected abstract IStorageObjectProperties StorageObjectProperties { get; }

		protected abstract IPropertyDefinitionFactory PropertyDefinitionFactory { get; }

		public ClientSideProperties ClientSideProperties { get; internal set; }

		public abstract Schema Schema { get; }

		protected virtual bool SupportsPropertyProblems
		{
			get
			{
				return true;
			}
		}

		internal PropertyConverter PropertyConverter { get; set; }

		public virtual PropertyProblem[] SaveAndGetPropertyProblems(NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] propertyTags)
		{
			return Array<PropertyProblem>.Empty;
		}

		protected virtual PropertyError[] InternalCopyTo(PropertyServerObject destinationPropertyServerObject, CopySubObjects copySubObjects, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] excludeProperties)
		{
			throw new RopExecutionException(string.Format("RopCopyTo is not supported for object of type {0).", base.GetType()), (ErrorCode)2147746050U);
		}

		protected virtual PropertyError[] InternalCopyProperties(PropertyServerObject destinationPropertyServerObject, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] properties)
		{
			throw new RopExecutionException(string.Format("RopCopyProperties is not supported for object of type {0).", base.GetType()), (ErrorCode)2147746050U);
		}

		protected virtual void OnBeforeOpenStream(StorePropertyDefinition propertyDefinition, OpenMode openMode)
		{
		}

		protected virtual void FixBodyPropertiesIfNeeded(PropertyValue[] values)
		{
		}

		protected virtual bool TryGetOneOffPropertyStream(PropertyTag propertyTag, OpenMode openMode, bool isAppend, out Stream momtStream, out uint length)
		{
			momtStream = null;
			length = 0U;
			return false;
		}

		protected virtual PropertyTag[] AdditionalPropertiesForGetPropertiesAll(bool useUnicodeType)
		{
			return null;
		}

		public virtual PropertyProblem[] SetProperties(PropertyValue[] propertyValues, bool trackChanges)
		{
			return this.InternalSetProperties(propertyValues);
		}

		protected PropertyProblem[] InternalSetProperties(PropertyValue[] propertyValues)
		{
			Util.ThrowOnNullArgument(propertyValues, "propertyValues");
			List<PropertyProblem> list = null;
			if (this.SupportsPropertyProblems)
			{
				list = new List<PropertyProblem>();
			}
			this.PropertyConverter.ConvertPropertyValuesFromClient(this.Session, this.StorageObjectProperties, propertyValues);
			if (propertyValues.Length > 0)
			{
				PropertyTag[] array = new PropertyTag[propertyValues.Length];
				for (int i = 0; i < propertyValues.Length; i++)
				{
					array[i] = propertyValues[i].PropertyTag;
				}
				NativeStorePropertyDefinition[] array2 = null;
				this.PropertyDefinitionFactory.TryGetPropertyDefinitionsFromPropertyTags(array, true, out array2);
				ushort num = 0;
				while ((int)num < propertyValues.Length)
				{
					NativeStorePropertyDefinition nativeStorePropertyDefinition = array2[(int)num];
					PropertyTag propertyTag = this.PropertyConverter.ConvertPropertyTagToClient(propertyValues[(int)num].PropertyTag);
					PropertyDefinition propertyDefinition;
					if (this.PropertyConverter.TryGetMappedPropertyDefinition(propertyValues[(int)num].PropertyTag, out propertyDefinition))
					{
						if (list != null)
						{
							list.Add(new PropertyProblem(num, propertyTag, (ErrorCode)2147746074U));
						}
					}
					else if (propertyTag == PropertyTag.SecureSubmitFlags)
					{
						if (list != null)
						{
							list.Add(new PropertyProblem(num, propertyTag, (ErrorCode)2147942405U));
						}
					}
					else if (nativeStorePropertyDefinition != null)
					{
						object value = MEDSPropertyTranslator.TranslatePropertyValue(this.Session, propertyValues[(int)num]);
						if (this.ShouldSkipPropertyChange(nativeStorePropertyDefinition))
						{
							if (list != null)
							{
								list.Add(new PropertyProblem(num, propertyTag, (ErrorCode)2147942405U));
							}
						}
						else
						{
							if (PropertyServerObject.TryFixPropertyValueIfNeeded(nativeStorePropertyDefinition, ref value))
							{
								try
								{
									this.StorageObjectProperties.SetProperty(nativeStorePropertyDefinition, value);
									goto IL_1B7;
								}
								catch (PropertyValidationException)
								{
									if (list != null)
									{
										list.Add(new PropertyProblem(num, propertyTag, (ErrorCode)2147942487U));
									}
									goto IL_1B7;
								}
							}
							if (list != null)
							{
								list.Add(new PropertyProblem(num, propertyTag, (ErrorCode)2147942487U));
							}
						}
					}
					else if (list != null)
					{
						list.Add(new PropertyProblem(num, propertyTag, (ErrorCode)2147746564U));
					}
					IL_1B7:
					num += 1;
				}
				PropertyProblem[] collection = this.SaveAndGetPropertyProblems(array2, array);
				if (list != null)
				{
					list.AddRange(collection);
				}
			}
			if (list == null)
			{
				return Array<PropertyProblem>.Empty;
			}
			return list.ToArray();
		}

		public virtual PropertyProblem[] DeleteProperties(PropertyTag[] propertyTags, bool trackChanges)
		{
			return this.InternalDeleteProperties(propertyTags);
		}

		protected PropertyProblem[] InternalDeleteProperties(PropertyTag[] propertyTags)
		{
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			NativeStorePropertyDefinition[] array = null;
			this.PropertyDefinitionFactory.TryGetPropertyDefinitionsFromPropertyTags(propertyTags, out array);
			if (array == null)
			{
				return Array<PropertyProblem>.Empty;
			}
			ushort num = 0;
			while ((int)num < array.Length)
			{
				NativeStorePropertyDefinition nativeStorePropertyDefinition = array[(int)num];
				if (nativeStorePropertyDefinition != null && !this.ShouldSkipPropertyChange(nativeStorePropertyDefinition))
				{
					this.StorageObjectProperties.DeleteProperty(nativeStorePropertyDefinition);
				}
				num += 1;
			}
			PropertyProblem[] result = this.SaveAndGetPropertyProblems(array, propertyTags);
			if (!this.SupportsPropertyProblems)
			{
				return Array<PropertyProblem>.Empty;
			}
			return result;
		}

		public PropertyValue[] GetPropertiesAll(ushort streamLimit, GetPropertiesFlags flags)
		{
			bool useUnicodeType = (flags & (GetPropertiesFlags)int.MinValue) != GetPropertiesFlags.None;
			PropertyTag[] validClientSideProperties = this.PropertyConverter.GetValidClientSideProperties(this, this.StorageObjectProperties.AllFoundProperties, useUnicodeType, this.AdditionalPropertiesForGetPropertiesAll(useUnicodeType));
			PropertyTag[] originalPropertyTags = null;
			return this.GetProperties(validClientSideProperties, flags, originalPropertyTags);
		}

		public PropertyTag[] GetPropertyList()
		{
			return this.PropertyConverter.GetValidClientSideProperties(this, this.StorageObjectProperties.AllFoundProperties, true, this.AdditionalPropertiesForGetPropertiesAll(true));
		}

		public PropertyValue[] GetPropertiesSpecific(ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags)
		{
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			if (propertyTags.Length == 0)
			{
				throw new RopExecutionException("Client requested 0 properties.", (ErrorCode)2147942487U);
			}
			return this.GetProperties(propertyTags, flags, propertyTags);
		}

		public FastTransferDownload FastTransferSourceCopyTo(byte level, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags)
		{
			Util.ThrowOnNullArgument(excludedPropertyTags, "excludedPropertyTags");
			flags &= ~RopHandlerHelper.FastTransferCopyClientOnlyFlags;
			RopHandler.CheckEnum<FastTransferCopyFlag>(flags);
			RopHandler.CheckEnum<FastTransferSendOption>(sendOptions);
			this.ClearCacheIfNeededForGetProperties();
			this.StorageObjectProperties.Load(StoreObjectSchema.ContentConversionProperties);
			return this.InternalFastTransferSourceCopyTo(level > 0, flags, sendOptions, excludedPropertyTags);
		}

		public FastTransferDownload FastTransferSourceCopyProperties(byte level, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] propertyTags)
		{
			flags &= ~RopHandlerHelper.FastTransferCopyPropertiesClientOnlyFlags;
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			RopHandler.CheckEnum<FastTransferCopyPropertiesFlag>(flags);
			RopHandler.CheckEnum<FastTransferSendOption>(sendOptions);
			if (flags == FastTransferCopyPropertiesFlag.Move)
			{
				throw Feature.NotImplemented(185369, string.Format("FastTransferCopyPropertiesFlag.Move is not implemented. flags = {0}.", flags));
			}
			this.ClearCacheIfNeededForGetProperties();
			this.StorageObjectProperties.Load(StoreObjectSchema.ContentConversionProperties);
			return this.InternalFastTransferSourceCopyProperties(level > 0, flags, sendOptions, propertyTags);
		}

		public FastTransferUpload FastTransferDestinationCopyOperationConfigure(FastTransferCopyOperation copyOperation, FastTransferCopyPropertiesFlag flags)
		{
			RopHandler.CheckEnum<FastTransferCopyOperation>(copyOperation);
			RopHandler.CheckEnum<FastTransferCopyPropertiesFlag>(flags);
			switch (copyOperation)
			{
			case FastTransferCopyOperation.CopyTo:
				return this.InternalFastTransferDestinationCopyTo();
			case FastTransferCopyOperation.CopyProperties:
				return this.InternalFastTransferDestinationCopyProperties();
			case FastTransferCopyOperation.CopyMessages:
				return RopHandler.Downcast<Folder>(this).InternalFastTransferDestinationCopyMessages();
			case FastTransferCopyOperation.CopyFolder:
				return RopHandler.Downcast<Folder>(this).InternalFastTransferDestinationCopyFolder();
			default:
				throw new ArgumentException(string.Format("Unrecognized FastTransferCopyOperation. {0}", copyOperation));
			}
		}

		public void GetNamedProperties(Guid? propertyGuid, bool skipKindString, bool skipKindId, out PropertyId[] propertyIds, out NamedProperty[] namedProperties)
		{
			NamedPropertyDefinition.NamedPropertyKey[] namesFromGuid = NamedPropConverter.GetNamesFromGuid(this.Session, (propertyGuid != null) ? propertyGuid.Value : Guid.Empty, skipKindString, skipKindId);
			namedProperties = (from key in namesFromGuid
			select key.ToNamedProperty()).ToArray<NamedProperty>();
			propertyIds = (from propertyId in NamedPropConverter.GetIdsFromNames(this.Session, false, namesFromGuid.ToList<NamedPropertyDefinition.NamedPropertyKey>())
			select (PropertyId)propertyId).ToArray<PropertyId>();
		}

		protected virtual FastTransferDownload InternalFastTransferSourceCopyTo(bool isShallowCopy, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags)
		{
			throw new RopExecutionException(string.Format("FastTransferSourceCopyTo not supported for object of type {0}.", base.GetType()), (ErrorCode)2147746050U);
		}

		protected virtual FastTransferDownload InternalFastTransferSourceCopyProperties(bool isShallowCopy, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] includedProperties)
		{
			throw new RopExecutionException(string.Format("FastTransferSourceCopyProperties not supported for object of type {0}.", base.GetType()), (ErrorCode)2147746050U);
		}

		protected virtual FastTransferUpload InternalFastTransferDestinationCopyProperties()
		{
			throw new RopExecutionException(string.Format("FastTransferDestinationCopyProperties not supported for object of type {0}.", base.GetType()), (ErrorCode)2147746050U);
		}

		protected virtual FastTransferUpload InternalFastTransferDestinationCopyTo()
		{
			throw new RopExecutionException(string.Format("FastTransferDestinationCopyTo not supported for object of type {0}.", base.GetType()), (ErrorCode)2147746050U);
		}

		protected virtual bool ShouldSkipPropertyChange(StorePropertyDefinition propertyDefinition)
		{
			return false;
		}

		protected abstract StreamSource GetStreamSource();

		internal override void OnAccess()
		{
			base.OnAccess();
			this.StorageObjectProperties.Load(null);
		}

		public PropertyProblem[] CopyTo(PropertyServerObject destinationPropertyServerObject, bool reportProgress, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludedPropertyTags)
		{
			Util.ThrowOnNullArgument(destinationPropertyServerObject, "destinationPropertyServerObject");
			Util.ThrowOnNullArgument(excludedPropertyTags, "excludedPropertyTags");
			if (reportProgress)
			{
				Feature.Stubbed(76040, "ropCopyTo with reporting progress");
				reportProgress = false;
			}
			NativeStorePropertyDefinition[] array = null;
			bool flag = this.PropertyDefinitionFactory.TryGetPropertyDefinitionsFromPropertyTags(excludedPropertyTags, out array);
			NativeStorePropertyDefinition[] array2 = null;
			if (!flag)
			{
				List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>();
				ushort num = 0;
				while ((int)num < array.Length)
				{
					if (array[(int)num] != null)
					{
						list.Add(array[(int)num]);
					}
					num += 1;
				}
				array2 = list.ToArray();
			}
			PropertyError[] propertyErrors = this.InternalCopyTo(destinationPropertyServerObject, copySubObjects ? CopySubObjects.Copy : CopySubObjects.DoNotCopy, PropertyServerObject.ToXsoCopyPropertiesFlags(copyPropertiesFlags), flag ? array : array2);
			return MEDSPropertyTranslator.ToPropertyProblems(this.Session, propertyErrors, this.PropertyConverter);
		}

		public PropertyProblem[] CopyProperties(PropertyServerObject destinationPropertyServerObject, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] propertyTags)
		{
			Util.ThrowOnNullArgument(destinationPropertyServerObject, "destinationPropertyServerObject");
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			if (reportProgress)
			{
				Feature.Stubbed(76040, "ropCopyProperties with reporting progress");
				reportProgress = false;
			}
			NativeStorePropertyDefinition[] array = null;
			bool flag = this.PropertyDefinitionFactory.TryGetPropertyDefinitionsFromPropertyTags(propertyTags, out array);
			NativeStorePropertyDefinition[] array2 = null;
			List<PropertyProblem> list = new List<PropertyProblem>();
			if (!flag)
			{
				List<NativeStorePropertyDefinition> list2 = new List<NativeStorePropertyDefinition>();
				ushort num = 0;
				while ((int)num < array.Length)
				{
					if (array[(int)num] == null)
					{
						PropertyProblem item = new PropertyProblem(num, propertyTags[(int)num], (ErrorCode)2147746063U);
						list.Add(item);
					}
					else
					{
						list2.Add(array[(int)num]);
					}
					num += 1;
				}
				array2 = list2.ToArray();
			}
			PropertyError[] propertyErrors = this.InternalCopyProperties(destinationPropertyServerObject, PropertyServerObject.ToXsoCopyPropertiesFlags(copyPropertiesFlags), flag ? array : array2);
			PropertyProblem[] collection = MEDSPropertyTranslator.ToPropertyProblems(this.Session, propertyErrors, this.PropertyConverter);
			list.AddRange(collection);
			return list.ToArray();
		}

		public Stream OpenStream(PropertyTag propertyTag, OpenMode openMode, out uint length)
		{
			length = 0U;
			if ((byte)(openMode & ~(OpenMode.ReadWrite | OpenMode.Create | OpenMode.OpenSoftDeleted)) != 0)
			{
				throw new RopExecutionException("OpenMode not supported.", (ErrorCode)2147746050U);
			}
			bool flag = (byte)(openMode & OpenMode.OpenSoftDeleted) != 0;
			openMode &= ~OpenMode.OpenSoftDeleted;
			if (flag && openMode != OpenMode.Create && openMode != OpenMode.ReadWrite)
			{
				throw new RopExecutionException("OpenMode.Append must be used with either OpenMode.Create or OpenMode.ReadWrite", (ErrorCode)2147746050U);
			}
			if (propertyTag.PropertyType != PropertyType.Object && propertyTag.PropertyType != PropertyType.Binary && propertyTag.PropertyType != PropertyType.String8 && propertyTag.PropertyType != PropertyType.Unicode)
			{
				throw new RopExecutionException("Streams are only supported on PropertyType binary, string or object.", (ErrorCode)2147746050U);
			}
			if (this.ClientSideProperties.Contains(propertyTag.PropertyId))
			{
				throw new RopExecutionException("Property not found.", (ErrorCode)2147746063U);
			}
			NativeStorePropertyDefinition[] array;
			if (!this.PropertyDefinitionFactory.TryGetPropertyDefinitionsFromPropertyTags(new PropertyTag[]
			{
				propertyTag
			}, out array))
			{
				throw new RopExecutionException("Property not found.", (ErrorCode)2147746063U);
			}
			if (openMode == OpenMode.BestAccess || openMode == OpenMode.NoBlock)
			{
				throw Feature.NotImplemented(33084, string.Format("Invalid stream OpenMode: {0}", openMode));
			}
			this.OnBeforeOpenStream(array[0], openMode);
			Stream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = null;
				if (this.TryGetOneOffPropertyStream(propertyTag, openMode, flag, out stream, out length))
				{
					disposeGuard.Add<Stream>(stream);
				}
				else
				{
					Stream stream2 = this.StorageObjectProperties.OpenStream(array[0], MEDSPropertyTranslator.OpenModeToPropertyOpenMode(openMode, (ErrorCode)2147749887U));
					disposeGuard.Add<Stream>(stream2);
					if (propertyTag.PropertyType == PropertyType.String8)
					{
						EncodedStream encodedStream = new EncodedStream(stream2, this.String8Encoding, base.LogonObject.ResourceTracker);
						disposeGuard.Add<EncodedStream>(encodedStream);
						stream2 = encodedStream;
					}
					if (flag)
					{
						stream2.Seek(0L, SeekOrigin.End);
					}
					length = (uint)stream2.Length;
					StreamSource streamSource = this.GetStreamSource();
					disposeGuard.Add<StreamSource>(streamSource);
					stream = Stream.Create(stream2, propertyTag.PropertyType, base.LogonObject, streamSource);
					disposeGuard.Add<Stream>(stream);
				}
				disposeGuard.Success();
				result = stream;
			}
			return result;
		}

		private static CopyPropertiesFlags ToXsoCopyPropertiesFlags(CopyPropertiesFlags copyPropertiesFlags)
		{
			switch (copyPropertiesFlags)
			{
			case CopyPropertiesFlags.None:
				return CopyPropertiesFlags.None;
			case CopyPropertiesFlags.Move:
				return CopyPropertiesFlags.Move;
			case CopyPropertiesFlags.NoReplace:
				return CopyPropertiesFlags.NoReplace;
			default:
				throw new RopExecutionException(string.Format("Invalid CopyPropertiesFlags found: {0}", copyPropertiesFlags), (ErrorCode)2147942487U);
			}
		}

		private static bool TryFixPropertyValueIfNeeded(NativeStorePropertyDefinition property, ref object value)
		{
			if (property is NamedPropertyDefinition)
			{
				if (property.Type == typeof(int) && value is short)
				{
					value = Convert.ToInt32(value);
				}
				else if (property.Type == typeof(short) && value is int)
				{
					try
					{
						value = Convert.ToInt16(value);
					}
					catch (OverflowException)
					{
						return false;
					}
					return true;
				}
			}
			return true;
		}

		private PropertyValue[] GetProperties(PropertyTag[] propertyTags, GetPropertiesFlags flags, PropertyTag[] originalPropertyTags)
		{
			Util.ThrowOnNullArgument(propertyTags, "propertyTags");
			if (propertyTags.Length == 0)
			{
				throw new ArgumentException("propertyTags");
			}
			if (flags != GetPropertiesFlags.None && flags != (GetPropertiesFlags)(-2147483648))
			{
				throw new ArgumentOutOfRangeException("flag");
			}
			bool flag = (flags & (GetPropertiesFlags)int.MinValue) != GetPropertiesFlags.None;
			PropertyTag[] propertyTags2;
			bool areAllPropertiesResolved;
			PropertyDefinition[] propertyDefinitionsToRead = this.GetPropertyDefinitionsToRead(propertyTags, flag, out propertyTags2, out areAllPropertiesResolved);
			ICollection<PropertyDefinition> propertiesToLoad = this.GetPropertiesToLoad(areAllPropertiesResolved, propertyDefinitionsToRead);
			this.ClearCacheIfNeededForGetProperties();
			this.StorageObjectProperties.Load(propertiesToLoad);
			object[] propertyValues = this.GetPropertyValues(propertyDefinitionsToRead);
			PropertyValue[] array = MEDSPropertyTranslator.TranslatePropertyValues(this.Session, propertyTags2, propertyValues, flag);
			this.FixBodyPropertiesIfNeeded(array);
			this.PropertyConverter.ConvertPropertyValuesToClientAndSuppressClientSide(this.Session, this.StorageObjectProperties, array, originalPropertyTags, this.ClientSideProperties);
			return array;
		}

		private PropertyDefinition[] GetPropertyDefinitionsToRead(PropertyTag[] propertyTags, bool useUnicodeTypeForUnspecified, out PropertyTag[] resolvedPropertyTags, out bool areAllPropertiesResolved)
		{
			PropertyTag[] array = this.PropertyConverter.ConvertPropertyTagsFromClient(propertyTags);
			NativeStorePropertyDefinition[] array2 = null;
			this.PropertyDefinitionFactory.TryGetPropertyDefinitionsFromPropertyTags(array, true, out array2);
			PropertyDefinition[] array3 = Array.ConvertAll<NativeStorePropertyDefinition, PropertyDefinition>(array2, (NativeStorePropertyDefinition nativePropertyDefinition) => nativePropertyDefinition);
			areAllPropertiesResolved = true;
			resolvedPropertyTags = null;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyDefinition propertyDefinition = null;
				if (this.PropertyConverter.TryGetMappedPropertyDefinition(propertyTags[i], out propertyDefinition))
				{
					array3[i] = propertyDefinition;
				}
				PropertyType propertyType = array[i].PropertyType;
				if (!EnumValidator<PropertyType>.IsValidValue(propertyType) || propertyType == PropertyType.Error)
				{
					throw new RopExecutionException(string.Format("Found invalid property type: {0}.", propertyType), (ErrorCode)2147942487U);
				}
				PropertyDefinition propertyDefinition2 = array3[i];
				if (propertyDefinition2 == null)
				{
					areAllPropertiesResolved = false;
				}
				else if (propertyType == PropertyType.Unspecified)
				{
					if (resolvedPropertyTags == null)
					{
						resolvedPropertyTags = new PropertyTag[array.Length];
						Array.Copy(array, resolvedPropertyTags, array.Length);
					}
					PropertyType propertyType2 = PropertyTag.FromClrType(propertyDefinition2.Type);
					if (propertyDefinition2 is NativeStorePropertyDefinition)
					{
						propertyType2 = (PropertyType)((NativeStorePropertyDefinition)propertyDefinition2).MapiPropertyType;
						if (!useUnicodeTypeForUnspecified)
						{
							if (propertyType2 == PropertyType.Unicode)
							{
								propertyType2 = PropertyType.String8;
							}
							else if (propertyType2 == PropertyType.MultiValueUnicode)
							{
								propertyType2 = PropertyType.MultiValueString8;
							}
						}
					}
					resolvedPropertyTags[i] = new PropertyTag(resolvedPropertyTags[i].PropertyId, propertyType2);
				}
			}
			if (resolvedPropertyTags == null)
			{
				resolvedPropertyTags = array;
			}
			return array3;
		}

		private ICollection<PropertyDefinition> GetPropertiesToLoad(bool areAllPropertiesResolved, PropertyDefinition[] propertyDefinitions)
		{
			ICollection<PropertyDefinition> collection;
			if (!areAllPropertiesResolved)
			{
				collection = new List<PropertyDefinition>(propertyDefinitions.Length);
				for (int i = 0; i < propertyDefinitions.Length; i++)
				{
					if (propertyDefinitions[i] != null)
					{
						collection.Add(propertyDefinitions[i]);
					}
				}
			}
			else
			{
				collection = propertyDefinitions;
			}
			return collection;
		}

		private object[] GetPropertyValues(PropertyDefinition[] propertyDefinitions)
		{
			object[] array = new object[propertyDefinitions.Length];
			for (int i = 0; i < propertyDefinitions.Length; i++)
			{
				if (propertyDefinitions[i] != null)
				{
					array[i] = this.StorageObjectProperties.TryGetProperty(propertyDefinitions[i]);
				}
				else
				{
					array[i] = PropertyServerObject.NotFoundPropertyError;
				}
			}
			return array;
		}

		public virtual void ClearCacheIfNeededForGetProperties()
		{
		}

		private static readonly PropertyError NotFoundPropertyError = new PropertyError(null, PropertyErrorCode.NotFound);
	}
}
