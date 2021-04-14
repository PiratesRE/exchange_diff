using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SynchronizerBase : BaseObject
	{
		protected SynchronizerBase(ReferenceCount<CoreFolder> syncRootFolder, SyncFlag syncFlags, SyncExtraFlag extraFlags, IcsState icsState)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<SynchronizerBase>(this);
				this.SyncRootFolder = syncRootFolder;
				this.SyncRootFolder.AddRef();
				this.SessionAdaptor = new SessionAdaptor(this.SyncRootFolder.ReferencedObject.Session);
				this.SyncFlags = syncFlags;
				this.ExtraFlags = extraFlags;
				this.IcsState = icsState;
				disposeGuard.Success();
			}
		}

		protected static void CheckRequiredProperties(IPropertyBag propertyBag, IEnumerable<PropertyTag> requiredProperties)
		{
			using (IEnumerator<PropertyTag> enumerator = propertyBag.WithNoValue(requiredProperties).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					PropertyTag propertyTag = enumerator.Current;
					throw new RopExecutionException(string.Format("Required property {0} is missing", propertyTag), (ErrorCode)2147942487U);
				}
			}
		}

		protected static void SetPropertyValuesFromServer(IPropertyBag propertyBag, StoreSession session, PropValue[] xsoPropValues)
		{
			NativeStorePropertyDefinition[] array = new NativeStorePropertyDefinition[xsoPropValues.Length];
			object[] array2 = new object[xsoPropValues.Length];
			for (int i = 0; i < xsoPropValues.Length; i++)
			{
				array[i] = (NativeStorePropertyDefinition)xsoPropValues[i].Property;
				array2[i] = xsoPropValues[i].Value;
			}
			ICollection<uint> first = PropertyTagCache.Cache.PropertyTagsFromPropertyDefinitions(session, array);
			ICollection<PropertyTag> propertyTags = from propertyTag in first
			select new PropertyTag(propertyTag);
			bool useUnicodeForRestrictions = true;
			PropertyValue[] array3 = MEDSPropertyTranslator.TranslatePropertyValues(session, propertyTags, array2, useUnicodeForRestrictions);
			foreach (PropertyValue property in array3)
			{
				if (!property.IsError)
				{
					propertyBag.SetProperty(property);
				}
			}
		}

		protected static void TranslateFlag(SyncFlag sourceFlag, ManifestConfigFlags destinationFlag, SyncFlag sourceFlags, ref ManifestConfigFlags destinationFlags)
		{
			if ((sourceFlags & sourceFlag) == sourceFlag)
			{
				destinationFlags |= destinationFlag;
				return;
			}
			destinationFlags &= ~destinationFlag;
		}

		protected byte[] ConvertIdToLongTermId(IPropertyBag propertyBag, PropertyTag property)
		{
			return this.SyncRootFolder.ReferencedObject.Session.IdConverter.GetLongTermIdFromId(propertyBag.GetAnnotatedProperty(property).PropertyValue.GetServerValue<long>());
		}

		protected override void InternalDispose()
		{
			this.SyncRootFolder.Release();
			base.InternalDispose();
		}

		protected readonly ISession SessionAdaptor;

		protected readonly ReferenceCount<CoreFolder> SyncRootFolder;

		protected readonly SyncExtraFlag ExtraFlags;

		protected readonly SyncFlag SyncFlags;

		protected readonly IcsState IcsState;
	}
}
