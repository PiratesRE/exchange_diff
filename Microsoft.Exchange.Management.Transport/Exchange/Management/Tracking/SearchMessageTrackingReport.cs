using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.Management.Tracking
{
	[Cmdlet("Search", "MessageTrackingReport", DefaultParameterSetName = "ParamSetSearchAsSender", SupportsShouldProcess = true)]
	public sealed class SearchMessageTrackingReport : GetTenantADObjectWithIdentityTaskBase<MailboxIdParameter, MessageTrackingSearchResult>
	{
		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "ParamSetSearchAsSender", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "ParamSetSearchAsRecip", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "ParamSetSearchAsRecip")]
		public SmtpAddress? Sender
		{
			get
			{
				return (SmtpAddress?)base.Fields["Sender"];
			}
			set
			{
				base.Fields["Sender"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		public SmtpAddress[] Recipients
		{
			get
			{
				return (SmtpAddress[])base.Fields["Recipients"];
			}
			set
			{
				base.Fields["Recipients"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsRecip")]
		public string Subject
		{
			get
			{
				return (string)base.Fields["MessageSubject"];
			}
			set
			{
				base.Fields["MessageSubject"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsRecip")]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return (Unlimited<uint>)base.Fields["ResultSize"];
			}
			set
			{
				base.Fields["ResultSize"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsRecip")]
		public string MessageId
		{
			get
			{
				return (string)base.Fields["MessageId"];
			}
			set
			{
				base.Fields["MessageId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsRecip")]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		public string MessageEntryId
		{
			get
			{
				return (string)base.Fields["MessageEntryId"];
			}
			set
			{
				base.Fields["MessageEntryId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsRecip")]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		public SwitchParameter BypassDelegateChecking
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassDelegationCheck"] ?? false);
			}
			set
			{
				base.Fields["BypassDelegationCheck"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsRecip")]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		public SwitchParameter DoNotResolve
		{
			get
			{
				return (SwitchParameter)(base.Fields["DoNotResolve"] ?? false);
			}
			set
			{
				base.Fields["DoNotResolve"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsSender")]
		[Parameter(Mandatory = false, ParameterSetName = "ParamSetSearchAsRecip")]
		public TraceLevel TraceLevel
		{
			get
			{
				return (TraceLevel)(base.Fields["TraceLevel"] ?? TraceLevel.Low);
			}
			set
			{
				base.Fields["TraceLevel"] = value;
			}
		}

		internal bool IsOwaJumpOffPointRequest
		{
			get
			{
				return !string.IsNullOrEmpty(this.MessageEntryId);
			}
		}

		private bool TrackingAsSender
		{
			get
			{
				return this.Sender == null;
			}
		}

		internal static DiagnosticsLevel GetDiagnosticsLevel(TraceLevel traceLevel, bool explicitlySpecified)
		{
			if (!explicitlySpecified && (!ServerCache.Instance.WriteToStatsLogs || ServerCache.Instance.DiagnosticsDisabled))
			{
				return DiagnosticsLevel.None;
			}
			switch (traceLevel)
			{
			case TraceLevel.Low:
				return DiagnosticsLevel.Basic;
			case TraceLevel.Medium:
				return DiagnosticsLevel.Verbose;
			case TraceLevel.High:
				return DiagnosticsLevel.Etw;
			default:
				throw new InvalidOperationException("TraceLevel not supported");
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSearchMessageTracking;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is TrackingSearchException || DataAccessHelper.IsDataAccessKnownException(exception) || base.IsKnownException(exception);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 336, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\Tracking\\SearchMessageTrackingReport.cs");
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			ADUser aduser = Utils.FindById<ADUser>(this.Identity, base.TenantGlobalCatalogSession);
			if (aduser == null)
			{
				ExTraceGlobals.SearchLibraryTracer.TraceError<MailboxIdParameter>((long)this.GetHashCode(), "Could not lookup user specified in MailboxIdParameter: {0}", this.Identity);
			}
			else
			{
				this.trackedMailbox = TrackedUser.Create(aduser);
			}
			if (this.trackedMailbox == null || this.trackedMailbox.ADRecipient == null)
			{
				this.trackedMailbox = null;
				ExTraceGlobals.SearchLibraryTracer.TraceError<MailboxIdParameter>((long)this.GetHashCode(), "Error, could not resolve user specified in Identity: {0}", this.Identity);
				return base.ResolveCurrentOrganization();
			}
			OrganizationId organizationId = this.trackedMailbox.ADRecipient.OrganizationId;
			if (!base.ExecutingUserOrganizationId.Equals(OrganizationId.ForestWideOrgId) && !base.ExecutingUserOrganizationId.Equals(organizationId) && !organizationId.OrganizationalUnit.IsDescendantOf(base.ExecutingUserOrganizationId.OrganizationalUnit))
			{
				ExTraceGlobals.SearchLibraryTracer.TraceError<MailboxIdParameter>((long)this.GetHashCode(), "Error, executing user is not parent of tracked mailbox: {0}", this.Identity);
				throw new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(this.Identity.ToString()));
			}
			return organizationId;
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (base.NeedSuppressingPiiData && base.ExchangeRunspaceConfig != null)
			{
				this.ResolvePiiParameters();
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
			}
			if (!base.Fields.IsModified("ResultSize"))
			{
				this.ResultSize = 50U;
			}
		}

		protected override void InternalValidate()
		{
			if (this.Identity == null || this.trackedMailbox == null)
			{
				this.HandleError((this.Identity == null) ? "Null identity in Search" : "null trackedmailbox in search", Strings.TrackingPermanentError, CoreStrings.InvalidIdentityForAdmin, null, ErrorCategory.InvalidData);
			}
			if (this.IsOwaJumpOffPointRequest)
			{
				this.SetSenderAndMessageId(this.trackedMailbox.ADUser);
			}
			if (base.Fields.Contains("Sender") && this.Sender != null && !this.Sender.Value.IsValidAddress)
			{
				base.WriteError(new ArgumentException(CoreStrings.InvalidSender, "Sender"), ErrorCategory.InvalidArgument, this.Sender.Value);
			}
			if (this.Recipients != null && this.Recipients.Length > 0)
			{
				foreach (SmtpAddress smtpAddress in this.Recipients)
				{
					if (!smtpAddress.IsValidAddress)
					{
						base.WriteError(new ArgumentException(Strings.InvalidRecipient), ErrorCategory.InvalidArgument, smtpAddress);
					}
				}
			}
			if (this.Sender != null)
			{
				this.trackedSender = TrackedUser.Create(this.Sender.Value.ToString(), base.TenantGlobalCatalogSession);
				if (this.trackedSender == null)
				{
					this.HandleError("Null trackedSender", Strings.TrackingPermanentError, CoreStrings.InvalidSenderForAdmin, null, ErrorCategory.InvalidData);
				}
				else if (!this.trackedSender.IsSupportedForTrackingAsSender)
				{
					this.HandleError("trackedSender type is unsupported.", CoreStrings.UnsupportedSenderForTracking, CoreStrings.UnsupportedSenderForTracking, null, ErrorCategory.InvalidData);
				}
			}
			if (this.Recipients != null && this.Recipients.Length > 0)
			{
				this.trackedRecipients = new TrackedUser[this.Recipients.Length];
				for (int j = 0; j < this.Recipients.Length; j++)
				{
					this.trackedRecipients[j] = TrackedUser.Create(this.Recipients[j].ToString(), base.TenantGlobalCatalogSession);
					if (this.trackedRecipients[j] == null)
					{
						this.HandleError(string.Format("Null trackedRecipient for recipient: {0}", this.Recipients[j]), Strings.TrackingPermanentError, CoreStrings.InvalidRecipientForAdmin(this.Recipients[j].ToString()), null, ErrorCategory.InvalidData);
					}
				}
			}
			bool flag = this.BypassDelegateChecking.IsPresent && this.BypassDelegateChecking.ToBool();
			ADObjectId executingUserId;
			base.TryGetExecutingUserId(out executingUserId);
			string debugMessage;
			if (!flag && !Utils.AccessCheck((ADObjectId)this.trackedMailbox.ADUser.Identity, executingUserId, base.TenantGlobalCatalogSession, out debugMessage))
			{
				this.HandleError(debugMessage, CoreStrings.TrackingSearchNotAuthorized, CoreStrings.TrackingSearchNotAuthorized, null, ErrorCategory.PermissionDenied);
			}
			if (!string.IsNullOrEmpty(this.MessageId))
			{
				string messageId = this.MessageId;
				if (messageId.StartsWith("<") && messageId.EndsWith(">"))
				{
					this.MessageId = messageId.Substring(1, messageId.Length - 2);
				}
			}
			base.InternalValidate();
		}

		private void SetSenderAndMessageId(ADUser mailbox)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			ExTraceGlobals.SearchLibraryTracer.TraceError((long)this.GetHashCode(), "EID: " + this.MessageEntryId);
			byte[] entryId;
			try
			{
				entryId = Convert.FromBase64String(this.MessageEntryId);
			}
			catch (FormatException e)
			{
				this.HandleError("Could not decode base64 value: " + this.MessageEntryId, Strings.InvalidMessageIdentity, e);
				return;
			}
			StoreObjectId itemId;
			try
			{
				itemId = StoreObjectId.FromProviderSpecificId(entryId);
			}
			catch (CorruptDataException e2)
			{
				this.HandleError("Could not create store ID from base64 decoded value.", Strings.InvalidMessageIdentity, e2);
				return;
			}
			ExchangePrincipal exchangePrincipal;
			try
			{
				exchangePrincipal = ExchangePrincipal.FromADUser(base.TenantGlobalCatalogSession.SessionSettings, mailbox, RemotingOptions.AllowCrossSite);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Search-MessageTrackingReport"))
				{
					PropertyDefinition[] propsToReturn = new PropertyDefinition[]
					{
						MessageItemSchema.SenderAddressType,
						MessageItemSchema.SenderEmailAddress,
						ItemSchema.InternetMessageId,
						ItemSchema.SentTime,
						ItemSchema.Subject,
						StoreObjectSchema.ItemClass
					};
					using (MessageItem messageItem = Item.BindAsMessage(mailboxSession, itemId, propsToReturn))
					{
						object obj = messageItem.TryGetProperty(ItemSchema.InternetMessageId);
						if (!(obj is PropertyError))
						{
							this.MessageId = (string)obj;
						}
						obj = messageItem.TryGetProperty(MessageItemSchema.SenderAddressType);
						if (!(obj is PropertyError))
						{
							text = (string)obj;
						}
						obj = messageItem.TryGetProperty(MessageItemSchema.SenderEmailAddress);
						if (!(obj is PropertyError))
						{
							text2 = (string)obj;
						}
						obj = messageItem.TryGetProperty(ItemSchema.SentTime);
						if (!(obj is PropertyError))
						{
							this.clientSubmitTime = new ExDateTime?((ExDateTime)obj);
						}
						obj = messageItem.TryGetProperty(ItemSchema.Subject);
						if (!(obj is PropertyError))
						{
							this.storeItemSubject = (string)obj;
						}
						obj = messageItem.TryGetProperty(StoreObjectSchema.ItemClass);
						if (!(obj is PropertyError))
						{
							this.messageClass = (string)obj;
						}
					}
				}
			}
			catch (StorageTransientException ex)
			{
				this.HandleError("StorageTransientException trying to retrieve message data: " + ex.Message + "|" + ex.ToString(), Strings.TrackingTransientError, Strings.ExceptionStorageOther(ex.ErrorCode, ex.Message), ex, ErrorCategory.ReadError);
				return;
			}
			catch (StoragePermanentException ex2)
			{
				this.HandleError("StoragePermanentException trying to retrieve message data: " + ex2.Message + "|" + ex2.ToString(), Strings.TrackingPermanentError, Strings.ExceptionStorageOther(ex2.ErrorCode, ex2.Message), ex2, ErrorCategory.InvalidOperation);
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				this.HandleError("Could not determine sender's routing type.", CoreStrings.InvalidSender);
			}
			else
			{
				if (string.IsNullOrEmpty(text2))
				{
					this.HandleError("Could not determine sender's email address.", CoreStrings.InvalidSender);
					return;
				}
				if (!SupportedMessageClasses.Classes.Contains(this.messageClass))
				{
					ExTraceGlobals.TaskTracer.TraceError<string>((long)this.GetHashCode(), "PR_MESSAGE_CLASS: {0} not supported for tracking", this.messageClass);
					base.WriteError(new TrackingExceptionMessageTypeNotSupported(), ErrorCategory.InvalidType, null);
				}
				SmtpAddress value = SmtpAddress.Empty;
				if (string.CompareOrdinal(text, "EX") == 0)
				{
					try
					{
						ADRecipient adrecipient = base.TenantGlobalCatalogSession.FindByLegacyExchangeDN(text2);
						if (adrecipient == null)
						{
							this.HandleError("Could not resolve the sender in AD.", CoreStrings.InvalidSender);
							return;
						}
						value = adrecipient.PrimarySmtpAddress;
						goto IL_383;
					}
					catch (ObjectNotFoundException e3)
					{
						this.HandleError("Could not resolve the sender in AD, the sender is not found.", CoreStrings.InvalidSender, e3);
						return;
					}
					catch (NonUniqueRecipientException e4)
					{
						this.HandleError("Could not resolve the sender in AD, the sender is non-unique.", CoreStrings.InvalidSender, e4);
						return;
					}
				}
				if (string.CompareOrdinal(text, "SMTP") != 0)
				{
					this.HandleError("Sender's routing type is not supported: " + text, CoreStrings.InvalidSender);
					return;
				}
				value = new SmtpAddress(text2);
				IL_383:
				if (!value.Equals(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()))
				{
					this.Sender = new SmtpAddress?(value);
				}
				return;
			}
		}

		private void ResolvePiiParameters()
		{
			SmtpAddress value;
			if (this.Sender != null && Utils.TryResolveRedactedSmtpAddress(this.Sender.Value, this, out value))
			{
				this.Sender = new SmtpAddress?(value);
			}
			Utils.TryResolveRedactedSmtpAddressArray(this.Recipients, this);
			string subject;
			if (Utils.TryResolveRedactedString(this.Subject, this, out subject))
			{
				this.Subject = subject;
			}
		}

		private void HandleError(string debugMessage, LocalizedString message)
		{
			this.HandleError(debugMessage, message, null);
		}

		private void HandleError(string debugMessage, LocalizedString userMessage, LocalizedString adminMessage, Exception e, ErrorCategory errorCategory)
		{
			Utils.HandleError(this, debugMessage, this.BypassDelegateChecking ? adminMessage : userMessage, true, false, errorCategory, null);
		}

		private void HandleError(string debugMessage, LocalizedString message, Exception e)
		{
			Utils.HandleError(this, debugMessage, message, true, false, ErrorCategory.InvalidData, null);
		}

		protected override void InternalProcessRecord()
		{
			bool flag;
			if ("Exchange Control Panel".Equals(base.Host.Name, StringComparison.OrdinalIgnoreCase))
			{
				flag = ServerCache.Instance.InitializeIfNeeded(HostId.ECPApplicationPool);
			}
			else
			{
				flag = ServerCache.Instance.InitializeIfNeeded(HostId.PowershellApplicationPool);
			}
			if (!flag)
			{
				Utils.HandleError(this, "Cannot initialize AD configuration", CoreStrings.TrackingErrorFailedToInitialize, this.BypassDelegateChecking, false, ErrorCategory.ObjectNotFound, null);
			}
			DirectoryContext directoryContext = null;
			int num = 0;
			TrackingEventBudget trackingEventBudget = null;
			bool flag2 = false;
			ClientContext clientContext = null;
			ClientSecurityContext clientSecurityContext = null;
			try
			{
				TrackingEventBudget.AcquireThread();
				if (this.TraceLevel == TraceLevel.High)
				{
					CommonDiagnosticsLogTracer traceWriter = new CommonDiagnosticsLogTracer();
					TraceWrapper.SearchLibraryTracer.Register(traceWriter);
					BaseTrace.CurrentThreadSettings.EnableTracing();
				}
				if (base.ExchangeRunspaceConfig == null)
				{
					ExTraceGlobals.TaskTracer.TraceError((long)this.GetHashCode(), "Search-MessageTrackingReport permissions cannot be retrieved because the ExchangeRunspaceConfiguration is null");
					this.HandleError("Search-MessageTrackingReport permissions cannot be retrieved because the ExchangeRunspaceConfiguration is null", CoreStrings.TrackingSearchNotAuthorized);
				}
				MultiValuedProperty<CultureInfo> executingUserLanguages = base.ExchangeRunspaceConfig.ExecutingUserLanguages;
				CultureInfo clientCulture = (executingUserLanguages != null && executingUserLanguages.Count > 0) ? executingUserLanguages[0] : CultureInfo.InvariantCulture;
				try
				{
					clientSecurityContext = Utils.GetSecurityContextForUser(base.ExchangeRunspaceConfig.SecurityAccessToken, base.ExchangeRunspaceConfig.DelegatedPrincipal, this.trackedMailbox.ADUser);
					clientContext = ClientContext.Create(clientSecurityContext, base.CurrentOrganizationId, null, null, clientCulture, null);
					OrganizationId currentOrganizationId = base.TenantGlobalCatalogSession.SessionSettings.CurrentOrganizationId;
					TrackingErrorCollection errors = new TrackingErrorCollection();
					TimeSpan timeout = Utils.GetTimeout(this.BypassDelegateChecking);
					trackingEventBudget = new TrackingEventBudget(errors, timeout);
					directoryContext = new DirectoryContext(clientContext, base.TenantGlobalCatalogSession.SessionSettings.CurrentOrganizationId, base.GlobalConfigSession, this.ConfigurationSession, base.TenantGlobalCatalogSession, trackingEventBudget, SearchMessageTrackingReport.GetDiagnosticsLevel(this.TraceLevel, base.Fields.IsModified("TraceLevel")), errors, false);
					directoryContext.Acquire();
					flag2 = true;
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Task, Names<DeliveryReportsTask>.Map[0]);
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[0]);
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Usr, this.trackedMailbox.SmtpAddress.ToString());
					string value = Names<DeliveryReportsSource>.Map[this.IsOwaJumpOffPointRequest ? 0 : 1];
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Src, value);
					if (this.BypassDelegateChecking)
					{
						ADObjectId adobjectId;
						if (base.TryGetExecutingUserId(out adobjectId))
						{
							directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.ExUser, adobjectId.Name);
						}
						else
						{
							ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "ExecutingUserId is null.");
						}
					}
					if (this.Sender != null)
					{
						directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Sender, this.Sender.ToString());
					}
					if (this.MessageId != null)
					{
						directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Mid, this.MessageId.ToString());
					}
					directoryContext.DiagnosticsContext.WriteEvent();
					this.searchMessageTracking = new SearchMessageTrackingReportImpl(directoryContext, SearchScope.World, this.trackedMailbox, this.trackedSender, null, this.trackedRecipients, null, this.Subject, this.MessageId, this.ResultSize, false, false, true, false);
					this.searchMessageTracking.Execute();
				}
				catch (AuthzException ex)
				{
					this.HandleError("AutzException occurred:" + ex.Message, CoreStrings.TrackingSearchNotAuthorized);
				}
				List<MessageTrackingSearchResult> list = this.searchMessageTracking.RunAuthorizationFilter(this.IsOwaJumpOffPointRequest, this.clientSubmitTime, this.storeItemSubject);
				if (list != null)
				{
					list = this.searchMessageTracking.FilterResultsBySubjectAndRecipients(list, executingUserLanguages);
				}
				num = (int)Math.Min(this.ResultSize.Value, 4096U);
				num = Math.Min(num, (list == null) ? 0 : list.Count);
				List<MessageTrackingSearchResult> list2 = new List<MessageTrackingSearchResult>(num);
				for (int i = 0; i < num; i++)
				{
					MessageTrackingSearchResult internalMessageTrackingSearchResult = list[i];
					MessageTrackingSearchResult messageTrackingSearchResult = new MessageTrackingSearchResult(internalMessageTrackingSearchResult);
					if (!this.TrackingAsSender)
					{
						messageTrackingSearchResult.RecipientAddresses = new SmtpAddress[]
						{
							this.trackedMailbox.SmtpAddress
						};
						messageTrackingSearchResult.RecipientDisplayNames = new string[]
						{
							this.trackedMailbox.DisplayName
						};
					}
					list2.Add(messageTrackingSearchResult);
				}
				if (list2.Count == 0 && this.IsOwaJumpOffPointRequest)
				{
					if (this.searchMessageTracking.Errors.Errors.Count == 0)
					{
						this.HandleZeroResultsFromJumpOffPoint();
					}
				}
				else
				{
					MessageTrackingSearchResult.FillDisplayNames(list2, base.TenantGlobalCatalogSession);
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[7]);
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[0]);
					directoryContext.DiagnosticsContext.WriteEvent();
					foreach (MessageTrackingSearchResult messageTrackingSearchResult2 in list2)
					{
						directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Time, messageTrackingSearchResult2.SubmittedDateTime);
						directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Mid, messageTrackingSearchResult2.MessageTrackingReportId.ToString());
						directoryContext.DiagnosticsContext.WriteEvent();
						this.WriteResult(messageTrackingSearchResult2);
					}
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[7]);
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[1]);
					directoryContext.DiagnosticsContext.WriteEvent();
				}
			}
			catch (TrackingTransientException ex2)
			{
				Utils.HandleTrackingException(directoryContext, ex2, this, !this.IsOwaJumpOffPointRequest, this.BypassDelegateChecking);
			}
			catch (TrackingFatalException ex3)
			{
				Utils.HandleTrackingException(directoryContext, ex3, this, !this.IsOwaJumpOffPointRequest, this.BypassDelegateChecking);
			}
			catch (DataSourceOperationException ex4)
			{
				Utils.HandleError(this, ex4.ToString(), ex4.LocalizedString, this.BypassDelegateChecking, false, ErrorCategory.InvalidData, null);
			}
			catch (DataValidationException ex5)
			{
				Utils.HandleError(this, ex5.ToString(), ex5.LocalizedString, this.BypassDelegateChecking, false, ErrorCategory.InvalidData, null);
			}
			catch (TransientException ex6)
			{
				Utils.HandleError(this, ex6.ToString(), ex6.LocalizedString, this.BypassDelegateChecking, true, ErrorCategory.InvalidData, null);
			}
			finally
			{
				if (directoryContext != null)
				{
					Utils.WriteWarnings(this, directoryContext, this.BypassDelegateChecking, directoryContext.Errors.Errors);
				}
				if (trackingEventBudget != null)
				{
					trackingEventBudget.Dispose();
				}
				if (directoryContext != null)
				{
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Task, Names<DeliveryReportsTask>.Map[0]);
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[1]);
					directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Cnt, num);
					directoryContext.DiagnosticsContext.WriteEvent();
					Utils.WriteDiagnostics(this, directoryContext.DiagnosticsContext, base.NeedSuppressingPiiData);
					if (flag2)
					{
						directoryContext.Yield();
					}
				}
				if (this.TraceLevel == TraceLevel.High)
				{
					TraceWrapper.SearchLibraryTracer.Unregister();
					BaseTrace.CurrentThreadSettings.DisableTracing();
				}
				TrackingEventBudget.ReleaseThread();
				if (directoryContext == null || Utils.AreAnyErrorsLocalToThisForest(directoryContext.Errors.Errors))
				{
					PerfCounterData.ResultCounter.AddFailure();
				}
				else if (!this.IsOwaJumpOffPointRequest)
				{
					PerfCounterData.ResultCounter.AddSuccess();
				}
				if (ServerCache.Instance.HostId == HostId.ECPApplicationPool)
				{
					InfoWorkerMessageTrackingPerformanceCounters.MessageTrackingFailureRateTask.RawValue = (long)PerfCounterData.ResultCounter.FailurePercentage;
				}
				if (clientContext != null)
				{
					clientContext.Dispose();
				}
				if (clientSecurityContext != null)
				{
					clientSecurityContext.Dispose();
				}
			}
		}

		private void HandleZeroResultsFromJumpOffPoint()
		{
			ExTraceGlobals.TaskTracer.TraceError((long)this.GetHashCode(), "No results were found for message submitted via OLK/OWA jump off point, trying to determine why");
			if (this.clientSubmitTime == null)
			{
				ExTraceGlobals.TaskTracer.TraceError((long)this.GetHashCode(), "Client submit time property not available and no search results found for this message, message logs have probably expired");
				base.WriteError(new TrackingExceptionNoResultsDueToLogsNotFound(), ErrorCategory.ObjectNotFound, this.Identity);
			}
			ExDateTime now = ExDateTime.Now;
			ExTraceGlobals.TaskTracer.TraceDebug<ExDateTime?, ExDateTime>((long)this.GetHashCode(), "Client submit time was: {0}, current time is: {1}", this.clientSubmitTime, now);
			if (this.clientSubmitTime - now > TimeSpan.FromDays(14.0))
			{
				ExTraceGlobals.TaskTracer.TraceError((long)this.GetHashCode(), "Message submitted more than 14 days ago, logs probably expired");
				base.WriteError(new TrackingExceptionNoResultsDueToLogsExpired(), ErrorCategory.ObjectNotFound, this.Identity);
			}
			if (now - this.clientSubmitTime < ServerCache.Instance.ExpectedLoggingLatency)
			{
				ExTraceGlobals.TaskTracer.TraceError<double>((long)this.GetHashCode(), "Message submitted less than {0} seconds ago, logs have probably not yet been picked up", ServerCache.Instance.ExpectedLoggingLatency.TotalSeconds);
				base.WriteError(new TrackingWarningNoResultsDueToTrackingTooEarly(), ErrorCategory.ObjectNotFound, this.Identity);
			}
			ExTraceGlobals.TaskTracer.TraceError((long)this.GetHashCode(), "Could not determine why we didn't get any results, logging could be off or this message produced no logs for some other reason");
			base.WriteError(new TrackingExceptionNoResultsDueToLogsNotFound(), ErrorCategory.ObjectNotFound, this.Identity);
		}

		private SearchMessageTrackingReportImpl searchMessageTracking;

		private TrackedUser trackedMailbox;

		private TrackedUser trackedSender;

		private TrackedUser[] trackedRecipients;

		private ExDateTime? clientSubmitTime;

		private string storeItemSubject;

		private string messageClass;
	}
}
