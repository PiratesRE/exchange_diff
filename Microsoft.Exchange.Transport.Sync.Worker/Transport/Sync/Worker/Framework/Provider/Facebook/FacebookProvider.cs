using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Net.Facebook;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.Facebook
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FacebookProvider : ISyncStorageProvider, ISyncStorageProviderItemRetriever
	{
		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return AggregationSubscriptionType.Facebook;
			}
		}

		public SyncStorageProviderState Bind(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			FacebookClient client = new FacebookClient(new Uri(this.ReadGraphApiEndpoint()));
			return new FacebookProviderState(subscription, syncLogSession, client);
		}

		public void Unbind(SyncStorageProviderState state)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			FacebookProviderState facebookProviderState = (FacebookProviderState)state;
			facebookProviderState.Client.Dispose();
		}

		public IAsyncResult BeginAuthenticate(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			FacebookProviderState facebookProviderState = (FacebookProviderState)state;
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult = new AsyncResult<FacebookProviderState, SyncProviderResultData>(this, facebookProviderState, callback, callbackState, syncPoisonContext);
			asyncResult.SetCompletedSynchronously();
			IConnectSubscription connectSubscription = (IConnectSubscription)facebookProviderState.Subscription;
			string accessTokenInClearText = connectSubscription.AccessTokenInClearText;
			if (string.IsNullOrEmpty(accessTokenInClearText))
			{
				asyncResult.ProcessCompleted(SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, new AccessTokenNullOrEmptyException()));
			}
			else
			{
				asyncResult.ProcessCompleted();
			}
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<FacebookProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginCheckForChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			FacebookProviderState providerState = (FacebookProviderState)state;
			IConnectSubscription connectSubscription = (IConnectSubscription)providerState.Subscription;
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult = new AsyncResult<FacebookProviderState, SyncProviderResultData>(this, providerState, callback, callbackState, syncPoisonContext);
			string accessToken = connectSubscription.AccessTokenInClearText;
			providerState.TriggerRequestEvent();
			FacebookProvider.HandleExceptionsInBeginCall(asyncResult, delegate
			{
				asyncResult.PendingAsyncResult = providerState.Client.BeginGetFriends(accessToken, FacebookProvider.CheckForChangesFields, string.Empty, string.Empty, asyncResult.GetAsyncCallbackWithPoisonContextAndUnhandledExceptionRedirect(new AsyncCallback(FacebookProvider.OnGetFriendsCompletedForCheckForChanges)), asyncResult);
			});
			return asyncResult;
		}

		private static void OnGetFriendsCompletedForCheckForChanges(IAsyncResult ar)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult = (AsyncResult<FacebookProviderState, SyncProviderResultData>)ar.AsyncState;
			FacebookProviderState state = asyncResult.State;
			try
			{
				FacebookUsersList facebookUsersList = state.Client.EndGetFriends(ar);
				state.HasNoChangesOnCloud = true;
				if (facebookUsersList.Users != null)
				{
					string text;
					((StringWatermark)state.BaseWatermark).Load(out text);
					state.CurrentWatermark = FacebookProvider.ComputeWatermark(facebookUsersList.Users);
					if (state.CurrentWatermark.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						state.SyncLogSession.LogDebugging((TSLID)1501UL, FacebookProvider.Tracer, "CheckForChanges: No changes detected with watermark: {0}", new object[]
						{
							text
						});
						state.HasNoChangesOnCloud = true;
						state.TriggerRequestEventWithNoChanges();
					}
					else
					{
						state.SyncLogSession.LogDebugging((TSLID)334UL, FacebookProvider.Tracer, "CheckForChanges: Changes detected in watermark. Old: {0}, New: {1}", new object[]
						{
							text,
							state.CurrentWatermark
						});
						state.HasNoChangesOnCloud = false;
						state.CloudUpdates = facebookUsersList.Users;
					}
				}
				asyncResult.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors, state.HasNoChangesOnCloud));
			}
			catch (TimeoutException innerException)
			{
				asyncResult.ProcessCompleted(SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.ConnectionError, innerException));
			}
			catch (ProtocolException innerException2)
			{
				asyncResult.ProcessCompleted(SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, innerException2));
			}
			catch (SerializationException ex)
			{
				state.SyncLogSession.LogError((TSLID)574UL, FacebookProvider.Tracer, "Exception during CheckForChanges when parsing Facebook contacts: {0}", new object[]
				{
					ex
				});
				FacebookProvider.CompleteProcessWithNonPromotableException(asyncResult, ex);
			}
			catch (Exception ex2)
			{
				state.SyncLogSession.LogError((TSLID)575UL, FacebookProvider.Tracer, "Exception during CheckForChanges: {0}", new object[]
				{
					ex2
				});
				FacebookProvider.CompleteProcessWithNonPromotableException(asyncResult, ex2);
			}
		}

		private static void CompleteProcessWithNonPromotableException(AsyncResult<FacebookProviderState, SyncProviderResultData> asyncState, Exception e)
		{
			asyncState.ProcessCompleted(SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.ProviderException, new FacebookNonPromotableTransientException(e), true));
		}

		internal static string ComputeWatermark(List<FacebookUser> users)
		{
			StringBuilder builder = new StringBuilder(10 * users.Count);
			List<FacebookUser> list = new List<FacebookUser>(users);
			list.Sort((FacebookUser x, FacebookUser y) => string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase));
			list.ForEach(delegate(FacebookUser u)
			{
				builder.AppendFormat("{0},{1},", u.Id, u.UpdatedTime);
			});
			return SyncUtilities.ComputeSHA512Hash(builder.ToString());
		}

		public AsyncOperationResult<SyncProviderResultData> EndCheckForChanges(IAsyncResult asyncResult)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<FacebookProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginEnumerateChanges(SyncStorageProviderState state, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			FacebookProviderState providerState = (FacebookProviderState)state;
			IConnectSubscription connectSubscription = (IConnectSubscription)providerState.Subscription;
			providerState.Changes = new List<SyncChangeEntry>();
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult = new AsyncResult<FacebookProviderState, SyncProviderResultData>(this, providerState, callback, callbackState, syncPoisonContext);
			string accessToken = connectSubscription.AccessTokenInClearText;
			if (providerState.CloudUpdates.IsNullOrEmpty())
			{
				int countStateStorageItems = 0;
				using (IEnumerator<string> cloudItemEnumerator = providerState.StateStorage.GetCloudItemEnumerator())
				{
					while (cloudItemEnumerator.MoveNext())
					{
						countStateStorageItems++;
					}
				}
				providerState.TriggerRequestEvent();
				FacebookProvider.HandleExceptionsInBeginCall(asyncResult, delegate
				{
					asyncResult.PendingAsyncResult = providerState.Client.BeginGetFriends(accessToken, FacebookProvider.EnumerateChangesFields, (providerState.MaxDownloadItems + 1).ToString(CultureInfo.InvariantCulture), countStateStorageItems.ToString(CultureInfo.InvariantCulture), asyncResult.GetAsyncCallbackWithPoisonContextAndUnhandledExceptionRedirect(new AsyncCallback(FacebookProvider.OnGetFriendsCompletedForEnumerateChanges)), asyncResult);
				});
			}
			else
			{
				FacebookProvider.ComputeDeletedUsers(providerState);
				List<string> userIdsToRequest = FacebookProvider.GetNewOrUpdatedUserIds(providerState);
				if (userIdsToRequest.Count > 0)
				{
					FacebookProvider.HandleExceptionsInBeginCall(asyncResult, delegate
					{
						asyncResult.PendingAsyncResult = providerState.Client.BeginGetUsers(accessToken, string.Join(",", userIdsToRequest), FacebookProvider.EnumerateChangesFields, asyncResult.GetAsyncCallbackWithPoisonContextAndUnhandledExceptionRedirect(new AsyncCallback(FacebookProvider.OnGetUsersCompletedForEnumerateChanges)), asyncResult);
					});
				}
				else
				{
					asyncResult.SetCompletedSynchronously();
					asyncResult.ProcessCompleted(new SyncProviderResultData(providerState.Changes, providerState.HasPermanentSyncErrors, providerState.HasTransientSyncErrors, providerState.Changes.Count, providerState.MoreItemsAvailable));
				}
			}
			return asyncResult;
		}

		private static void HandleExceptionsInBeginCall(AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult, Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				asyncResult.State.SyncLogSession.LogError((TSLID)37UL, FacebookProvider.Tracer, "Exception during BeginCall: {0}", new object[]
				{
					ex
				});
				FacebookProvider.CompleteProcessWithNonPromotableException(asyncResult, ex);
			}
		}

		private static void ComputeDeletedUsers(FacebookProviderState providerState)
		{
			HashSet<string> cloudLookup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			providerState.CloudUpdates.ForEach(delegate(FacebookUser u)
			{
				cloudLookup.Add(u.Id);
			});
			using (IEnumerator<string> cloudItemEnumerator = providerState.StateStorage.GetCloudItemEnumerator())
			{
				while (cloudItemEnumerator.MoveNext())
				{
					string text = cloudItemEnumerator.Current;
					if (text != null && !cloudLookup.Contains(text))
					{
						if (providerState.Changes.Count == providerState.MaxDownloadItems)
						{
							providerState.MoreItemsAvailable = true;
							break;
						}
						providerState.Changes.Add(new SyncChangeEntry(ChangeType.Delete, SchemaType.Contact, text));
					}
				}
			}
		}

		private static List<string> GetNewOrUpdatedUserIds(FacebookProviderState providerState)
		{
			List<string> list = new List<string>();
			foreach (FacebookUser facebookUser in providerState.CloudUpdates)
			{
				string text;
				string value;
				if (providerState.StateStorage.TryFindItem(facebookUser.Id, out text, out value))
				{
					if (string.IsNullOrEmpty(facebookUser.UpdatedTime) || !facebookUser.UpdatedTime.Equals(value))
					{
						list.Add(facebookUser.Id);
					}
				}
				else
				{
					list.Add(facebookUser.Id);
				}
				if (providerState.Changes.Count + list.Count == providerState.MaxDownloadItems + 1)
				{
					break;
				}
			}
			return list;
		}

		private static void OnGetFriendsCompletedForEnumerateChanges(IAsyncResult ar)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult = (AsyncResult<FacebookProviderState, SyncProviderResultData>)ar.AsyncState;
			FacebookProviderState state = asyncResult.State;
			try
			{
				FacebookUsersList result = state.Client.EndGetFriends(ar);
				FacebookProvider.ProcessUsersForEnumerateChanges(asyncResult, result);
			}
			catch (TimeoutException innerException)
			{
				asyncResult.ProcessCompleted(SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.ConnectionError, innerException));
			}
			catch (ProtocolException innerException2)
			{
				asyncResult.ProcessCompleted(SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, innerException2));
			}
			catch (SerializationException ex)
			{
				state.SyncLogSession.LogError((TSLID)576UL, FacebookProvider.Tracer, "Exception during EnumerateChanges when parsing Facebook contacts: {0}", new object[]
				{
					ex
				});
				FacebookProvider.CompleteProcessWithNonPromotableException(asyncResult, ex);
			}
			catch (Exception ex2)
			{
				state.SyncLogSession.LogError((TSLID)577UL, FacebookProvider.Tracer, "Exception during EnumerateChanges: {0}", new object[]
				{
					ex2
				});
				FacebookProvider.CompleteProcessWithNonPromotableException(asyncResult, ex2);
			}
		}

		private static void OnGetUsersCompletedForEnumerateChanges(IAsyncResult ar)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult = (AsyncResult<FacebookProviderState, SyncProviderResultData>)ar.AsyncState;
			FacebookProviderState state = asyncResult.State;
			try
			{
				FacebookUsersList result = state.Client.EndGetUsers(ar);
				FacebookProvider.ProcessUsersForEnumerateChanges(asyncResult, result);
			}
			catch (TimeoutException innerException)
			{
				asyncResult.ProcessCompleted(SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.ConnectionError, innerException));
			}
			catch (ProtocolException innerException2)
			{
				asyncResult.ProcessCompleted(SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, innerException2));
			}
			catch (SerializationException ex)
			{
				state.SyncLogSession.LogError((TSLID)578UL, FacebookProvider.Tracer, "Exception during EnumerateChanges when parsing Facebook contacts: {0}", new object[]
				{
					ex
				});
				FacebookProvider.CompleteProcessWithNonPromotableException(asyncResult, ex);
			}
			catch (Exception ex2)
			{
				state.SyncLogSession.LogError((TSLID)579UL, FacebookProvider.Tracer, "Exception during EnumerateChanges: {0}", new object[]
				{
					ex2
				});
				FacebookProvider.CompleteProcessWithNonPromotableException(asyncResult, ex2);
			}
		}

		private static void ProcessUsersForEnumerateChanges(AsyncResult<FacebookProviderState, SyncProviderResultData> asyncState, FacebookUsersList result)
		{
			FacebookProviderState state = asyncState.State;
			if (result.Users != null)
			{
				foreach (FacebookUser facebookUser in result.Users)
				{
					if (string.IsNullOrEmpty(facebookUser.Id))
					{
						state.SyncLogSession.LogError((TSLID)180UL, FacebookProvider.Tracer, "ProcessUsersForEnumerateChanges: user id is BLANK.  First name: '{0}'; Last name: '{1}'.  Skipping.", new object[]
						{
							facebookUser.FirstName,
							facebookUser.LastName
						});
					}
					else
					{
						if (state.Changes.Count == state.MaxDownloadItems)
						{
							state.MoreItemsAvailable = true;
							break;
						}
						ChangeType changeType = state.StateStorage.ContainsItem(facebookUser.Id) ? ChangeType.Change : ChangeType.Add;
						SyncChangeEntry syncChangeEntry = new SyncChangeEntry(changeType, SchemaType.Contact, facebookUser.Id, new FacebookContact(facebookUser, new ExDateTime(ExTimeZone.UtcTimeZone, state.Subscription.CreationTime)));
						syncChangeEntry.CloudVersion = facebookUser.UpdatedTime;
						state.Changes.Add(syncChangeEntry);
						state.TriggerContactDownloadEvent();
					}
				}
			}
			asyncState.ProcessCompleted(new SyncProviderResultData(state.Changes, state.HasPermanentSyncErrors, state.HasTransientSyncErrors, state.Changes.Count, state.MoreItemsAvailable));
		}

		public AsyncOperationResult<SyncProviderResultData> EndEnumerateChanges(IAsyncResult asyncResult)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<FacebookProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginAcknowledgeChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, bool hasPermanentSyncErrors, bool hasTransientSyncErrors, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncUtilities.ThrowIfArgumentNull("state", state);
			SyncUtilities.ThrowIfArgumentNull("changeList", changeList);
			FacebookProviderState facebookProviderState = (FacebookProviderState)state;
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult = new AsyncResult<FacebookProviderState, SyncProviderResultData>(this, facebookProviderState, callback, callbackState, syncPoisonContext);
			facebookProviderState.HasPermanentSyncErrors = hasPermanentSyncErrors;
			facebookProviderState.HasTransientSyncErrors = hasTransientSyncErrors;
			if (facebookProviderState.HasTransientSyncErrors)
			{
				facebookProviderState.SyncLogSession.LogVerbose((TSLID)3010UL, FacebookProvider.Tracer, "Sync had transient errors, setting MoreItemsAvailable = true.", new object[0]);
				facebookProviderState.MoreItemsAvailable = true;
			}
			if (!facebookProviderState.HasTransientSyncErrors && !facebookProviderState.MoreItemsAvailable)
			{
				string watermark;
				if (!string.IsNullOrEmpty(facebookProviderState.CurrentWatermark))
				{
					watermark = facebookProviderState.CurrentWatermark;
				}
				else
				{
					List<FacebookUser> list = new List<FacebookUser>();
					using (IEnumerator<string> cloudItemEnumerator = facebookProviderState.StateStorage.GetCloudItemEnumerator())
					{
						while (cloudItemEnumerator.MoveNext())
						{
							string text = cloudItemEnumerator.Current;
							string text2;
							string updatedTime;
							facebookProviderState.StateStorage.TryFindItem(text, out text2, out updatedTime);
							list.Add(new FacebookUser
							{
								Id = text,
								UpdatedTime = updatedTime
							});
						}
					}
					watermark = FacebookProvider.ComputeWatermark(list);
				}
				((StringWatermark)facebookProviderState.BaseWatermark).Save(watermark);
			}
			else
			{
				facebookProviderState.SyncLogSession.LogInformation((TSLID)3011UL, FacebookProvider.Tracer, "BeginAcknowledgeChanges: Skipping Watermark update, MoreItemsAvailable:{0}, HasTransientSyncErrors:{1}.", new object[]
				{
					facebookProviderState.MoreItemsAvailable,
					facebookProviderState.HasTransientSyncErrors
				});
			}
			asyncResult.SetCompletedSynchronously();
			asyncResult.ProcessCompleted(SyncProviderResultData.CreateAcknowledgeChangesResult(facebookProviderState.Changes, facebookProviderState.HasPermanentSyncErrors, facebookProviderState.HasTransientSyncErrors, (facebookProviderState.Changes == null) ? 0 : facebookProviderState.Changes.Count, facebookProviderState.MoreItemsAvailable));
			return asyncResult;
		}

		public AsyncOperationResult<SyncProviderResultData> EndAcknowledgeChanges(IAsyncResult asyncResult)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<FacebookProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginApplyChanges(SyncStorageProviderState state, IList<SyncChangeEntry> changeList, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			throw new NotSupportedException("Two-way sync is not supported by the Facebook provider");
		}

		public AsyncOperationResult<SyncProviderResultData> EndApplyChanges(IAsyncResult asyncResult)
		{
			throw new NotSupportedException("Two-way sync is not supported by the Facebook provider");
		}

		public IAsyncResult BeginGetItem(object itemRetrieverState, SyncChangeEntry item, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			FacebookProviderState facebookProviderState = (FacebookProviderState)itemRetrieverState;
			AsyncResult<FacebookProviderState, SyncChangeEntry> asyncResult = new AsyncResult<FacebookProviderState, SyncChangeEntry>(this, facebookProviderState, callback, callbackState, syncPoisonContext);
			facebookProviderState.ItemBeingRetrieved = item;
			asyncResult.ProcessCompleted(facebookProviderState.ItemBeingRetrieved);
			return asyncResult;
		}

		public AsyncOperationResult<SyncChangeEntry> EndGetItem(IAsyncResult asyncResult)
		{
			AsyncResult<FacebookProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<FacebookProviderState, SyncChangeEntry>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public void Cancel(IAsyncResult asyncResult)
		{
			AsyncResult<FacebookProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<FacebookProviderState, SyncProviderResultData>)asyncResult;
			FacebookProviderState state = asyncResult2.State;
			state.Client.Cancel();
		}

		public void CancelGetItem(IAsyncResult asyncResult)
		{
			AsyncResult<FacebookProviderState, SyncChangeEntry> asyncResult2 = (AsyncResult<FacebookProviderState, SyncChangeEntry>)asyncResult;
			FacebookProviderState state = asyncResult2.State;
			state.Client.Cancel();
		}

		private string ReadGraphApiEndpoint()
		{
			string graphApiEndpoint;
			try
			{
				graphApiEndpoint = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook().GraphApiEndpoint;
			}
			catch (ExchangeConfigurationException innerException)
			{
				throw SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.ConfigurationError, innerException);
			}
			return graphApiEndpoint;
		}

		internal static readonly Trace Tracer = ExTraceGlobals.FacebookProviderTracer;

		private static readonly string CheckForChangesFields = string.Join(",", new string[]
		{
			"id",
			"updated_time"
		});

		private static readonly string EnumerateChangesFields = string.Join(",", new string[]
		{
			"activities",
			"birthday",
			"first_name",
			"id",
			"interests",
			"education",
			"email",
			"last_name",
			"location",
			"mobile_phone",
			"link",
			"updated_time",
			"website",
			"work"
		});
	}
}
