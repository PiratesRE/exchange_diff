using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.SACLWatcher.Messages;

namespace Microsoft.Exchange.Servicelets.SACLWatcher
{
	public class Servicelet : Servicelet
	{
		public override void Work()
		{
			SecurityIdentifier exsSid = null;
			ADDomain dom = null;
			Exception ex = null;
			try
			{
				dom = ADForest.GetLocalForest().FindLocalDomain();
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (DataValidationException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorNoLocalDomain, string.Empty, new object[0]);
				return;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 108, "Work", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\SACLWatcher\\program\\SACLWatcherServicelet.cs");
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 114, "Work", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\SACLWatcher\\program\\SACLWatcherServicelet.cs");
			try
			{
				exsSid = Servicelet.GetSidForExchangeKnownGuid(tenantOrRootOrgRecipientSession, WellKnownGuid.ExSWkGuid, tenantOrTopologyConfigurationSession.ConfigurationNamingContext.DistinguishedName);
			}
			catch (ErrorExchangeGroupNotFoundException)
			{
				this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorExchangeGroupNotFound, string.Empty, new object[]
				{
					WellKnownGuid.ExSWkGuid
				});
				return;
			}
			do
			{
				this.VerifyAndRecoverSaclRight(dom, exsSid);
			}
			while (!base.StopEvent.WaitOne(this.sleepInterval, false));
		}

