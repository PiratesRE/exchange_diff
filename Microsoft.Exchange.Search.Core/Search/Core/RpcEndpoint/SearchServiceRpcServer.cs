using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Search;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.RpcEndpoint
{
	internal sealed class SearchServiceRpcServer : SearchRpcServer
	{
		public static SearchServiceRpcServer Server { get; set; }

		public SearchServiceRpcServer.HandleRecordDocumentProcessing RecordDocumentProcessingHandler { get; set; }

		public SearchServiceRpcServer.HandleRecordDocumentFailure RecordDocumentFailureHandler { get; set; }

		public Action UpdateIndexSystemsHandler { get; set; }

		public Action<Guid> ResumeIndexingHandler { get; set; }

		public Action<Guid> RebuildIndexSystemHandler { get; set; }

		public override void RecordDocumentProcessing(Guid mdbGuid, Guid instance, Guid correlationId, long docId)
		{
			if (this.RecordDocumentProcessingHandler != null)
			{
				this.RecordDocumentProcessingHandler(mdbGuid, instance, correlationId, docId);
				return;
			}
			SearchServiceRpcServer.tracer.TraceError<Guid>((long)SearchServiceRpcServer.tracingContext, "No Handler Registered - Failed to track document with CorrelationId {0}", correlationId);
		}

		public override void RecordDocumentFailure(Guid mdbGuid, Guid correlationId, long docId, string errorMessage)
		{
			if (this.RecordDocumentFailureHandler != null)
			{
				SearchServiceRpcServer.tracer.TraceDebug<Guid>((long)SearchServiceRpcServer.tracingContext, "Handling RPC - RecordDocumentFailure for CorrelationId: {0}", correlationId);
				this.RecordDocumentFailureHandler(mdbGuid, correlationId, docId, errorMessage);
				return;
			}
			SearchServiceRpcServer.tracer.TraceError<Guid, long, string>((long)SearchServiceRpcServer.tracingContext, "No Handler Registered - Failed to record a permanent poison failure for document. MdbGuid: {0}, DocId: {1}, ErrorMessage {2}", mdbGuid, docId, errorMessage);
		}

		public override void UpdateIndexSystems()
		{
			if (this.UpdateIndexSystemsHandler != null)
			{
				this.UpdateIndexSystemsHandler();
				return;
			}
			SearchServiceRpcServer.tracer.TraceError((long)SearchServiceRpcServer.tracingContext, "No Handler Registered - Failed to update index systems");
		}

		public override void ResumeIndexing(Guid databaseGuid)
		{
			if (this.ResumeIndexingHandler != null)
			{
				this.ResumeIndexingHandler(databaseGuid);
				return;
			}
			SearchServiceRpcServer.tracer.TraceError<Guid>((long)SearchServiceRpcServer.tracingContext, "No Handler Registered - Failed to resume indexing on database {0}", databaseGuid);
		}

		public override void RebuildIndexSystem(Guid databaseGuid)
		{
			if (this.RebuildIndexSystemHandler != null)
			{
				this.RebuildIndexSystemHandler(databaseGuid);
				return;
			}
			SearchServiceRpcServer.tracer.TraceError<Guid>((long)SearchServiceRpcServer.tracingContext, "No Handler Registered - Failed to rebuild index system for database {0}", databaseGuid);
		}

		internal static void StartServer()
		{
			ObjectSecurity serverAdminSecurity = SearchServiceRpcServer.GetServerAdminSecurity();
			if (serverAdminSecurity != null)
			{
				SearchServiceRpcServer.Server = (SearchServiceRpcServer)RpcServerBase.RegisterServer(typeof(SearchServiceRpcServer), serverAdminSecurity, 1);
				SearchServiceRpcServer.tracingContext = SearchServiceRpcServer.Server.GetHashCode();
				SearchServiceRpcServer.tracer.TraceDebug((long)SearchServiceRpcServer.tracingContext, "Rpc Endpoint successfully Registered.");
				return;
			}
			throw new ComponentFailedPermanentException(Strings.RpcEndpointFailedToRegister);
		}

		internal static void StopServer()
		{
			RpcServerBase.StopServer(SearchRpcServer.RpcIntfHandle);
			SearchServiceRpcServer.Server = null;
			SearchServiceRpcServer.tracer.TraceDebug((long)SearchServiceRpcServer.tracingContext, "Rpc Endpoint stopped.");
		}

		private static ObjectSecurity GetServerAdminSecurity()
		{
			FileSecurity securityDescriptor = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 278, "GetServerAdminSecurity", "f:\\15.00.1497\\sources\\dev\\Search\\src\\Core\\RpcEndpoint\\SearchServiceRpcServer.cs");
				Server server = null;
				try
				{
					server = topologyConfigurationSession.FindLocalServer();
				}
				catch (LocalServerNotFoundException)
				{
					return;
				}
				catch (ADTopologyPermanentException innerException)
				{
					throw new ComponentFailedPermanentException(innerException);
				}
				RawSecurityDescriptor rawSecurityDescriptor = server.ReadSecurityDescriptor();
				if (rawSecurityDescriptor != null)
				{
					securityDescriptor = new FileSecurity();
					byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
					rawSecurityDescriptor.GetBinaryForm(array, 0);
					securityDescriptor.SetSecurityDescriptorBinaryForm(array);
					IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 309, "GetServerAdminSecurity", "f:\\15.00.1497\\sources\\dev\\Search\\src\\Core\\RpcEndpoint\\SearchServiceRpcServer.cs");
					SecurityIdentifier exchangeServersUsgSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
					FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(exchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
					securityDescriptor.SetAccessRule(fileSystemAccessRule);
					SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
					fileSystemAccessRule = new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow);
					securityDescriptor.AddAccessRule(fileSystemAccessRule);
					return;
				}
			}, 3);
			return securityDescriptor;
		}

		private static readonly Trace tracer = ExTraceGlobals.SearchRpcServerTracer;

		private static int tracingContext;

		public delegate void HandleRecordDocumentProcessing(Guid mdbGuid, Guid flowInstance, Guid correlationId, long docId);

		public delegate void HandleRecordDocumentFailure(Guid mdbGuid, Guid correlationId, long docId, string errorMessage);
	}
}
