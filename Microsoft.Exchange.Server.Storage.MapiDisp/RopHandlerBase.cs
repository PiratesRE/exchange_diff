using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal abstract class RopHandlerBase : DisposableBase, IRopHandlerWithContext, IRopHandler, IDisposable
	{
		public RopResult Abort(IServerObject serverObject, AbortResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.Abort"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult AbortSubmit(IServerObject serverObject, StoreId folderId, StoreId messageId, AbortSubmitResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.AbortSubmit"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.AbortSubmit, RopHandlerBase.AbortSubmitClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.AbortSubmit, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.AbortSubmit, mapiBase);
								ropResult = this.AbortSubmit(this.mapiContext, mapiBase, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 52U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 52U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 52U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.AbortSubmit, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult AbortSubmit(MapiContext context, MapiBase serverObject, ExchangeId folderId, ExchangeId messageId, AbortSubmitResultFactory resultFactory);

		public RopResult AddressTypes(IServerObject serverObject, AddressTypesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.AddressTypes"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.AddressTypes, RopHandlerBase.AddressTypesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.AddressTypes, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.AddressTypes, mapiBase);
								ropResult = this.AddressTypes(this.mapiContext, mapiBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 73U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 73U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 73U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.AddressTypes, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult AddressTypes(MapiContext context, MapiBase serverObject, AddressTypesResultFactory resultFactory);

		public RopResult CloneStream(IServerObject serverObject, CloneStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CloneStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Stream).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CloneStream, RopHandlerBase.CloneStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CloneStream, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CloneStream, mapiStream);
								ropResult = this.CloneStream(this.mapiContext, mapiStream, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Stream);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 59U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 59U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 59U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CloneStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CloneStream(MapiContext context, MapiStream serverObject, CloneStreamResultFactory resultFactory);

		public RopResult CollapseRow(IServerObject serverObject, StoreId categoryId, CollapseRowResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CollapseRow"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CollapseRow, RopHandlerBase.CollapseRowClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CollapseRow, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CollapseRow, mapiViewTableBase);
								ropResult = this.CollapseRow(this.mapiContext, mapiViewTableBase, RcaTypeHelpers.StoreIdToExchangeId(categoryId, logon.StoreMailbox), resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 90U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 90U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 90U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CollapseRow, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CollapseRow(MapiContext context, MapiViewTableBase serverObject, ExchangeId categoryId, CollapseRowResultFactory resultFactory);

		public RopResult CommitStream(IServerObject serverObject, CommitStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CommitStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsCommitStreamSharedMailboxOperation(mapiStream);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CommitStream, RopHandlerBase.CommitStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CommitStream, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CommitStream, mapiStream);
								ropResult = this.CommitStream(this.mapiContext, mapiStream, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 93U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 93U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 93U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CommitStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CommitStream(MapiContext context, MapiStream serverObject, CommitStreamResultFactory resultFactory);

		public RopResult CopyFolder(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, bool recurse, StoreId sourceSubFolderId, string destinationSubFolderName, CopyFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CopyFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = sourceServerObject as MapiFolder;
				MapiFolder mapiFolder2 = destinationServerObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null || mapiFolder2 == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CopyFolder, RopHandlerBase.CopyFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CopyFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CopyFolder, mapiFolder);
								ropResult = this.CopyFolder(this.mapiContext, mapiFolder, mapiFolder2, reportProgress, recurse, RcaTypeHelpers.StoreIdToExchangeId(sourceSubFolderId, logon.StoreMailbox), destinationSubFolderName, out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(sourceSubFolderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 54U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 54U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 54U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CopyFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CopyFolder(MapiContext context, MapiFolder sourceServerObject, MapiFolder destinationServerObject, bool reportProgress, bool recurse, ExchangeId sourceSubFolderId, string destinationSubFolderName, out bool outputIsPartiallyCompleted, CopyFolderResultFactory resultFactory);

		public RopResult CopyProperties(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] propertyTags, CopyPropertiesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CopyProperties"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = sourceServerObject as MapiPropBagBase;
				MapiPropBagBase mapiPropBagBase2 = destinationServerObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null || mapiPropBagBase2 == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = 4 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsCopyPropertiesSharedMailboxOperation(mapiPropBagBase);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CopyProperties, RopHandlerBase.CopyPropertiesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CopyProperties, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CopyProperties, mapiPropBagBase);
								ropResult = this.CopyProperties(this.mapiContext, mapiPropBagBase, mapiPropBagBase2, reportProgress, copyPropertiesFlags, propertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 103U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 103U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 103U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CopyProperties, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CopyProperties(MapiContext context, MapiPropBagBase sourceServerObject, MapiPropBagBase destinationServerObject, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] propertyTags, CopyPropertiesResultFactory resultFactory);

		public RopResult CopyTo(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludePropertyTags, CopyToResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CopyTo"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = sourceServerObject as MapiPropBagBase;
				MapiPropBagBase mapiPropBagBase2 = destinationServerObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null || mapiPropBagBase2 == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = 4 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsCopyToSharedMailboxOperation(mapiPropBagBase);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CopyTo, RopHandlerBase.CopyToClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CopyTo, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CopyTo, mapiPropBagBase);
								ropResult = this.CopyTo(this.mapiContext, mapiPropBagBase, mapiPropBagBase2, reportProgress, copySubObjects, copyPropertiesFlags, excludePropertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 57U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 57U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 57U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CopyTo, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CopyTo(MapiContext context, MapiPropBagBase sourceServerObject, MapiPropBagBase destinationServerObject, bool reportProgress, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludePropertyTags, CopyToResultFactory resultFactory);

		public RopResult CopyToExtended(IServerObject sourceServerObject, IServerObject destinationServerObject, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludePropertyTags, CopyToExtendedResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.CopyToExtended"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult CopyToStream(IServerObject sourceServerObject, IServerObject destinationServerObject, ulong bytesToCopy, CopyToStreamResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.CopyToStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ulong bytesRead = 0UL;
				ulong bytesWritten = 0UL;
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U, bytesRead, bytesWritten);
			}
			return result;
		}

		public RopResult CreateAttachment(IServerObject serverObject, CreateAttachmentResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CreateAttachment"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Attachment).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CreateAttachment, RopHandlerBase.CreateAttachmentClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CreateAttachment, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CreateAttachment, mapiMessage);
								ropResult = this.CreateAttachment(this.mapiContext, mapiMessage, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Attachment);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 35U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 35U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 35U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CreateAttachment, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CreateAttachment(MapiContext context, MapiMessage serverObject, CreateAttachmentResultFactory resultFactory);

		public RopResult CreateBookmark(IServerObject serverObject, CreateBookmarkResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CreateBookmark"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CreateBookmark, RopHandlerBase.CreateBookmarkClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CreateBookmark, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CreateBookmark, mapiViewTableBase);
								ropResult = this.CreateBookmark(this.mapiContext, mapiViewTableBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 27U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 27U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 27U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CreateBookmark, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CreateBookmark(MapiContext context, MapiViewTableBase serverObject, CreateBookmarkResultFactory resultFactory);

		public RopResult CreateFolder(IServerObject serverObject, FolderType folderType, CreateFolderFlags flags, string displayName, string folderComment, StoreLongTermId? longTermId, CreateFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CreateFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Folder).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CreateFolder, RopHandlerBase.CreateFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CreateFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CreateFolder, mapiFolder);
								ropResult = this.CreateFolder(this.mapiContext, mapiFolder, folderType, flags, displayName, folderComment, longTermId, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Folder);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 28U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 28U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 28U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CreateFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CreateFolder(MapiContext context, MapiFolder serverObject, FolderType folderType, CreateFolderFlags flags, string displayName, string folderComment, StoreLongTermId? longTermId, CreateFolderResultFactory resultFactory);

		public RopResult CreateMessage(IServerObject serverObject, ushort codePageId, StoreId folderId, bool createAssociated, CreateMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CreateMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Message).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CreateMessage, RopHandlerBase.CreateMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CreateMessage, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CreateMessage, mapiBase);
								ropResult = this.CreateMessage(this.mapiContext, mapiBase, codePageId, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), createAssociated, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Message);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 6U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 6U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 6U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CreateMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CreateMessage(MapiContext context, MapiBase serverObject, ushort codePageId, ExchangeId folderId, bool createAssociated, CreateMessageResultFactory resultFactory);

		public RopResult CreateMessageExtended(IServerObject serverObject, ushort codePageId, StoreId folderId, CreateMessageExtendedFlags createFlags, CreateMessageExtendedResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.CreateMessageExtended"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Message).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.CreateMessageExtended, RopHandlerBase.CreateMessageExtendedClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.CreateMessageExtended, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.CreateMessageExtended, mapiBase);
								ropResult = this.CreateMessageExtended(this.mapiContext, mapiBase, codePageId, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), createFlags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Message);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 159U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 159U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 159U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.CreateMessageExtended, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult CreateMessageExtended(MapiContext context, MapiBase serverObject, ushort codePageId, ExchangeId folderId, CreateMessageExtendedFlags createFlags, CreateMessageExtendedResultFactory resultFactory);

		public RopResult DeleteAttachment(IServerObject serverObject, uint attachmentNumber, DeleteAttachmentResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.DeleteAttachment"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 4 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.DeleteAttachment, RopHandlerBase.DeleteAttachmentClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.DeleteAttachment, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.DeleteAttachment, mapiMessage);
								ropResult = this.DeleteAttachment(this.mapiContext, mapiMessage, attachmentNumber, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 36U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 36U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 36U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.DeleteAttachment, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult DeleteAttachment(MapiContext context, MapiMessage serverObject, uint attachmentNumber, DeleteAttachmentResultFactory resultFactory);

		public RopResult DeleteFolder(IServerObject serverObject, DeleteFolderFlags deleteFolderFlags, StoreId folderId, DeleteFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.DeleteFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.DeleteFolder, RopHandlerBase.DeleteFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.DeleteFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.DeleteFolder, mapiFolder);
								ropResult = this.DeleteFolder(this.mapiContext, mapiFolder, deleteFolderFlags, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 29U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 29U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 29U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.DeleteFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult DeleteFolder(MapiContext context, MapiFolder serverObject, DeleteFolderFlags deleteFolderFlags, ExchangeId folderId, DeleteFolderResultFactory resultFactory);

		public RopResult DeleteMessages(IServerObject serverObject, bool reportProgress, bool isOkToSendNonReadNotification, StoreId[] messageIds, DeleteMessagesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.DeleteMessages"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.DeleteMessages, RopHandlerBase.DeleteMessagesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.DeleteMessages, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.DeleteMessages, mapiFolder);
								ropResult = this.DeleteMessages(this.mapiContext, mapiFolder, reportProgress, isOkToSendNonReadNotification, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 30U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 30U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 30U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.DeleteMessages, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult DeleteMessages(MapiContext context, MapiFolder serverObject, bool reportProgress, bool isOkToSendNonReadNotification, ExchangeId[] messageIds, out bool outputIsPartiallyCompleted, DeleteMessagesResultFactory resultFactory);

		public RopResult DeleteProperties(IServerObject serverObject, PropertyTag[] propertyTags, DeletePropertiesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.DeleteProperties"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsDeletePropertiesSharedMailboxOperation(mapiPropBagBase);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.DeleteProperties, RopHandlerBase.DeletePropertiesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.DeleteProperties, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.DeleteProperties, mapiPropBagBase);
								ropResult = this.DeleteProperties(this.mapiContext, mapiPropBagBase, propertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 11U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 11U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 11U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.DeleteProperties, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult DeleteProperties(MapiContext context, MapiPropBagBase serverObject, PropertyTag[] propertyTags, DeletePropertiesResultFactory resultFactory);

		public RopResult DeletePropertiesNoReplicate(IServerObject serverObject, PropertyTag[] propertyTags, DeletePropertiesNoReplicateResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.DeletePropertiesNoReplicate"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsDeletePropertiesNoReplicateSharedMailboxOperation(mapiPropBagBase);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.DeletePropertiesNoReplicate, RopHandlerBase.DeletePropertiesNoReplicateClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.DeletePropertiesNoReplicate, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.DeletePropertiesNoReplicate, mapiPropBagBase);
								ropResult = this.DeletePropertiesNoReplicate(this.mapiContext, mapiPropBagBase, propertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 122U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 122U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 122U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.DeletePropertiesNoReplicate, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult DeletePropertiesNoReplicate(MapiContext context, MapiPropBagBase serverObject, PropertyTag[] propertyTags, DeletePropertiesNoReplicateResultFactory resultFactory);

		public RopResult EchoBinary(byte[] inputParameter, EchoBinaryResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.EchoBinary"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				result = resultFactory.CreateSuccessfulResult(0, inputParameter);
			}
			return result;
		}

		public RopResult EchoInt(int inputParameter, EchoIntResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.EchoInt"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				result = resultFactory.CreateSuccessfulResult(0, inputParameter);
			}
			return result;
		}

		public RopResult EchoString(string inputParameter, EchoStringResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.EchoString"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				result = resultFactory.CreateSuccessfulResult(string.Empty, inputParameter);
			}
			return result;
		}

		public RopResult EmptyFolder(IServerObject serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, EmptyFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.EmptyFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.EmptyFolder, RopHandlerBase.EmptyFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.EmptyFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.EmptyFolder, mapiFolder);
								ropResult = this.EmptyFolder(this.mapiContext, mapiFolder, reportProgress, emptyFolderFlags, out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 88U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 88U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 88U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.EmptyFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult EmptyFolder(MapiContext context, MapiFolder serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, out bool outputIsPartiallyCompleted, EmptyFolderResultFactory resultFactory);

		public RopResult ExpandRow(IServerObject serverObject, short maxRows, StoreId categoryId, ExpandRowResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ExpandRow"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ExpandRow, RopHandlerBase.ExpandRowClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ExpandRow, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ExpandRow, mapiViewTableBase);
								ropResult = this.ExpandRow(this.mapiContext, mapiViewTableBase, maxRows, RcaTypeHelpers.StoreIdToExchangeId(categoryId, logon.StoreMailbox), resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 89U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 89U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 89U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ExpandRow, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ExpandRow(MapiContext context, MapiViewTableBase serverObject, short maxRows, ExchangeId categoryId, ExpandRowResultFactory resultFactory);

		public RopResult FastTransferDestinationCopyOperationConfigure(IServerObject serverObject, FastTransferCopyOperation copyOperation, FastTransferCopyPropertiesFlag flags, FastTransferDestinationCopyOperationConfigureResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferDestinationCopyOperationConfigure"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferDestination).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferDestinationCopyOperationConfigure, RopHandlerBase.FastTransferDestinationCopyOperationConfigureClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferDestinationCopyOperationConfigure, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferDestinationCopyOperationConfigure, mapiPropBagBase);
								ropResult = this.FastTransferDestinationCopyOperationConfigure(this.mapiContext, mapiPropBagBase, copyOperation, flags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FastTransferDestination);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 83U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 83U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 83U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferDestinationCopyOperationConfigure, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferDestinationCopyOperationConfigure(MapiContext context, MapiPropBagBase serverObject, FastTransferCopyOperation copyOperation, FastTransferCopyPropertiesFlag flags, FastTransferDestinationCopyOperationConfigureResultFactory resultFactory);

		public RopResult FastTransferDestinationPutBuffer(IServerObject serverObject, ArraySegment<byte>[] dataChunks, FastTransferDestinationPutBufferResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferDestinationPutBuffer"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				ushort progressCount = 0;
				ushort totalStepCount = 0;
				bool moveUserOperation = false;
				ushort usedBufferSize = 0;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferDestinationPutBuffer, RopHandlerBase.FastTransferDestinationPutBufferClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferDestinationPutBuffer, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferDestinationPutBuffer, mapiBase);
								ropResult = this.FastTransferDestinationPutBuffer(this.mapiContext, mapiBase, dataChunks, out progressCount, out totalStepCount, out moveUserOperation, out usedBufferSize, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 84U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 84U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 84U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferDestinationPutBuffer, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, progressCount, totalStepCount, moveUserOperation, usedBufferSize);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferDestinationPutBuffer(MapiContext context, MapiBase serverObject, ArraySegment<byte>[] dataChunks, out ushort outputProgress, out ushort outputSteps, out bool outputIsMoveUser, out ushort outputUsedBufferSize, FastTransferDestinationPutBufferResultFactory resultFactory);

		public RopResult FastTransferDestinationPutBufferExtended(IServerObject serverObject, ArraySegment<byte>[] dataChunks, FastTransferDestinationPutBufferExtendedResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferDestinationPutBufferExtended"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				uint progressCount = 0U;
				uint totalStepCount = 0U;
				bool moveUserOperation = false;
				ushort usedBufferSize = 0;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferDestinationPutBufferExtended, RopHandlerBase.FastTransferDestinationPutBufferExtendedClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferDestinationPutBufferExtended, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferDestinationPutBufferExtended, mapiBase);
								ropResult = this.FastTransferDestinationPutBufferExtended(this.mapiContext, mapiBase, dataChunks, out progressCount, out totalStepCount, out moveUserOperation, out usedBufferSize, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 157U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 157U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 157U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferDestinationPutBufferExtended, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, progressCount, totalStepCount, moveUserOperation, usedBufferSize);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferDestinationPutBufferExtended(MapiContext context, MapiBase serverObject, ArraySegment<byte>[] dataChunks, out uint outputProgress, out uint outputSteps, out bool outputIsMoveUser, out ushort outputUsedBufferSize, FastTransferDestinationPutBufferExtendedResultFactory resultFactory);

		public RopResult FastTransferGetIncrementalState(IServerObject serverObject, FastTransferGetIncrementalStateResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferGetIncrementalState"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferSource).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferGetIncrementalState, RopHandlerBase.FastTransferGetIncrementalStateClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferGetIncrementalState, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferGetIncrementalState, mapiBase);
								ropResult = this.FastTransferGetIncrementalState(this.mapiContext, mapiBase, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FastTransferSource);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 130U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 130U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 130U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferGetIncrementalState, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferGetIncrementalState(MapiContext context, MapiBase serverObject, FastTransferGetIncrementalStateResultFactory resultFactory);

		public RopResult FastTransferSourceCopyFolder(IServerObject serverObject, FastTransferCopyFolderFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferSourceCopyFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferSource).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferSourceCopyFolder, RopHandlerBase.FastTransferSourceCopyFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferSourceCopyFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferSourceCopyFolder, mapiFolder);
								ropResult = this.FastTransferSourceCopyFolder(this.mapiContext, mapiFolder, flags, sendOptions, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FastTransferSource);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 76U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 76U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 76U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferSourceCopyFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferSourceCopyFolder(MapiContext context, MapiFolder serverObject, FastTransferCopyFolderFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyFolderResultFactory resultFactory);

		public RopResult FastTransferSourceCopyMessages(IServerObject serverObject, StoreId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyMessagesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferSourceCopyMessages"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferSource).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferSourceCopyMessages, RopHandlerBase.FastTransferSourceCopyMessagesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferSourceCopyMessages, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferSourceCopyMessages, mapiFolder);
								ropResult = this.FastTransferSourceCopyMessages(this.mapiContext, mapiFolder, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), flags, sendOptions, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FastTransferSource);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 75U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 75U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 75U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferSourceCopyMessages, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferSourceCopyMessages(MapiContext context, MapiFolder serverObject, ExchangeId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyMessagesResultFactory resultFactory);

		public RopResult FastTransferSourceCopyProperties(IServerObject serverObject, byte level, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] propertyTags, FastTransferSourceCopyPropertiesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferSourceCopyProperties"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferSource).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferSourceCopyProperties, RopHandlerBase.FastTransferSourceCopyPropertiesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferSourceCopyProperties, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferSourceCopyProperties, mapiPropBagBase);
								ropResult = this.FastTransferSourceCopyProperties(this.mapiContext, mapiPropBagBase, level, flags, sendOptions, propertyTags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FastTransferSource);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 105U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 105U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 105U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferSourceCopyProperties, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferSourceCopyProperties(MapiContext context, MapiPropBagBase serverObject, byte level, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] propertyTags, FastTransferSourceCopyPropertiesResultFactory resultFactory);

		public RopResult FastTransferSourceCopyTo(IServerObject serverObject, byte level, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags, FastTransferSourceCopyToResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferSourceCopyTo"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferSource).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferSourceCopyTo, RopHandlerBase.FastTransferSourceCopyToClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferSourceCopyTo, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferSourceCopyTo, mapiPropBagBase);
								ropResult = this.FastTransferSourceCopyTo(this.mapiContext, mapiPropBagBase, level, flags, sendOptions, excludedPropertyTags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FastTransferSource);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 77U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 77U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 77U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferSourceCopyTo, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferSourceCopyTo(MapiContext context, MapiPropBagBase serverObject, byte level, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags, FastTransferSourceCopyToResultFactory resultFactory);

		public RopResult FastTransferSourceGetBuffer(IServerObject serverObject, ushort bufferSize, FastTransferSourceGetBufferResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferSourceGetBuffer"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferSourceGetBuffer, RopHandlerBase.FastTransferSourceGetBufferClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferSourceGetBuffer, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferSourceGetBuffer, mapiBase);
								ropResult = this.FastTransferSourceGetBuffer(this.mapiContext, mapiBase, bufferSize, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 78U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 78U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 78U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferSourceGetBuffer, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferSourceGetBuffer(MapiContext context, MapiBase serverObject, ushort bufferSize, FastTransferSourceGetBufferResultFactory resultFactory);

		public RopResult FastTransferSourceGetBufferExtended(IServerObject serverObject, ushort bufferSize, FastTransferSourceGetBufferExtendedResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FastTransferSourceGetBufferExtended"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FastTransferSourceGetBufferExtended, RopHandlerBase.FastTransferSourceGetBufferExtendedClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FastTransferSourceGetBufferExtended, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FastTransferSourceGetBufferExtended, mapiBase);
								ropResult = this.FastTransferSourceGetBufferExtended(this.mapiContext, mapiBase, bufferSize, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 156U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 156U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 156U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FastTransferSourceGetBufferExtended, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FastTransferSourceGetBufferExtended(MapiContext context, MapiBase serverObject, ushort bufferSize, FastTransferSourceGetBufferExtendedResultFactory resultFactory);

		public RopResult FindRow(IServerObject serverObject, FindRowFlags flags, Restriction restriction, BookmarkOrigin bookmarkOrigin, byte[] bookmark, FindRowResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FindRow"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FindRow, RopHandlerBase.FindRowClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FindRow, RopHandlerBase.FindRowClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FindRow, mapiViewTableBase);
								ropResult = this.FindRow(this.mapiContext, mapiViewTableBase, flags, restriction, bookmarkOrigin, bookmark, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 79U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 79U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 79U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FindRow, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FindRow(MapiContext context, MapiViewTableBase serverObject, FindRowFlags flags, Restriction restriction, BookmarkOrigin bookmarkOrigin, byte[] bookmark, FindRowResultFactory resultFactory);

		public RopResult FlushRecipients(IServerObject serverObject, PropertyTag[] extraPropertyTags, RecipientRow[] recipientRows, FlushRecipientsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FlushRecipients"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FlushRecipients, RopHandlerBase.FlushRecipientsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FlushRecipients, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FlushRecipients, mapiMessage);
								ropResult = this.FlushRecipients(this.mapiContext, mapiMessage, extraPropertyTags, recipientRows, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 14U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 14U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 14U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FlushRecipients, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FlushRecipients(MapiContext context, MapiMessage serverObject, PropertyTag[] extraPropertyTags, RecipientRow[] recipientRows, FlushRecipientsResultFactory resultFactory);

		public RopResult FreeBookmark(IServerObject serverObject, byte[] bookmark, FreeBookmarkResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.FreeBookmark"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.FreeBookmark, RopHandlerBase.FreeBookmarkClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.FreeBookmark, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.FreeBookmark, mapiViewTableBase);
								ropResult = this.FreeBookmark(this.mapiContext, mapiViewTableBase, bookmark, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 137U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 137U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 137U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.FreeBookmark, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult FreeBookmark(MapiContext context, MapiViewTableBase serverObject, byte[] bookmark, FreeBookmarkResultFactory resultFactory);

		public RopResult GetAllPerUserLongTermIds(IServerObject serverObject, StoreLongTermId startId, GetAllPerUserLongTermIdsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetAllPerUserLongTermIds"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 3 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetAllPerUserLongTermIds, RopHandlerBase.GetAllPerUserLongTermIdsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetAllPerUserLongTermIds, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetAllPerUserLongTermIds, mapiLogon);
								ropResult = this.GetAllPerUserLongTermIds(this.mapiContext, mapiLogon, startId, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 125U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 125U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 125U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetAllPerUserLongTermIds, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetAllPerUserLongTermIds(MapiContext context, MapiLogon serverObject, StoreLongTermId startId, GetAllPerUserLongTermIdsResultFactory resultFactory);

		public RopResult GetAttachmentTable(IServerObject serverObject, TableFlags tableFlags, GetAttachmentTableResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetAttachmentTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.AttachmentView).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetAttachmentTable, RopHandlerBase.GetAttachmentTableClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetAttachmentTable, RopHandlerBase.GetAttachmentTableClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetAttachmentTable, mapiMessage);
								ropResult = this.GetAttachmentTable(this.mapiContext, mapiMessage, tableFlags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.AttachmentView);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 33U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 33U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 33U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetAttachmentTable, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetAttachmentTable(MapiContext context, MapiMessage serverObject, TableFlags tableFlags, GetAttachmentTableResultFactory resultFactory);

		public RopResult GetCollapseState(IServerObject serverObject, StoreId rowId, uint rowInstanceNumber, GetCollapseStateResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetCollapseState"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetCollapseState, RopHandlerBase.GetCollapseStateClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetCollapseState, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetCollapseState, mapiViewTableBase);
								ropResult = this.GetCollapseState(this.mapiContext, mapiViewTableBase, RcaTypeHelpers.StoreIdToExchangeId(rowId, logon.StoreMailbox), rowInstanceNumber, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 107U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 107U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 107U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetCollapseState, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetCollapseState(MapiContext context, MapiViewTableBase serverObject, ExchangeId rowId, uint rowInstanceNumber, GetCollapseStateResultFactory resultFactory);

		public RopResult GetContentsTable(IServerObject serverObject, TableFlags tableFlags, GetContentsTableResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetContentsTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.MessageView).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetContentsTable, RopHandlerBase.GetContentsTableClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetContentsTable, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetContentsTable, mapiFolder);
								ropResult = this.GetContentsTable(this.mapiContext, mapiFolder, tableFlags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.MessageView);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 5U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 5U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 5U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetContentsTable, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetContentsTable(MapiContext context, MapiFolder serverObject, TableFlags tableFlags, GetContentsTableResultFactory resultFactory);

		public RopResult GetContentsTableExtended(IServerObject serverObject, ExtendedTableFlags extendedTableFlags, GetContentsTableExtendedResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetContentsTableExtended"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.MessageView).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetContentsTableExtended, RopHandlerBase.GetContentsTableExtendedClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetContentsTableExtended, RopHandlerBase.GetContentsTableExtendedClientTypesAllowedOnReadOnlyDatabase);
									RopHandlerBase.CheckGetContentsTableExtendedConditionsForReadOnlyDatabase(this.mapiContext, mapiFolder, extendedTableFlags);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetContentsTableExtended, mapiFolder);
								ropResult = this.GetContentsTableExtended(this.mapiContext, mapiFolder, extendedTableFlags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.MessageView);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 164U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 164U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 164U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetContentsTableExtended, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetContentsTableExtended(MapiContext context, MapiFolder serverObject, ExtendedTableFlags extendedTableFlags, GetContentsTableExtendedResultFactory resultFactory);

		public RopResult GetEffectiveRights(IServerObject serverObject, byte[] addressBookId, StoreId folderId, GetEffectiveRightsResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.GetEffectiveRights"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult GetHierarchyTable(IServerObject serverObject, TableFlags tableFlags, GetHierarchyTableResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetHierarchyTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FolderView).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetHierarchyTable, RopHandlerBase.GetHierarchyTableClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetHierarchyTable, RopHandlerBase.GetHierarchyTableClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetHierarchyTable, mapiFolder);
								ropResult = this.GetHierarchyTable(this.mapiContext, mapiFolder, tableFlags, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FolderView);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 4U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 4U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 4U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetHierarchyTable, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetHierarchyTable(MapiContext context, MapiFolder serverObject, TableFlags tableFlags, GetHierarchyTableResultFactory resultFactory);

		public RopResult GetIdsFromNames(IServerObject serverObject, GetIdsFromNamesFlags flags, NamedProperty[] namedProperties, GetIdsFromNamesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetIdsFromNames"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetIdsFromNames, RopHandlerBase.GetIdsFromNamesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetIdsFromNames, RopHandlerBase.GetIdsFromNamesClientTypesAllowedOnReadOnlyDatabase);
									RopHandlerBase.CheckGetIdsFromNamesConditionsForReadOnlyDatabase(this.mapiContext, mapiBase, flags, namedProperties);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetIdsFromNames, mapiBase);
								ropResult = this.GetIdsFromNames(this.mapiContext, mapiBase, flags, namedProperties, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 86U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 86U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 86U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetIdsFromNames, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetIdsFromNames(MapiContext context, MapiBase serverObject, GetIdsFromNamesFlags flags, NamedProperty[] namedProperties, GetIdsFromNamesResultFactory resultFactory);

		public RopResult GetLocalReplicationIds(IServerObject serverObject, uint idCount, GetLocalReplicationIdsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetLocalReplicationIds"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 3 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetLocalReplicationIds, RopHandlerBase.GetLocalReplicationIdsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetLocalReplicationIds, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetLocalReplicationIds, mapiLogon);
								ropResult = this.GetLocalReplicationIds(this.mapiContext, mapiLogon, idCount, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 127U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 127U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 127U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetLocalReplicationIds, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetLocalReplicationIds(MapiContext context, MapiLogon serverObject, uint idCount, GetLocalReplicationIdsResultFactory resultFactory);

		public RopResult GetMessageStatus(IServerObject serverObject, StoreId messageId, GetMessageStatusResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetMessageStatus"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetMessageStatus, RopHandlerBase.GetMessageStatusClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetMessageStatus, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetMessageStatus, mapiFolder);
								ropResult = this.GetMessageStatus(this.mapiContext, mapiFolder, RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 31U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 31U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 31U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetMessageStatus, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetMessageStatus(MapiContext context, MapiFolder serverObject, ExchangeId messageId, GetMessageStatusResultFactory resultFactory);

		public RopResult GetNamesFromIDs(IServerObject serverObject, PropertyId[] propertyIds, GetNamesFromIDsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetNamesFromIDs"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetNamesFromIDs, RopHandlerBase.GetNamesFromIDsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetNamesFromIDs, RopHandlerBase.GetNamesFromIDsClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetNamesFromIDs, mapiBase);
								ropResult = this.GetNamesFromIDs(this.mapiContext, mapiBase, propertyIds, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 85U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 85U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 85U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetNamesFromIDs, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetNamesFromIDs(MapiContext context, MapiBase serverObject, PropertyId[] propertyIds, GetNamesFromIDsResultFactory resultFactory);

		public RopResult GetOptionsData(IServerObject serverObject, string addressType, bool wantWin32, GetOptionsDataResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.GetOptionsData"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult GetOwningServers(IServerObject serverObject, StoreId folderId, GetOwningServersResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.GetOwningServers"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult GetPermissionsTable(IServerObject serverObject, TableFlags tableFlags, GetPermissionsTableResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.GetPermissionsTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult GetPerUserGuid(IServerObject serverObject, StoreLongTermId publicFolderLongTermId, GetPerUserGuidResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetPerUserGuid"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 3 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetPerUserGuid, RopHandlerBase.GetPerUserGuidClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetPerUserGuid, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetPerUserGuid, mapiLogon);
								ropResult = this.GetPerUserGuid(this.mapiContext, mapiLogon, publicFolderLongTermId, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 97U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 97U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 97U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetPerUserGuid, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetPerUserGuid(MapiContext context, MapiLogon serverObject, StoreLongTermId publicFolderLongTermId, GetPerUserGuidResultFactory resultFactory);

		public RopResult GetPerUserLongTermIds(IServerObject serverObject, Guid databaseGuid, GetPerUserLongTermIdsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetPerUserLongTermIds"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 3 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetPerUserLongTermIds, RopHandlerBase.GetPerUserLongTermIdsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetPerUserLongTermIds, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetPerUserLongTermIds, mapiLogon);
								ropResult = this.GetPerUserLongTermIds(this.mapiContext, mapiLogon, databaseGuid, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 96U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 96U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 96U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetPerUserLongTermIds, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetPerUserLongTermIds(MapiContext context, MapiLogon serverObject, Guid databaseGuid, GetPerUserLongTermIdsResultFactory resultFactory);

		public RopResult GetPropertiesAll(IServerObject serverObject, ushort streamLimit, GetPropertiesFlags flags, GetPropertiesAllResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetPropertiesAll"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = this.IsGetPropertiesAllSharedMailboxOperation(mapiPropBagBase);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetPropertiesAll, RopHandlerBase.GetPropertiesAllClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetPropertiesAll, RopHandlerBase.GetPropertiesAllClientTypesAllowedOnReadOnlyDatabase);
									RopHandlerBase.CheckGetPropertiesAllConditionsForReadOnlyDatabase(this.mapiContext, mapiPropBagBase, streamLimit, flags);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetPropertiesAll, mapiPropBagBase);
								ropResult = this.GetPropertiesAll(this.mapiContext, mapiPropBagBase, streamLimit, flags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 8U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 8U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 8U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetPropertiesAll, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetPropertiesAll(MapiContext context, MapiPropBagBase serverObject, ushort streamLimit, GetPropertiesFlags flags, GetPropertiesAllResultFactory resultFactory);

		public RopResult GetPropertiesSpecific(IServerObject serverObject, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags, GetPropertiesSpecificResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetPropertiesSpecific"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = this.IsGetPropertiesSpecificSharedMailboxOperation(mapiPropBagBase, streamLimit, flags, propertyTags);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetPropertiesSpecific, RopHandlerBase.GetPropertiesSpecificClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetPropertiesSpecific, RopHandlerBase.GetPropertiesSpecificClientTypesAllowedOnReadOnlyDatabase);
									RopHandlerBase.CheckGetPropertiesSpecificConditionsForReadOnlyDatabase(this.mapiContext, mapiPropBagBase, streamLimit, flags, propertyTags);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetPropertiesSpecific, mapiPropBagBase);
								ropResult = this.GetPropertiesSpecific(this.mapiContext, mapiPropBagBase, streamLimit, flags, propertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 7U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 7U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 7U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetPropertiesSpecific, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetPropertiesSpecific(MapiContext context, MapiPropBagBase serverObject, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags, GetPropertiesSpecificResultFactory resultFactory);

		public RopResult GetPropertyList(IServerObject serverObject, GetPropertyListResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetPropertyList"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetPropertyList, RopHandlerBase.GetPropertyListClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetPropertyList, RopHandlerBase.GetPropertyListClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetPropertyList, mapiPropBagBase);
								ropResult = this.GetPropertyList(this.mapiContext, mapiPropBagBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 9U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 9U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 9U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetPropertyList, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetPropertyList(MapiContext context, MapiPropBagBase serverObject, GetPropertyListResultFactory resultFactory);

		public RopResult GetReceiveFolder(IServerObject serverObject, string messageClass, GetReceiveFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetReceiveFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetReceiveFolder, RopHandlerBase.GetReceiveFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetReceiveFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetReceiveFolder, mapiLogon);
								ropResult = this.GetReceiveFolder(this.mapiContext, mapiLogon, messageClass, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 39U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 39U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 39U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetReceiveFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetReceiveFolder(MapiContext context, MapiLogon serverObject, string messageClass, GetReceiveFolderResultFactory resultFactory);

		public RopResult GetReceiveFolderTable(IServerObject serverObject, GetReceiveFolderTableResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetReceiveFolderTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetReceiveFolderTable, RopHandlerBase.GetReceiveFolderTableClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetReceiveFolderTable, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetReceiveFolderTable, mapiBase);
								ropResult = this.GetReceiveFolderTable(this.mapiContext, mapiBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 104U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 104U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 104U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetReceiveFolderTable, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetReceiveFolderTable(MapiContext context, MapiBase serverObject, GetReceiveFolderTableResultFactory resultFactory);

		public RopResult GetRulesTable(IServerObject serverObject, TableFlags tableFlags, GetRulesTableResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.GetRulesTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult GetSearchCriteria(IServerObject serverObject, GetSearchCriteriaFlags flags, GetSearchCriteriaResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetSearchCriteria"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetSearchCriteria, RopHandlerBase.GetSearchCriteriaClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetSearchCriteria, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetSearchCriteria, mapiFolder);
								ropResult = this.GetSearchCriteria(this.mapiContext, mapiFolder, flags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 49U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 49U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 49U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetSearchCriteria, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetSearchCriteria(MapiContext context, MapiFolder serverObject, GetSearchCriteriaFlags flags, GetSearchCriteriaResultFactory resultFactory);

		public RopResult GetStatus(IServerObject serverObject, GetStatusResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.GetStatus"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult GetStoreState(IServerObject serverObject, GetStoreStateResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.GetStoreState"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult GetStreamSize(IServerObject serverObject, GetStreamSizeResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.GetStreamSize"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.GetStreamSize, RopHandlerBase.GetStreamSizeClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.GetStreamSize, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.GetStreamSize, mapiStream);
								ropResult = this.GetStreamSize(this.mapiContext, mapiStream, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 94U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 94U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 94U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.GetStreamSize, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult GetStreamSize(MapiContext context, MapiStream serverObject, GetStreamSizeResultFactory resultFactory);

		public RopResult HardDeleteMessages(IServerObject serverObject, bool reportProgress, bool isOkToSendNonReadNotification, StoreId[] messageIds, HardDeleteMessagesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.HardDeleteMessages"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.HardDeleteMessages, RopHandlerBase.HardDeleteMessagesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.HardDeleteMessages, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.HardDeleteMessages, mapiFolder);
								ropResult = this.HardDeleteMessages(this.mapiContext, mapiFolder, reportProgress, isOkToSendNonReadNotification, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 145U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 145U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 145U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.HardDeleteMessages, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult HardDeleteMessages(MapiContext context, MapiFolder serverObject, bool reportProgress, bool isOkToSendNonReadNotification, ExchangeId[] messageIds, out bool outputIsPartiallyCompleted, HardDeleteMessagesResultFactory resultFactory);

		public RopResult HardEmptyFolder(IServerObject serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, HardEmptyFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.HardEmptyFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.HardEmptyFolder, RopHandlerBase.HardEmptyFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.HardEmptyFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.HardEmptyFolder, mapiFolder);
								ropResult = this.HardEmptyFolder(this.mapiContext, mapiFolder, reportProgress, emptyFolderFlags, out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 146U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 146U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 146U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.HardEmptyFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult HardEmptyFolder(MapiContext context, MapiFolder serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, out bool outputIsPartiallyCompleted, HardEmptyFolderResultFactory resultFactory);

		public RopResult IdFromLongTermId(IServerObject serverObject, StoreLongTermId longTermId, IdFromLongTermIdResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.IdFromLongTermId"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.IdFromLongTermId, RopHandlerBase.IdFromLongTermIdClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.IdFromLongTermId, RopHandlerBase.IdFromLongTermIdClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.IdFromLongTermId, mapiBase);
								ropResult = this.IdFromLongTermId(this.mapiContext, mapiBase, longTermId, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 68U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 68U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 68U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.IdFromLongTermId, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult IdFromLongTermId(MapiContext context, MapiBase serverObject, StoreLongTermId longTermId, IdFromLongTermIdResultFactory resultFactory);

		public RopResult ImportDelete(IServerObject serverObject, ImportDeleteFlags importDeleteFlags, PropertyValue[] deleteChanges, ImportDeleteResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ImportDelete"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				IcsUploadContext icsUploadContext = serverObject as IcsUploadContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (icsUploadContext == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = icsUploadContext;
					MapiLogon logon = icsUploadContext.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ImportDelete, RopHandlerBase.ImportDeleteClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ImportDelete, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ImportDelete, icsUploadContext);
								ropResult = this.ImportDelete(this.mapiContext, icsUploadContext, importDeleteFlags, deleteChanges, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 116U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 116U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 116U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ImportDelete, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ImportDelete(MapiContext context, IcsUploadContext serverObject, ImportDeleteFlags importDeleteFlags, PropertyValue[] deleteChanges, ImportDeleteResultFactory resultFactory);

		public RopResult ImportHierarchyChange(IServerObject serverObject, PropertyValue[] hierarchyPropertyValues, PropertyValue[] folderPropertyValues, ImportHierarchyChangeResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ImportHierarchyChange"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				IcsHierarchyUploadContext icsHierarchyUploadContext = serverObject as IcsHierarchyUploadContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (icsHierarchyUploadContext == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = icsHierarchyUploadContext;
					MapiLogon logon = icsHierarchyUploadContext.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ImportHierarchyChange, RopHandlerBase.ImportHierarchyChangeClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ImportHierarchyChange, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ImportHierarchyChange, icsHierarchyUploadContext);
								ropResult = this.ImportHierarchyChange(this.mapiContext, icsHierarchyUploadContext, hierarchyPropertyValues, folderPropertyValues, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 115U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 115U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 115U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ImportHierarchyChange, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ImportHierarchyChange(MapiContext context, IcsHierarchyUploadContext serverObject, PropertyValue[] hierarchyPropertyValues, PropertyValue[] folderPropertyValues, ImportHierarchyChangeResultFactory resultFactory);

		public RopResult ImportMessageChange(IServerObject serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangeResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ImportMessageChange"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				IcsContentUploadContext icsContentUploadContext = serverObject as IcsContentUploadContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (icsContentUploadContext == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = icsContentUploadContext;
					MapiLogon logon = icsContentUploadContext.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Message).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ImportMessageChange, RopHandlerBase.ImportMessageChangeClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ImportMessageChange, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ImportMessageChange, icsContentUploadContext);
								ropResult = this.ImportMessageChange(this.mapiContext, icsContentUploadContext, importMessageChangeFlags, propertyValues, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Message);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 114U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 114U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 114U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ImportMessageChange, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ImportMessageChange(MapiContext context, IcsContentUploadContext serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangeResultFactory resultFactory);

		public RopResult ImportMessageChangePartial(IServerObject serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangePartialResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ImportMessageChangePartial"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				IcsContentUploadContext icsContentUploadContext = serverObject as IcsContentUploadContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (icsContentUploadContext == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = icsContentUploadContext;
					MapiLogon logon = icsContentUploadContext.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Message).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ImportMessageChangePartial, RopHandlerBase.ImportMessageChangePartialClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ImportMessageChangePartial, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ImportMessageChangePartial, icsContentUploadContext);
								ropResult = this.ImportMessageChangePartial(this.mapiContext, icsContentUploadContext, importMessageChangeFlags, propertyValues, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Message);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 153U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 153U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 153U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ImportMessageChangePartial, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ImportMessageChangePartial(MapiContext context, IcsContentUploadContext serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangePartialResultFactory resultFactory);

		public RopResult ImportMessageMove(IServerObject serverObject, byte[] sourceFolder, byte[] sourceMessage, byte[] predecessorChangeList, byte[] destinationMessage, byte[] destinationChangeNumber, ImportMessageMoveResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ImportMessageMove"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				IcsContentUploadContext icsContentUploadContext = serverObject as IcsContentUploadContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (icsContentUploadContext == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = icsContentUploadContext;
					MapiLogon logon = icsContentUploadContext.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ImportMessageMove, RopHandlerBase.ImportMessageMoveClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ImportMessageMove, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ImportMessageMove, icsContentUploadContext);
								ropResult = this.ImportMessageMove(this.mapiContext, icsContentUploadContext, sourceFolder, sourceMessage, predecessorChangeList, destinationMessage, destinationChangeNumber, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 120U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 120U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 120U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ImportMessageMove, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ImportMessageMove(MapiContext context, IcsContentUploadContext serverObject, byte[] sourceFolder, byte[] sourceMessage, byte[] predecessorChangeList, byte[] destinationMessage, byte[] destinationChangeNumber, ImportMessageMoveResultFactory resultFactory);

		public RopResult ImportReads(IServerObject serverObject, MessageReadState[] messageReadStates, ImportReadsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ImportReads"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				IcsContentUploadContext icsContentUploadContext = serverObject as IcsContentUploadContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (icsContentUploadContext == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = icsContentUploadContext;
					MapiLogon logon = icsContentUploadContext.Logon;
					bool flag = 5 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsImportReadsSharedMailboxOperation(icsContentUploadContext);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ImportReads, RopHandlerBase.ImportReadsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ImportReads, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ImportReads, icsContentUploadContext);
								ropResult = this.ImportReads(this.mapiContext, icsContentUploadContext, messageReadStates, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 128U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 128U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 128U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ImportReads, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ImportReads(MapiContext context, IcsContentUploadContext serverObject, MessageReadState[] messageReadStates, ImportReadsResultFactory resultFactory);

		public RopResult IncrementalConfig(IServerObject serverObject, IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, StoreId[] messageIds, IncrementalConfigResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.IncrementalConfig"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferSource).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.IncrementalConfig, RopHandlerBase.IncrementalConfigClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.IncrementalConfig, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.IncrementalConfig, mapiFolder);
								ropResult = this.IncrementalConfig(this.mapiContext, mapiFolder, configOptions, sendOptions, syncFlags, restriction, extraFlags, propertyTags, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.FastTransferSource);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 112U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 112U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 112U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.IncrementalConfig, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult IncrementalConfig(MapiContext context, MapiFolder serverObject, IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, ExchangeId[] messageIds, IncrementalConfigResultFactory resultFactory);

		public RopResult LockRegionStream(IServerObject serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, LockRegionStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.LockRegionStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.LockRegionStream, RopHandlerBase.LockRegionStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.LockRegionStream, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.LockRegionStream, mapiStream);
								ropResult = this.LockRegionStream(this.mapiContext, mapiStream, offset, regionLength, lockType, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 91U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 91U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 91U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.LockRegionStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult LockRegionStream(MapiContext context, MapiStream serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, LockRegionStreamResultFactory resultFactory);

		public RopResult Logon(LogonFlags logonFlags, OpenFlags openFlags, StoreState storeState, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, LocaleInfo? localeInfo, string applicationId, AuthenticationContext authenticationContext, byte[] tenantHint, LogonResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.Logon"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				ErrorCode errorCode = (ErrorCode)2147746065U;
				RopResult ropResult = null;
				try
				{
					if (mailboxId != null)
					{
						methodFrame.CurrentThreadInfo.MailboxGuid = mailboxId.Value.MailboxGuid;
					}
					ropResult = this.Logon(this.mapiContext, logonFlags, openFlags, storeState, extendedFlags, mailboxId, localeInfo, applicationId, authenticationContext, tenantHint, resultFactory);
				}
				catch (StoreException ex)
				{
					this.mapiContext.OnExceptionCatch(ex);
					ropResult = null;
					DiagnosticContext.TraceDword((LID)56872U, 254U);
					ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)42712U, ex);
					if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
					{
						throw;
					}
					errorCode = (ErrorCode)ex.Error;
				}
				catch (RopExecutionException ex2)
				{
					this.mapiContext.OnExceptionCatch(ex2);
					ropResult = null;
					DiagnosticContext.TraceDword((LID)44584U, 254U);
					ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)59096U, ex2);
					if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
					{
						throw;
					}
					errorCode = ex2.ErrorCode;
				}
				catch (BufferParseException exception)
				{
					this.mapiContext.OnExceptionCatch(exception);
					ropResult = null;
					DiagnosticContext.TraceDword((LID)60968U, 254U);
					ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)34520U, exception);
					if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
					{
						throw;
					}
					errorCode = ErrorCode.RpcFormat;
				}
				this.AssertSessionIsNotTerminating(RopId.Logon, errorCode, ropResult);
				if (ropResult == null)
				{
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider((MapiExecutionDiagnostics)this.MapiContext.Diagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult Logon(MapiContext context, LogonFlags logonFlags, OpenFlags openFlags, StoreState storeState, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, LocaleInfo? localeInfo, string applicationId, AuthenticationContext authenticationContext, byte[] tenantHint, LogonResultFactory resultFactory);

		public RopResult LongTermIdFromId(IServerObject serverObject, StoreId storeId, LongTermIdFromIdResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.LongTermIdFromId"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.LongTermIdFromId, RopHandlerBase.LongTermIdFromIdClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.LongTermIdFromId, RopHandlerBase.LongTermIdFromIdClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.LongTermIdFromId, mapiBase);
								ropResult = this.LongTermIdFromId(this.mapiContext, mapiBase, RcaTypeHelpers.StoreIdToExchangeId(storeId, logon.StoreMailbox), resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 67U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 67U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 67U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.LongTermIdFromId, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult LongTermIdFromId(MapiContext context, MapiBase serverObject, ExchangeId storeId, LongTermIdFromIdResultFactory resultFactory);

		public RopResult ModifyPermissions(IServerObject serverObject, ModifyPermissionsFlags modifyPermissionsFlags, ModifyTableRow[] permissions, ModifyPermissionsResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.ModifyPermissions"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult ModifyRules(IServerObject serverObject, ModifyRulesFlags modifyRulesFlags, ModifyTableRow[] rulesData, ModifyRulesResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.ModifyRules"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult MoveCopyMessages(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, MoveCopyMessagesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.MoveCopyMessages"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = sourceServerObject as MapiFolder;
				MapiFolder mapiFolder2 = destinationServerObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null || mapiFolder2 == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.MoveCopyMessages, RopHandlerBase.MoveCopyMessagesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.MoveCopyMessages, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.MoveCopyMessages, mapiFolder);
								ropResult = this.MoveCopyMessages(this.mapiContext, mapiFolder, mapiFolder2, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), reportProgress, isCopy, out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 51U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 51U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 51U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.MoveCopyMessages, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult MoveCopyMessages(MapiContext context, MapiFolder sourceServerObject, MapiFolder destinationServerObject, ExchangeId[] messageIds, bool reportProgress, bool isCopy, out bool outputIsPartiallyCompleted, MoveCopyMessagesResultFactory resultFactory);

		public RopResult MoveCopyMessagesExtended(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, MoveCopyMessagesExtendedResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.MoveCopyMessagesExtended"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = sourceServerObject as MapiFolder;
				MapiFolder mapiFolder2 = destinationServerObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null || mapiFolder2 == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.MoveCopyMessagesExtended, RopHandlerBase.MoveCopyMessagesExtendedClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.MoveCopyMessagesExtended, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.MoveCopyMessagesExtended, mapiFolder);
								ropResult = this.MoveCopyMessagesExtended(this.mapiContext, mapiFolder, mapiFolder2, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), reportProgress, isCopy, propertyValues, out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 155U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 155U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 155U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.MoveCopyMessagesExtended, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult MoveCopyMessagesExtended(MapiContext context, MapiFolder sourceServerObject, MapiFolder destinationServerObject, ExchangeId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, out bool outputIsPartiallyCompleted, MoveCopyMessagesExtendedResultFactory resultFactory);

		public RopResult MoveCopyMessagesExtendedWithEntryIds(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, MoveCopyMessagesExtendedWithEntryIdsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.MoveCopyMessagesExtendedWithEntryIds"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = sourceServerObject as MapiFolder;
				MapiFolder mapiFolder2 = destinationServerObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null || mapiFolder2 == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.MoveCopyMessagesExtendedWithEntryIds, RopHandlerBase.MoveCopyMessagesExtendedWithEntryIdsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.MoveCopyMessagesExtendedWithEntryIds, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.MoveCopyMessagesExtendedWithEntryIds, mapiFolder);
								ropResult = this.MoveCopyMessagesExtendedWithEntryIds(this.mapiContext, mapiFolder, mapiFolder2, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), reportProgress, isCopy, propertyValues, out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 160U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 160U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 160U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.MoveCopyMessagesExtendedWithEntryIds, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult MoveCopyMessagesExtendedWithEntryIds(MapiContext context, MapiFolder sourceServerObject, MapiFolder destinationServerObject, ExchangeId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, out bool outputIsPartiallyCompleted, MoveCopyMessagesExtendedWithEntryIdsResultFactory resultFactory);

		public RopResult MoveFolder(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, StoreId sourceSubFolderId, string destinationSubFolderName, MoveFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.MoveFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (sourceServerObject == null)
				{
					throw new ArgumentNullException("sourceServerObject");
				}
				if (destinationServerObject == null)
				{
					throw new ArgumentNullException("destinationServerObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = sourceServerObject as MapiFolder;
				MapiFolder mapiFolder2 = destinationServerObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null || mapiFolder2 == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.MoveFolder, RopHandlerBase.MoveFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.MoveFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.MoveFolder, mapiFolder);
								ropResult = this.MoveFolder(this.mapiContext, mapiFolder, mapiFolder2, reportProgress, RcaTypeHelpers.StoreIdToExchangeId(sourceSubFolderId, logon.StoreMailbox), destinationSubFolderName, out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(sourceSubFolderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 53U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 53U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 53U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.MoveFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult MoveFolder(MapiContext context, MapiFolder sourceServerObject, MapiFolder destinationServerObject, bool reportProgress, ExchangeId sourceSubFolderId, string destinationSubFolderName, out bool outputIsPartiallyCompleted, MoveFolderResultFactory resultFactory);

		public RopResult OpenAttachment(IServerObject serverObject, OpenMode openMode, uint attachmentNumber, OpenAttachmentResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.OpenAttachment"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Attachment).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.OpenAttachment, RopHandlerBase.OpenAttachmentClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.OpenAttachment, RopHandlerBase.OpenAttachmentClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.OpenAttachment, mapiMessage);
								ropResult = this.OpenAttachment(this.mapiContext, mapiMessage, openMode, attachmentNumber, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Attachment);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 34U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 34U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 34U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.OpenAttachment, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult OpenAttachment(MapiContext context, MapiMessage serverObject, OpenMode openMode, uint attachmentNumber, OpenAttachmentResultFactory resultFactory);

		public RopResult OpenCollector(IServerObject serverObject, bool wantMessageCollector, OpenCollectorResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.OpenCollector"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.UntrackedObject).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.OpenCollector, RopHandlerBase.OpenCollectorClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.OpenCollector, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.OpenCollector, mapiFolder);
								ropResult = this.OpenCollector(this.mapiContext, mapiFolder, wantMessageCollector, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.UntrackedObject);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 126U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 126U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 126U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.OpenCollector, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult OpenCollector(MapiContext context, MapiFolder serverObject, bool wantMessageCollector, OpenCollectorResultFactory resultFactory);

		public RopResult OpenEmbeddedMessage(IServerObject serverObject, ushort codePageId, OpenMode openMode, OpenEmbeddedMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.OpenEmbeddedMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiAttachment mapiAttachment = serverObject as MapiAttachment;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiAttachment == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiAttachment;
					MapiLogon logon = mapiAttachment.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Message).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.OpenEmbeddedMessage, RopHandlerBase.OpenEmbeddedMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.OpenEmbeddedMessage, RopHandlerBase.OpenEmbeddedMessageClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.OpenEmbeddedMessage, mapiAttachment);
								ropResult = this.OpenEmbeddedMessage(this.mapiContext, mapiAttachment, codePageId, openMode, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Message);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 70U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 70U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 70U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.OpenEmbeddedMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult OpenEmbeddedMessage(MapiContext context, MapiAttachment serverObject, ushort codePageId, OpenMode openMode, OpenEmbeddedMessageResultFactory resultFactory);

		public RopResult OpenFolder(IServerObject serverObject, StoreId folderId, OpenMode openMode, OpenFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.OpenFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Folder).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.OpenFolder, RopHandlerBase.OpenFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.OpenFolder, RopHandlerBase.OpenFolderClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.OpenFolder, mapiBase);
								ropResult = this.OpenFolder(this.mapiContext, mapiBase, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), openMode, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Folder);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 2U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 2U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 2U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.OpenFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult OpenFolder(MapiContext context, MapiBase serverObject, ExchangeId folderId, OpenMode openMode, OpenFolderResultFactory resultFactory);

		public RopResult OpenMessage(IServerObject serverObject, ushort codePageId, StoreId folderId, OpenMode openMode, StoreId messageId, OpenMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.OpenMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Message).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.OpenMessage, RopHandlerBase.OpenMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.OpenMessage, RopHandlerBase.OpenMessageClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.OpenMessage, mapiBase);
								ropResult = this.OpenMessage(this.mapiContext, mapiBase, codePageId, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), openMode, RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Message);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 3U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 3U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 3U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.OpenMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult OpenMessage(MapiContext context, MapiBase serverObject, ushort codePageId, ExchangeId folderId, OpenMode openMode, ExchangeId messageId, OpenMessageResultFactory resultFactory);

		public RopResult OpenStream(IServerObject serverObject, PropertyTag propertyTag, OpenMode openMode, OpenStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.OpenStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Stream).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.OpenStream, RopHandlerBase.OpenStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.OpenStream, RopHandlerBase.OpenStreamClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.OpenStream, mapiPropBagBase);
								ropResult = this.OpenStream(this.mapiContext, mapiPropBagBase, propertyTag, openMode, resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Stream);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 43U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 43U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 43U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.OpenStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult OpenStream(MapiContext context, MapiPropBagBase serverObject, PropertyTag propertyTag, OpenMode openMode, OpenStreamResultFactory resultFactory);

		public RopResult PrereadMessages(IServerObject serverObject, StoreIdPair[] messages, PrereadMessagesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.PrereadMessages"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.PrereadMessages, RopHandlerBase.PrereadMessagesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.PrereadMessages, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.PrereadMessages, mapiLogon);
								ropResult = this.PrereadMessages(this.mapiContext, mapiLogon, messages, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 162U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 162U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 162U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.PrereadMessages, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult PrereadMessages(MapiContext context, MapiLogon serverObject, StoreIdPair[] messages, PrereadMessagesResultFactory resultFactory);

		public RopResult Progress(IServerObject serverObject, bool wantCancel, ProgressResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.Progress"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult PublicFolderIsGhosted(IServerObject serverObject, StoreId folderId, PublicFolderIsGhostedResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.PublicFolderIsGhosted"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult QueryColumnsAll(IServerObject serverObject, QueryColumnsAllResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.QueryColumnsAll"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.QueryColumnsAll, RopHandlerBase.QueryColumnsAllClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.QueryColumnsAll, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.QueryColumnsAll, mapiViewTableBase);
								ropResult = this.QueryColumnsAll(this.mapiContext, mapiViewTableBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 55U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 55U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 55U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.QueryColumnsAll, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult QueryColumnsAll(MapiContext context, MapiViewTableBase serverObject, QueryColumnsAllResultFactory resultFactory);

		public RopResult QueryNamedProperties(IServerObject serverObject, QueryNamedPropertyFlags queryFlags, Guid? propertyGuid, QueryNamedPropertiesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.QueryNamedProperties"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = 3 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.QueryNamedProperties, RopHandlerBase.QueryNamedPropertiesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.QueryNamedProperties, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.QueryNamedProperties, mapiPropBagBase);
								ropResult = this.QueryNamedProperties(this.mapiContext, mapiPropBagBase, queryFlags, propertyGuid, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 95U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 95U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 95U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.QueryNamedProperties, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult QueryNamedProperties(MapiContext context, MapiPropBagBase serverObject, QueryNamedPropertyFlags queryFlags, Guid? propertyGuid, QueryNamedPropertiesResultFactory resultFactory);

		public RopResult QueryPosition(IServerObject serverObject, QueryPositionResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.QueryPosition"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.QueryPosition, RopHandlerBase.QueryPositionClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.QueryPosition, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.QueryPosition, mapiViewTableBase);
								ropResult = this.QueryPosition(this.mapiContext, mapiViewTableBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 23U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 23U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 23U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.QueryPosition, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult QueryPosition(MapiContext context, MapiViewTableBase serverObject, QueryPositionResultFactory resultFactory);

		public RopResult QueryRows(IServerObject serverObject, QueryRowsFlags flags, bool useForwardDirection, ushort rowCount, QueryRowsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.QueryRows"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.QueryRows, RopHandlerBase.QueryRowsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.QueryRows, RopHandlerBase.QueryRowsClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.QueryRows, mapiViewTableBase);
								ropResult = this.QueryRows(this.mapiContext, mapiViewTableBase, flags, useForwardDirection, rowCount, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 21U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 21U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 21U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.QueryRows, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult QueryRows(MapiContext context, MapiViewTableBase serverObject, QueryRowsFlags flags, bool useForwardDirection, ushort rowCount, QueryRowsResultFactory resultFactory);

		public RopResult ReadPerUserInformation(IServerObject serverObject, StoreLongTermId longTermId, bool wantIfChanged, uint dataOffset, ushort maxDataSize, ReadPerUserInformationResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ReadPerUserInformation"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 5 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ReadPerUserInformation, RopHandlerBase.ReadPerUserInformationClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ReadPerUserInformation, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ReadPerUserInformation, mapiLogon);
								ropResult = this.ReadPerUserInformation(this.mapiContext, mapiLogon, longTermId, wantIfChanged, dataOffset, maxDataSize, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 99U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 99U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 99U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ReadPerUserInformation, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ReadPerUserInformation(MapiContext context, MapiLogon serverObject, StoreLongTermId longTermId, bool wantIfChanged, uint dataOffset, ushort maxDataSize, ReadPerUserInformationResultFactory resultFactory);

		public RopResult ReadRecipients(IServerObject serverObject, uint recipientRowId, PropertyTag[] extraUnicodePropertyTags, ReadRecipientsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ReadRecipients"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ReadRecipients, RopHandlerBase.ReadRecipientsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ReadRecipients, RopHandlerBase.ReadRecipientsClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ReadRecipients, mapiMessage);
								ropResult = this.ReadRecipients(this.mapiContext, mapiMessage, recipientRowId, extraUnicodePropertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 15U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 15U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 15U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ReadRecipients, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ReadRecipients(MapiContext context, MapiMessage serverObject, uint recipientRowId, PropertyTag[] extraUnicodePropertyTags, ReadRecipientsResultFactory resultFactory);

		public RopResult ReadStream(IServerObject serverObject, ushort byteCount, ReadStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ReadStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ReadStream, RopHandlerBase.ReadStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ReadStream, RopHandlerBase.ReadStreamClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ReadStream, mapiStream);
								ropResult = this.ReadStream(this.mapiContext, mapiStream, byteCount, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 44U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 44U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 44U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ReadStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ReadStream(MapiContext context, MapiStream serverObject, ushort byteCount, ReadStreamResultFactory resultFactory);

		public RopResult RegisterNotification(IServerObject serverObject, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId, RegisterNotificationResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.RegisterNotification"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.Session.GetPerSessionObjectCounter(MapiObjectTrackedType.Notify).CheckObjectQuota(true);
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.RegisterNotification, RopHandlerBase.RegisterNotificationClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.RegisterNotification, RopHandlerBase.RegisterNotificationClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.RegisterNotification, mapiLogon);
								ropResult = this.RegisterNotification(this.mapiContext, mapiLogon, flags, eventFlags, wantGlobalScope, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), resultFactory);
								if (ropResult.ReturnObject != null)
								{
									IMapiObject mapiObject = (IMapiObject)ropResult.ReturnObject;
									mapiObject.IncrementObjectCounter(MapiObjectTrackingScope.Session, MapiObjectTrackedType.Notify);
									this.mapiContext.RegisterStateAction(null, delegate(Context ctx)
									{
										mapiObject.Dispose();
									});
								}
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 41U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 41U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 41U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.RegisterNotification, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult RegisterNotification(MapiContext context, MapiLogon serverObject, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, ExchangeId folderId, ExchangeId messageId, RegisterNotificationResultFactory resultFactory);

		public RopResult RegisterSynchronizationNotifications(IServerObject serverObject, StoreId[] folderIds, uint[] changeNumbers, RegisterSynchronizationNotificationsResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.RegisterSynchronizationNotifications"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public void Release(IServerObject serverObject)
		{
			using (Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi.ExTraceGlobals.FaultInjectionTracer.DisableAllTraces())
			{
				using (Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.FaultInjectionTracer.DisableAllTraces())
				{
					using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.Release"))
					{
						if (serverObject == null)
						{
							throw new ArgumentNullException("serverObject");
						}
						MapiBase mapiBase = serverObject as MapiBase;
						if (mapiBase != null && !mapiBase.IsDisposed)
						{
							mapiBase.DecrementObjectCounter(MapiObjectTrackingScope.All);
							MapiLogon logon = mapiBase.Logon;
							if (logon.IsValid)
							{
								bool flag = this.IsReleaseSharedMailboxOperation(mapiBase);
								bool flag2 = logon.IsDeferedReleaseSharedOperation();
								this.mapiContext.Initialize(logon, flag && flag2, true);
								methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
								MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
								bool flag3 = false;
								try
								{
									mapiExecutionDiagnostics.ClearExceptionHistory();
									ErrorCode errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, true, true, true);
									if (errorCode == ErrorCode.None)
									{
										flag3 = true;
										bool commit = false;
										try
										{
											mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
											logon.ProcessDeferedReleaseROPs(this.mapiContext);
											this.Release(this.mapiContext, mapiBase);
											if (this.mapiContext.IsMailboxOperationStarted)
											{
												commit = (!this.mapiContext.LockedMailboxState.Quarantined || ((this.mapiContext.ClientType == ClientType.Migration || this.mapiContext.ClientType == ClientType.PublicFolderSystem) && MailboxQuarantineProvider.Instance.IsMigrationAccessAllowed(this.mapiContext.LockedMailboxState.DatabaseGuid, this.mapiContext.LockedMailboxState.MailboxGuid)));
											}
											goto IL_1CB;
										}
										finally
										{
											try
											{
												mapiBase.Dispose();
												if (mapiBase is MapiLogon)
												{
													this.mapiContext.SetMapiLogon(null);
												}
											}
											finally
											{
												if (this.mapiContext.IsMailboxOperationStarted)
												{
													this.mapiContext.EndMailboxOperation(commit, true);
												}
											}
										}
										goto IL_1B9;
										IL_1CB:
										goto IL_343;
									}
									IL_1B9:
									throw new StoreException((LID)42272U, (ErrorCodeValue)errorCode);
								}
								catch (StoreException exception)
								{
									this.mapiContext.OnExceptionCatch(exception);
									DiagnosticContext.TraceDword((LID)36392U, 1U);
									ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)50904U, exception);
									if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
									{
										throw;
									}
									if (mapiBase is MapiLogon)
									{
										using (this.mapiContext.CriticalBlock((LID)43852U, CriticalBlockScope.MailboxSession))
										{
											throw;
										}
									}
									if (!flag3)
									{
										logon.DeferReleaseROP(mapiBase);
									}
								}
								catch (RopExecutionException exception2)
								{
									this.mapiContext.OnExceptionCatch(exception2);
									DiagnosticContext.TraceDword((LID)52776U, 1U);
									ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)47832U, exception2);
									if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
									{
										throw;
									}
									if (mapiBase is MapiLogon)
									{
										using (this.mapiContext.CriticalBlock((LID)37708U, CriticalBlockScope.MailboxSession))
										{
											throw;
										}
									}
								}
								catch (BufferParseException exception3)
								{
									this.mapiContext.OnExceptionCatch(exception3);
									DiagnosticContext.TraceDword((LID)46632U, 1U);
									ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)64216U, exception3);
									if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
									{
										throw;
									}
									if (mapiBase is MapiLogon)
									{
										using (this.mapiContext.CriticalBlock((LID)45900U, CriticalBlockScope.MailboxSession))
										{
											throw;
										}
									}
								}
							}
							IL_343:
							this.AssertSessionIsNotTerminating(RopId.Release, (ErrorCode)2147746069U, null);
						}
					}
				}
			}
		}

		protected abstract void Release(MapiContext context, MapiBase serverObject);

		public RopResult ReloadCachedInformation(IServerObject serverObject, PropertyTag[] extraUnicodePropertyTags, ReloadCachedInformationResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ReloadCachedInformation"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ReloadCachedInformation, RopHandlerBase.ReloadCachedInformationClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ReloadCachedInformation, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ReloadCachedInformation, mapiMessage);
								ropResult = this.ReloadCachedInformation(this.mapiContext, mapiMessage, extraUnicodePropertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 16U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 16U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 16U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ReloadCachedInformation, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ReloadCachedInformation(MapiContext context, MapiMessage serverObject, PropertyTag[] extraUnicodePropertyTags, ReloadCachedInformationResultFactory resultFactory);

		public RopResult RemoveAllRecipients(IServerObject serverObject, RemoveAllRecipientsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.RemoveAllRecipients"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.RemoveAllRecipients, RopHandlerBase.RemoveAllRecipientsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.RemoveAllRecipients, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.RemoveAllRecipients, mapiMessage);
								ropResult = this.RemoveAllRecipients(this.mapiContext, mapiMessage, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 13U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 13U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 13U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.RemoveAllRecipients, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult RemoveAllRecipients(MapiContext context, MapiMessage serverObject, RemoveAllRecipientsResultFactory resultFactory);

		public RopResult ResetTable(IServerObject serverObject, ResetTableResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.ResetTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.ResetTable, RopHandlerBase.ResetTableClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.ResetTable, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.ResetTable, mapiViewTableBase);
								ropResult = this.ResetTable(this.mapiContext, mapiViewTableBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 129U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 129U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 129U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.ResetTable, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult ResetTable(MapiContext context, MapiViewTableBase serverObject, ResetTableResultFactory resultFactory);

		public RopResult Restrict(IServerObject serverObject, RestrictFlags flags, Restriction restriction, RestrictResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.Restrict"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.Restrict, RopHandlerBase.RestrictClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.Restrict, RopHandlerBase.RestrictClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.Restrict, mapiViewTableBase);
								ropResult = this.Restrict(this.mapiContext, mapiViewTableBase, flags, restriction, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 20U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 20U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 20U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.Restrict, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				if (mapiViewTableBase != null)
				{
					mapiViewTableBase.ConfigurationError.Restrict = ropResult.ErrorCode;
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult Restrict(MapiContext context, MapiViewTableBase serverObject, RestrictFlags flags, Restriction restriction, RestrictResultFactory resultFactory);

		public RopResult SaveChangesAttachment(IServerObject serverObject, SaveChangesMode saveChangesMode, SaveChangesAttachmentResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SaveChangesAttachment"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiAttachment mapiAttachment = serverObject as MapiAttachment;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiAttachment == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiAttachment;
					MapiLogon logon = mapiAttachment.Logon;
					bool flag = 4 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsSaveChangesAttachmentSharedMailboxOperation(mapiAttachment);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SaveChangesAttachment, RopHandlerBase.SaveChangesAttachmentClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SaveChangesAttachment, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SaveChangesAttachment, mapiAttachment);
								ropResult = this.SaveChangesAttachment(this.mapiContext, mapiAttachment, saveChangesMode, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 37U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 37U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 37U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SaveChangesAttachment, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SaveChangesAttachment(MapiContext context, MapiAttachment serverObject, SaveChangesMode saveChangesMode, SaveChangesAttachmentResultFactory resultFactory);

		public RopResult SaveChangesMessage(IServerObject serverObject, SaveChangesMode saveChangesMode, SaveChangesMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SaveChangesMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 4 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsSaveChangesMessageSharedMailboxOperation(mapiMessage);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SaveChangesMessage, RopHandlerBase.SaveChangesMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SaveChangesMessage, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SaveChangesMessage, mapiMessage);
								ropResult = this.SaveChangesMessage(this.mapiContext, mapiMessage, saveChangesMode, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 12U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 12U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 12U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SaveChangesMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SaveChangesMessage(MapiContext context, MapiMessage serverObject, SaveChangesMode saveChangesMode, SaveChangesMessageResultFactory resultFactory);

		public RopResult SeekRow(IServerObject serverObject, BookmarkOrigin bookmarkOrigin, int rowCount, bool wantMoveCount, SeekRowResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SeekRow"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SeekRow, RopHandlerBase.SeekRowClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SeekRow, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SeekRow, mapiViewTableBase);
								ropResult = this.SeekRow(this.mapiContext, mapiViewTableBase, bookmarkOrigin, rowCount, wantMoveCount, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 24U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 24U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 24U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SeekRow, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SeekRow(MapiContext context, MapiViewTableBase serverObject, BookmarkOrigin bookmarkOrigin, int rowCount, bool wantMoveCount, SeekRowResultFactory resultFactory);

		public RopResult SeekRowApproximate(IServerObject serverObject, uint numerator, uint denominator, SeekRowApproximateResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SeekRowApproximate"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SeekRowApproximate, RopHandlerBase.SeekRowApproximateClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SeekRowApproximate, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SeekRowApproximate, mapiViewTableBase);
								ropResult = this.SeekRowApproximate(this.mapiContext, mapiViewTableBase, numerator, denominator, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 26U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 26U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 26U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SeekRowApproximate, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SeekRowApproximate(MapiContext context, MapiViewTableBase serverObject, uint numerator, uint denominator, SeekRowApproximateResultFactory resultFactory);

		public RopResult SeekRowBookmark(IServerObject serverObject, byte[] bookmark, int rowCount, bool wantMoveCount, SeekRowBookmarkResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SeekRowBookmark"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SeekRowBookmark, RopHandlerBase.SeekRowBookmarkClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SeekRowBookmark, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SeekRowBookmark, mapiViewTableBase);
								ropResult = this.SeekRowBookmark(this.mapiContext, mapiViewTableBase, bookmark, rowCount, wantMoveCount, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 25U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 25U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 25U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SeekRowBookmark, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SeekRowBookmark(MapiContext context, MapiViewTableBase serverObject, byte[] bookmark, int rowCount, bool wantMoveCount, SeekRowBookmarkResultFactory resultFactory);

		public RopResult SeekStream(IServerObject serverObject, StreamSeekOrigin streamSeekOrigin, long offset, SeekStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SeekStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SeekStream, RopHandlerBase.SeekStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SeekStream, RopHandlerBase.SeekStreamClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SeekStream, mapiStream);
								ropResult = this.SeekStream(this.mapiContext, mapiStream, streamSeekOrigin, offset, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 46U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 46U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 46U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SeekStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SeekStream(MapiContext context, MapiStream serverObject, StreamSeekOrigin streamSeekOrigin, long offset, SeekStreamResultFactory resultFactory);

		public RopResult SetCollapseState(IServerObject serverObject, byte[] collapseState, SetCollapseStateResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetCollapseState"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else if (mapiViewTableBase.ConfigurationError.HasConfigurationError)
				{
					errorCode = mapiViewTableBase.ConfigurationError.ErrorCode;
					DiagnosticContext.TraceDword((LID)60464U, (uint)errorCode);
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetCollapseState, RopHandlerBase.SetCollapseStateClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetCollapseState, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetCollapseState, mapiViewTableBase);
								ropResult = this.SetCollapseState(this.mapiContext, mapiViewTableBase, collapseState, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 108U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 108U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 108U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetCollapseState, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetCollapseState(MapiContext context, MapiViewTableBase serverObject, byte[] collapseState, SetCollapseStateResultFactory resultFactory);

		public RopResult SetColumns(IServerObject serverObject, SetColumnsFlags flags, PropertyTag[] propertyTags, SetColumnsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetColumns"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetColumns, RopHandlerBase.SetColumnsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetColumns, RopHandlerBase.SetColumnsClientTypesAllowedOnReadOnlyDatabase);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetColumns, mapiViewTableBase);
								ropResult = this.SetColumns(this.mapiContext, mapiViewTableBase, flags, propertyTags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 18U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 18U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 18U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetColumns, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				if (mapiViewTableBase != null)
				{
					mapiViewTableBase.ConfigurationError.SetColumns = ropResult.ErrorCode;
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetColumns(MapiContext context, MapiViewTableBase serverObject, SetColumnsFlags flags, PropertyTag[] propertyTags, SetColumnsResultFactory resultFactory);

		public RopResult SetLocalReplicaMidsetDeleted(IServerObject serverObject, LongTermIdRange[] longTermIdRanges, SetLocalReplicaMidsetDeletedResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.SetLocalReplicaMidsetDeleted"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult SetMessageFlags(IServerObject serverObject, StoreId messageId, MessageFlags flags, MessageFlags flagsMask, SetMessageFlagsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetMessageFlags"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetMessageFlags, RopHandlerBase.SetMessageFlagsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetMessageFlags, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetMessageFlags, mapiFolder);
								ropResult = this.SetMessageFlags(this.mapiContext, mapiFolder, RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), flags, flagsMask, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 154U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 154U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 154U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetMessageFlags, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetMessageFlags(MapiContext context, MapiFolder serverObject, ExchangeId messageId, MessageFlags flags, MessageFlags flagsMask, SetMessageFlagsResultFactory resultFactory);

		public RopResult SetMessageStatus(IServerObject serverObject, StoreId messageId, MessageStatusFlags status, MessageStatusFlags statusMask, SetMessageStatusResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetMessageStatus"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetMessageStatus, RopHandlerBase.SetMessageStatusClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetMessageStatus, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetMessageStatus, mapiFolder);
								ropResult = this.SetMessageStatus(this.mapiContext, mapiFolder, RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), status, statusMask, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 32U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 32U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 32U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetMessageStatus, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetMessageStatus(MapiContext context, MapiFolder serverObject, ExchangeId messageId, MessageStatusFlags status, MessageStatusFlags statusMask, SetMessageStatusResultFactory resultFactory);

		public RopResult SetProperties(IServerObject serverObject, PropertyValue[] propertyValues, SetPropertiesResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetProperties"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsSetPropertiesSharedMailboxOperation(mapiPropBagBase);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetProperties, RopHandlerBase.SetPropertiesClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetProperties, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetProperties, mapiPropBagBase);
								ropResult = this.SetProperties(this.mapiContext, mapiPropBagBase, propertyValues, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 10U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 10U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 10U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetProperties, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetProperties(MapiContext context, MapiPropBagBase serverObject, PropertyValue[] propertyValues, SetPropertiesResultFactory resultFactory);

		public RopResult SetPropertiesNoReplicate(IServerObject serverObject, PropertyValue[] propertyValues, SetPropertiesNoReplicateResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetPropertiesNoReplicate"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiPropBagBase mapiPropBagBase = serverObject as MapiPropBagBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiPropBagBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiPropBagBase;
					MapiLogon logon = mapiPropBagBase.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsSetPropertiesNoReplicateSharedMailboxOperation(mapiPropBagBase);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetPropertiesNoReplicate, RopHandlerBase.SetPropertiesNoReplicateClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetPropertiesNoReplicate, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetPropertiesNoReplicate, mapiPropBagBase);
								ropResult = this.SetPropertiesNoReplicate(this.mapiContext, mapiPropBagBase, propertyValues, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 121U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 121U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 121U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetPropertiesNoReplicate, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetPropertiesNoReplicate(MapiContext context, MapiPropBagBase serverObject, PropertyValue[] propertyValues, SetPropertiesNoReplicateResultFactory resultFactory);

		public RopResult SetReadFlag(IServerObject serverObject, SetReadFlagFlags flags, SetReadFlagResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetReadFlag"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 5 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsSetReadFlagSharedMailboxOperation(mapiMessage);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetReadFlag, RopHandlerBase.SetReadFlagClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetReadFlag, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetReadFlag, mapiMessage);
								ropResult = this.SetReadFlag(this.mapiContext, mapiMessage, flags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 17U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 17U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 17U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetReadFlag, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetReadFlag(MapiContext context, MapiMessage serverObject, SetReadFlagFlags flags, SetReadFlagResultFactory resultFactory);

		public RopResult SetReadFlags(IServerObject serverObject, bool reportProgress, SetReadFlagFlags flags, StoreId[] messageIds, SetReadFlagsResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetReadFlags"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				bool isPartiallyCompleted = false;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = 5 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsSetReadFlagsSharedMailboxOperation(mapiFolder);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetReadFlags, RopHandlerBase.SetReadFlagsClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetReadFlags, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetReadFlags, mapiFolder);
								ropResult = this.SetReadFlags(this.mapiContext, mapiFolder, reportProgress, flags, RcaTypeHelpers.StoreIdsToExchangeIds(messageIds, logon.StoreMailbox), out isPartiallyCompleted, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 102U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 102U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 102U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetReadFlags, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetReadFlags(MapiContext context, MapiFolder serverObject, bool reportProgress, SetReadFlagFlags flags, ExchangeId[] messageIds, out bool outputIsPartiallyCompleted, SetReadFlagsResultFactory resultFactory);

		public RopResult SetReceiveFolder(IServerObject serverObject, StoreId folderId, string messageClass, SetReceiveFolderResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetReceiveFolder"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetReceiveFolder, RopHandlerBase.SetReceiveFolderClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetReceiveFolder, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetReceiveFolder, mapiBase);
								ropResult = this.SetReceiveFolder(this.mapiContext, mapiBase, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), messageClass, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 38U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 38U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 38U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetReceiveFolder, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetReceiveFolder(MapiContext context, MapiBase serverObject, ExchangeId folderId, string messageClass, SetReceiveFolderResultFactory resultFactory);

		public RopResult SetSearchCriteria(IServerObject serverObject, Restriction restriction, StoreId[] folderIds, SetSearchCriteriaFlags setSearchCriteriaFlags, SetSearchCriteriaResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetSearchCriteria"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiFolder mapiFolder = serverObject as MapiFolder;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiFolder == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiFolder;
					MapiLogon logon = mapiFolder.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetSearchCriteria, RopHandlerBase.SetSearchCriteriaClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetSearchCriteria, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetSearchCriteria, mapiFolder);
								ropResult = this.SetSearchCriteria(this.mapiContext, mapiFolder, restriction, RcaTypeHelpers.StoreIdsToExchangeIds(folderIds, logon.StoreMailbox), setSearchCriteriaFlags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 48U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 48U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 48U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetSearchCriteria, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetSearchCriteria(MapiContext context, MapiFolder serverObject, Restriction restriction, ExchangeId[] folderIds, SetSearchCriteriaFlags setSearchCriteriaFlags, SetSearchCriteriaResultFactory resultFactory);

		public RopResult SetSizeStream(IServerObject serverObject, ulong streamSize, SetSizeStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetSizeStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetSizeStream, RopHandlerBase.SetSizeStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetSizeStream, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetSizeStream, mapiStream);
								ropResult = this.SetSizeStream(this.mapiContext, mapiStream, streamSize, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 47U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 47U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 47U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetSizeStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetSizeStream(MapiContext context, MapiStream serverObject, ulong streamSize, SetSizeStreamResultFactory resultFactory);

		public RopResult SetSpooler(IServerObject serverObject, SetSpoolerResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetSpooler"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetSpooler, RopHandlerBase.SetSpoolerClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetSpooler, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetSpooler, mapiLogon);
								ropResult = this.SetSpooler(this.mapiContext, mapiLogon, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 71U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 71U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 71U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetSpooler, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetSpooler(MapiContext context, MapiLogon serverObject, SetSpoolerResultFactory resultFactory);

		public RopResult SetSynchronizationNotificationGuid(IServerObject serverObject, Guid notificationGuid, SetSynchronizationNotificationGuidResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.SetSynchronizationNotificationGuid"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult SetTransport(IServerObject serverObject, SetTransportResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SetTransport"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 1 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SetTransport, RopHandlerBase.SetTransportClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SetTransport, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SetTransport, mapiLogon);
								ropResult = this.SetTransport(this.mapiContext, mapiLogon, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 109U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 109U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 109U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SetTransport, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SetTransport(MapiContext context, MapiLogon serverObject, SetTransportResultFactory resultFactory);

		public RopResult SortTable(IServerObject serverObject, SortTableFlags flags, ushort categoryCount, ushort expandedCount, SortOrder[] sortOrders, SortTableResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SortTable"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiViewTableBase mapiViewTableBase = serverObject as MapiViewTableBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiViewTableBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiViewTableBase;
					MapiLogon logon = mapiViewTableBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SortTable, RopHandlerBase.SortTableClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SortTable, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SortTable, mapiViewTableBase);
								ropResult = this.SortTable(this.mapiContext, mapiViewTableBase, flags, categoryCount, expandedCount, sortOrders, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 19U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 19U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 19U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SortTable, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				if (mapiViewTableBase != null)
				{
					mapiViewTableBase.ConfigurationError.SortTable = ropResult.ErrorCode;
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SortTable(MapiContext context, MapiViewTableBase serverObject, SortTableFlags flags, ushort categoryCount, ushort expandedCount, SortOrder[] sortOrders, SortTableResultFactory resultFactory);

		public RopResult SpoolerLockMessage(IServerObject serverObject, StoreId messageId, LockState lockState, SpoolerLockMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SpoolerLockMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SpoolerLockMessage, RopHandlerBase.SpoolerLockMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SpoolerLockMessage, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SpoolerLockMessage, mapiLogon);
								ropResult = this.SpoolerLockMessage(this.mapiContext, mapiLogon, RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), lockState, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 72U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 72U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 72U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SpoolerLockMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SpoolerLockMessage(MapiContext context, MapiLogon serverObject, ExchangeId messageId, LockState lockState, SpoolerLockMessageResultFactory resultFactory);

		public RopResult SpoolerRules(IServerObject serverObject, StoreId folderId, SpoolerRulesResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.SpoolerRules"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult SubmitMessage(IServerObject serverObject, SubmitMessageFlags submitFlags, SubmitMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.SubmitMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.SubmitMessage, RopHandlerBase.SubmitMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.SubmitMessage, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.SubmitMessage, mapiMessage);
								ropResult = this.SubmitMessage(this.mapiContext, mapiMessage, submitFlags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 50U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 50U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 50U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.SubmitMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult SubmitMessage(MapiContext context, MapiMessage serverObject, SubmitMessageFlags submitFlags, SubmitMessageResultFactory resultFactory);

		public RopResult SynchronizationOpenAdvisor(IServerObject serverObject, SynchronizationOpenAdvisorResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.SynchronizationOpenAdvisor"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult TellVersion(IServerObject serverObject, ushort productVersion, ushort buildMajorVersion, ushort buildMinorVersion, TellVersionResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.TellVersion"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				FastTransferContext fastTransferContext = serverObject as FastTransferContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (fastTransferContext == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = fastTransferContext;
					MapiLogon logon = fastTransferContext.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.TellVersion, RopHandlerBase.TellVersionClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.TellVersion, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.TellVersion, fastTransferContext);
								ropResult = this.TellVersion(this.mapiContext, fastTransferContext, productVersion, buildMajorVersion, buildMinorVersion, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 134U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 134U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 134U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.TellVersion, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult TellVersion(MapiContext context, FastTransferContext serverObject, ushort productVersion, ushort buildMajorVersion, ushort buildMinorVersion, TellVersionResultFactory resultFactory);

		public RopResult TransportDeliverMessage(IServerObject serverObject, TransportRecipientType recipientType, TransportDeliverMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.TransportDeliverMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.TransportDeliverMessage, RopHandlerBase.TransportDeliverMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.TransportDeliverMessage, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.TransportDeliverMessage, mapiMessage);
								ropResult = this.TransportDeliverMessage(this.mapiContext, mapiMessage, recipientType, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 148U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 148U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 148U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.TransportDeliverMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult TransportDeliverMessage(MapiContext context, MapiMessage serverObject, TransportRecipientType recipientType, TransportDeliverMessageResultFactory resultFactory);

		public RopResult TransportDeliverMessage2(IServerObject serverObject, TransportRecipientType recipientType, TransportDeliverMessage2ResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.TransportDeliverMessage2"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.TransportDeliverMessage2, RopHandlerBase.TransportDeliverMessage2ClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.TransportDeliverMessage2, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.TransportDeliverMessage2, mapiMessage);
								ropResult = this.TransportDeliverMessage2(this.mapiContext, mapiMessage, recipientType, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 158U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 158U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 158U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.TransportDeliverMessage2, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult TransportDeliverMessage2(MapiContext context, MapiMessage serverObject, TransportRecipientType recipientType, TransportDeliverMessage2ResultFactory resultFactory);

		public RopResult TransportDoneWithMessage(IServerObject serverObject, TransportDoneWithMessageResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.TransportDoneWithMessage"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.TransportDoneWithMessage, RopHandlerBase.TransportDoneWithMessageClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.TransportDoneWithMessage, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.TransportDoneWithMessage, mapiMessage);
								ropResult = this.TransportDoneWithMessage(this.mapiContext, mapiMessage, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 149U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 149U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 149U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.TransportDoneWithMessage, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult TransportDoneWithMessage(MapiContext context, MapiMessage serverObject, TransportDoneWithMessageResultFactory resultFactory);

		public RopResult TransportDuplicateDeliveryCheck(IServerObject serverObject, byte flags, ExDateTime submitTime, string internetMessageId, TransportDuplicateDeliveryCheckResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.TransportDuplicateDeliveryCheck"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = 3 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.TransportDuplicateDeliveryCheck, RopHandlerBase.TransportDuplicateDeliveryCheckClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.TransportDuplicateDeliveryCheck, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.TransportDuplicateDeliveryCheck, mapiMessage);
								ropResult = this.TransportDuplicateDeliveryCheck(this.mapiContext, mapiMessage, flags, submitTime, internetMessageId, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 161U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 161U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 161U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.TransportDuplicateDeliveryCheck, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult TransportDuplicateDeliveryCheck(MapiContext context, MapiMessage serverObject, byte flags, ExDateTime submitTime, string internetMessageId, TransportDuplicateDeliveryCheckResultFactory resultFactory);

		public RopResult TransportNewMail(IServerObject serverObject, StoreId folderId, StoreId messageId, string messageClass, MessageFlags messageFlags, TransportNewMailResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.TransportNewMail"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.TransportNewMail, RopHandlerBase.TransportNewMailClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.TransportNewMail, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.TransportNewMail, mapiLogon);
								ropResult = this.TransportNewMail(this.mapiContext, mapiLogon, RcaTypeHelpers.StoreIdToExchangeId(folderId, logon.StoreMailbox), RcaTypeHelpers.StoreIdToExchangeId(messageId, logon.StoreMailbox), messageClass, messageFlags, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.OnMid(messageId);
								mapiExecutionDiagnostics.MapiExMonLogger.OnFid(folderId);
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 81U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 81U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 81U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.TransportNewMail, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult TransportNewMail(MapiContext context, MapiLogon serverObject, ExchangeId folderId, ExchangeId messageId, string messageClass, MessageFlags messageFlags, TransportNewMailResultFactory resultFactory);

		public RopResult TransportSend(IServerObject serverObject, TransportSendResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.TransportSend"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiMessage mapiMessage = serverObject as MapiMessage;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiMessage == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiMessage;
					MapiLogon logon = mapiMessage.Logon;
					bool flag = false;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.TransportSend, RopHandlerBase.TransportSendClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.TransportSend, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.TransportSend, mapiMessage);
								ropResult = this.TransportSend(this.mapiContext, mapiMessage, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 74U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 74U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 74U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.TransportSend, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult TransportSend(MapiContext context, MapiMessage serverObject, TransportSendResultFactory resultFactory);

		public RopResult UnlockRegionStream(IServerObject serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, UnlockRegionStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.UnlockRegionStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.UnlockRegionStream, RopHandlerBase.UnlockRegionStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.UnlockRegionStream, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.UnlockRegionStream, mapiStream);
								ropResult = this.UnlockRegionStream(this.mapiContext, mapiStream, offset, regionLength, lockType, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 92U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 92U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 92U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.UnlockRegionStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult UnlockRegionStream(MapiContext context, MapiStream serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, UnlockRegionStreamResultFactory resultFactory);

		public RopResult UpdateDeferredActionMessages(IServerObject serverObject, byte[] serverEntryId, byte[] clientEntryId, UpdateDeferredActionMessagesResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.UpdateDeferredActionMessages"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			return result;
		}

		public RopResult UploadStateStreamBegin(IServerObject serverObject, PropertyTag propertyTag, uint size, UploadStateStreamBeginResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.UploadStateStreamBegin"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.UploadStateStreamBegin, RopHandlerBase.UploadStateStreamBeginClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.UploadStateStreamBegin, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.UploadStateStreamBegin, mapiBase);
								ropResult = this.UploadStateStreamBegin(this.mapiContext, mapiBase, propertyTag, size, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 117U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 117U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 117U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.UploadStateStreamBegin, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult UploadStateStreamBegin(MapiContext context, MapiBase serverObject, PropertyTag propertyTag, uint size, UploadStateStreamBeginResultFactory resultFactory);

		public RopResult UploadStateStreamContinue(IServerObject serverObject, ArraySegment<byte> data, UploadStateStreamContinueResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.UploadStateStreamContinue"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.UploadStateStreamContinue, RopHandlerBase.UploadStateStreamContinueClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.UploadStateStreamContinue, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.UploadStateStreamContinue, mapiBase);
								ropResult = this.UploadStateStreamContinue(this.mapiContext, mapiBase, data, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 118U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 118U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 118U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.UploadStateStreamContinue, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult UploadStateStreamContinue(MapiContext context, MapiBase serverObject, ArraySegment<byte> data, UploadStateStreamContinueResultFactory resultFactory);

		public RopResult UploadStateStreamEnd(IServerObject serverObject, UploadStateStreamEndResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.UploadStateStreamEnd"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiBase mapiBase = serverObject as MapiBase;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiBase == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiBase;
					MapiLogon logon = mapiBase.Logon;
					bool flag = true;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.UploadStateStreamEnd, RopHandlerBase.UploadStateStreamEndClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.UploadStateStreamEnd, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.UploadStateStreamEnd, mapiBase);
								ropResult = this.UploadStateStreamEnd(this.mapiContext, mapiBase, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 119U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 119U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 119U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.UploadStateStreamEnd, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult UploadStateStreamEnd(MapiContext context, MapiBase serverObject, UploadStateStreamEndResultFactory resultFactory);

		public RopResult WriteCommitStream(IServerObject serverObject, byte[] data, WriteCommitStreamResultFactory resultFactory)
		{
			RopResult result;
			using (this.CreateThreadManagerMethodFrame("MapiRop.WriteCommitStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ushort byteCount = 0;
				result = resultFactory.CreateFailedResult((ErrorCode)2147746050U, byteCount);
			}
			return result;
		}

		public RopResult WritePerUserInformation(IServerObject serverObject, StoreLongTermId longTermId, bool hasFinished, uint dataOffset, byte[] data, Guid? replicaGuid, WritePerUserInformationResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.WritePerUserInformation"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiLogon mapiLogon = serverObject as MapiLogon;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				if (mapiLogon == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiLogon;
					MapiLogon logon = mapiLogon.Logon;
					bool flag = 5 <= ConfigurationSchema.ConfigurableSharedLockStage.Value && this.IsWritePerUserInformationSharedMailboxOperation(hasFinished);
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = false;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.WritePerUserInformation, RopHandlerBase.WritePerUserInformationClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.WritePerUserInformation, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.WritePerUserInformation, mapiLogon);
								ropResult = this.WritePerUserInformation(this.mapiContext, mapiLogon, longTermId, hasFinished, dataOffset, data, replicaGuid, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 100U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 100U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 100U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.WritePerUserInformation, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult WritePerUserInformation(MapiContext context, MapiLogon serverObject, StoreLongTermId longTermId, bool hasFinished, uint dataOffset, byte[] data, Guid? replicaGuid, WritePerUserInformationResultFactory resultFactory);

		public RopResult WriteStream(IServerObject serverObject, ArraySegment<byte> data, WriteStreamResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.WriteStream"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				ushort byteCount = 0;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.WriteStream, RopHandlerBase.WriteStreamClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.WriteStream, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.WriteStream, mapiStream);
								ropResult = this.WriteStream(this.mapiContext, mapiStream, data, out byteCount, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 45U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 45U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 45U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.WriteStream, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, byteCount);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult WriteStream(MapiContext context, MapiStream serverObject, ArraySegment<byte> data, out ushort outputByteCount, WriteStreamResultFactory resultFactory);

		public RopResult WriteStreamExtended(IServerObject serverObject, ArraySegment<byte>[] dataChunks, WriteStreamExtendedResultFactory resultFactory)
		{
			RopResult result;
			using (ThreadManager.MethodFrame methodFrame = this.CreateThreadManagerMethodFrame("MapiRop.WriteStreamExtended"))
			{
				if (resultFactory == null)
				{
					throw new ArgumentNullException("resultFactory");
				}
				if (serverObject == null)
				{
					throw new ArgumentNullException("serverObject");
				}
				ErrorCode errorCode = (ErrorCode)2147500037U;
				RopResult ropResult = null;
				MapiStream mapiStream = serverObject as MapiStream;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)this.mapiContext.Diagnostics;
				uint byteCount = 0U;
				if (mapiStream == null)
				{
					errorCode = (ErrorCode)2147746050U;
				}
				else
				{
					mapiExecutionDiagnostics.MapiObject = mapiStream;
					MapiLogon logon = mapiStream.Logon;
					bool flag = 2 <= ConfigurationSchema.ConfigurableSharedLockStage.Value;
					bool flag2 = logon.IsDeferedReleaseSharedOperation();
					bool flag3 = true;
					this.mapiContext.Initialize(logon, flag && flag2, flag3);
					methodFrame.CurrentThreadInfo.MailboxGuid = logon.MailboxGuid;
					logon.DatabaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(this.mapiContext, logon.DatabaseInfo.MdbGuid);
					logon.LoggedOnUserAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo.ObjectId);
					logon.MailboxOwnerAddressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByObjectId(this.mapiContext, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxOwnerAddressInfo.ObjectId);
					if (!logon.MailboxInfo.IsDisconnected)
					{
						logon.MailboxInfo = RopHandlerBase.GetMailboxInfo(this.mapiContext, logon);
						if (!RopHandlerBase.ValidateMailboxType(logon.MapiMailbox.SharedState, logon.MailboxInfo))
						{
							throw new StoreException((LID)62412U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					try
					{
						errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
						if (errorCode == ErrorCode.None)
						{
							bool commit = false;
							try
							{
								logon.ProcessDeferedReleaseROPs(this.mapiContext);
								if (!flag2 && flag3)
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(true, true);
									}
									this.mapiContext.Initialize(logon, flag, flag3);
									errorCode = (ErrorCode)this.mapiContext.StartMailboxOperation(MailboxCreation.DontAllow, false, false, true);
								}
								if (errorCode != ErrorCode.None)
								{
									throw new StoreException((LID)60236U, (ErrorCodeValue)errorCode);
								}
								this.CheckClientTypeIsAllowedOnMoveTarget(logon, RopId.WriteStreamExtended, RopHandlerBase.WriteStreamExtendedClientTypesAllowedOnMoveTarget);
								if (this.mapiContext.Database.IsReadOnly)
								{
									RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(this.mapiContext, RopId.WriteStreamExtended, null);
								}
								this.mapiContext.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.WriteStreamExtended, mapiStream);
								ropResult = this.WriteStreamExtended(this.mapiContext, mapiStream, dataChunks, out byteCount, resultFactory);
								commit = true;
							}
							finally
							{
								mapiExecutionDiagnostics.MapiExMonLogger.AccessedMailboxLegacyDn = logon.MailboxOwnerAddressInfo.LegacyExchangeDN;
								try
								{
									if (this.mapiContext.IsMailboxOperationStarted)
									{
										this.mapiContext.EndMailboxOperation(commit, true);
									}
								}
								finally
								{
									this.mapiContext.SkipDatabaseLogsFlush = false;
								}
							}
						}
					}
					catch (StoreException ex)
					{
						this.mapiContext.OnExceptionCatch(ex);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)63016U, 163U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)39640U, ex);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = (ErrorCode)ex.Error;
					}
					catch (RopExecutionException ex2)
					{
						this.mapiContext.OnExceptionCatch(ex2);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)38440U, 163U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)56024U, ex2);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ex2.ErrorCode;
					}
					catch (BufferParseException exception)
					{
						this.mapiContext.OnExceptionCatch(exception);
						ropResult = null;
						DiagnosticContext.TraceDword((LID)54824U, 163U);
						ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer, (LID)43736U, exception);
						if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
						{
							throw;
						}
						errorCode = ErrorCode.RpcFormat;
					}
				}
				this.AssertSessionIsNotTerminating(RopId.WriteStreamExtended, errorCode, ropResult);
				if (ropResult == null)
				{
					if (errorCode == (ErrorCode)2147746817U)
					{
						throw new StoreException((LID)58656U, (ErrorCodeValue)errorCode);
					}
					ropResult = resultFactory.CreateFailedResult(errorCode, byteCount);
				}
				if (ropResult.ErrorCode != ErrorCode.None)
				{
					ropResult.SetDiagnosticInfoProvider(mapiExecutionDiagnostics);
				}
				result = ropResult;
			}
			return result;
		}

		protected abstract RopResult WriteStreamExtended(MapiContext context, MapiStream serverObject, ArraySegment<byte>[] dataChunks, out uint outputByteCount, WriteStreamExtendedResultFactory resultFactory);

		public MapiContext MapiContext
		{
			get
			{
				return this.mapiContext;
			}
			set
			{
				this.mapiContext = value;
			}
		}

		internal static bool SkipDatabaseLogFlush(RopId ropId, MapiBase mapiObject)
		{
			if (ropId <= RopId.FindRow)
			{
				if (ropId <= RopId.SaveChangesAttachment)
				{
					if (ropId == RopId.Release)
					{
						return !(mapiObject is MapiStream);
					}
					switch (ropId)
					{
					case RopId.SaveChangesMessage:
						return ((MapiMessage)mapiObject).StoreMessage.IsEmbedded;
					case RopId.RemoveAllRecipients:
					case RopId.FlushRecipients:
					case RopId.ReadRecipients:
					case RopId.ReloadCachedInformation:
					case RopId.GetStatus:
					case RopId.CreateFolder:
					case RopId.DeleteFolder:
					case RopId.DeleteMessages:
					case RopId.SetMessageStatus:
					case RopId.GetAttachmentTable:
					case RopId.OpenAttachment:
						return false;
					case RopId.SetReadFlag:
					case RopId.SetColumns:
					case RopId.SortTable:
					case RopId.Restrict:
					case RopId.QueryRows:
					case RopId.QueryPosition:
					case RopId.SeekRow:
					case RopId.SeekRowBookmark:
					case RopId.SeekRowApproximate:
					case RopId.CreateBookmark:
					case RopId.GetMessageStatus:
					case RopId.CreateAttachment:
					case RopId.DeleteAttachment:
					case RopId.SaveChangesAttachment:
						break;
					default:
						return false;
					}
				}
				else if (ropId != RopId.OpenEmbeddedMessage)
				{
					switch (ropId)
					{
					case RopId.FastTransferSourceGetBuffer:
					case RopId.FindRow:
						break;
					default:
						return false;
					}
				}
			}
			else if (ropId <= RopId.FastTransferGetIncrementalState)
			{
				switch (ropId)
				{
				case RopId.ExpandRow:
				case RopId.CollapseRow:
					break;
				default:
					if (ropId != RopId.FastTransferGetIncrementalState)
					{
						return false;
					}
					break;
				}
			}
			else if (ropId != RopId.FastTransferSourceGetBufferExtended && ropId != RopId.TransportDuplicateDeliveryCheck && ropId != RopId.Logon)
			{
				return false;
			}
			return true;
		}

		internal static bool SkipHomeMdbValidation(MapiContext context, OpenStoreFlags openStoreFlags)
		{
			return (context.ClientType == ClientType.Migration || ClientTypeHelper.IsContentIndexing(context.ClientType)) && OpenStoreFlags.None != (openStoreFlags & OpenStoreFlags.OverrideHomeMdb);
		}

		internal static MailboxInfo GetMailboxInfo(MapiContext context, MapiLogon logon)
		{
			GetMailboxInfoFlags flags = GetMailboxInfoFlags.None;
			if (RopHandlerBase.SkipHomeMdbValidation(context, logon.OpenStoreFlags))
			{
				flags = GetMailboxInfoFlags.IgnoreHomeMdb;
			}
			return Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetMailboxInfo(context, logon.MapiMailbox.SharedState.TenantHint, logon.MailboxInfo.MailboxGuid, flags);
		}

		protected static void CheckClientTypeIsAllowedOnReadOnlyDatabase(MapiContext context, RopId ropId, IList<ClientType> clientTypesAllowedOnReadOnlyDatabase)
		{
			if (clientTypesAllowedOnReadOnlyDatabase != null && clientTypesAllowedOnReadOnlyDatabase.Contains(context.ClientType))
			{
				return;
			}
			string message = string.Format("Client {0} is not allowed to execute Rop {1} on read-only databases.", context.ClientType, ropId);
			DiagnosticContext.TraceDword((LID)45180U, (uint)context.ClientType);
			DiagnosticContext.TraceDword((LID)61564U, (uint)ropId);
			if (context.Database.IsOnlinePassiveAttachedReadOnly)
			{
				throw new StoreException((LID)36988U, ErrorCodeValue.MdbNotInitialized, message);
			}
			throw new ExExceptionAccessDenied((LID)38652U, message);
		}

		protected static bool ValidateMailboxType(MailboxState mailboxState, MailboxInfo mailboxInfo)
		{
			return (mailboxState.MailboxType != MailboxInfo.MailboxType.PublicFolderPrimary && mailboxState.MailboxType != MailboxInfo.MailboxType.PublicFolderSecondary && mailboxInfo.Type != MailboxInfo.MailboxType.PublicFolderPrimary && mailboxInfo.Type != MailboxInfo.MailboxType.PublicFolderSecondary) || !ConfigurationSchema.ValidatePublicFolderMailboxTypeMatch.Value || mailboxState.MailboxType == mailboxInfo.Type;
		}

		private static void CheckGetContentsTableExtendedConditionsForReadOnlyDatabase(MapiContext context, MapiFolder mapiFolder, ExtendedTableFlags extendedTableFlags)
		{
			if ((extendedTableFlags & ExtendedTableFlags.DocumentIdView) == ExtendedTableFlags.None)
			{
				DiagnosticContext.TraceDword((LID)47612U, (uint)context.ClientType);
				DiagnosticContext.TraceDword((LID)63996U, (uint)extendedTableFlags);
				throw new ExExceptionNoSupport((LID)55804U, string.Format("Only DocumentId view is supported when calling GetContentsTableExtended on read-only databases (ClientType={0}).", context.ClientType));
			}
		}

		private static void CheckGetIdsFromNamesConditionsForReadOnlyDatabase(MapiContext context, MapiBase mapiBase, GetIdsFromNamesFlags flags, NamedProperty[] namedProperties)
		{
			if ((byte)(flags & GetIdsFromNamesFlags.Create) != 0)
			{
				DiagnosticContext.TraceDword((LID)43516U, (uint)context.ClientType);
				DiagnosticContext.TraceDword((LID)59900U, (uint)flags);
				throw new ExExceptionNoSupport((LID)35324U, string.Format("The Create flag is not supported when calling GetIdsFromNames on read-only databases (ClientType={0}).", context.ClientType));
			}
		}

		private static void CheckGetPropertiesAllConditionsForReadOnlyDatabase(MapiContext context, MapiPropBagBase propertyBag, ushort streamLimit, GetPropertiesFlags flags)
		{
			if (propertyBag.MapiObjectType == MapiObjectType.Logon)
			{
				DiagnosticContext.TraceDword((LID)51708U, (uint)context.ClientType);
				throw new ExExceptionNoSupport((LID)45564U, string.Format("GetPropertiesAll for MapiLogon is not currently supported on read-only databases (ClientType={0}).", context.ClientType));
			}
		}

		private static void CheckGetPropertiesSpecificConditionsForReadOnlyDatabase(MapiContext context, MapiPropBagBase propertyBag, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags)
		{
			if (RopHandlerBase.IsRetrievingLocalDirectoryEntryIdFromMapiLogon(propertyBag, propertyTags))
			{
				DiagnosticContext.TraceDword((LID)61948U, (uint)context.ClientType);
				throw new ExExceptionNoSupport((LID)37372U, string.Format("GetPropertiesSpecific for LocalDirectoryEntryId from MapiLogon is not currently supported on read-only databases (ClientType={0}).", context.ClientType));
			}
		}

		private static bool IsRetrievingLocalDirectoryEntryIdFromMapiLogon(MapiPropBagBase propertyBag, IList<PropertyTag> propertyTags)
		{
			if (propertyBag.MapiObjectType == MapiObjectType.Logon && propertyTags != null)
			{
				foreach (PropertyTag propertyTag in propertyTags)
				{
					if (propertyTag.PropertyId == (PropertyId)13334)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private static ErrorCode ErrorToThrowForSessionTermination(ErrorCode error, RopResult result)
		{
			if (error == ErrorCode.None || error == (ErrorCode)2147500037U)
			{
				error = (ErrorCode)2147746069U;
				if (result != null && result.ErrorCode != ErrorCode.None)
				{
					error = result.ErrorCode;
				}
			}
			return ErrorCode.CreateWithLid((LID)61848U, (ErrorCodeValue)error);
		}

		private static bool IsSetReadStatusSharedLock(Folder folder)
		{
			return folder.IsPerUserReadUnreadTrackingEnabled;
		}

		internal void CheckClientTypeIsAllowedOnMoveTarget(MapiLogon logon, RopId ropId, ClientType[] clientTypesAllowedOnMoveTarget)
		{
			if (PropertyBagHelpers.TestPropertyFlags(this.mapiContext, logon.StoreMailbox, PropTag.Mailbox.MailboxFlags, 16, 16))
			{
				bool flag = false;
				foreach (ClientType clientType in clientTypesAllowedOnMoveTarget)
				{
					if (this.mapiContext.ClientType == clientType)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					DiagnosticContext.TraceDword((LID)40104U, (uint)this.mapiContext.ClientType);
					DiagnosticContext.TraceDword((LID)56488U, (uint)ropId);
					throw new ExExceptionNoSupport((LID)44200U, string.Format("Client {0} is not allowed to execute Rop {1} on the target of a mailbox move.", this.mapiContext.ClientType, ropId));
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RopHandlerBase>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
		}

		private void AssertSessionIsNotTerminating(RopId ropId, ErrorCode error, RopResult result)
		{
			if (this.mapiContext.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
			{
				DiagnosticContext.TraceDword((LID)35256U, (uint)ropId);
				throw new StoreException((LID)59448U, RopHandlerBase.ErrorToThrowForSessionTermination(error, result));
			}
		}

		private bool IsGetPropertiesAllSharedMailboxOperation(MapiPropBagBase propertyBag)
		{
			return propertyBag.MapiObjectType != MapiObjectType.Logon;
		}

		private bool IsGetPropertiesSpecificSharedMailboxOperation(MapiPropBagBase propertyBag, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags)
		{
			return !RopHandlerBase.IsRetrievingLocalDirectoryEntryIdFromMapiLogon(propertyBag, propertyTags);
		}

		private bool IsImportReadsSharedMailboxOperation(IcsContentUploadContext uploadContext)
		{
			return this.IsSetReadFlagsSharedMailboxOperation(uploadContext.ParentObject as MapiFolder);
		}

		private bool IsCopyPropertiesSharedMailboxOperation(MapiPropBagBase mapiPropBag)
		{
			return mapiPropBag.CanUseSharedMailboxLockForCopy;
		}

		private bool IsCopyToSharedMailboxOperation(MapiPropBagBase mapiPropBag)
		{
			return mapiPropBag.CanUseSharedMailboxLockForCopy;
		}

		private bool IsSaveChangesAttachmentSharedMailboxOperation(MapiAttachment mapiAttachment)
		{
			return mapiAttachment.CanUseSharedMailboxLockForSave;
		}

		private bool IsSaveChangesMessageSharedMailboxOperation(MapiMessage mapiMessage)
		{
			return mapiMessage.CanUseSharedMailboxLockForSave;
		}

		private bool IsSetReadFlagSharedMailboxOperation(MapiMessage mapiMessage)
		{
			if (mapiMessage.StoreMessage.IsEmbedded)
			{
				return true;
			}
			Folder parentFolder = ((TopMessage)mapiMessage.StoreMessage).ParentFolder;
			return RopHandlerBase.IsSetReadStatusSharedLock(parentFolder);
		}

		private bool IsSetReadFlagsSharedMailboxOperation(MapiFolder mapiFolder)
		{
			return RopHandlerBase.IsSetReadStatusSharedLock(mapiFolder.StoreFolder);
		}

		private bool IsWritePerUserInformationSharedMailboxOperation(bool hasFinished)
		{
			return !hasFinished;
		}

		private bool IsReleaseSharedMailboxOperation(MapiBase serverObject)
		{
			MapiStream mapiStream = serverObject as MapiStream;
			return mapiStream == null || !mapiStream.ReleaseMayNeedExclusiveLock;
		}

		private bool IsPropertyChangeSharedMailboxOperation(MapiPropBagBase propertyBag)
		{
			return propertyBag is MapiAttachment || (propertyBag is MapiMessage && ConfigurationSchema.ConfigurableSharedLockStage.Value >= 5 && this.IsSetReadFlagSharedMailboxOperation((MapiMessage)propertyBag));
		}

		private bool IsDeletePropertiesSharedMailboxOperation(MapiPropBagBase propertyBag)
		{
			return this.IsPropertyChangeSharedMailboxOperation(propertyBag);
		}

		private bool IsDeletePropertiesNoReplicateSharedMailboxOperation(MapiPropBagBase propertyBag)
		{
			return this.IsPropertyChangeSharedMailboxOperation(propertyBag);
		}

		private bool IsSetPropertiesSharedMailboxOperation(MapiPropBagBase propertyBag)
		{
			return this.IsPropertyChangeSharedMailboxOperation(propertyBag) || (ConfigurationSchema.ConfigurableSharedLockStage.Value >= 6 && propertyBag is FastTransferStream);
		}

		private bool IsSetPropertiesNoReplicateSharedMailboxOperation(MapiPropBagBase propertyBag)
		{
			return this.IsSetPropertiesSharedMailboxOperation(propertyBag);
		}

		private bool IsCommitStreamSharedMailboxOperation(MapiStream stream)
		{
			return stream.ParentObject is MapiMessage || stream.ParentObject is MapiAttachment;
		}

		private ThreadManager.MethodFrame CreateThreadManagerMethodFrame(string currentMethodName)
		{
			ThreadManager.ThreadInfo threadInfo;
			ThreadManager.MethodFrame result = ThreadManager.NewMethodFrame(currentMethodName, out threadInfo);
			bool flag = false;
			try
			{
				if (this.mapiContext != null)
				{
					if (this.mapiContext.Session != null)
					{
						threadInfo.Client = this.mapiContext.Session.ApplicationId;
						threadInfo.UserGuid = this.mapiContext.Session.UserGuid;
						threadInfo.User = this.mapiContext.Session.UserDN;
					}
					else
					{
						threadInfo.UserGuid = this.mapiContext.UserIdentity;
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					result.Dispose();
				}
			}
			return result;
		}

		private const ClientType[] AbortSubmitClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] AddressTypesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CloneStreamClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CollapseRowClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CommitStreamClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CopyFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CopyPropertiesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CopyToClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CreateAttachmentClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CreateBookmarkClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CreateFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CreateMessageClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] CreateMessageExtendedClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] DeleteAttachmentClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] DeleteFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] DeleteMessagesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] DeletePropertiesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] DeletePropertiesNoReplicateClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] EmptyFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ExpandRowClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferDestinationCopyOperationConfigureClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferDestinationPutBufferClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferDestinationPutBufferExtendedClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferGetIncrementalStateClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferSourceCopyFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferSourceCopyMessagesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferSourceCopyPropertiesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferSourceCopyToClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferSourceGetBufferClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FastTransferSourceGetBufferExtendedClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FlushRecipientsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] FreeBookmarkClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetAllPerUserLongTermIdsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetCollapseStateClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetContentsTableClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetLocalReplicationIdsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetMessageStatusClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetPerUserGuidClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetPerUserLongTermIdsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetReceiveFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetReceiveFolderTableClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetSearchCriteriaClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] GetStreamSizeClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] HardDeleteMessagesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] HardEmptyFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ImportDeleteClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ImportHierarchyChangeClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ImportMessageChangeClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ImportMessageChangePartialClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ImportMessageMoveClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ImportReadsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] IncrementalConfigClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] LockRegionStreamClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] LogonClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] MoveCopyMessagesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] MoveCopyMessagesExtendedClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] MoveCopyMessagesExtendedWithEntryIdsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] MoveFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] OpenCollectorClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] PrereadMessagesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] QueryColumnsAllClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] QueryNamedPropertiesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] QueryPositionClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ReadPerUserInformationClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ReleaseClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ReloadCachedInformationClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] RemoveAllRecipientsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] ResetTableClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SaveChangesAttachmentClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SaveChangesMessageClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SeekRowClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SeekRowApproximateClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SeekRowBookmarkClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetCollapseStateClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetMessageFlagsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetMessageStatusClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetPropertiesClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetPropertiesNoReplicateClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetReadFlagClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetReadFlagsClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetReceiveFolderClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetSearchCriteriaClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetSizeStreamClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetSpoolerClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SetTransportClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SortTableClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SpoolerLockMessageClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] SubmitMessageClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] TellVersionClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] TransportDeliverMessageClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] TransportDeliverMessage2ClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] TransportDoneWithMessageClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] TransportDuplicateDeliveryCheckClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] TransportNewMailClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] TransportSendClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] UnlockRegionStreamClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] UploadStateStreamBeginClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] UploadStateStreamContinueClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] UploadStateStreamEndClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] WritePerUserInformationClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] WriteStreamClientTypesAllowedOnReadOnlyDatabase = null;

		private const ClientType[] WriteStreamExtendedClientTypesAllowedOnReadOnlyDatabase = null;

		private static readonly ClientType[] AbortSubmitClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] AddressTypesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CloneStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CollapseRowClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CommitStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CopyFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CopyPropertiesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CopyToClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CreateAttachmentClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CreateBookmarkClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CreateFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CreateMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] CreateMessageExtendedClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] DeleteAttachmentClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] DeleteFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] DeleteMessagesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] DeletePropertiesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] DeletePropertiesNoReplicateClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] EmptyFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ExpandRowClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferDestinationCopyOperationConfigureClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferDestinationPutBufferClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferDestinationPutBufferExtendedClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferGetIncrementalStateClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferSourceCopyFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferSourceCopyMessagesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferSourceCopyPropertiesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferSourceCopyToClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferSourceGetBufferClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FastTransferSourceGetBufferExtendedClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FindRowClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] FindRowClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] FlushRecipientsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] FreeBookmarkClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetAllPerUserLongTermIdsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetAttachmentTableClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetAttachmentTableClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetCollapseStateClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetContentsTableClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetContentsTableExtendedClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetContentsTableExtendedClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetHierarchyTableClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetHierarchyTableClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static readonly ClientType[] GetIdsFromNamesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetIdsFromNamesClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static readonly ClientType[] GetLocalReplicationIdsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetMessageStatusClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetNamesFromIDsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetNamesFromIDsClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetPerUserGuidClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetPerUserLongTermIdsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetPropertiesAllClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetPropertiesAllClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetPropertiesSpecificClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetPropertiesSpecificClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static readonly ClientType[] GetPropertyListClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetPropertyListClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] GetReceiveFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetReceiveFolderTableClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetSearchCriteriaClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] GetStreamSizeClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] HardDeleteMessagesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] HardEmptyFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] IdFromLongTermIdClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] IdFromLongTermIdClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] ImportDeleteClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ImportHierarchyChangeClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ImportMessageChangeClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ImportMessageChangePartialClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ImportMessageMoveClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ImportReadsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] IncrementalConfigClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] LockRegionStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] LogonClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] LongTermIdFromIdClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] LongTermIdFromIdClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] MoveCopyMessagesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] MoveCopyMessagesExtendedClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] MoveCopyMessagesExtendedWithEntryIdsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] MoveFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] OpenAttachmentClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenAttachmentClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenCollectorClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] OpenEmbeddedMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenEmbeddedMessageClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenFolderClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static readonly ClientType[] OpenMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenMessageClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] OpenStreamClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] PrereadMessagesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] QueryColumnsAllClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] QueryNamedPropertiesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] QueryPositionClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] QueryRowsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] QueryRowsClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static readonly ClientType[] ReadPerUserInformationClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ReadRecipientsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] ReadRecipientsClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] ReadStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] ReadStreamClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] RegisterNotificationClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] RegisterNotificationClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static readonly ClientType[] ReleaseClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ReloadCachedInformationClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] RemoveAllRecipientsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] ResetTableClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] RestrictClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] RestrictClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] SaveChangesAttachmentClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SaveChangesMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SeekRowClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SeekRowApproximateClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SeekRowBookmarkClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SeekStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] SeekStreamClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] SetCollapseStateClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetColumnsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration,
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination
		};

		private static readonly ClientType[] SetColumnsClientTypesAllowedOnReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static readonly ClientType[] SetMessageFlagsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetMessageStatusClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetPropertiesClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetPropertiesNoReplicateClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetReadFlagClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetReadFlagsClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetReceiveFolderClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetSearchCriteriaClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetSizeStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetSpoolerClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SetTransportClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SortTableClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SpoolerLockMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] SubmitMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] TellVersionClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] TransportDeliverMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] TransportDeliverMessage2ClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] TransportDoneWithMessageClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] TransportDuplicateDeliveryCheckClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] TransportNewMailClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] TransportSendClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] UnlockRegionStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] UploadStateStreamBeginClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] UploadStateStreamContinueClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] UploadStateStreamEndClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] WritePerUserInformationClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] WriteStreamClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private static readonly ClientType[] WriteStreamExtendedClientTypesAllowedOnMoveTarget = new ClientType[]
		{
			ClientType.Migration
		};

		private MapiContext mapiContext;
	}
}
