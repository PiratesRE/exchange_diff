using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class ADDeviceManager
	{
		internal ADDeviceManager(IAirSyncContext context) : this(context.DeviceIdentity, MobileClientType.EAS, context.User.OrganizationId, context.User.Name, context.User.ExchangePrincipal.ObjectId, context.User.ADUser, context.ProtocolLogger, context.User.Budget, ExTraceGlobals.RequestsTracer, AirSyncEventLogConstants.Tuple_UnableToCreateADDevice, AirSyncEventLogConstants.Tuple_DirectoryAccessDenied)
		{
		}

		internal ADDeviceManager(DeviceIdentity deviceIdentity, MobileClientType clientType, OrganizationId organizationId, string userName, ADObjectId userId, ADObject user, ProtocolLogger protocolLogger, IBudget budget, Trace tracer, ExEventLog.EventTuple unableToCreateADDeviceEventTuple, ExEventLog.EventTuple directoryAccessDeniedEventTuple)
		{
			ArgumentValidator.ThrowIfNull("deviceIdentity", deviceIdentity);
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfNull("userName", userName);
			ArgumentValidator.ThrowIfNull("userId", userId);
			ArgumentValidator.ThrowIfNull("user", user);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.deviceIdentity = deviceIdentity;
			this.clientType = clientType;
			this.organizationId = organizationId;
			this.userName = userName;
			this.userId = userId;
			this.user = user;
			this.protocolLogger = protocolLogger;
			this.budget = budget;
			this.tracer = tracer;
			this.unableToCreateADDeviceEventTuple = unableToCreateADDeviceEventTuple;
			this.directoryAccessDeniedEventTuple = directoryAccessDeniedEventTuple;
			this.session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 219, ".ctor", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ADDeviceManager.cs");
			this.session.UseConfigNC = false;
			this.defaultDeviceFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, MobileDeviceSchema.DeviceId, this.deviceIdentity.DeviceId),
				new ComparisonFilter(ComparisonOperator.Equal, MobileDeviceSchema.DeviceType, this.deviceIdentity.DeviceType),
				new ComparisonFilter(ComparisonOperator.Equal, MobileDeviceSchema.ClientType, this.clientType)
			});
		}

		internal MobileDevice GetMobileDevice()
		{
			MobileDevice[] devices = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				devices = this.session.Find<MobileDevice>(MobileDevice.GetRootId(this.userId), QueryScope.OneLevel, this.defaultDeviceFilter, ADDeviceManager.defaultSortOrder, 50);
			});
			if (this.protocolLogger != null)
			{
				this.protocolLogger.SetValue(ProtocolLoggerData.DomainController, this.session.LastUsedDc);
			}
			if (devices.Length == 1)
			{
				return devices[0];
			}
			if (devices.Length == 0)
			{
				return null;
			}
			AirSyncDiagnostics.TraceDebug<string>(this.tracer, this, "User \"{0}\" has more than one device with same device ID and device type.", this.userName);
			if (this.protocolLogger != null)
			{
				this.protocolLogger.SetValue(ProtocolLoggerData.Error, "UserHasMoreThanOneDeviceWithSameDeviceIDandDeviceType-Cleaned");
			}
			return this.CleanMangledObjects(devices);
		}

		internal MobileDevice CreateMobileDevice(GlobalInfo globalInfo, ExDateTime syncStorageCreationTime, bool checkForMaxDevices, MailboxSession mailboxSession = null)
		{
			return this.CreateMobileDevice(globalInfo, syncStorageCreationTime, checkForMaxDevices, true, mailboxSession);
		}

		internal void UpdateMobileDevice(MobileDevice mobileDevice, GlobalInfo globalInfo)
		{
			if (mobileDevice == null)
			{
				throw new ArgumentNullException("mobileDevice");
			}
			GlobalInfo.CopyValuesFromGlobalInfo(mobileDevice, globalInfo);
			try
			{
				if (this.user != null)
				{
					mobileDevice.UserDisplayName = this.user.ToString();
				}
				ADNotificationAdapter.RunADOperation(delegate()
				{
					this.session.Save(mobileDevice);
				});
				if (this.protocolLogger != null)
				{
					this.protocolLogger.SetValue(ProtocolLoggerData.DomainController, this.session.LastUsedDc);
				}
			}
			catch (ADOperationException ex)
			{
				DirectoryOperationException ex2 = ex.InnerException as DirectoryOperationException;
				if (ex2 != null && ex2.Response != null && ex2.Response.ResultCode == ResultCode.InsufficientAccessRights)
				{
					AirSyncDiagnostics.TraceDebug<string, ADOperationException>(this.tracer, this, "Failed to save MobileDevice data to Active Directory for \"{0}\". \r\nException:\r\n{1}", this.userName, ex);
					AirSyncDiagnostics.LogEvent(this.directoryAccessDeniedEventTuple, new string[]
					{
						this.userName
					});
					throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, ex, false)
					{
						ErrorStringForProtocolLogger = ex2.GetType().FullName + ":ADDeviceSaveAccessDenied"
					};
				}
				throw;
			}
		}

		internal void UpdateMobileDevice(MobileDevice mobileDevice)
		{
			this.UpdateMobileDevice(mobileDevice, null);
		}

		private static string EasDeviceCnString(MobileDevice device)
		{
			return device.DeviceType + '§' + device.DeviceId;
		}

		private static string MowaDeviceCnString(MobileDevice device)
		{
			return string.Concat(new object[]
			{
				"MOWA",
				'§',
				device.DeviceType,
				'§',
				device.DeviceId
			});
		}

		internal bool DeleteMobileDevice(MobileDevice mobileDevice)
		{
			bool result;
			try
			{
				mobileDevice.SetIsReadOnly(false);
				mobileDevice.SetExchangeVersion(mobileDevice.MaximumSupportedExchangeObjectVersion);
				ADNotificationAdapter.RunADOperation(delegate()
				{
					this.session.Delete(mobileDevice);
				});
				result = true;
			}
			catch (ADOperationException ex)
			{
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, ex, false)
				{
					ErrorStringForProtocolLogger = string.Format("ADOperationException1:Message:{0}-Type:{1}", ex.Message.Replace(" ", "+").Substring(0, 20), ex.GetType())
				};
			}
			return result;
		}

		internal ActiveSyncDevices GetActiveSyncDeviceContainer()
		{
			ADDeviceManager.<>c__DisplayClassf CS$<>8__locals1 = new ADDeviceManager.<>c__DisplayClassf();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ExchangeActiveSyncDevices");
			CS$<>8__locals1.containers = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				CS$<>8__locals1.containers = CS$<>8__locals1.<>4__this.session.Find<ActiveSyncDevices>(CS$<>8__locals1.<>4__this.userId, QueryScope.OneLevel, CS$<>8__locals1.filter, ADDeviceManager.defaultSortOrder, 50);
			});
			if (this.protocolLogger != null)
			{
				this.protocolLogger.SetValue(ProtocolLoggerData.DomainController, this.session.LastUsedDc);
			}
			if (CS$<>8__locals1.containers.Length == 1)
			{
				return CS$<>8__locals1.containers[0];
			}
			if (CS$<>8__locals1.containers.Length == 0)
			{
				return null;
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "User \"{0}\" has more than one device container.", this.userName);
			if (this.protocolLogger != null)
			{
				this.protocolLogger.SetValue(ProtocolLoggerData.Error, "TooManyDeviceContainers");
			}
			int num = -1;
			int i = 0;
			while (i < CS$<>8__locals1.containers.Length)
			{
				if (!CS$<>8__locals1.containers[i].Id.Rdn.EscapedName.Equals("ExchangeActiveSyncDevices") || num != -1)
				{
					ActiveSyncDevice[] devices = null;
					ADNotificationAdapter.RunADOperation(delegate()
					{
						devices = CS$<>8__locals1.<>4__this.session.Find<ActiveSyncDevice>(CS$<>8__locals1.containers[i].Id, QueryScope.OneLevel, null, ADDeviceManager.defaultSortOrder, 50);
					});
					if (devices != null)
					{
						ActiveSyncDevice[] devices2 = devices;
						for (int j = 0; j < devices2.Length; j++)
						{
							ActiveSyncDevice device = devices2[j];
							try
							{
								ADNotificationAdapter.RunADOperation(delegate()
								{
									CS$<>8__locals1.<>4__this.session.Delete(device);
								});
							}
							catch (LocalizedException ex)
							{
								AirSyncDiagnostics.TraceError<string, string, string>(ExTraceGlobals.RequestsTracer, this, "Failed to delete device object {0} under CNF-mangled container {1} because: {2}", device.Id.DistinguishedName, CS$<>8__locals1.containers[i].Id.DistinguishedName, ex.Message);
							}
						}
					}
					try
					{
						ADNotificationAdapter.RunADOperation(delegate()
						{
							CS$<>8__locals1.<>4__this.session.Delete(CS$<>8__locals1.containers[i]);
						});
						goto IL_244;
					}
					catch (LocalizedException ex2)
					{
						AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "Failed to delete CNF-mangled container {0} because: {0}", CS$<>8__locals1.containers[i].Id.DistinguishedName, ex2.Message);
						goto IL_244;
					}
					goto IL_23C;
				}
				goto IL_23C;
				IL_244:
				i++;
				continue;
				IL_23C:
				num = i;
				goto IL_244;
			}
			if (num == -1)
			{
				return this.CreateActiveSyncDeviceContainer(true);
			}
			return CS$<>8__locals1.containers[num];
		}

		private static void ReadStaticADData(ProtocolLogger logger)
		{
			if (!ADDeviceManager.initialized)
			{
				lock (ADDeviceManager.syncLock)
				{
					if (!ADDeviceManager.initialized)
					{
						ITopologyConfigurationSession configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 587, "ReadStaticADData", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ADDeviceManager.cs");
						configSession.UseConfigNC = false;
						configSession.UseGlobalCatalog = true;
						ADGroup exchangeServersGroup = null;
						ADNotificationAdapter.RunADOperation(delegate()
						{
							exchangeServersGroup = configSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.ExSWkGuid, configSession.ConfigurationNamingContext);
						});
						if (exchangeServersGroup == null)
						{
							throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, EASServerStrings.FailedToResolveWellKnownGuid(WellKnownGuid.ExSWkGuid.ToString(), "Exchange Server Security Group"), true)
							{
								ErrorStringForProtocolLogger = "ExSrvGrpSchemaGuidNotFound"
							};
						}
						ADDeviceManager.exchangeServersGroupSid = exchangeServersGroup.Sid;
						if (logger != null)
						{
							logger.SetValue(ProtocolLoggerData.DomainController, configSession.LastUsedDc);
						}
						configSession.UseGlobalCatalog = false;
						ADSchemaClassObject[] objClass = null;
						ADNotificationAdapter.RunADOperation(delegate()
						{
							objClass = configSession.Find<ADSchemaClassObject>(configSession.SchemaNamingContext, QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, ADSchemaObjectSchema.DisplayName, "msExchActiveSyncDevice"), ADDeviceManager.defaultSortOrder, 2);
						});
						if (objClass == null || objClass.Length == 0 || objClass[0].SchemaIDGuid == Guid.Empty)
						{
							throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, EASServerStrings.CannotFindSchemaClassException("msExchActiveSyncDevice", configSession.SchemaNamingContext.DistinguishedName), true);
						}
						if (objClass.Length > 1 && logger != null)
						{
							logger.SetValue(ProtocolLoggerData.Error, "DisplayNameIsNotUniqueForClassMsExchActiveSyncDevice");
						}
						ADDeviceManager.activeSyncDeviceClass = objClass[0].SchemaIDGuid;
						if (logger != null)
						{
							logger.SetValue(ProtocolLoggerData.DomainController, configSession.LastUsedDc);
						}
						ADDeviceManager.initialized = true;
					}
				}
			}
		}

		private ActiveSyncDevices CreateActiveSyncDeviceContainer(bool retryIfFailed)
		{
			ActiveSyncDevices container = new ActiveSyncDevices();
			try
			{
				container.Name = "ExchangeActiveSyncDevices";
				container.SetId(this.userId.GetChildId(container.Name));
				container.OrganizationId = this.organizationId;
				ADNotificationAdapter.RunADOperation(delegate()
				{
					this.session.Save(container);
					this.session.DomainController = container.OriginatingServer;
				});
			}
			catch (ADObjectAlreadyExistsException innerException)
			{
				container = this.GetActiveSyncDeviceContainer();
				if (container == null)
				{
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, innerException, false)
					{
						ErrorStringForProtocolLogger = "ADObjectAlreadyExistsException:ButDevicesContainerDoesNotExist"
					};
				}
			}
			catch (ADOperationException ex)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_UnableToCreateADDevicesContainer, container.Name, new string[]
				{
					this.userId.ToDNString(),
					ex.Message
				});
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, ex, false)
				{
					ErrorStringForProtocolLogger = "ADOperationException1:" + ex.Message
				};
			}
			bool flag = false;
			try
			{
				this.SetActiveSyncDeviceContainerPermissions(container);
				flag = true;
			}
			catch (ADNoSuchObjectException innerException2)
			{
				if (retryIfFailed)
				{
					return this.CreateActiveSyncDeviceContainer(false);
				}
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, innerException2, false)
				{
					ErrorStringForProtocolLogger = "ADNoSuchObjectException:OnDevicesContainerPermsSet"
				};
			}
			catch (ADOperationException ex2)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_UnableToCreateADDevicesContainer, "ExchangeActiveSyncDevices", new string[]
				{
					this.userId.ToDNString(),
					ex2.Message
				});
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, ex2, false)
				{
					ErrorStringForProtocolLogger = "ADOperationException2:" + ex2.Message
				};
			}
			finally
			{
				if (!flag)
				{
					try
					{
						ADNotificationAdapter.RunADOperation(delegate()
						{
							this.session.Delete(container);
						});
					}
					catch (LocalizedException arg)
					{
						AirSyncDiagnostics.TraceError<LocalizedException>(this.tracer, this, "Failed to delete user container {0}", arg);
					}
				}
			}
			return container;
		}

		private MobileDevice CreateMobileDevice(GlobalInfo globalInfo, ExDateTime syncStorageCreationTime, bool checkForMaxDevices, bool retryIfFailed, MailboxSession mailboxSession)
		{
			ActiveSyncDevices activeSyncDevices = this.GetActiveSyncDeviceContainer();
			if (activeSyncDevices == null)
			{
				activeSyncDevices = this.CreateActiveSyncDeviceContainer(true);
			}
			AirSyncDiagnostics.TraceInfo<MobileClientType, DeviceIdentity, string>(ExTraceGlobals.RequestsTracer, null, "ADDeviceManager::CreateMobileDevice - ClientType: {0}, DeviceIdentity: {1}, mailboxSession from: {2}", this.clientType, this.deviceIdentity, (mailboxSession == null) ? "CurrentCommand" : "parameter");
			this.CleanUpOldDevices(mailboxSession ?? Command.CurrentCommand.MailboxSession);
			MobileDevice mobileDevice = this.InternalCreateDevice(globalInfo, syncStorageCreationTime);
			IThrottlingPolicy throttlingPolicy = (this.budget != null) ? this.budget.ThrottlingPolicy : null;
			if (checkForMaxDevices && throttlingPolicy != null)
			{
				if (!throttlingPolicy.EasMaxDevices.IsUnlimited)
				{
					MobileDevice[] easDevices = null;
					ADNotificationAdapter.RunADOperation(delegate()
					{
						easDevices = this.session.Find<MobileDevice>(MobileDevice.GetRootId(this.userId), QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2012), null, 0);
					});
					if (this.protocolLogger != null)
					{
						this.protocolLogger.SetValue(ProtocolLoggerData.DomainController, this.session.LastUsedDc);
					}
					if (easDevices != null && (long)easDevices.Length >= (long)((ulong)throttlingPolicy.EasMaxDevices.Value))
					{
						this.SendMaxDevicesExceededMailIfNeeded(easDevices.Length, throttlingPolicy.EasMaxDevices.Value);
						throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.MaximumDevicesReached, null, false)
						{
							ErrorStringForProtocolLogger = "MaxDevicesExceeded"
						};
					}
				}
				else
				{
					AirSyncDiagnostics.TraceDebug(this.tracer, this, "throttlingPolicy.EasMaxDevices is unlimited. Skipping max devices check.");
				}
			}
			else
			{
				AirSyncDiagnostics.TraceDebug(this.tracer, this, "No throttling policy is found. Skipping max devices check.");
			}
			switch (mobileDevice.ClientType)
			{
			case MobileClientType.EAS:
				mobileDevice.SetId(activeSyncDevices.Id.GetChildId(ADDeviceManager.EasDeviceCnString(mobileDevice)));
				break;
			case MobileClientType.MOWA:
				mobileDevice.SetId(activeSyncDevices.Id.GetChildId(ADDeviceManager.MowaDeviceCnString(mobileDevice)));
				break;
			default:
				throw new PlatformNotSupportedException("New MobileClientType is not supported.");
			}
			try
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					this.session.Save(mobileDevice);
				});
			}
			catch (ADObjectAlreadyExistsException)
			{
				mobileDevice = this.GetMobileDevice();
				if (mobileDevice == null)
				{
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, EASServerStrings.FailedToCreateNewActiveDevice(this.deviceIdentity.DeviceId, this.deviceIdentity.DeviceType, this.userName), true)
					{
						ErrorStringForProtocolLogger = "CreateActiveSyncDevice:ADObjectAlreadyExistsException"
					};
				}
				if (globalInfo != null)
				{
					this.UpdateMobileDevice(mobileDevice, globalInfo);
				}
			}
			catch (ADOperationException ex)
			{
				DirectoryOperationException ex2 = ex.InnerException as DirectoryOperationException;
				if (retryIfFailed)
				{
					if (ex.ErrorCode != 5)
					{
						if (ex2 == null || ex2.Response == null || ex2.Response.ResultCode != ResultCode.InsufficientAccessRights)
						{
							goto IL_308;
						}
					}
					try
					{
						this.SetActiveSyncDeviceContainerPermissions(activeSyncDevices);
					}
					catch (ADOperationException ex3)
					{
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, EASServerStrings.FailedToApplySecurityToContainer(activeSyncDevices.DistinguishedName), ex3, true)
						{
							ErrorStringForProtocolLogger = "SetEASDevContainerPerms:ADOperationException:" + ex3.Message
						};
					}
					return this.CreateMobileDevice(globalInfo, syncStorageCreationTime, checkForMaxDevices, false, mailboxSession);
				}
				IL_308:
				AirSyncDiagnostics.LogEvent(this.unableToCreateADDeviceEventTuple, new string[]
				{
					mobileDevice.DeviceType,
					mobileDevice.DeviceId,
					activeSyncDevices.Id.ToDNString(),
					ex.Message
				});
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerErrorRetryLater, ex, false)
				{
					ErrorStringForProtocolLogger = "CreateActiveSyncDevice:ADOperationException" + ex.Message
				};
			}
			return mobileDevice;
		}

		private void CleanUpOldDevices(MailboxSession mailboxSession)
		{
			int num = 0;
			try
			{
				IThrottlingPolicy throttlingPolicy = (this.budget != null) ? this.budget.ThrottlingPolicy : null;
				if (throttlingPolicy == null || throttlingPolicy.EasMaxInactivityForDeviceCleanup.IsUnlimited || GlobalSettings.MaxNoOfPartnershipToAutoClean == 0)
				{
					AirSyncDiagnostics.TraceInfo<string, int>(ExTraceGlobals.RequestsTracer, null, "No cleanUp required for stale devices. Reason:{0}, MaxNoOfPartnershipsToAutoClean:{1}", (throttlingPolicy == null) ? "No throttling policy set." : "EasMaxInactivityForDeviceCleanup set to Unlimited.", GlobalSettings.MaxNoOfPartnershipToAutoClean);
				}
				else
				{
					TimeSpan timeSpan = TimeSpan.FromDays(throttlingPolicy.EasMaxInactivityForDeviceCleanup.Value);
					ExDateTime utcNow = ExDateTime.UtcNow;
					MobileDevice[] easDevices = null;
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						easDevices = this.session.Find<MobileDevice>(MobileDevice.GetRootId(this.userId), QueryScope.OneLevel, null, null, 0);
					});
					if (!adoperationResult.Succeeded)
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation during . Message:{0}", adoperationResult.Exception.Message);
					}
					else
					{
						if (easDevices != null)
						{
							List<MobileDevice> list = new List<MobileDevice>(easDevices);
							if (!this.CleanUpMangledDevices(list, out num))
							{
								goto IL_271;
							}
							using (List<MobileDevice>.Enumerator enumerator = list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									MobileDevice device = enumerator.Current;
									if (device.WhenChangedUTC == null || !(ExDateTime.UtcNow.Subtract(new ExDateTime(ExTimeZone.UtcTimeZone, device.WhenChangedUTC.Value)) < timeSpan))
									{
										if (DeviceInfo.CleanUpMobileDevice(mailboxSession, DeviceIdentity.FromMobileDevice(device), timeSpan))
										{
											AirSyncDiagnostics.TraceDebug<MobileClientType, string, string>(ExTraceGlobals.RequestsTracer, null, "Delete device from AD as it doesn't exist in sync state. ClientType :{0} , DeviceType:{1}, DeviceId:{2}", device.ClientType, device.DeviceType, device.DeviceId);
											adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
											{
												this.session.Delete(device);
											});
											if (!adoperationResult.Succeeded)
											{
												AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation during . Message:{0}", adoperationResult.Exception.Message);
											}
											num++;
										}
										TimeSpan t = ExDateTime.UtcNow.Subtract(utcNow);
										if (num >= GlobalSettings.MaxNoOfPartnershipToAutoClean || t >= GlobalSettings.MaxCleanUpExecutionTime)
										{
											AirSyncDiagnostics.TraceDebug<int, double>(ExTraceGlobals.RequestsTracer, null, "Done Cleaning up stale devices. DevicesCleaned:{0}, ExecutionTime(in ms):{1}", num, t.TotalMilliseconds);
											break;
										}
									}
								}
								goto IL_271;
							}
						}
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "No devices in AD for current user.");
						IL_271:;
					}
				}
			}
			finally
			{
				if (this.clientType != MobileClientType.MOWA && Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.NoOfDevicesRemoved, num);
				}
			}
		}

		private bool CleanUpMangledDevices(List<MobileDevice> mobileDevices, out int noOfDevicesRemoved)
		{
			noOfDevicesRemoved = 0;
			ExDateTime utcNow = ExDateTime.UtcNow;
			for (int i = mobileDevices.Count - 1; i >= 0; i--)
			{
				MobileDevice device = mobileDevices[i];
				if (ADDeviceManager.DnIsMangled(device.Name) || ADDeviceManager.DnIsMangled(device.DeviceId))
				{
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						this.session.Delete(device);
					});
					if (!adoperationResult.Succeeded)
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation during . Message:{0}", adoperationResult.Exception.Message);
					}
					noOfDevicesRemoved++;
					mobileDevices.RemoveAt(i);
				}
				TimeSpan t = ExDateTime.UtcNow.Subtract(utcNow);
				if (t >= GlobalSettings.MaxCleanUpExecutionTime)
				{
					AirSyncDiagnostics.TraceDebug<int, double>(ExTraceGlobals.RequestsTracer, null, "Done Cleaning up stale devices. DevicesCleaned:{0}, ExecutionTime(in ms):{1}", noOfDevicesRemoved, t.TotalMilliseconds);
					return false;
				}
			}
			return true;
		}

		private void SetActiveSyncDeviceContainerPermissions(ActiveSyncDevices container)
		{
			ADDeviceManager.ReadStaticADData(this.protocolLogger);
			RawSecurityDescriptor rawSecurityDescriptor = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				rawSecurityDescriptor = this.session.ReadSecurityDescriptor(container.Id);
			});
			if (rawSecurityDescriptor == null)
			{
				if (this.protocolLogger != null)
				{
					this.protocolLogger.SetValue(ProtocolLoggerData.Error, "ADObjectWithNoSecurityDescriptor");
				}
				AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, EASServerStrings.NullNTSD(container.Id.DistinguishedName), true);
				throw ex;
			}
			AirSyncDiagnostics.TraceDebug<string>(this.tracer, this, "Setting ACL on device container for user \"{0}\".", this.userName);
			ActiveDirectorySecurity acl = new ActiveDirectorySecurity();
			byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
			rawSecurityDescriptor.GetBinaryForm(array, 0);
			acl.SetSecurityDescriptorBinaryForm(array);
			acl.AddAccessRule(new ActiveDirectoryAccessRule(ADDeviceManager.exchangeServersGroupSid, ActiveDirectoryRights.CreateChild | ActiveDirectoryRights.DeleteChild | ActiveDirectoryRights.ListChildren | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ADDeviceManager.activeSyncDeviceClass, ActiveDirectorySecurityInheritance.None));
			acl.AddAccessRule(new ActiveDirectoryAccessRule(ADDeviceManager.exchangeServersGroupSid, ActiveDirectoryRights.Delete | ActiveDirectoryRights.ReadProperty | ActiveDirectoryRights.WriteProperty | ActiveDirectoryRights.ListObject, AccessControlType.Allow, ActiveDirectorySecurityInheritance.Children, ADDeviceManager.activeSyncDeviceClass));
			ADNotificationAdapter.RunADOperation(delegate()
			{
				this.session.SaveSecurityDescriptor(container, new RawSecurityDescriptor(acl.GetSecurityDescriptorBinaryForm(), 0));
			});
		}

		private MobileDevice InternalCreateDevice(GlobalInfo globalInfo, ExDateTime syncStorageCreationTime)
		{
			MobileDevice mobileDevice = new MobileDevice();
			mobileDevice.FirstSyncTime = new DateTime?((DateTime)syncStorageCreationTime.ToUtc());
			mobileDevice.ClientType = this.clientType;
			mobileDevice.DeviceId = this.deviceIdentity.DeviceId;
			mobileDevice.DeviceType = this.deviceIdentity.DeviceType;
			mobileDevice.OrganizationId = this.organizationId;
			if (mobileDevice.ClientType == MobileClientType.EAS)
			{
				mobileDevice.SetExchangeVersion(this.exchangeVersion);
			}
			AirSyncDiagnostics.TraceDebug<string, OrganizationId>(this.tracer, this, "Creating device with DeviceId {0} and OrganizationId {1}.", mobileDevice.DeviceId, mobileDevice.OrganizationId);
			if (this.user != null)
			{
				mobileDevice.UserDisplayName = this.user.ToString();
			}
			GlobalInfo.CopyValuesFromGlobalInfo(mobileDevice, globalInfo);
			return mobileDevice;
		}

		private void SendMaxDevicesExceededMailIfNeeded(int deviceCount, uint maxDevicesLimit)
		{
			if (Command.CurrentCommand == null)
			{
				return;
			}
			MailboxSession mailboxSession = Command.CurrentCommand.MailboxSession;
			using (SyncStateRootStorage orCreateSyncStateRootStorage = SyncStateRootStorage.GetOrCreateSyncStateRootStorage(mailboxSession, "AirSync", null))
			{
				if (orCreateSyncStateRootStorage == null)
				{
					AirSyncDiagnostics.TraceError(this.tracer, null, "Error: Could not load SyncStateRootStorage!");
				}
				else
				{
					using (AirSyncRootInfo airSyncRootInfo = AirSyncRootInfo.LoadFromMailbox(mailboxSession, orCreateSyncStateRootStorage))
					{
						if (airSyncRootInfo == null)
						{
							AirSyncDiagnostics.TraceError(this.tracer, null, "Error: Could not load AirSyncRootInfo!");
						}
						else if (airSyncRootInfo.LastMaxDevicesExceededMailSentTime == null || airSyncRootInfo.LastMaxDevicesExceededMailSentTime.Value.AddHours(24.0) < ExDateTime.UtcNow)
						{
							CultureInfo preferedCulture = mailboxSession.PreferedCulture;
							SystemMessageHelper.PostMessage(mailboxSession, Strings.MaxDevicesExceededMailSubject.ToString(preferedCulture), Strings.MaxDevicesExceededMailBody(deviceCount, maxDevicesLimit).ToString(preferedCulture), null, Importance.High);
							airSyncRootInfo.LastMaxDevicesExceededMailSentTime = new ExDateTime?(ExDateTime.UtcNow);
							airSyncRootInfo.SaveToMailbox();
						}
					}
				}
			}
		}

		private MobileDevice CleanMangledObjects(MobileDevice[] devices)
		{
			ADDeviceManager.<>c__DisplayClass3b CS$<>8__locals1 = new ADDeviceManager.<>c__DisplayClass3b();
			CS$<>8__locals1.devices = devices;
			CS$<>8__locals1.<>4__this = this;
			int num = -1;
			for (int i = 0; i < CS$<>8__locals1.devices.Length; i++)
			{
				if (!this.DnIsMangled(CS$<>8__locals1.devices[i]))
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				num = 0;
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "All device objects are CNF-mangled. Using {0}", CS$<>8__locals1.devices[0].Id.DistinguishedName);
			}
			int idx;
			for (idx = 0; idx < CS$<>8__locals1.devices.Length; idx++)
			{
				if (idx != num)
				{
					try
					{
						ADNotificationAdapter.RunADOperation(delegate()
						{
							CS$<>8__locals1.<>4__this.session.Delete(CS$<>8__locals1.devices[idx]);
						});
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Deleted CNF-mangled device object: {0}", CS$<>8__locals1.devices[idx].Id.DistinguishedName);
					}
					catch (LocalizedException ex)
					{
						AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "Failed to delete CNF-mangled device object {0} because: {1}", CS$<>8__locals1.devices[idx].Id.DistinguishedName, ex.Message);
					}
				}
			}
			return CS$<>8__locals1.devices[num];
		}

		private bool DnIsMangled(MobileDevice device)
		{
			string escapedName = device.Id.Rdn.EscapedName;
			string value = ADDeviceManager.EasDeviceCnString(device);
			if (!escapedName.Equals(value, StringComparison.Ordinal))
			{
				string value2 = ADDeviceManager.MowaDeviceCnString(device);
				if (!escapedName.Equals(value2, StringComparison.Ordinal))
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Found mangled device object DN: {0}", device.Id.DistinguishedName);
					return true;
				}
			}
			return false;
		}

		internal static bool DnIsMangled(string deviceId)
		{
			return deviceId.Contains("\n");
		}

		private const string DefaultDeviceContainerName = "ExchangeActiveSyncDevices";

		private const int MaxDevicesToReturn = 50;

		private static SortBy defaultSortOrder = new SortBy(ADObjectSchema.WhenCreated, SortOrder.Ascending);

		private static object syncLock = new object();

		private static bool initialized;

		private static Guid activeSyncDeviceClass = Guid.Empty;

		private static SecurityIdentifier exchangeServersGroupSid;

		private readonly QueryFilter defaultDeviceFilter;

		private readonly ADObjectId userId;

		private readonly DeviceIdentity deviceIdentity;

		private readonly MobileClientType clientType;

		private readonly ExchangeObjectVersion exchangeVersion = ExchangeObjectVersion.Exchange2010;

		private readonly OrganizationId organizationId;

		private readonly string userName;

		private readonly ADObject user;

		private readonly ProtocolLogger protocolLogger;

		private readonly IBudget budget;

		private readonly Trace tracer;

		private readonly ExEventLog.EventTuple unableToCreateADDeviceEventTuple;

		private readonly ExEventLog.EventTuple directoryAccessDeniedEventTuple;

		private IConfigurationSession session;
	}
}
