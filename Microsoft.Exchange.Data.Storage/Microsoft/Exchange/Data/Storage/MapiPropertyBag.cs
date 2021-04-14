using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MapiPropertyBag : IDisposeTrackable, IDisposable
	{
		internal MapiPropertyBag(StoreSession storeSession, MapiProp mapiProp)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.mapiProp = mapiProp;
			this.exTimeZone = storeSession.ExTimeZone;
			this.storeSession = storeSession;
			if (mapiProp.DisposeTracker != null)
			{
				mapiProp.DisposeTracker.AddExtraDataWithStackTrace("MapiPropertyBag owns mapiProp at");
			}
		}

		public DisposeTracker DisposeTracker
		{
			get
			{
				return this.disposeTracker;
			}
		}

		internal static MapiPropertyBag CreateMapiPropertyBag(StoreSession storeSession, StoreObjectId id)
		{
			Util.ThrowOnNullArgument(storeSession, "storeSession");
			Util.ThrowOnNullArgument(id, "id");
			MapiProp disposable = null;
			MapiPropertyBag mapiPropertyBag = null;
			bool flag = false;
			MapiPropertyBag result;
			try
			{
				disposable = storeSession.GetMapiProp(id);
				mapiPropertyBag = new MapiPropertyBag(storeSession, disposable);
				flag = true;
				result = mapiPropertyBag;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(mapiPropertyBag);
					Util.DisposeIfPresent(disposable);
				}
			}
			return result;
		}

		internal static object GetValueFromPropValue(StoreSession storeSession, ExTimeZone exTimeZone, StorePropertyDefinition propertyDefinition, PropValue propertyValue)
		{
			if (propertyValue.IsError())
			{
				int num = propertyValue.GetErrorValue();
				if (num == -2147220732 && (propertyDefinition.SpecifiedWith == PropertyTypeSpecifier.GuidId || propertyDefinition.SpecifiedWith == PropertyTypeSpecifier.GuidString))
				{
					num = -2147221233;
				}
				return MapiPropertyBag.CreatePropertyError(num, propertyDefinition);
			}
			PropType propType = propertyValue.PropTag.ValueType();
			if (propType == PropType.Restriction)
			{
				if (storeSession == null)
				{
					throw new NotSupportedException("PT_RESTRICTION is not supported when we do not have a session. This very likely is an attachment.");
				}
				return FilterRestrictionConverter.CreateFilter(storeSession, storeSession.Mailbox.MapiStore, (Restriction)propertyValue.Value, false);
			}
			else
			{
				if (propertyValue.Value == null)
				{
					return MapiPropertyBag.CreatePropertyError(-2147221233, propertyDefinition);
				}
				if (propType == PropType.Actions)
				{
					if (storeSession == null)
					{
						throw new NotSupportedException("RuleAction type not supported without a session.");
					}
					RuleAction[] array = (RuleAction[])propertyValue.Value;
					RuleAction[] array2 = new RuleAction[array.Length];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = RuleActionConverter.ConvertRuleAction(storeSession, exTimeZone, array[i]);
					}
					return array2;
				}
				else if (propType == PropType.SysTime)
				{
					DateTime dateTime = (DateTime)propertyValue.Value;
					if (ExDateTime.IsValidDateTime(dateTime))
					{
						return new ExDateTime(exTimeZone, dateTime);
					}
					return MapiPropertyBag.CreatePropertyError(-2147221221, propertyDefinition);
				}
				else
				{
					if (propType == PropType.SysTimeArray)
					{
						DateTime[] array3 = (DateTime[])propertyValue.Value;
						foreach (DateTime dateTime2 in array3)
						{
							if (!ExDateTime.IsValidDateTime(dateTime2))
							{
								return MapiPropertyBag.CreatePropertyError(-2147221221, propertyDefinition);
							}
						}
						return ExTimeZoneHelperForMigrationOnly.ToExDateTimeArray(exTimeZone, array3);
					}
					return propertyValue.Value;
				}
			}
		}

		internal static PropValue GetPropValueFromValue(StoreSession session, ExTimeZone timeZone, PropTag propTag, object value)
		{
			return new PropValue(propTag, MapiPropertyBag.GetMapiValueFromValue(session, timeZone, value));
		}

		internal static PropValue[] MapiPropValuesFromXsoProperties(StoreSession storeSession, MapiProp mapiProp, PropertyDefinition[] propertyDefinitions, object[] propertyValues)
		{
			PropValue[] array = new PropValue[propertyValues.Length];
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions<PropertyDefinition>(mapiProp, storeSession, false, propertyDefinitions);
			int num = 0;
			foreach (PropTag propTag in collection)
			{
				array[num] = MapiPropertyBag.GetPropValueFromValue(storeSession, storeSession.ExTimeZone, propTag, propertyValues[num]);
				num++;
			}
			return array;
		}

		internal PropertyBagSaveFlags SaveFlags
		{
			get
			{
				this.CheckDisposed("MapiPropertyBag.SaveFlags.get");
				return this.saveFlags;
			}
			set
			{
				this.CheckDisposed("MapiPropertyBag.SaveFlags.set");
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.saveFlags = value;
			}
		}

		internal void SetUpdateImapIdFlag()
		{
			this.updateImapItemId = true;
		}

		internal ICollection<PropValue> GetAllProperties()
		{
			PropValue[] mapiPropValues = null;
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				mapiPropValues = this.mapiProp.GetAllProps();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::GetAllProperties. MapiProp = {0}.", this.MapiProp),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::GetAllProperties. MapiProp = {0}.", this.MapiProp),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			NativeStorePropertyDefinition[] array;
			PropTag[] allPropTags;
			object[] array2;
			PropertyTagCache.ResolveAndFilterPropertyValues(NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, this.storeSession, this.mapiProp, this.exTimeZone, mapiPropValues, out array, out allPropTags, out array2);
			this.RetryBigProperties(array, allPropTags, array2);
			return new ComputedElementCollection<NativeStorePropertyDefinition, object, PropValue>(new Func<NativeStorePropertyDefinition, object, PropValue>(PropValue.CreatePropValue<NativeStorePropertyDefinition>), array, array2, array2.Length);
		}

		internal object[] GetProperties(IList<NativeStorePropertyDefinition> propertyDefinitions)
		{
			this.CheckDisposed("GetProperties");
			if (propertyDefinitions == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "MapiPropertyBag::GetProperties. {0} == null.", "propertyDefinitions");
				throw new ArgumentNullException("propertyDefinitions");
			}
			int count = propertyDefinitions.Count;
			if (count == 0)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "MapiPropertyBag::GetProperties. {0} contain zero elements.", "propertyDefinitions");
				throw new ArgumentException(ServerStrings.ExEmptyCollection("propertyDefinitions"), "propertyDefinitions");
			}
			object[] array = new object[count];
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(this.MapiProp, this.storeSession, true, !this.storeSession.Capabilities.IsReadOnly, !this.storeSession.Capabilities.IsReadOnly, propertyDefinitions);
			PropValue[] array2 = null;
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				array2 = this.MapiProp.GetProps(collection);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::GetProperties. MapiProp = {0}.", this.MapiProp),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::GetProperties. MapiProp = {0}.", this.MapiProp),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			for (int i = 0; i < count; i++)
			{
				array[i] = MapiPropertyBag.GetValueFromPropValue(this.storeSession, this.ExTimeZone, InternalSchema.ToStorePropertyDefinition(propertyDefinitions[i]), array2[i]);
			}
			this.RetryBigProperties(propertyDefinitions, collection, array);
			return array;
		}

		internal PropertyError[] SetPropertiesWithChangeKeyCheck(byte[] changeKey, PropertyDefinition[] propertyDefinitions, object[] propertyValues)
		{
			if (changeKey != null)
			{
				return this.InternalSetProperties(propertyDefinitions, propertyValues, (PropValue[] propValues) => ((MapiFolder)this.MapiProp).SetPropsConditional(Restriction.EQ(PropTag.ChangeKey, changeKey), propValues));
			}
			return this.SetProperties(propertyDefinitions, propertyValues);
		}

		internal PropertyError[] SetProperties(PropertyDefinition[] propertyDefinitions, object[] propertyValues)
		{
			return this.InternalSetProperties(propertyDefinitions, propertyValues, (PropValue[] propValues) => this.MapiProp.SetProps(propValues, (this.SaveFlags & PropertyBagSaveFlags.NoChangeTracking) != PropertyBagSaveFlags.NoChangeTracking));
		}

		internal PropertyError[] DeleteProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			this.CheckDisposed("DeleteProperties");
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyDefinitions", 1));
			}
			if (propertyDefinitions.Count == 0)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "MapiPropertyBag::DeleteProperties. {0} contain zero elements.", "propertyDefinitions");
				throw new ArgumentException(ServerStrings.ExEmptyCollection("propertyDefinitions"), "propertyDefinitions");
			}
			IList<PropertyDefinition> list = new List<PropertyDefinition>();
			IList<PropertyDefinition> list2 = new List<PropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
				NativeStorePropertyDefinition nativeStorePropertyDefinition = storePropertyDefinition as NativeStorePropertyDefinition;
				if (nativeStorePropertyDefinition != null)
				{
					list.Add(storePropertyDefinition);
				}
				else
				{
					list2.Add(storePropertyDefinition);
				}
			}
			ICollection<PropTag> tags = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions<PropertyDefinition>(this.MapiProp, this.storeSession, false, list);
			PropProblem[] array = null;
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				array = this.MapiProp.DeleteProps(tags, (this.SaveFlags & PropertyBagSaveFlags.NoChangeTracking) != PropertyBagSaveFlags.NoChangeTracking);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotDeleteProperties, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::SetProperty. MapiProp = {0}.", this.MapiProp),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotDeleteProperties, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::SetProperty. MapiProp = {0}.", this.MapiProp),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			PropertyError[] array2;
			if (array != null || list2.Count > 0)
			{
				int num = list2.Count;
				if (array != null)
				{
					num += array.Length;
				}
				array2 = new PropertyError[num];
				int num2 = 0;
				if (array != null)
				{
					int i = 0;
					while (i < array.Length)
					{
						int scode = array[i].Scode;
						string errorDescription;
						PropertyErrorCode error = MapiPropertyHelper.MapiErrorToXsoError(scode, out errorDescription);
						PropertyError propertyError = new PropertyError(list[array[i].Index], error, errorDescription);
						array2[num2] = propertyError;
						ExTraceGlobals.StorageTracer.TraceError<string, PropertyError>((long)this.GetHashCode(), "MapiPropertyBag::DeleteProperties. Failed. DisplayName = {0}, Error = {1}.", list[array[i].Index].Name, propertyError);
						i++;
						num2++;
					}
				}
				int count = list2.Count;
				int j = 0;
				while (j < count)
				{
					PropertyError propertyError2 = new PropertyError(list2[j], PropertyErrorCode.SetCalculatedPropertyError, ServerStrings.ExSetNotSupportedForCalculatedProperty(list2[j].Name));
					array2[num2] = propertyError2;
					ExTraceGlobals.StorageTracer.TraceError<string, PropertyError>((long)this.GetHashCode(), "MapiPropertyBag::DeleteProperties. Failed. DisplayName = {0}, Error = {1}.", list2[j].Name, propertyError2);
					j++;
					num2++;
				}
			}
			else
			{
				array2 = MapiPropertyBag.EmptyPropertyErrorArray;
			}
			return array2;
		}

		internal MapiProp MapiProp
		{
			get
			{
				this.CheckDisposed("MapiProp::get");
				return this.mapiProp;
			}
		}

		internal StoreSession StoreSession
		{
			get
			{
				this.CheckDisposed("MapiPropertyBag.StoreSession::get");
				return this.storeSession;
			}
		}

		internal Stream OpenPropertyStream(NativeStorePropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			return this.OpenPropertyStream(propertyDefinition, openMode, true);
		}

		internal Stream OpenPropertyStream(NativeStorePropertyDefinition propertyDefinition, PropertyOpenMode openMode, bool bufferStream)
		{
			this.CheckDisposed("OpenPropertyStream");
			EnumValidator.AssertValid<PropertyOpenMode>(openMode);
			PropTag propTag = PropertyTagCache.Cache.PropTagFromPropertyDefinition(this.MapiProp, this.storeSession, propertyDefinition);
			OpenPropertyFlags flags;
			switch (openMode)
			{
			case PropertyOpenMode.ReadOnly:
				flags = OpenPropertyFlags.DeferredErrors;
				break;
			case PropertyOpenMode.Modify:
				flags = OpenPropertyFlags.Modify;
				break;
			case PropertyOpenMode.Create:
				flags = (OpenPropertyFlags.Create | OpenPropertyFlags.Modify);
				break;
			default:
				throw new ArgumentException(ServerStrings.ExInvalidParameter("openMode", 2), "openMode");
			}
			Stream stream = null;
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				stream = new MapiStreamWrapper(this.MapiProp.OpenStream(propTag, flags), this.StoreSession);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExUnableToGetStreamProperty(propertyDefinition.Name), ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::OpenPropertyStream. Failed to open property stream for property {0}, openMode = {1}.", propertyDefinition, openMode),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExUnableToGetStreamProperty(propertyDefinition.Name), ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::OpenPropertyStream. Failed to open property stream for property {0}, openMode = {1}.", propertyDefinition, openMode),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			if (bufferStream)
			{
				stream = new PooledBufferedStream(stream, StorageLimits.Instance.PropertyStreamPageSize);
			}
			return stream;
		}

		internal void SaveChanges(bool force)
		{
			this.CheckDisposed("SaveChanges");
			SaveChangesFlags saveChangesFlags = SaveChangesFlags.KeepOpenReadWrite;
			if (this.updateImapItemId)
			{
				saveChangesFlags |= SaveChangesFlags.ChangeIMAPId;
			}
			if (force)
			{
				saveChangesFlags |= SaveChangesFlags.ForceSave;
			}
			if ((this.SaveFlags & PropertyBagSaveFlags.ForceNotificationPublish) == PropertyBagSaveFlags.ForceNotificationPublish)
			{
				saveChangesFlags |= SaveChangesFlags.ForceNotificationPublish;
			}
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.MapiProp.SaveChanges(saveChangesFlags);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveChanges, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::SaveChanges.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveChanges, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::SaveChanges.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		internal ExTimeZone ExTimeZone
		{
			get
			{
				return this.exTimeZone;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ExTimeZone");
				}
				this.exTimeZone = value;
			}
		}

		internal ICollection<PropertyDefinition> GetAllFoundProperties()
		{
			PropTag[] propTags = null;
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				propTags = this.mapiProp.GetPropList();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotGetPropertyList, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get propList from the mapiProp.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotGetPropertyList, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get propList from the mapiProp.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, this.storeSession.Mailbox.MapiStore, this.storeSession, propTags);
		}

		private static object GetMapiValueFromValue(StoreSession session, ExTimeZone timeZone, object value)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(timeZone, "timeZone");
			if (value is PropertyError)
			{
				throw new ArgumentException(string.Format("We should never need to translate a PropertyError into mapi.net, because it makes no sense to set an error. PropertyError found = {0}.", value));
			}
			QueryFilter queryFilter = value as QueryFilter;
			if (queryFilter != null)
			{
				return FilterRestrictionConverter.CreateRestriction(session, timeZone, session.Mailbox.MapiStore, queryFilter);
			}
			RuleAction[] array = value as RuleAction[];
			if (array != null)
			{
				RuleAction[] array2 = new RuleAction[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = RuleActionConverter.ConvertRuleAction(session, timeZone, array[i]);
				}
				return array2;
			}
			return ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime(timeZone, value);
		}

		private static PropertyError CreatePropertyError(int mapiErrorCode, StorePropertyDefinition propertyDefinition)
		{
			string errorDescription;
			PropertyErrorCode propertyErrorCode = MapiPropertyHelper.MapiErrorToXsoError(mapiErrorCode, out errorDescription);
			if (propertyErrorCode == PropertyErrorCode.NotEnoughMemory && propertyDefinition.Type != typeof(string) && propertyDefinition.Type != typeof(byte[]))
			{
				propertyErrorCode = PropertyErrorCode.CorruptedData;
			}
			PropertyError propertyError;
			if (propertyErrorCode == PropertyErrorCode.NotFound)
			{
				propertyError = propertyDefinition.GetNotFoundError();
			}
			else if (propertyErrorCode == PropertyErrorCode.NotEnoughMemory)
			{
				propertyError = propertyDefinition.GetNotEnoughMemoryError();
			}
			else
			{
				propertyError = new PropertyError(propertyDefinition, propertyErrorCode, errorDescription);
				ExTraceGlobals.StorageTracer.TraceError<PropertyError>(0L, "MapiPropertyBag::GetValueFromPropValue. Error = {0}.", propertyError);
			}
			return propertyError;
		}

		private static bool IsVariableLength(NativeStorePropertyDefinition propDef)
		{
			Type type = propDef.Type;
			return type == typeof(string) || type.IsArray;
		}

		private static bool IsPropertyRetriable(NativeStorePropertyDefinition propDef)
		{
			return (propDef.PropertyFlags & PropertyFlags.Streamable) != PropertyFlags.Streamable && MapiPropertyBag.IsVariableLength(propDef);
		}

		private PropertyError[] InternalSetProperties(PropertyDefinition[] propertyDefinitions, object[] propertyValues, MapiPropertyBag.MapiSetProps mapiSetProps)
		{
			this.CheckDisposed("SetProperties");
			if (propertyDefinitions == null)
			{
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyDefinitions", 1));
			}
			if (propertyValues == null)
			{
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyValues", 2));
			}
			if (propertyDefinitions.Length == 0)
			{
				return MapiPropertyBag.EmptyPropertyErrorArray;
			}
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions<PropertyDefinition>(this.MapiProp, this.storeSession, (this.saveFlags & PropertyBagSaveFlags.IgnoreUnresolvedHeaders) == PropertyBagSaveFlags.IgnoreUnresolvedHeaders, true, (this.saveFlags & PropertyBagSaveFlags.DisableNewXHeaderMapping) != PropertyBagSaveFlags.DisableNewXHeaderMapping, propertyDefinitions);
			List<PropValue> list = new List<PropValue>(propertyDefinitions.Length);
			int num = 0;
			foreach (PropTag propTag in collection)
			{
				if (propTag != PropTag.Unresolved)
				{
					InternalSchema.CheckPropertyValueType(propertyDefinitions[num], propertyValues[num]);
					list.Add(MapiPropertyBag.GetPropValueFromValue(this.storeSession, this.ExTimeZone, propTag, propertyValues[num]));
				}
				num++;
			}
			PropProblem[] array = null;
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					array = mapiSetProps(list.ToArray());
				}
				catch (MapiExceptionNotEnoughMemory mapiExceptionNotEnoughMemory)
				{
					ExTraceGlobals.StorageTracer.TraceError<MapiExceptionNotEnoughMemory>((long)this.GetHashCode(), "MapiPropertyBag::InternalSetProperties. Failed to SetProps due to MapiException {0}.", mapiExceptionNotEnoughMemory);
					string errorDescription = mapiExceptionNotEnoughMemory.ToString();
					List<PropertyError> list2 = new List<PropertyError>();
					foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
					{
						list2.Add(new PropertyError(propertyDefinition, PropertyErrorCode.NotEnoughMemory, errorDescription));
					}
					throw PropertyError.ToException(list2.ToArray());
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetProps, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::InternalSetProperties.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetProps, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("MapiPropertyBag::InternalSetProperties.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			PropertyError[] array2 = MapiPropertyBag.EmptyPropertyErrorArray;
			if (array != null)
			{
				array2 = new PropertyError[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					int scode = array[j].Scode;
					PropertyDefinition propertyDefinition2 = null;
					int num2 = 0;
					foreach (PropTag propTag2 in collection)
					{
						if (array[j].PropTag == propTag2)
						{
							propertyDefinition2 = propertyDefinitions[num2];
							break;
						}
						num2++;
					}
					string errorDescription2;
					PropertyErrorCode error = MapiPropertyHelper.MapiErrorToXsoError(scode, out errorDescription2);
					array2[j] = new PropertyError(propertyDefinition2, error, errorDescription2);
					ExTraceGlobals.StorageTracer.TraceError<string, MapiProp, PropertyError>((long)this.GetHashCode(), "MapiPropertyBag::InternalSetProperties. Failed. PropDef display name= {0}, MapiProp = {1}, Error = {2}.", propertyDefinition2.Name, this.MapiProp, array2[j]);
				}
			}
			return array2;
		}

		private void RetryBigProperties(IList<NativeStorePropertyDefinition> allPropDefs, ICollection<PropTag> allPropTags, object[] allValues)
		{
			List<int> list = null;
			int count = allPropDefs.Count;
			for (int i = 0; i < count; i++)
			{
				if (PropertyError.IsPropertyValueTooBig(allValues[i]) && MapiPropertyBag.IsPropertyRetriable(allPropDefs[i]))
				{
					if (list == null)
					{
						list = new List<int>(10);
					}
					list.Add(i);
				}
			}
			if (list == null)
			{
				return;
			}
			PropTag[] array = new PropTag[count];
			allPropTags.CopyTo(array, 0);
			bool flag = true;
			while (flag && list.Count > 0)
			{
				flag = false;
				PropTag[] array2 = new PropTag[list.Count];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = array[list[j]];
				}
				PropValue[] array3 = null;
				StoreSession storeSession = this.StoreSession;
				bool flag2 = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					array3 = this.mapiProp.GetProps(array2);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiPropertyBag::RetryBigProperties. MapiProp = {0}.", this.MapiProp),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ExGetPropsFailed, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("MapiPropertyBag::RetryBigProperties. MapiProp = {0}.", this.MapiProp),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag2)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				for (int k = array3.Length - 1; k >= 0; k--)
				{
					int num = list[k];
					object valueFromPropValue = MapiPropertyBag.GetValueFromPropValue(this.storeSession, this.ExTimeZone, InternalSchema.ToStorePropertyDefinition(allPropDefs[num]), array3[k]);
					if (!PropertyError.IsPropertyValueTooBig(valueFromPropValue))
					{
						allValues[num] = valueFromPropValue;
						list.RemoveAt(k);
						flag = true;
					}
				}
			}
		}

		private string CollectExtraDataOnFailedMapiPropDisposal(Exception e)
		{
			StackTrace stackTrace = new StackTrace(1, DisposeTrackerOptions.UseFullSymbols);
			return string.Format(CultureInfo.InvariantCulture, "MapiPropertyBag failed to dispose the contained MapiProp with callstack and exception details:{0}{1}{2}{3}", new object[]
			{
				Environment.NewLine,
				stackTrace,
				Environment.NewLine,
				e
			});
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiPropertyBag>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
			this.isDisposed = true;
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.InternalDispose(disposing);
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		internal void InternalDispose(bool disposing)
		{
			if (this.mapiProp != null && this.mapiProp.DisposeTracker != null)
			{
				this.mapiProp.DisposeTracker.AddExtraDataWithStackTrace(string.Format(CultureInfo.InvariantCulture, "MapiPropertyBag.InternalDispose({0}) called with stack", new object[]
				{
					disposing
				}));
			}
			if (disposing)
			{
				if (this.mapiProp != null)
				{
					try
					{
						StoreSession storeSession = null;
						bool flag = false;
						try
						{
							if (storeSession != null)
							{
								storeSession.BeginMapiCall();
								storeSession.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							this.mapiProp.Dispose();
						}
						catch (MapiPermanentException ex)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.StoreOperationFailed, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("We cannot recover from an exception thrown from a Dispose call.", new object[0]),
								ex
							});
						}
						catch (MapiRetryableException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.StoreOperationFailed, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("We cannot recover from an exception thrown from a Dispose call.", new object[0]),
								ex2
							});
						}
						finally
						{
							try
							{
								if (storeSession != null)
								{
									storeSession.EndMapiCall();
									if (flag)
									{
										storeSession.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
					}
					catch (Exception ex3)
					{
						if (this.mapiProp.DisposeTracker != null)
						{
							if (this.mapiProp.DisposeTracker.HasCollectedStackTrace)
							{
								string extraData = this.CollectExtraDataOnFailedMapiPropDisposal(ex3);
								this.mapiProp.DisposeTracker.AddExtraData(extraData);
							}
						}
						else if (this.disposeTracker.HasCollectedStackTrace)
						{
							string extraData2 = this.CollectExtraDataOnFailedMapiPropDisposal(ex3);
							this.disposeTracker.AddExtraData(extraData2);
						}
						ExDiagnostics.FailFast(string.Format("We cannot recover from an exception thrown from a Dispose call. Exception = {0}", ex3.ToString()), false);
						this.mapiProp = null;
						throw;
					}
					this.mapiProp = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		internal MapiProp DetachMapiProp()
		{
			MapiProp mapiProp = this.mapiProp;
			if (mapiProp.DisposeTracker != null)
			{
				mapiProp.DisposeTracker.AddExtraDataWithStackTrace("MapiPropertyBag detached mapiProp at");
			}
			this.mapiProp = null;
			this.Dispose();
			return mapiProp;
		}

		private MapiProp mapiProp;

		private bool isDisposed;

		private readonly DisposeTracker disposeTracker;

		private ExTimeZone exTimeZone = ExTimeZone.UtcTimeZone;

		private readonly StoreSession storeSession;

		private PropertyBagSaveFlags saveFlags;

		private bool updateImapItemId;

		internal static readonly PropertyError[] EmptyPropertyErrorArray = Array<PropertyError>.Empty;

		private delegate PropProblem[] MapiSetProps(PropValue[] propValues);
	}
}
