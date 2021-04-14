using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarRemoteParticipant : CalendarParticipant
	{
		internal CalendarRemoteParticipant(UserObject userObject, ExDateTime validateFrom, ExDateTime validateUntil, MailboxSession session, Uri endpoint) : base(userObject, validateFrom, validateUntil)
		{
			if (userObject.ExchangePrincipal == null)
			{
				throw new ArgumentNullException("userObject.ExchangePrincipal");
			}
			this.localSession = session;
			this.ExchangePrincipal = userObject.ExchangePrincipal;
			this.calendarConverter = new CalendarItemConverter();
			this.binding = new MeetingValidatorEwsBinding(this.ExchangePrincipal, endpoint);
		}

		internal ExchangePrincipal ExchangePrincipal { get; private set; }

		public override void Dispose()
		{
			if (this.binding != null)
			{
				this.binding.Dispose();
				this.binding = null;
			}
			base.Dispose();
		}

		internal override void ValidateMeetings(ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent, Action<long> onItemRepaired)
		{
			bool shouldProcessMailbox = CalendarParticipant.InternalShouldProcessMailbox(this.ExchangePrincipal);
			try
			{
				List<SearchExpressionType> list = new List<SearchExpressionType>();
				foreach (CalendarInstanceContext calendarInstanceContext in base.ItemList.Values)
				{
					calendarInstanceContext.ValidationContext.CalendarInstance = new CalendarRemoteItem(this.ExchangePrincipal, this.binding);
					calendarInstanceContext.ValidationContext.CalendarInstance.ShouldProcessMailbox = shouldProcessMailbox;
					GlobalObjectId globalObjectId = calendarInstanceContext.ValidationContext.BaseItem.GlobalObjectId;
					string value = Convert.ToBase64String(globalObjectId.CleanGlobalObjectIdBytes);
					SearchExpressionType item = new IsEqualToType
					{
						Item = CalendarItemFields.CleanGlobalObjectIdProp,
						FieldURIOrConstant = new FieldURIOrConstantType
						{
							Item = new ConstantValueType
							{
								Value = value
							}
						}
					};
					list.Add(item);
				}
				ItemType[] remoteCalendarItems = this.GetRemoteCalendarItems(list);
				if (remoteCalendarItems != null)
				{
					Dictionary<GlobalObjectId, CalendarItemType> dictionary = new Dictionary<GlobalObjectId, CalendarItemType>();
					foreach (ItemType itemType in remoteCalendarItems)
					{
						CalendarItemType calendarItemType = itemType as CalendarItemType;
						GlobalObjectId globalObjectId2 = CalendarItemFields.GetGlobalObjectId(calendarItemType);
						dictionary.Add(globalObjectId2, calendarItemType);
					}
					foreach (KeyValuePair<GlobalObjectId, CalendarInstanceContext> keyValuePair in base.ItemList)
					{
						if (dictionary.ContainsKey(keyValuePair.Key))
						{
							CalendarItemType remoteItem = dictionary[keyValuePair.Key];
							CalendarInstanceContext value2 = keyValuePair.Value;
							try
							{
								try
								{
									CalendarItemBase calendarItemBase = CalendarItem.Create(this.localSession, this.localSession.GetDefaultFolderId(DefaultFolderType.Calendar));
									Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "Converting the EWS item to XSO.");
									this.calendarConverter.ConvertItem(calendarItemBase, remoteItem);
									value2.ValidationContext.OppositeItem = calendarItemBase;
								}
								catch (FormatException ex)
								{
									string text = string.Format("Could not convert the remote item, exception = {0}", ex.GetType());
									Globals.ConsistencyChecksTracer.TraceError((long)this.GetHashCode(), text);
									value2.ValidationContext.CalendarInstance.LoadInconsistency = Inconsistency.CreateInstance(value2.ValidationContext.OppositeRole, text, CalendarInconsistencyFlag.StorageException, value2.ValidationContext);
								}
								catch (CorruptDataException ex2)
								{
									string text2 = string.Format("Could not convert the remote item, exception = {0}", ex2.GetType());
									Globals.ConsistencyChecksTracer.TraceError((long)this.GetHashCode(), text2);
									value2.ValidationContext.CalendarInstance.LoadInconsistency = Inconsistency.CreateInstance(value2.ValidationContext.OppositeRole, text2, CalendarInconsistencyFlag.StorageException, value2.ValidationContext);
								}
								catch (StorageTransientException ex3)
								{
									string text3 = string.Format("Could not convert the remote item, exception = {0}", ex3.GetType());
									Globals.ConsistencyChecksTracer.TraceError((long)this.GetHashCode(), text3);
									value2.ValidationContext.CalendarInstance.LoadInconsistency = Inconsistency.CreateInstance(value2.ValidationContext.OppositeRole, text3, CalendarInconsistencyFlag.StorageException, value2.ValidationContext);
								}
								continue;
							}
							finally
							{
								base.ValidateInstance(value2, organizerRumsSent, onItemRepaired);
								if (value2.ValidationContext.OppositeItem != null)
								{
									value2.ValidationContext.OppositeItem.Dispose();
									value2.ValidationContext.OppositeItem = null;
								}
							}
						}
						Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetItem didn't return an expected GlobalObjectId.");
					}
				}
				foreach (CalendarInstanceContext calendarInstanceContext2 in base.ItemList.Values)
				{
					if (!calendarInstanceContext2.IsValidationDone)
					{
						if (calendarInstanceContext2.ValidationContext.OppositeRole == RoleType.Organizer && calendarInstanceContext2.ValidationContext.OppositeItem == null)
						{
							calendarInstanceContext2.ValidationContext.OppositeRoleOrganizerIsValid = true;
						}
						base.ValidateInstance(calendarInstanceContext2, organizerRumsSent, onItemRepaired);
					}
				}
			}
			catch (ProtocolViolationException exception)
			{
				this.HandleRemoteException(exception);
			}
			catch (SecurityException exception2)
			{
				this.HandleRemoteException(exception2);
			}
			catch (ArgumentException exception3)
			{
				this.HandleRemoteException(exception3);
			}
			catch (InvalidOperationException exception4)
			{
				this.HandleRemoteException(exception4);
			}
			catch (NotSupportedException exception5)
			{
				this.HandleRemoteException(exception5);
			}
			catch (XmlException exception6)
			{
				this.HandleRemoteException(exception6);
			}
			catch (XPathException exception7)
			{
				this.HandleRemoteException(exception7);
			}
			catch (SoapException exception8)
			{
				this.HandleRemoteException(exception8);
			}
			catch (IOException exception9)
			{
				this.HandleRemoteException(exception9);
			}
		}

		private ItemType[] GetRemoteCalendarItems(List<SearchExpressionType> searchItems)
		{
			FindItemType findItemType = new FindItemType();
			findItemType.ItemShape = CalendarItemFields.CalendarQueryShape;
			findItemType.ParentFolderIds = new BaseFolderIdType[]
			{
				new DistinguishedFolderIdType
				{
					Id = DistinguishedFolderIdNameType.calendar
				}
			};
			findItemType.Restriction = new RestrictionType
			{
				Item = new OrType
				{
					Items = searchItems.ToArray()
				}
			};
			FindItemResponseType response = this.binding.FindItem(findItemType);
			ItemType[] array = this.HandleFindItemResponse(response);
			if (array == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "FindItem returned NULL ArrayOfRealItemsType.");
				return null;
			}
			List<ItemIdType> list = new List<ItemIdType>();
			foreach (ItemType itemType in array)
			{
				if (itemType is CalendarItemType)
				{
					CalendarItemType calendarItemType = itemType as CalendarItemType;
					if (calendarItemType.CalendarItemType1 == CalendarItemTypeType.Single)
					{
						list.Add(itemType.ItemId);
					}
					else
					{
						OccurrencesRangeType occurrencesRangeType = new OccurrencesRangeType
						{
							Start = base.ValidateFrom.UniversalTime,
							StartSpecified = true,
							End = base.ValidateUntil.UniversalTime,
							EndSpecified = true,
							Count = 100,
							CountSpecified = true
						};
						RecurringMasterItemIdRangesType item = new RecurringMasterItemIdRangesType
						{
							Id = itemType.ItemId.Id,
							ChangeKey = itemType.ItemId.ChangeKey,
							Ranges = new OccurrencesRangeType[]
							{
								occurrencesRangeType
							}
						};
						list.Add(item);
					}
				}
				else
				{
					Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "FindItem returned an item which is not a CalendarItemType. Skipping it.");
				}
			}
			if (list.Count < 1)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "FindItem didn't return valid items.");
				return null;
			}
			GetItemType getItem = new GetItemType
			{
				ItemShape = CalendarItemFields.CalendarItemShape,
				ItemIds = list.ToArray()
			};
			GetItemResponseType item2 = this.binding.GetItem(getItem);
			ItemType[] array3 = this.HandleGetItemResponse(item2);
			if (array3 == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetItem returned NULL ItemType[].");
			}
			return array3;
		}

		private ResponseMessageType[] HandleBaseResponseMessage(BaseResponseMessageType response)
		{
			if (response.ResponseMessages == null || response.ResponseMessages.Items == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "Web request returned NULL ResponseMessages.");
				return null;
			}
			ResponseMessageType[] items = response.ResponseMessages.Items;
			if (items == null || items.Length < 1)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "Web request returned NULL ResponseMessageType.");
				return null;
			}
			return items;
		}

		private ItemType[] HandleFindItemResponse(BaseResponseMessageType response)
		{
			ResponseMessageType[] array = this.HandleBaseResponseMessage(response);
			if (array == null || array[0] == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "FindItem returned NULL responseType.");
				return null;
			}
			ResponseMessageType responseMessageType = array[0];
			if (responseMessageType.ResponseCode != ResponseCodeType.NoError)
			{
				Globals.ConsistencyChecksTracer.TraceDebug<ResponseCodeType>((long)this.GetHashCode(), "Web request returned ResponseCodeType {0}.", responseMessageType.ResponseCode);
				return null;
			}
			FindItemResponseMessageType findItemResponseMessageType = responseMessageType as FindItemResponseMessageType;
			if (findItemResponseMessageType == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "FindItem web request returned NULL FindItemResponseMessageType.");
				return null;
			}
			if (findItemResponseMessageType.RootFolder == null || findItemResponseMessageType.RootFolder.Item == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "FindItem web request returned NULL RootFolder.");
				return null;
			}
			ArrayOfRealItemsType arrayOfRealItemsType = findItemResponseMessageType.RootFolder.Item as ArrayOfRealItemsType;
			if (arrayOfRealItemsType == null || arrayOfRealItemsType.Items == null || arrayOfRealItemsType.Items[0] == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "FindItem web request returned NULL ItemType.");
				return null;
			}
			return arrayOfRealItemsType.Items;
		}

		private ItemType[] HandleGetItemResponse(BaseResponseMessageType response)
		{
			ResponseMessageType[] array = this.HandleBaseResponseMessage(response);
			if (array == null)
			{
				Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetItem returned NULL responseType.");
				return null;
			}
			List<ItemType> list = new List<ItemType>();
			foreach (ResponseMessageType responseMessageType in array)
			{
				if (responseMessageType.ResponseCode != ResponseCodeType.NoError)
				{
					Globals.ConsistencyChecksTracer.TraceDebug<ResponseCodeType>((long)this.GetHashCode(), "Web request returned ResponseCodeType {0}.", responseMessageType.ResponseCode);
				}
				else
				{
					ItemInfoResponseMessageType itemInfoResponseMessageType = responseMessageType as ItemInfoResponseMessageType;
					if (itemInfoResponseMessageType == null || itemInfoResponseMessageType.Items == null || itemInfoResponseMessageType.Items.Items == null)
					{
						Globals.ConsistencyChecksTracer.TraceDebug((long)this.GetHashCode(), "GetItem web request returned NULL ItemType.");
					}
					else
					{
						list.AddRange(itemInfoResponseMessageType.Items.Items);
					}
				}
			}
			return list.ToArray();
		}

		private void HandleRemoteException(Exception exception)
		{
			Globals.ConsistencyChecksTracer.TraceError<Exception, SmtpAddress>((long)this.GetHashCode(), "{0}: Could not access remote server to open mailbox {1}.", exception, this.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress);
			Globals.CalendarRepairLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_ErrorAccessingRemoteMailbox, this.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), new object[]
			{
				this.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(),
				exception
			});
		}

		private MailboxSession localSession;

		private MeetingValidatorEwsBinding binding;

		private CalendarItemConverter calendarConverter;
	}
}
