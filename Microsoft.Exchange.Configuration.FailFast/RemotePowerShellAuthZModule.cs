using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.FailFast.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Configuration.FailFast
{
	public class RemotePowerShellAuthZModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.AuthenticateRequest += this.AuthenticateRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void AuthenticateRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[RemotePowerShellAuthModule::AuthenticateRequest] Enter.");
			HttpContext httpContext = HttpContext.Current;
			if (!httpContext.Request.IsAuthenticated || httpContext.User == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[RemotePowerShellAuthModule::AuthenticateRequest] Request is not authenticated.");
				return;
			}
			try
			{
				this.CheckRemotePSEnabledFlag();
			}
			catch (Exception ex)
			{
				ExTraceGlobals.HttpModuleTracer.TraceError((long)this.GetHashCode(), string.Format("[RemotePowerShellAuthModule::AuthenticateRequest] Got an error '{0}' During CheckRemotePSEnabledFlag.", ex.Message));
				HttpLogger.SafeAppendGenericError("AuthenticateRequest", ex.Message, false);
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[RemotePowerShellAuthModule::AuthenticateRequest] Exit.");
		}

		private void CheckRemotePSEnabledFlag()
		{
			int hashCode = this.GetHashCode();
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::CheckRemotePSEnabledFlag] Enter.");
			IPrincipal user = HttpContext.Current.User;
			if (user != null)
			{
				PartitionId partitionId = null;
				string text;
				SecurityIdentifier userSid = this.GetUserSid(user, out partitionId, out text);
				if (userSid != null)
				{
					bool rpsEnabledFlag;
					if (!RemotePowerShellAuthZModule.rpsEnableCache.TryGetValue(userSid, out rpsEnabledFlag))
					{
						string text2 = null;
						rpsEnabledFlag = this.GetRpsEnabledFlag(userSid, partitionId, out text2);
						RemotePowerShellAuthZModule.rpsEnableCache.InsertAbsolute(userSid, rpsEnabledFlag, RemotePowerShellAuthZModule.timeSpanForRpsEnableFlag, null);
						if (text2 != null)
						{
							HttpLogger.SafeAppendGenericError("CheckRemotePSEnabledFlag", text2, false);
							ExTraceGlobals.HttpModuleTracer.TraceError((long)hashCode, string.Format("[RemotePowerShellAuthModule::CheckRemotePSEnabledFlag] Got an error '{0}' During Find ADRawEntry.", text2));
						}
						else
						{
							HttpLogger.SafeAppendGenericInfo("RpsEnabled", rpsEnabledFlag.ToString());
						}
					}
					else
					{
						HttpLogger.SafeAppendGenericInfo("RpsEnabled", "HitCache");
					}
					if (!rpsEnabledFlag)
					{
						if (!string.IsNullOrEmpty(text))
						{
							this.TriggerFailFast(text);
						}
						this.AccessDenied();
					}
				}
				else
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)hashCode, "Identity in the context is not valid. It should be either LiveIDIdentity or WindowsIdentity.");
				}
			}
			else
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)hashCode, "Current user is null.");
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::CheckRemotePSEnabledFlag] Exit.");
		}

		private SecurityIdentifier GetUserSid(IPrincipal user, out PartitionId partitionId, out string wlID)
		{
			int hashCode = this.GetHashCode();
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::GetUserSid] Enter.");
			SecurityIdentifier result = null;
			partitionId = null;
			wlID = null;
			LiveIDIdentity liveIDIdentity = user.Identity as LiveIDIdentity;
			if (liveIDIdentity != null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)hashCode, "[RemotePowerShellAuthModule::GetUserSid] Current user has a LiveId Identity.");
				result = liveIDIdentity.Sid;
				wlID = liveIDIdentity.MemberName;
				if (!string.IsNullOrEmpty(liveIDIdentity.PartitionId))
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)hashCode, string.Format("[RemotePowerShellAuthModule::GetUserSid] Current user have a PartitionId {0}.", liveIDIdentity.PartitionId));
					PartitionId.TryParse(liveIDIdentity.PartitionId, out partitionId);
				}
			}
			else
			{
				WindowsIdentity windowsIdentity = user.Identity as WindowsIdentity;
				if (windowsIdentity != null)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)hashCode, "[RemotePowerShellAuthModule::GetUserSid] Current user has a Windows Identity.");
					result = windowsIdentity.User;
				}
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::GetUserSid] Exit");
			return result;
		}

		private bool GetRpsEnabledFlag(SecurityIdentifier sid, PartitionId partitionId, out string errorMsg)
		{
			int hashCode = this.GetHashCode();
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::GetRpsEnabledFlag] Enter.");
			bool result = true;
			errorMsg = null;
			try
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)hashCode, "[RemotePowerShellAuthModule::GetRpsEnabledFlag] Find ADRawEntry by Sid.");
				ADRawEntry adrawEntry = UserTokenStaticHelper.GetADRawEntry(partitionId, null, sid);
				if (adrawEntry != null)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)hashCode, "[RemotePowerShellAuthModule::GetRpsEnabledFlag] Find a ADRawEntry.");
					result = (adrawEntry[ADRecipientSchema.RemotePowerShellEnabled] == null || (bool)adrawEntry[ADRecipientSchema.RemotePowerShellEnabled]);
				}
			}
			catch (TransientException ex)
			{
				errorMsg = ex.Message;
			}
			catch (DataSourceOperationException ex2)
			{
				errorMsg = ex2.Message;
			}
			catch (DataValidationException ex3)
			{
				errorMsg = ex3.Message;
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::GetRpsEnabledFlag] Exit.");
			return result;
		}

		private void AccessDenied()
		{
			int hashCode = this.GetHashCode();
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::AccessDenied] Enter.");
			HttpContext httpContext = HttpContext.Current;
			httpContext.Response.StatusCode = 400;
			HttpLogger.SafeAppendGenericError("RemotePowerShellAuthModule", "RemotePowerShell not enabled user.", false);
			ExTraceGlobals.HttpModuleTracer.TraceError<string>((long)hashCode, "[ProxySecurityContextModule::AccessDenied] RpsEnabled is false.", "RemotePowerShell not enabled user.");
			httpContext.Response.Write(string.Format("[FailureCategory={0}] ", FailureCategory.AuthZ + "-RpsNotEnabled") + Strings.ErrorRpsNotEnabled);
			httpContext.Response.End();
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::AccessDenied] Exit.");
		}

		private void TriggerFailFast(string wlID)
		{
			int hashCode = this.GetHashCode();
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::TriggerFailFast] Enter.");
			FailFastUserCache.Instance.AddUserToCache(wlID, BlockedType.NewRequest, TimeSpan.Zero);
			HttpLogger.SafeAppendGenericInfo("TriggerFailFast", wlID);
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)hashCode, "[RemotePowerShellAuthModule::TriggerFailFast] Exit.");
		}

		private static readonly TimeoutCache<SecurityIdentifier, bool> rpsEnableCache = new TimeoutCache<SecurityIdentifier, bool>(20, 5000, false);

		private static readonly TimeSpan timeSpanForRpsEnableFlag = TimeSpan.FromMinutes(10.0);
	}
}
