using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.AirSync
{
	internal class AirSyncConditionalHandlerSchema : ConditionalHandlerSchema
	{
		public static readonly SimpleProviderPropertyDefinition FilterChangeSync = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("FilterChangeSync");

		public static readonly SimpleProviderPropertyDefinition InitialSync = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("InitialSync");

		public static readonly SimpleProviderPropertyDefinition IcsSync = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("IcsSync");

		public static readonly SimpleProviderPropertyDefinition ItemQuerySync = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("ItemQuerySync");

		public static readonly SimpleProviderPropertyDefinition AnyCollectionEmptyWithMoreAvailable = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("AnyCollectionEmptyWithMoreAvailable");

		public static readonly SimpleProviderPropertyDefinition IsLogicallyEmptyResponse = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("IsLogicallyEmptyResponse");

		public static readonly SimpleProviderPropertyDefinition PerCallTracing = ConditionalHandlerSchema.BuildStringPropDef("PerCallTracing");

		public static readonly SimpleProviderPropertyDefinition EmptyRequest = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("EmptyRequest");

		public static readonly SimpleProviderPropertyDefinition PartialRequest = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("PartialRequest");

		public static readonly SimpleProviderPropertyDefinition LoadedCachedRequest = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("LoadedCachedRequest");

		public static readonly SimpleProviderPropertyDefinition HangTimedOut = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("HangTimedOut");

		public static readonly SimpleProviderPropertyDefinition StolenNM = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("StolenNM");

		public static readonly SimpleProviderPropertyDefinition SyncHangingHint = ConditionalHandlerSchema.BuildStringPropDef("SyncHangingHint");

		public static readonly SimpleProviderPropertyDefinition DeviceAccessState = ConditionalHandlerSchema.BuildValueTypePropDef<DeviceAccessState>("DeviceAccessState");

		public static readonly SimpleProviderPropertyDefinition DeviceAccessStateReason = ConditionalHandlerSchema.BuildValueTypePropDef<DeviceAccessStateReason>("DeviceAccessStateReason");

		public static readonly SimpleProviderPropertyDefinition DeviceId = ConditionalHandlerSchema.BuildStringPropDef("DeviceId");

		public static readonly SimpleProviderPropertyDefinition DeviceType = ConditionalHandlerSchema.BuildStringPropDef("DeviceType");

		public static readonly SimpleProviderPropertyDefinition ProtocolVersion = ConditionalHandlerSchema.BuildStringPropDef("ProtocolVersion");

		public static readonly SimpleProviderPropertyDefinition ProxyToServer = ConditionalHandlerSchema.BuildStringPropDef("ProxyToServer");

		public static readonly SimpleProviderPropertyDefinition ProxyFromServer = ConditionalHandlerSchema.BuildStringPropDef("ProxyFromServer");

		public static readonly SimpleProviderPropertyDefinition WasProxied = ConditionalHandlerSchema.BuildValueTypePropDef<bool>("WasProxied");

		public static readonly SimpleProviderPropertyDefinition ProxyElapsed = ConditionalHandlerSchema.BuildValueTypePropDef<TimeSpan>("ProxyElapsed");

		public static readonly SimpleProviderPropertyDefinition TimeTracker = ConditionalHandlerSchema.BuildStringPropDef("TimeTracker");

		public static readonly SimpleProviderPropertyDefinition XmlRequest = ConditionalHandlerSchema.BuildStringPropDef("XmlRequest");

		public static readonly SimpleProviderPropertyDefinition XmlResponse = ConditionalHandlerSchema.BuildStringPropDef("XmlResponse");

		public static readonly SimpleProviderPropertyDefinition ProtocolLoggerData = ConditionalHandlerSchema.BuildStringPropDef("ProtocolLoggerData");

		public static readonly SimpleProviderPropertyDefinition CompletedWithDelay = ConditionalHandlerSchema.BuildValueTypePropDef<TimeSpan>("CompletedWithDelay");

		public static readonly SimpleProviderPropertyDefinition WbXmlRequestSize = ConditionalHandlerSchema.BuildValueTypePropDef<int>("WbXmlRequestSize");

		public static readonly SimpleProviderPropertyDefinition HttpStatus = ConditionalHandlerSchema.BuildValueTypePropDef<int>("HttpStatus");

		public static readonly SimpleProviderPropertyDefinition EasStatus = ConditionalHandlerSchema.BuildValueTypePropDef<int>("EasStatus");

		public static readonly SimpleProviderPropertyDefinition ProtocolError = ConditionalHandlerSchema.BuildStringPropDef("ProtocolError");

		public static readonly SimpleProviderPropertyDefinition EasMaxDevices = ConditionalHandlerSchema.BuildUnlimitedPropDef("EasMaxDevices");

		public static readonly SimpleProviderPropertyDefinition EasMaxDeviceDeletesPerMonth = ConditionalHandlerSchema.BuildUnlimitedPropDef("EasMaxDeviceDeletesPerMonth");

		public static readonly SimpleProviderPropertyDefinition EasMaxInactivityForDeviceCleanup = ConditionalHandlerSchema.BuildUnlimitedPropDef("EasMaxInactivityForDeviceCleanup");

		public static readonly SimpleProviderPropertyDefinition Traces = ConditionalHandlerSchema.BuildStringPropDef("Traces");

		public static readonly SimpleProviderPropertyDefinition RequestHeaders = ConditionalHandlerSchema.BuildStringPropDef("RequestHeaders");

		public static readonly SimpleProviderPropertyDefinition ResponseHeaders = ConditionalHandlerSchema.BuildStringPropDef("ResponseHeaders");

		public static readonly SimpleProviderPropertyDefinition UserWLMData = ConditionalHandlerSchema.BuildRefTypePropDef<UserWorkloadManagerResult>("UserWLMData");

		public static readonly SimpleProviderPropertyDefinition IsConsumerOrganizationUser = ConditionalHandlerSchema.BuildRefTypePropDef<UserWorkloadManagerResult>("IsConsumerOrganizationUser");
	}
}
