using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.OnlineMeetings.ResourceContract;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class UcwaNewOnlineMeetingWorker : UcwaServerToServerClient, IOnlineMeetingWorker
	{
		public UcwaNewOnlineMeetingWorker(Uri ucwaUrl, OAuthCredentials oauthCredentials, CultureInfo culture) : base(ucwaUrl.AbsoluteUri, oauthCredentials)
		{
			if (!Uri.UriSchemeHttps.Equals(ucwaUrl.Scheme))
			{
				throw new ArgumentException("The UCWA URL scheme must be '" + Uri.UriSchemeHttps + "'");
			}
			this.requestFactory = new OAuthRequestFactory(oauthCredentials);
			this.culture = culture;
		}

		public UcwaNewOnlineMeetingWorker(Uri ucwaUrl, string webTicket) : base(ucwaUrl.AbsoluteUri, null)
		{
			if (!Uri.UriSchemeHttps.Equals(ucwaUrl.Scheme))
			{
				throw new ArgumentException("The UCWA URL scheme must be '" + Uri.UriSchemeHttps + "'");
			}
			this.requestFactory = new WebTicketRequestFactory(webTicket);
		}

		public UcwaNewOnlineMeetingWorker(Uri ucwaUrl, UcwaRequestFactory requestFactory) : base(ucwaUrl.AbsoluteUri, null)
		{
			if (!Uri.UriSchemeHttps.Equals(ucwaUrl.Scheme))
			{
				throw new ArgumentException("The UCWA URL scheme must be '" + Uri.UriSchemeHttps + "'");
			}
			this.requestFactory = requestFactory;
		}

		public Task<OnlineMeetingResult> GetMeetingAsync(string meetingId)
		{
			IApiAdapter apiAdapter = this.GetApiAdapter();
			Task<string> task = this.FindMeetingUrlByIdAsync(apiAdapter, meetingId);
			Task<OnlineMeetingResource> task2 = task.ContinueWith<Task<OnlineMeetingResource>>(delegate(Task<string> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to find url for given meetingId");
				}
				return this.GetMeetingCoreAsync(apiAdapter, t.Result);
			}).Unwrap<OnlineMeetingResource>();
			return task2.ContinueWith<OnlineMeetingResult>(delegate(Task<OnlineMeetingResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting");
				}
				return this.BuildOnlineMeetingResult(apiAdapter, t.Result, null, null, null, null);
			});
		}

		public Task<MyOnlineMeetingsResource> GetMeetingSummariesAsync()
		{
			IApiAdapter apiAdapter = this.GetApiAdapter();
			return this.GetMeetingSummariesCoreAsync(apiAdapter);
		}

		public Task<OnlineMeetingResult> CreateDefaultMeetingAsync(OnlineMeetingSettings meetingSettings)
		{
			OnlineMeetingLogEntry onlineMeetingLogEntry = new OnlineMeetingLogEntry();
			onlineMeetingLogEntry.MeetingSettings = meetingSettings;
			IApiAdapter apiAdapter = this.GetApiAdapter();
			Task<OnlineMeetingResource> assignedMeetingTask = this.GetAssignedMeetingAsync(apiAdapter);
			Task<OnlineMeetingDefaultValuesResource> defaultValuesTask = this.GetMeetingDefaultValuesAsync(apiAdapter);
			Task<OnlineMeetingPoliciesResource> policiesTask = this.GetMeetingPoliciesAsync(apiAdapter);
			Task<OnlineMeetingResult> task = Task.Factory.ContinueWhenAll<Task<OnlineMeetingResult>>(new Task[]
			{
				defaultValuesTask,
				policiesTask,
				assignedMeetingTask
			}, delegate(Task[] _)
			{
				if (defaultValuesTask.IsFaulted)
				{
					this.SaveAndThrowException(onlineMeetingLogEntry, defaultValuesTask.Exception.AsHttpOperationException() ?? defaultValuesTask.Exception.AsOperationFailureException("Unable to retrieve meeting default values"));
				}
				if (policiesTask.IsFaulted)
				{
					this.SaveAndThrowException(onlineMeetingLogEntry, policiesTask.Exception.AsHttpOperationException() ?? policiesTask.Exception.AsOperationFailureException("Unable to retrieve meeting policies"));
				}
				if (assignedMeetingTask.IsFaulted)
				{
					this.SaveAndThrowException(onlineMeetingLogEntry, assignedMeetingTask.Exception.AsHttpOperationException() ?? assignedMeetingTask.Exception.AsOperationFailureException("Unable to retrieve assigned meeting"));
				}
				onlineMeetingLogEntry.DefaultValuesResource = defaultValuesTask.Result;
				onlineMeetingLogEntry.PoliciesResource = policiesTask.Result;
				if (assignedMeetingTask.Result == null || defaultValuesTask.Result.DefaultOnlineMeetingRel != OnlineMeetingRel.MyAssignedOnlineMeeting)
				{
					return this.CreatePrivateMeetingCoreAsync(apiAdapter, meetingSettings, defaultValuesTask.Result, policiesTask.Result);
				}
				return Task.Factory.StartNew<OnlineMeetingResult>(() => this.BuildOnlineMeetingResultFromTask(apiAdapter, assignedMeetingTask, null, null, null, null));
			}).Unwrap<OnlineMeetingResult>();
			if (task.Result != null)
			{
				task.Result.LogEntry = onlineMeetingLogEntry;
			}
			return task;
		}

		private void SaveAndThrowException(OnlineMeetingLogEntry onlineMeetingLogEntry, Exception ex)
		{
			onlineMeetingLogEntry.AddExceptionToLog(ex);
			throw ex;
		}

		public Task<PhoneDialInInformationResource> GetPstnDialInInformationAsync(IApiAdapter apiAdapter)
		{
			Task<PhoneDialInInformationResource> task = null;
			try
			{
				Task<Uri> dialInUriAsync = this.GetDialInUriAsync(apiAdapter);
				task = dialInUriAsync.ContinueWith<Task<PhoneDialInInformationResource>>((Task<Uri> uriTask) => apiAdapter.SendRequestAsync<PhoneDialInInformationResource>(uriTask.Result, "GET", null)).Unwrap<PhoneDialInInformationResource>();
			}
			catch (ArgumentException)
			{
				throw new InvalidOperationException("Unable to send request to token: phonedialininformation");
			}
			return task.ContinueWith<PhoneDialInInformationResource>(delegate(Task<PhoneDialInInformationResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting summaries");
				}
				return t.Result;
			});
		}

		public Task<OnlineMeetingResult> CreatePrivateMeetingAsync(OnlineMeetingSettings meetingSettings)
		{
			IApiAdapter apiAdapter = this.GetApiAdapter();
			if (meetingSettings == null)
			{
				throw new ArgumentNullException("meetingSettings");
			}
			Task<OnlineMeetingDefaultValuesResource> defaultValuesTask = this.GetMeetingDefaultValuesAsync(apiAdapter);
			Task<OnlineMeetingPoliciesResource> policiesTask = this.GetMeetingPoliciesAsync(apiAdapter);
			return Task.Factory.ContinueWhenAll<Task<OnlineMeetingResult>>(new Task[]
			{
				defaultValuesTask,
				policiesTask
			}, delegate(Task[] _)
			{
				if (defaultValuesTask.IsFaulted)
				{
					throw defaultValuesTask.Exception.AsHttpOperationException() ?? defaultValuesTask.Exception.AsOperationFailureException("Unable to retrieve meeting default values");
				}
				if (policiesTask.IsFaulted)
				{
					throw policiesTask.Exception.AsHttpOperationException() ?? policiesTask.Exception.AsOperationFailureException("Unable to retrieve meeting policies");
				}
				return this.CreatePrivateMeetingCoreAsync(apiAdapter, meetingSettings, defaultValuesTask.Result, policiesTask.Result);
			}).Unwrap<OnlineMeetingResult>();
		}

		public Task DeleteMeetingAsync(string meetingId)
		{
			IApiAdapter apiAdapter = this.GetApiAdapter();
			Task<string> task = this.FindMeetingUrlByIdAsync(apiAdapter, meetingId);
			return task.ContinueWith<Task>(delegate(Task<string> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to find url for given meetingId");
				}
				return this.DeleteMeetingCoreAsync(apiAdapter, t.Result);
			}).Unwrap();
		}

		public Task<OnlineMeetingResult> UpdatePrivateMeetingAsync(string meetingId, OnlineMeetingSettings meetingSettings)
		{
			OnlineMeetingLogEntry onlineMeetingLogEntry = new OnlineMeetingLogEntry();
			onlineMeetingLogEntry.MeetingSettings = meetingSettings;
			IApiAdapter apiAdapter = this.GetApiAdapter();
			Task<OnlineMeetingResource> meetingResourceByIdAsync = this.GetMeetingResourceByIdAsync(apiAdapter, meetingId);
			return meetingResourceByIdAsync.ContinueWith<Task<OnlineMeetingResult>>(delegate(Task<OnlineMeetingResource> t)
			{
				if (t.IsFaulted)
				{
					this.SaveAndThrowException(onlineMeetingLogEntry, t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to find url for given meetingId"));
				}
				if (t.Result == null)
				{
					this.SaveAndThrowException(onlineMeetingLogEntry, new OperationFailureException("Unable to find existing meeting to be updated"));
				}
				Task<OnlineMeetingResult> task = this.UpdatePrivateMeetingCoreAsync(apiAdapter, t.Result, meetingSettings);
				if (task.Result != null)
				{
					task.Result.LogEntry = onlineMeetingLogEntry;
				}
				return task;
			}).Unwrap<OnlineMeetingResult>();
		}

		public Task<OnlineMeetingResult> GetOrCreatePublicMeetingAsync()
		{
			IApiAdapter apiAdapter = this.GetApiAdapter();
			Task<OnlineMeetingResource> assignedMeetingAsync = this.GetAssignedMeetingAsync(apiAdapter);
			Task<OnlineMeetingResource> task = assignedMeetingAsync.ContinueWith<Task<OnlineMeetingResource>>(delegate(Task<OnlineMeetingResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to find url for given meetingId");
				}
				return this.GetMeetingCoreAsync(apiAdapter, t.Result.SelfUri);
			}).Unwrap<OnlineMeetingResource>();
			return task.ContinueWith<OnlineMeetingResult>(delegate(Task<OnlineMeetingResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting");
				}
				return this.BuildOnlineMeetingResult(apiAdapter, t.Result, null, null, null, null);
			});
		}

		private static string FindMeetingUrlByIdCore(MyOnlineMeetingsResource summaries, string meetingId)
		{
			string text = null;
			if (summaries.MyOnlineMeetings != null)
			{
				text = (from m in summaries.MyOnlineMeetings
				where m.OnlineMeetingId.Equals(meetingId)
				select m.SelfUri).FirstOrDefault<string>();
			}
			if (text == null && summaries.AssignedOnlineMeetings != null)
			{
				text = (from m in summaries.AssignedOnlineMeetings
				where m.OnlineMeetingId.Equals(meetingId)
				select m.SelfUri).FirstOrDefault<string>();
			}
			if (text == null)
			{
				throw new OperationFailureException("Meeting not found");
			}
			return text;
		}

		private static OnlineMeetingResult BuildOnlineMeetingResultCore(OnlineMeetingResource conference, OnlineMeetingDefaultValuesResource defaultValues, OnlineMeetingInvitationCustomizationResource customValues, PhoneDialInInformationResource pstnValues, OnlineMeetingPoliciesResource policiesValues)
		{
			OnlineMeetingResult onlineMeetingResult = new OnlineMeetingResult();
			if (defaultValues != null)
			{
				onlineMeetingResult.CustomizationValues = customValues.ToOnlineMeetingCustomizationValues();
				onlineMeetingResult.DefaultValues = defaultValues.ToOnlineMeetingDefaultValues();
				onlineMeetingResult.DialIn = pstnValues.ToOnlineMeetingDialInValues();
			}
			if (policiesValues != null)
			{
				onlineMeetingResult.MeetingPolicies = policiesValues.ToOnlineMeetingValue();
			}
			if (conference != null)
			{
				onlineMeetingResult.OnlineMeeting = conference.ToOnlineMeetingValue();
			}
			return onlineMeetingResult;
		}

		private static void UpdateCreateMeetingRequestWithSpecifiedSettings(OnlineMeetingSettings meetingSettings, ref OnlineMeetingResource createMeetingRequest)
		{
			createMeetingRequest.Description = meetingSettings.Description;
			createMeetingRequest.ExpirationTime = meetingSettings.ExpiryTime;
			createMeetingRequest.Subject = meetingSettings.Subject;
			createMeetingRequest.Leaders = meetingSettings.Leaders.ToArray<string>();
			createMeetingRequest.Attendees = meetingSettings.Attendees.ToArray<string>();
		}

		private static OnlineMeetingResource GetCreateMeetingRequestWithDefaultSettings(OnlineMeetingDefaultValuesResource defaultValues, OnlineMeetingPoliciesResource policies)
		{
			return new OnlineMeetingResource(string.Empty)
			{
				EntryExitAnnouncement = new EntryExitAnnouncement?(defaultValues.EntryExitAnnouncement),
				AutomaticLeaderAssignment = new AutomaticLeaderAssignment?(defaultValues.AutomaticLeaderAssignment),
				AccessLevel = new AccessLevel?(defaultValues.AccessLevel),
				PhoneUserAdmission = new PhoneUserAdmission?((policies.PhoneUserAdmission == GenericPolicy.Enabled) ? PhoneUserAdmission.Enabled : PhoneUserAdmission.Disabled),
				LobbyBypassForPhoneUsers = new LobbyBypassForPhoneUsers?(defaultValues.LobbyBypassForPhoneUsers)
			};
		}

		private Task<MyOnlineMeetingsResource> GetMeetingSummariesCoreAsync(IApiAdapter apiAdapter)
		{
			Task<MyOnlineMeetingsResource> task = null;
			try
			{
				task = apiAdapter.SendRequestToTokenAsync<MyOnlineMeetingsResource>("myOnlineMeetings", "GET", null);
			}
			catch (ArgumentException)
			{
				throw new InvalidOperationException("Unable to send request to token: scheduled/summaries");
			}
			return task.ContinueWith<MyOnlineMeetingsResource>(delegate(Task<MyOnlineMeetingsResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting summaries");
				}
				return t.Result;
			});
		}

		private IApiAdapter GetApiAdapter()
		{
			return new NewApiAdapter(this.requestFactory, UcwaNewOnlineMeetingWorker.Serializer, base.UcwaUrl, this.applicationId, this.culture);
		}

		private Task<OnlineMeetingResource> GetAssignedMeetingAsync(IApiAdapter apiAdapter)
		{
			Task<Uri> task = apiAdapter.FindTokenAsync("onlinemeetingeligiblevalues");
			Task<OnlineMeetingEligibleValuesResource> getEligibleValues = task.ContinueWith<Task<OnlineMeetingEligibleValuesResource>>(delegate(Task<Uri> uriTask)
			{
				if (uriTask.Result == null)
				{
					return Task<OnlineMeetingEligibleValuesResource>.Factory.StartNew(() => null);
				}
				return apiAdapter.SendRequestAsync<OnlineMeetingEligibleValuesResource>(uriTask.Result, "GET", null);
			}).Unwrap<OnlineMeetingEligibleValuesResource>();
			Task<OnlineMeetingResource> task2 = getEligibleValues.ContinueWith<Task<OnlineMeetingResource>>(delegate(Task<OnlineMeetingEligibleValuesResource> _)
			{
				if (getEligibleValues.IsFaulted)
				{
					throw getEligibleValues.Exception.AsHttpOperationException() ?? getEligibleValues.Exception.AsOperationFailureException("Unable to get meeting eligible values");
				}
				if (getEligibleValues.Result == null)
				{
					return Task<OnlineMeetingResource>.Factory.StartNew(() => null);
				}
				Link link = (from l in getEligibleValues.Result.Links
				where string.Equals(l.Token, "myassignedonlinemeeting", StringComparison.OrdinalIgnoreCase)
				select l).FirstOrDefault<Link>();
				if (link != null)
				{
					return apiAdapter.SendRequestAsync<OnlineMeetingResource>(new Uri(link.Href, UriKind.Relative), "GET", null);
				}
				return Task<OnlineMeetingResource>.Factory.StartNew(() => null);
			}).Unwrap<OnlineMeetingResource>();
			return task2.ContinueWith<OnlineMeetingResource>(delegate(Task<OnlineMeetingResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting summaries");
				}
				return t.Result;
			});
		}

		private Task<OnlineMeetingDefaultValuesResource> GetMeetingDefaultValuesAsync(IApiAdapter apiAdapter)
		{
			Task<OnlineMeetingDefaultValuesResource> task = null;
			try
			{
				task = apiAdapter.SendRequestToTokenAsync<OnlineMeetingDefaultValuesResource>("onlinemeetingdefaultvalues", "GET", null);
			}
			catch (ArgumentException)
			{
				throw new InvalidOperationException("Unable to send request to token: scheduled/schedulingoptions");
			}
			return task.ContinueWith<OnlineMeetingDefaultValuesResource>(delegate(Task<OnlineMeetingDefaultValuesResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting default values");
				}
				return t.Result;
			});
		}

		private Task<OnlineMeetingInvitationCustomizationResource> GetMeetingCustomizationAsync(IApiAdapter apiAdapter)
		{
			Task<OnlineMeetingInvitationCustomizationResource> task = null;
			try
			{
				task = apiAdapter.SendRequestToTokenAsync<OnlineMeetingInvitationCustomizationResource>("onlinemeetinginvitationcustomization", "GET", null);
			}
			catch (ArgumentException)
			{
				throw new InvalidOperationException("Unable to send request to token: onlinemeetinginvitationcustomization");
			}
			return task.ContinueWith<OnlineMeetingInvitationCustomizationResource>(delegate(Task<OnlineMeetingInvitationCustomizationResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting summaries");
				}
				return t.Result;
			});
		}

		private Task<OnlineMeetingPoliciesResource> GetMeetingPoliciesAsync(IApiAdapter apiAdapter)
		{
			Task<OnlineMeetingPoliciesResource> task = null;
			try
			{
				task = apiAdapter.SendRequestToTokenAsync<OnlineMeetingPoliciesResource>("onlinemeetingpolicies", "GET", null);
			}
			catch (ArgumentException)
			{
				throw new InvalidOperationException("Unable to send request to token: onlinemeetinginvitationcustomization");
			}
			return task.ContinueWith<OnlineMeetingPoliciesResource>(delegate(Task<OnlineMeetingPoliciesResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting summaries");
				}
				return t.Result;
			});
		}

		private Task<OnlineMeetingResult> CreatePrivateMeetingCoreAsync(IApiAdapter apiAdapter, OnlineMeetingSettings meetingSettings, OnlineMeetingDefaultValuesResource defaultValues, OnlineMeetingPoliciesResource policies)
		{
			OnlineMeetingResource createMeetingRequestWithDefaultSettings = UcwaNewOnlineMeetingWorker.GetCreateMeetingRequestWithDefaultSettings(defaultValues, policies);
			if (meetingSettings != null)
			{
				UcwaNewOnlineMeetingWorker.UpdateCreateMeetingRequestWithSpecifiedSettings(meetingSettings, ref createMeetingRequestWithDefaultSettings);
			}
			Task<OnlineMeetingResource> task = apiAdapter.SendRequestToTokenAsync<OnlineMeetingResource>("myOnlineMeetings", "POST", createMeetingRequestWithDefaultSettings);
			return task.ContinueWith<OnlineMeetingResult>(delegate(Task<OnlineMeetingResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to schedule meeting");
				}
				return this.BuildOnlineMeetingResult(apiAdapter, t.Result, defaultValues, null, null, policies);
			});
		}

		private Task<Uri> GetDialInUriAsync(IApiAdapter apiAdapter)
		{
			Task<Uri> phoneUriTask = apiAdapter.FindTokenAsync("phonedialininformation");
			return Task.Factory.ContinueWhenAll<Uri>(new Task<Uri>[]
			{
				phoneUriTask
			}, delegate(Task[] _)
			{
				if (phoneUriTask.IsFaulted)
				{
					throw phoneUriTask.Exception.AsHttpOperationException() ?? phoneUriTask.Exception.AsOperationFailureException("Unable to find the token for phone dial-in information");
				}
				return phoneUriTask.Result;
			});
		}

		private Task<string> FindMeetingUrlByIdAsync(IApiAdapter adapter, string meetingId)
		{
			Task<MyOnlineMeetingsResource> meetingSummariesCoreAsync = this.GetMeetingSummariesCoreAsync(adapter);
			return meetingSummariesCoreAsync.ContinueWith<string>(delegate(Task<MyOnlineMeetingsResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting summaries");
				}
				return UcwaNewOnlineMeetingWorker.FindMeetingUrlByIdCore(t.Result, meetingId);
			});
		}

		private Task<OnlineMeetingResource> GetMeetingCoreAsync(IApiAdapter apiAdapter, string uri)
		{
			if (!string.IsNullOrEmpty(uri))
			{
				Uri uri2 = new Uri(uri, UriKind.RelativeOrAbsolute);
				Task<OnlineMeetingResource> task = apiAdapter.SendRequestAsync<OnlineMeetingResource>(uri2, "GET", null);
				return task.ContinueWith<OnlineMeetingResource>(delegate(Task<OnlineMeetingResource> t)
				{
					if (t.IsFaulted)
					{
						throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to get meeting");
					}
					return t.Result;
				});
			}
			return Task.Factory.StartNew<OnlineMeetingResource>(() => null);
		}

		private Task<OnlineMeetingResource> GetMeetingResourceByIdAsync(IApiAdapter apiAdapter, string meetingId)
		{
			Task<string> task = this.FindMeetingUrlByIdAsync(apiAdapter, meetingId);
			return task.ContinueWith<Task<OnlineMeetingResource>>(delegate(Task<string> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to find url for given meetingId");
				}
				return this.GetMeetingCoreAsync(apiAdapter, t.Result);
			}).Unwrap<OnlineMeetingResource>();
		}

		private OnlineMeetingResult BuildOnlineMeetingResultFromTask(IApiAdapter apiAdapter, Task<OnlineMeetingResource> task, OnlineMeetingDefaultValuesResource defaultValues = null, OnlineMeetingInvitationCustomizationResource customValues = null, PhoneDialInInformationResource pstnValues = null, OnlineMeetingPoliciesResource policiesValues = null)
		{
			if (task.IsFaulted)
			{
				throw task.Exception.AsHttpOperationException() ?? task.Exception.AsOperationFailureException("Unable to get meeting");
			}
			return this.BuildOnlineMeetingResult(apiAdapter, task.Result, defaultValues, customValues, pstnValues, policiesValues);
		}

		private OnlineMeetingResult BuildOnlineMeetingResult(IApiAdapter apiAdapter, OnlineMeetingResource conferenceSettings, OnlineMeetingDefaultValuesResource defaultValues = null, OnlineMeetingInvitationCustomizationResource customValues = null, PhoneDialInInformationResource pstnValues = null, OnlineMeetingPoliciesResource policiesValues = null)
		{
			if (conferenceSettings == null)
			{
				return null;
			}
			Task<OnlineMeetingDefaultValuesResource> defaultValuesTask = (defaultValues == null) ? this.GetMeetingDefaultValuesAsync(apiAdapter) : Helper.CompletedTask<OnlineMeetingDefaultValuesResource>(defaultValues);
			Task<OnlineMeetingInvitationCustomizationResource> customValuesTask = (customValues == null) ? this.GetMeetingCustomizationAsync(apiAdapter) : Helper.CompletedTask<OnlineMeetingInvitationCustomizationResource>(customValues);
			Task<PhoneDialInInformationResource> pstnValuesTask = (pstnValues == null) ? this.GetPstnDialInInformationAsync(apiAdapter) : Helper.CompletedTask<PhoneDialInInformationResource>(pstnValues);
			Task<OnlineMeetingPoliciesResource> policiesValuesTask = (policiesValues == null) ? this.GetMeetingPoliciesAsync(apiAdapter) : Helper.CompletedTask<OnlineMeetingPoliciesResource>(policiesValues);
			return Task.Factory.ContinueWhenAll<OnlineMeetingResult>(new Task[]
			{
				defaultValuesTask,
				customValuesTask,
				pstnValuesTask,
				policiesValuesTask
			}, delegate(Task[] _)
			{
				OnlineMeetingResult result;
				try
				{
					result = UcwaNewOnlineMeetingWorker.BuildOnlineMeetingResultCore(conferenceSettings, defaultValuesTask.Result, customValuesTask.Result, pstnValuesTask.Result, policiesValuesTask.Result);
				}
				catch (AggregateException exception)
				{
					throw exception.AsHttpOperationException() ?? exception.AsOperationFailureException("Unable to get meeting policies");
				}
				return result;
			}).Result;
		}

		private Task DeleteMeetingCoreAsync(IApiAdapter apiAdapter, string uri)
		{
			if (!string.IsNullOrEmpty(uri))
			{
				Uri uri2 = new Uri(uri, UriKind.RelativeOrAbsolute);
				Task task = apiAdapter.SendRequestAsync<OnlineMeetingResource>(uri2, "DELETE", null);
				return task.ContinueWith(delegate(Task t)
				{
					if (t.IsFaulted)
					{
						throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to delete meeting");
					}
				});
			}
			return Task.Factory.StartNew<OnlineMeetingResource>(() => null);
		}

		private Task<OnlineMeetingResult> UpdatePrivateMeetingCoreAsync(IApiAdapter apiAdapter, OnlineMeetingResource currentMeetingSettings, OnlineMeetingSettings meetingSettings)
		{
			OnlineMeetingResource request = currentMeetingSettings;
			if (meetingSettings != null)
			{
				UcwaNewOnlineMeetingWorker.UpdateCreateMeetingRequestWithSpecifiedSettings(meetingSettings, ref request);
			}
			Task<OnlineMeetingResource> task = apiAdapter.SendRequestAsync<OnlineMeetingResource>(new Uri(currentMeetingSettings.SelfUri, UriKind.RelativeOrAbsolute), "PUT", request);
			return task.ContinueWith<OnlineMeetingResult>(delegate(Task<OnlineMeetingResource> t)
			{
				if (t.IsFaulted)
				{
					throw t.Exception.AsHttpOperationException() ?? t.Exception.AsOperationFailureException("Unable to update meeting");
				}
				return this.BuildOnlineMeetingResult(apiAdapter, t.Result, null, null, null, null);
			});
		}

		private static readonly ResourceJsonSerializer Serializer = new ResourceJsonSerializer();

		private readonly UcwaRequestFactory requestFactory;

		private readonly string applicationId = Guid.NewGuid().ToString();

		private readonly CultureInfo culture;
	}
}
