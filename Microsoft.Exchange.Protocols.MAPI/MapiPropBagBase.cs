using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public abstract class MapiPropBagBase : MapiBase
	{
		internal MapiPropBagBase(MapiObjectType mapiObjectType) : base(mapiObjectType)
		{
		}

		protected abstract PropertyBag StorePropertyBag { get; }

		public virtual bool IsReadOnly
		{
			get
			{
				MapiPropBagBase parentObject = base.ParentObject;
				return parentObject != null && parentObject.IsReadOnly;
			}
			set
			{
				DiagnosticContext.TraceLocation((LID)47976U);
			}
		}

		public List<MapiBase> SubobjectsForTest
		{
			get
			{
				return this.subObjects;
			}
		}

		public bool NoReplicateOperationInProgress
		{
			get
			{
				return this.StorePropertyBag.NoReplicateOperationInProgress;
			}
			set
			{
				this.StorePropertyBag.NoReplicateOperationInProgress = value;
			}
		}

		public virtual bool CanUseSharedMailboxLockForCopy
		{
			get
			{
				return false;
			}
		}

		public MapiPropBagBase.PropertyReader GetPropsReader(MapiContext context, IList<StorePropTag> propTags)
		{
			base.ThrowIfNotValid(null);
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.PropertyGet, (LID)44167U);
			return new MapiPropBagBase.PropertyReader(this, propTags);
		}

		internal Properties GetProps(MapiContext context, IList<StorePropTag> propTags)
		{
			MapiPropBagBase.PropertyReader propsReader = this.GetPropsReader(context, propTags);
			Properties result = new Properties(propsReader.PropertyCount);
			Property prop;
			while (propsReader.ReadNext(context, out prop))
			{
				result.Add(prop);
			}
			return result;
		}

		public Property GetOneProp(MapiContext context, StorePropTag propTag)
		{
			base.ThrowIfNotValid(null);
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.PropertyGet, (LID)60551U);
			ErrorCode first = this.CheckPropertyOperationAllowed(context, MapiPropBagBase.PropOperation.GetProps, propTag, null);
			if (first != ErrorCode.NoError)
			{
				if (ExTraceGlobals.GetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.GetPropsPropertiesTracer.TraceDebug<StorePropTag>(0L, "GetProp blocked: tag:[{0}]", propTag);
				}
				return new Property(propTag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
			}
			Property property;
			if (MapiPropBagBase.getPropTestHook.Value != null)
			{
				property = MapiPropBagBase.getPropTestHook.Value(propTag);
			}
			else
			{
				property = this.InternalGetOneProp(context, propTag);
			}
			if (ExTraceGlobals.GetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.GetPropsPropertiesTracer.TraceDebug<Property>(0L, "GetProp: {0}", property);
			}
			return property;
		}

		public object GetOnePropValue(MapiContext context, StorePropTag propTag)
		{
			base.ThrowIfNotValid(null);
			Property oneProp = this.GetOneProp(context, propTag);
			if (oneProp.IsError)
			{
				return null;
			}
			return oneProp.Value;
		}

		public ErrorCode SetOneProp(MapiContext context, StorePropTag propTag, object value)
		{
			base.ThrowIfNotValid(null);
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.PropertySet, (LID)35975U);
			ErrorCode errorCode = this.CheckPropertyOperationAllowed(context, MapiPropBagBase.PropOperation.SetProps, propTag, value).Propagate((LID)36560U);
			if (errorCode == ErrorCode.NoError)
			{
				if (MapiPropBagBase.setPropTestHook.Value != null)
				{
					errorCode = MapiPropBagBase.setPropTestHook.Value(propTag, value);
				}
				else
				{
					errorCode = this.InternalSetOneProp(context, propTag, value);
				}
			}
			if (errorCode != ErrorCode.NoError)
			{
				if (propTag.IsCategory(11))
				{
					if (ExTraceGlobals.SetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.SetPropsPropertiesTracer.TraceDebug<ErrorCode, Property>(0L, "SetProp ignoring error {0}: {1}", errorCode, new Property(propTag, value));
					}
					errorCode = ErrorCode.NoError;
				}
				else if (ExTraceGlobals.SetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.SetPropsPropertiesTracer.TraceDebug<ErrorCode, Property>(0L, "SetProp error {0}: {1}", errorCode, new Property(propTag, value));
				}
			}
			else if (ExTraceGlobals.SetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.SetPropsPropertiesTracer.TraceDebug<Property>(0L, "SetProp: {0}", new Property(propTag, value));
			}
			return errorCode;
		}

		public virtual void SetProps(MapiContext context, Properties properties, ref List<MapiPropertyProblem> propProblems)
		{
			base.ThrowIfNotValid(null);
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.PropertySet, (LID)52359U);
			bool flag = ExTraceGlobals.SetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace);
			for (int i = 0; i < properties.Count; i++)
			{
				StorePropTag tag = properties[i].Tag;
				ErrorCode errorCode = this.CheckPropertyOperationAllowed(context, MapiPropBagBase.PropOperation.SetProps, tag, properties[i].Value).Propagate((LID)41064U);
				if (errorCode == ErrorCode.NoError)
				{
					if (MapiPropBagBase.setPropTestHook.Value != null)
					{
						errorCode = MapiPropBagBase.setPropTestHook.Value(tag, properties[i].Value);
					}
					else
					{
						errorCode = this.InternalSetOneProp(context, tag, properties[i].Value);
					}
				}
				if (errorCode != ErrorCode.NoError)
				{
					if (!tag.IsCategory(11))
					{
						if (flag)
						{
							ExTraceGlobals.SetPropsPropertiesTracer.TraceDebug<ErrorCode, Property>(0L, "SetProp error {0}: {1}", errorCode, properties[i]);
						}
						MapiPropertyProblem item = default(MapiPropertyProblem);
						item.MapiPropTag = tag;
						item.ErrorCode = errorCode;
						if (propProblems == null)
						{
							propProblems = new List<MapiPropertyProblem>();
						}
						propProblems.Add(item);
					}
					else if (flag)
					{
						ExTraceGlobals.SetPropsPropertiesTracer.TraceDebug<ErrorCode, Property>(0L, "SetProp ignoring error {0}: {1}", errorCode, properties[i]);
					}
				}
				else if (flag)
				{
					ExTraceGlobals.SetPropsPropertiesTracer.TraceDebug<Property>(0L, "SetProp: {0}", properties[i]);
				}
			}
		}

		public ErrorCode DeleteOneProp(MapiContext context, StorePropTag propTag)
		{
			base.ThrowIfNotValid(null);
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.PropertyDelete, (LID)44391U);
			ErrorCode errorCode = this.CheckPropertyOperationAllowed(context, MapiPropBagBase.PropOperation.DeleteProps, propTag, null).Propagate((LID)45160U);
			if (errorCode == ErrorCode.NoError)
			{
				if (MapiPropBagBase.setPropTestHook.Value != null)
				{
					errorCode = MapiPropBagBase.setPropTestHook.Value(propTag, null);
				}
				else
				{
					errorCode = this.InternalDeleteOneProp(context, propTag);
				}
			}
			if (errorCode != ErrorCode.NoError)
			{
				if (ExTraceGlobals.DeletePropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DeletePropsPropertiesTracer.TraceDebug<ErrorCode, StorePropTag>(0L, "DeleteProp error {0}: tag:[{1}]", errorCode, propTag);
				}
			}
			else if (ExTraceGlobals.DeletePropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.DeletePropsPropertiesTracer.TraceDebug<StorePropTag>(0L, "DeleteProp: tag:[{0}]", propTag);
			}
			return errorCode;
		}

		public virtual void DeleteProps(MapiContext context, StorePropTag[] propTags, ref List<MapiPropertyProblem> propProblems)
		{
			base.ThrowIfNotValid(null);
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty, AccessCheckOperation.PropertyDelete, (LID)60775U);
			bool flag = ExTraceGlobals.DeletePropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace);
			foreach (StorePropTag storePropTag in propTags)
			{
				ErrorCode errorCode = this.CheckPropertyOperationAllowed(context, MapiPropBagBase.PropOperation.DeleteProps, storePropTag, null).Propagate((LID)36904U);
				if (errorCode == ErrorCode.NoError)
				{
					if (MapiPropBagBase.setPropTestHook.Value != null)
					{
						errorCode = MapiPropBagBase.setPropTestHook.Value(storePropTag, null);
					}
					else
					{
						errorCode = this.InternalDeleteOneProp(context, storePropTag);
					}
				}
				if (errorCode != ErrorCode.NoError)
				{
					if (flag)
					{
						ExTraceGlobals.DeletePropsPropertiesTracer.TraceDebug<ErrorCode, StorePropTag>(0L, "DeleteProp error {0}: tag:[{1}]", errorCode, storePropTag);
					}
					MapiPropertyProblem item = default(MapiPropertyProblem);
					item.MapiPropTag = storePropTag;
					item.ErrorCode = errorCode;
					if (propProblems == null)
					{
						propProblems = new List<MapiPropertyProblem>();
					}
					propProblems.Add(item);
				}
				else if (flag)
				{
					ExTraceGlobals.DeletePropsPropertiesTracer.TraceDebug<StorePropTag>(0L, "DeleteProp: tag:[{0}]", storePropTag);
				}
			}
		}

		public List<Property> GetAllProperties(MapiContext context, GetPropListFlags flags, bool loadValue)
		{
			base.ThrowIfNotValid(null);
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, loadValue ? AccessCheckOperation.PropertyGet : AccessCheckOperation.PropertyGetList, (LID)36199U);
			List<Property> list;
			if (MapiPropBagBase.getAllPropertiesTestHook.Value != null)
			{
				list = MapiPropBagBase.getAllPropertiesTestHook.Value();
			}
			else
			{
				List<PropCategory> allowedCategories = new List<PropCategory>();
				List<PropCategory> blockedCategories = new List<PropCategory>();
				if ((flags & GetPropListFlags.FastTransfer) == GetPropListFlags.None)
				{
					blockedCategories.Add(PropCategory.NoGetPropList);
				}
				else
				{
					blockedCategories.Add(PropCategory.NoGetPropListForFastTransfer);
				}
				if (context.ClientType == ClientType.Migration)
				{
					allowedCategories.Add(PropCategory.FacebookProtectedProperties);
				}
				if (base.Logon.IsMoveUser)
				{
					allowedCategories.Add(PropCategory.SetPropAllowedForMailboxMove);
				}
				Predicate<StorePropTag> propertyFilter = (StorePropTag tag) => MapiPropBagBase.GetAllPropertiesPropertyCategoryFilter(tag, allowedCategories, blockedCategories);
				list = this.InternalGetAllProperties(context, flags, loadValue, propertyFilter);
			}
			ValueHelper.SortAndRemoveDuplicates<Property>(list, PropertyComparerByTag.Comparer);
			return list;
		}

		private static bool GetAllPropertiesPropertyCategoryFilter(StorePropTag tag, List<PropCategory> allowedCategories, List<PropCategory> blockedCategories)
		{
			for (int i = 0; i < allowedCategories.Count; i++)
			{
				if (tag.IsCategory((int)allowedCategories[i]))
				{
					if (ExTraceGlobals.GetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.GetPropsPropertiesTracer.TraceDebug<StorePropTag>(0L, "GetAllProperties: specifically allowed tag:[{0}]", tag);
					}
					return true;
				}
			}
			for (int j = 0; j < blockedCategories.Count; j++)
			{
				if (tag.IsCategory((int)blockedCategories[j]))
				{
					if (ExTraceGlobals.GetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.GetPropsPropertiesTracer.TraceDebug<StorePropTag>(0L, "GetAllProperties: specifically blocked tag:[{0}]", tag);
					}
					return false;
				}
			}
			return true;
		}

		public virtual void CopyTo(MapiContext context, MapiPropBagBase destination, IList<StorePropTag> propTagsExclude, CopyToFlags flags, ref List<MapiPropertyProblem> propProblems)
		{
			base.ThrowIfNotValid(null);
			if (base.GetType() != destination.GetType())
			{
				throw new ExExceptionInvalidParameter((LID)45816U, "Invalid destination object type in MapiMessage.CopyTo");
			}
			List<StorePropTag> list;
			if (MapiPropBagBase.getPropListTestHook.Value != null)
			{
				list = MapiPropBagBase.getPropListTestHook.Value();
			}
			else
			{
				list = this.InternalGetPropList(context, GetPropListFlags.None);
			}
			this.CopyToRemoveNoAccessProps(context, destination, list);
			MapiPropBagBase.CopyToRemoveExcludeProps(propTagsExclude, list);
			if (MapiPropBagBase.copyToTestHook.Value != null)
			{
				MapiPropBagBase.copyToTestHook.Value(list);
				return;
			}
			if ((CopyToFlags.DoNotReplaceProperties & flags) != CopyToFlags.None)
			{
				MapiPropBagBase.CopyToRemovePreexistingDestinationProperties(context, destination, list);
			}
			if (list.Count != 0)
			{
				this.CopyToCopyPropertiesToDestination(context, list, destination, ref propProblems);
				if ((CopyToFlags.MoveProperties & flags) != CopyToFlags.None)
				{
					MapiPropBagBase.CopyToRemoveProblemProperties(propProblems, list);
					this.CopyToRemoveSourceProperties(context, list, ref propProblems);
				}
			}
			this.CopyToInternal(context, destination, propTagsExclude, flags, ref propProblems);
		}

		public virtual void CopyProps(MapiContext context, MapiPropBagBase destination, IList<StorePropTag> propTags, bool replaceIfExists, ref List<MapiPropertyProblem> propProblems)
		{
			throw this.CreateCopyPropsNotSupportedException((LID)62200U, destination);
		}

		public virtual bool IsStreamSizeInvalid(MapiContext context, long size)
		{
			return false;
		}

		public MapiStream OpenStream(MapiContext context, StreamFlags flags, StorePropTag propTag, CodePage codePage)
		{
			MapiStream result;
			ErrorCode errorCode = this.OpenStream(context, flags, propTag, codePage, out result);
			if (errorCode != ErrorCode.NoError)
			{
				throw new StoreException((LID)36704U, errorCode);
			}
			return result;
		}

		public ErrorCode OpenStream(MapiContext context, StreamFlags flags, StorePropTag propTag, CodePage codePage, out MapiStream stream)
		{
			bool flag = flags == StreamFlags.AllowRead;
			MapiStream mapiStream = null;
			MapiStream mapiStream2 = null;
			bool flag2 = false;
			stream = null;
			this.CheckPropertyRights(context, FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty, AccessCheckOperation.StreamOpen, (LID)52583U);
			if (PropertyType.Unicode != propTag.PropType && PropertyType.Binary != propTag.PropType && PropertyType.Object != propTag.PropType)
			{
				string message = string.Format("MapiPropBagBase:OpenStream(): Only ptags for the following types are supported: PT_UNICODE, PT_STRING8, PT_BINARY, PT_OBJECT. Invalid propTag {0}. throwing ExExceptionInvalidParameter", propTag);
				ExTraceGlobals.GeneralTracer.TraceError(0L, message);
				return ErrorCode.CreateNotSupported((LID)37624U);
			}
			MapiPropBagBase.PropOperation propOperation = ((flags & (StreamFlags.AllowAppend | StreamFlags.AllowWrite)) == (StreamFlags)0) ? MapiPropBagBase.PropOperation.GetProps : MapiPropBagBase.PropOperation.SetProps;
			ErrorCode first = this.CheckPropertyOperationAllowed(context, propOperation, propTag, null).Propagate((LID)45592U);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)47912U);
			}
			if (this.IsReadOnly && propOperation == MapiPropBagBase.PropOperation.SetProps)
			{
				string message2 = string.Format("MapiPropBagBase:OpenStream(): Cannot open a writable stream on a read-only object. Property {0}.", propTag);
				ExTraceGlobals.GeneralTracer.TraceError(0L, message2);
				return ErrorCode.CreateNoAccess((LID)54008U);
			}
			if (MapiPropBagBase.openStreamTestHook.Value == null)
			{
				if (this.subObjects != null)
				{
					foreach (MapiBase mapiBase in this.subObjects)
					{
						MapiStream mapiStream3 = mapiBase as MapiStream;
						if (mapiStream3 != null && propTag.PropId == mapiStream3.Ptag.PropId && (mapiStream3.ShouldAllowWrite || mapiStream3.ShouldAppend))
						{
							if (!flag)
							{
								return ErrorCode.CreateNotSupported((LID)41720U);
							}
							mapiStream = mapiStream3;
							break;
						}
					}
				}
				ErrorCode noError;
				try
				{
					mapiStream2 = new MapiStream();
					try
					{
						if (mapiStream != null)
						{
							first = mapiStream2.ConfigureStream(context, mapiStream, flags, codePage);
						}
						else
						{
							first = mapiStream2.ConfigureStream(context, this, flags, propTag, codePage);
						}
						if (first != ErrorCode.NoError)
						{
							return first.Propagate((LID)43904U);
						}
					}
					catch (NullColumnException exception)
					{
						context.OnExceptionCatch(exception);
						return ErrorCode.CreateNotFound((LID)58104U);
					}
					flag2 = true;
					stream = mapiStream2;
					noError = ErrorCode.NoError;
				}
				finally
				{
					if (!flag2 && mapiStream2 != null)
					{
						mapiStream2.Dispose();
					}
				}
				return noError;
			}
			stream = MapiPropBagBase.openStreamTestHook.Value(propTag, flags);
			if (stream == null)
			{
				return ErrorCode.CreateNotFound((LID)99999U, propTag.PropTag);
			}
			return ErrorCode.NoError;
		}

		internal static IDisposable SetGetPropListTestHook(Func<List<StorePropTag>> callback)
		{
			return MapiPropBagBase.getPropListTestHook.SetTestHook(callback);
		}

		internal static IDisposable SetGetAllPropertiesTestHook(Func<List<Property>> callback)
		{
			return MapiPropBagBase.getAllPropertiesTestHook.SetTestHook(callback);
		}

		internal static IDisposable SetGetPropTestHook(Func<StorePropTag, Property> callback)
		{
			return MapiPropBagBase.getPropTestHook.SetTestHook(callback);
		}

		internal static IDisposable SetSetPropTestHook(Func<StorePropTag, object, ErrorCode> callback)
		{
			return MapiPropBagBase.setPropTestHook.SetTestHook(callback);
		}

		internal static IDisposable SetOpenStreamTestHook(Func<StorePropTag, StreamFlags, MapiStream> callback)
		{
			return MapiPropBagBase.openStreamTestHook.SetTestHook(callback);
		}

		internal static IDisposable SetCopyToTestHook(Action<List<StorePropTag>> callback)
		{
			return MapiPropBagBase.copyToTestHook.SetTestHook(callback);
		}

		protected static void CopyToRemoveExcludeProps(IList<StorePropTag> propTagsToExclude, List<StorePropTag> propTagsToCopy)
		{
			if (propTagsToExclude != null && propTagsToExclude.Count != 0)
			{
				for (int i = 0; i < propTagsToCopy.Count; i++)
				{
					bool flag = false;
					for (int j = 0; j < propTagsToExclude.Count; j++)
					{
						if (propTagsToCopy[i].PropTag == propTagsToExclude[j].PropTag)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						propTagsToCopy.RemoveAt(i);
						i--;
					}
				}
			}
		}

		protected static void CopyToRemovePreexistingDestinationProperties(MapiContext context, MapiPropBagBase destination, List<StorePropTag> propTagsToCopy)
		{
			for (int i = 0; i < propTagsToCopy.Count; i++)
			{
				object onePropValue = destination.GetOnePropValue(context, propTagsToCopy[i]);
				if (onePropValue != null)
				{
					propTagsToCopy.RemoveAt(i);
					i--;
				}
			}
		}

		protected static void CopyToRemoveInvalidProps(Properties props)
		{
			for (int i = props.Count - 1; i >= 0; i--)
			{
				if (props[i].IsError)
				{
					props.RemoveAt(i);
				}
			}
		}

		protected static void CopyToRemoveProblemProperties(List<MapiPropertyProblem> propProblems, List<StorePropTag> propTags)
		{
			if (propProblems != null)
			{
				for (int i = 0; i < propProblems.Count; i++)
				{
					propTags.Remove(propProblems[i].MapiPropTag);
				}
			}
		}

		protected virtual ErrorCode CheckPropertyOperationAllowed(MapiContext context, MapiPropBagBase.PropOperation operation, StorePropTag propTag, object value)
		{
			return MapiPropBagBase.CheckPropertyOperationAllowed(context, base.Logon, false, operation, propTag, value);
		}

		internal static ErrorCode CheckPropertyOperationAllowed(MapiContext context, MapiLogon logon, bool isEmbedded, MapiPropBagBase.PropOperation operation, StorePropTag propTag, object value)
		{
			ErrorCode result = ErrorCode.NoError;
			switch (operation)
			{
			case MapiPropBagBase.PropOperation.GetProps:
				if (propTag.IsCategory(0))
				{
					bool flag = false;
					if (propTag.IsCategory(8))
					{
						ClientType clientType = context.ClientType;
						if (clientType != ClientType.OWA)
						{
							switch (clientType)
							{
							case ClientType.Migration:
							case ClientType.TransportSync:
								break;
							default:
								if (clientType != ClientType.EDiscoverySearch)
								{
									goto IL_18B;
								}
								break;
							}
						}
						flag = true;
					}
					IL_18B:
					if (!flag)
					{
						result = ErrorCode.CreateNotFound((LID)50063U, propTag.PropTag);
					}
				}
				break;
			case MapiPropBagBase.PropOperation.SetProps:
			case MapiPropBagBase.PropOperation.DeleteProps:
				if (operation == MapiPropBagBase.PropOperation.SetProps)
				{
					if (propTag.IsNamedProperty && propTag.PropInfo == null)
					{
						result = ErrorCode.CreateUnregisteredNamedProp((LID)65128U, propTag.PropTag);
						break;
					}
					if (propTag.PropType == PropertyType.Error || propTag.PropType == PropertyType.Invalid || propTag.PropType == PropertyType.Unspecified)
					{
						result = ErrorCode.CreateInvalidType((LID)42567U, propTag.PropTag);
						break;
					}
				}
				if (propTag.IsCategory(3))
				{
					bool flag2 = false;
					if ((logon.IsMoveUser || (logon.AdminRights && (context.ClientType == ClientType.Migration || context.ClientType == ClientType.PublicFolderSystem))) && propTag.IsCategory(4))
					{
						flag2 = true;
					}
					else if (logon.AdminRights && propTag.IsCategory(5))
					{
						flag2 = true;
					}
					else if (logon.ExchangeTransportServiceRights && propTag.IsCategory(6))
					{
						flag2 = true;
					}
					else if (isEmbedded && propTag.IsCategory(7))
					{
						flag2 = true;
					}
					if (!flag2)
					{
						if (propTag.IsCategory(10))
						{
							result = ErrorCode.CreateComputed((LID)64024U, propTag.PropTag);
						}
						else
						{
							result = ErrorCode.CreateNoAccess((LID)45653U, propTag.PropTag);
						}
					}
				}
				break;
			}
			return result;
		}

		protected List<StorePropTag> InternalGetPropList(MapiContext context, GetPropListFlags flags)
		{
			base.ThrowIfNotValid(null);
			List<Property> list = this.InternalGetAllProperties(context, flags, false, null);
			List<StorePropTag> list2 = new List<StorePropTag>();
			if (list != null && list.Count > 0)
			{
				list2 = new List<StorePropTag>(list.Count);
				foreach (Property property in list)
				{
					list2.Add(property.Tag);
				}
			}
			return list2;
		}

		protected virtual List<Property> InternalGetAllProperties(MapiContext context, GetPropListFlags flags, bool loadValues, Predicate<StorePropTag> propertyFilter)
		{
			base.ThrowIfNotValid(null);
			List<Property> propList = new List<Property>(10);
			this.StorePropertyBag.EnumerateProperties(context, delegate(StorePropTag propTag, object propValue)
			{
				if (propertyFilter == null || propertyFilter(propTag))
				{
					propList.Add(new Property(propTag, propValue));
				}
				return true;
			}, loadValues);
			return propList;
		}

		protected virtual Property InternalGetOneProp(MapiContext context, StorePropTag propTag)
		{
			base.ThrowIfNotValid(null);
			object obj = null;
			StorePropTag tag;
			if (propTag.PropType == PropertyType.Unspecified)
			{
				if (!this.TryGetPropertyImp(context, propTag.PropId, out tag, out obj))
				{
					obj = LegacyHelper.BoxedErrorCodeNotFound;
					tag = propTag.ConvertToError();
				}
			}
			else
			{
				obj = this.GetPropertyValueImp(context, propTag);
				if (obj == null)
				{
					obj = LegacyHelper.BoxedErrorCodeNotFound;
					tag = propTag.ConvertToError();
				}
				else
				{
					tag = propTag;
				}
			}
			MapiProtocolsHelpers.AssertPropValueIsNotSqlType(obj);
			return new Property(tag, obj);
		}

		internal void InternalSetPropsShouldNotFail(MapiContext context, Properties properties)
		{
			for (int i = 0; i < properties.Count; i++)
			{
				this.InternalSetOnePropShouldNotFail(context, properties[i].Tag, properties[i].Value);
			}
		}

		internal void InternalSetOnePropShouldNotFail(MapiContext context, StorePropTag propTag, object value)
		{
			ErrorCode errorCode = this.InternalSetOneProp(context, propTag, value);
			if (errorCode != ErrorCode.NoError)
			{
				throw new ExExceptionInvalidObject((LID)33679U, string.Format("Got a problem on InternalSetOnePropShouldNotFail we should not have gotten: {0}", errorCode));
			}
		}

		protected virtual ErrorCode InternalSetOneProp(MapiContext context, StorePropTag propTag, object value)
		{
			base.ThrowIfNotValid(null);
			return MapiPropBagBase.InternalSetOneProp(context, base.Logon, this.StorePropertyBag, propTag, value);
		}

		protected static ErrorCode InternalSetOneProp(MapiContext context, MapiLogon logon, PropertyBag propertyBag, StorePropTag propTag, object value)
		{
			if (value is ErrorCodeValue)
			{
				ExTraceGlobals.SetPropsPropertiesTracer.TraceError<StorePropTag>(0L, "Property {0} cannot be set to error", propTag);
				return ErrorCode.CreateInvalidParameter((LID)63512U);
			}
			object obj = value;
			if (propTag.IsMultiValued && obj != null && ((Array)obj).Length == 0)
			{
				obj = null;
			}
			int countOfBlobProperties = propertyBag.CountOfBlobProperties;
			StorePropTag storePropTag;
			object obj2;
			if (countOfBlobProperties >= ConfigurationSchema.MaxNumberOfMapiProperties.Value && obj != null && !logon.AllowLargeItem() && !propertyBag.TryGetBlobProperty(context, propTag.PropId, out storePropTag, out obj2))
			{
				throw new StoreException((LID)33740U, ErrorCodeValue.TooManyProps);
			}
			return propertyBag.SetProperty(context, propTag, obj);
		}

		internal void InternalDeletePropsShouldNotFail(MapiContext context, StorePropTag[] propTags)
		{
			for (int i = 0; i < propTags.Length; i++)
			{
				this.InternalDeleteOnePropShouldNotFail(context, propTags[i]);
			}
		}

		internal void InternalDeleteOnePropShouldNotFail(MapiContext context, StorePropTag propTag)
		{
			base.ThrowIfNotValid(null);
			ErrorCode errorCode = this.InternalDeleteOneProp(context, propTag);
			if (errorCode != ErrorCode.NoError)
			{
				throw new ExExceptionInvalidObject((LID)64167U, string.Format("Got a problem on InternalDeleteOnePropShouldNotFail we should not have gotten ({0})", errorCode));
			}
		}

		protected virtual ErrorCode InternalDeleteOneProp(MapiContext context, StorePropTag propTag)
		{
			base.ThrowIfNotValid(null);
			if (ExTraceGlobals.DeletePropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.DeletePropsPropertiesTracer.TraceDebug<StorePropTag>(0L, "PropTag={0}", propTag);
			}
			return this.StorePropertyBag.SetProperty(context, propTag, null);
		}

		protected virtual bool TryGetPropertyImp(MapiContext context, ushort propId, out StorePropTag actualPropTag, out object propValue)
		{
			return this.StorePropertyBag.TryGetProperty(context, propId, out actualPropTag, out propValue);
		}

		protected virtual object GetPropertyValueImp(MapiContext context, StorePropTag propTag)
		{
			return this.StorePropertyBag.GetPropertyValue(context, propTag);
		}

		protected virtual void CopyToRemoveNoAccessProps(MapiContext context, MapiPropBagBase destination, List<StorePropTag> propTagsToCopy)
		{
			for (int i = propTagsToCopy.Count - 1; i >= 0; i--)
			{
				if (propTagsToCopy[i].IsCategory(9))
				{
					propTagsToCopy.RemoveAt(i);
				}
			}
		}

		protected void CopyToCopyPropertiesToDestination(MapiContext context, List<StorePropTag> propTagsToCopy, MapiPropBagBase destination, ref List<MapiPropertyProblem> propProblems)
		{
			Properties props = this.GetProps(context, propTagsToCopy);
			MapiPropBagBase.CopyToRemoveInvalidProps(props);
			int i = 0;
			while (i < props.Count)
			{
				StorePropTag tag = props[i].Tag;
				object value = props[i].Value;
				LargeValue largeValue = value as LargeValue;
				ErrorCode errorCode;
				if (largeValue != null)
				{
					Stream stream = null;
					Stream stream2 = null;
					try
					{
						errorCode = this.StorePropertyBag.OpenPropertyReadStream(context, tag, out stream);
						if (errorCode == ErrorCode.NoError)
						{
							errorCode = destination.StorePropertyBag.OpenPropertyWriteStream(context, tag, out stream2);
							if (errorCode == ErrorCode.NoError)
							{
								TempStream.CopyStream(stream, stream2);
							}
							else
							{
								ExTraceGlobals.SetPropsPropertiesTracer.TraceError<StorePropTag>(0L, "Setting property ({0}) failed to open destination value stream.", tag);
							}
						}
						else
						{
							ExTraceGlobals.SetPropsPropertiesTracer.TraceError<StorePropTag>(0L, "Setting property ({0}) failed to open source value stream.", tag);
						}
						goto IL_E3;
					}
					finally
					{
						if (stream != null)
						{
							stream.Dispose();
						}
						if (stream2 != null)
						{
							stream2.Dispose();
						}
					}
					goto IL_D8;
				}
				goto IL_D8;
				IL_E3:
				if (errorCode != ErrorCode.NoError)
				{
					MapiPropertyProblem item = default(MapiPropertyProblem);
					item.MapiPropTag = tag;
					item.ErrorCode = errorCode;
					if (propProblems == null)
					{
						propProblems = new List<MapiPropertyProblem>();
					}
					propProblems.Add(item);
				}
				i++;
				continue;
				IL_D8:
				errorCode = destination.InternalSetOneProp(context, tag, value);
				goto IL_E3;
			}
		}

		protected void CopyToRemoveSourceProperties(MapiContext context, List<StorePropTag> propTagsToRemove, ref List<MapiPropertyProblem> propProblems)
		{
			List<MapiPropertyProblem> list = null;
			for (int i = 0; i < propTagsToRemove.Count; i++)
			{
				StorePropTag storePropTag = propTagsToRemove[i];
				ErrorCode errorCode = this.InternalDeleteOneProp(context, storePropTag);
				if (errorCode != ErrorCode.NoError)
				{
					MapiPropertyProblem item = default(MapiPropertyProblem);
					item.MapiPropTag = storePropTag;
					item.ErrorCode = errorCode;
					if (propProblems == null)
					{
						propProblems = new List<MapiPropertyProblem>();
					}
					propProblems.Add(item);
				}
			}
			if (propProblems == null)
			{
				propProblems = list;
				return;
			}
			if (list != null && list.Count != 0)
			{
				foreach (MapiPropertyProblem item2 in list)
				{
					propProblems.Add(item2);
				}
			}
		}

		protected Exception CreateCopyPropsNotSupportedException(LID lid, MapiPropBagBase destination)
		{
			string message = string.Format("CopyProperties does not support copying from object type {0} to {1}", base.GetType(), destination.GetType());
			return new ExExceptionNoSupport(lid, message);
		}

		protected Exception CreateCopyToNotSupportedException(LID lid, MapiPropBagBase destination)
		{
			string message = string.Format("CopyTo does not support copying from object type {0} to {1}", base.GetType(), destination.GetType());
			return new ExExceptionNoSupport(lid, message);
		}

		protected virtual void CopyToInternal(MapiContext context, MapiPropBagBase destination, IList<StorePropTag> propTagsExclude, CopyToFlags flags, ref List<MapiPropertyProblem> propProblems)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiPropBagBase>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			try
			{
				if (calledFromDispose && this.subObjects != null)
				{
					for (int i = this.subObjects.Count - 1; i >= 0; i--)
					{
						MapiBase mapiBase = this.subObjects[i];
						mapiBase.Dispose();
					}
					this.subObjects = null;
				}
			}
			finally
			{
				base.InternalDispose(calledFromDispose);
			}
		}

		internal void AddSubObject(MapiBase subObject)
		{
			if (this.subObjects == null)
			{
				this.subObjects = new List<MapiBase>(4);
			}
			this.subObjects.Add(subObject);
		}

		internal void RemoveSubObject(MapiBase subObject)
		{
			if (!base.IsDisposing)
			{
				this.subObjects.Remove(subObject);
			}
		}

		internal void CheckRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, AccessCheckOperation operation, LID lid)
		{
			this.CheckRights(context, requestedRights, true, operation, lid);
		}

		internal virtual void CheckPropertyRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, AccessCheckOperation operation, LID lid)
		{
			this.CheckRights(context, requestedRights, operation, lid);
		}

		internal abstract void CheckRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, AccessCheckOperation operation, LID lid);

		internal void CommitDirtyStreams(MapiContext context)
		{
			if (this.subObjects != null)
			{
				for (int i = 0; i < this.subObjects.Count; i++)
				{
					MapiStream mapiStream = this.subObjects[i] as MapiStream;
					if (mapiStream != null && mapiStream.IsValid)
					{
						if (mapiStream.StreamSizeInvalid)
						{
							throw new ExExceptionMaxSubmissionExceeded((LID)36120U, "Exceeded the size limitation");
						}
						mapiStream.Commit(context);
					}
				}
			}
		}

		internal ErrorCode GetDataReader(MapiContext context, StorePropTag propTag, out Stream stream)
		{
			base.ThrowIfNotValid(null);
			return this.StorePropertyBag.OpenPropertyReadStream(context, propTag, out stream);
		}

		internal ErrorCode GetDataWriter(MapiContext context, StorePropTag propTag, out Stream stream)
		{
			base.ThrowIfNotValid(null);
			return this.StorePropertyBag.OpenPropertyWriteStream(context, propTag, out stream);
		}

		private static Hookable<Func<List<StorePropTag>>> getPropListTestHook = Hookable<Func<List<StorePropTag>>>.Create(false, null);

		private static Hookable<Func<List<Property>>> getAllPropertiesTestHook = Hookable<Func<List<Property>>>.Create(false, null);

		private static Hookable<Func<StorePropTag, Property>> getPropTestHook = Hookable<Func<StorePropTag, Property>>.Create(false, null);

		private static Hookable<Func<StorePropTag, object, ErrorCode>> setPropTestHook = Hookable<Func<StorePropTag, object, ErrorCode>>.Create(false, null);

		private static Hookable<Func<StorePropTag, StreamFlags, MapiStream>> openStreamTestHook = Hookable<Func<StorePropTag, StreamFlags, MapiStream>>.Create(false, null);

		protected static Hookable<Action<List<StorePropTag>>> copyToTestHook = Hookable<Action<List<StorePropTag>>>.Create(false, null);

		private List<MapiBase> subObjects;

		protected internal enum PropOperation
		{
			GetProps,
			SetProps,
			DeleteProps
		}

		public struct PropertyReader
		{
			internal PropertyReader(MapiPropBagBase propertyBag, IList<StorePropTag> propTags)
			{
				this.propertyBag = propertyBag;
				this.propTags = propTags;
				this.index = 0;
			}

			public int PropertyCount
			{
				get
				{
					if (this.propTags == null)
					{
						return 0;
					}
					return this.propTags.Count;
				}
			}

			public bool ReadNext(MapiContext context, out Property property)
			{
				if (this.propTags == null || this.index >= this.propTags.Count)
				{
					property = default(Property);
					return false;
				}
				StorePropTag storePropTag = this.propTags[this.index];
				ErrorCode first = this.propertyBag.CheckPropertyOperationAllowed(context, MapiPropBagBase.PropOperation.GetProps, storePropTag, null);
				if (first != ErrorCode.NoError)
				{
					if (ExTraceGlobals.GetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.GetPropsPropertiesTracer.TraceDebug<StorePropTag>(0L, "GetProp blocked: tag:[{0}]", storePropTag);
					}
					property = new Property(storePropTag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
				}
				else
				{
					if (MapiPropBagBase.getPropTestHook.Value != null)
					{
						property = MapiPropBagBase.getPropTestHook.Value(storePropTag);
					}
					else
					{
						property = this.propertyBag.InternalGetOneProp(context, storePropTag);
					}
					if (ExTraceGlobals.GetPropsPropertiesTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.GetPropsPropertiesTracer.TraceDebug<Property>(0L, "GetProp: {0}", property);
					}
				}
				this.index++;
				return true;
			}

			private MapiPropBagBase propertyBag;

			private IList<StorePropTag> propTags;

			private int index;
		}
	}
}
