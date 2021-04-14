using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UnifiedGroupsTask
	{
		public UnifiedGroupsTask(ADUser accessingUser, IRecipientSession adSession) : this(accessingUser, adSession, UnifiedGroupsTask.GetActivityId())
		{
		}

		public UnifiedGroupsTask(ADUser accessingUser, IRecipientSession adSession, Guid activityId)
		{
			this.AccessingUser = accessingUser;
			this.ADSession = adSession;
			this.AADClient = AADClientFactory.Create(accessingUser);
			this.ActivityId = activityId;
		}

		public UnifiedGroupsTask.UnifiedGroupsAction ErrorAction { get; private set; }

		public Exception ErrorException { get; private set; }

		public string ErrorCode { get; private set; }

		private protected ADUser AccessingUser { protected get; private set; }

		private protected IRecipientSession ADSession { protected get; private set; }

		protected ICredentials ActAsUserCredentials
		{
			get
			{
				if (this.actAsUserCredentials == null)
				{
					this.actAsUserCredentials = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(this.AccessingUser.OrganizationId, this.AccessingUser, null);
				}
				return this.actAsUserCredentials;
			}
		}

		private protected AADClient AADClient { protected get; private set; }

		protected bool IsAADEnabled
		{
			get
			{
				return this.AADClient != null;
			}
		}

		protected bool IsSharePointEnabled
		{
			get
			{
				return this.AADClient != null && SharePointUrl.GetRootSiteUrl(this.AccessingUser.OrganizationId) != null;
			}
		}

		protected UnifiedGroupsTask.UnifiedGroupsAction CurrentAction { get; set; }

		private protected Guid ActivityId { protected get; private set; }

		protected abstract string TaskName { get; }

		public bool Run()
		{
			return this.RunWithLogging();
		}

		protected static bool QueueTask(UnifiedGroupsTask task)
		{
			return ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						task.RunWithLogging();
					});
				}
				catch (GrayException arg)
				{
					UnifiedGroupsTask.Tracer.TraceError<Guid, GrayException>(0L, "ActivityId: {0}. GrayException: {1}", task.ActivityId, arg);
					FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
					{
						{
							FederatedDirectoryLogSchema.TraceTag.TaskName,
							task.TaskName
						},
						{
							FederatedDirectoryLogSchema.TraceTag.ActivityId,
							task.ActivityId
						},
						{
							FederatedDirectoryLogSchema.TraceTag.CurrentAction,
							task.CurrentAction
						},
						{
							FederatedDirectoryLogSchema.TraceTag.Message,
							"GrayException: " + arg
						}
					});
				}
			});
		}

		protected abstract void RunInternal();

		private static Guid GetActivityId()
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope == null)
			{
				return Guid.NewGuid();
			}
			return currentActivityScope.ActivityId;
		}

		private bool RunWithLogging()
		{
			Exception ex = null;
			try
			{
				this.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.None;
				this.RunInternal();
			}
			catch (AADDataException ex2)
			{
				this.ErrorCode = ex2.Code.ToString();
				ex = ex2;
			}
			catch (LocalizedException ex3)
			{
				ex = ex3;
			}
			catch (WebException ex4)
			{
				ex = ex4;
			}
			catch (ClientRequestException ex5)
			{
				ex = ex5;
			}
			catch (ServerException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				UnifiedGroupsTask.Tracer.TraceError<UnifiedGroupsTask.UnifiedGroupsAction, Exception>((long)this.GetHashCode(), "UnifiedGroupsTask.RunWithLogging: RunInternal threw an exception. CurrentAction: {0}, exception: {1}", this.CurrentAction, ex);
				FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.ExceptionTag>
				{
					{
						FederatedDirectoryLogSchema.ExceptionTag.TaskName,
						this.TaskName
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.ActivityId,
						this.ActivityId
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.ExceptionType,
						ex.GetType()
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.ExceptionDetail,
						ex
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.CurrentAction,
						this.CurrentAction
					},
					{
						FederatedDirectoryLogSchema.ExceptionTag.Message,
						"RunInternal threw an exception"
					}
				});
				this.ErrorAction = this.CurrentAction;
				this.ErrorException = ex;
				return false;
			}
			return true;
		}

		internal static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private ICredentials actAsUserCredentials;

		public enum UnifiedGroupsAction
		{
			None,
			AADCreate,
			AADAddOwnerAsMember,
			AADCompleteCallback,
			AADPostCreate,
			AADUpdate,
			AADDelete,
			ExchangeCreate,
			ExchangeUpdate,
			ExchangeDelete,
			ResolveExternalIdentities,
			SharePointCreate,
			SharePointSetMailboxUrls,
			SharePointUpdate,
			SharePointDelete,
			Completed
		}
	}
}
