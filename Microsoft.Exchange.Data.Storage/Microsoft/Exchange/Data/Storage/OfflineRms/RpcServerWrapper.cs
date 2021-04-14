using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.OfflineRms;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcServerWrapper : OfflineRmsRpcServer
	{
		public override byte[] AcquireTenantLicenses(int version, byte[] inputParameterBytes)
		{
			XmlNode[] rac = null;
			XmlNode[] clc = null;
			AcquireTenantLicensesRpcParameters acquireTenantLicensesRpcParameters = null;
			byte[] result;
			try
			{
				acquireTenantLicensesRpcParameters = new AcquireTenantLicensesRpcParameters(inputParameterBytes);
				ServerManager.AcquireTenantLicenses(acquireTenantLicensesRpcParameters.ClientManagerContext, acquireTenantLicensesRpcParameters.MachineCertificateChain, acquireTenantLicensesRpcParameters.Identity, out rac, out clc);
				AcquireTenantLicensesRpcResults acquireTenantLicensesRpcResults = new AcquireTenantLicensesRpcResults(new OverallRpcResult(null), rac, clc);
				result = acquireTenantLicensesRpcResults.Serialize();
			}
			catch (Exception ex)
			{
				if (!(ex is RightsManagementServerException))
				{
					ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcServerWrapper, ServerManagerLog.EventType.Error, (acquireTenantLicensesRpcParameters != null) ? acquireTenantLicensesRpcParameters.ClientManagerContext : null, string.Format("ServerManager.AcquireTenantLicenses has thrown unhandled exception {0}", ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
					ExWatson.SendReportAndCrashOnAnotherThread(ex);
				}
				AcquireTenantLicensesRpcResults acquireTenantLicensesRpcResults2 = new AcquireTenantLicensesRpcResults(new OverallRpcResult(ex), Array<XmlNode>.Empty, Array<XmlNode>.Empty);
				result = acquireTenantLicensesRpcResults2.Serialize();
			}
			return result;
		}

		public override byte[] AcquireUseLicenses(int version, byte[] inputParameterBytes)
		{
			AcquireUseLicensesRpcParameters acquireUseLicensesRpcParameters = null;
			byte[] result;
			try
			{
				acquireUseLicensesRpcParameters = new AcquireUseLicensesRpcParameters(inputParameterBytes);
				UseLicenseResult[] originalResults = ServerManager.AcquireUseLicenses(acquireUseLicensesRpcParameters.ClientManagerContext, acquireUseLicensesRpcParameters.RightsAccountCertificate, acquireUseLicensesRpcParameters.IssuanceLicense, acquireUseLicensesRpcParameters.LicenseeIdentities);
				AcquireUseLicensesRpcResults acquireUseLicensesRpcResults = new AcquireUseLicensesRpcResults(new OverallRpcResult(null), originalResults);
				result = acquireUseLicensesRpcResults.Serialize();
			}
			catch (Exception ex)
			{
				if (!(ex is RightsManagementServerException))
				{
					ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcServerWrapper, ServerManagerLog.EventType.Error, (acquireUseLicensesRpcParameters != null) ? acquireUseLicensesRpcParameters.ClientManagerContext : null, string.Format("ServerManager.AcquireTenantLicenses has thrown unhandled exception {0}", ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
					ExWatson.SendReportAndCrashOnAnotherThread(ex);
				}
				AcquireUseLicensesRpcResults acquireUseLicensesRpcResults2 = new AcquireUseLicensesRpcResults(new OverallRpcResult(ex), Array<UseLicenseResult>.Empty);
				result = acquireUseLicensesRpcResults2.Serialize();
			}
			return result;
		}

		public override byte[] GetTenantActiveCryptoMode(int version, byte[] inputParameterBytes)
		{
			GetTenantActiveCryptoModeRpcParameters getTenantActiveCryptoModeRpcParameters = null;
			byte[] result;
			try
			{
				getTenantActiveCryptoModeRpcParameters = new GetTenantActiveCryptoModeRpcParameters(inputParameterBytes);
				int tenantActiveCryptoMode = ServerManager.GetTenantActiveCryptoMode(getTenantActiveCryptoModeRpcParameters.ClientManagerContext);
				ActiveCryptoModeResult[] originalResults = new ActiveCryptoModeResult[]
				{
					new ActiveCryptoModeResult(tenantActiveCryptoMode, null)
				};
				GetTenantActiveCryptoModeRpcResults getTenantActiveCryptoModeRpcResults = new GetTenantActiveCryptoModeRpcResults(new OverallRpcResult(null), originalResults);
				result = getTenantActiveCryptoModeRpcResults.Serialize();
			}
			catch (Exception ex)
			{
				if (!(ex is RightsManagementServerException))
				{
					ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcServerWrapper, ServerManagerLog.EventType.Error, (getTenantActiveCryptoModeRpcParameters != null) ? getTenantActiveCryptoModeRpcParameters.ClientManagerContext : null, string.Format("ServerManager.GetTenantActiveCryptoMode has thrown unhandled exception {0}", ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
					ExWatson.SendReportAndCrashOnAnotherThread(ex);
				}
				GetTenantActiveCryptoModeRpcResults getTenantActiveCryptoModeRpcResults2 = new GetTenantActiveCryptoModeRpcResults(new OverallRpcResult(ex), Array<ActiveCryptoModeResult>.Empty);
				result = getTenantActiveCryptoModeRpcResults2.Serialize();
			}
			return result;
		}
	}
}
