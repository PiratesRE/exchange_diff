using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class UserSettingAutodiscovery : DisposeTrackableBase, IAutodiscoveryClient
	{
		public UserSettingAutodiscovery(List<MailboxInfo> mailboxes, Uri autodiscoverEndpoint, ICredentials credentials, CallerInfo callerInfo)
		{
			this.mailboxes = mailboxes;
			this.autodiscoveryEndpoint = autodiscoverEndpoint;
			this.client = new DefaultBinding_Autodiscover(base.GetType().FullName, new RemoteCertificateValidationCallback(CertificateValidation.CertificateErrorHandler));
			this.client.Url = autodiscoverEndpoint.ToString();
			this.client.UserAgent = base.GetType().ToString();
			this.client.RequestedServerVersionValue = UserSettingAutodiscovery.Exchange2013RequestedServerVersion;
			this.client.PreAuthenticate = true;
			this.client.Credentials = credentials;
			this.callerInfo = callerInfo;
		}

		public IAsyncResult BeginAutodiscover(AsyncCallback callback, object state)
		{
			this.callback = callback;
			this.asyncResult = new AsyncResult(callback, state);
			User[] array = new User[this.mailboxes.Count];
			for (int i = 0; i < this.mailboxes.Count; i++)
			{
				array[i] = UserSettingAutodiscovery.CreateUserFromMailboxInfo(this.mailboxes[i]);
			}
			this.request = new GetUserSettingsRequest
			{
				Users = array,
				RequestedSettings = UserSettingAutodiscovery.RequestedSettings,
				RequestedVersion = new ExchangeVersion?(ExchangeVersion.Exchange2013)
			};
			this.client.BeginGetUserSettings(this.request, new AsyncCallback(this.UsersettingsDiscoveryCompleted), null);
			return this.asyncResult;
		}

		public Dictionary<GroupId, List<MailboxInfo>> EndAutodiscover(IAsyncResult asyncResult)
		{
			this.redirects = 0;
			this.asyncResult.AsyncWaitHandle.WaitOne();
			return this.groups;
		}

		public void CancelAutodiscover()
		{
			lock (this)
			{
				if (this.groups != null && !this.cancelled)
				{
					this.cancelled = true;
					this.groups = new Dictionary<GroupId, List<MailboxInfo>>(1);
					this.groups.Add(new GroupId(new MultiMailboxSearchException(Strings.AutodiscoverTimedOut)), this.mailboxes);
					this.ReportCompletion();
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.asyncResult != null)
				{
					this.asyncResult.Dispose();
					this.asyncResult = null;
				}
				if (this.client != null)
				{
					this.client.Dispose();
					this.client = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UserSettingAutodiscovery>(this);
		}

		private void UsersettingsDiscoveryCompleted(IAsyncResult result)
		{
			lock (this)
			{
				if (!this.cancelled)
				{
					Exception ex = null;
					bool flag2 = false;
					try
					{
						GetUserSettingsResponse response = this.client.EndGetUserSettings(result);
						this.groups = this.CreateGroupIdFromAutoDiscoverResponse(response);
						flag2 = true;
					}
					catch (LocalizedException ex2)
					{
						ex = ex2;
					}
					catch (IOException ex3)
					{
						ex = ex3;
					}
					catch (WebException ex4)
					{
						if (!this.ShouldHandleRedirect(ex4.Response as HttpWebResponse))
						{
							ex = ex4;
						}
					}
					catch (SoapException ex5)
					{
						ex = ex5;
					}
					catch (InvalidOperationException ex6)
					{
						ex = ex6;
					}
					if (ex != null)
					{
						Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoveryAutodiscoverError, null, new object[]
						{
							this.autodiscoveryEndpoint.ToString(),
							ex.ToString(),
							this.callerInfo.QueryCorrelationId.ToString()
						});
						this.CreateErrorGroup(ex);
						flag2 = true;
					}
					if (flag2)
					{
						this.ReportCompletion();
					}
					else
					{
						this.client.BeginGetUserSettings(this.request, new AsyncCallback(this.UsersettingsDiscoveryCompleted), null);
					}
				}
			}
		}

		private void CreateErrorGroup(Exception exception)
		{
			Factory.Current.AutodiscoverTracer.TraceError<Guid, string, string>((long)this.GetHashCode(), "Correlation Id:{0}. Request '{1}' failed with exception {2}.", this.callerInfo.QueryCorrelationId, this.autodiscoveryEndpoint.ToString(), exception.ToString());
			this.groups = new Dictionary<GroupId, List<MailboxInfo>>();
			this.groups.Add(new GroupId(exception), this.mailboxes);
		}

		private void ReportCompletion()
		{
			this.asyncResult.ReportCompletion();
			if (this.callback != null)
			{
				this.callback(this.asyncResult);
			}
		}

		private bool ShouldHandleRedirect(HttpWebResponse webResponse)
		{
			if (webResponse.StatusCode != HttpStatusCode.Found && webResponse.StatusCode != HttpStatusCode.MovedPermanently)
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, HttpStatusCode>((long)this.GetHashCode(), "Correlation Id:{0}. The StatusCode in WebException is not an redirect: {1}", this.callerInfo.QueryCorrelationId, webResponse.StatusCode);
				return false;
			}
			this.redirects++;
			if (this.redirects > 5)
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, int>((long)this.GetHashCode(), "Correlation Id:{0}. Stopped following redirects because it exceeded maximum {1}", this.callerInfo.QueryCorrelationId, 5);
				return false;
			}
			string text = webResponse.Headers[HttpResponseHeader.Location];
			if (!Uri.IsWellFormedUriString(text, UriKind.Absolute))
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Not a valid redirect URL: {1}", this.callerInfo.QueryCorrelationId, text);
				return false;
			}
			Uri uri = new Uri(text, UriKind.Absolute);
			if (uri.Scheme != Uri.UriSchemeHttps)
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, string>((long)this.GetHashCode(), "Correlation Id:{0}. Not a secure redirect URL: {1}", this.callerInfo.QueryCorrelationId, text);
				return false;
			}
			UriBuilder uriBuilder = new UriBuilder
			{
				Scheme = Uri.UriSchemeHttps,
				Host = uri.Host,
				Path = this.autodiscoveryEndpoint.PathAndQuery
			};
			this.autodiscoveryEndpoint = uriBuilder.Uri;
			this.client.Url = this.autodiscoveryEndpoint.ToString();
			return true;
		}

		private static User CreateUserFromMailboxInfo(MailboxInfo mailbox)
		{
			SmtpAddress primarySmtpAddress;
			if (mailbox.IsArchive)
			{
				string text = (mailbox.ArchiveDomain != null) ? mailbox.ArchiveDomain.Domain : null;
				if (text == null)
				{
					text = mailbox.PrimarySmtpAddress.Domain;
				}
				primarySmtpAddress = new SmtpAddress(SmtpProxyAddress.EncapsulateExchangeGuid(text, mailbox.ArchiveGuid));
			}
			else if (mailbox.ExternalEmailAddress != null)
			{
				primarySmtpAddress = new SmtpAddress(mailbox.ExternalEmailAddress.AddressString);
			}
			else
			{
				primarySmtpAddress = mailbox.PrimarySmtpAddress;
			}
			return new User
			{
				Mailbox = primarySmtpAddress.ToString()
			};
		}

		private int ParseVersionString(string serverVersion)
		{
			if (serverVersion == null)
			{
				return Server.E15MinVersion;
			}
			string[] array = serverVersion.Split(new char[]
			{
				'.'
			});
			if (array.Length != 4)
			{
				return Server.E15MinVersion;
			}
			int result;
			try
			{
				int major = int.Parse(array[0]);
				int minor = int.Parse(array[1]);
				int build = int.Parse(array[2]);
				int revision = int.Parse(array[3]);
				result = new ServerVersion(major, minor, build, revision, null).ToInt();
			}
			catch (FormatException)
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, object, string>((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Server version string was invalid {2}", this.callerInfo.QueryCorrelationId, TraceContext.Get(), serverVersion);
				result = Server.E15MinVersion;
			}
			catch (OverflowException)
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, object, string>((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Server version string was invalid {2}", this.callerInfo.QueryCorrelationId, TraceContext.Get(), serverVersion);
				result = Server.E15MinVersion;
			}
			catch (ArgumentNullException)
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, object, string>((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Server version string was invalid {2}", this.callerInfo.QueryCorrelationId, TraceContext.Get(), serverVersion);
				result = Server.E15MinVersion;
			}
			return result;
		}

		private Dictionary<GroupId, List<MailboxInfo>> CreateGroupIdFromAutoDiscoverResponse(GetUserSettingsResponse response)
		{
			Dictionary<GroupId, List<MailboxInfo>> dictionary = new Dictionary<GroupId, List<MailboxInfo>>(2);
			Dictionary<Uri, List<MailboxInfo>> dictionary2 = new Dictionary<Uri, List<MailboxInfo>>();
			for (int i = 0; i < this.mailboxes.Count; i++)
			{
				GroupId groupId = this.GetGroupId(this.mailboxes[i], response.UserResponses[i]);
				List<MailboxInfo> list;
				if (!dictionary2.TryGetValue(groupId.Uri, out list))
				{
					list = new List<MailboxInfo>();
					dictionary2.Add(groupId.Uri, list);
					dictionary.Add(groupId, list);
				}
				list.Add(this.mailboxes[i]);
			}
			return dictionary;
		}

		private GroupId GetGroupId(MailboxInfo mailbox, UserResponse userResponse)
		{
			if (userResponse == null)
			{
				return new GroupId(new MultiMailboxSearchException(Strings.descSoapAutoDiscoverInvalidResponseError(this.autodiscoveryEndpoint.ToString())));
			}
			GroupId result = null;
			if (userResponse.ErrorCodeSpecified && this.HasResponseErrorCode(mailbox, userResponse, out result))
			{
				return result;
			}
			if (this.HasSettingErrorInResponse(mailbox, userResponse, out result))
			{
				return result;
			}
			string stringSettingFromResponse = this.GetStringSettingFromResponse(userResponse, mailbox, UserSettingAutodiscovery.ExternalEwsVersion);
			string stringSettingFromResponse2 = this.GetStringSettingFromResponse(userResponse, mailbox, UserSettingAutodiscovery.ExternalEwsUrl);
			if (stringSettingFromResponse2 == null)
			{
				Factory.Current.AutodiscoverTracer.TraceError<Guid, string, string>((long)this.GetHashCode(), "Correlation Id:{0}. Request '{1}' for user {2} got no URL value.", this.callerInfo.QueryCorrelationId, this.autodiscoveryEndpoint.ToString(), mailbox.ToString());
				return new GroupId(new MultiMailboxSearchException(Strings.descSoapAutoDiscoverRequestUserSettingInvalidError(this.autodiscoveryEndpoint.ToString(), UserSettingAutodiscovery.ExternalEwsUrl)));
			}
			return new GroupId(GroupType.CrossPremise, new Uri(stringSettingFromResponse2), this.ParseVersionString(stringSettingFromResponse), null)
			{
				Domain = mailbox.GetDomain()
			};
		}

		private bool HasResponseErrorCode(MailboxInfo mailbox, UserResponse userResponse, out GroupId groupId)
		{
			groupId = null;
			if (!userResponse.ErrorCodeSpecified)
			{
				return false;
			}
			switch (userResponse.ErrorCode)
			{
			case ErrorCode.NoError:
				return false;
			case ErrorCode.RedirectAddress:
				Factory.Current.AutodiscoverTracer.TraceDebug((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Request '{2}' got address redirect response for user {3} to {4}", new object[]
				{
					this.callerInfo.QueryCorrelationId,
					TraceContext.Get(),
					this,
					mailbox.ToString(),
					userResponse.RedirectTarget
				});
				groupId = new GroupId(new MultiMailboxSearchException(new LocalizedString("Redirect address for autodiscovery is not supported")));
				return true;
			case ErrorCode.RedirectUrl:
				Factory.Current.AutodiscoverTracer.TraceError((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Request '{2}' got URL redirect response for user {3} but the redirect value is not valid: {4}", new object[]
				{
					this.callerInfo.QueryCorrelationId,
					TraceContext.Get(),
					this,
					mailbox.ToString(),
					userResponse.RedirectTarget
				});
				groupId = new GroupId(new MultiMailboxSearchException(new LocalizedString("Redirect urls for autodiscovery is not supported")));
				return true;
			default:
				groupId = new GroupId(new MultiMailboxSearchException(Strings.descSoapAutoDiscoverRequestUserSettingError(this.autodiscoveryEndpoint.ToString(), UserSettingAutodiscovery.ExternalEwsUrl, userResponse.ErrorCode.ToString())));
				return true;
			}
		}

		private bool HasSettingErrorInResponse(MailboxInfo mailbox, UserResponse userResponse, out GroupId groupId)
		{
			groupId = null;
			UserSettingError settingErrorFromResponse = this.GetSettingErrorFromResponse(userResponse, UserSettingAutodiscovery.ExternalEwsUrl);
			if (settingErrorFromResponse != null)
			{
				Factory.Current.AutodiscoverTracer.TraceError((long)this.GetHashCode(), "Correlation Id:{0}. Request '{1}' got error response for user {2}. Error: {3}:{4}:{5}", new object[]
				{
					this.callerInfo.QueryCorrelationId,
					this,
					mailbox.ToString(),
					settingErrorFromResponse.SettingName,
					settingErrorFromResponse.ErrorCode,
					settingErrorFromResponse.ErrorMessage
				});
				groupId = new GroupId(new MultiMailboxSearchException(Strings.descSoapAutoDiscoverRequestUserSettingError(this.autodiscoveryEndpoint.ToString(), settingErrorFromResponse.SettingName, settingErrorFromResponse.ErrorMessage)));
				return true;
			}
			return false;
		}

		private UserSettingError GetSettingErrorFromResponse(UserResponse userResponse, string settingName)
		{
			if (userResponse.UserSettingErrors != null)
			{
				UserSettingError[] userSettingErrors = userResponse.UserSettingErrors;
				int i = 0;
				while (i < userSettingErrors.Length)
				{
					UserSettingError userSettingError = userSettingErrors[i];
					if (userSettingError != null && StringComparer.InvariantCulture.Equals(userSettingError.SettingName, settingName))
					{
						if (userSettingError.ErrorCode != ErrorCode.NoError)
						{
							return userSettingError;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			return null;
		}

		private string GetStringSettingFromResponse(UserResponse userResponse, MailboxInfo mailbox, string settingName)
		{
			if (userResponse.UserSettings == null)
			{
				return null;
			}
			UserSetting userSetting = null;
			foreach (UserSetting userSetting2 in userResponse.UserSettings)
			{
				if (StringComparer.InvariantCulture.Equals(userSetting2.Name, settingName))
				{
					userSetting = userSetting2;
					break;
				}
			}
			if (userSetting == null)
			{
				Factory.Current.AutodiscoverTracer.TraceError((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Request '{2}' for user {3} got response without setting {4}.", new object[]
				{
					this.callerInfo.QueryCorrelationId,
					TraceContext.Get(),
					this,
					mailbox.ToString(),
					settingName
				});
				return null;
			}
			StringSetting stringSetting = userSetting as StringSetting;
			if (stringSetting == null)
			{
				Factory.Current.AutodiscoverTracer.TraceError((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Request '{2}' for user {3} got response for setting {4} of unexpected type: {5}", new object[]
				{
					this.callerInfo.QueryCorrelationId,
					TraceContext.Get(),
					this,
					mailbox.ToString(),
					settingName,
					userSetting.GetType().Name
				});
				return null;
			}
			if (string.IsNullOrEmpty(stringSetting.Value))
			{
				Factory.Current.AutodiscoverTracer.TraceError((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Request '{2}' for user {3} got response with empty value for setting {4}", new object[]
				{
					this.callerInfo.QueryCorrelationId,
					TraceContext.Get(),
					this,
					mailbox.ToString(),
					settingName
				});
				return null;
			}
			Factory.Current.AutodiscoverTracer.TraceDebug((long)this.GetHashCode(), "Correlation Id:{0}. {1}: Request '{2}' for user {3} got response for setting {4} with value: {5}", new object[]
			{
				this.callerInfo.QueryCorrelationId,
				TraceContext.Get(),
				this,
				mailbox.ToString(),
				settingName,
				stringSetting.Value
			});
			return stringSetting.Value;
		}

		private const int MaximumRedirects = 5;

		private static string ExternalEwsUrl = "ExternalEwsUrl";

		private static string ExternalEwsVersion = "ExternalEwsVersion";

		private static readonly string[] RequestedSettings = new string[]
		{
			UserSettingAutodiscovery.ExternalEwsUrl,
			UserSettingAutodiscovery.ExternalEwsVersion
		};

		private static readonly RequestedServerVersion Exchange2013RequestedServerVersion = new RequestedServerVersion
		{
			Text = new string[]
			{
				"Exchange2013"
			}
		};

		private readonly List<MailboxInfo> mailboxes;

		private readonly CallerInfo callerInfo;

		private Uri autodiscoveryEndpoint;

		private DefaultBinding_Autodiscover client;

		private Dictionary<GroupId, List<MailboxInfo>> groups;

		private AsyncCallback callback;

		private AsyncResult asyncResult;

		private GetUserSettingsRequest request;

		private int redirects;

		private bool cancelled;
	}
}
