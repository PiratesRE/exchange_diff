using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetItem : MultiStepServiceCommand<GetItemRequest, ItemType[]>, IDisposable
	{
		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.SharingRead;
			}
		}

		public GetItem(CallContext callContext, GetItemRequest request) : base(callContext, request)
		{
			this.itemIds = base.Request.Ids;
			this.responseShape = Global.ResponseShapeResolver.GetResponseShape<ItemResponseShape>(base.Request.ShapeName, base.Request.ItemShape, base.CallContext.FeaturesManager);
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseItemId>(this.itemIds, "itemIds", "GetItem::PreExecuteCommand");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "this.responseShape", "GetItem::PreExecuteCommand");
			RequestDetailsLogger.Current.AppendGenericInfo("Count", this.itemIds.Count);
			if (request.PrefetchItems)
			{
				RequestDetailsLogger.Current.AppendGenericInfo("Prefetch", "1");
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal override ServiceResult<ItemType[]> Execute()
		{
			ServiceResult<ItemType[]> serviceResult = null;
			GrayException.MapAndReportGrayExceptions(delegate()
			{
				ServiceError serviceError = null;
				Item item = null;
				try
				{
					IdAndSession idAndSession = this.IdConverter.ConvertItemIdToIdAndSessionReadOnly(this.itemIds[this.CurrentStep], BasicTypes.Item, false, ref item);
					if (this.CurrentStep == 0 && this.Request.PrefetchItems)
					{
						this.PrefetchItems(idAndSession, this.Request.PrefetchItemStoreIds);
					}
					List<ItemType> list = new List<ItemType>(1);
					list.Add(this.GetItemObject(idAndSession, ref item, out serviceError));
					if (this.itemIds[this.CurrentStep] is RecurringMasterItemIdRanges && serviceError == null)
					{
						this.EnumerateAndGetOccurrences((RecurringMasterItemIdRanges)this.itemIds[this.CurrentStep], item, out serviceError, list);
					}
					this.objectsChanged++;
					ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2969972029U, list[0].ItemId.Id);
					if (serviceError == null)
					{
						serviceResult = new ServiceResult<ItemType[]>(list.ToArray());
					}
					else
					{
						serviceResult = new ServiceResult<ItemType[]>(list.ToArray(), serviceError);
					}
				}
				finally
				{
					if (item != null)
					{
						item.Dispose();
					}
				}
			}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			return serviceResult;
		}

		private static void FindMinMax(RecurringMasterItemIdRanges.OccurrencesRange[] ranges, out ExDateTime minStart, out ExDateTime maxEnd)
		{
			minStart = ExDateTime.MaxValue;
			maxEnd = ExDateTime.MinValue;
			if (ranges != null && ranges.Length > 0)
			{
				foreach (RecurringMasterItemIdRanges.OccurrencesRange occurrencesRange in ranges)
				{
					if (occurrencesRange.Count > 0)
					{
						if (minStart > occurrencesRange.StartExDateTime)
						{
							minStart = occurrencesRange.StartExDateTime;
						}
						if (maxEnd < occurrencesRange.EndExDateTime)
						{
							maxEnd = occurrencesRange.EndExDateTime;
						}
					}
				}
			}
		}

		private bool IsAtLeastOneValidRange(RecurringMasterItemIdRanges.OccurrencesRange[] ranges)
		{
			bool result = false;
			foreach (RecurringMasterItemIdRanges.OccurrencesRange occurrencesRange in ranges)
			{
				if (occurrencesRange.StartExDateTime.CompareTo(occurrencesRange.EndExDateTime) <= 0)
				{
					result = true;
				}
				else
				{
					occurrencesRange.Count = 0;
				}
			}
			return result;
		}

		private bool IsOccurrenceInRanges(OccurrenceInfo occurrence, RecurringMasterItemIdRanges.OccurrencesRange[] ranges)
		{
			bool flag = false;
			foreach (RecurringMasterItemIdRanges.OccurrencesRange occurrencesRange in ranges)
			{
				if (occurrencesRange.Count > 0)
				{
					bool flag2;
					if (occurrencesRange.CompareOriginalStartTime)
					{
						flag2 = (occurrencesRange.StartExDateTime.CompareTo(occurrence.OriginalStartTime) <= 0 && occurrencesRange.EndExDateTime.CompareTo(occurrence.OriginalStartTime) >= 0);
					}
					else
					{
						flag2 = (occurrencesRange.StartExDateTime.CompareTo(occurrence.EndTime) <= 0 && occurrencesRange.EndExDateTime.CompareTo(occurrence.StartTime) >= 0);
					}
					if (flag2)
					{
						occurrencesRange.Count--;
					}
					flag = (flag || flag2);
				}
			}
			return flag;
		}

		private void EnumerateAndGetOccurrences(RecurringMasterItemIdRanges rangesInfo, Item recurringMaster, out ServiceError warning, List<ItemType> itemsList)
		{
			warning = null;
			CalendarItem calendarItem = recurringMaster as CalendarItem;
			if (calendarItem == null || calendarItem.Recurrence == null)
			{
				return;
			}
			RecurringMasterItemIdRanges.OccurrencesRange[] ranges;
			Func<IList<OccurrenceInfo>> getOccurrences;
			if (rangesInfo.Ranges != null)
			{
				ranges = rangesInfo.Ranges;
				getOccurrences = delegate()
				{
					ExDateTime startView;
					ExDateTime endView;
					GetItem.FindMinMax(ranges, out startView, out endView);
					return calendarItem.Recurrence.GetOccurrenceInfoList(startView, endView, 732);
				};
			}
			else
			{
				if (rangesInfo.ExpandAroundDateOccurrenceRange == null)
				{
					return;
				}
				ranges = new RecurringMasterItemIdRanges.OccurrencesRange[]
				{
					rangesInfo.ExpandAroundDateOccurrenceRange
				};
				getOccurrences = delegate()
				{
					RecurringMasterItemIdRanges.ExpandAroundDateOccurrenceRangeType expandAroundDateOccurrenceRange = rangesInfo.ExpandAroundDateOccurrenceRange;
					int count = Math.Min(expandAroundDateOccurrenceRange.Count, 732);
					return calendarItem.Recurrence.GetRecentOccurrencesInfoList(expandAroundDateOccurrenceRange.ExpandOccurrencesAroundExDateTime.Value, count);
				};
			}
			if (ranges == null || ranges.Length == 0 || !this.IsAtLeastOneValidRange(ranges))
			{
				return;
			}
			warning = this.AddFilteredOccurrences(itemsList, ranges, getOccurrences, calendarItem);
		}

		private ServiceError AddFilteredOccurrences(List<ItemType> itemsList, RecurringMasterItemIdRanges.OccurrencesRange[] ranges, Func<IList<OccurrenceInfo>> getOccurrences, CalendarItem calendarItem)
		{
			ServiceError serviceError = null;
			IList<OccurrenceInfo> list = getOccurrences();
			foreach (OccurrenceInfo occurrenceInfo in list)
			{
				if (this.IsOccurrenceInRanges(occurrenceInfo, ranges))
				{
					Item item = null;
					try
					{
						item = calendarItem.OpenOccurrence(occurrenceInfo.VersionedId.ObjectId, null);
						IdAndSession idAndSession = IdAndSession.CreateFromItem(item);
						itemsList.Add(this.GetItemObject(idAndSession, ref item, out serviceError));
						if (serviceError != null)
						{
							return serviceError;
						}
					}
					finally
					{
						if (item != null)
						{
							item.Dispose();
						}
					}
				}
			}
			return null;
		}

		internal override int StepCount
		{
			get
			{
				return this.itemIds.Count;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetItemResponse getItemResponse = new GetItemResponse();
			getItemResponse.BuildForResults<ItemType[]>(base.Results);
			return getItemResponse;
		}

		private ItemType GetItemObject(IdAndSession idAndSession, ref Item xsoItem, out ServiceError warning)
		{
			warning = null;
			base.CallContext.AuthZBehavior.OnGetItem(idAndSession.GetAsStoreObjectId());
			ItemResponseShape itemResponseShape = null;
			if (base.CallContext.IsExternalUser)
			{
				warning = ExternalUserHandler.CheckAndGetResponseShape(base.GetType(), idAndSession as ExternalUserIdAndSession, this.responseShape, out itemResponseShape);
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3238407485U, this.itemIds[base.CurrentStep].GetId());
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(idAndSession.Id, idAndSession.Session, (itemResponseShape == null) ? this.responseShape : itemResponseShape, base.ParticipantResolver);
			PropertyDefinition[] array = toServiceObjectPropertyList.GetPropertyDefinitions();
			if (!((IList)array).Contains(ItemSchema.Size))
			{
				PropertyDefinition[] array2 = new PropertyDefinition[array.Length + 1];
				array2[0] = ItemSchema.Size;
				array.CopyTo(array2, 1);
				array = array2;
			}
			if (xsoItem == null)
			{
				xsoItem = idAndSession.GetRootXsoItem(array);
			}
			else
			{
				xsoItem.Load(array);
			}
			if (base.CallContext.IsExternalUser)
			{
				List<PropertyPath> allowedProperties = ExternalUserHandler.GetAllowedProperties(idAndSession as ExternalUserIdAndSession, xsoItem);
				if (allowedProperties != null)
				{
					toServiceObjectPropertyList.FilterProperties(allowedProperties);
				}
			}
			else if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) && xsoItem is CalendarItemBase && xsoItem.Session is MailboxSession && ((CalendarItemBase)xsoItem).Sensitivity != Sensitivity.Normal && ((MailboxSession)xsoItem.Session).ShouldFilterPrivateItems)
			{
				toServiceObjectPropertyList.FilterProperties(ExternalUserCalendarResponseShape.CalendarPropertiesPrivateItemWithSubject);
			}
			this.SuperSizeCheck(xsoItem);
			toServiceObjectPropertyList.CharBuffer = this.charBuffer;
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
			ItemType itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
			if (IrmUtils.IsIrmEnabled(this.responseShape.ClientSupportsIrm, idAndSession.Session))
			{
				IrmUtils.DecodeIrmMessage(idAndSession.Session, xsoItem, true);
			}
			if (idAndSession.Session is PublicFolderSession && ClientInfo.OWA.IsMatch(idAndSession.Session.ClientInfoString))
			{
				toServiceObjectPropertyList.CommandOptions |= CommandOptions.ConvertParentFolderIdToPublicFolderId;
			}
			ServiceCommandBase.LoadServiceObject(itemType, xsoItem, idAndSession, this.responseShape, toServiceObjectPropertyList);
			bool flag = !string.IsNullOrWhiteSpace(itemType.Preview);
			if (flag && IrmUtils.IsMessageRestrictedAndDecoded(xsoItem))
			{
				itemType.Preview = IrmUtils.GetItemPreview(xsoItem);
			}
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				EWSSettings.SetInlineAttachmentFlags(itemType);
			}
			return itemType;
		}

		private void SuperSizeCheck(Item xsoItem)
		{
			if (!this.responseShape.IncludeMimeContent && !this.ResponseShapeIncludes(PropertyUriEnum.MimeContent) && !this.ResponseShapeIncludes(PropertyUriEnum.MimeContentUTF8))
			{
				return;
			}
			uint num = 0U;
			try
			{
				object obj = xsoItem[ItemSchema.Size];
				if (obj is PropertyError)
				{
					ExTraceGlobals.GetItemCallTracer.TraceDebug<int>((long)this.GetHashCode(), "[GetItem:SuperSizeCheck] Hashcode: {0}. Item size not found", this.GetHashCode());
				}
				else
				{
					num = (uint)((int)obj);
				}
			}
			catch (NotInBagPropertyErrorException)
			{
				ExTraceGlobals.GetItemCallTracer.TraceDebug<int>((long)this.GetHashCode(), "[GetItem:SuperSizeCheck] Hashcode: {0}. Item size not in property bag", this.GetHashCode());
			}
			if (this.ResponseShapeIncludes(PropertyUriEnum.MimeContent) && this.ResponseShapeIncludes(PropertyUriEnum.MimeContentUTF8))
			{
				num *= 2U;
			}
			if ((ulong)num > (ulong)((long)Global.GetAttachmentSizeLimit))
			{
				throw new MessageTooBigException(CoreResources.ErrorMessageSizeExceeded, null);
			}
		}

		private bool ResponseShapeIncludes(PropertyUriEnum propertyUri)
		{
			if (this.responseShape.AdditionalProperties == null)
			{
				return false;
			}
			foreach (PropertyPath propertyPath in this.responseShape.AdditionalProperties)
			{
				PropertyUri propertyUri2 = propertyPath as PropertyUri;
				if (propertyUri2 != null && propertyUri2.Uri == propertyUri)
				{
					return true;
				}
			}
			return false;
		}

		private void PrefetchItems(IdAndSession idAndSession, List<StoreId> itemIds)
		{
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			if (mailboxSession != null)
			{
				mailboxSession.PrereadMessages(itemIds.ToArray());
			}
		}

		private void Dispose(bool isDisposing)
		{
			ExTraceGlobals.GetItemCallTracer.TraceDebug<int, bool, bool>((long)this.GetHashCode(), "[GetItem:Dispose(bool)] Hashcode: {0}. IsDisposing: {1}, Already Disposed: {2}", this.GetHashCode(), isDisposing, this.isDisposed);
			if (!this.isDisposed)
			{
				if (isDisposing)
				{
					this.charBuffer = null;
				}
				this.isDisposed = true;
			}
		}

		internal const int BufferSize = 32768;

		private IList<BaseItemId> itemIds;

		private ItemResponseShape responseShape;

		private bool isDisposed;

		private char[] charBuffer = new char[32768];
	}
}
