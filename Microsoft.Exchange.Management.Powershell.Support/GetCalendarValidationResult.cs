using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Infoworker.MeetingValidator;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "CalendarValidationResult", DefaultParameterSetName = "Identity")]
	public sealed class GetCalendarValidationResult : GetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		public GetCalendarValidationResult()
		{
			int num;
			ThreadPool.GetMaxThreads(out this.maxThreadPoolThreads, out num);
			this.validatorObjectCount = -1;
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, Position = 0)]
		public new MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime IntervalStartDate
		{
			get
			{
				return (ExDateTime)(base.Fields["IntervalStartDate"] ?? ExDateTime.Today);
			}
			set
			{
				base.Fields["IntervalStartDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime IntervalEndDate
		{
			get
			{
				return (ExDateTime)(base.Fields["IntervalEndDate"] ?? ExDateTime.Today.AddDays(1.0));
			}
			set
			{
				base.Fields["IntervalEndDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OnlyReportErrors
		{
			get
			{
				return (bool)(base.Fields["OnlyReportErrors"] ?? true);
			}
			set
			{
				base.Fields["OnlyReportErrors"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IncludeAnalysis
		{
			get
			{
				return (bool)(base.Fields["IncludeAnalysis"] ?? false);
			}
			set
			{
				base.Fields["IncludeAnalysis"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Location
		{
			get
			{
				return (string)(base.Fields["Location"] ?? string.Empty);
			}
			set
			{
				base.Fields["Location"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Subject
		{
			get
			{
				return (string)(base.Fields["Subject"] ?? string.Empty);
			}
			set
			{
				base.Fields["Subject"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true)]
		public string MeetingID
		{
			get
			{
				return (string)(base.Fields["MeetingID"] ?? string.Empty);
			}
			set
			{
				base.Fields["MeetingID"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FailureCategory FailureCategoryType
		{
			get
			{
				return (FailureCategory)(base.Fields["FailureCategoryType"] ?? FailureCategory.All);
			}
			set
			{
				base.Fields["FailureCategoryType"] = value;
			}
		}

		[ValidateRange(1, 100)]
		[Parameter(Mandatory = false)]
		public int MaxThreads
		{
			get
			{
				return (int)base.Fields["MaxThreads"];
			}
			set
			{
				base.Fields["MaxThreads"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.Fields["MaxThreads"] != null && this.MaxThreads > this.maxThreadPoolThreads)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMaxThreadPoolThreads(this.maxThreadPoolThreads)), ErrorCategory.InvalidOperation, this);
			}
			if (this.IntervalStartDate >= this.IntervalEndDate)
			{
				base.WriteError(new ArgumentException(Strings.ErrorStartDateEqualGreaterThanEndDate(this.IntervalStartDate.ToString(), this.IntervalEndDate.ToString())), ErrorCategory.InvalidOperation, this);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Identity != null)
			{
				LocalizedString? localizedString;
				IEnumerable<ADUser> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
				this.WriteResult<ADUser>(dataObjects);
				if (!base.HasErrors && base.WriteObjectCount == 0U && this.validatorObjectCount == -1)
				{
					string source = (base.DataSession != null) ? base.DataSession.Source : null;
					Exception exception = new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), source));
					base.WriteError(exception, ErrorCategory.InvalidData, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			if (dataObject == null)
			{
				return;
			}
			try
			{
				ADUser user = (ADUser)dataObject;
				this.DoCalendarValidation(user);
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void DoCalendarValidation(ADUser user)
		{
			List<MeetingValidationResult> list = null;
			base.WriteProgress(Strings.CalendarValidationTask, Strings.ValidatingCalendar(this.Identity.ToString()), 0);
			try
			{
				TopologyProvider.SetProcessTopologyMode(false, false);
				string mailboxUserAddress = string.Format("SMTP:{0}", user.PrimarySmtpAddress);
				CalendarValidator calendarValidator;
				if (!string.IsNullOrEmpty(this.MeetingID))
				{
					VersionedId meetingId = new VersionedId(this.MeetingID);
					calendarValidator = CalendarValidator.CreateMeetingSpecificValidatingInstance(mailboxUserAddress, user.OrganizationId, base.RootOrgContainerId, meetingId);
				}
				else
				{
					calendarValidator = CalendarValidator.CreateRangeValidatingInstance(mailboxUserAddress, user.OrganizationId, base.RootOrgContainerId, this.IntervalStartDate, this.IntervalEndDate, this.Location, this.Subject);
				}
				list = calendarValidator.Run();
			}
			catch (WrongServerException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, this);
			}
			catch (MailboxUserNotFoundException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidData, this);
			}
			catch (CorruptDataException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidData, this);
			}
			finally
			{
				TopologyProvider.SetProcessTopologyMode(true, false);
			}
			this.validatorObjectCount = 0;
			int num = (list.Count > 0) ? list.Count : 1;
			foreach (MeetingValidationResult meetingValidationResult in list)
			{
				if (!this.OnlyReportErrors || !meetingValidationResult.IsConsistent || !meetingValidationResult.WasValidationSuccessful || meetingValidationResult.DuplicatesDetected)
				{
					MeetingValidationResult meetingValidationResult2 = MeetingValidationResult.CreateOutputObject(meetingValidationResult);
					MeetingValidationResult.FilterResultsLists(meetingValidationResult2, meetingValidationResult, this.FailureCategoryType, this.OnlyReportErrors);
					this.validatorObjectCount++;
					bool flag = true;
					if (meetingValidationResult2.ResultsPerAttendee.Length < 1)
					{
						flag = false;
					}
					if ((this.FailureCategoryType & FailureCategory.CorruptMeetings) == FailureCategory.CorruptMeetings && !meetingValidationResult2.WasValidationSuccessful)
					{
						flag = true;
					}
					else if ((this.FailureCategoryType & FailureCategory.DuplicateMeetings) == FailureCategory.DuplicateMeetings && meetingValidationResult2.DuplicatesDetected)
					{
						flag = true;
					}
					if (flag)
					{
						meetingValidationResult2.SetIsReadOnly(true);
						base.WriteResult(meetingValidationResult2);
					}
					base.WriteProgress(Strings.CalendarValidationTask, Strings.ValidatingCalendar(this.Identity.ToString()), this.validatorObjectCount / num * 100);
				}
			}
			base.WriteProgress(Strings.CalendarValidationTask, Strings.ValidatingCalendar(this.Identity.ToString()), 100);
		}

		private const int DefaultInitialObjectCount = -1;

		private int maxThreadPoolThreads;

		private int validatorObjectCount;
	}
}
