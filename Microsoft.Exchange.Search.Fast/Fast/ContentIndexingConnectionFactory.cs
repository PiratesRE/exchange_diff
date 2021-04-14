using System;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal static class ContentIndexingConnectionFactory
	{
		internal static IDiagnosticsSession Diagnostics
		{
			get
			{
				return ContentIndexingConnectionFactory.diagnosticsSession;
			}
		}

		internal static void InitializeIfNeeded()
		{
			if (ContentIndexingConnectionFactory.configuration != null)
			{
				return;
			}
			ContentIndexingConnectionFactory.configuration = new FlightingSearchConfig();
			ContentIndexingConnectionFactory.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("ContentIndexingConnection", ContentIndexingConnectionFactory.configuration.ServiceName, ExTraceGlobals.XSOMailboxSessionTracer, 0L);
			ContentIndexingConnectionFactory.Diagnostics.TraceDebug("ContentIndexingConnectionFactory initialized", new object[0]);
		}

		internal static void ResetForTest(SearchConfig testConfiguration)
		{
			ContentIndexingConnectionFactory.configuration = testConfiguration;
			ContentIndexingConnectionFactory.DropConnection();
			ContentIndexingConnectionFactory.nextConnectionRetry = DateTime.MinValue;
		}

		internal static void Cleanup()
		{
			lock (ContentIndexingConnectionFactory.lockObject)
			{
				ContentIndexingConnectionFactory.DropConnection();
			}
			ContentIndexingConnectionFactory.Diagnostics.TraceDebug("ContentIndexingConnectionFactory cleaned up", new object[0]);
		}

		internal static ContentIndexingConnectionFactory.ConnectionUsageFrame GetConnection(out ContentIndexingConnection connectionForUse)
		{
			connectionForUse = null;
			ContentIndexingConnectionFactory.ConnectionUsageFrame result;
			lock (ContentIndexingConnectionFactory.lockObject)
			{
				if (ContentIndexingConnectionFactory.currentConnection == null)
				{
					if (ContentIndexingConnectionFactory.nextConnectionRetry > DateTime.UtcNow)
					{
						connectionForUse = null;
						return new ContentIndexingConnectionFactory.ConnectionUsageFrame(null);
					}
					bool flag2 = false;
					ContentIndexingConnection contentIndexingConnection = null;
					try
					{
						contentIndexingConnection = new ContentIndexingConnection(ContentIndexingConnectionFactory.configuration);
						ContentIndexingConnectionFactory.Diagnostics.TraceDebug<int>("New connection created: {0}", contentIndexingConnection.GetHashCode());
						ContentIndexingConnectionFactory.currentConnection = new ReferenceCount<ContentIndexingConnection>(contentIndexingConnection);
						contentIndexingConnection = null;
						flag2 = true;
					}
					catch (PerformingFastOperationException arg)
					{
						ContentIndexingConnectionFactory.Diagnostics.TraceDebug<PerformingFastOperationException>("GetConnection called. Connection creation failed with PerformingFastOperationException: {0}", arg);
					}
					catch (FastConnectionException arg2)
					{
						ContentIndexingConnectionFactory.Diagnostics.TraceDebug<FastConnectionException>("GetConnection called. Connection creation failed with FastConnectionException: {0}", arg2);
					}
					catch (IndexSystemNotFoundException arg3)
					{
						ContentIndexingConnectionFactory.Diagnostics.TraceDebug<IndexSystemNotFoundException>("GetConnection called. Connection creation failed with IndexSystemNotFoundException: {0}", arg3);
					}
					finally
					{
						if (flag2)
						{
							ContentIndexingConnectionFactory.nextConnectionRetry = DateTime.MinValue;
						}
						else
						{
							ContentIndexingConnectionFactory.nextConnectionRetry = DateTime.UtcNow + ContentIndexingConnectionFactory.configuration.ConnectionRetryDelay;
						}
						if (contentIndexingConnection != null)
						{
							contentIndexingConnection.Dispose();
						}
					}
				}
				if (ContentIndexingConnectionFactory.currentConnection != null)
				{
					connectionForUse = ContentIndexingConnectionFactory.currentConnection.ReferencedObject;
				}
				result = new ContentIndexingConnectionFactory.ConnectionUsageFrame(ContentIndexingConnectionFactory.currentConnection);
			}
			return result;
		}

		internal static void OnConnectionLevelFailure(ContentIndexingConnection connection, bool fatalFailure)
		{
			ContentIndexingConnectionFactory.Diagnostics.Assert(connection != null, "Null connection", new object[0]);
			lock (ContentIndexingConnectionFactory.lockObject)
			{
				bool flag2 = ContentIndexingConnectionFactory.currentConnection == null || ContentIndexingConnectionFactory.currentConnection.ReferencedObject == null;
				ContentIndexingConnectionFactory.Diagnostics.TraceDebug<int, int, bool>("OnConnectionLevelFailure called. Failed connection: {0}. Current connection: {1}. Fatal failure: {2}", connection.GetHashCode(), flag2 ? 0 : ContentIndexingConnectionFactory.currentConnection.ReferencedObject.GetHashCode(), fatalFailure);
				if (!flag2 && object.ReferenceEquals(connection, ContentIndexingConnectionFactory.currentConnection.ReferencedObject))
				{
					bool flag3 = false;
					if (fatalFailure)
					{
						flag3 = true;
					}
					else
					{
						int numberOfConnectionLevelFailures = connection.NumberOfConnectionLevelFailures;
						if (numberOfConnectionLevelFailures >= ContentIndexingConnectionFactory.configuration.ConnectionLevelFailuresThreshold)
						{
							ContentIndexingConnectionFactory.Diagnostics.TraceDebug<int, int>("Connection exceeded connection level threshold and will be dropped [{0}, {1}]", numberOfConnectionLevelFailures, connection.NumberOfConnectionLevelFailures);
							flag3 = true;
						}
					}
					if (flag3)
					{
						ContentIndexingConnectionFactory.DropConnection();
						ContentIndexingConnectionFactory.nextConnectionRetry = DateTime.UtcNow + ContentIndexingConnectionFactory.configuration.ConnectionRetryDelay;
					}
				}
			}
		}

		private static void DropConnection()
		{
			if (ContentIndexingConnectionFactory.currentConnection != null)
			{
				ContentIndexingConnectionFactory.currentConnection.Release();
				ContentIndexingConnectionFactory.currentConnection = null;
			}
		}

		private static SearchConfig configuration;

		private static IDiagnosticsSession diagnosticsSession;

		private static ReferenceCount<ContentIndexingConnection> currentConnection;

		private static object lockObject = new object();

		private static DateTime nextConnectionRetry = DateTime.MinValue;

		public struct ConnectionUsageFrame : IDisposable
		{
			internal ConnectionUsageFrame(ReferenceCount<ContentIndexingConnection> connection)
			{
				this.connection = connection;
				if (connection != null)
				{
					this.connection.AddRef();
				}
			}

			public void Dispose()
			{
				if (this.connection != null)
				{
					this.connection.Release();
				}
			}

			private ReferenceCount<ContentIndexingConnection> connection;
		}
	}
}
