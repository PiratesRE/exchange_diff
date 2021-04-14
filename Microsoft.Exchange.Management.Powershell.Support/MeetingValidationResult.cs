using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Infoworker.MeetingValidator;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class MeetingValidationResult : ConfigurableObject, IConfigurable, IVersionable, ICloneable
	{
		public MeetingValidationResult() : base(new SimplePropertyBag())
		{
			this.MeetingType = string.Empty;
			this.ValidatingRole = string.Empty;
			this.PrimarySmtpAddress = SmtpAddress.Empty;
			this.IntervalStartDate = ExDateTime.MinValue;
			this.IntervalEndDate = ExDateTime.MinValue;
			this.StartTime = ExDateTime.MinValue;
			this.EndTime = ExDateTime.MinValue;
			this.ErrorDescription = string.Empty;
			this.MeetingId = string.Empty;
			this.GlobalObjectId = string.Empty;
			this.CleanGlobalObjectId = string.Empty;
			this.CreationTime = ExDateTime.MinValue;
			this.LastModifiedTime = ExDateTime.MinValue;
			this.Location = string.Empty;
			this.Organizer = string.Empty;
			this.IsConsistent = true;
			this.WasValidationSuccessful = true;
			this.DuplicatesDetected = false;
			this.HasConflicts = false;
			this.ExtractVersion = 0L;
			this.ExtractTime = ExDateTime.MinValue;
			this.NumDelegates = 0;
			this.InternetMessageId = string.Empty;
			this.SequenceNumber = 0;
			this.OwnerApptId = 0;
			this.OwnerCriticalChangeTime = ExDateTime.MinValue;
			this.AttendeeCriticalChangeTime = ExDateTime.MinValue;
			this.WasValidationSuccessful = true;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MeetingValidationResult.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public static MeetingValidationResult CreateOutputObject(MeetingValidationResult result)
		{
			MeetingValidationResult meetingValidationResult = new MeetingValidationResult();
			meetingValidationResult.PrimarySmtpAddress = new SmtpAddress(result.MailboxUserPrimarySmtpAddress);
			meetingValidationResult.ValidatingRole = (result.IsOrganizer ? "Organizer" : "Attendee");
			meetingValidationResult.WasValidationSuccessful = result.WasValidationSuccessful;
			meetingValidationResult.StartTime = result.StartTime;
			meetingValidationResult.EndTime = result.EndTime;
			meetingValidationResult.IntervalStartDate = result.IntervalStartDate;
			meetingValidationResult.IntervalEndDate = result.IntervalEndDate;
			meetingValidationResult.Location = result.Location;
			meetingValidationResult.Subject = result.Subject;
			if (result.MeetingId != null)
			{
				meetingValidationResult.MeetingId = result.MeetingId.ToBase64String();
			}
			meetingValidationResult.MeetingType = result.ItemType.ToString();
			if (result.GlobalObjectId != null)
			{
				meetingValidationResult.GlobalObjectId = result.GlobalObjectId.ToString();
			}
			if (result.CleanGlobalObjectId != null)
			{
				meetingValidationResult.CleanGlobalObjectId = result.CleanGlobalObjectId.ToString();
			}
			meetingValidationResult.CreationTime = result.CreationTime;
			meetingValidationResult.LastModifiedTime = result.LastModifiedTime;
			meetingValidationResult.DuplicatesDetected = result.DuplicatesDetected;
			meetingValidationResult.IsConsistent = result.IsConsistent;
			if (!string.IsNullOrEmpty(result.OrganizerPrimarySmtpAddress))
			{
				meetingValidationResult.Organizer = result.OrganizerPrimarySmtpAddress;
			}
			if (result.MeetingData != null)
			{
				meetingValidationResult.InternetMessageId = result.MeetingData.InternetMessageId;
				meetingValidationResult.ExtractVersion = result.MeetingData.ExtractVersion;
				meetingValidationResult.ExtractTime = result.MeetingData.ExtractTime;
				meetingValidationResult.HasConflicts = result.MeetingData.HasConflicts;
				meetingValidationResult.NumDelegates = result.NumberOfDelegates;
				meetingValidationResult.SequenceNumber = result.MeetingData.SequenceNumber;
				meetingValidationResult.OwnerApptId = (result.MeetingData.OwnerAppointmentId ?? -2);
				meetingValidationResult.OwnerCriticalChangeTime = result.MeetingData.OwnerCriticalChangeTime;
				meetingValidationResult.AttendeeCriticalChangeTime = result.MeetingData.AttendeeCriticalChangeTime;
			}
			if (!string.IsNullOrEmpty(result.ErrorDescription))
			{
				meetingValidationResult.ErrorDescription = result.ErrorDescription;
			}
			return meetingValidationResult;
		}

		public static void FilterResultsLists(MeetingValidationResult cmdLetValidationResult, MeetingValidationResult result, FailureCategory failureCategory, bool onlyReportErrors)
		{
			List<ResultsPerAttendee> list = new List<ResultsPerAttendee>();
			foreach (KeyValuePair<string, MeetingComparisonResult> keyValuePair in result.ResultsPerAttendee)
			{
				ResultsPerAttendee resultsPerAttendee = new ResultsPerAttendee();
				resultsPerAttendee.PrimarySMTPAddress = new SmtpAddress(keyValuePair.Key);
				foreach (ConsistencyCheckResult consistencyCheckResult in keyValuePair.Value)
				{
					if (!onlyReportErrors || consistencyCheckResult.Status != CheckStatusType.Passed)
					{
						foreach (Inconsistency inconsistency in consistencyCheckResult)
						{
							switch (consistencyCheckResult.CheckType)
							{
							case ConsistencyCheckType.CanValidateOwnerCheck:
								if (inconsistency.Flag == CalendarInconsistencyFlag.Organizer)
								{
									resultsPerAttendee.CantOpen = "CanValidateCheck failed " + inconsistency.Description;
								}
								else
								{
									resultsPerAttendee.MailboxUnavailable = "CanValidateCheck failed " + consistencyCheckResult.ErrorString;
								}
								break;
							case ConsistencyCheckType.MeetingExistenceCheck:
								if (inconsistency.Flag == CalendarInconsistencyFlag.Response)
								{
									resultsPerAttendee.IntentionalWrongTrackingInfo = string.Format("MeetingExistenceCheck failed (intent: {0}) {1}", inconsistency.Intent, consistencyCheckResult.ErrorString);
								}
								else if (inconsistency.Flag == CalendarInconsistencyFlag.OrphanedMeeting && ClientIntentQuery.CheckDesiredClientIntent(inconsistency.Intent, new ClientIntentFlags[]
								{
									ClientIntentFlags.MeetingCanceled,
									ClientIntentFlags.MeetingExceptionCanceled
								}))
								{
									resultsPerAttendee.IntentionalMissingMeetings = string.Format("MeetingExistenceCheck failed (intent: {0}) {1}", inconsistency.Intent, consistencyCheckResult.ErrorString);
								}
								else
								{
									resultsPerAttendee.MissingMeetings = string.Format("MeetingExistenceCheck failed (intent: {0}) {1}", inconsistency.Intent, consistencyCheckResult.ErrorString);
								}
								break;
							case ConsistencyCheckType.ValidateStoreObjectCheck:
								resultsPerAttendee.CantOpen = "ValidateStoreObjectCheck failed " + inconsistency.Description;
								break;
							case ConsistencyCheckType.MeetingCancellationCheck:
								resultsPerAttendee.WrongTrackingInfo = "MeetingCancellationCheck failed " + consistencyCheckResult.ErrorString;
								break;
							case ConsistencyCheckType.AttendeeOnListCheck:
								resultsPerAttendee.WrongTrackingInfo = "AttendeeOnListCheck failed " + consistencyCheckResult.ErrorString;
								break;
							case ConsistencyCheckType.CorrectResponseCheck:
							{
								ResponseInconsistency responseInconsistency = inconsistency as ResponseInconsistency;
								if (responseInconsistency != null)
								{
									resultsPerAttendee.WrongTrackingInfo = "CorrectResponseCheck: Attendee has a response that is different from what was sent to the organizer.";
									ResultsPerAttendee resultsPerAttendee2 = resultsPerAttendee;
									resultsPerAttendee2.WrongTrackingInfo += string.Format("User {0} at {1}, the organizer expected {2} at {3}. ", new object[]
									{
										responseInconsistency.ExpectedResponse,
										responseInconsistency.AttendeeReplyTime,
										responseInconsistency.ActualResponse,
										responseInconsistency.OrganizerRecordedTime
									});
									ResultsPerAttendee resultsPerAttendee3 = resultsPerAttendee;
									resultsPerAttendee3.WrongTrackingInfo += responseInconsistency.Description;
								}
								break;
							}
							case ConsistencyCheckType.MeetingPropertiesMatchCheck:
							{
								PropertyInconsistency propertyInconsistency = inconsistency as PropertyInconsistency;
								if (propertyInconsistency != null)
								{
									string text;
									if (result.IsOrganizer)
									{
										text = propertyInconsistency.ActualValue;
									}
									else
									{
										text = propertyInconsistency.ExpectedValue;
									}
									if (propertyInconsistency.PropertyName.Contains("StartTime"))
									{
										Match match = Regex.Match(text, "Start\\[(?<value>.+)\\]");
										resultsPerAttendee.WrongStartTime = (match.Success ? match.Groups["value"].Value : text);
									}
									else if (propertyInconsistency.PropertyName.Contains("EndTime"))
									{
										Match match2 = Regex.Match(text, "End\\[(?<value>.+)\\]");
										resultsPerAttendee.WrongEndTime = (match2.Success ? match2.Groups["value"].Value : text);
									}
									else if (propertyInconsistency.PropertyName.Contains("Location"))
									{
										resultsPerAttendee.WrongLocation = text;
									}
									else if (propertyInconsistency.PropertyName.Contains("MeetingOverlap"))
									{
										resultsPerAttendee.WrongOverlap = text;
									}
									else if (propertyInconsistency.PropertyName.Contains("SequenceNumber"))
									{
										ResultsPerAttendee resultsPerAttendee4 = resultsPerAttendee;
										resultsPerAttendee4.DelayedUpdatesWrongVersion = resultsPerAttendee4.DelayedUpdatesWrongVersion + text + ", ";
									}
									else if (propertyInconsistency.PropertyName.Contains("OwnerCriticalChangeTime"))
									{
										ResultsPerAttendee resultsPerAttendee5 = resultsPerAttendee;
										resultsPerAttendee5.DelayedUpdatesWrongVersion = resultsPerAttendee5.DelayedUpdatesWrongVersion + text + ", ";
									}
									else if (propertyInconsistency.PropertyName.Contains("AttendeeCriticalChangeTime"))
									{
										ResultsPerAttendee resultsPerAttendee6 = resultsPerAttendee;
										resultsPerAttendee6.DelayedUpdatesWrongVersion = resultsPerAttendee6.DelayedUpdatesWrongVersion + text + ", ";
									}
								}
								break;
							}
							case ConsistencyCheckType.RecurrenceBlobsConsistentCheck:
								resultsPerAttendee.RecurrenceProblems = "RecurrenceBlobsConsistentCheck failed " + consistencyCheckResult.ErrorString;
								break;
							case ConsistencyCheckType.RecurrencesMatchCheck:
								resultsPerAttendee.RecurrenceProblems = "RecurrencesMatchCheck failed " + consistencyCheckResult.ErrorString;
								break;
							case ConsistencyCheckType.TimeZoneMatchCheck:
							{
								PropertyInconsistency propertyInconsistency2 = inconsistency as PropertyInconsistency;
								if (propertyInconsistency2 != null)
								{
									resultsPerAttendee.WrongTimeZone = propertyInconsistency2.ActualValue + "(Expected: " + propertyInconsistency2.ExpectedValue + ")";
								}
								break;
							}
							}
						}
					}
				}
				bool flag = false;
				if (failureCategory != FailureCategory.All)
				{
					if ((failureCategory & FailureCategory.DuplicateMeetings) == FailureCategory.DuplicateMeetings && !string.IsNullOrEmpty(resultsPerAttendee.Duplicates))
					{
						flag = true;
					}
					else if ((failureCategory & FailureCategory.WrongLocation) == FailureCategory.WrongLocation && !string.IsNullOrEmpty(resultsPerAttendee.WrongLocation))
					{
						flag = true;
					}
					else if ((failureCategory & FailureCategory.WrongTime) == FailureCategory.WrongTime && (!string.IsNullOrEmpty(resultsPerAttendee.WrongStartTime) || !string.IsNullOrEmpty(resultsPerAttendee.WrongEndTime)))
					{
						flag = true;
					}
					else if ((failureCategory & FailureCategory.WrongTrackingStatus) == FailureCategory.WrongTrackingStatus && !string.IsNullOrEmpty(resultsPerAttendee.WrongTrackingInfo))
					{
						flag = true;
					}
					else if ((failureCategory & FailureCategory.CorruptMeetings) == FailureCategory.CorruptMeetings && (!string.IsNullOrEmpty(resultsPerAttendee.CantOpen) || !string.IsNullOrEmpty(cmdLetValidationResult.ErrorDescription)))
					{
						flag = true;
					}
					else if ((failureCategory & FailureCategory.MissingMeetings) == FailureCategory.MissingMeetings && !string.IsNullOrEmpty(resultsPerAttendee.MissingMeetings))
					{
						flag = true;
					}
					else if ((failureCategory & FailureCategory.RecurrenceProblems) == FailureCategory.RecurrenceProblems && !string.IsNullOrEmpty(resultsPerAttendee.RecurrenceProblems))
					{
						flag = true;
					}
					else if ((failureCategory & FailureCategory.MailboxUnavailable) == FailureCategory.MailboxUnavailable && !string.IsNullOrEmpty(resultsPerAttendee.MailboxUnavailable))
					{
						flag = true;
					}
				}
				else if (!onlyReportErrors || resultsPerAttendee.HasErrors())
				{
					flag = true;
				}
				if (flag)
				{
					list.Add(resultsPerAttendee);
				}
			}
			cmdLetValidationResult.ResultsPerAttendee = list.ToArray();
		}

		public string MeetingType
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.MeetingType];
			}
			internal set
			{
				this[MeetingValidationResultSchema.MeetingType] = value;
			}
		}

		public string ValidatingRole
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.ValidatingRole];
			}
			internal set
			{
				this[MeetingValidationResultSchema.ValidatingRole] = value;
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[MeetingValidationResultSchema.PrimarySmtpAddress];
			}
			internal set
			{
				this[MeetingValidationResultSchema.PrimarySmtpAddress] = value;
			}
		}

		public ExDateTime IntervalStartDate
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.IntervalStartDate];
			}
			internal set
			{
				this[MeetingValidationResultSchema.IntervalStartDate] = value;
			}
		}

		public ExDateTime IntervalEndDate
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.IntervalEndDate];
			}
			internal set
			{
				this[MeetingValidationResultSchema.IntervalEndDate] = value;
			}
		}

		public ExDateTime StartTime
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.StartTime];
			}
			internal set
			{
				this[MeetingValidationResultSchema.StartTime] = value;
			}
		}

		public ExDateTime EndTime
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.EndTime];
			}
			internal set
			{
				this[MeetingValidationResultSchema.EndTime] = value;
			}
		}

		public string ErrorDescription
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.ErrorDescription];
			}
			internal set
			{
				this[MeetingValidationResultSchema.ErrorDescription] = value;
			}
		}

		public string MeetingId
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.MeetingId];
			}
			internal set
			{
				this[MeetingValidationResultSchema.MeetingId] = value;
			}
		}

		public string GlobalObjectId
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.GlobalObjectId];
			}
			internal set
			{
				this[MeetingValidationResultSchema.GlobalObjectId] = value;
			}
		}

		public string CleanGlobalObjectId
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.CleanGlobalObjectId];
			}
			internal set
			{
				this[MeetingValidationResultSchema.CleanGlobalObjectId] = value;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.CreationTime];
			}
			internal set
			{
				this[MeetingValidationResultSchema.CreationTime] = value;
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.LastModifiedTime];
			}
			internal set
			{
				this[MeetingValidationResultSchema.LastModifiedTime] = value;
			}
		}

		public string Location
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.Location];
			}
			internal set
			{
				this[MeetingValidationResultSchema.Location] = value;
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.Subject];
			}
			internal set
			{
				this[MeetingValidationResultSchema.Subject] = value;
			}
		}

		public string Organizer
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.Organizer];
			}
			internal set
			{
				this[MeetingValidationResultSchema.Organizer] = value;
			}
		}

		public bool IsConsistent
		{
			get
			{
				return (bool)this[MeetingValidationResultSchema.IsConsistent];
			}
			internal set
			{
				this[MeetingValidationResultSchema.IsConsistent] = value;
			}
		}

		public bool DuplicatesDetected
		{
			get
			{
				return (bool)this[MeetingValidationResultSchema.DuplicatesDetected];
			}
			internal set
			{
				this[MeetingValidationResultSchema.DuplicatesDetected] = value;
			}
		}

		public bool HasConflicts
		{
			get
			{
				return (bool)this[MeetingValidationResultSchema.HasConflicts];
			}
			internal set
			{
				this[MeetingValidationResultSchema.HasConflicts] = value;
			}
		}

		public long ExtractVersion
		{
			get
			{
				return (long)this[MeetingValidationResultSchema.ExtractVersion];
			}
			internal set
			{
				this[MeetingValidationResultSchema.ExtractVersion] = value;
			}
		}

		public ExDateTime ExtractTime
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.ExtractTime];
			}
			internal set
			{
				this[MeetingValidationResultSchema.ExtractTime] = value;
			}
		}

		public int NumDelegates
		{
			get
			{
				return (int)this[MeetingValidationResultSchema.NumDelegates];
			}
			internal set
			{
				this[MeetingValidationResultSchema.NumDelegates] = value;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return (string)this[MeetingValidationResultSchema.InternetMessageId];
			}
			internal set
			{
				this[MeetingValidationResultSchema.InternetMessageId] = value;
			}
		}

		public int SequenceNumber
		{
			get
			{
				return (int)this[MeetingValidationResultSchema.SequenceNumber];
			}
			internal set
			{
				this[MeetingValidationResultSchema.SequenceNumber] = value;
			}
		}

		public int OwnerApptId
		{
			get
			{
				return (int)this[MeetingValidationResultSchema.OwnerApptId];
			}
			internal set
			{
				this[MeetingValidationResultSchema.OwnerApptId] = value;
			}
		}

		public ExDateTime OwnerCriticalChangeTime
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.OwnerCriticalChangeTime];
			}
			internal set
			{
				this[MeetingValidationResultSchema.OwnerCriticalChangeTime] = value;
			}
		}

		public ExDateTime AttendeeCriticalChangeTime
		{
			get
			{
				return (ExDateTime)this[MeetingValidationResultSchema.AttendeeCriticalChangeTime];
			}
			internal set
			{
				this[MeetingValidationResultSchema.AttendeeCriticalChangeTime] = value;
			}
		}

		public bool WasValidationSuccessful
		{
			get
			{
				return (bool)this[MeetingValidationResultSchema.WasValidationSuccessful];
			}
			internal set
			{
				this[MeetingValidationResultSchema.WasValidationSuccessful] = value;
			}
		}

		public ResultsPerAttendee[] ResultsPerAttendee
		{
			get
			{
				return this.resultsPerAttendee;
			}
			internal set
			{
				this.resultsPerAttendee = value;
			}
		}

		private ResultsPerAttendee[] resultsPerAttendee;

		private static InMemoryObjectSchema schema = ObjectSchema.GetInstance<MeetingValidationResultSchema>();
	}
}
