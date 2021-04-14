using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class FilterRestrictionConverter
	{
		internal static Restriction CreateRestriction(StoreSession session, ExTimeZone exTimeZone, MapiProp mapiProp, QueryFilter filter)
		{
			return FilterRestrictionConverter.InternalCreateRestriction(session, exTimeZone, mapiProp, filter, 0);
		}

		internal static QueryFilter CreateFilter(StoreSession storeSession, MapiProp mapiProp, Restriction restriction, bool needsConvertToSmartFilter)
		{
			return FilterRestrictionConverter.InternalCreateFilter(storeSession, storeSession.ExTimeZone, mapiProp, restriction, 0, needsConvertToSmartFilter);
		}

		internal static void RegisterFilterTranslation(SmartPropertyDefinition property, Type filterType)
		{
			List<SmartPropertyDefinition> list = null;
			if (!FilterRestrictionConverter.registeredSmartProperties.TryGetValue(filterType, out list))
			{
				list = new List<SmartPropertyDefinition>();
				FilterRestrictionConverter.registeredSmartProperties[filterType] = list;
			}
			list.Add(property);
		}

		private static Restriction InternalCreateAndRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, AndFilter filter, int depth)
		{
			Restriction[] restrictions = FilterRestrictionConverter.InternalCreateRestrictions(storeSession, exTimeZone, mapiProp, filter, depth);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				result = Restriction.And(restrictions);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateOrRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, OrFilter filter, int depth)
		{
			Restriction[] restrictions = FilterRestrictionConverter.InternalCreateRestrictions(storeSession, exTimeZone, mapiProp, filter, depth);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				result = Restriction.Or(restrictions);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateNearRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, NearFilter filter, int depth)
		{
			Restriction restriction = FilterRestrictionConverter.InternalCreateRestriction(storeSession, exTimeZone, mapiProp, filter.Filter, depth);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				result = Restriction.Near((int)filter.Distance, filter.Ordered, (Restriction.AndRestriction)restriction);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateNotRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, NotFilter filter, int depth)
		{
			Restriction restriction = FilterRestrictionConverter.InternalCreateRestriction(storeSession, exTimeZone, mapiProp, filter.Filter, depth);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				result = Restriction.Not(restriction);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateSinglePropertyRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, SinglePropertyFilter filter, int depth)
		{
			Restriction calculatedPropertyRestriction = FilterRestrictionConverter.GetCalculatedPropertyRestriction(storeSession, exTimeZone, mapiProp, filter, depth);
			if (calculatedPropertyRestriction != null)
			{
				return calculatedPropertyRestriction;
			}
			bool flag = false;
			PropTag propTag = FilterRestrictionConverter.GetFilterPropertyTag(storeSession, mapiProp, filter, out flag);
			if (filter is ComparisonFilter)
			{
				propTag = FilterRestrictionConverter.GetSingleValueFilterPropertyTag(propTag, flag);
				ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
				PropValue propValueFromValue = MapiPropertyBag.GetPropValueFromValue(storeSession, exTimeZone, propTag, comparisonFilter.PropertyValue);
				if (filter is MultivaluedInstanceComparisonFilter)
				{
					propTag |= (PropTag)12288U;
				}
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
				{
					Restriction.RelOp relOp = Restriction.RelOp.Equal;
					goto IL_C5;
				}
				case ComparisonOperator.NotEqual:
				{
					Restriction.RelOp relOp = Restriction.RelOp.NotEqual;
					goto IL_C5;
				}
				case ComparisonOperator.LessThan:
				{
					Restriction.RelOp relOp = Restriction.RelOp.LessThan;
					goto IL_C5;
				}
				case ComparisonOperator.LessThanOrEqual:
				{
					Restriction.RelOp relOp = Restriction.RelOp.LessThanOrEqual;
					goto IL_C5;
				}
				case ComparisonOperator.GreaterThan:
				{
					Restriction.RelOp relOp = Restriction.RelOp.GreaterThan;
					goto IL_C5;
				}
				case ComparisonOperator.GreaterThanOrEqual:
				{
					Restriction.RelOp relOp = Restriction.RelOp.GreaterThanOrEqual;
					goto IL_C5;
				}
				case ComparisonOperator.IsMemberOf:
				{
					Restriction.RelOp relOp = Restriction.RelOp.MemberOfDL;
					goto IL_C5;
				}
				}
				throw new ArgumentException(ServerStrings.ExInvalidComparisonOperatorInComparisonFilter, "filter");
				IL_C5:
				object thisObject = null;
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
					Restriction.RelOp relOp;
					return new Restriction.PropertyRestriction(relOp, propTag, flag, propValueFromValue);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			}
			if (filter is ExistsFilter)
			{
				ExistsFilter existsFilter = (ExistsFilter)filter;
				object thisObject2 = null;
				bool flag3 = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag3 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					return Restriction.Exist(propTag);
				}
				catch (MapiPermanentException ex3)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex3, storeSession, thisObject2, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
						ex3
					});
				}
				catch (MapiRetryableException ex4)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex4, storeSession, thisObject2, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
						ex4
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag3)
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
			if (filter is ContentFilter)
			{
				propTag = FilterRestrictionConverter.GetSingleValueFilterPropertyTag(propTag, flag);
				ContentFilter contentFilter = (ContentFilter)filter;
				ContentFlags flags = FilterRestrictionConverter.CalculateMapiContentFlags(contentFilter.MatchFlags, contentFilter.MatchOptions);
				object value;
				if (filter is TextFilter)
				{
					value = ((TextFilter)filter).Text;
				}
				else
				{
					value = ((BinaryFilter)filter).BinaryData;
				}
				object thisObject3 = null;
				bool flag4 = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag4 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					return new Restriction.ContentRestriction(propTag, flag, value, flags);
				}
				catch (MapiPermanentException ex5)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex5, storeSession, thisObject3, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
						ex5
					});
				}
				catch (MapiRetryableException ex6)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex6, storeSession, thisObject3, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
						ex6
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag4)
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
			if (flag)
			{
				throw new ArgumentException(ServerStrings.ExInvalidMultivaluePropertyFilter(filter.GetType().ToString()), "filter");
			}
			if (filter is BitMaskFilter)
			{
				BitMaskFilter bitMaskFilter = (BitMaskFilter)filter;
				int mask = (int)bitMaskFilter.Mask;
				object thisObject4 = null;
				bool flag5 = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag5 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					if (bitMaskFilter.IsNonZero)
					{
						return Restriction.BitMaskNonZero(propTag, mask);
					}
					return Restriction.BitMaskZero(propTag, mask);
				}
				catch (MapiPermanentException ex7)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex7, storeSession, thisObject4, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
						ex7
					});
				}
				catch (MapiRetryableException ex8)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex8, storeSession, thisObject4, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
						ex8
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag5)
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
			throw new ArgumentException(ServerStrings.ExUnknownFilterType, "filter");
		}

		private static Restriction InternalCreateSubRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, SubFilter filter, int depth)
		{
			PropTag propertyTag;
			switch (filter.SubFilterProperty)
			{
			case SubFilterProperties.Attachments:
				propertyTag = (PropTag)InternalSchema.MessageAttachments.PropertyTag;
				break;
			case SubFilterProperties.Recipients:
				propertyTag = (PropTag)InternalSchema.MessageRecipients.PropertyTag;
				break;
			default:
				throw new ArgumentException(ServerStrings.ExInvalidSubFilterProperty, "filter");
			}
			Restriction restriction = FilterRestrictionConverter.InternalCreateRestriction(storeSession, exTimeZone, mapiProp, filter.Filter, depth);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				result = Restriction.Sub(propertyTag, restriction);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreatePropertyComparisonRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, PropertyComparisonFilter filter, int depth)
		{
			NativeStorePropertyDefinition nativeStorePropertyDefinition = filter.Property1 as NativeStorePropertyDefinition;
			NativeStorePropertyDefinition nativeStorePropertyDefinition2 = filter.Property2 as NativeStorePropertyDefinition;
			if (nativeStorePropertyDefinition == null || nativeStorePropertyDefinition2 == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<PropertyDefinition, PropertyDefinition>(0L, "Either of the properties in the PropertyComparisonFilter is not native store property. Property1 = {0}, property2 = {1}.", filter.Property1, filter.Property2);
				List<PropertyDefinition> list = new List<PropertyDefinition>();
				if (nativeStorePropertyDefinition == null)
				{
					list.Add(filter.Property1);
				}
				if (nativeStorePropertyDefinition2 == null)
				{
					list.Add(filter.Property2);
				}
				throw new FilterNotSupportedException(ServerStrings.ExComparisonFilterPropertiesNotSupported(filter.Property1.ToString(), filter.Property2.ToString()), filter, list.ToArray());
			}
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(mapiProp, storeSession, new NativeStorePropertyDefinition[]
			{
				nativeStorePropertyDefinition,
				nativeStorePropertyDefinition2
			});
			PropTag tag = PropTag.Null;
			PropTag tag2 = PropTag.Null;
			int num = 0;
			foreach (PropTag propTag in collection)
			{
				if (num == 0)
				{
					tag = propTag;
				}
				else if (num == 1)
				{
					tag2 = propTag;
				}
				num++;
			}
			if (num < 2)
			{
				throw new InvalidOperationException("Incorrect number of propertyTags returned: " + num.ToString(CultureInfo.InvariantCulture));
			}
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				switch (filter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					result = Restriction.EQ(tag, tag2);
					break;
				case ComparisonOperator.NotEqual:
					result = Restriction.NE(tag, tag2);
					break;
				case ComparisonOperator.LessThan:
					result = Restriction.LT(tag, tag2);
					break;
				case ComparisonOperator.LessThanOrEqual:
					result = Restriction.LE(tag, tag2);
					break;
				case ComparisonOperator.GreaterThan:
					result = Restriction.GT(tag, tag2);
					break;
				case ComparisonOperator.GreaterThanOrEqual:
					result = Restriction.GE(tag, tag2);
					break;
				default:
					throw new ArgumentException(ServerStrings.ExInvalidComparisionOperatorInPropertyComparisionFilter, "filter");
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateCommentRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, CommentFilter filter, int depth)
		{
			NativeStorePropertyDefinition[] array = filter.Properties as NativeStorePropertyDefinition[];
			if (array == null)
			{
				throw new FilterNotSupportedException(ServerStrings.ExCommentFilterPropertiesNotSupported, filter, filter.Properties);
			}
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(mapiProp, storeSession, array);
			PropValue[] array2 = new PropValue[filter.Properties.Length];
			int num = 0;
			foreach (PropTag propTag in collection)
			{
				array2[num] = new PropValue(propTag, filter.Values[num]);
				num++;
			}
			Restriction restriction = FilterRestrictionConverter.InternalCreateRestriction(storeSession, exTimeZone, mapiProp, filter.Filter, depth);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				result = Restriction.Comment(restriction, array2);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateCountRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, CountFilter filter, int depth)
		{
			Restriction restriction = FilterRestrictionConverter.InternalCreateRestriction(storeSession, exTimeZone, mapiProp, filter.Filter, depth);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				result = Restriction.Count((int)filter.Count, restriction);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateSizeRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, SizeFilter filter, int depth)
		{
			NativeStorePropertyDefinition nativeStorePropertyDefinition = filter.Property as NativeStorePropertyDefinition;
			if (nativeStorePropertyDefinition == null)
			{
				throw new FilterNotSupportedException(ServerStrings.ExSizeFilterPropertyNotSupported, filter, new PropertyDefinition[]
				{
					nativeStorePropertyDefinition
				});
			}
			PropTag tag = PropertyTagCache.Cache.PropTagFromPropertyDefinition(mapiProp, storeSession, nativeStorePropertyDefinition);
			object thisObject = null;
			bool flag = false;
			Restriction result;
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
				switch (filter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					result = Restriction.SizeEQ(tag, (int)filter.PropertySize);
					break;
				case ComparisonOperator.NotEqual:
					result = Restriction.SizeNE(tag, (int)filter.PropertySize);
					break;
				case ComparisonOperator.LessThan:
					result = Restriction.SizeLT(tag, (int)filter.PropertySize);
					break;
				case ComparisonOperator.LessThanOrEqual:
					result = Restriction.SizeLE(tag, (int)filter.PropertySize);
					break;
				case ComparisonOperator.GreaterThan:
					result = Restriction.SizeGT(tag, (int)filter.PropertySize);
					break;
				case ComparisonOperator.GreaterThanOrEqual:
					result = Restriction.SizeGE(tag, (int)filter.PropertySize);
					break;
				default:
					throw new ArgumentException("Invalid comparison operator in SizeFilter.", "filter");
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateRestriction, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("FilterRestrictionConverter::CreateRestriction. Failed on create a {0} restriction.", filter),
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
			return result;
		}

		private static Restriction InternalCreateRestriction(StoreSession storeSession, ExTimeZone exTimeZone, MapiProp mapiProp, QueryFilter filter, int depth)
		{
			if (depth >= 256)
			{
				throw new ArgumentException(ServerStrings.ExFilterHierarchyIsTooDeep, "filter");
			}
			CompositeFilter compositeFilter = filter as CompositeFilter;
			if ((compositeFilter != null && !compositeFilter.IgnoreWhenVerifyingMaxDepth) || filter is NotFilter || filter is SubFilter)
			{
				depth++;
			}
			if (filter is AndFilter)
			{
				return FilterRestrictionConverter.InternalCreateAndRestriction(storeSession, exTimeZone, mapiProp, (AndFilter)filter, depth);
			}
			if (filter is OrFilter)
			{
				return FilterRestrictionConverter.InternalCreateOrRestriction(storeSession, exTimeZone, mapiProp, (OrFilter)filter, depth);
			}
			if (filter is NearFilter)
			{
				return FilterRestrictionConverter.InternalCreateNearRestriction(storeSession, exTimeZone, mapiProp, (NearFilter)filter, depth);
			}
			if (filter is NotFilter)
			{
				return FilterRestrictionConverter.InternalCreateNotRestriction(storeSession, exTimeZone, mapiProp, (NotFilter)filter, depth);
			}
			if (filter is SinglePropertyFilter)
			{
				return FilterRestrictionConverter.InternalCreateSinglePropertyRestriction(storeSession, exTimeZone, mapiProp, (SinglePropertyFilter)filter, depth);
			}
			if (filter is SubFilter)
			{
				return FilterRestrictionConverter.InternalCreateSubRestriction(storeSession, exTimeZone, mapiProp, (SubFilter)filter, depth);
			}
			if (filter is PropertyComparisonFilter)
			{
				return FilterRestrictionConverter.InternalCreatePropertyComparisonRestriction(storeSession, exTimeZone, mapiProp, (PropertyComparisonFilter)filter, depth);
			}
			if (filter is CommentFilter)
			{
				return FilterRestrictionConverter.InternalCreateCommentRestriction(storeSession, exTimeZone, mapiProp, (CommentFilter)filter, depth);
			}
			if (filter is CountFilter)
			{
				return FilterRestrictionConverter.InternalCreateCountRestriction(storeSession, exTimeZone, mapiProp, (CountFilter)filter, depth);
			}
			if (filter is SizeFilter)
			{
				return FilterRestrictionConverter.InternalCreateSizeRestriction(storeSession, exTimeZone, mapiProp, (SizeFilter)filter, depth);
			}
			if (filter is TrueFilter)
			{
				return Restriction.True();
			}
			if (filter is FalseFilter)
			{
				return Restriction.False();
			}
			if (filter is NullFilter)
			{
				return null;
			}
			throw new ArgumentException(ServerStrings.ExUnknownFilterType, "filter");
		}

		private static Restriction GetCalculatedPropertyRestriction(StoreSession storeSession, ExTimeZone timeZone, MapiProp mapiProp, SinglePropertyFilter filter, int depth)
		{
			PropertyDefinition property = filter.Property;
			if (property is NativeStorePropertyDefinition)
			{
				return null;
			}
			if (property is SmartPropertyDefinition)
			{
				SmartPropertyDefinition smartPropertyDefinition = (SmartPropertyDefinition)property;
				QueryFilter queryFilter = smartPropertyDefinition.SmartFilterToNativeFilter(filter);
				if (queryFilter != null)
				{
					return FilterRestrictionConverter.InternalCreateRestriction(storeSession, timeZone, mapiProp, queryFilter, depth);
				}
			}
			FilterNotSupportedException ex = new FilterNotSupportedException(ServerStrings.ExFilterNotSupportedForProperty(filter.ToString(), property.Name), filter, new PropertyDefinition[]
			{
				property
			});
			ExTraceGlobals.StorageTracer.TraceError(0L, ex.Message);
			throw ex;
		}

		private static Restriction[] InternalCreateRestrictions(StoreSession storeSession, ExTimeZone timeZone, MapiProp mapiProp, CompositeFilter filter, int depth)
		{
			Restriction[] array = new Restriction[filter.FilterCount];
			int num = 0;
			foreach (QueryFilter filter2 in filter.Filters)
			{
				array[num++] = FilterRestrictionConverter.InternalCreateRestriction(storeSession, timeZone, mapiProp, filter2, depth);
			}
			return array;
		}

		private static PropTag GetFilterPropertyTag(StoreSession storeSession, MapiProp mapiProp, SinglePropertyFilter filter, out bool isComparisonAgainstMultivalueProperty)
		{
			NativeStorePropertyDefinition propertyDefinition = (NativeStorePropertyDefinition)filter.Property;
			PropTag propTag = PropertyTagCache.Cache.PropTagFromPropertyDefinition(mapiProp, storeSession, propertyDefinition);
			isComparisonAgainstMultivalueProperty = ((propTag & (PropTag)4096U) == (PropTag)4096U);
			return propTag;
		}

		private static PropTag GetSingleValueFilterPropertyTag(PropTag propTag, bool isComparisonAgainstMultivalueProperty)
		{
			PropTag result = propTag;
			if (isComparisonAgainstMultivalueProperty)
			{
				result = (propTag & (PropTag)4294963199U);
			}
			return result;
		}

		private static ContentFlags CalculateMapiContentFlags(MatchFlags matchFlags, MatchOptions matchOptions)
		{
			ContentFlags contentFlags = ContentFlags.FullString;
			if (matchOptions == MatchOptions.Suffix)
			{
				throw new NotSupportedException(ServerStrings.ExSuffixTextFilterNotSupported);
			}
			if (matchOptions == MatchOptions.SubString)
			{
				contentFlags |= ContentFlags.SubString;
			}
			else if (matchOptions == MatchOptions.Prefix)
			{
				contentFlags |= ContentFlags.Prefix;
			}
			else if (matchOptions == MatchOptions.PrefixOnWords)
			{
				contentFlags |= ContentFlags.PrefixOnWords;
			}
			else if (matchOptions == MatchOptions.ExactPhrase)
			{
				contentFlags |= ContentFlags.ExactPhrase;
			}
			else
			{
				contentFlags = contentFlags;
			}
			if ((matchFlags & MatchFlags.IgnoreCase) == MatchFlags.IgnoreCase)
			{
				contentFlags |= ContentFlags.IgnoreCase;
			}
			if ((matchFlags & MatchFlags.IgnoreNonSpace) == MatchFlags.IgnoreNonSpace)
			{
				contentFlags |= ContentFlags.IgnoreNonSpace;
			}
			if ((matchFlags & MatchFlags.Loose) == MatchFlags.Loose)
			{
				contentFlags |= ContentFlags.Loose;
			}
			return contentFlags;
		}

		private static ComparisonOperator GetComparisonOperator(Restriction.RelOp relOp)
		{
			switch (relOp)
			{
			case Restriction.RelOp.LessThan:
				return ComparisonOperator.LessThan;
			case Restriction.RelOp.LessThanOrEqual:
				return ComparisonOperator.LessThanOrEqual;
			case Restriction.RelOp.GreaterThan:
				return ComparisonOperator.GreaterThan;
			case Restriction.RelOp.GreaterThanOrEqual:
				return ComparisonOperator.GreaterThanOrEqual;
			case Restriction.RelOp.Equal:
				return ComparisonOperator.Equal;
			case Restriction.RelOp.NotEqual:
				return ComparisonOperator.NotEqual;
			default:
				if (relOp != Restriction.RelOp.MemberOfDL)
				{
					throw new NotSupportedException(ServerStrings.ExComparisonOperatorNotSupported(relOp));
				}
				return ComparisonOperator.IsMemberOf;
			}
		}

		private static QueryFilter[] InternalCreateFilters(StoreSession storeSession, ExTimeZone timeZone, MapiProp mapiProp, Restriction[] restrictions, int depth, bool needsConvertToSmartFilter)
		{
			QueryFilter[] array = new QueryFilter[restrictions.Length];
			for (int i = 0; i < restrictions.Length; i++)
			{
				array[i] = FilterRestrictionConverter.InternalCreateFilter(storeSession, timeZone, mapiProp, restrictions[i], depth, needsConvertToSmartFilter);
			}
			return array;
		}

		private static QueryFilter InternalCreateFilter(StoreSession storeSession, ExTimeZone timeZone, MapiProp mapiProp, Restriction restriction, int depth, bool needsConvertToSmartFilter)
		{
			if (depth >= 256)
			{
				throw new CorruptDataException(ServerStrings.ExFilterHierarchyIsTooDeep);
			}
			QueryFilter queryFilter;
			if (restriction is Restriction.AndRestriction)
			{
				Restriction.AndRestriction andRestriction = (Restriction.AndRestriction)restriction;
				queryFilter = new AndFilter(FilterRestrictionConverter.InternalCreateFilters(storeSession, timeZone, mapiProp, andRestriction.Restrictions, depth + 1, needsConvertToSmartFilter));
			}
			else if (restriction is Restriction.OrRestriction)
			{
				Restriction.OrRestriction orRestriction = (Restriction.OrRestriction)restriction;
				queryFilter = new OrFilter(FilterRestrictionConverter.InternalCreateFilters(storeSession, timeZone, mapiProp, orRestriction.Restrictions, depth + 1, needsConvertToSmartFilter));
			}
			else if (restriction is Restriction.NearRestriction)
			{
				Restriction.NearRestriction nearRestriction = (Restriction.NearRestriction)restriction;
				queryFilter = new NearFilter((uint)nearRestriction.Distance, nearRestriction.Ordered, (AndFilter)FilterRestrictionConverter.InternalCreateFilter(storeSession, timeZone, mapiProp, nearRestriction.Restriction, depth + 1, needsConvertToSmartFilter));
			}
			else if (restriction is Restriction.NotRestriction)
			{
				Restriction.NotRestriction notRestriction = (Restriction.NotRestriction)restriction;
				queryFilter = new NotFilter(FilterRestrictionConverter.InternalCreateFilter(storeSession, timeZone, mapiProp, notRestriction.Restriction, depth + 1, needsConvertToSmartFilter));
			}
			else if (restriction is Restriction.SubRestriction)
			{
				SubFilterProperties subFilterProperty;
				if (restriction is Restriction.AttachmentRestriction)
				{
					subFilterProperty = SubFilterProperties.Attachments;
				}
				else
				{
					if (!(restriction is Restriction.RecipientRestriction))
					{
						throw new NotSupportedException(ServerStrings.ExFilterNotSupported(restriction));
					}
					subFilterProperty = SubFilterProperties.Recipients;
				}
				Restriction.SubRestriction subRestriction = (Restriction.SubRestriction)restriction;
				queryFilter = new SubFilter(subFilterProperty, FilterRestrictionConverter.InternalCreateFilter(storeSession, timeZone, mapiProp, subRestriction.Restriction, depth + 1, needsConvertToSmartFilter));
			}
			else if (restriction is Restriction.BitMaskRestriction)
			{
				Restriction.BitMaskRestriction bitMaskRestriction = (Restriction.BitMaskRestriction)restriction;
				PropertyDefinition propertyDefinition = FilterRestrictionConverter.GetPropertyDefinition(storeSession, bitMaskRestriction.Tag);
				bool isNonZero = bitMaskRestriction.Bmr == Restriction.RelBmr.NotEqualToZero;
				queryFilter = new BitMaskFilter(propertyDefinition, (ulong)bitMaskRestriction.Mask, isNonZero);
			}
			else if (restriction is Restriction.PropertyRestriction)
			{
				bool flag = false;
				Restriction.PropertyRestriction propertyRestriction = (Restriction.PropertyRestriction)restriction;
				PropTag propTag = FilterRestrictionConverter.GetPropTagFromRestrictionPropTag(propertyRestriction.PropTag, propertyRestriction.MultiValued);
				if ((propTag & (PropTag)12288U) == (PropTag)12288U)
				{
					flag = true;
					propTag &= (PropTag)4294955007U;
					propTag |= (PropTag)4096U;
				}
				PropertyDefinition propertyDefinition2 = FilterRestrictionConverter.GetPropertyDefinition(storeSession, propTag);
				ComparisonOperator comparisonOperator = FilterRestrictionConverter.GetComparisonOperator(propertyRestriction.Op);
				object valueFromPropValue = MapiPropertyBag.GetValueFromPropValue(storeSession, timeZone, InternalSchema.ToStorePropertyDefinition(propertyDefinition2), propertyRestriction.PropValue);
				if (flag)
				{
					queryFilter = new MultivaluedInstanceComparisonFilter(comparisonOperator, propertyDefinition2, valueFromPropValue);
				}
				else
				{
					queryFilter = new ComparisonFilter(comparisonOperator, propertyDefinition2, valueFromPropValue);
				}
			}
			else if (restriction is Restriction.ContentRestriction)
			{
				Restriction.ContentRestriction contentRestriction = (Restriction.ContentRestriction)restriction;
				PropTag propTagFromRestrictionPropTag = FilterRestrictionConverter.GetPropTagFromRestrictionPropTag(contentRestriction.PropTag, contentRestriction.MultiValued);
				PropertyDefinition propertyDefinition3 = FilterRestrictionConverter.GetPropertyDefinition(storeSession, propTagFromRestrictionPropTag);
				ContentFlags flags = contentRestriction.Flags;
				MatchFlags matchFlags = FilterRestrictionConverter.CalculateMatchFlags(flags);
				MatchOptions matchOptions = FilterRestrictionConverter.CalculateMatchOptions(flags);
				string text = contentRestriction.PropValue.Value as string;
				if (text != null)
				{
					queryFilter = new TextFilter(propertyDefinition3, (string)contentRestriction.PropValue.Value, matchOptions, matchFlags);
				}
				else
				{
					queryFilter = new BinaryFilter(propertyDefinition3, (byte[])contentRestriction.PropValue.Value, matchOptions, matchFlags);
				}
			}
			else if (restriction is Restriction.ExistRestriction)
			{
				Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)restriction;
				PropertyDefinition propertyDefinition4 = FilterRestrictionConverter.GetPropertyDefinition(storeSession, existRestriction.Tag);
				queryFilter = new ExistsFilter(propertyDefinition4);
			}
			else if (restriction is Restriction.ComparePropertyRestriction)
			{
				Restriction.ComparePropertyRestriction comparePropertyRestriction = (Restriction.ComparePropertyRestriction)restriction;
				PropertyDefinition propertyDefinition5 = FilterRestrictionConverter.GetPropertyDefinition(storeSession, comparePropertyRestriction.TagLeft);
				PropertyDefinition propertyDefinition6 = FilterRestrictionConverter.GetPropertyDefinition(storeSession, comparePropertyRestriction.TagRight);
				ComparisonOperator comparisonOperator2 = FilterRestrictionConverter.GetComparisonOperator(comparePropertyRestriction.Op);
				queryFilter = new PropertyComparisonFilter(comparisonOperator2, propertyDefinition5, propertyDefinition6);
			}
			else if (restriction is Restriction.CommentRestriction)
			{
				Restriction.CommentRestriction commentRestriction = (Restriction.CommentRestriction)restriction;
				PropTag[] array = new PropTag[commentRestriction.Values.Length];
				object[] array2 = new object[commentRestriction.Values.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = commentRestriction.Values[i].PropTag;
					array2[i] = commentRestriction.Values[i].Value;
				}
				NativeStorePropertyDefinition[] properties = PropertyTagCache.Cache.SafePropertyDefinitionsFromPropTags(storeSession, array);
				queryFilter = new CommentFilter(properties, array2, FilterRestrictionConverter.InternalCreateFilter(storeSession, timeZone, mapiProp, commentRestriction.Restriction, depth + 1, needsConvertToSmartFilter));
			}
			else if (restriction is Restriction.CountRestriction)
			{
				Restriction.CountRestriction countRestriction = (Restriction.CountRestriction)restriction;
				queryFilter = new CountFilter((uint)countRestriction.Count, FilterRestrictionConverter.InternalCreateFilter(storeSession, timeZone, mapiProp, countRestriction.Restriction, depth + 1, needsConvertToSmartFilter));
			}
			else if (restriction is Restriction.SizeRestriction)
			{
				Restriction.SizeRestriction sizeRestriction = (Restriction.SizeRestriction)restriction;
				queryFilter = new SizeFilter(FilterRestrictionConverter.GetComparisonOperator(sizeRestriction.Op), FilterRestrictionConverter.GetPropertyDefinition(storeSession, sizeRestriction.Tag), (uint)sizeRestriction.Size);
			}
			else if (restriction is Restriction.TrueRestriction)
			{
				queryFilter = new TrueFilter();
			}
			else if (restriction is Restriction.FalseRestriction)
			{
				queryFilter = new FalseFilter();
			}
			else
			{
				if (restriction == null)
				{
					return new NullFilter();
				}
				throw new CorruptDataException(ServerStrings.ExUnknownRestrictionType);
			}
			if (!needsConvertToSmartFilter)
			{
				return queryFilter;
			}
			QueryFilter queryFilter2 = FilterRestrictionConverter.SmartPropertyCreateFilter(queryFilter);
			if (queryFilter2 != null)
			{
				return queryFilter2;
			}
			return queryFilter;
		}

		private static PropertyDefinition GetPropertyDefinition(StoreSession storeSession, PropTag propTag)
		{
			return PropertyTagCache.Cache.SafePropertyDefinitionsFromPropTags(storeSession, new PropTag[]
			{
				propTag
			})[0];
		}

		private static QueryFilter SmartPropertyCreateFilter(QueryFilter filter)
		{
			Type type = filter.GetType();
			List<SmartPropertyDefinition> list = null;
			if (FilterRestrictionConverter.registeredSmartProperties.TryGetValue(type, out list))
			{
				foreach (SmartPropertyDefinition smartPropertyDefinition in list)
				{
					QueryFilter queryFilter = smartPropertyDefinition.NativeFilterToSmartFilter(filter);
					if (queryFilter != null)
					{
						return queryFilter;
					}
				}
			}
			return null;
		}

		private static MatchFlags CalculateMatchFlags(ContentFlags mapiFlags)
		{
			MatchFlags matchFlags = MatchFlags.Default;
			if ((mapiFlags & ContentFlags.IgnoreCase) == ContentFlags.IgnoreCase)
			{
				matchFlags |= MatchFlags.IgnoreCase;
			}
			if ((mapiFlags & ContentFlags.IgnoreNonSpace) == ContentFlags.IgnoreNonSpace)
			{
				matchFlags |= MatchFlags.IgnoreNonSpace;
			}
			if ((mapiFlags & ContentFlags.Loose) == ContentFlags.Loose)
			{
				matchFlags |= MatchFlags.Loose;
			}
			return matchFlags;
		}

		private static MatchOptions CalculateMatchOptions(ContentFlags mapiFlags)
		{
			MatchOptions result = MatchOptions.FullString;
			if ((mapiFlags & ContentFlags.Prefix) == ContentFlags.Prefix)
			{
				result = MatchOptions.Prefix;
			}
			if ((mapiFlags & ContentFlags.SubString) == ContentFlags.SubString)
			{
				result = MatchOptions.SubString;
			}
			if ((mapiFlags & ContentFlags.PrefixOnWords) == ContentFlags.PrefixOnWords)
			{
				result = MatchOptions.PrefixOnWords;
			}
			if ((mapiFlags & ContentFlags.ExactPhrase) == ContentFlags.ExactPhrase)
			{
				result = MatchOptions.ExactPhrase;
			}
			return result;
		}

		private static PropTag GetPropTagFromRestrictionPropTag(PropTag restrictionPropTag, bool isRestrictionMultiValued)
		{
			PropTag result = restrictionPropTag;
			if (isRestrictionMultiValued)
			{
				result = (restrictionPropTag | (PropTag)4096U);
			}
			return result;
		}

		private const int MaxFilterHierarchyDepth = 256;

		private static Dictionary<Type, List<SmartPropertyDefinition>> registeredSmartProperties = new Dictionary<Type, List<SmartPropertyDefinition>>();
	}
}
