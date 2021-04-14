using System;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal abstract class AdminRpc
	{
		protected AdminRpc(AdminMethod methodId, ClientSecurityContext callerSecurityContext, byte[] auxiliaryIn)
		{
			this.clientSecurityContext = callerSecurityContext;
			this.auxiliaryIn = auxiliaryIn;
			this.methodId = methodId;
		}

		protected AdminRpc(AdminMethod methodId, ClientSecurityContext callerSecurityContext, Guid? mdbGuid, byte[] auxiliaryIn) : this(methodId, callerSecurityContext, auxiliaryIn)
		{
			this.hasMdbGuidArgument = true;
			this.mdbGuid = mdbGuid;
		}

		protected AdminRpc(AdminMethod methodId, ClientSecurityContext callerSecurityContext, Guid? mdbGuid, AdminRpc.ExpectedDatabaseState expectedDatabaseState, byte[] auxiliaryIn) : this(methodId, callerSecurityContext, mdbGuid, auxiliaryIn)
		{
			this.expectedDatabaseState = expectedDatabaseState;
		}

		public Guid? MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
			set
			{
				this.mdbGuid = value;
			}
		}

		public byte[] AuxiliaryIn
		{
			get
			{
				return this.auxiliaryIn;
			}
		}

		public byte[] AuxiliaryOut
		{
			get
			{
				return this.auxiliaryOut;
			}
		}

		internal virtual int OperationDetail
		{
			get
			{
				return 0;
			}
		}

		protected StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		protected DatabaseInfo DatabaseInfo
		{
			get
			{
				return this.databaseInfo;
			}
		}

		protected ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.clientSecurityContext;
			}
		}

		protected int RequestId
		{
			get
			{
				return this.requestId;
			}
		}

		protected string ClientId
		{
			get
			{
				return this.clientId;
			}
		}

		private bool EnableDiagnosticTracing
		{
			get
			{
				return this.requestId != 0;
			}
		}

		public ErrorCode EcExecute()
		{
			AdminRpc.<>c__DisplayClass1 CS$<>8__locals1 = new AdminRpc.<>c__DisplayClass1();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.localErrorCode = ErrorCode.NoError;
			CS$<>8__locals1.exceptionThrown = true;
			CS$<>8__locals1.executionDiagnostics = new AdminExecutionDiagnostics(this.methodId, this.OperationDetail);
			WatsonOnUnhandledException.Guard(CS$<>8__locals1.executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<EcExecute>b__0)));
			if (CS$<>8__locals1.exceptionThrown)
			{
				CS$<>8__locals1.localErrorCode = ErrorCode.CreateCallFailed((LID)37528U);
			}
			return CS$<>8__locals1.localErrorCode;
		}

		internal void ParseAuxiliaryInput()
		{
			if (this.auxiliaryIn == null || this.auxiliaryIn.Length >= 1048576)
			{
				return;
			}
			AuxiliaryData auxiliaryData = AuxiliaryData.Parse(this.auxiliaryIn);
			for (int i = 0; i < auxiliaryData.Input.Count; i++)
			{
				AuxiliaryBlock auxiliaryBlock = auxiliaryData.Input[i];
				if (!(auxiliaryBlock is UnknownAuxiliaryBlock))
				{
					AuxiliaryBlockTypes type = auxiliaryBlock.Type;
					if (type != AuxiliaryBlockTypes.DiagCtxReqId)
					{
						if (type == AuxiliaryBlockTypes.DiagCtxClientId)
						{
							DiagCtxClientIdAuxiliaryBlock diagCtxClientIdAuxiliaryBlock = (DiagCtxClientIdAuxiliaryBlock)auxiliaryBlock;
							this.clientId = diagCtxClientIdAuxiliaryBlock.ClientId;
						}
					}
					else
					{
						DiagCtxReqIdAuxiliaryBlock diagCtxReqIdAuxiliaryBlock = (DiagCtxReqIdAuxiliaryBlock)auxiliaryBlock;
						this.requestId = diagCtxReqIdAuxiliaryBlock.RequestId;
					}
				}
			}
		}

		protected abstract ErrorCode EcExecuteRpc(MapiContext context);

		protected virtual ErrorCode EcInitializeResources(MapiContext context)
		{
			ErrorCode result = ErrorCode.NoError;
			if (this.hasMdbGuidArgument)
			{
				((AdminExecutionDiagnostics)context.Diagnostics).AdminExMonLogger.SetMdbGuid(this.mdbGuid.Value);
				this.database = Storage.FindDatabase(this.mdbGuid.Value);
				if (this.database == null)
				{
					result = ErrorCode.CreateMdbNotInitialized((LID)38525U);
				}
				else
				{
					try
					{
						this.databaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, this.database.MdbGuid);
					}
					catch (DatabaseNotFoundException exception)
					{
						context.OnExceptionCatch(exception);
						Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MountedStoreNotInActiveDirectory, new object[]
						{
							this.database.MdbGuid
						});
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Mounted database has no associated record in AD.");
					}
					context.Connect(this.database);
					switch (this.expectedDatabaseState)
					{
					case AdminRpc.ExpectedDatabaseState.OnlineActive:
						if (!this.database.IsOnlineActive)
						{
							this.database.TraceState((LID)46076U);
							result = ErrorCode.CreateMdbNotInitialized((LID)34901U);
						}
						break;
					case AdminRpc.ExpectedDatabaseState.OnlinePassive:
						if (!this.database.IsOnlinePassive)
						{
							this.database.TraceState((LID)62460U);
							result = ErrorCode.CreateMdbNotInitialized((LID)42576U);
						}
						break;
					case AdminRpc.ExpectedDatabaseState.AnyOnlineState:
						if (!this.database.IsOnlineActive && !this.database.IsOnlinePassive)
						{
							this.database.TraceState((LID)37884U);
							result = ErrorCode.CreateMdbNotInitialized((LID)58960U);
						}
						break;
					case AdminRpc.ExpectedDatabaseState.OnlinePassiveAttachedReadOnly:
						if (!this.database.IsOnlinePassiveAttachedReadOnly)
						{
							this.database.TraceState((LID)33788U);
							result = ErrorCode.CreateMdbNotInitialized((LID)58364U);
						}
						break;
					case AdminRpc.ExpectedDatabaseState.AnyAttachedState:
						if (!this.database.IsOnlineActive && !this.database.IsOnlinePassiveAttachedReadOnly)
						{
							this.database.TraceState((LID)41980U);
							result = ErrorCode.CreateMdbNotInitialized((LID)38140U);
						}
						break;
					}
					((AdminExecutionDiagnostics)context.Diagnostics).OnBeforeRpc(this.mdbGuid.Value, RopSummaryCollector.GetRopSummaryCollector(context));
				}
			}
			return result;
		}

		protected virtual void CleanupResources(MapiContext context)
		{
		}

		protected virtual ErrorCode EcValidateArguments(MapiContext context)
		{
			ErrorCode result = ErrorCode.NoError;
			if (this.hasMdbGuidArgument && this.mdbGuid == null)
			{
				result = ErrorCode.CreateInvalidParameter((LID)47741U);
			}
			return result;
		}

		protected virtual ErrorCode EcCheckPermissions(MapiContext context)
		{
			return AdminRpcPermissionChecks.EcDefaultCheck(context, this.DatabaseInfo);
		}

		private ErrorCode EcExecute_Unwrapped(AdminExecutionDiagnostics executionDiagnostics)
		{
			if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ExTraceGlobals.AdminRpcTracer.TraceFunction<AdminMethod>(35453, 0L, "Entering EcExecute_Unwrapped for {0}", this.methodId);
			}
			LockManager.AssertNoLocksHeld();
			bool flag = true;
			ErrorCode errorCode = ErrorCode.NoError;
			bool flag2 = false;
			this.ParseAuxiliaryInput();
			ClientType clientType;
			if (!ClientTypeHelper.TryGetClientType(this.clientId, out clientType))
			{
				clientType = ClientType.Administrator;
			}
			using (MapiContext mapiContext = MapiContext.CreateSessionless(executionDiagnostics, this.clientSecurityContext, clientType, LocaleMap.GetLcidFromCulture(CultureHelper.DefaultCultureInfo)))
			{
				using (AdminExMonLogger adminExMonLogger = new AdminExMonLogger(false, string.Empty))
				{
					try
					{
						executionDiagnostics.AdminExMonLogger = adminExMonLogger;
						executionDiagnostics.OnRpcBegin();
						adminExMonLogger.ServiceName = this.clientId;
						DiagnosticContext.Reset();
						errorCode = AdminRpcInterface.EcEnterRpcCall();
						if (errorCode != ErrorCode.NoError)
						{
							errorCode = errorCode.Propagate((LID)46717U);
						}
						else
						{
							flag2 = true;
							errorCode = this.EcValidateArguments(mapiContext);
							if (errorCode != ErrorCode.NoError)
							{
								errorCode = errorCode.Propagate((LID)63101U);
							}
							else
							{
								errorCode = this.EcInitializeResources(mapiContext);
								if (errorCode != ErrorCode.NoError)
								{
									errorCode = errorCode.Propagate((LID)34221U);
								}
								else
								{
									errorCode = this.EcCheckPermissions(mapiContext);
									if (errorCode != ErrorCode.NoError)
									{
										errorCode = errorCode.Propagate((LID)54909U);
									}
									else
									{
										errorCode = this.EcExecuteRpc(mapiContext);
										if (errorCode != ErrorCode.NoError)
										{
											errorCode = errorCode.Propagate((LID)42621U);
										}
										else if (mapiContext.IsConnected)
										{
											mapiContext.Commit();
										}
									}
								}
							}
						}
						flag = false;
					}
					catch (StoreException ex)
					{
						mapiContext.OnExceptionCatch(ex);
						ErrorHelper.TraceException(ExTraceGlobals.AdminRpcTracer, (LID)59009U, ex);
						errorCode = ErrorCode.CreateWithLid((LID)53912U, ex.Error);
						flag = false;
					}
					catch (BufferParseException exception)
					{
						mapiContext.OnExceptionCatch(exception);
						ErrorHelper.TraceException(ExTraceGlobals.AdminRpcTracer, (LID)61880U, exception);
						errorCode = ErrorCode.CreateRpcFormat((LID)37304U);
						flag = false;
					}
					catch (NonFatalDatabaseException ex2)
					{
						mapiContext.OnExceptionCatch(ex2);
						ErrorHelper.TraceException(ExTraceGlobals.AdminRpcTracer, (LID)56264U, ex2);
						errorCode = ErrorCode.CreateWithLid((LID)46280U, ex2.Error);
						flag = false;
					}
					catch (FatalDatabaseException exception2)
					{
						mapiContext.OnExceptionCatch(exception2);
						ErrorHelper.TraceException(ExTraceGlobals.AdminRpcTracer, (LID)54472U, exception2);
						errorCode = ErrorCode.CreateDatabaseError((LID)42184U);
						flag = false;
					}
					finally
					{
						try
						{
							this.CleanupResources(mapiContext);
						}
						finally
						{
							try
							{
								if (this.database != null)
								{
									if (mapiContext.IsConnected)
									{
										mapiContext.Disconnect();
									}
									this.database = null;
								}
							}
							catch (NonFatalDatabaseException exception3)
							{
								mapiContext.OnExceptionCatch(exception3);
								errorCode = ErrorCode.CreateDatabaseError((LID)63688U);
							}
							catch (FatalDatabaseException exception4)
							{
								mapiContext.OnExceptionCatch(exception4);
								errorCode = ErrorCode.CreateDatabaseError((LID)41400U);
							}
						}
						if (flag2)
						{
							AdminRpcInterface.ExitRpcCall();
						}
						mapiContext.DismountOnCriticalFailure();
						executionDiagnostics.OnRpcEnd(errorCode);
						if (!flag)
						{
							this.ProduceAuxiliaryOutput(executionDiagnostics);
						}
						bool flag3 = flag || errorCode != ErrorCode.NoError;
						if (ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.FunctionTrace) || (flag3 && ExTraceGlobals.AdminRpcTracer.IsTraceEnabled(TraceType.ErrorTrace)))
						{
							string message = string.Format("Exiting EcExecute_Unwrapped for {0}: (ec = {1:X})", this.methodId, (flag && errorCode == ErrorCode.NoError) ? ErrorCodeValue.ExceptionThrown : errorCode);
							if (flag3)
							{
								ExTraceGlobals.AdminRpcTracer.TraceError(48188, 0L, message);
							}
							else
							{
								ExTraceGlobals.AdminRpcTracer.TraceFunction(36349, 0L, message);
							}
						}
						LockManager.AssertNoLocksHeld();
					}
				}
			}
			if (errorCode != ErrorCode.NoError && !ErrorHelper.ShouldSkipBreadcrumb(1, (byte)this.methodId, errorCode, 0U))
			{
				ErrorHelper.AddBreadcrumb(BreadcrumbKind.AdminError, 1, (byte)this.methodId, (byte)clientType, (this.mdbGuid != null) ? this.mdbGuid.GetHashCode() : 0, executionDiagnostics.MailboxNumber, (int)errorCode, null);
			}
			return errorCode;
		}

		private void ProduceAuxiliaryOutput(AdminExecutionDiagnostics executionDiagnostics)
		{
			AuxiliaryData auxiliaryData = AuxiliaryData.Parse(null);
			if (this.EnableDiagnosticTracing && DiagnosticContext.HasData)
			{
				auxiliaryData.AppendOutput(new DiagCtxCtxDataAuxiliaryBlock(DiagnosticContext.PackInfo()));
			}
			auxiliaryData.AppendOutput(executionDiagnostics.CreateRpcStatisticsAuxiliaryBlock(null));
			byte[] array = new byte[auxiliaryData.CalculateSerializedOutputSize()];
			int num;
			auxiliaryData.Serialize(new ArraySegment<byte>(array), out num);
			this.auxiliaryOut = array;
		}

		private const int SizeofExchangePerfHeader = 4;

		private readonly AdminMethod methodId;

		private ClientSecurityContext clientSecurityContext;

		private byte[] auxiliaryIn;

		private byte[] auxiliaryOut;

		private bool hasMdbGuidArgument;

		private AdminRpc.ExpectedDatabaseState expectedDatabaseState = AdminRpc.ExpectedDatabaseState.OnlineActive;

		private Guid? mdbGuid;

		private StoreDatabase database;

		private DatabaseInfo databaseInfo;

		private int requestId;

		private string clientId;

		[Flags]
		internal enum ExpectedDatabaseState
		{
			OnlineActive = 1,
			OnlinePassive = 2,
			OnlinePassiveAttachedReadOnly = 4,
			AnyOnlineState = 3,
			AnyAttachedState = 5
		}
	}
}
