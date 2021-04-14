using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupMailboxExchangeResourcesPublisher
	{
		public GroupMailboxExchangeResourcesPublisher(ADUser group, Guid activityId = default(Guid))
		{
			this.groupADUser = group;
			this.activityId = activityId;
		}

		public static bool IsPublishedVersionOutdated(int? publishedVersion)
		{
			return publishedVersion == null || publishedVersion < 1;
		}

		public bool Publish(int? publishedVersion)
		{
			if (!GroupMailboxExchangeResourcesPublisher.IsPublishedVersionOutdated(publishedVersion))
			{
				return true;
			}
			MailboxUrls mailboxUrls = new MailboxUrls(ExchangePrincipal.FromADUser(this.groupADUser, null), true);
			AADClient aadclient = AADClientFactory.Create(this.groupADUser.OrganizationId, GraphProxyVersions.Version14);
			if (aadclient == null)
			{
				string value = string.Format("AADClient was null for organization {0} and group {1}", this.groupADUser.OrganizationId, this.groupADUser.ExternalDirectoryObjectId);
				throw new LocalizedException(new LocalizedString(value));
			}
			if (publishedVersion == null)
			{
				Group group = aadclient.GetGroup(this.groupADUser.ExternalDirectoryObjectId, false);
				if (group == null)
				{
					FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
					{
						{
							FederatedDirectoryLogSchema.TraceTag.TaskName,
							"PublishResourcesToAAD"
						},
						{
							FederatedDirectoryLogSchema.TraceTag.ActivityId,
							this.activityId
						},
						{
							FederatedDirectoryLogSchema.TraceTag.CurrentAction,
							"GetGroupFromAAD"
						},
						{
							FederatedDirectoryLogSchema.TraceTag.Message,
							string.Format("Unable to find group in AAD. ExternalId={0}", this.groupADUser.ExternalDirectoryObjectId)
						}
					});
					return false;
				}
				string[] array;
				if (group.exchangeResources == null)
				{
					array = new string[0];
				}
				else
				{
					array = (from s in @group.exchangeResources
					select s.ToLower()).ToArray<string>();
				}
				string[] array2 = array;
				string[] array3 = (from s in mailboxUrls.ToExchangeResources()
				select s.ToLower()).ToArray<string>();
				if (array2.Length == array3.Length && array2.Except(array3).Count<string>() == 0)
				{
					return true;
				}
			}
			string[] exchangeResources = mailboxUrls.ToExchangeResources();
			aadclient.UpdateGroup(this.groupADUser.ExternalDirectoryObjectId, null, exchangeResources, null, null);
			this.TryNotifySharePointForExchangeResources(mailboxUrls);
			return true;
		}

		private void TryNotifySharePointForExchangeResources(MailboxUrls mailboxUrls)
		{
			OrganizationId orgId = this.groupADUser.OrganizationId;
			string externalDirectoryObjectId = this.groupADUser.ExternalDirectoryObjectId;
			Guid activityId = this.activityId;
			Stopwatch timer = Stopwatch.StartNew();
			Task.Factory.StartNew(delegate()
			{
				ICredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(orgId, "dummyRealm");
				using (SharePointNotification sharePointNotification = new SharePointNotification(SharePointNotification.NotificationType.Update, externalDirectoryObjectId, orgId, oauthCredentialsForAppToken, activityId))
				{
					foreach (KeyValuePair<string, string> keyValuePair in mailboxUrls.ToExchangeResourcesDictionary())
					{
						sharePointNotification.SetResourceValue(keyValuePair.Key, keyValuePair.Value, false);
					}
					sharePointNotification.Execute();
					string value = string.Format("Success;Group={0};Org={1};ElapsedTime={2}", externalDirectoryObjectId, orgId, timer.ElapsedMilliseconds);
					FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
					{
						{
							FederatedDirectoryLogSchema.TraceTag.TaskName,
							"UpdateSiteCollectionTask"
						},
						{
							FederatedDirectoryLogSchema.TraceTag.ActivityId,
							activityId
						},
						{
							FederatedDirectoryLogSchema.TraceTag.CurrentAction,
							"SharePointSetMailboxUrls"
						},
						{
							FederatedDirectoryLogSchema.TraceTag.Message,
							value
						}
					});
				}
			}).ContinueWith(delegate(Task t)
			{
				Exception ex = null;
				if (t.Exception != null)
				{
					ex = t.Exception.InnerException;
				}
				string value = string.Format("Failed;Group={0};Org={1};ElapsedTime={2}", externalDirectoryObjectId, orgId, timer.ElapsedMilliseconds);
				FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.ExceptionTag>
				{
					{
						FederatedDirectoryLogSchema.ExceptionTag.TaskName,
						"UpdateSiteCollectionTask"
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.ActivityId,
						activityId
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.CurrentAction,
						"SharePointSetMailboxUrls"
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.ExceptionType,
						(ex != null) ? ex.GetType().ToString() : "Unknown"
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.ExceptionDetail,
						ex
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.Message,
						value
					}
				});
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		public const int CurrentResourcesVersion = 1;

		private readonly Guid activityId;

		private readonly ADUser groupADUser;
	}
}
