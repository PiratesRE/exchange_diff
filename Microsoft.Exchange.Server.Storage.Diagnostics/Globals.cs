using System;
using System.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public static class Globals
	{
		internal static DiagnosticQueryHandler QueryHandler
		{
			get
			{
				if (Microsoft.Exchange.Server.Storage.Diagnostics.Globals.queryHandler == null)
				{
					DiagnosticQueryFactory factory = StoreQueryFactory.CreateFactory();
					Microsoft.Exchange.Server.Storage.Diagnostics.Globals.queryHandler = DiagnosticQueryHandler.Create(factory);
				}
				return Microsoft.Exchange.Server.Storage.Diagnostics.Globals.queryHandler;
			}
		}

		public static void Initialize()
		{
			DatabaseSchema.Initialize();
			SimpleQueryTargets.Initialize();
			Microsoft.Exchange.Server.Storage.Diagnostics.Globals.QueryHandler.Register();
			StoreQueryTargets.Register<ThreadManager.ThreadDiagnosticInfo>(ThreadManager.Instance, Visibility.Public);
			StoreQueryTargets.Register<ThreadManager.ProcessThreadInfo>(ThreadManager.Instance, Visibility.Public);
		}

		public static void Terminate()
		{
			Microsoft.Exchange.Server.Storage.Diagnostics.Globals.QueryHandler.Deregister();
		}

		public static void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			DatabaseSchema.Initialize(database);
		}

		public static void DatabaseMounting(Context context, StoreDatabase database)
		{
			DatabaseSchema.PostMountInitialize(context, database);
			SimpleQueryTargets.MountEventHandler(database);
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
		}

		[Conditional("DEBUG")]
		internal static void Assert(bool assertCondition, string message)
		{
		}

		private static DiagnosticQueryHandler queryHandler;
	}
}
