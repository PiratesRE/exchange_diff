using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class ClassificationDefinitionsDiagnosticsReporter : IClassificationDefinitionsDiagnosticsReporter
	{
		internal static IClassificationDefinitionsDiagnosticsReporter Instance
		{
			get
			{
				return ClassificationDefinitionsDiagnosticsReporter.singletonInstance;
			}
		}

		internal Trace Tracer
		{
			get
			{
				return this.dlpTracer;
			}
		}

		private ClassificationDefinitionsDiagnosticsReporter()
		{
			this.dlpTracer = ExTraceGlobals.ClassificationDefinitionsTracer;
		}

		public void WriteCorruptRulePackageDiagnosticsInformation(int traceSourceHashCode, OrganizationId currentOrganizationId, string offendingRulePackageObjectDn, Exception underlyingException)
		{
			string organizationId = currentOrganizationId.ToString();
			string exceptionDetails = underlyingException.ToString();
			Task.Factory.StartNew(delegate()
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_CorruptClassificationRuleCollection, new string[]
				{
					organizationId,
					offendingRulePackageObjectDn,
					exceptionDetails
				});
			});
			string eventMessage = string.Format("A corrupted classification rule collection has been identified under organization: '{0}'. Source object DN: {1}. Error details: {2}", organizationId, offendingRulePackageObjectDn, exceptionDetails);
			Task.Factory.StartNew(delegate()
			{
				EventNotificationItem.Publish(ClassificationDefinitionsDiagnosticsReporter.ServiceName, "ClassificationManagement", "CorruptClassificationRuleCollection", eventMessage, ResultSeverityLevel.Warning, false);
			});
			this.dlpTracer.TraceError((long)traceSourceHashCode, eventMessage);
		}

		public void WriteDuplicateRuleIdAcrossRulePacksDiagnosticsInformation(int traceSourceHashCode, OrganizationId currentOrganizationId, string offendingRulePackageObjectDn1, string offendingRulePackageObjectDn2, string duplicateRuleId)
		{
			string organizationId = currentOrganizationId.ToString();
			Task.Factory.StartNew(delegate()
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_DuplicateDataClassificationIdAcrossRulePack, new string[]
				{
					organizationId,
					duplicateRuleId,
					offendingRulePackageObjectDn1,
					offendingRulePackageObjectDn2
				});
			});
			string eventMessage = string.Format("A duplicate data classification identifier '{1}' has been identified in objects with DN '{2}' and '{3}' under organization '{0}'.", new object[]
			{
				organizationId,
				duplicateRuleId,
				offendingRulePackageObjectDn1,
				offendingRulePackageObjectDn2
			});
			Task.Factory.StartNew(delegate()
			{
				EventNotificationItem.Publish(ClassificationDefinitionsDiagnosticsReporter.ServiceName, "ClassificationManagement", "NonUniqueDataClassificationIdentifierFound", eventMessage, ResultSeverityLevel.Warning, false);
			});
			this.dlpTracer.TraceError((long)traceSourceHashCode, eventMessage);
		}

		public void WriteClassificationEngineConfigurationErrorInformation(int traceSourceHashCode, Exception underlyingException)
		{
			string exceptionDetails = underlyingException.ToString();
			Task.Factory.StartNew(delegate()
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ClassificationEngineConfigurationError, new string[]
				{
					exceptionDetails
				});
			});
			string eventMessage = string.Format("Unable to obtain information from Microsoft Classification Engine configuration for classification rule collection validation purpose. Error details: {0}", exceptionDetails);
			Task.Factory.StartNew(delegate()
			{
				EventNotificationItem.Publish(ClassificationDefinitionsDiagnosticsReporter.ServiceName, "ClassificationManagement", "MceConfigIssue", eventMessage, ResultSeverityLevel.Warning, false);
			});
			this.dlpTracer.TraceError((long)traceSourceHashCode, eventMessage);
		}

		public void WriteClassificationEngineUnexpectedFailureInValidation(int traceSourceHashCode, OrganizationId currentOrganizationId, int engineErrorCode)
		{
			string organizationId = (!object.ReferenceEquals(currentOrganizationId, null)) ? currentOrganizationId.ToString() : string.Empty;
			string hexEngineErrorCode = engineErrorCode.ToString("X8");
			Task.Factory.StartNew(delegate()
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ClassificationEngineFailure, new string[]
				{
					organizationId,
					string.Format("0x{0}", hexEngineErrorCode)
				});
			});
			string eventMessage = string.Format("MCE returned an error when trying to validate a classification rule collection for organization '{0}'. Error code returned was 0x{1}.", organizationId, hexEngineErrorCode);
			Task.Factory.StartNew(delegate()
			{
				EventNotificationItem.Publish(ClassificationDefinitionsDiagnosticsReporter.ServiceName, "ClassificationManagement", "MceFailureIssue", eventMessage, ResultSeverityLevel.Error, false);
			});
			this.dlpTracer.TraceError((long)traceSourceHashCode, eventMessage);
		}

		public void WriteClassificationEngineTimeoutInValidation(int traceSourceHashCode, OrganizationId currentOrganizationId, int validationTimeout)
		{
			string organizationId = (!object.ReferenceEquals(currentOrganizationId, null)) ? currentOrganizationId.ToString() : string.Empty;
			string timeout = validationTimeout.ToString(CultureInfo.InvariantCulture);
			Task.Factory.StartNew(delegate()
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ClassificationEngineTimeout, new string[]
				{
					organizationId,
					timeout
				});
			});
			string eventMessage = string.Format("MCE timed-out when trying to validate a classification rule collection for organization '{0}'. The operation timeout was {1} ms.", organizationId, timeout);
			Task.Factory.StartNew(delegate()
			{
				EventNotificationItem.Publish(ClassificationDefinitionsDiagnosticsReporter.ServiceName, "ClassificationManagement", "MceTimeoutIssue", eventMessage, ResultSeverityLevel.Error, false);
			});
			this.dlpTracer.TraceError((long)traceSourceHashCode, eventMessage);
		}

		public void WriteInvalidObjectInformation(int traceSourceHashCode, OrganizationId currentOrganizationId, string offendingRulePackageObjectDn)
		{
			string arg = currentOrganizationId.ToString();
			this.dlpTracer.TraceWarning<string, string>((long)traceSourceHashCode, "A classification definition management operation is attempted against object outside ClassificationDefinitions container. Organization: '{0}'. Incorrect object DN: '{1}'", arg, offendingRulePackageObjectDn);
		}

		private const string ComponentName = "ClassificationManagement";

		private static readonly ClassificationDefinitionsDiagnosticsReporter singletonInstance = new ClassificationDefinitionsDiagnosticsReporter();

		private static readonly string ServiceName = ExchangeComponent.Classification.Name;

		private readonly Trace dlpTracer;
	}
}
