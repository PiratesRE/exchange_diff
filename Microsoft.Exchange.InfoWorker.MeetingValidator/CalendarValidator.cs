using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Assistants;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal class CalendarValidator
	{
		public event Action<long> OnItemInspected;

		public event Action<long> OnItemRepaired;

		private CalendarValidator()
		{
		}

		internal static CalendarValidator CreateRepairingInstance(MailboxSession mailboxSession, ExDateTime rangeStart, ExDateTime rangeEnd, CalendarRepairPolicy repairPolicy, TimeSpan cvsTimeout)
		{
			if (CalendarItemBase.IsTenantToBeFixed(mailboxSession))
			{
				rangeStart = new ExDateTime(ExTimeZone.UtcTimeZone, new DateTime(2014, 8, 4));
				rangeEnd = new ExDateTime(ExTimeZone.UtcTimeZone, new DateTime(2014, 9, 16));
			}
			CalendarValidator calendarValidator = new CalendarValidator();
			ADSessionSettings adsessionSettings = mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings();
			calendarValidator.InitializeRangeValidation(mailboxSession, mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), rangeStart, rangeEnd, repairPolicy, null, null, adsessionSettings.CurrentOrganizationId, adsessionSettings.RootOrgId, cvsTimeout);
			return calendarValidator;
		}

		internal static CalendarValidator CreateRangeValidatingInstance(string mailboxUserAddress, OrganizationId currentOrganizationId, ADObjectId rootOrgContainerId, ExDateTime rangeStart, ExDateTime rangeEnd, string locationFilter, string subjectFilter)
		{
			CalendarValidator calendarValidator = new CalendarValidator();
			calendarValidator.InitializeRangeValidation(null, mailboxUserAddress, rangeStart, rangeEnd, null, locationFilter, subjectFilter, currentOrganizationId, rootOrgContainerId, CalendarVersionStoreQueryPolicy.DefaultWaitTimeForPopulation);
			return calendarValidator;
		}

		internal static CalendarValidator CreateMeetingSpecificValidatingInstance(string mailboxUserAddress, OrganizationId currentOrganizationId, ADObjectId rootOrgContainerId, StoreId meetingId)
		{
			CalendarValidator calendarValidator = new CalendarValidator();
			calendarValidator.meetingToValidate = meetingId;
			calendarValidator.Initialize(null, mailboxUserAddress, false, null, currentOrganizationId, rootOrgContainerId, CalendarVersionStoreQueryPolicy.DefaultWaitTimeForPopulation);
			return calendarValidator;
		}

		internal static bool IsValidParticipant(Participant participant)
		{
			return !(participant == null) && participant.ValidationStatus == ParticipantValidationStatus.NoError && !(participant.RoutingType == null) && !(participant.RoutingType == "MAPIPDL") && !string.IsNullOrEmpty(participant.EmailAddress);
		}

		private static List<MeetingData> GetDuplicates(MeetingData meeting, List<MeetingData> meetings)
		{
			if (meetings.Count <= 1)
			{
				return null;
			}
			return meetings.FindAll((MeetingData meetingToCheck) => meeting.CalendarItemType == meetingToCheck.CalendarItemType && meeting.GlobalObjectId != null && meetingToCheck.GlobalObjectId != null && GlobalObjectId.Equals(meetingToCheck.GlobalObjectId, meeting.GlobalObjectId));
		}

		internal List<MeetingValidationResult> Run()
		{
			CalendarValidator.<>c__DisplayClass6 CS$<>8__locals1 = new CalendarValidator.<>c__DisplayClass6();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.results = new List<MeetingValidationResult>();
			using (SessionManager sessionManager = this.independentSession ? new SessionManager(this.mailboxUser.ExchangePrincipal, "Client=ResourceHealth;Action=Meeting Validator") : new SessionManager(this.mailboxSession))
			{
				if (sessionManager.PrimarySession.Session.GetDefaultFolderId(DefaultFolderType.Calendar) == null)
				{
					CS$<>8__locals1.meetings = new List<MeetingData>();
				}
				else
				{
					using (CalendarFolder calendarFolder = CalendarFolder.Bind(sessionManager.PrimarySession.Session, DefaultFolderType.Calendar))
					{
						if (this.validatingRange)
						{
							CS$<>8__locals1.meetings = CalendarQuery.GetMeetingsInRange(sessionManager.PrimarySession.Session, this.mailboxUser, calendarFolder, this.rangeStart, this.rangeEnd, this.locationToFilterWith, this.subjectToFilterWith, CS$<>8__locals1.results);
						}
						else
						{
							CS$<>8__locals1.meetings = CalendarQuery.GetCorrelatedMeetings(sessionManager.PrimarySession.Session, this.mailboxUser, this.meetingToValidate);
						}
						if (CS$<>8__locals1.meetings.Count > 0)
						{
							try
							{
								CS$<>8__locals1.meetings.Sort();
								if (CS$<>8__locals1.meetings.Count > Configuration.MaxMeetingsToProcessIncludingDuplicates)
								{
									CS$<>8__locals1.meetings = CS$<>8__locals1.meetings.GetRange(0, Configuration.MaxMeetingsToProcessIncludingDuplicates);
								}
								Util.CoreCatchMeIfYouCan(delegate
								{
									CS$<>8__locals1.results.AddRange(CS$<>8__locals1.<>4__this.RemoveDuplicates(sessionManager.PrimarySession.Session, CS$<>8__locals1.meetings));
								}, "CalendarValidator");
							}
							catch (GrayException arg)
							{
								Globals.ValidatorTracer.TraceError<string, GrayException>((long)this.GetHashCode(), "CalendarValidator.Run - Failed to remove duplicates for (Owner: '{0}'; Exception: {1}", sessionManager.PrimarySession.Session.DisplayName, arg);
							}
						}
						if (CS$<>8__locals1.meetings.Count > Configuration.MaxMeetingsPerMailbox)
						{
							CS$<>8__locals1.meetings = CS$<>8__locals1.meetings.GetRange(0, Configuration.MaxMeetingsPerMailbox);
						}
						int i = 0;
						while (i < CS$<>8__locals1.meetings.Count)
						{
							int num = CS$<>8__locals1.meetings.Count - i;
							List<MeetingData> range;
							if (num > Configuration.MaxNumberOfLocalMeetingsPerBatch)
							{
								range = CS$<>8__locals1.meetings.GetRange(i, Configuration.MaxNumberOfLocalMeetingsPerBatch);
							}
							else
							{
								range = CS$<>8__locals1.meetings.GetRange(i, num);
							}
							i += range.Count;
							try
							{
								using (List<MeetingData>.Enumerator enumerator = range.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										CalendarValidator.<>c__DisplayClassb CS$<>8__locals3 = new CalendarValidator.<>c__DisplayClassb();
										CS$<>8__locals3.meeting = enumerator.Current;
										StoreId meetingId = CS$<>8__locals3.meeting.Id;
										MeetingValidationResult result = null;
										try
										{
											Util.CoreCatchMeIfYouCan(delegate
											{
												result = CS$<>8__locals1.<>4__this.ProcessMeetingId(meetingId, sessionManager, CS$<>8__locals3.meeting.HasDuplicates);
											}, "CalendarValidator");
										}
										catch (GrayException arg2)
										{
											Globals.ValidatorTracer.TraceError<string, StoreId, GrayException>((long)this.GetHashCode(), "CalendarValidator.Run - Failed to process (Owner: '{0}'; ID: {1}). Exception: {2}", sessionManager.PrimarySession.Session.DisplayName, meetingId, arg2);
										}
										if (result != null)
										{
											CS$<>8__locals1.results.Add(result);
										}
									}
								}
								using (Dictionary<UserObject, CalendarParticipant>.ValueCollection.Enumerator enumerator2 = this.calendarParticipantList.Values.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										CalendarParticipant calendarParticipant = enumerator2.Current;
										try
										{
											Util.CoreCatchMeIfYouCan(delegate
											{
												calendarParticipant.ValidateMeetings(ref CS$<>8__locals1.<>4__this.organizerRumsSent, CS$<>8__locals1.<>4__this.OnItemRepaired);
											}, "CalendarValidator");
										}
										catch (GrayException arg3)
										{
											Globals.ValidatorTracer.TraceError<string, GrayException>((long)this.GetHashCode(), "CalendarValidator.Run - Failed to process (Owner: '{0}'. Exception: {1}", sessionManager.PrimarySession.Session.DisplayName, arg3);
										}
									}
								}
							}
							finally
							{
								foreach (CalendarParticipant calendarParticipant2 in this.calendarParticipantList.Values)
								{
									calendarParticipant2.Dispose();
								}
								this.calendarParticipantList.Clear();
								this.inaccessibleMailboxes.Clear();
							}
						}
					}
				}
			}
			return CS$<>8__locals1.results;
		}

		private MeetingValidationResult ProcessMeetingId(StoreId meetingId, SessionManager sessionManager, bool hasDuplicates)
		{
			MeetingValidationResult result = null;
			CalendarItemBase calendarItemBase = null;
			this.isCalendarItemOnParticipantList = false;
			bool flag = false;
			try
			{
				calendarItemBase = CalendarItemBase.Bind(sessionManager.PrimarySession.Session, meetingId, CalendarQuery.CalendarQueryProps);
				string errorDescription;
				if (calendarItemBase == null)
				{
					result = new MeetingValidationResult(this.rangeStart, this.rangeEnd, this.mailboxUser, MeetingData.CreateInstance(this.mailboxUser, meetingId), false, "CalendarValidator.Run(): CalItem is null.");
				}
				else if (CalendarQuery.ArePropertiesValid(calendarItemBase, out errorDescription))
				{
					flag = true;
				}
				else
				{
					result = new MeetingValidationResult(this.rangeStart, this.rangeEnd, this.mailboxUser, MeetingData.CreateInstance(this.mailboxUser, meetingId), false, errorDescription);
					Globals.ValidatorTracer.TraceError<StoreId>((long)this.GetHashCode(), "Could not get properties for meeting '{0}'", meetingId);
				}
			}
			catch (ObjectNotFoundException ex)
			{
				result = new MeetingValidationResult(this.rangeStart, this.rangeEnd, this.mailboxUser, MeetingData.CreateInstance(this.mailboxUser, meetingId), false, "Could not find item.\n" + ex.ToString());
				Globals.ValidatorTracer.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "Could not find item, exception = {0}", ex);
			}
			catch (ArgumentException arg)
			{
				string errorString = string.Format("Could not bind to item as CalendarItemBase  Exception: {0}", arg);
				result = CalendarQuery.GetMVResultWithMoreCalItemData(sessionManager.PrimarySession.Session, this.mailboxUser, meetingId, errorString, null, this.rangeStart, this.rangeEnd);
				Globals.ValidatorTracer.TraceError<ArgumentException>((long)this.GetHashCode(), "ArgumentException occurred while processing attendee meeting, exception = {0}", arg);
			}
			catch (CorruptDataException arg2)
			{
				string errorString2 = string.Format("Could not bind to item as CalendarItemBase  Exception: {0}", arg2);
				result = CalendarQuery.GetMVResultWithMoreCalItemData(sessionManager.PrimarySession.Session, this.mailboxUser, meetingId, errorString2, null, this.rangeStart, this.rangeEnd);
				Globals.ValidatorTracer.TraceError<CorruptDataException>((long)this.GetHashCode(), "CorruptDataException occurred while processing this meeting, exception = {0}", arg2);
			}
			catch (ConversionFailedException arg3)
			{
				string errorString3 = string.Format("Could not bind to item as CalendarItemBase  Exception: {0}", arg3);
				result = CalendarQuery.GetMVResultWithMoreCalItemData(sessionManager.PrimarySession.Session, this.mailboxUser, meetingId, errorString3, null, this.rangeStart, this.rangeEnd);
				Globals.ValidatorTracer.TraceError<ConversionFailedException>((long)this.GetHashCode(), "ConversionFailedException occurred while processing this meeting, exception = {0}", arg3);
			}
			try
			{
				if (flag)
				{
					result = this.ValidateMeeting(sessionManager, calendarItemBase, this.rangeStart, this.rangeEnd, hasDuplicates);
				}
			}
			finally
			{
				if (calendarItemBase != null && !this.isCalendarItemOnParticipantList)
				{
					calendarItemBase.Dispose();
				}
			}
			return result;
		}

		internal void PrintValidatorInformation(XmlWriter writer)
		{
			writer.WriteStartElement("ValidatorInformation");
			writer.WriteElementString("Version", "1.4");
			writer.WriteStartElement("ConsistencyChecksList");
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.CanValidateOwnerCheck.ToString());
			writer.WriteElementString("ItemType", "Organizer");
			writer.WriteElementString("Description", "Checks whether the counterpart user can be validated or not.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.MeetingExistenceCheck.ToString());
			writer.WriteElementString("ItemType", "Organizer");
			writer.WriteElementString("Description", "Checkes whether the item can be found in the owner's calendar or not.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.MeetingCancellationCheck.ToString());
			writer.WriteElementString("ItemType", "Organizer");
			writer.WriteElementString("Description", "Checkes to make sure that the meeting cancellations statuses match.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.CanValidateOwnerCheck.ToString());
			writer.WriteElementString("ItemType", "Attendee");
			writer.WriteElementString("Description", "Checks whether the counterpart user can be validated or not.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.MeetingExistenceCheck.ToString());
			writer.WriteElementString("ItemType", "Attendee");
			writer.WriteElementString("Description", "Checkes whether the item can be found in the owner's calendar or not.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.MeetingCancellationCheck.ToString());
			writer.WriteElementString("ItemType", "Attendee");
			writer.WriteElementString("Description", "Checkes to make sure that the meeting cancellations statuses match.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.AttendeeOnListCheck.ToString());
			writer.WriteElementString("ItemType", "Attendee");
			writer.WriteElementString("Description", "Checkes to make sure that the attendee is listed on the organizer's list of attendees.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.CorrectResponseCheck.ToString());
			writer.WriteElementString("ItemType", "Both");
			writer.WriteElementString("Description", "Checkes to make sure that the organizaer has the correct response recorded for the attendee.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.MeetingPropertiesMatchCheck.ToString());
			writer.WriteElementString("ItemType", "Both");
			writer.WriteElementString("Description", "Checks to make sure that the attendee has the correct critical properties for the meeting.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.RecurrenceBlobsConsistentCheck.ToString());
			writer.WriteElementString("ItemType", "Both");
			writer.WriteElementString("Description", "Checks to make sure that the recurrence blobs are internally consistent.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.RecurrencesMatchCheck.ToString());
			writer.WriteElementString("ItemType", "Both");
			writer.WriteElementString("Description", "Checkes to make sure that the attendee has the correct recurrence pattern.");
			writer.WriteEndElement();
			writer.WriteStartElement("ConsistencyCheck");
			writer.WriteElementString("Type", ConsistencyCheckType.TimeZoneMatchCheck.ToString());
			writer.WriteElementString("ItemType", "Both");
			writer.WriteElementString("Description", "Checks to make sure that the attendee has correct recurring time zone information with the organizer.");
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		private void InitializeRangeValidation(MailboxSession session, string mailboxUserAddress, ExDateTime rangeStart, ExDateTime rangeEnd, CalendarRepairPolicy repairPolicy, string locationFilter, string subjectFilter, OrganizationId currentOrganizationId, ADObjectId rootOrgContainerId, TimeSpan cvsTimeout)
		{
			if (!string.IsNullOrEmpty(locationFilter))
			{
				this.locationToFilterWith = locationFilter;
			}
			if (!string.IsNullOrEmpty(subjectFilter))
			{
				this.subjectToFilterWith = subjectFilter;
			}
			if (rangeStart >= rangeEnd)
			{
				throw new InvalidDateRangeException(rangeStart, rangeEnd);
			}
			this.rangeStart = rangeStart;
			this.rangeEnd = rangeEnd;
			this.Initialize(session, mailboxUserAddress, true, repairPolicy, currentOrganizationId, rootOrgContainerId, cvsTimeout);
		}

		private void Initialize(MailboxSession session, string mailboxUserAddress, bool validatingRange, CalendarRepairPolicy repairPolicy, OrganizationId currentOrganizationId, ADObjectId rootOrgContainerId, TimeSpan cvsTimeout)
		{
			this.validatingRange = validatingRange;
			if (rootOrgContainerId != null)
			{
				this.rootOrgContainerId = rootOrgContainerId;
			}
			if (currentOrganizationId != null)
			{
				this.currentOrganizationId = currentOrganizationId;
			}
			if (session != null)
			{
				this.mailboxSession = session;
				this.independentSession = false;
			}
			else
			{
				this.independentSession = true;
			}
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.rootOrgContainerId, this.currentOrganizationId, this.currentOrganizationId, true), 713, "Initialize", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\MeetingValidator\\CalendarValidator.cs");
			this.attendeeExtractor = new AttendeeExtractor(this.recipientSession);
			ADRecipient adrecipient = null;
			ProxyAddress proxyAddress = ProxyAddress.Parse(mailboxUserAddress);
			if (!ADRecipient.TryGetFromProxyAddress(proxyAddress, this.recipientSession, out adrecipient) || adrecipient == null)
			{
				throw new MailboxUserNotFoundException(mailboxUserAddress);
			}
			this.mailboxUser = new UserObject(adrecipient);
			if (this.mailboxUser.ExchangePrincipal == null)
			{
				throw new MailboxUserNotFoundException(mailboxUserAddress);
			}
			RumFactory.Instance.Initialize(repairPolicy ?? CalendarValidator.defaultNoRepairForValidatePolicy);
			CalendarValidator.cvsGateway = new CalendarVersionStoreGateway(new CalendarVersionStoreQueryPolicy(cvsTimeout), true);
		}

		private MeetingValidationResult ValidateMeeting(SessionManager sessionManager, CalendarItemBase calendarItem, ExDateTime searchRangeStart, ExDateTime searchRangeEnd, bool hasDuplicates)
		{
			if (this.OnItemInspected != null)
			{
				this.OnItemInspected(1L);
			}
			Participant organizer = calendarItem.Organizer;
			bool flag = RumFactory.Instance.Policy.RepairMode == CalendarRepairType.RepairAndValidate;
			MeetingValidationResult meetingValidationResult;
			if (CalendarValidator.IsValidParticipant(organizer))
			{
				bool flag2 = calendarItem.IsOrganizer();
				UserObject organizer2 = flag2 ? this.mailboxUser : new UserObject(organizer, this.recipientSession);
				MeetingData meetingData = MeetingData.CreateInstance(this.mailboxUser, organizer2, calendarItem);
				meetingValidationResult = new MeetingValidationResult(searchRangeStart, searchRangeEnd, this.mailboxUser, meetingData, hasDuplicates, string.Empty);
				bool flag3 = !flag || !hasDuplicates;
				if (!flag3)
				{
					return meetingValidationResult;
				}
				IEnumerable<UserObject> usersToCompare = this.GetUsersToCompare(calendarItem, organizer2, flag2);
				using (IEnumerator<UserObject> enumerator = usersToCompare.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						UserObject userObject = enumerator.Current;
						if (!flag || !userObject.CalendarRepairDisabled)
						{
							CalendarValidationContext validationContext = CalendarValidationContext.CreateInstance(calendarItem, flag2, this.mailboxUser, userObject, CalendarValidator.cvsGateway, this.attendeeExtractor);
							CalendarParticipant calendarParticipant = null;
							if (this.calendarParticipantList.ContainsKey(userObject))
							{
								calendarParticipant = this.calendarParticipantList[userObject];
							}
							else if (!this.inaccessibleMailboxes.Contains(userObject))
							{
								calendarParticipant = CalendarParticipant.Create(userObject, searchRangeStart, searchRangeEnd, sessionManager);
								if (calendarParticipant == null)
								{
									this.inaccessibleMailboxes.Add(userObject);
								}
								else
								{
									this.calendarParticipantList.Add(userObject, calendarParticipant);
								}
							}
							if (calendarParticipant != null)
							{
								if (calendarParticipant.ItemList.ContainsKey(calendarItem.GlobalObjectId))
								{
									meetingValidationResult = new MeetingValidationResult(searchRangeStart, searchRangeEnd, this.mailboxUser, meetingData, hasDuplicates, "Found duplicate GOID when checking for inconsistencies.");
									meetingValidationResult.IsConsistent = false;
								}
								else
								{
									CalendarInstanceContext value = new CalendarInstanceContext(meetingValidationResult, validationContext);
									calendarParticipant.ItemList.Add(calendarItem.GlobalObjectId, value);
									this.isCalendarItemOnParticipantList = true;
								}
							}
							else
							{
								Globals.ValidatorTracer.TraceDebug<ExchangePrincipal>((long)this.GetHashCode(), "CalendarValidator.ValidateMeeting - Could not access participant {0}", userObject.ExchangePrincipal);
							}
						}
					}
					return meetingValidationResult;
				}
			}
			MeetingData meetingData2 = MeetingData.CreateInstance(this.mailboxUser, null, calendarItem);
			meetingValidationResult = new MeetingValidationResult(searchRangeStart, searchRangeEnd, this.mailboxUser, meetingData2, hasDuplicates, "Unable to retrieve the organizer of the meeting.");
			meetingValidationResult.IsConsistent = false;
			return meetingValidationResult;
		}

		private IEnumerable<UserObject> GetUsersToCompare(CalendarItemBase calendarItem, UserObject organizer, bool isOrganizer)
		{
			if (isOrganizer)
			{
				return this.attendeeExtractor.ExtractAttendees(calendarItem, false);
			}
			return new List<UserObject>(1)
			{
				organizer
			};
		}

		private List<MeetingValidationResult> RemoveDuplicates(MailboxSession session, List<MeetingData> meetings)
		{
			List<MeetingValidationResult> list = new List<MeetingValidationResult>();
			List<StoreId> list2 = new List<StoreId>();
			List<MeetingData> list3 = new List<MeetingData>();
			bool flag = RumFactory.Instance.Policy.RepairMode == CalendarRepairType.RepairAndValidate;
			for (int i = meetings.Count - 1; i >= 0; i--)
			{
				MeetingData meetingData = meetings[i];
				if (meetingData.Exception == null && (meetingData.CalendarItemType == CalendarItemType.RecurringMaster || meetingData.CalendarItemType == CalendarItemType.Single) && !list2.Contains(meetingData.Id))
				{
					List<MeetingData> duplicates = CalendarValidator.GetDuplicates(meetingData, meetings);
					if (duplicates != null && duplicates.Count > 1)
					{
						int num = -1;
						for (int j = duplicates.Count - 1; j >= 0; j--)
						{
							MeetingData meetingData2 = duplicates[j];
							if (meetingData2.Exception == null)
							{
								num = j;
								break;
							}
						}
						if (num >= 0)
						{
							MeetingData meetingData3 = duplicates[num];
							meetingData3.HasDuplicates = true;
							Globals.ValidatorTracer.TraceDebug((long)this.GetHashCode(), "CalendarValidator.RemoveDuplicates - Keeping meeting in mailbox {0}- GlobalObjectId:{1}, Subject:{2}, SequenceNumber:{3}", new object[]
							{
								this.mailboxUser.Alias,
								meetingData3.GlobalObjectId,
								meetingData3.Subject,
								meetingData3.SequenceNumber
							});
							duplicates.RemoveAt(num);
							List<MeetingValidationResult> list4 = new List<MeetingValidationResult>();
							foreach (MeetingData meetingData4 in duplicates)
							{
								if (!list2.Contains(meetingData4.Id))
								{
									list4.Add(new MeetingValidationResult(this.rangeStart, this.rangeEnd, this.mailboxUser, meetingData4, true, (meetingData4.Exception != null) ? meetingData4.Exception.ToString() : null)
									{
										WasValidationSuccessful = true,
										IsDuplicate = true,
										IsDuplicateRemoved = false
									});
									list2.Add(meetingData4.Id);
									list3.Add(meetingData4);
									Globals.ValidatorTracer.TraceDebug((long)this.GetHashCode(), "CalendarValidator.RemoveDuplicates - Removing duplicate meeting in mailbox {0} - GlobalObjectId:{1}, Subject:{2}, SequenceNumber:{3}", new object[]
									{
										this.mailboxUser.Alias,
										meetingData4.GlobalObjectId,
										meetingData4.Subject,
										meetingData4.SequenceNumber
									});
								}
							}
							MeetingValidationResult item = new MeetingValidationResult(this.rangeStart, this.rangeEnd, this.mailboxUser, meetingData3, list4);
							list.Add(item);
						}
					}
				}
			}
			if (list2.Count > 0)
			{
				foreach (MeetingData item2 in list3)
				{
					meetings.Remove(item2);
				}
				if (flag)
				{
					Globals.ValidatorTracer.TraceDebug<string, int>((long)this.GetHashCode(), "CalendarValidator.RemoveDuplicates - Deleting duplicate meetings mailbox {0} - Count:{1}}", this.mailboxUser.Alias, list2.Count);
					AggregateOperationResult aggregateOperationResult = session.Delete(DeleteItemFlags.MoveToDeletedItems, list2.ToArray());
					if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
					{
						using (List<MeetingValidationResult>.Enumerator enumerator3 = list.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								MeetingValidationResult meetingValidationResult = enumerator3.Current;
								if (meetingValidationResult.IsDuplicate)
								{
									meetingValidationResult.IsDuplicateRemoved = true;
								}
							}
							return list;
						}
					}
					Globals.ValidatorTracer.TraceError<string, AggregateOperationResult>((long)this.GetHashCode(), "CalendarValidator.RemoveDuplicates - Deleting duplicate meetings failed on mailbox {0} - failure {1}}", this.mailboxUser.Alias, aggregateOperationResult);
				}
			}
			return list;
		}

		private const string CalendarValidatorName = "CalendarValidator";

		private const string ConsistencyCheckString = "ConsistencyCheck";

		private const string TypeString = "Type";

		private const string ItemTypeString = "ItemType";

		private const string OrganizerString = "Organizer";

		private const string AttendeeString = "Attendee";

		private const string BothString = "Both";

		private const string DescriptionString = "Description";

		private const string ValidatorClientInfoString = "Client=ResourceHealth;Action=Meeting Validator";

		private static CalendarRepairPolicy defaultNoRepairForValidatePolicy = CalendarRepairPolicy.CreateInstance("DefaultNoRepairForValidate");

		private static CalendarVersionStoreGateway cvsGateway;

		private UserObject mailboxUser;

		private OrganizationId currentOrganizationId = OrganizationId.ForestWideOrgId;

		private ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();

		private ExDateTime rangeStart;

		private ExDateTime rangeEnd;

		private StoreId meetingToValidate;

		private bool validatingRange;

		private string locationToFilterWith;

		private string subjectToFilterWith;

		private MailboxSession mailboxSession;

		private IRecipientSession recipientSession;

		private AttendeeExtractor attendeeExtractor;

		private bool independentSession;

		private Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent;

		private bool isCalendarItemOnParticipantList;

		private Dictionary<UserObject, CalendarParticipant> calendarParticipantList = new Dictionary<UserObject, CalendarParticipant>();

		private List<UserObject> inaccessibleMailboxes = new List<UserObject>();
	}
}
