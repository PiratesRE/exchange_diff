using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.Management.Tasks.MailboxSearch;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Search", "Mailbox", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SearchMailbox : RecipientObjectActionTask<MailboxOrMailUserIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override MailboxOrMailUserIdParameter Identity
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Mailbox")]
		public MailboxIdParameter TargetMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["TargetMailbox"];
			}
			set
			{
				base.Fields["TargetMailbox"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Mailbox")]
		public string TargetFolder
		{
			get
			{
				return (string)base.Fields["TargetFolder"];
			}
			set
			{
				base.Fields["TargetFolder"] = value.Trim();
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "Mailbox")]
		public SwitchParameter DeleteContent
		{
			get
			{
				return (SwitchParameter)(base.Fields["DeleteContent"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DeleteContent"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SearchQuery
		{
			get
			{
				return (string)base.Fields["SearchQuery"];
			}
			set
			{
				base.Fields["SearchQuery"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SearchDumpster
		{
			get
			{
				return (SwitchParameter)(base.Fields["SearchDumpster"] ?? new SwitchParameter(true));
			}
			set
			{
				base.Fields["SearchDumpster"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SearchDumpsterOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["SearchDumpsterOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SearchDumpsterOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Mailbox")]
		public LoggingLevel LogLevel
		{
			get
			{
				return (LoggingLevel)(base.Fields["LogLevel"] ?? LoggingLevel.Basic);
			}
			set
			{
				base.Fields["LogLevel"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Mailbox")]
		public SwitchParameter LogOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["LogOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["LogOnly"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EstimateResult")]
		public SwitchParameter EstimateResultOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["EstimateResultOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EstimateResultOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeUnsearchableItems
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeUnsearchableItems"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeUnsearchableItems"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter DoNotIncludeArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["DoNotIncludeArchive"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DoNotIncludeArchive"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		private void OnProgressEvent(object sender, SearchProgressEvent e)
		{
			int percentCompleted = e.PercentCompleted;
			base.WriteProgress(e.Activity, e.StatusDescription, percentCompleted);
		}

		private void OnExceptionEvent(object sender, SearchExceptionEvent e)
		{
			if (e.SourceIndex == null)
			{
				base.ThrowTerminatingError(e.Exception, ErrorCategory.InvalidArgument, null);
				return;
			}
			this.errorMessages.Add(Strings.SearchWorkerError(this.sourceUser.DisplayName, e.Exception.Message).ToString());
			this.WriteError(e.Exception, ErrorCategory.ReadError, e.SourceIndex, false);
		}

		private void OnRequestLogBodyEvent(object sender, RequestLogBodyEvent e)
		{
			this.ComposeLogItemBody(e.ItemBody);
		}

		private StoreId GetTargetFolderId(MailboxSession targetMailbox)
		{
			StoreId result = null;
			using (Folder folder = Folder.Bind(targetMailbox, DefaultFolderType.Root))
			{
				if (string.IsNullOrEmpty(this.TargetFolder))
				{
					result = folder.Id;
				}
				else
				{
					using (Folder folder2 = Folder.Create(targetMailbox, folder.Id, StoreObjectType.Folder, this.TargetFolder, CreateMode.OpenIfExists))
					{
						folder2.Save();
						folder2.Load();
						result = folder2.Id;
					}
				}
			}
			return result;
		}

		private static void ReplaceLogFieldTags(StringBuilder sb, Globals.LogFields logField, object value)
		{
			sb = sb.Replace(logField.ToLabelTag(), LocalizedDescriptionAttribute.FromEnum(typeof(Globals.LogFields), logField) + ":");
			sb = sb.Replace(logField.ToValueTag(), string.Format("{0}", value));
		}

		private void ComposeLogItemBody(Body itemBody)
		{
			if (itemBody == null)
			{
				throw new ArgumentNullException("itemBody");
			}
			using (TextWriter textWriter = itemBody.OpenTextWriter(BodyFormat.TextHtml))
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (StreamReader streamReader = new StreamReader(Assembly.GetAssembly(typeof(SearchMailboxExecuter)).GetManifestResourceStream("SimpleLogMailTemplate.htm")))
				{
					stringBuilder.Append(streamReader.ReadToEnd());
				}
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.LastStartTime, this.searchMailboxExecuter.SearchStartTime);
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SearchQuery, this.SearchQuery);
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.TargetMailbox, this.targetUser.Id.DomainUserName());
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SearchDumpster, this.SearchDumpster);
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.LogLevel, this.LogLevel);
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SourceRecipients, (from x in this.searchMailboxExecuter.SearchMailboxCriteria.SearchUserScope
				select x.Id.DomainUserName()).AggregateOfDefault((string s, string x) => s + ", " + x));
				ADObjectId adobjectId = null;
				base.TryGetExecutingUserId(out adobjectId);
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.LastRunBy, (adobjectId == null) ? string.Empty : adobjectId.DomainUserName());
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.NumberMailboxesToSearch, this.searchMailboxExecuter.SearchMailboxCriteria.SearchUserScope.Length);
				if (this.DeleteContent.IsPresent)
				{
					if (base.ParameterSetName == "Mailbox")
					{
						SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SearchOperation, Strings.CopyAndDeleteOperation);
					}
					else
					{
						SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SearchOperation, Strings.DeleteOperation);
					}
				}
				else if (this.LogOnly.IsPresent)
				{
					SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SearchOperation, Strings.LogOnlyOperation);
				}
				else
				{
					SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SearchOperation, Strings.CopyOperation);
				}
				string str = this.errorMessages.AggregateOfDefault((string s, string x) => s + ", " + x);
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.Errors, str.ValueOrDefault(Strings.LogMailNone));
				long num = 0L;
				ByteQuantifiedSize byteQuantifiedSize = ByteQuantifiedSize.Zero;
				if (this.searchMailboxExecuter.SearchState != SearchState.InProgress)
				{
					foreach (SearchMailboxResult searchMailboxResult in this.searchMailboxExecuter.GetSearchResult())
					{
						num += (long)searchMailboxResult.ResultItemsCount;
						byteQuantifiedSize += searchMailboxResult.ResultItemsSize;
					}
				}
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ResultNumber, num);
				SearchMailbox.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ResultSize, byteQuantifiedSize);
				SearchState searchState = (this.searchMailboxExecuter.SearchState == SearchState.InProgress) ? SearchState.Failed : this.searchMailboxExecuter.SearchState;
				stringBuilder = stringBuilder.Replace(Globals.LogFields.LogMailHeader.ToLabelTag(), Strings.LogMailSimpleHeader(searchState.ToString()));
				stringBuilder = stringBuilder.Replace(Globals.LogFields.LogMailSeeAttachment.ToLabelTag(), Strings.LogMailSeeAttachment);
				stringBuilder = stringBuilder.Replace(Globals.LogFields.LogMailFooter.ToLabelTag(), Strings.LogMailFooter);
				textWriter.Write(stringBuilder.ToString());
			}
		}

		private List<SearchMailboxAction> CreateSearchActions()
		{
			List<SearchMailboxAction> list = new List<SearchMailboxAction>();
			if (this.LogOnly.IsPresent)
			{
				list.Add(new LogSearchMailboxAction(this.LogLevel));
			}
			if (this.DeleteContent.IsPresent)
			{
				list.Add(new LogSearchMailboxAction(this.LogLevel));
				if (this.targetUser != null)
				{
					list.Add(new CopySearchMailboxAction());
				}
				list.Add(new DeleteSearchMailboxAction());
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}

		private void InternalPrevalidate()
		{
			this.WriteWarning(Strings.SearchMaxResultCountWarning(10000));
			if (this.DeleteContent.IsPresent)
			{
				if (this.IncludeUnsearchableItems == true)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.DeletionAndUnsearchableNotPermitted), ErrorCategory.InvalidArgument, null);
				}
				if (this.LogOnly.IsPresent)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.DeletionAndLogOnlyNotPermitted), ErrorCategory.InvalidArgument, null);
				}
			}
			if (this.LogOnly.IsPresent && this.LogLevel == LoggingLevel.Suppress)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.WrongLogLevel), ErrorCategory.InvalidArgument, null);
			}
			if (this.SearchQuery != null && this.SearchQuery.Trim() == "")
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.EmptySearchQuery), ErrorCategory.InvalidArgument, null);
			}
		}

		private void WriteResult(SearchMailboxResult[] searchResults)
		{
			foreach (SearchMailboxResult sendToPipeline in searchResults)
			{
				base.WriteObject(sendToPipeline);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (!this.DeleteContent.IsPresent)
				{
					return Strings.ConfirmSearchMailboxTask(this.Identity.ToString());
				}
				return Strings.ConfirmSearchMailboxDeleteContent(this.Identity.ToString());
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			IConfigurable dataObject = null;
			ADSessionSettingsFactory.RunWithInactiveMailboxVisibilityEnablerForDatacenter(delegate
			{
				dataObject = this.<>n__FabricatedMethod9();
			});
			return dataObject;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalBeginProcessing();
				if (base.ParameterSetName == "Identity" && !this.DeleteContent.IsPresent)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.EmptyTargetMailbox), ErrorCategory.InvalidArgument, null);
				}
				this.InternalPrevalidate();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					if (this.DeleteContent.IsPresent && !this.Force && !base.ShouldContinue(Strings.ConfirmSearchMailboxDeleteContent(this.Identity.ToString())))
					{
						TaskLogger.LogExit();
					}
					else
					{
						ADUser dataObject = this.DataObject;
						if (dataObject == null || (dataObject.RecipientType != RecipientType.UserMailbox && !RemoteMailbox.IsRemoteMailbox(dataObject.RecipientTypeDetails)))
						{
							this.WriteWarning(Strings.ErrorInvalidRecipientType(dataObject.ToString(), dataObject.RecipientType.ToString()));
						}
						else if (this.sourceUserIds.ContainsKey(dataObject.Id))
						{
							this.WriteWarning(Strings.SearchDuplicateSource(dataObject.ToString()));
						}
						else
						{
							Utils.VerifyMailboxVersion(dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
							if (base.ScopeSet != null)
							{
								Utils.VerifyIsInScopes(dataObject, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
							}
							this.targetSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 682, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Search\\SearchMailbox.cs");
							if (base.ParameterSetName == "Mailbox" && this.targetUser == null)
							{
								try
								{
									this.targetSession.SessionSettings.IncludeInactiveMailbox = false;
									this.targetUser = (base.GetDataObject<ADUser>(this.TargetMailbox, this.targetSession, this.RootId, new LocalizedString?(Strings.ErrorUserNotFound(this.TargetMailbox.ToString())), new LocalizedString?(Strings.ExceptionUserObjectAmbiguous)) as ADUser);
								}
								finally
								{
									if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
									{
										this.targetSession.SessionSettings.IncludeInactiveMailbox = true;
									}
								}
								if (this.targetUser == null || this.targetUser.RecipientType != RecipientType.UserMailbox)
								{
									base.ThrowTerminatingError(new ArgumentException(Strings.ErrorInvalidRecipientType(this.targetUser.ToString(), this.targetUser.RecipientType.ToString())), ErrorCategory.InvalidArgument, "TargetMailbox");
								}
								if (base.ScopeSet != null)
								{
									Utils.VerifyIsInScopes(this.targetUser, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
								}
							}
							if (base.ParameterSetName == "Mailbox" && dataObject.Id.Equals(this.targetUser.Id))
							{
								this.WriteWarning(Strings.SearchSourceTargetTheSame(dataObject.ToString()));
							}
							else
							{
								this.sourceUser = this.targetSession.ReadMiniRecipient(dataObject.Id, null);
							}
						}
					}
				}
			}
			catch (ManagementObjectNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, base.CurrentObjectIndex);
			}
			catch (ManagementObjectAmbiguousException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, base.CurrentObjectIndex);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalStateReset()
		{
			using (new ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler())
			{
				base.InternalStateReset();
				this.sourceUser = null;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (this.sourceUser != null)
			{
				this.sourceUserIds.Add(this.sourceUser.Id, null);
				this.sourceUserList.Add(this.sourceUser);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			try
			{
				if (!base.HasErrors && this.sourceUserList.Count > 0)
				{
					List<SearchUser> list = new List<SearchUser>();
					foreach (MiniRecipient miniRecipient in this.sourceUserList)
					{
						list.Add(new SearchUser(miniRecipient.Id, miniRecipient.DisplayName, miniRecipient.ServerLegacyDN));
					}
					SearchMailboxCriteria searchMailboxCriteria = new SearchMailboxCriteria(Thread.CurrentThread.CurrentCulture, this.SearchQuery, list.ToArray());
					searchMailboxCriteria.SearchDumpster = this.SearchDumpster.IsPresent;
					searchMailboxCriteria.SearchDumpsterOnly = this.SearchDumpsterOnly.IsPresent;
					searchMailboxCriteria.IncludeUnsearchableItems = this.IncludeUnsearchableItems;
					searchMailboxCriteria.IncludePersonalArchive = !this.DoNotIncludeArchive;
					searchMailboxCriteria.ExcludePurgesFromDumpster = this.DeleteContent.IsPresent;
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 829, "InternalEndProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Search\\SearchMailbox.cs");
					this.searchMailboxExecuter = new SearchMailboxExecuter(this.targetSession, tenantOrTopologyConfigurationSession, searchMailboxCriteria, this.targetUser);
					GenericIdentity executingIdentityFromRunspace = SearchUtils.GetExecutingIdentityFromRunspace(base.ExchangeRunspaceConfig);
					this.searchMailboxExecuter.TargetFolder = this.TargetFolder;
					SearchMailboxExecuter searchMailboxExecuter = this.searchMailboxExecuter;
					searchMailboxExecuter.ProgressHandler = (EventHandler<SearchProgressEvent>)Delegate.Combine(searchMailboxExecuter.ProgressHandler, new EventHandler<SearchProgressEvent>(this.OnProgressEvent));
					SearchMailboxExecuter searchMailboxExecuter2 = this.searchMailboxExecuter;
					searchMailboxExecuter2.SearchExceptionHandler = (EventHandler<SearchExceptionEvent>)Delegate.Combine(searchMailboxExecuter2.SearchExceptionHandler, new EventHandler<SearchExceptionEvent>(this.OnExceptionEvent));
					SearchMailboxExecuter searchMailboxExecuter3 = this.searchMailboxExecuter;
					searchMailboxExecuter3.RequestLogBodyHandler = (EventHandler<RequestLogBodyEvent>)Delegate.Combine(searchMailboxExecuter3.RequestLogBodyHandler, new EventHandler<RequestLogBodyEvent>(this.OnRequestLogBodyEvent));
					this.searchMailboxExecuter.LogLevel = this.LogLevel;
					this.searchMailboxExecuter.SearchActions = this.CreateSearchActions();
					this.searchMailboxExecuter.OwnerIdentity = executingIdentityFromRunspace;
					this.searchMailboxExecuter.ExecuteSearch();
					if (this.searchMailboxExecuter.SearchState != SearchState.Stopped)
					{
						base.WriteProgress(Strings.ProgressCompleting, Strings.ProgressCompletingSearch, 100);
						this.WriteResult(this.searchMailboxExecuter.GetSearchResult());
					}
				}
			}
			catch (ParserException exception)
			{
				base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (SearchQueryEmptyException exception2)
			{
				base.ThrowTerminatingError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (StoragePermanentException exception3)
			{
				base.ThrowTerminatingError(exception3, ErrorCategory.InvalidArgument, null);
			}
			catch (StorageTransientException exception4)
			{
				base.ThrowTerminatingError(exception4, ErrorCategory.InvalidArgument, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalStopProcessing()
		{
			TaskLogger.LogEnter();
			if (this.searchMailboxExecuter != null)
			{
				this.searchMailboxExecuter.Abort();
			}
			TaskLogger.LogExit();
		}

		private const string ParameterTargetMailbox = "TargetMailbox";

		private const string ParameterTargetFolder = "TargetFolder";

		private const string ParameterSearchQuery = "SearchQuery";

		private const string ParameterSearchDumpster = "SearchDumpster";

		private const string ParameterSearchDumpsterOnly = "SearchDumpsterOnly";

		private const string ParameterLogLevel = "LogLevel";

		private const string ParameterLogOnly = "LogOnly";

		private const string ParameterEstimateResultOnly = "EstimateResultOnly";

		private const string ParameterIncludeUnsearchableItems = "IncludeUnsearchableItems";

		private const string ParameterDoNotIncludeArchive = "DoNotIncludeArchive";

		private const string ParameterIncludeRemoteAccounts = "IncludeRemoteAccounts";

		private const string ParameterDeleteContent = "DeleteContent";

		private const string ParameterSetMailbox = "Mailbox";

		private const string ParameterSetEstimateResult = "EstimateResult";

		private Dictionary<ADObjectId, object> sourceUserIds = new Dictionary<ADObjectId, object>();

		private List<MiniRecipient> sourceUserList = new List<MiniRecipient>();

		private ADUser targetUser;

		private MiniRecipient sourceUser;

		private SearchMailboxExecuter searchMailboxExecuter;

		private List<string> errorMessages = new List<string>();

		private IRecipientSession targetSession;
	}
}