		private bool TryVerifyAccountPrivilegeOnDomainController(ADDomain domain, SecurityIdentifier sid, string privilege, out bool privilegeFound)
		{
			SafeLsaPolicyHandle safeLsaPolicyHandle = null;
			LsaNativeMethods.SafeLsaUnicodeString safeLsaUnicodeString = null;
			SafeLsaMemoryHandle safeLsaMemoryHandle = null;
			privilegeFound = false;
			bool result;
			try
			{
				if (string.IsNullOrEmpty(domain.OriginatingServer))
				{
					this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorNoDomainController, string.Empty, new object[]
					{
						domain.Name
					});
					result = false;
				}
				else
				{
					string originatingServer = domain.OriginatingServer;
					LsaNativeMethods.LsaObjectAttributes objectAttributes = new LsaNativeMethods.LsaObjectAttributes();
					safeLsaUnicodeString = new LsaNativeMethods.SafeLsaUnicodeString(originatingServer);
					int ntstatus = LsaNativeMethods.LsaOpenPolicy(safeLsaUnicodeString, objectAttributes, LsaNativeMethods.PolicyAccess.LookupNames, out safeLsaPolicyHandle);
					int num = LsaNativeMethods.LsaNtStatusToWinError(ntstatus);
					if (num != 0)
					{
						if (5 != num)
						{
							this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorOpenPolicyFailed, string.Empty, new object[]
							{
								originatingServer,
								domain.Name,
								(uint)num
							});
						}
						result = false;
					}
					else
					{
						byte[] array = new byte[sid.BinaryLength];
						sid.GetBinaryForm(array, 0);
						int num2 = 0;
						ntstatus = LsaNativeMethods.LsaEnumerateAccountRights(safeLsaPolicyHandle, array, out safeLsaMemoryHandle, out num2);
						num = LsaNativeMethods.LsaNtStatusToWinError(ntstatus);
						if (num != 0)
						{
							if (2 == num)
							{
								privilegeFound = false;
								result = true;
							}
							else
							{
								if (5 != num)
								{
									this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorEnumerateRightsFailed, string.Empty, new object[]
									{
										sid.ToString(),
										(uint)num
									});
								}
								result = false;
							}
						}
						else
						{
							long num3 = safeLsaMemoryHandle.DangerousGetHandle().ToInt64();
							for (int i = 0; i < num2; i++)
							{
								LsaNativeMethods.LsaUnicodeString lsaUnicodeString = (LsaNativeMethods.LsaUnicodeString)Marshal.PtrToStructure(new IntPtr(num3), typeof(LsaNativeMethods.LsaUnicodeString));
								if (string.Compare(lsaUnicodeString.Value, privilege, StringComparison.OrdinalIgnoreCase) == 0)
								{
									privilegeFound = true;
									break;
								}
								num3 += (long)Marshal.SizeOf(lsaUnicodeString);
							}
							result = true;
						}
					}
				}
			}
			finally
			{
				if (safeLsaUnicodeString != null)
				{
					safeLsaUnicodeString.Dispose();
				}
				if (safeLsaMemoryHandle != null)
				{
					safeLsaMemoryHandle.Dispose();
				}
				if (safeLsaPolicyHandle != null)
				{
					safeLsaPolicyHandle.Dispose();
				}
			}
			return result;
		}

		private bool TryAddAccountPrivilegeToDomainController(ADDomain domain, SecurityIdentifier sid, string privilege)
		{
			SafeLsaPolicyHandle safeLsaPolicyHandle = null;
			LsaNativeMethods.SafeLsaUnicodeString safeLsaUnicodeString = null;
			LsaNativeMethods.SafeLsaUnicodeString safeLsaUnicodeString2 = null;
			if (string.IsNullOrEmpty(domain.OriginatingServer))
			{
				this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorNoDomainController, string.Empty, new object[]
				{
					domain.Name
				});
				return false;
			}
			bool result;
			try
			{
				string originatingServer = domain.OriginatingServer;
				LsaNativeMethods.LsaObjectAttributes objectAttributes = new LsaNativeMethods.LsaObjectAttributes();
				safeLsaUnicodeString2 = new LsaNativeMethods.SafeLsaUnicodeString(originatingServer);
				int ntstatus = LsaNativeMethods.LsaOpenPolicy(safeLsaUnicodeString2, objectAttributes, LsaNativeMethods.PolicyAccess.AllAccess, out safeLsaPolicyHandle);
				int num = LsaNativeMethods.LsaNtStatusToWinError(ntstatus);
				if (num != 0)
				{
					if (5 != num)
					{
						this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorOpenPolicyFailed, string.Empty, new object[]
						{
							originatingServer,
							domain.Name,
							(uint)num
						});
					}
					result = false;
				}
				else
				{
					byte[] array = new byte[sid.BinaryLength];
					sid.GetBinaryForm(array, 0);
					safeLsaUnicodeString = new LsaNativeMethods.SafeLsaUnicodeString(privilege);
					LsaNativeMethods.LsaUnicodeStringStruct[] userRights = new LsaNativeMethods.LsaUnicodeStringStruct[]
					{
						new LsaNativeMethods.LsaUnicodeStringStruct(safeLsaUnicodeString)
					};
					ntstatus = LsaNativeMethods.LsaAddAccountRights(safeLsaPolicyHandle, array, userRights, 1);
					num = LsaNativeMethods.LsaNtStatusToWinError(ntstatus);
					if (num != 0)
					{
						if (5 != num)
						{
							this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorAddAccountRightsFailed, string.Empty, new object[]
							{
								privilege,
								sid.ToString(),
								(uint)num
							});
						}
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			finally
			{
				if (safeLsaUnicodeString2 != null)
				{
					safeLsaUnicodeString2.Dispose();
				}
				if (safeLsaUnicodeString != null)
				{
					safeLsaUnicodeString.Dispose();
				}
				if (safeLsaPolicyHandle != null)
				{
					safeLsaPolicyHandle.Dispose();
				}
			}
			return result;
		}

		private void VerifyAndRecoverSaclRight(ADDomain dom, SecurityIdentifier exsSid)
		{
			bool flag = false;
			if (dom == null || string.IsNullOrEmpty(dom.OriginatingServer))
			{
				this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_ErrorDomainControllerNotFound, string.Empty, new object[0]);
				return;
			}
			if (this.TryVerifyAccountPrivilegeOnDomainController(dom, exsSid, "SeSecurityPrivilege", out flag) && !flag)
			{
				this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_WarningPrivilegeRemoved, string.Empty, new object[]
				{
					"SeSecurityPrivilege",
					exsSid.ToString()
				});
				if (this.TryAddAccountPrivilegeToDomainController(dom, exsSid, "SeSecurityPrivilege"))
				{
					this.EventLog.LogEvent(SACLWatcherEventLogConstants.Tuple_InfoPrivilegeRecovered, string.Empty, new object[]
					{
						"SeSecurityPrivilege",
						exsSid.ToString()
					});
				}
			}
		}

		internal static SecurityIdentifier GetSidForExchangeKnownGuid(IRecipientSession session, Guid knownGuid, string containerDN)
		{
			ADGroup adgroup = session.ResolveWellKnownGuid<ADGroup>(knownGuid, containerDN);
			if (adgroup == null)
			{
				throw new ErrorExchangeGroupNotFoundException(knownGuid);
			}
			return adgroup.Sid;
		}

		private const string EventSource = "MSExchange SACL Watcher";

		private const int WinErrorSuccess = 0;

		private const int WinErrorFileNotFound = 2;

		private const int WinErrorAccessDenied = 5;

		private readonly TimeSpan sleepInterval = TimeSpan.FromMinutes(5.0);

		private static readonly Guid ComponentGuid = new Guid("8A59C35C-1124-4116-BDFD-AE3C25A9EC87");

		private readonly ExEventLog EventLog = new ExEventLog(Servicelet.ComponentGuid, "MSExchange SACL Watcher");
	}
}
