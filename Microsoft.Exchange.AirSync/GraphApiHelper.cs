using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.AirSync
{
	internal static class GraphApiHelper
	{
		internal static bool GetDeviceStatus(OrganizationId orgId, MobileDevice device, string externalUserObjectId, out bool isManaged, out bool isCompliant)
		{
			if (device == null)
			{
				isManaged = true;
				isCompliant = true;
				return true;
			}
			isManaged = device.IsManaged;
			isCompliant = device.IsCompliant;
			return !device.IsDisabled;
		}

		internal static bool GetDeviceStatus(OrganizationId orgId, string deviceID, string externalUserObjectId, out bool isManaged, out bool isCompliant)
		{
			bool flag = false;
			bool flag2 = true;
			isManaged = false;
			isCompliant = false;
			try
			{
				AadDevice aadDevice;
				if (GraphApiHelper.deviceStatusCache.Value.TryGetValue(GraphApiHelper.GetCacheKey(externalUserObjectId, deviceID), out aadDevice))
				{
					flag = true;
					isManaged = (aadDevice.IsManaged != null && aadDevice.IsManaged.Value);
					isCompliant = (aadDevice.IsCompliant != null && aadDevice.IsCompliant.Value);
					flag2 = (aadDevice.AccountEnabled == null || aadDevice.AccountEnabled.Value);
					AirSyncDiagnostics.TraceDebug<bool, bool, bool>(ExTraceGlobals.RequestsTracer, 0, "Retrieved device status from Cache. IsManaged:{0}, IsCompliant:{1}, IsEnabled:{2}", isManaged, isCompliant, flag2);
					return flag2;
				}
				if (!GraphApiHelper.aadClients.ContainsKey(orgId))
				{
					lock (GraphApiHelper.lockObject)
					{
						if (!GraphApiHelper.aadClients.ContainsKey(orgId))
						{
							IAadClient aadClient = GraphApiHelper.CreateAadClient(orgId, GraphProxyVersions.Version122);
							if (!GlobalSettings.DisableAadClientCache && aadClient != null)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, 0, "Added AADClient to cache.");
								GraphApiHelper.aadClients.Add(orgId, aadClient);
							}
						}
					}
				}
				IAadClient aadClient2 = GraphApiHelper.aadClients.ContainsKey(orgId) ? GraphApiHelper.aadClients[orgId] : null;
				if (aadClient2 == null)
				{
					throw new InvalidOperationException(string.Format("Failed to create AAD client for Org {0}", orgId));
				}
				List<AadDevice> userDevicesWithEasID = aadClient2.GetUserDevicesWithEasID(deviceID, externalUserObjectId);
				if (userDevicesWithEasID != null && userDevicesWithEasID.Count > 0)
				{
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, 0, "Retrieved {0} devices from AAD ", userDevicesWithEasID.Count);
					userDevicesWithEasID.Sort((AadDevice device1, AadDevice device2) => device1.LastUpdated.CompareTo(device2.LastUpdated));
					aadDevice = userDevicesWithEasID[0];
					isManaged = (aadDevice.IsManaged != null && aadDevice.IsManaged.Value);
					isCompliant = (aadDevice.IsCompliant != null && aadDevice.IsCompliant.Value);
					TimeSpan expiration = (isManaged && isCompliant) ? GlobalSettings.DeviceStatusCacheExpirationInterval : GlobalSettings.NegativeDeviceStatusCacheExpirationInterval;
					if (!GlobalSettings.DisableDeviceHealthStatusCache && !GraphApiHelper.deviceStatusCache.Value.TryAddAbsolute(GraphApiHelper.GetCacheKey(externalUserObjectId, deviceID), aadDevice, expiration))
					{
						AirSyncDiagnostics.TraceDebug<OrganizationId, string, string>(ExTraceGlobals.RequestsTracer, 0, "Failed to Add device {1} to Device Status Cache for user {2}. OrganizationId:{0} ", orgId, deviceID, externalUserObjectId);
					}
					if (Command.CurrentCommand != null)
					{
						Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("num:{0},id:{1}", userDevicesWithEasID.Count, aadDevice.DeviceId));
					}
					return aadDevice.AccountEnabled == null || aadDevice.AccountEnabled.Value;
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, 0, "Device not found in AAD ");
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("msg:{0}", "DeviceNotFoundInAAD"));
				}
			}
			catch (AADException ex)
			{
				AADDataException ex2 = ex as AADDataException;
				string text = (ex2 != null) ? ex2.Code.ToString() : "n/a";
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, "0", "Exception retrieving deviceStatus for device:{0}, user:{1},OrgId:{2},ErrorCode:{3},Message:{4} ", new object[]
				{
					deviceID,
					externalUserObjectId,
					orgId,
					text,
					ex.Message
				});
				if (Command.CurrentCommand != null)
				{
					if (ex2 != null)
					{
						Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, text);
					}
					AirSyncUtility.LogCompressedStackTrace(ex, Command.CurrentCommand.Context);
				}
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, ex, false);
			}
			finally
			{
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("cu:{0}", flag ? "F" : "T"));
					Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("Mngd:{0},Cmpl:{1},Enbld:{2}", isManaged, isCompliant, flag2));
				}
			}
			return flag2;
		}

		internal static bool GetDeviceStatus(OrganizationId orgId, string deviceID, string externalUserObjectId, bool isSupportedDevice, out DeviceAccessState accessState, out DeviceAccessStateReason accessReason)
		{
			bool flag = false;
			bool flag2 = true;
			accessState = DeviceAccessState.Unknown;
			accessReason = DeviceAccessStateReason.Unknown;
			string text = string.Empty;
			try
			{
				if (!GlobalSettings.DisableDeviceHealthStatusCache && GraphApiHelper.deviceComplianceStatusCache.Value.TryGetValue(GraphApiHelper.GetCacheKey(externalUserObjectId, deviceID), out text))
				{
					flag = true;
					flag2 = GraphApiHelper.ParsePolicyEvaluationStatusToAccessState(text, out accessState, out accessReason);
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, 0, "Retrieved device status from Cache. AccessState:{1}, AccessReason:{2}, IsEnabled:{3}, DeviceStatus:{0}, ", new object[]
					{
						accessState,
						accessReason,
						flag2,
						text
					});
					return flag2;
				}
				IAadClient aadClient = null;
				if (!GraphApiHelper.aadClients.ContainsKey(orgId))
				{
					lock (GraphApiHelper.lockObject)
					{
						if (!GraphApiHelper.aadClients.ContainsKey(orgId))
						{
							aadClient = GraphApiHelper.CreateAadClient(orgId, GraphProxyVersions.Version142);
							if (!GlobalSettings.DisableAadClientCache && aadClient != null)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, 0, "Added AADClient to cache.");
								GraphApiHelper.aadClients.Add(orgId, aadClient);
							}
						}
					}
				}
				if (aadClient == null)
				{
					if (!GraphApiHelper.aadClients.ContainsKey(orgId))
					{
						throw new InvalidOperationException(string.Format("Failed to create AAD client for Org {0}", orgId));
					}
					aadClient = GraphApiHelper.aadClients[orgId];
				}
				text = aadClient.EvaluateAuthPolicy(deviceID, externalUserObjectId, isSupportedDevice);
				flag2 = GraphApiHelper.ParsePolicyEvaluationStatusToAccessState(text, out accessState, out accessReason);
				TimeSpan expiration = (accessState == DeviceAccessState.Allowed) ? GlobalSettings.DeviceStatusCacheExpirationInterval : GlobalSettings.NegativeDeviceStatusCacheExpirationInterval;
				if (!GlobalSettings.DisableDeviceHealthStatusCache && !GraphApiHelper.deviceComplianceStatusCache.Value.TryAddAbsolute(GraphApiHelper.GetCacheKey(externalUserObjectId, deviceID), text, expiration))
				{
					AirSyncDiagnostics.TraceDebug<OrganizationId, string, string>(ExTraceGlobals.RequestsTracer, 0, "Failed to Add deviceStatus {1} to Device Status Cache for user {2}. UserId:{0} ", orgId, text, externalUserObjectId);
				}
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("msg:{0}", text));
				}
			}
			catch (AADException ex)
			{
				AADDataException ex2 = ex as AADDataException;
				string text2 = (ex2 != null) ? ex2.Code.ToString() : "n/a";
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, "0", "Exception retrieving deviceStatus for device:{0}, user:{1},OrgId:{2},ErrorCode:{3},Message:{4} ", new object[]
				{
					deviceID,
					externalUserObjectId,
					orgId,
					text2,
					ex.Message
				});
				if (Command.CurrentCommand != null)
				{
					if (ex2 != null)
					{
						Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, text2);
					}
					AirSyncUtility.LogCompressedStackTrace(ex, Command.CurrentCommand.Context);
				}
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, ex, false);
			}
			finally
			{
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("cu:{0}", flag ? "F" : "T"));
					Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.GraphApiCallData, string.Format("deviceStatus", text));
				}
			}
			return flag2;
		}

		internal static void ClearAadClientCache()
		{
			GraphApiHelper.aadClients.Clear();
		}

		private static bool ParsePolicyEvaluationStatusToAccessState(string evalPolicyStatus, out DeviceAccessState accessState, out DeviceAccessStateReason accessReason)
		{
			accessState = DeviceAccessState.Unknown;
			accessReason = DeviceAccessStateReason.ExternallyManaged;
			bool result = true;
			DeviceQueryStatus deviceQueryStatus;
			if (!Enum.TryParse<DeviceQueryStatus>(evalPolicyStatus, out deviceQueryStatus))
			{
				deviceQueryStatus = DeviceQueryStatus.Unknown;
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, 0, "Invalid status returned by graph query {0} ", evalPolicyStatus);
			}
			switch (deviceQueryStatus)
			{
			case DeviceQueryStatus.Success:
				accessState = DeviceAccessState.Allowed;
				break;
			case DeviceQueryStatus.DeviceNotFound:
			case DeviceQueryStatus.DeviceNotManaged:
				accessState = DeviceAccessState.Quarantined;
				accessReason = DeviceAccessStateReason.ExternalEnrollment;
				break;
			case DeviceQueryStatus.DeviceNotCompliant:
				accessState = DeviceAccessState.Quarantined;
				accessReason = DeviceAccessStateReason.ExternalCompliance;
				break;
			case DeviceQueryStatus.DeviceNotEnabled:
				result = false;
				accessState = DeviceAccessState.Blocked;
				break;
			case DeviceQueryStatus.PolicyEvaluationFailure:
				accessState = DeviceAccessState.Blocked;
				break;
			default:
				throw new ArgumentException(string.Format("Invalid device status returned by AAD. {0}", evalPolicyStatus));
			}
			return result;
		}

		private static IAadClient CreateAadClient(OrganizationId organizationId, GraphProxyVersions graphProxyVersion)
		{
			if (TestHooks.GraphApi_GetAadClient != null)
			{
				return TestHooks.GraphApi_GetAadClient(organizationId);
			}
			return AADClientFactory.Create(organizationId, graphProxyVersion);
		}

		private static string GetCacheKey(string userObjectGuid, string easID)
		{
			return string.Format("{0}_{1}", userObjectGuid, easID);
		}

		private static Dictionary<OrganizationId, IAadClient> aadClients = new Dictionary<OrganizationId, IAadClient>();

		private static Lazy<ExactTimeoutCache<string, AadDevice>> deviceStatusCache = new Lazy<ExactTimeoutCache<string, AadDevice>>(() => new ExactTimeoutCache<string, AadDevice>(null, null, null, GlobalSettings.RequestCacheMaxCount, false));

		private static Lazy<ExactTimeoutCache<string, string>> deviceComplianceStatusCache = new Lazy<ExactTimeoutCache<string, string>>(() => new ExactTimeoutCache<string, string>(null, null, null, GlobalSettings.RequestCacheMaxCount, false));

		private static object lockObject = new object();
	}
}
