using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.AirSync.SyncStateConverter;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class Command : IDisposeTrackable, IDisposable, ITask, ITaskTimeout
	{
		private static Stack<Command> CommandStack
		{
			get
			{
				if (Command.commandStack == null)
				{
					Command.commandStack = new Stack<Command>();
				}
				return Command.commandStack;
			}
		}

		internal static OrganizationId CurrentOrganizationId
		{
			get
			{
				return Command.CurrentCommand.User.OrganizationId;
			}
		}

		internal static Command CurrentCommand
		{
			get
			{
				if (Command.CommandStack.Count != 0)
				{
					return Command.CommandStack.Peek();
				}
				return null;
			}
		}

		protected static string MachineName
		{
			get
			{
				return Command.machineName;
			}
		}

		public static void DetermineDeviceAccessState(IOrganizationSettingsData organizationSettingsData, string deviceType, string deviceModel, string userAgent, string deviceOS, out DeviceAccessState deviceAccessState, out DeviceAccessStateReason accessStateReason, out ADObjectId deviceAccessControlRule)
		{
			if (organizationSettingsData == null)
			{
				throw new ArgumentNullException("organizationSettingsData");
			}
			if (deviceType == null)
			{
				throw new ArgumentNullException("deviceType");
			}
			if (deviceModel == null)
			{
				throw new ArgumentNullException("deviceModel");
			}
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "Determine access state for device type '{0}', model '{1}', OS '{2}', UserAgent '{3}'.", new object[]
			{
				deviceType,
				deviceModel,
				deviceOS,
				userAgent
			});
			if (!organizationSettingsData.IsRulesListEmpty)
			{
				DeviceAccessRuleData deviceAccessRuleData;
				if (!string.IsNullOrEmpty(userAgent))
				{
					deviceAccessRuleData = organizationSettingsData.EvaluateDevice(DeviceAccessCharacteristic.UserAgent, userAgent);
					if (deviceAccessRuleData != null)
					{
						accessStateReason = DeviceAccessStateReason.DeviceRule;
						deviceAccessControlRule = deviceAccessRuleData.Identity;
						deviceAccessState = deviceAccessRuleData.AccessState;
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] UserAgent: '{0}' - Org Settings Data evaluated to true.  AccessState: '{1}', Rule: '{2}', Reason: '{3}'", new object[]
						{
							userAgent,
							deviceAccessState,
							deviceAccessControlRule,
							accessStateReason
						});
						return;
					}
				}
				if (!string.IsNullOrEmpty(deviceOS))
				{
					deviceAccessRuleData = organizationSettingsData.EvaluateDevice(DeviceAccessCharacteristic.DeviceOS, deviceOS);
					if (deviceAccessRuleData != null)
					{
						accessStateReason = DeviceAccessStateReason.DeviceRule;
						deviceAccessControlRule = deviceAccessRuleData.Identity;
						deviceAccessState = deviceAccessRuleData.AccessState;
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] DeviceOS: '{0}' - Org Settings Data evaluated to true.  AccessState: '{1}', Rule: '{2}', Reason: '{3}'", new object[]
						{
							deviceOS,
							deviceAccessState,
							deviceAccessControlRule,
							accessStateReason
						});
						return;
					}
				}
				deviceAccessRuleData = organizationSettingsData.EvaluateDevice(DeviceAccessCharacteristic.DeviceModel, deviceModel);
				if (deviceAccessRuleData != null)
				{
					accessStateReason = DeviceAccessStateReason.DeviceRule;
					deviceAccessControlRule = deviceAccessRuleData.Identity;
					deviceAccessState = deviceAccessRuleData.AccessState;
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] DeviceModel: '{0}' - Org Settings Data evaluated to true.  AccessState: '{1}', Rule: '{2}', Reason: '{3}'", new object[]
					{
						deviceModel,
						deviceAccessState,
						deviceAccessControlRule,
						accessStateReason
					});
					return;
				}
				deviceAccessRuleData = organizationSettingsData.EvaluateDevice(DeviceAccessCharacteristic.DeviceType, deviceType);
				if (deviceAccessRuleData != null)
				{
					accessStateReason = DeviceAccessStateReason.DeviceRule;
					deviceAccessControlRule = deviceAccessRuleData.Identity;
					deviceAccessState = deviceAccessRuleData.AccessState;
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] DeviceType: '{0}' - Org Settings Data evaluated to true.  AccessState: '{1}', Rule: '{2}', Reason: '{3}'", new object[]
					{
						deviceType,
						deviceAccessState,
						deviceAccessControlRule,
						accessStateReason
					});
					return;
				}
			}
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] Check organization global device access.");
			deviceAccessControlRule = null;
			accessStateReason = DeviceAccessStateReason.Global;
			switch (organizationSettingsData.DefaultAccessLevel)
			{
			case DeviceAccessLevel.Allow:
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] Global allow due to org Settings Data.");
				deviceAccessState = DeviceAccessState.Allowed;
				return;
			case DeviceAccessLevel.Block:
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] Global block due to org Settings Data.");
				deviceAccessState = DeviceAccessState.Blocked;
				return;
			case DeviceAccessLevel.Quarantine:
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] Global quarantine due to org Settings Data.");
				deviceAccessState = DeviceAccessState.Quarantined;
				return;
			default:
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "[Command.DetermineDeviceAccessState] Global UNKNOWN due to org Settings Data.");
				deviceAccessState = DeviceAccessState.Unknown;
				if (Command.CurrentCommand != null && Command.CurrentCommand.ProtocolLogger != null)
				{
					Command.CurrentCommand.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.Error, "ABQGlobalUnknown");
				}
				return;
			}
		}

		internal static void ClearContextDataInTls()
		{
			Command.CommandStack.Pop();
		}

		internal static float GetFolderHierarchyICSPercentage()
		{
			long num = Command.numSkinnyICSFolderChecks;
			long num2 = Command.numFatDeepTraversalFolderChecks;
			double num3 = (double)(num + num2);
			if (num3 != 0.0)
			{
				return (float)((double)num / num3) * 100f;
			}
			return 0f;
		}

		internal static void DetectPolicyChange(IPolicyData policyData, IGlobalInfo mbxInfo, int version, out bool policyIsCompatibleWithDevice)
		{
			policyIsCompatibleWithDevice = true;
			if (policyData == null)
			{
				if (mbxInfo.LastPolicyXMLHash != null)
				{
					mbxInfo.PolicyKeyNeeded = 0U;
					mbxInfo.PolicyKeyWaitingAck = Command.GenerateNewPolicyKey(mbxInfo);
					mbxInfo.LastPolicyXMLHash = null;
					mbxInfo.DevicePolicyApplied = null;
				}
				return;
			}
			policyIsCompatibleWithDevice = policyData.GetVersionCompatibility(version);
			int hashCode = policyData.GetHashCode(version);
			if (mbxInfo.LastPolicyXMLHash != null && hashCode == mbxInfo.LastPolicyXMLHash.Value)
			{
				return;
			}
			mbxInfo.PolicyKeyNeeded = Command.GenerateNewPolicyKey(mbxInfo);
			mbxInfo.PolicyKeyWaitingAck = Command.GenerateNewPolicyKey(mbxInfo);
			mbxInfo.LastPolicyXMLHash = new int?(hashCode);
			mbxInfo.DevicePolicyApplied = policyData.Identity;
		}

		internal bool TryParseDeviceOSFromUserAgent(out string deviceOS)
		{
			deviceOS = null;
			if (!GlobalSettings.DeviceTypesToParseOSVersion.Contains(this.Context.DeviceIdentity.DeviceType.ToLower()))
			{
				return false;
			}
			AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "TryParseDeviceOSFromUserAgent:: deviceUserAgent:{0}, deviceType:{1}", this.EffectiveUserAgent, this.Context.DeviceIdentity.DeviceType);
			if (!string.IsNullOrEmpty(this.EffectiveUserAgent))
			{
				if (this.Context.DeviceIdentity.DeviceType.ToUpper().Contains("SAMSUNG"))
				{
					int num = this.EffectiveUserAgent.LastIndexOf(".");
					if (num > 0)
					{
						string text = this.EffectiveUserAgent.Substring(num + 1);
						int num2;
						if (int.TryParse(text, out num2))
						{
							deviceOS = string.Format("{0} {1}", "Android", text.Replace('0', '.'));
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "TryParseDeviceOSFromUserAgent:: deviceOSWithVersion:{0}", deviceOS);
							return true;
						}
					}
				}
				else
				{
					int num3 = this.EffectiveUserAgent.IndexOf("-");
					if (num3 > 0)
					{
						string text2 = this.EffectiveUserAgent.Substring(0, num3);
						deviceOS = text2.Replace("/", " ");
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "TryParseDeviceOSFromUserAgent:: deviceOSWithVersion:{0}", deviceOS);
						return true;
					}
				}
			}
			return false;
		}

		protected void SaveLatestIcsFolderHierarchySnapshot(FolderHierarchyChangeDetector.SyncHierarchyManifestState latestState)
		{
			if (latestState != null)
			{
				this.DeviceSyncStateMetadata.RecordLatestFolderHierarchySnapshot(latestState);
				return;
			}
			this.DeviceSyncStateMetadata.RecordLatestFolderHierarchySnapshot(this.MailboxSession, this.Context);
		}

		protected Command.IcsFolderCheckResults PerformICSFolderHierarchyChangeCheck(ref SyncState folderIdMappingState, out FolderHierarchyChangeDetector.SyncHierarchyManifestState latestState)
		{
			Command.IcsFolderCheckResults icsFolderCheckResults = Command.IcsFolderCheckResults.ChangesNeedDeepCheck;
			latestState = null;
			Command.IcsFolderCheckResults icsFolderCheckResults2;
			try
			{
				FolderHierarchyChangeDetector.MailboxChangesManifest folderHierarchyICSChanges = this.DeviceSyncStateMetadata.GetFolderHierarchyICSChanges(this.MailboxSession, out latestState, this.Context);
				if (folderHierarchyICSChanges != null)
				{
					if (!folderHierarchyICSChanges.HasChanges)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[Command.PerformICSFolderHierarchyChangeCheck] Folder Hierarchy ICS check says state has not changed.");
						icsFolderCheckResults = Command.IcsFolderCheckResults.NoChanges;
						return icsFolderCheckResults;
					}
					if (folderHierarchyICSChanges.DeletedFolders.Count > 0)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[Command.PerformICSFolderHierarchyChangeCheck] Folder Hierarchy ICS check says folders were deleted.  Skipping deep traversal.");
						icsFolderCheckResults = Command.IcsFolderCheckResults.ChangesNoDeepCheck;
						return icsFolderCheckResults;
					}
					if (folderIdMappingState == null)
					{
						folderIdMappingState = this.SyncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
					}
					if (folderIdMappingState == null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[Command.PerformICSFolderHierarchyChangeCheck] FolderIdMapping sync state is missing.  Must do folder sync.");
						icsFolderCheckResults = Command.IcsFolderCheckResults.ChangesNoDeepCheck;
						return icsFolderCheckResults;
					}
					FolderTree folderTree = (FolderTree)folderIdMappingState[CustomStateDatumType.FullFolderTree];
					if (folderTree == null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[Command.PerformICSFolderHierarchyChangeCheck] FullFolderTree data is missing.  Must do folder sync.");
						icsFolderCheckResults = Command.IcsFolderCheckResults.ChangesNoDeepCheck;
						return icsFolderCheckResults;
					}
					foreach (KeyValuePair<StoreObjectId, string> keyValuePair in folderHierarchyICSChanges.ChangedFolders)
					{
						MailboxSyncItemId folderId = MailboxSyncItemId.CreateForNewItem(keyValuePair.Key);
						if (!folderTree.Contains(folderId))
						{
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[Command.PerformICSFolderHierarchyChangeCheck] Folder '{0}' was an add.  Must do folder sync.", keyValuePair.Value);
							icsFolderCheckResults = Command.IcsFolderCheckResults.ChangesNoDeepCheck;
							return icsFolderCheckResults;
						}
					}
				}
				icsFolderCheckResults2 = icsFolderCheckResults;
			}
			finally
			{
				switch (icsFolderCheckResults)
				{
				case Command.IcsFolderCheckResults.NoChanges:
				case Command.IcsFolderCheckResults.ChangesNoDeepCheck:
					Interlocked.Increment(ref Command.numSkinnyICSFolderChecks);
					this.ProtocolLogger.SetValue(ProtocolLoggerData.QuickHierarchyChangeCheck, "T");
					break;
				case Command.IcsFolderCheckResults.ChangesNeedDeepCheck:
					Interlocked.Increment(ref Command.numFatDeepTraversalFolderChecks);
					this.ProtocolLogger.SetValue(ProtocolLoggerData.QuickHierarchyChangeCheck, "F");
					break;
				}
			}
			return icsFolderCheckResults2;
		}

		private static uint GenerateNewPolicyKey(IGlobalInfo mbxInfo)
		{
			byte[] array = new byte[4];
			uint num2;
			using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				uint num = 0U;
				while (num == 0U || num == mbxInfo.PolicyKeyWaitingAck || num == mbxInfo.PolicyKeyNeeded)
				{
					rngcryptoServiceProvider.GetNonZeroBytes(array);
					num = BitConverter.ToUInt32(array, 0);
				}
				num2 = num;
			}
			return num2;
		}

		private static string ReadStringFromStream(Stream stream, Encoding encoding)
		{
			AirSyncDiagnostics.Assert(stream != null);
			AirSyncDiagnostics.Assert(encoding != null);
			if (!stream.CanRead)
			{
				return string.Empty;
			}
			byte[] array = new byte[4096];
			int num = 0;
			for (;;)
			{
				int num2 = stream.Read(array, num, array.Length - num);
				if (num2 == 0)
				{
					break;
				}
				num += num2;
				if (num == array.Length)
				{
					byte[] array2 = new byte[array.Length * 2];
					array.CopyTo(array2, 0);
					array = array2;
				}
			}
			return encoding.GetString(array, 0, num);
		}

		public AnnotationsManager RequestAnnotations { get; private set; }

		public Command()
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.WorkloadSettings = new WorkloadSettings(WorkloadType.Eas, false);
			this.RequestAnnotations = new AnnotationsManager();
		}

		public bool PerUserTracingEnabled
		{
			get
			{
				return this.perUserTracingEnabled;
			}
			set
			{
				this.perUserTracingEnabled = value;
			}
		}

		internal ProtocolLogger ProtocolLogger
		{
			get
			{
				return this.context.ProtocolLogger;
			}
		}

		internal DeviceIdentity DeviceIdentity
		{
			get
			{
				return this.context.DeviceIdentity;
			}
		}

		internal XmlElement XmlRequest
		{
			get
			{
				return this.context.Request.CommandXml;
			}
		}

		internal Stream InputStream
		{
			get
			{
				return this.context.Request.InputStream;
			}
		}

		internal string EffectiveUserAgent
		{
			get
			{
				if (this.GlobalInfo != null && string.IsNullOrEmpty(this.context.Request.UserAgent))
				{
					return this.GlobalInfo.UserAgent;
				}
				return this.context.Request.UserAgent;
			}
		}

		internal XmlDocument XmlResponse
		{
			get
			{
				return this.context.Response.XmlDocument;
			}
			set
			{
				this.context.Response.XmlDocument = value;
			}
		}

		internal Stream OutputStream
		{
			get
			{
				return this.context.Response.OutputStream;
			}
		}

		internal IAirSyncRequest Request
		{
			get
			{
				return this.context.Request;
			}
		}

		internal int Version
		{
			get
			{
				return this.context.Request.Version;
			}
		}

		internal bool HasOutlookExtensions
		{
			get
			{
				return this.context.Request.HasOutlookExtensions;
			}
		}

		internal LazyAsyncResult LazyAsyncResult
		{
			set
			{
				this.result = value;
			}
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				if (this.securityContextAndSession != null)
				{
					return this.securityContextAndSession.MailboxSession;
				}
				return null;
			}
		}

		internal SyncStateStorage SyncStateStorage
		{
			get
			{
				return this.syncStateStorage;
			}
		}

		internal ISyncStatusData SyncStatusSyncData
		{
			get
			{
				return this.syncStatusSyncData;
			}
			set
			{
				this.syncStatusSyncData = value;
			}
		}

		internal GlobalInfo GlobalInfo { get; private set; }

		internal UserSyncStateMetadata UserSyncStateMetadata
		{
			get
			{
				return UserSyncStateMetadataCache.Singleton.Get(this.MailboxSession, null);
			}
		}

		internal DeviceSyncStateMetadata DeviceSyncStateMetadata
		{
			get
			{
				return this.UserSyncStateMetadata.GetDevice(this.MailboxSession, this.DeviceIdentity, null);
			}
		}

		internal MeetingOrganizerSyncState MeetingOrganizerSyncState { get; private set; }

		internal MailboxLogger MailboxLogger
		{
			get
			{
				return this.mailboxLogger;
			}
		}

		internal bool MailboxLoggingEnabled
		{
			get
			{
				if (this.mailboxLoggingEnabled == null)
				{
					if (this.MailboxSession == null)
					{
						return false;
					}
					this.mailboxLoggingEnabled = new bool?(SyncStateStorage.GetMailboxLoggingEnabled(this.MailboxSession, this.Context));
				}
				return this.mailboxLoggingEnabled.Value;
			}
		}

		internal int RequestId
		{
			get
			{
				return this.requestId;
			}
		}

		internal Stopwatch RequestWaitWatch
		{
			get
			{
				return this.requestWaitWatch;
			}
			set
			{
				this.requestWaitWatch = value;
			}
		}

		internal virtual bool RequiresPolicyCheck
		{
			get
			{
				return true;
			}
		}

		internal virtual bool ShouldOpenGlobalSyncState
		{
			get
			{
				return this.RequiresPolicyCheck;
			}
		}

		internal virtual int MinVersion
		{
			get
			{
				return 20;
			}
		}

		internal virtual int MaxVersion
		{
			get
			{
				return int.MaxValue;
			}
		}

		public IAirSyncContext Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
				if (value != null)
				{
					this.budget = this.context.User.Budget;
				}
			}
		}

		internal virtual bool ShouldSaveSyncStatus
		{
			get
			{
				return false;
			}
		}

		internal Dictionary<StoreObjectId, Dictionary<AttachmentId, string>> InlineAttachmentContentIdLookUp
		{
			get
			{
				if (this.inlineAttachmentContentIdLookUp == null)
				{
					this.inlineAttachmentContentIdLookUp = new Dictionary<StoreObjectId, Dictionary<AttachmentId, string>>(1);
				}
				return this.inlineAttachmentContentIdLookUp;
			}
		}

		protected virtual bool ShouldOpenSyncState
		{
			get
			{
				return true;
			}
		}

		protected abstract string RootNodeName { get; }

		protected virtual string RootNodeNamespace
		{
			get
			{
				return this.RootNodeName + ":";
			}
		}

		protected ExPerformanceCounter PerfCounter
		{
			get
			{
				return this.perfCounter;
			}
			set
			{
				this.perfCounter = value;
			}
		}

		protected ExDateTime NextPolicyRefreshTime
		{
			get
			{
				return this.nextPolicyRefreshTime;
			}
		}

		protected bool SendServerUpgradeHeader
		{
			get
			{
				return this.sendServerUpgradeHeader;
			}
			set
			{
				this.sendServerUpgradeHeader = value;
			}
		}

		protected internal IAirSyncUser User
		{
			get
			{
				return this.context.User;
			}
		}

		protected internal bool PartialFailure { get; set; }

		private protected DeviceAccessState CurrentAccessState { protected get; private set; }

		private protected DeviceAccessState PreviousAccessState { protected get; private set; }

		protected bool IsInQuarantinedState
		{
			get
			{
				return this.CurrentAccessState == DeviceAccessState.Quarantined || this.CurrentAccessState == DeviceAccessState.DeviceDiscovery;
			}
		}

		protected bool IsQuarantineMailAvailable
		{
			get
			{
				return this.IsInQuarantinedState && this.GlobalInfo != null && (this.GlobalInfo.ABQMailState == ABQMailState.MailPosted || this.GlobalInfo.ABQMailState == ABQMailState.MailSent);
			}
		}

		protected SecurityContextAndSession SecurityContextAndSession
		{
			get
			{
				return this.securityContextAndSession;
			}
		}

		internal virtual bool RightsManagementSupportFlag
		{
			get
			{
				return false;
			}
		}

		private ADDeviceManager DeviceManager
		{
			get
			{
				if (this.deviceManager == null)
				{
					this.deviceManager = new ADDeviceManager(this.context);
				}
				return this.deviceManager;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Command>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SetContextDataInTls()
		{
			Command.CommandStack.Push(this);
		}

		internal void RegisterDisposableData(IDisposable data)
		{
			if (data != null)
			{
				this.dataToBeDisposed.Add(data);
			}
		}

		internal void ReleaseDisposableData()
		{
			foreach (IDisposable disposable in this.dataToBeDisposed)
			{
				disposable.Dispose();
			}
			this.dataToBeDisposed.Clear();
		}

		internal virtual XmlDocument GetInvalidParametersXml()
		{
			return null;
		}

		internal void AddInteractiveCall()
		{
			IEasDeviceBudget easDeviceBudget = this.Budget as IEasDeviceBudget;
			if (easDeviceBudget != null)
			{
				easDeviceBudget.AddInteractiveCall();
			}
		}

		internal virtual void SetStateData(Command.StateData data)
		{
		}

		internal abstract Command.ExecutionState ExecuteCommand();

		protected virtual bool IsInteractiveCommand
		{
			get
			{
				return false;
			}
		}

		protected virtual Validator GetValidator()
		{
			return new Validator(this.Version, this.HasOutlookExtensions);
		}

		internal virtual bool ValidateXml()
		{
			bool flag2;
			using (this.context.Tracker.Start(TimeId.CommandValidateXML))
			{
				Validator validator = this.GetValidator();
				bool flag = validator.ValidateXml(this.XmlRequest, this.RootNodeName);
				if (!flag)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, this.RootNodeName + " Command validation failed!");
					this.XmlResponse = this.GetValidationErrorXml();
					this.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.Error, "InvalidXml");
					if (validator.ValidationErrors.Count > 0)
					{
						for (int i = 0; i < validator.ValidationErrors.Count; i++)
						{
							Validator.XmlValidationError xmlValidationError = validator.ValidationErrors[i];
							this.Context.Response.AppendHeader("X-MS-ASError", xmlValidationError.ToString(), true);
						}
					}
				}
				flag2 = flag;
			}
			return flag2;
		}

		internal virtual XmlDocument GetValidationErrorXml()
		{
			throw new NotSupportedException();
		}

		internal XmlDocument GetCommandXmlStub()
		{
			string xml = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><{0} xmlns=\"{1}\"></{0}>", this.RootNodeName, this.RootNodeNamespace);
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(xml);
			return xmlDocument;
		}

		protected virtual bool PreProcessRequest()
		{
			bool flag = true;
			if (this.Context.Request.IsEmpty && this.Context.Request.CommandType != CommandType.Options)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.EmptyRequest, 1);
			}
			return flag;
		}

		protected virtual void LogRequestToMailboxLog(string requestToLog)
		{
			this.mailboxLogger.SetData(MailboxLogDataName.RequestBody, this.GetRequestDataToLog(requestToLog));
		}

		internal void WorkerThread()
		{
			if (this.perUserTracingEnabled)
			{
				AirSyncDiagnostics.SetThreadTracing();
			}
			else
			{
				AirSyncDiagnostics.ClearThreadTracing();
			}
			this.executionState = Command.ExecutionState.Invalid;
			AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Entry point of the worker thread", 30763);
			Guid serviceProviderRequestId = Microsoft.Exchange.Diagnostics.Trace.TraceCasStart(CasTraceEventType.ActiveSync);
			AirSyncSyncStateTypeFactory.EnsureSyncStateTypesRegistered();
			string action = null;
			try
			{
				this.SetContextDataInTls();
				ActivityContext.SetThreadScope(this.User.Context.ActivityScope);
				action = this.User.Context.ActivityScope.Action;
				this.User.Context.ActivityScope.Action = this.RootNodeName;
				this.User.Context.ActivityScope.Component = "ActiveSync";
				if (this.perfCounter != null)
				{
					this.perfCounter.Increment();
				}
				AirSyncDiagnostics.TracePfd<int, int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Checking AirSync version: {1}", 16427, this.Version);
				this.ProtocolLogger.SetValue(ProtocolLoggerData.ProtocolVersion, this.Version);
				AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Opening mailbox session", 24619);
				bool flag;
				this.GetOrCreateNotificationManager(out flag);
				if (flag)
				{
					this.executionState = Command.ExecutionState.Pending;
				}
				else
				{
					if (this.ShouldOpenSyncState)
					{
						this.OpenMailboxSession(this.User);
						AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Opening Sync Storage", 20523);
						AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.RequestsTracer, this, "DeviceIdentity: {0}.", this.DeviceIdentity);
						this.OpenSyncStorage(this.ShouldOpenGlobalSyncState);
						this.SetNotificationManagerMailboxLogging(this.MailboxLoggingEnabled);
					}
					bool flag2 = this.PreProcessRequest();
					if (flag2)
					{
						string text = (this.XmlRequest == null) ? Command.ReadStringFromStream(this.InputStream, this.context.Request.ContentEncoding) : AirSyncUtility.BuildOuterXml(this.XmlRequest.OwnerDocument, true);
						AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Request: {0}", text);
						if (this.MailboxLoggingEnabled && this.mailboxLogger != null && this.mailboxLogger.Enabled)
						{
							this.mailboxLogger.SetData(MailboxLogDataName.RequestTime, ExDateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo));
							this.mailboxLogger.SetData(MailboxLogDataName.ServerName, Command.MachineName);
							this.mailboxLogger.SetData(MailboxLogDataName.AssemblyVersion, "15.00.1497.015");
							this.requestId = this.GetNextNumber(0, true);
							this.mailboxLogger.SetData(MailboxLogDataName.Identifier, this.requestId.ToString("X", CultureInfo.InvariantCulture.NumberFormat));
							this.mailboxLogger.LogRequestHeader(this.Request);
							this.LogRequestToMailboxLog(text);
						}
					}
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.TimeDeviceAccessCheckStarted, ExDateTime.UtcNow);
					if (!this.IsDeviceAccessAllowed())
					{
						this.executionState = Command.ExecutionState.Complete;
					}
					else
					{
						AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Checking policy", 28715);
						AirSyncDiagnostics.FaultInjectionTracer.TraceTest(3179687229U);
						this.context.ProtocolLogger.SetValue(ProtocolLoggerData.TimePolicyCheckStarted, ExDateTime.UtcNow);
						bool flag3;
						if (!this.IsPolicyCompliant(out this.nextPolicyRefreshTime, out flag3))
						{
							AirSyncDiagnostics.TraceInfo<string, DeviceIdentity>(ExTraceGlobals.RequestsTracer, this, "user {0}, device identity {1}, is blocked by policy.", this.User.Name, this.DeviceIdentity);
							this.executionState = Command.ExecutionState.Complete;
							if (!flag3 && this.GlobalInfo != null)
							{
								this.GlobalInfo.DeviceAccessState = DeviceAccessState.Blocked;
								this.GlobalInfo.DeviceAccessStateReason = DeviceAccessStateReason.Policy;
							}
						}
						else if (flag2)
						{
							AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Calling Execute method for the command", 20011);
							if (this.RootNodeName == "Invalid" || this.ValidateXml())
							{
								if (this.IsInteractiveCommand)
								{
									this.AddInteractiveCall();
								}
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.TimeExecuteStarted, ExDateTime.UtcNow);
								this.executionState = this.ExecuteCommand();
								this.context.ProtocolLogger.SetValue(ProtocolLoggerData.TimeExecuteFinished, ExDateTime.UtcNow);
							}
							else
							{
								this.executionState = Command.ExecutionState.Complete;
							}
						}
						else
						{
							this.XmlResponse = this.GetInvalidParametersXml();
							if (this.XmlResponse != null)
							{
								this.context.Response.HttpStatusCode = HttpStatusCode.OK;
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[Command.WorkerThread] Returning InvalidParameters - we need them to reissue the request because it is invalid.");
							}
							else
							{
								AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[Command.WorkerThread] GetInvalidParametersXml was not overridden and returned null.  Convering into HTTP 400 response.  Command Type: {0}", base.GetType().Name);
								this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidStoredRequest");
								this.Context.Response.SetErrorResponse(HttpStatusCode.BadRequest, StatusCode.InvalidStoredRequest);
							}
							this.executionState = Command.ExecutionState.Complete;
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (this.mailboxLogger != null)
				{
					this.mailboxLogger.SetData(MailboxLogDataName.Command_WorkerThread_Exception, new AirSyncUtility.ExceptionToStringHelper(ex));
				}
				this.executionState = Command.ExecutionState.Complete;
				AirSyncUtility.ProcessException(ex, this, this.context);
			}
			finally
			{
				try
				{
					AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - Releasing the resources and Logging protocol data to the IIS logs", 28203);
					this.TraceStop(serviceProviderRequestId);
					switch (this.executionState)
					{
					case Command.ExecutionState.Pending:
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Command is pending.");
						ActiveSyncRequestData activeSyncRequestData = ActiveSyncRequestCache.Instance.Get(this.Context.ActivityScope.ActivityId);
						activeSyncRequestData.IsHanging = true;
						this.LogResponseToMailbox(true);
						if (this.GlobalInfo != null)
						{
							this.CompleteDeviceAccessProcessing();
							this.GlobalInfo.SaveToMailbox();
						}
						this.CommitSyncStatusSyncState();
						this.ReleaseResources();
						break;
					}
					case Command.ExecutionState.Complete:
						try
						{
							AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - The request has completed successfully", 24107);
							if (this.sendServerUpgradeHeader)
							{
								if (this.User.IsConsumerOrganizationUser)
								{
									this.AddHeadersForConsumerOrgUser();
								}
								else
								{
									this.AddHeadersForEnterpriseOrgUser();
								}
							}
							if (this.GlobalInfo != null)
							{
								this.GlobalInfo.DeviceActiveSyncVersion = this.Request.VersionString;
								this.CompleteDeviceAccessProcessing();
								if (!this.GlobalInfo.HaveSentBoostrapMailForWM61 && this.GlobalInfo.BootstrapMailForWM61TriggeredTime != null && this.GlobalInfo.BootstrapMailForWM61TriggeredTime + Command.bootstrapMailDeliveryDelay < ExDateTime.UtcNow)
								{
									OTABootstrapMail.SendBootstrapMailForWM61(this.User);
									this.GlobalInfo.HaveSentBoostrapMailForWM61 = true;
								}
								if (this.MailboxLoggingEnabled && this.MailboxLogger != null && this.MailboxLogger.Enabled)
								{
									this.MailboxLogger.SetData(MailboxLogDataName.AccessState, this.GlobalInfo.DeviceAccessState);
									this.MailboxLogger.SetData(MailboxLogDataName.AccessStateReason, this.GlobalInfo.DeviceAccessStateReason);
									if (this.GlobalInfo.DeviceAccessControlRule != null)
									{
										this.MailboxLogger.SetData(MailboxLogDataName.DeviceAccessControlRule, this.GlobalInfo.DeviceAccessControlRule);
									}
								}
								this.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoOS, this.GlobalInfo.DeviceOS, 50);
								string value;
								if (this.GlobalInfo.DeviceAccessStateReason < DeviceAccessStateReason.UserAgentsChanges || this.GlobalInfo.DeviceAccessStateReason > DeviceAccessStateReason.CommandFrequency)
								{
									value = this.GlobalInfo.DeviceAccessState.ToString() + this.GlobalInfo.DeviceAccessStateReason.ToString()[0];
								}
								else
								{
									value = this.GlobalInfo.DeviceAccessState.ToString() + "AB" + this.GlobalInfo.DeviceAccessStateReason.ToString()[0];
								}
								this.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.AccessStateAndReason, value);
								this.GlobalInfo.SaveToMailbox();
							}
							if (this.MeetingOrganizerSyncState != null && this.MeetingOrganizerSyncState.MeetingOrganizerInfo.IsDirty)
							{
								this.MeetingOrganizerSyncState.IsDirty = true;
								this.MeetingOrganizerSyncState.SaveToMailbox();
							}
						}
						catch (Exception ex2)
						{
							if (this.mailboxLogger != null)
							{
								this.mailboxLogger.SetData(MailboxLogDataName.Command_WorkerThread_Exception, new AirSyncUtility.ExceptionToStringHelper(ex2));
							}
							AirSyncUtility.ProcessException(ex2, this, this.context);
						}
						finally
						{
							if (this.GlobalInfo != null)
							{
								this.GlobalInfo.Dispose();
								this.GlobalInfo = null;
							}
							if (this.MeetingOrganizerSyncState != null)
							{
								this.MeetingOrganizerSyncState.Dispose();
								this.MeetingOrganizerSyncState = null;
							}
						}
						if (this.context.Response.HttpStatusCode == HttpStatusCode.OK)
						{
							if (this.XmlResponse != null)
							{
								this.Context.Response.IssueWbXmlResponse();
							}
							else if (string.IsNullOrEmpty(this.Context.Response.ContentType) || string.Equals("text/html", this.Context.Response.ContentType, StringComparison.OrdinalIgnoreCase))
							{
								this.Context.Response.ContentType = "application/vnd.ms-sync.wbxml";
							}
						}
						this.CommitSyncStatusSyncState();
						AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Response: {0}", (this.XmlResponse == null) ? "[No XmlResponse]" : AirSyncUtility.BuildOuterXml(this.XmlResponse, true));
						this.LogResponseToMailbox(false);
						this.ReleaseResources();
						break;
					default:
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Invalid execution state.");
						throw new InvalidOperationException();
					}
					if (this.perUserTracingEnabled)
					{
						AirSyncDiagnostics.ClearThreadTracing();
					}
				}
				catch (Exception ex3)
				{
					if (!AirSyncUtility.HandleNonCriticalException(ex3, true))
					{
						throw;
					}
				}
				finally
				{
					try
					{
						if (this is IAsyncCommand)
						{
							this.ProcessQueuedEvents();
						}
					}
					finally
					{
						Command.ClearContextDataInTls();
						ActivityContext.ClearThreadScope();
						if (this.User != null && this.User.Context != null && this.User.Context.ActivityScope != null)
						{
							this.User.Context.ActivityScope.Action = action;
						}
					}
				}
			}
		}

		public void ExecuteWithCommandTls(Action action)
		{
			this.SetContextDataInTls();
			try
			{
				action();
			}
			finally
			{
				Command.ClearContextDataInTls();
			}
		}

		internal void UpdateADDevice(GlobalInfo globalInfo)
		{
			if (globalInfo == null)
			{
				return;
			}
			if (this.User.IsConsumerOrganizationUser)
			{
				return;
			}
			if ((globalInfo.DeviceAccessStateReason >= DeviceAccessStateReason.UserAgentsChanges && globalInfo.DeviceAccessStateReason < DeviceAccessStateReason.ExternallyManaged && (!GlobalSettings.AutoBlockWriteToAd || ADNotificationManager.GetAutoBlockThreshold(this.GlobalInfo.DeviceAccessStateReason).DeviceBlockDuration == TimeSpan.Zero)) || (this.Context.DeviceBehavior != null && this.Context.DeviceBehavior.TimeToUpdateAD > ExDateTime.UtcNow))
			{
				return;
			}
			int num = GlobalInfo.ComputeADDeviceInfoHash(globalInfo);
			int? addeviceInfoHash = this.GlobalInfo.ADDeviceInfoHash;
			bool flag = this.GlobalInfo.ADCreationTime != null && this.GlobalInfo.ADCreationTime != null && this.GlobalInfo.ADCreationTime.Value.AddHours((double)GlobalSettings.ADDataSyncInterval) < ExDateTime.UtcNow;
			if (addeviceInfoHash != null && addeviceInfoHash.Value == num && ADObjectId.Equals(globalInfo.UserADObjectId, this.User.ADUser.OriginalId) && globalInfo.DeviceADObjectId != null && !flag)
			{
				AirSyncDiagnostics.TraceInfo<string, DeviceIdentity, bool>(ExTraceGlobals.RequestsTracer, this, "Skip updating AD device object for user {0}, device id {1}, ShouldForceUpdateAD: {2}", this.User.Name, this.DeviceIdentity, flag);
				return;
			}
			this.DeviceManager.GetActiveSyncDeviceContainer();
			MobileDevice mobileDevice = this.DeviceManager.GetMobileDevice();
			int num2 = num;
			if (flag)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Compare the Hash value from AD and Update is required.");
				num2 = GlobalInfo.ComputeADDeviceInfoHash(mobileDevice);
				this.GlobalInfo.ADCreationTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			bool flag2 = false;
			if (mobileDevice != null && mobileDevice.MaximumSupportedExchangeObjectVersion.IsOlderThan(mobileDevice.ExchangeVersion))
			{
				flag2 = this.deviceManager.DeleteMobileDevice(mobileDevice);
				mobileDevice = null;
				AirSyncDiagnostics.TraceInfo<string, DeviceIdentity, bool>(ExTraceGlobals.RequestsTracer, this, "mobile device is readonly. UserName {0}, device identity {1}, ShouldCreateNewDevice {2} .", this.User.Name, this.DeviceIdentity, flag2);
			}
			if (mobileDevice == null)
			{
				AirSyncDiagnostics.TraceInfo<string, DeviceIdentity>(ExTraceGlobals.RequestsTracer, this, "No ActiveSyncDevice object found in AD for user {0}, device identity: {1}", this.User.Name, this.DeviceIdentity);
				if (!this.isNewSyncStateStorage && this.syncStateStorage.CreationTime.AddHours((double)GlobalSettings.ADDataSyncInterval) >= ExDateTime.UtcNow && this.GlobalInfo.ADCreationTime != null && this.GlobalInfo.ADCreationTime.Value.AddHours((double)GlobalSettings.ADDataSyncInterval) >= ExDateTime.UtcNow && ADObjectId.Equals(globalInfo.UserADObjectId, this.User.ADUser.OriginalId) && !flag2)
				{
					return;
				}
				AirSyncDiagnostics.TraceInfo<string, DeviceIdentity>(ExTraceGlobals.RequestsTracer, this, "Creating new ActiveSyncDevice object in AD for user {0}, device identity {1}.", this.User.Name, this.DeviceIdentity);
				ActiveSyncRequestData activeSyncRequestData = ActiveSyncRequestCache.Instance.Get(this.Context.ActivityScope.ActivityId);
				activeSyncRequestData.NewDeviceCreated = true;
				this.DeviceManager.CreateMobileDevice(globalInfo, this.syncStateStorage.CreationTime, true, null);
				this.GlobalInfo.ADCreationTime = new ExDateTime?(ExDateTime.UtcNow);
				mobileDevice = this.DeviceManager.GetMobileDevice();
				if (mobileDevice == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Creating new ActiveSyncDevice object in AD failed.");
					return;
				}
			}
			else if (addeviceInfoHash == null || addeviceInfoHash.Value != num || addeviceInfoHash.Value != num2)
			{
				AirSyncDiagnostics.TraceInfo<string, DeviceIdentity>(ExTraceGlobals.RequestsTracer, this, "Updating ActiveSyncDevice object in AD for user {0}, device identity {1}.", this.User.Name, this.DeviceIdentity);
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.ADWriteReason, string.Format("FU:{0},SSH:{1},ADH:{2},DHA:{3},SCD:{4}", new object[]
				{
					flag,
					(addeviceInfoHash == null) ? -1 : addeviceInfoHash.Value,
					num,
					num2,
					flag2
				}));
				this.DeviceManager.UpdateMobileDevice(mobileDevice, globalInfo);
			}
			globalInfo.ADDeviceInfoHash = new int?(num);
			globalInfo.DeviceADObjectId = mobileDevice.OriginalId;
			globalInfo.UserADObjectId = this.User.ADUser.OriginalId;
		}

		internal void InitializeSyncStatusSyncState()
		{
			if (this.syncStatusSyncData == null)
			{
				if (this.User.Features.IsEnabled(EasFeature.SyncStatusOnGlobalInfo))
				{
					this.syncStatusSyncData = NewSyncStatusData.Load(this.GlobalInfo, this.syncStateStorage);
				}
				else
				{
					this.syncStatusSyncData = SyncStatusData.Load(this.syncStateStorage);
				}
				Interlocked.Exchange(ref this.validToCommitSyncStatusSyncState, 1);
			}
		}

		internal bool IsDeviceAccessAllowed()
		{
			if (this.GlobalInfo != null)
			{
				if (this.GlobalInfo.RemoteWipeRequestedTime != null)
				{
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceWipeIsRequested");
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Device wipe is requested.");
					return true;
				}
				if (string.IsNullOrEmpty(this.GlobalInfo.DeviceModel))
				{
					this.GlobalInfo.DeviceModel = this.DeviceIdentity.DeviceType;
				}
				else if (this.GlobalInfo.IsSyncStateJustUpgraded)
				{
					this.GlobalInfo.DeviceInformationReceived = true;
				}
			}
			IOrganizationSettingsData organizationSettingsData = ADNotificationManager.GetOrganizationSettingsData(this.User);
			DeviceAccessStateReason deviceAccessStateReason;
			if (this.Context.User.Features.IsEnabled(EasFeature.CloudMDMEnrolled) || organizationSettingsData.IsIntuneManaged)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.ExternallyManaged, "T");
				bool isSupportedDevice = true;
				deviceAccessStateReason = DeviceAccessStateReason.ExternallyManaged;
				if (this.Request.CommandType == CommandType.Options)
				{
					deviceAccessStateReason = DeviceAccessStateReason.Global;
					this.CurrentAccessState = DeviceAccessState.Allowed;
				}
				else if (!this.GlobalInfo.DeviceInformationReceived && !this.GlobalInfo.DeviceInformationPromoted)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "We haven't received device information settings yet.");
					if (this.syncStateStorage.CreationTime.AddMinutes((double)GlobalSettings.DeviceDiscoveryPeriod) >= ExDateTime.UtcNow)
					{
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "DeviceDiscovery.");
						this.CurrentAccessState = DeviceAccessState.DeviceDiscovery;
					}
					else
					{
						this.context.ProtocolLogger.AppendValue(ProtocolLoggerData.Message, "FallBackToUnSupportedPlatform");
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "device information not received within configured Discovery period. fall back to unsupported platform.");
						isSupportedDevice = false;
					}
				}
				else if (!DeviceCapability.DeviceSupportedForMdm(this.GlobalInfo))
				{
					this.context.ProtocolLogger.AppendValue(ProtocolLoggerData.Message, "DeviceNotSupported");
					isSupportedDevice = false;
				}
				if (this.CurrentAccessState == DeviceAccessState.Unknown)
				{
					this.CurrentAccessState = this.IsCloudMDMPolicyCompliant(isSupportedDevice, organizationSettingsData, out deviceAccessStateReason);
					AirSyncDiagnostics.TraceInfo<string, DeviceIdentity>(ExTraceGlobals.RequestsTracer, this, "user {0}, device identity: {1} is blocked by cloudMDM policy.", this.User.Name, this.DeviceIdentity);
					this.context.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, (!this.isManaged) ? "BlockedMDMManaged" : "BlockedMDMCompliant");
				}
			}
			else
			{
				this.CurrentAccessState = this.DetermineDeviceAccessState(out deviceAccessStateReason);
			}
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "DetermineDeviceAccessState for user '{0}', device identifier: '{1}', AccessState '{2}', AccessReason '{3}'", new object[]
			{
				this.User.Name,
				(this.Request.CommandType == CommandType.Options) ? null : this.DeviceIdentity.ToString(),
				this.CurrentAccessState,
				deviceAccessStateReason
			});
			bool flag = true;
			if (this.CurrentAccessState == DeviceAccessState.Blocked)
			{
				flag = false;
				if (deviceAccessStateReason < DeviceAccessStateReason.UserAgentsChanges || deviceAccessStateReason > DeviceAccessStateReason.CommandFrequency)
				{
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceIsBlockedForThisUser");
					this.context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.DeviceIsBlockedForThisUser);
				}
				else
				{
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceIsAutoBlocked");
					this.context.Response.SetErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.None);
					IAsyncCommand asyncCommand = this as IAsyncCommand;
					uint heartbeatInterval;
					if (asyncCommand != null && (ulong)(heartbeatInterval = asyncCommand.GetHeartbeatInterval()) > (ulong)((long)GlobalSettings.ErrorResponseDelay))
					{
						this.context.Response.TimeToRespond = this.context.RequestTime.AddSeconds(heartbeatInterval);
					}
				}
				this.context.Response.AppendHeader("X-MS-ASThrottle", deviceAccessStateReason.ToString());
			}
			if (this.ShouldOpenSyncState)
			{
				if (this.CurrentAccessState == DeviceAccessState.Quarantined || this.CurrentAccessState == DeviceAccessState.DeviceDiscovery)
				{
					flag = this.HandleQuarantinedState();
				}
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Stamping access state and reason on GlobalInfo for user: {0}, device identity: {1}, AccessState: {2}, AccessReason: {3}", new object[]
				{
					this.User.Name,
					this.DeviceIdentity,
					this.CurrentAccessState,
					deviceAccessStateReason
				});
				this.PreviousAccessState = this.GlobalInfo.DeviceAccessState;
				this.GlobalInfo.DeviceAccessState = this.CurrentAccessState;
				this.GlobalInfo.DeviceAccessStateReason = deviceAccessStateReason;
				this.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.DeviceAccessState, this.GlobalInfo.DeviceAccessState);
				this.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.DeviceAccessStateReason, this.GlobalInfo.DeviceAccessStateReason);
			}
			return flag;
		}

		internal bool IsPolicyCompliant(out ExDateTime nextRefreshTime, out bool isPolicyRefresh)
		{
			AirSyncDiagnostics.TracePfd<int, IAirSyncContext, Command>(ExTraceGlobals.PfdInitTraceTracer, this, "PFD EAI {0} - IsPolicyCompliant - Context: {1}, Command: {2}", 32299, this.context, this);
			nextRefreshTime = ExDateTime.MaxValue;
			isPolicyRefresh = false;
			if (this.Context.User.Features.IsEnabled(EasFeature.CloudMDMEnrolled) || ADNotificationManager.GetOrganizationSettingsData(this.User).IsIntuneManaged)
			{
				AirSyncDiagnostics.TracePfd(ExTraceGlobals.RequestsTracer, this, "Skip Policy Check for Cloud MDM user.");
				return true;
			}
			if (this.User.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.MonitoringMailbox)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Bypassing provisioning for user {0}", this.User.Name);
				return true;
			}
			int version = this.Version;
			AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - the command requires policy check", 26155);
			if (!this.RequiresPolicyCheck && (version < 140 || !(this is PingCommand)))
			{
				return true;
			}
			uint? policyKey = this.Request.PolicyKey;
			if (policyKey != null)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.PolicyKeyReceived, policyKey.Value);
			}
			bool flag = false;
			TimeSpan timeSpan = TimeSpan.MaxValue;
			bool flag2 = false;
			PolicyData policyData = ADNotificationManager.GetPolicyData(this.User);
			if (policyData != null)
			{
				flag = policyData.AllowNonProvisionableDevices;
				flag2 = policyData.RequireStorageCardEncryption;
				if (!policyData.DevicePolicyRefreshInterval.IsUnlimited)
				{
					timeSpan = policyData.DevicePolicyRefreshInterval.Value;
				}
			}
			AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - the device is allowed to sync", 17963);
			AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - Sync State Storage is not null", 22059);
			if (this.SyncStateStorage == null)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_DeviceIdAndDeviceTypeMustBePresent, "DeviceIdAndDeviceTypeMustBePresent" + this.User.Name, new string[]
				{
					this.User.Name
				});
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoSyncStateWhileCheckingPolicy");
				this.context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.SyncStateNotFound);
				return false;
			}
			AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Read the provision information from the user's mailbox", 30251);
			GlobalInfo globalInfo = this.GlobalInfo;
			this.SetStateData(new Command.StateData
			{
				DevicePhoneNumberForSms = globalInfo.DevicePhoneNumberForSms,
				DeviceEnableOutboundSMS = globalInfo.DeviceEnableOutboundSMS
			});
			if (!this.RequiresPolicyCheck)
			{
				return true;
			}
			AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Detect if the user's policy has changed", 18987);
			PolicyData policyData2 = ADNotificationManager.GetPolicyData(this.User);
			bool flag3;
			Command.DetectPolicyChange(policyData2, globalInfo, this.Version, out flag3);
			if (!flag3)
			{
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceNotFullyProvisionable");
				this.context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.DeviceNotFullyProvisionable);
				return false;
			}
			AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - if remote wipe was requested", 27179);
			if (globalInfo.RemoteWipeRequestedTime != null)
			{
				if (policyKey == null)
				{
					this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "RemoteWipeRequested");
					this.context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.RemoteWipeRequested);
					return false;
				}
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "RemoteWipeRequested");
				this.context.Response.SetErrorResponse((HttpStatusCode)449, StatusCode.RemoteWipeRequested);
				return false;
			}
			else
			{
				AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - PolicyKeyNeeded and policyKeyHeader are set", 23083);
				if (globalInfo.PolicyKeyNeeded == 0U && (policyKey == null || policyKey == 0U))
				{
					return true;
				}
				AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - policyKeyHeader is set", 31275);
				if (policyKey == null)
				{
					if (flag)
					{
						return true;
					}
					AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_NonconformingDeviceError, "NonconformingDeviceError: " + this.User.Name, new string[]
					{
						this.User.Name,
						(this.DeviceIdentity == null) ? null : this.DeviceIdentity.DeviceId
					});
					this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LegacyDeviceOnStrictPolicy");
					this.context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.LegacyDeviceOnStrictPolicy);
					return false;
				}
				else
				{
					if (flag2 && version < 120 && !flag)
					{
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LegacyDeviceCannotEncrypt");
						this.context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.DeviceNotFullyProvisionable);
						return false;
					}
					AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - LastPolicyTime is set", 16939);
					if (globalInfo.LastPolicyTime == null)
					{
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceNotProvisioned");
						this.context.Response.SetErrorResponse((HttpStatusCode)449, StatusCode.DeviceNotProvisioned);
						return false;
					}
					AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - timeSinceLastProvision is greater than refreshInterval", 25131);
					if (ExDateTime.UtcNow - globalInfo.LastPolicyTime > timeSpan)
					{
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "PolicyRefresh");
						this.context.Response.SetErrorResponse((HttpStatusCode)449, StatusCode.PolicyRefresh);
						isPolicyRefresh = true;
						return false;
					}
					if (timeSpan != TimeSpan.MaxValue && ExDateTime.MaxValue - timeSpan > globalInfo.LastPolicyTime.Value)
					{
						nextRefreshTime = globalInfo.LastPolicyTime.Value + timeSpan;
					}
					AirSyncDiagnostics.TracePfd<int>(ExTraceGlobals.PfdInitTraceTracer, null, "PFD EAI {0} - Checking - if there is no policy mismatch", 21035);
					if (policyKey != globalInfo.PolicyKeyNeeded)
					{
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidPolicyKey");
						this.context.Response.SetErrorResponse((HttpStatusCode)449, StatusCode.InvalidPolicyKey);
						return false;
					}
					return true;
				}
			}
		}

		internal DeviceAccessState IsCloudMDMPolicyCompliant(bool isSupportedDevice, IOrganizationSettingsData organizationSettingsData, out DeviceAccessStateReason accessStateReason)
		{
			accessStateReason = DeviceAccessStateReason.ExternallyManaged;
			if (this.User.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.MonitoringMailbox)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Bypassing provisioning for user {0}", this.User.Name);
				return DeviceAccessState.Allowed;
			}
			if (this.Request.CommandType == CommandType.Options)
			{
				return DeviceAccessState.Allowed;
			}
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.PolicyEvaluationEnabled && !isSupportedDevice)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "user {0}, device id {1}, device type {2}, is {3} by cloudMDM policy of Unsupported platforms.", new object[]
				{
					this.User.Name,
					this.Request.DeviceIdentity.DeviceId,
					this.Request.DeviceIdentity.DeviceType,
					organizationSettingsData.AllowAccessForUnSupportedPlatform ? "Allowed" : "Blocked"
				});
				if (!organizationSettingsData.AllowAccessForUnSupportedPlatform)
				{
					return DeviceAccessState.Blocked;
				}
				return DeviceAccessState.Allowed;
			}
			else
			{
				bool deviceStatus;
				DeviceAccessState deviceAccessState;
				if (GlobalSettings.SkipAzureADCall)
				{
					deviceStatus = GraphApiHelper.GetDeviceStatus(this.User.OrganizationId, this.DeviceManager.GetMobileDevice(), this.User.ADUser.ExternalDirectoryObjectId, out this.isManaged, out this.isCompliant);
					deviceAccessState = ((this.isManaged && this.isCompliant) ? DeviceAccessState.Allowed : DeviceAccessState.Quarantined);
					if (deviceAccessState == DeviceAccessState.Quarantined)
					{
						accessStateReason = (this.isManaged ? DeviceAccessStateReason.ExternalCompliance : DeviceAccessStateReason.ExternalEnrollment);
					}
				}
				else if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.PolicyEvaluationEnabled)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Call GraphApi Helper GetDeviceStatus for device {0}", this.Request.DeviceIdentity.DeviceId);
					deviceStatus = GraphApiHelper.GetDeviceStatus(this.User.OrganizationId, this.Request.DeviceIdentity.DeviceId, this.User.ADUser.ExternalDirectoryObjectId, isSupportedDevice, out deviceAccessState, out accessStateReason);
				}
				else
				{
					deviceStatus = GraphApiHelper.GetDeviceStatus(this.User.OrganizationId, this.Request.DeviceIdentity.DeviceId, this.User.ADUser.ExternalDirectoryObjectId, out this.isManaged, out this.isCompliant);
					deviceAccessState = ((this.isManaged && this.isCompliant) ? DeviceAccessState.Allowed : DeviceAccessState.Quarantined);
					if (deviceAccessState == DeviceAccessState.Quarantined)
					{
						accessStateReason = (this.isManaged ? DeviceAccessStateReason.ExternalCompliance : DeviceAccessStateReason.ExternalEnrollment);
					}
				}
				if (!deviceStatus)
				{
					this.ProtocolLogger.SetValue(ProtocolLoggerData.Message, "DeviceDisabledInAAD");
					throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.DeviceIsBlockedForThisUser, null, false);
				}
				return deviceAccessState;
			}
		}

		internal void BorrowSecurityContextAndSession(Command sourceCommand)
		{
			if (this.securityContextAndSession != null && !this.sessionBorrowed)
			{
				throw new InvalidOperationException(string.Format("[Command.BorrowSecurityContextAndSession] Command {0} already has a security context and session that is not borrowed. Cannot replace.", base.GetType().Name));
			}
			this.securityContextAndSession = sourceCommand.securityContextAndSession;
			this.syncStateStorage = sourceCommand.syncStateStorage;
			this.sessionBorrowed = true;
		}

		internal bool GetOrCreateConversation(ConversationId conversationId, bool shouldLoadBodySummary, out Conversation conversation)
		{
			if (conversationId == null)
			{
				throw new ArgumentNullException("conversationId");
			}
			bool flag = false;
			if (this.cachedConversation == null || !this.cachedConversation.ConversationId.Equals(conversationId))
			{
				AirSyncDiagnostics.TraceDebug<ConversationId>(ExTraceGlobals.RequestsTracer, this, "Loading Conversation for id {0}.", conversationId);
				bool isIrmEnabled = false;
				if (this.Context.User.IrmEnabled && this.Context.Request.IsSecureConnection && this.RightsManagementSupportFlag)
				{
					isIrmEnabled = true;
				}
				this.cachedConversation = Conversation.Load(this.MailboxSession, conversationId, isIrmEnabled, new PropertyDefinition[0]);
				if (this.shouldWatsonWhenReloadingSameConversation)
				{
					if (this.openedConversationIdList.Contains(conversationId))
					{
						AirSyncDiagnostics.TraceError<ConversationId>(ExTraceGlobals.RequestsTracer, this, "Loading same conversation twice.", conversationId);
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ReloadingSameConversationTwice");
					}
					else
					{
						this.openedConversationIdList.Add(conversationId);
					}
				}
				bool flag2 = false;
				foreach (IConversationTreeNode conversationTreeNode in this.cachedConversation.ConversationTree)
				{
					flag2 = (bool)conversationTreeNode.StorePropertyBags[0].TryGetProperty(MessageItemSchema.MapiHasAttachment);
					if (flag2)
					{
						break;
					}
				}
				if (flag2)
				{
					this.cachedConversation.OnBeforeItemLoad += BodyConversionUtilities.OnBeforeItemLoadInConversationForceOpen;
				}
				else
				{
					this.cachedConversation.OnBeforeItemLoad += BodyConversionUtilities.OnBeforeItemLoadInConversation;
				}
				if (shouldLoadBodySummary)
				{
					this.cachedConversation.LoadBodySummaries();
				}
				flag = true;
			}
			conversation = this.cachedConversation;
			return flag;
		}

		internal void EnableConversationDoubleLoadCheck(bool shouldEnableCheck)
		{
			this.shouldWatsonWhenReloadingSameConversation = shouldEnableCheck;
			if (this.shouldWatsonWhenReloadingSameConversation)
			{
				this.openedConversationIdList.Clear();
			}
		}

		protected virtual void GetOrCreateNotificationManager(out bool notificationManagerWasTaken)
		{
			notificationManagerWasTaken = false;
		}

		protected virtual void SetNotificationManagerMailboxLogging(bool mailboxLogging)
		{
		}

		protected virtual void ProcessQueuedEvents()
		{
			throw new NotSupportedException("ProcessQueuedEvents");
		}

		protected abstract bool HandleQuarantinedState();

		protected internal void CommitSyncStatusSyncState()
		{
			if (this.syncStatusSyncData != null)
			{
				try
				{
					if (this.ShouldSaveSyncStatus)
					{
						if (Interlocked.CompareExchange(ref this.validToCommitSyncStatusSyncState, 0, 1) == 1)
						{
							this.syncStatusSyncData.SaveAndDispose();
							this.syncStatusSyncData = null;
						}
						else
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "syncStatusSyncState has already been committed.");
						}
					}
				}
				catch (LocalizedException arg)
				{
					AirSyncDiagnostics.TraceError<LocalizedException>(ExTraceGlobals.RequestsTracer, this, "Failed to commit syncStatusSyncState: {0}", arg);
				}
			}
		}

		protected virtual void CompleteHttpRequest()
		{
			ExDateTime timeToRespond = this.context.Response.TimeToRespond;
			if (!this.context.Response.IsErrorResponse && this.PartialFailure)
			{
				timeToRespond.AddSeconds((double)GlobalSettings.ErrorResponseDelay);
			}
			if (!(this is OptionsCommand) && (this.XmlResponse == null || this.XmlResponse.FirstChild == null))
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.EmptyResponse, 1);
			}
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, (this.Context as AirSyncContext).GetParticipantCacheData());
			TimeSpan timeSpan = timeToRespond - ExDateTime.UtcNow;
			if (timeSpan <= Command.timeAllowedToCompleteEarly || (this.User != null && this.User.IsMonitoringTestUser))
			{
				this.CompleteHttpRequestCallback(null);
				return;
			}
			this.ProtocolLogger.SetValue(ProtocolLoggerData.CompletionOffset, (int)timeSpan.TotalMilliseconds);
			this.completionTimer = new Timer(new TimerCallback(this.CompleteHttpRequestCallback), this, timeSpan, Command.disablePeriodsTimespan);
		}

		protected internal void OpenMailboxSession(IAirSyncUser user)
		{
			this.CheckDisposed();
			if (this.securityContextAndSession != null)
			{
				return;
			}
			lock (this.contextAndSessionLock)
			{
				this.CheckDisposed();
				if (this.securityContextAndSession != null)
				{
					return;
				}
				SecurityContextAndSession securityContextAndSession = null;
				this.User.Context.ActivityScope.ClientInfo = string.Format("Client=ActiveSync;UserAgent={0};Action={1}", this.EffectiveUserAgent, this.context.Request.PathAndQuery);
				if (MailboxSessionCache.TryGetAndRemoveValue(user.ExchangePrincipal.ObjectId.ObjectGuid, out securityContextAndSession))
				{
					this.securityContextAndSession = securityContextAndSession;
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "Reuse mailbox session for " + user.Name);
					this.securityContextAndSession.MailboxSession.Connect();
				}
				else
				{
					string clientInfoString = string.Format("Client=ActiveSync;User={0}", user.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
					MailboxSession mailboxSession = null;
					bool flag2 = false;
					try
					{
						if (this.context.Request.WasProxied)
						{
							if (user.ADUser == null)
							{
								this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ProxyUserNotFound");
								AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_NoMailboxRights, new string[]
								{
									user.Name
								});
								throw new AirSyncPermanentException(HttpStatusCode.ServiceUnavailable, StatusCode.ServerErrorRetryLater, null, false);
							}
							mailboxSession = MailboxSession.OpenWithBestAccess(user.ExchangePrincipal, user.ADUser.LegacyExchangeDN, user.ClientSecurityContextWrapper.ClientSecurityContext, this.context.Request.Culture, clientInfoString);
							if (!mailboxSession.CanActAsOwner)
							{
								this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoMailboxRights");
								AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_NoMailboxRights, new string[]
								{
									user.Name
								});
								throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, null, false);
							}
						}
						else
						{
							mailboxSession = MailboxSession.Open(user.ExchangePrincipal, user.ClientSecurityContextWrapper.ClientSecurityContext, this.context.Request.Culture, clientInfoString);
						}
						this.securityContextAndSession = new SecurityContextAndSession(user.ClientSecurityContextWrapper, mailboxSession);
						this.ProtocolLogger.SetValue(ProtocolLoggerData.NewMailboxSession, 1);
						flag2 = true;
					}
					finally
					{
						if (!flag2 && mailboxSession != null)
						{
							mailboxSession.Dispose();
							mailboxSession = null;
						}
					}
				}
				this.sessionBorrowed = false;
				if (GlobalSettings.IsGCCEnabled)
				{
					if (GlobalSettings.AreGccStoredSecretKeysValid)
					{
						GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(this.MailboxSession, this.context.Request.GetRawHttpRequest());
					}
					else
					{
						AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_NoGccStoredSecretKey, "NoGccStoredSecretKey", new string[0]);
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "No gcc stored secret key");
					}
				}
			}
			this.MailboxSession.ExTimeZone = ExTimeZone.UtcTimeZone;
		}

		protected internal void UpdateRecipientInfoCache(RecipientCollection recipients, HashSet<RecipientId> originalRecipients)
		{
			using (RecipientInfoCache recipientInfoCache = RecipientInfoCache.Create(this.MailboxSession, "OWA.AutocompleteCache"))
			{
				List<RecipientInfoCacheEntry> list;
				try
				{
					list = recipientInfoCache.Load("AutoCompleteCache");
				}
				catch (CorruptDataException arg)
				{
					AirSyncDiagnostics.TraceDebug<CorruptDataException>(ExTraceGlobals.RequestsTracer, this, "Got a corrupt data exception! {0}", arg);
					list = new List<RecipientInfoCacheEntry>(recipients.Count);
				}
				Dictionary<string, RecipientInfoCacheEntry> dictionary = new Dictionary<string, RecipientInfoCacheEntry>(list.Count * 2 + recipients.Count, StringComparer.Ordinal);
				foreach (RecipientInfoCacheEntry recipientInfoCacheEntry in list)
				{
					if (recipientInfoCacheEntry.SmtpAddress != null)
					{
						dictionary[recipientInfoCacheEntry.SmtpAddress] = recipientInfoCacheEntry;
					}
					if (recipientInfoCacheEntry.RoutingAddress != null)
					{
						dictionary[recipientInfoCacheEntry.RoutingAddress] = recipientInfoCacheEntry;
					}
				}
				foreach (Recipient recipient in recipients)
				{
					if (originalRecipients == null || !originalRecipients.Contains(recipient.Id))
					{
						RecipientInfoCacheEntry recipientInfoCacheEntry2 = null;
						string text = recipient.Participant.TryGetProperty(ParticipantSchema.SmtpAddress) as string;
						bool flag = false;
						if (text != null)
						{
							flag = dictionary.TryGetValue(text, out recipientInfoCacheEntry2);
						}
						if (!flag && recipient.Participant.EmailAddress != null)
						{
							flag = dictionary.TryGetValue(recipient.Participant.EmailAddress, out recipientInfoCacheEntry2);
						}
						if (!flag)
						{
							recipientInfoCacheEntry2 = new RecipientInfoCacheEntry(recipient.Participant.DisplayName, text, recipient.Participant.EmailAddress, null, recipient.Participant.RoutingType, AddressOrigin.OneOff, 0, null, EmailAddressIndex.Email1, null, null);
							if (recipientInfoCacheEntry2.SmtpAddress != null)
							{
								dictionary[recipientInfoCacheEntry2.SmtpAddress] = recipientInfoCacheEntry2;
							}
							if (recipientInfoCacheEntry2.RoutingAddress != null)
							{
								dictionary[recipientInfoCacheEntry2.RoutingAddress] = recipientInfoCacheEntry2;
							}
							list.Add(recipientInfoCacheEntry2);
						}
						else
						{
							recipientInfoCacheEntry2.UpdateTimeStamp();
							recipientInfoCacheEntry2.IncrementUsage();
						}
					}
				}
				recipientInfoCache.Save(list, "AutoCompleteCache", 100);
			}
		}

		protected void SetHttpStatusCodeForTerminatedAccount(AccountState accountState)
		{
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "SetHttpStatusCodeForTerminatedAccount {0}", accountState.ToString());
			switch (accountState)
			{
			case AccountState.AccountEnabled:
				throw new InvalidOperationException("SetHttpStatusCodeForTerminatedAccount called for an enabled acount!");
			case AccountState.AccountDisabled:
				this.Context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.AccountDisabled);
				return;
			case AccountState.PasswordExpired:
				this.Context.Response.SetErrorResponse(HttpStatusCode.Unauthorized, StatusCode.AccessDenied);
				this.Context.Response.AppendHeader("WWW-Authenticate", "Basic Realm=\"\"");
				return;
			case AccountState.AccountDeleted:
				this.Context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.AccessDenied);
				return;
			case AccountState.MailboxDisabled:
				this.Context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserHasNoMailbox);
				return;
			case AccountState.ProtocolDisabled:
				this.Context.Response.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.UserDisabledForSync);
				return;
			default:
				throw new InvalidOperationException("SetHttpStatusCodeForTerminatedAccount called with an unknown account state");
			}
		}

		private void CreateNewSyncStateOnFailedUpgrade(bool shouldOpenGlobalSyncState)
		{
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Sync state storage not found.");
			this.sendServerUpgradeHeader = true;
			this.ProtocolLogger.SetValue(ProtocolLoggerData.SyncStateNotFound, "T");
			this.syncStateStorage = SyncStateStorage.Create(this.MailboxSession, this.DeviceIdentity, StateStorageFeatures.ContentState, this.Context);
			this.syncStateStorage.SaveOnDirectItems = this.Context.User.Features.IsEnabled(EasFeature.SyncStateOnDirectItems);
			AirSyncDiagnostics.TraceDebug<bool>(ExTraceGlobals.RequestsTracer, this, "[Command.CreateNewSyncStateOnFailedUpgrade] SaveOnDirectItems? {0}", this.syncStateStorage.SaveOnDirectItems);
			this.isNewSyncStateStorage = true;
			try
			{
				if (this.User.ADUser != null && !this.User.IsConsumerOrganizationUser)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[Command.CreateNewSyncStateOnFailedUpgrade] Begin updating HasDevicePartnership, Found AdUser");
					IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.User.OrganizationId), 3505, "CreateNewSyncStateOnFailedUpgrade", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\Command.cs");
					recipientSession.UseGlobalCatalog = false;
					ADObjectId adobjectId = (ADObjectId)this.User.ADUser.Identity;
					ADRecipient recipient = recipientSession.Read(adobjectId);
					this.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, recipientSession.LastUsedDc);
					ADUser aduser = recipient as ADUser;
					if (aduser != null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[Command.CreateNewSyncStateOnFailedUpgrade] Update HasDevicePartnership to true");
						aduser.MobileMailboxFlags |= MobileMailboxFlags.HasDevicePartnership;
						ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
						{
							recipientSession.Save(recipient);
						});
						if (!adoperationResult.Succeeded)
						{
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation. Message:{0}", adoperationResult.Exception.Message);
							this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SetUserHasDeviceAdException");
						}
						this.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, recipientSession.LastUsedDc);
						this.Context.ProtocolLogger.SetValue(ProtocolLoggerData.UpdateUserHasPartnerships, "T");
					}
					else
					{
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ADUserNotFound");
						AirSyncDiagnostics.TraceDebug<ADObjectId>(ExTraceGlobals.RequestsTracer, null, "Could not find ADUser with ID {0} when trying to set MobileMailboxFlags.", adobjectId);
					}
				}
			}
			catch (ADOperationException)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ADOperationException");
			}
			catch (InvalidObjectOperationException innerException)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UserObjectInvalid");
				throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, innerException, false);
			}
		}

		private SyncStateUpgradeResult UpgradeSyncStateStorage(bool shouldOpenGlobalSyncState)
		{
			SyncStateUpgradeResult syncStateUpgradeResult = SyncStateUpgradeResult.NoTiSyncState;
			if (this.Version < 120)
			{
				try
				{
					syncStateUpgradeResult = SyncStateUpgrader.CheckAndUpgradeSyncStates(this.MailboxSession, this.DeviceIdentity);
				}
				catch
				{
					this.ProtocolLogger.SetValue(ProtocolLoggerData.Ssu, "FailDuringUpgrade");
					throw;
				}
				switch (syncStateUpgradeResult)
				{
				case SyncStateUpgradeResult.UpgradeFailed:
					this.ProtocolLogger.SetValue(ProtocolLoggerData.Ssu, "FailDuringUpgrade");
					break;
				case SyncStateUpgradeResult.UpgradeComplete:
					this.ProtocolLogger.SetValue(ProtocolLoggerData.Ssu, "2003");
					break;
				}
				if (syncStateUpgradeResult == SyncStateUpgradeResult.UpgradeComplete)
				{
					this.syncStateStorage = SyncStateStorage.Bind(this.MailboxSession, this.DeviceIdentity, this.Context);
					if (this.syncStateStorage == null)
					{
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Ssu, "FailToBindToFolder");
						syncStateUpgradeResult = SyncStateUpgradeResult.UpgradeFailed;
					}
					else
					{
						this.syncStateStorage.SaveOnDirectItems = this.Context.User.Features.IsEnabled(EasFeature.SyncStateOnDirectItems);
						AirSyncDiagnostics.TraceDebug<bool>(ExTraceGlobals.RequestsTracer, this, "[Command.UpgradeSyncStateStorage] SaveOnDirectItems? {0}", this.syncStateStorage.SaveOnDirectItems);
						this.ProtocolLogger.SetValue(ProtocolLoggerData.Ssu, "2003");
						this.isNewSyncStateStorage = true;
					}
				}
			}
			if (syncStateUpgradeResult != SyncStateUpgradeResult.UpgradeComplete)
			{
				this.CreateNewSyncStateOnFailedUpgrade(shouldOpenGlobalSyncState);
			}
			return syncStateUpgradeResult;
		}

		internal void LoadMeetingOrganizerSyncState()
		{
			this.CheckDisposed();
			if (this.MeetingOrganizerSyncState != null)
			{
				return;
			}
			lock (this.contextAndSessionLock)
			{
				this.CheckDisposed();
				if (this.MeetingOrganizerSyncState == null)
				{
					if (this.syncStateStorage == null)
					{
						throw new InvalidOperationException("Sync State Storage must be opened before opening meeting organizer sync state.");
					}
					this.MeetingOrganizerSyncState = MeetingOrganizerSyncState.LoadFromMailbox(this.MailboxSession, this.syncStateStorage, this.ProtocolLogger);
				}
			}
		}

		protected internal void OpenSyncStorage(bool shouldOpenGlobalSyncState)
		{
			this.CheckDisposed();
			SyncStateUpgradeResult syncStateUpgradeResult = SyncStateUpgradeResult.NoTiSyncState;
			if (this.syncStateStorage == null)
			{
				lock (this.contextAndSessionLock)
				{
					this.CheckDisposed();
					if (this.syncStateStorage == null)
					{
						this.syncStateStorage = SyncStateStorage.Bind(this.MailboxSession, this.DeviceIdentity, this.Context);
						if (this.syncStateStorage == null)
						{
							syncStateUpgradeResult = this.UpgradeSyncStateStorage(shouldOpenGlobalSyncState);
						}
						else
						{
							this.syncStateStorage.SaveOnDirectItems = this.Context.User.Features.IsEnabled(EasFeature.SyncStateOnDirectItems);
							AirSyncDiagnostics.TraceDebug<bool>(ExTraceGlobals.RequestsTracer, this, "[Command.OpenSyncStorage] SaveOnDirectItems? {0}", this.syncStateStorage.SaveOnDirectItems);
						}
					}
					goto IL_AD;
				}
				return;
				IL_AD:
				AirSyncDiagnostics.FaultInjectionTracer.TraceTest(2242260285U);
				bool flag2 = false;
				if (shouldOpenGlobalSyncState && this.GlobalInfo == null)
				{
					this.GlobalInfo = GlobalInfo.LoadFromMailbox(this.MailboxSession, this.SyncStateStorage, this.ProtocolLogger, out flag2);
					if (syncStateUpgradeResult != SyncStateUpgradeResult.NoTiSyncState)
					{
						this.GlobalInfo.SyncStateUpgradeTime = new ExDateTime?(ExDateTime.UtcNow);
					}
				}
				if (flag2 && !this.User.IsConsumerOrganizationUser)
				{
					DeviceInfo.UpdateDeviceHasPartnership(this.MailboxSession, true);
				}
				if (this.ShouldSaveSyncStatus && this.syncStatusSyncData == null)
				{
					this.InitializeSyncStatusSyncState();
				}
				if (this.MailboxLoggingEnabled)
				{
					if (this.mailboxLogger == null)
					{
						bool flag3 = false;
						AirSyncDiagnostics.FaultInjectionTracer.TraceTest<bool>(3622186301U, ref flag3);
						if (!flag3 && this.GlobalInfo != null)
						{
							ExDateTime? nextTimeToClearMailboxLogs = this.GlobalInfo.NextTimeToClearMailboxLogs;
							flag3 = (nextTimeToClearMailboxLogs == null || ExDateTime.UtcNow > nextTimeToClearMailboxLogs.Value);
						}
						this.mailboxLogger = new MailboxLogger(this.MailboxSession, this.DeviceIdentity, flag3);
						if (!this.mailboxLogger.Enabled)
						{
							this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MailboxLoggerError");
							AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "MailboxLogger failed to initialize.  Exception message: {0}\nStack trace: {1}", this.mailboxLogger.LastError.Message, this.mailboxLogger.LastError.StackTrace);
						}
						else if (flag3 && this.GlobalInfo != null)
						{
							this.GlobalInfo.NextTimeToClearMailboxLogs = new ExDateTime?(ExDateTime.UtcNow.AddHours(6.0));
						}
					}
					if (this.mailboxLogger.MailboxSession == null)
					{
						this.mailboxLogger.MailboxSession = this.MailboxSession;
					}
				}
				return;
			}
		}

		protected virtual void ReleaseResources()
		{
			if (this.mailboxLogger != null)
			{
				this.mailboxLogger.MailboxSession = null;
			}
			if (this.syncStatusSyncData != null)
			{
				IDisposable disposable = this.syncStatusSyncData as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				this.syncStatusSyncData = null;
			}
			if (this.Context != null && this.Context.DeviceBehavior != null)
			{
				this.Context.DeviceBehavior.Owner = null;
				this.Context.DeviceBehavior.ProtocolLogger = null;
				this.Context.DeviceBehavior = null;
			}
			if (this.GlobalInfo != null)
			{
				this.GlobalInfo.Dispose();
				this.GlobalInfo = null;
			}
			if (this.MeetingOrganizerSyncState != null)
			{
				this.MeetingOrganizerSyncState.Dispose();
				this.MeetingOrganizerSyncState = null;
			}
			if (!this.sessionBorrowed)
			{
				if (this.syncStateStorage != null)
				{
					this.syncStateStorage.Dispose();
					this.syncStateStorage = null;
				}
				if (this.securityContextAndSession != null)
				{
					bool flag = false;
					if (!this.PartialFailure && !this.context.Response.IsErrorResponse)
					{
						MailboxSession mailboxSession = this.MailboxSession;
						if (mailboxSession.IsConnected)
						{
							mailboxSession.Disconnect();
						}
						flag = MailboxSessionCache.AddOrReplace(this.User.ExchangePrincipal.ObjectId.ObjectGuid, this.securityContextAndSession);
					}
					if (!flag)
					{
						MailboxSessionCache.IncrementDiscardedSessions();
						this.securityContextAndSession.Dispose();
					}
					this.securityContextAndSession = null;
				}
			}
			if (this.deviceManager != null)
			{
				this.deviceManager = null;
			}
		}

		protected int GetNextNumber(int number)
		{
			return this.GetNextNumber(number, false);
		}

		protected int GetNextNumber(int number, bool alwaysGetRandom)
		{
			int num = 0;
			if (!alwaysGetRandom)
			{
				return number + 1;
			}
			lock (Command.randomGenerator)
			{
				while (num == 0 || num == number)
				{
					num = Command.randomGenerator.Next();
				}
			}
			AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, this, "Previous number: {0}, New GetNextNumber: {1}", number, num);
			return num;
		}

		internal void DecodeIrmMessage(Item mailboxItem, bool acquireLicense)
		{
			RightsManagedMessageItem rightsManagedMessageItem = mailboxItem as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null)
			{
				if (!this.Context.User.IrmEnabled)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "User {0} is not IrmEnabled or the client access server is not IrmEnable. Ignoring the Irm decoding of messages.", this.User.DisplayName);
					return;
				}
				if (!this.Context.Request.IsSecureConnection)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "User {0} sent request on non SSL channnel. Ignoring the Irm decoding of messages.", this.User.DisplayName);
					return;
				}
				if (!rightsManagedMessageItem.IsRestricted)
				{
					AirSyncDiagnostics.TraceError<VersionedId>(ExTraceGlobals.RequestsTracer, null, "Rights managed item {0} is not restricted", mailboxItem.Id);
					return;
				}
				if (!rightsManagedMessageItem.CanDecode)
				{
					AirSyncDiagnostics.TraceDebug<VersionedId>(ExTraceGlobals.RequestsTracer, null, "Rights managed item {0} can not be decoded on server", mailboxItem.Id);
					return;
				}
				if (rightsManagedMessageItem.IsDecoded)
				{
					return;
				}
				RightsManagedMessageDecryptionStatus rightsManagedMessageDecryptionStatus = rightsManagedMessageItem.TryDecode(AirSyncUtility.GetOutboundConversionOptions(), acquireLicense);
				if (rightsManagedMessageDecryptionStatus.Failed)
				{
					AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.XsoTracer, null, "Failed to decode the message. Exception {0}", rightsManagedMessageDecryptionStatus.Exception);
					if (this.MailboxLogger != null)
					{
						this.MailboxLogger.SetData(MailboxLogDataName.IRM_Exception, rightsManagedMessageDecryptionStatus.Exception);
						RightsManagementPermanentException ex = rightsManagedMessageDecryptionStatus.Exception as RightsManagementPermanentException;
						if (ex != null)
						{
							this.MailboxLogger.SetData(MailboxLogDataName.IRM_FailureCode, ex.FailureCode);
						}
					}
				}
			}
			AirSyncCounters.NumberOfIRMMailsDownloads.Increment();
		}

		internal void SaveLicense(Item item)
		{
			if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
			{
				RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
				try
				{
					rightsManagedMessageItem.SaveUseLicense();
				}
				catch (AccessDeniedException)
				{
					AirSyncDiagnostics.TraceError<VersionedId, string>(ExTraceGlobals.XsoTracer, null, "Failed to write the license back for the messageItem {0} for user {1}", item.Id, this.Context.User.DisplayName);
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.ReleaseDisposableData();
				lock (this.contextAndSessionLock)
				{
					this.ReleaseResources();
					this.disposed = true;
				}
				if (this.completionTimer != null)
				{
					this.completionTimer.Dispose();
					this.completionTimer = null;
				}
				if (this.mailboxLogger != null)
				{
					this.mailboxLogger.Dispose();
					this.mailboxLogger = null;
				}
				if (this.budget != null)
				{
					try
					{
						this.budget.Dispose();
					}
					catch (FailFastException arg)
					{
						AirSyncDiagnostics.TraceError<FailFastException>(ExTraceGlobals.RequestsTracer, null, "Budget.Dispose failed with exception: {0}", arg);
					}
				}
				this.disposed = true;
			}
		}

		protected bool IsInboxFolder(StoreObjectId folderId)
		{
			return this.MailboxSession != null && folderId != null && folderId.Equals(this.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox));
		}

		private void CompleteHttpRequestCallback(object state)
		{
			this.ProtocolLogger.SetValue(ProtocolLoggerData.FinalElapsedTime, (int)ExDateTime.UtcNow.Subtract(this.Context.RequestTime).TotalMilliseconds);
			this.context.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.TimeCompleted, ExDateTime.UtcNow);
			this.result.InvokeCallback();
		}

		private void LogResponseToMailbox(bool pending)
		{
			try
			{
				if (this.MailboxLoggingEnabled && this.mailboxLogger != null && this.mailboxLogger.Enabled)
				{
					if (!pending)
					{
						this.mailboxLogger.LogResponseHead(this.context.Response);
						if (!this.mailboxLogger.DataExists(MailboxLogDataName.ResponseBody))
						{
							this.mailboxLogger.SetData(MailboxLogDataName.ResponseBody, (this.XmlResponse == null) ? "[No XmlResponse]" : AirSyncUtility.BuildOuterXml(this.XmlResponse, !GlobalSettings.EnableMailboxLoggingVerboseMode));
						}
						this.mailboxLogger.SetData(MailboxLogDataName.ResponseTime, ExDateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo));
					}
					this.mailboxLogger.SaveLogToMailbox();
				}
			}
			catch (LocalizedException arg)
			{
				AirSyncDiagnostics.TraceError<LocalizedException>(ExTraceGlobals.RequestsTracer, this, "Failed to LogResponseToMailbox: {0}", arg);
			}
		}

		private void TraceStop(Guid serviceProviderRequestId)
		{
			Microsoft.Exchange.Diagnostics.Trace.TraceCasStop(CasTraceEventType.ActiveSync, serviceProviderRequestId, this.Request.ContentLength, 0, Command.MachineName, this.User.Name, "WorkerThread", this.Request.PathAndQuery, string.Empty);
		}

		private DeviceAccessState DetermineDeviceAccessState(out DeviceAccessStateReason accessStateReason)
		{
			DeviceAccessState deviceAccessState = DeviceAccessState.Unknown;
			accessStateReason = DeviceAccessStateReason.Unknown;
			DeviceAccessState deviceAccessState2;
			try
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "DetermineDeviceAccessState: Enter");
				if (this.User.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.MonitoringMailbox)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Monitoring mailboxes are always allowed.");
					accessStateReason = DeviceAccessStateReason.Global;
					deviceAccessState = DeviceAccessState.Allowed;
					deviceAccessState2 = deviceAccessState;
				}
				else if (this.Request.CommandType == CommandType.Options)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "OPTIONS is always allowed.");
					accessStateReason = DeviceAccessStateReason.Global;
					deviceAccessState = DeviceAccessState.Allowed;
					deviceAccessState2 = deviceAccessState;
				}
				else
				{
					if (this.GlobalInfo == null)
					{
						throw new InvalidOperationException("GlobalInfo should not be null at this point!");
					}
					this.GlobalInfo.DeviceAccessControlRule = null;
					string item = (this.Request.CommandType == CommandType.Options) ? null : this.DeviceIdentity.DeviceId;
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "1. Check individual user blocked list");
					MultiValuedProperty<string> activeSyncBlockedDeviceIDs = this.User.ADUser.ActiveSyncBlockedDeviceIDs;
					if (activeSyncBlockedDeviceIDs.Contains(item))
					{
						accessStateReason = DeviceAccessStateReason.Individual;
						deviceAccessState = DeviceAccessState.Blocked;
						deviceAccessState2 = deviceAccessState;
					}
					else
					{
						this.Context.DeviceBehavior = DeviceBehavior.GetDeviceBehavior(this.User.ADUser.OriginalId.ObjectGuid, this.DeviceIdentity, this.GlobalInfo, this, this.ProtocolLogger);
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "User agent is " + this.EffectiveUserAgent);
						TimeSpan t;
						DeviceAccessStateReason deviceAccessStateReason = this.Context.DeviceBehavior.IsDeviceAutoBlocked(this.EffectiveUserAgent, out t);
						if (deviceAccessStateReason != DeviceAccessStateReason.Unknown)
						{
							if (t > TimeSpan.Zero)
							{
								AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Device is still blocked, no need to re-check");
							}
							this.context.Response.AppendHeader("Retry-After", t.TotalSeconds.ToString(), false);
							accessStateReason = deviceAccessStateReason;
							deviceAccessState = DeviceAccessState.Blocked;
							deviceAccessState2 = deviceAccessState;
						}
						else
						{
							this.Context.DeviceBehavior.RecordNewUserAgent(this.Request.UserAgent);
							if (this.Request.CommandType != CommandType.Sync && this.Request.CommandType != CommandType.Ping)
							{
								this.Context.DeviceBehavior.RecordCommand(this);
							}
							AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "1.1 Check auto block.");
							deviceAccessStateReason = this.Context.DeviceBehavior.IsDeviceAutoBlocked(this.Context.RequestTime, out t);
							if (deviceAccessStateReason != DeviceAccessStateReason.Unknown && t > TimeSpan.Zero)
							{
								this.context.Response.AppendHeader("Retry-After", t.TotalSeconds.ToString(), false);
								accessStateReason = deviceAccessStateReason;
								deviceAccessState = DeviceAccessState.Blocked;
								deviceAccessState2 = deviceAccessState;
							}
							else
							{
								if (this.User.OrganizationId != OrganizationId.ForestWideOrgId)
								{
									IOrganizationSettingsData organizationSettingsData = ADNotificationManager.GetOrganizationSettingsData(OrganizationId.ForestWideOrgId, this.User.Context);
									if (organizationSettingsData == null)
									{
										AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Cannot find OrganizationSetting for forest wide org");
										AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_NoOrgSettings, new string[]
										{
											this.User.OrganizationId.ToString()
										});
										throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, null, false)
										{
											ErrorStringForProtocolLogger = "ForestWideOrgSettingsNotFound"
										};
									}
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "2.1 Check forest-wide rules.");
									ADObjectId adobjectId;
									Command.DetermineDeviceAccessState(organizationSettingsData, this.DeviceIdentity.DeviceType, this.GlobalInfo.DeviceModel, this.EffectiveUserAgent, this.GlobalInfo.DeviceOS, out deviceAccessState, out accessStateReason, out adobjectId);
									if (adobjectId != null)
									{
										AirSyncDiagnostics.TraceInfo<ADObjectId>(ExTraceGlobals.RequestsTracer, this, "Found matching rule. {0}", adobjectId);
										this.GlobalInfo.DeviceAccessControlRule = adobjectId;
										return deviceAccessState;
									}
								}
								AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "3. Check individual user allowed list.");
								MultiValuedProperty<string> activeSyncAllowedDeviceIDs = this.User.ADUser.ActiveSyncAllowedDeviceIDs;
								if (activeSyncAllowedDeviceIDs.Contains(item))
								{
									accessStateReason = DeviceAccessStateReason.Individual;
									deviceAccessState = DeviceAccessState.Allowed;
									deviceAccessState2 = deviceAccessState;
								}
								else
								{
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4. Check device access rule settings.");
									IOrganizationSettingsData organizationSettingsData2 = ADNotificationManager.GetOrganizationSettingsData(this.User);
									if (organizationSettingsData2 == null)
									{
										AirSyncDiagnostics.TraceInfo<OrganizationId>(ExTraceGlobals.RequestsTracer, this, "Cannot find OrganizationSetting for org {0}", this.User.OrganizationId);
										AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_NoOrgSettings, new string[]
										{
											this.User.OrganizationId.ToString()
										});
										throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, null, false)
										{
											ErrorStringForProtocolLogger = "OrgSettingsNotFound"
										};
									}
									if (!organizationSettingsData2.IsRulesListEmpty)
									{
										AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.1 Did we receive <DeviceInformationSettings> already?");
										if (!this.GlobalInfo.DeviceInformationReceived && !this.GlobalInfo.DeviceInformationPromoted)
										{
											AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.1.1 We haven't received device information settings yet.");
											if (this.GlobalInfo.SyncStateUpgradeTime != null && this.GlobalInfo.SyncStateUpgradeTime.Value.AddMinutes((double)GlobalSettings.UpgradeGracePeriod) >= ExDateTime.UtcNow)
											{
												AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.1.1.1 We are in upgrade grace period.");
												accessStateReason = DeviceAccessStateReason.Upgrade;
												deviceAccessState = DeviceAccessState.Allowed;
												return deviceAccessState;
											}
											if (this.syncStateStorage.CreationTime.AddMinutes((double)GlobalSettings.DeviceDiscoveryPeriod) >= ExDateTime.UtcNow)
											{
												AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.1.1.2 DeviceDiscovery.");
												accessStateReason = DeviceAccessStateReason.DeviceRule;
												deviceAccessState = DeviceAccessState.DeviceDiscovery;
												return deviceAccessState;
											}
										}
									}
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "3.1.2 We have device information or we have passed upgrade grace period and DeviceDiscovery check.");
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.2 Check rules and global state.");
									ADObjectId adobjectId;
									Command.DetermineDeviceAccessState(organizationSettingsData2, this.DeviceIdentity.DeviceType, this.GlobalInfo.DeviceModel, this.EffectiveUserAgent, this.GlobalInfo.DeviceOS, out deviceAccessState, out accessStateReason, out adobjectId);
									if (adobjectId != null)
									{
										AirSyncDiagnostics.TraceInfo<ADObjectId>(ExTraceGlobals.RequestsTracer, this, "Found matching rule. {0}", adobjectId);
										this.GlobalInfo.DeviceAccessControlRule = adobjectId;
										deviceAccessState2 = deviceAccessState;
									}
									else
									{
										if (accessStateReason == DeviceAccessStateReason.Global && deviceAccessState == DeviceAccessState.Quarantined)
										{
											AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.3 Global access level is quarantine.");
											if (this.GlobalInfo.SyncStateUpgradeTime != null && this.GlobalInfo.SyncStateUpgradeTime.Value.AddMinutes((double)GlobalSettings.UpgradeGracePeriod) >= ExDateTime.UtcNow)
											{
												AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.3.1 We are in upgrade grace period.");
												accessStateReason = DeviceAccessStateReason.Upgrade;
												deviceAccessState = DeviceAccessState.Allowed;
												return deviceAccessState;
											}
											if (this.GlobalInfo.DeviceInformationReceived)
											{
												AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.3.2 We have received device information.");
												accessStateReason = DeviceAccessStateReason.Global;
												deviceAccessState = DeviceAccessState.Quarantined;
												return deviceAccessState;
											}
											if (this.GlobalInfo.DeviceInformationPromoted)
											{
												this.ProtocolLogger.SetValue(ProtocolLoggerData.AccessStateAndReason, "DeviceInformationPromoted");
												AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Device type has been promoted to device model.");
												accessStateReason = DeviceAccessStateReason.Global;
												deviceAccessState = DeviceAccessState.Quarantined;
												return deviceAccessState;
											}
											if (this.syncStateStorage.CreationTime.AddMinutes((double)GlobalSettings.DeviceDiscoveryPeriod) >= ExDateTime.UtcNow)
											{
												AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.3.3 DeviceDiscovery.");
												accessStateReason = DeviceAccessStateReason.Global;
												deviceAccessState = DeviceAccessState.DeviceDiscovery;
												return deviceAccessState;
											}
										}
										AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "4.4 Everything else...");
										deviceAccessState2 = deviceAccessState;
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				AirSyncDiagnostics.TraceInfo<DeviceAccessState, DeviceAccessStateReason>(ExTraceGlobals.RequestsTracer, this, "Result: State:{0}, Reason {1}", deviceAccessState, accessStateReason);
			}
			return deviceAccessState2;
		}

		private bool ShouldSendMDMComplianceNotificationMail()
		{
			if ((!this.Context.User.Features.IsEnabled(EasFeature.CloudMDMEnrolled) && !ADNotificationManager.GetOrganizationSettingsData(this.User).IsIntuneManaged) || !DeviceCapability.DeviceSupportedForMdm(this.GlobalInfo))
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Skip Mdm email because not does not qualify.");
				return false;
			}
			if (this.GlobalInfo.DeviceAccessState == this.PreviousAccessState)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Skip Mdm email device is in previous state.");
				return false;
			}
			if (this.GlobalInfo.DeviceAccessState != DeviceAccessState.Quarantined || this.GlobalInfo.DeviceAccessStateReason <= DeviceAccessStateReason.ExternallyManaged)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Skip Mdm email device is not externally managed.");
				return false;
			}
			return true;
		}

		private bool ShouldSendABQNotificationMail()
		{
			return this.GlobalInfo.DeviceAccessStateReason != DeviceAccessStateReason.Policy && this.GlobalInfo.DeviceAccessState != this.PreviousAccessState && (this.GlobalInfo.DeviceAccessState == DeviceAccessState.Quarantined || this.GlobalInfo.DeviceAccessState == DeviceAccessState.Blocked) && this.GlobalInfo.DeviceAccessStateReason < DeviceAccessStateReason.UserAgentsChanges;
		}

		private bool ShouldSendAutoBlockNotificationMail(out TimeSpan blockTime)
		{
			blockTime = TimeSpan.Zero;
			return this.GlobalInfo.DeviceAccessStateReason >= DeviceAccessStateReason.UserAgentsChanges && this.GlobalInfo.DeviceAccessState != this.PreviousAccessState && ADNotificationManager.GetAutoBlockThreshold(AutoblockThresholdType.EnableNotificationEmail).BehaviorTypeIncidenceLimit != 0 && !(ADNotificationManager.GetAutoBlockThreshold(this.GlobalInfo.DeviceAccessStateReason).DeviceBlockDuration == TimeSpan.Zero) && this.Context.DeviceBehavior.IsDeviceAutoBlocked(this.Context.RequestTime, out blockTime) != DeviceAccessStateReason.Unknown && !(blockTime <= TimeSpan.Zero);
		}

		private object GetRequestDataToLog(string requestToLog)
		{
			if (GlobalSettings.EnableMailboxLoggingVerboseMode)
			{
				return requestToLog;
			}
			string value = Regex.Match(requestToLog, "<Mime>([\\s\\S]*)</Mime>", RegexOptions.IgnoreCase).Value;
			return Regex.Replace(requestToLog, "<Mime>([\\s\\S]*)</Mime>", string.Format("<Mime>[Mime Removed] BytesCount = {0}</Mime>", Encoding.Default.GetByteCount(value).ToString()), RegexOptions.IgnoreCase);
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("Cannot open disposable state on a command after the command has been disposed of.");
			}
		}

		private void CompleteDeviceAccessProcessing()
		{
			if (string.IsNullOrEmpty(this.GlobalInfo.DeviceModel))
			{
				this.GlobalInfo.DeviceModel = this.DeviceIdentity.DeviceType;
			}
			if (!string.IsNullOrEmpty(this.Request.UserAgent) && !string.Equals(this.GlobalInfo.UserAgent, this.Request.UserAgent, StringComparison.OrdinalIgnoreCase))
			{
				this.GlobalInfo.UserAgent = this.Request.UserAgent;
			}
			string deviceOS;
			if ((string.IsNullOrEmpty(this.GlobalInfo.DeviceOS) || string.Equals(this.GlobalInfo.DeviceOS, "Android", StringComparison.OrdinalIgnoreCase)) && this.TryParseDeviceOSFromUserAgent(out deviceOS))
			{
				this.GlobalInfo.DeviceOS = deviceOS;
			}
			this.UpdateADDevice(this.GlobalInfo);
			if (!string.IsNullOrEmpty(this.DeviceIdentity.DeviceType) && !string.IsNullOrEmpty(this.GlobalInfo.DeviceModel) && !this.User.IsConsumerOrganizationUser)
			{
				DeviceClassCache.Instance.Add(this.User.OrganizationId, this.DeviceIdentity.DeviceType, this.GlobalInfo.DeviceModel);
			}
			if (this.ShouldSendABQNotificationMail())
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sending notification mail for user: '{0}', device identity: '{1}', AccessState: {2}, Previous AccessState: {3}, PreviousAccessStateReason: {4}", new object[]
				{
					this.User.Name,
					this.DeviceIdentity,
					this.CurrentAccessState,
					this.GlobalInfo.DeviceAccessState,
					this.GlobalInfo.DeviceAccessStateReason
				});
				IOrganizationSettingsData organizationSettingsData = ADNotificationManager.GetOrganizationSettingsData(this.User);
				ABQMailHelper abqmailHelper = new ABQMailHelper(this.GlobalInfo, this.Context, organizationSettingsData);
				abqmailHelper.SendABQNotificationMail();
				this.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.MailSent, "ABQ");
			}
			TimeSpan blockTime;
			if (this.ShouldSendAutoBlockNotificationMail(out blockTime))
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sending auto-block notification mail for user: '{0}', device identifier: '{1}', AccessState: {2}, Previous AccessState: {3}, PreviousAccessStateReason: {4}", new object[]
				{
					this.User.Name,
					this.DeviceIdentity,
					this.CurrentAccessState,
					this.GlobalInfo.DeviceAccessState,
					this.GlobalInfo.DeviceAccessStateReason
				});
				IOrganizationSettingsData organizationSettingsData2 = ADNotificationManager.GetOrganizationSettingsData(OrganizationId.ForestWideOrgId, this.Context);
				ABQMailHelper abqmailHelper2 = new ABQMailHelper(this.GlobalInfo, this.Context, organizationSettingsData2);
				abqmailHelper2.SendAutoBlockNotificationMail(blockTime, ADNotificationManager.GetAutoBlockThreshold(this.Context.DeviceBehavior.AutoBlockReason).AdminEmailInsert);
				this.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.MailSent, "B");
			}
			if (this.ShouldSendMDMComplianceNotificationMail())
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sending MDM Quarantine email for user: '{0}', Device Identity: '{1}', AccessState: {2}, Previous AccessState: {3}, PreviousAccessStateReason: {4}, IsManaged: {5}, isCompliant: {6}", new object[]
				{
					this.User.Name,
					this.DeviceIdentity,
					this.CurrentAccessState,
					this.GlobalInfo.DeviceAccessState,
					this.GlobalInfo.DeviceAccessStateReason,
					this.isManaged,
					this.isCompliant
				});
				IOrganizationSettingsData organizationSettingsData3 = ADNotificationManager.GetOrganizationSettingsData(this.User);
				ABQMailHelper abqmailHelper3 = new ABQMailHelper(this.GlobalInfo, this.Context, organizationSettingsData3);
				abqmailHelper3.SendMdmQuarantineEmail(this.GlobalInfo.DeviceAccessStateReason != DeviceAccessStateReason.ExternalEnrollment);
				this.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.MailSent, (this.GlobalInfo.DeviceAccessStateReason == DeviceAccessStateReason.ExternalEnrollment) ? "M" : "C");
			}
		}

		internal bool ScheduleTask()
		{
			string action = null;
			try
			{
				ActivityContext.SetThreadScope(this.context.ActivityScope);
				action = this.User.Context.ActivityScope.Action;
				this.User.Context.ActivityScope.Action = this.RootNodeName;
				this.maxExecutionTime = this.context.RequestTime + TimeSpan.FromSeconds((double)GlobalSettings.MaxThrottlingDelay) - ExDateTime.UtcNow;
				if (this.maxExecutionTime <= TimeSpan.Zero)
				{
					AirSyncDiagnostics.TraceError<ExDateTime, int, ExDateTime>(ExTraceGlobals.RequestsTracer, this, "Request is over max delay! requestTime: {0}, MaxThrottlingDelay: {1}, utcNow: {2}", this.context.RequestTime, GlobalSettings.MaxThrottlingDelay, ExDateTime.UtcNow);
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "RequestOverMaxDelay");
					this.context.Response.IssueErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.ServerErrorRetryLater);
					this.CompleteHttpRequest();
					return false;
				}
			}
			finally
			{
				ActivityContext.ClearThreadScope();
				if (this.User != null && this.User.Context != null && this.User.Context.ActivityScope != null)
				{
					this.User.Context.ActivityScope.Action = action;
				}
			}
			TimeSpan timeSpan = ExDateTime.UtcNow - this.context.RequestTime;
			this.context.SetDiagnosticValue(ConditionalHandlerSchema.PreWlmElapsed, timeSpan);
			AirSyncDiagnostics.TraceInfo<TimeSpan>(ExTraceGlobals.RequestsTracer, this, "[Command.ScheduleTask] Submitting task to WLM.  Pre-WLM time spent: {0}", timeSpan);
			bool flag = UserWorkloadManager.Singleton.TrySubmitNewTask(this);
			if (!flag)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "TrySubmitNewTask failure!");
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "TrySubmitNewTaskFailure");
				this.context.Response.IssueErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.ServerErrorRetryLater);
				this.CompleteHttpRequest();
			}
			return flag;
		}

		public IActivityScope GetActivityScope()
		{
			if (this.User != null && this.User.Context != null)
			{
				return this.User.Context.ActivityScope;
			}
			return null;
		}

		public TaskExecuteResult Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			this.ExecuteWithCommandTls(delegate
			{
				AirSyncDiagnostics.TraceInfo<TimeSpan, TimeSpan>(ExTraceGlobals.RequestsTracer, this, "[Command.Execute] ITask.Execute called.  QueueAndDelayTime: {0}, TotalTime: {1}", queueAndDelayTime, totalTime);
				this.ProtocolLogger.SetValue(ProtocolLoggerData.ThrottledTime, (int)queueAndDelayTime.TotalMilliseconds);
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.TimeStarted, ExDateTime.UtcNow);
				using (ExPerfTrace.RelatedActivity(this.GetTraceActivityId()))
				{
					this.WorkerThread();
				}
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.TimeFinished, ExDateTime.UtcNow);
			});
			this.context.SetDiagnosticValue(ConditionalHandlerSchema.CommandElapsed, ExDateTime.UtcNow - utcNow);
			return TaskExecuteResult.ProcessingComplete;
		}

		public void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.ExecuteWithCommandTls(delegate
			{
				AirSyncDiagnostics.TraceInfo<TimeSpan, TimeSpan, Command.ExecutionState>(ExTraceGlobals.RequestsTracer, this, "[Command.Complete] ITask.Complete called.  QueueAndDelayTime: {0}, TotalTime: {1}.  ExecutionState: {2}", queueAndDelayTime, totalTime, this.executionState);
				if (this.executionState != Command.ExecutionState.Pending)
				{
					this.context.SetDiagnosticValue(ConditionalHandlerIntermediateSchema.PostWlmStartTime, ExDateTime.UtcNow);
					this.CompleteHttpRequest();
				}
			});
		}

		public void Cancel()
		{
			this.ExecuteWithCommandTls(delegate
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CancelFromWLM");
				this.context.SetDiagnosticValue(ConditionalHandlerIntermediateSchema.PostWlmStartTime, ExDateTime.UtcNow);
				this.context.Response.SetErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.ServerErrorRetryLater);
				this.context.Response.IssueWbXmlResponse();
				this.CompleteHttpRequest();
			});
		}

		public IBudget Budget
		{
			get
			{
				return this.budget;
			}
		}

		public TimeSpan MaxExecutionTime
		{
			get
			{
				return this.maxExecutionTime;
			}
		}

		public void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.context.SetDiagnosticValue(ConditionalHandlerSchema.WlmQueueElapsed, queueAndDelayTime);
			this.ExecuteWithCommandTls(delegate
			{
				AirSyncDiagnostics.TraceInfo<TimeSpan, TimeSpan>(ExTraceGlobals.RequestsTracer, this, "[Command.Timeout] ITask.Timeout called.  QueueAndDelayTime: {0}, TotalTime: {1}", queueAndDelayTime, totalTime);
				this.context.SetDiagnosticValue(ConditionalHandlerIntermediateSchema.PostWlmStartTime, ExDateTime.UtcNow);
				this.ProtocolLogger.SetValue(ProtocolLoggerData.ThrottledTime, (int)queueAndDelayTime.TotalMilliseconds);
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "WLMTimeout");
				this.context.Response.SetErrorResponse(HttpStatusCode.ServiceUnavailable, StatusCode.ServerErrorRetryLater);
				this.context.Response.IssueWbXmlResponse();
				this.CompleteHttpRequest();
			});
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			this.ExecuteWithCommandTls(delegate
			{
				AirSyncDiagnostics.TraceInfo<LocalizedException>(ExTraceGlobals.RequestsTracer, this, "[Command.Execute] ITask.CancelStep called.  Exception: {0}", exception);
				AirSyncUtility.ProcessException(exception, this, this.context);
				this.context.Response.IssueWbXmlResponse();
			});
			return TaskExecuteResult.ProcessingComplete;
		}

		public ResourceKey[] GetResources()
		{
			return null;
		}

		public WorkloadSettings WorkloadSettings { get; private set; }

		public object State
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public string Description
		{
			get
			{
				return this.context.TaskDescription;
			}
			set
			{
			}
		}

		public TimeSpan GetActionTimeout(CostType costType)
		{
			return Microsoft.Exchange.Data.Directory.Budget.GetMaxActionTime(costType);
		}

		protected Guid GetTraceActivityId()
		{
			IActivityScope activityScope = this.GetActivityScope();
			if (activityScope != null)
			{
				return activityScope.ActivityId;
			}
			return Guid.NewGuid();
		}

		protected void AddHeadersForEnterpriseOrgUser()
		{
			AirSyncDiagnostics.Assert(!this.User.IsConsumerOrganizationUser, "User does not belong to the enterprise org.", new object[0]);
			string value;
			if (this.Context.Request.CommandType == CommandType.Options)
			{
				value = (this.Context.User.Features.IsEnabled(EasFeature.EnableV160) ? Constants.ProtocolExperimantalVersionsHeaderValue : Constants.ProtocolVersionsHeaderValue);
				if (this.Version == 120)
				{
					this.Context.Response.AppendHeader("MS-ASSeamlessUpgradeVersions", "12.1");
				}
			}
			else
			{
				value = Constants.ProtocolVersionsHeaderValue;
				this.Context.Response.AppendHeader("X-MS-RP", value);
			}
			this.Context.Response.AppendHeader("MS-ASProtocolVersions", value);
			this.Context.Response.AppendHeader("MS-ASProtocolCommands", "Sync,SendMail,SmartForward,SmartReply,GetAttachment,GetHierarchy,CreateCollection,DeleteCollection,MoveCollection,FolderSync,FolderCreate,FolderDelete,FolderUpdate,MoveItems,GetItemEstimate,MeetingResponse,Search,Settings,Ping,ItemOperations,Provision,ResolveRecipients,ValidateCert");
		}

		protected void AddHeadersForConsumerOrgUser()
		{
			AirSyncDiagnostics.Assert(this.User.IsConsumerOrganizationUser, "User does not belong to the consumer org.", new object[0]);
			string value;
			string value2;
			if (DeviceFilterManager.ContactsOnly)
			{
				value = "14.0";
				value2 = "Sync,FolderSync,GetItemEstimate,Ping";
			}
			else if (DeviceFilterManager.V25OnlyInOptions)
			{
				value = "2.5";
				value2 = "Sync,SendMail,SmartForward,SmartReply,GetAttachment,FolderSync,FolderCreate,FolderDelete,FolderUpdate,MoveItems,GetItemEstimate,MeetingResponse,Ping";
			}
			else
			{
				value = "2.5,14.0";
				value2 = "Sync,SendMail,SmartForward,SmartReply,GetAttachment,FolderSync,FolderCreate,FolderDelete,FolderUpdate,MoveItems,GetItemEstimate,MeetingResponse,Search,Settings,Ping,ItemOperations";
			}
			if (this.Context.Request.CommandType != CommandType.Options)
			{
				this.Context.Response.AppendHeader("X-MS-RP", value);
			}
			this.Context.Response.AppendHeader("MS-ASProtocolVersions", value);
			this.Context.Response.AppendHeader("MS-ASProtocolCommands", value2);
			this.Context.Response.AppendHeader("X-OLK-Extensions", "1=0E47");
		}

		private const int ExpectedPropertyGroupMappingId = 514;

		private const string AndroidBaseOSString = "Android";

		private const string regexToMatchMimeTag = "<Mime>([\\s\\S]*)</Mime>";

		private const string replacedMimeTagText = "<Mime>[Mime Removed] BytesCount = {0}</Mime>";

		private static long numSkinnyICSFolderChecks = 0L;

		private static long numFatDeepTraversalFolderChecks = 0L;

		private static readonly string machineName = Environment.MachineName;

		private static readonly TimeSpan timeAllowedToCompleteEarly = new TimeSpan(0, 0, 0, 0, GlobalSettings.EarlyCompletionTolerance);

		private static readonly TimeSpan disablePeriodsTimespan = new TimeSpan(0, 0, 0, 0, -1);

		private static readonly TimeSpan bootstrapMailDeliveryDelay = new TimeSpan(0, 0, GlobalSettings.BootstrapMailDeliveryDelay);

		[ThreadStatic]
		private static Stack<Command> commandStack;

		private static Random randomGenerator = new Random();

		private IStandardBudget budget;

		private bool? mailboxLoggingEnabled;

		private bool disposed;

		private bool sessionBorrowed;

		private bool isManaged = true;

		private bool isCompliant = true;

		protected int validToCommitSyncStatusSyncState;

		private Stopwatch requestWaitWatch;

		private IAirSyncContext context;

		private bool sendServerUpgradeHeader;

		private ISyncStatusData syncStatusSyncData;

		private ExPerformanceCounter perfCounter;

		private int requestId;

		private bool perUserTracingEnabled;

		private MailboxLogger mailboxLogger;

		private SecurityContextAndSession securityContextAndSession;

		private object contextAndSessionLock = new object();

		private SyncStateStorage syncStateStorage;

		private LazyAsyncResult result;

		private DisposeTracker disposeTracker;

		private ADDeviceManager deviceManager;

		private ExDateTime nextPolicyRefreshTime;

		private Timer completionTimer;

		private bool isNewSyncStateStorage;

		private HashSet<IDisposable> dataToBeDisposed = new HashSet<IDisposable>();

		private Conversation cachedConversation;

		private Dictionary<StoreObjectId, Dictionary<AttachmentId, string>> inlineAttachmentContentIdLookUp;

		private HashSet<ConversationId> openedConversationIdList = new HashSet<ConversationId>();

		private TimeSpan maxExecutionTime;

		private Command.ExecutionState executionState;

		private bool shouldWatsonWhenReloadingSameConversation;

		protected enum IcsFolderCheckResults
		{
			NoChanges,
			ChangesNoDeepCheck,
			ChangesNeedDeepCheck
		}

		internal enum ExecutionState
		{
			Invalid,
			Pending,
			Complete
		}

		public class StateData
		{
			public string DevicePhoneNumberForSms { get; set; }

			public bool DeviceEnableOutboundSMS { get; set; }
		}
	}
}
