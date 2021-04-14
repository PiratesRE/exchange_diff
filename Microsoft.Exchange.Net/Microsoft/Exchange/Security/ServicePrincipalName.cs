using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security
{
	internal class ServicePrincipalName
	{
		public static int RegisterServiceClass(string serviceClass)
		{
			return ServicePrincipalName.DsServerRegisterSpn(NativeMethods.SpnWriteOperation.Add, serviceClass, null);
		}

		public static int UnregisterServiceClass(string serviceClass)
		{
			return ServicePrincipalName.DsServerRegisterSpn(NativeMethods.SpnWriteOperation.Delete, serviceClass, null);
		}

		public static int DsServerRegisterSpn(NativeMethods.SpnWriteOperation operation, string serviceClass, string userObjectDN)
		{
			if (string.IsNullOrEmpty(serviceClass))
			{
				throw new ArgumentNullException("serviceClass");
			}
			ExTraceGlobals.DirectoryServicesTracer.TraceDebug<NativeMethods.SpnWriteOperation, string, string>(0L, "DsServerRegisterSpn({0}, {1}, {2})", operation, serviceClass, userObjectDN ?? "(null)");
			int num = NativeMethods.DsServerRegisterSpn(operation, serviceClass, userObjectDN);
			if (num == 0)
			{
				return num;
			}
			string[] spns = null;
			num = ServicePrincipalName.GetFormattedSpns(serviceClass, out spns);
			if (num != 0)
			{
				ExTraceGlobals.DirectoryServicesTracer.TraceError<int>(0L, "Failed in GetFormattedSpns with status {0}", num);
				return num;
			}
			if (string.IsNullOrEmpty(userObjectDN))
			{
				num = ServicePrincipalName.GetComputerObjectDN(out userObjectDN);
				if (num != 0)
				{
					ExTraceGlobals.DirectoryServicesTracer.TraceError<int>(0L, "Failed in GetComputerObjectDN with status {0}", num);
					return num;
				}
			}
			string text = ServicePrincipalName.GetCurrentADDomainFromUserIdentity();
			if (string.IsNullOrEmpty(text))
			{
				text = ComputerInformation.DnsDomainName;
				ExTraceGlobals.DirectoryServicesTracer.TraceDebug(0L, "Could not locate current AD domain from user identity, using registry info");
			}
			ExTraceGlobals.DirectoryServicesTracer.TraceDebug<string>(0L, "Current AD domain is {0}", text);
			if (ServicePrincipalName.InternalDsServerRegisterSpn(operation, spns, text, userObjectDN) == 0)
			{
				return 0;
			}
			WindowsImpersonationContext windowsImpersonationContext = null;
			int result;
			try
			{
				try
				{
					using (AuthenticationContext authenticationContext = new AuthenticationContext())
					{
						SecurityStatus securityStatus = authenticationContext.LogonAsMachineAccount();
						if (securityStatus != SecurityStatus.OK)
						{
							result = (int)securityStatus;
						}
						else
						{
							windowsImpersonationContext = authenticationContext.Identity.Impersonate();
							result = ServicePrincipalName.InternalDsServerRegisterSpn(operation, spns, text, userObjectDN);
						}
					}
				}
				finally
				{
					if (windowsImpersonationContext != null)
					{
						windowsImpersonationContext.Undo();
						windowsImpersonationContext.Dispose();
					}
				}
			}
			catch
			{
				throw;
			}
			return result;
		}

		public override string ToString()
		{
			return this.spn;
		}

		public int FormatSpn(NativeMethods.SpnNameType spnNameType, string serviceClass)
		{
			uint num = 0U;
			this.spn = null;
			SafeSpnArrayHandle safeSpnArrayHandle;
			int num2 = NativeMethods.DsGetSpn(spnNameType, serviceClass, null, 0, 0, null, null, out num, out safeSpnArrayHandle);
			if (num2 != 0)
			{
				return num2;
			}
			if (safeSpnArrayHandle != null && !safeSpnArrayHandle.IsInvalid)
			{
				using (safeSpnArrayHandle)
				{
					if (num2 == 0 && num == 1U)
					{
						string[] spnStrings = safeSpnArrayHandle.GetSpnStrings(num);
						this.spn = spnStrings[0];
					}
					else
					{
						safeSpnArrayHandle.SetCount(num);
					}
				}
			}
			if (!string.IsNullOrEmpty(this.spn))
			{
				return 0;
			}
			return 13;
		}

		public static int GetFormattedSpns(string serviceClass, out string[] spns)
		{
			spns = new string[2];
			int num = 0;
			ServicePrincipalName servicePrincipalName = new ServicePrincipalName();
			int num2 = servicePrincipalName.FormatSpn(NativeMethods.SpnNameType.DnsHost, serviceClass);
			if (num2 != 0)
			{
				ExTraceGlobals.DirectoryServicesTracer.TraceError<string, int>(0L, "Spn.FormatSpn DnsHost failed for {0} because of {1}", serviceClass, num2);
				return num2;
			}
			spns[num++] = servicePrincipalName.ToString();
			num2 = servicePrincipalName.FormatSpn(NativeMethods.SpnNameType.NetbiosHost, serviceClass);
			if (num2 == 0)
			{
				spns[num++] = servicePrincipalName.ToString();
				return num2;
			}
			ExTraceGlobals.DirectoryServicesTracer.TraceError<string, int>(0L, "Spn.FormatSpn NetbiosHost failed for {0} because of {1}", serviceClass, num2);
			return num2;
		}

		private static int InternalDsServerRegisterSpn(NativeMethods.SpnWriteOperation operation, string[] spns, string domain, string userObjectDN)
		{
			if (operation != NativeMethods.SpnWriteOperation.Add && operation != NativeMethods.SpnWriteOperation.Delete)
			{
				throw new NotSupportedException();
			}
			SafeDomainControllerInfoHandle safeDomainControllerInfoHandle;
			uint num = NativeMethods.DsGetDcName(null, domain, null, NativeMethods.DsGetDCNameFlags.WritableRequired, out safeDomainControllerInfoHandle);
			NativeMethods.DomainControllerInformation domainControllerInfo;
			using (safeDomainControllerInfoHandle)
			{
				if (num != 0U)
				{
					ExTraceGlobals.DirectoryServicesTracer.TraceError<uint>(0L, "DsGetDcName failed {0}", num);
					return (int)num;
				}
				domainControllerInfo = safeDomainControllerInfoHandle.GetDomainControllerInfo();
			}
			SafeDsHandle safeDsHandle;
			num = NativeMethods.DsBind(domainControllerInfo.DomainControllerName, null, out safeDsHandle);
			if (num != 0U)
			{
				ExTraceGlobals.DirectoryServicesTracer.TraceError<string, uint>(0L, "DsBind failed to connect to server {0}, the error code: {1} ", domainControllerInfo.DomainControllerName, num);
				return (int)num;
			}
			int result;
			using (safeDsHandle)
			{
				num = NativeMethods.DsWriteAccountSpn(safeDsHandle, operation, userObjectDN, (uint)spns.Length, spns);
				if (num != 0U)
				{
					ExTraceGlobals.DirectoryServicesTracer.TraceError<uint>(0L, "DsWriteAccountSpn failed {0}", num);
				}
				result = (int)num;
			}
			return result;
		}

		private static int GetComputerObjectDN(out string userObjectDN)
		{
			int num = 0;
			userObjectDN = null;
			StringBuilder stringBuilder = new StringBuilder(512);
			int capacity = stringBuilder.Capacity;
			if (!NativeMethods.GetComputerObjectName(NativeMethods.ExtendedNameFormat.FullyQualifiedDN, stringBuilder, ref capacity))
			{
				num = Marshal.GetLastWin32Error();
				if (num == 122)
				{
					stringBuilder.EnsureCapacity(capacity);
					if (!NativeMethods.GetComputerObjectName(NativeMethods.ExtendedNameFormat.FullyQualifiedDN, stringBuilder, ref capacity))
					{
						num = Marshal.GetLastWin32Error();
					}
					else
					{
						num = 0;
					}
				}
				if (num != 0)
				{
					ExTraceGlobals.DirectoryServicesTracer.TraceError<int>(0L, "GetComputerObjectName failed {0}", num);
					return num;
				}
			}
			userObjectDN = stringBuilder.ToString();
			return num;
		}

		private static string GetCurrentADDomainFromUserIdentity()
		{
			string text = null;
			WindowsImpersonationContext windowsImpersonationContext = null;
			string result;
			try
			{
				try
				{
					for (int i = 1; i <= 2; i++)
					{
						if (i == 2)
						{
							using (AuthenticationContext authenticationContext = new AuthenticationContext())
							{
								SecurityStatus securityStatus = authenticationContext.LogonAsMachineAccount();
								if (securityStatus != SecurityStatus.OK)
								{
									ExTraceGlobals.DirectoryServicesTracer.TraceError<SecurityStatus>(0L, "Failed to impersonate machine account with status {0}", securityStatus);
									break;
								}
								if (windowsImpersonationContext != null)
								{
									windowsImpersonationContext.Dispose();
									windowsImpersonationContext = null;
								}
								windowsImpersonationContext = authenticationContext.Identity.Impersonate();
							}
						}
						using (WindowsIdentity current = WindowsIdentity.GetCurrent())
						{
							try
							{
								if (!current.User.IsWellKnown(WellKnownSidType.LocalSystemSid) && !current.User.IsWellKnown(WellKnownSidType.LocalServiceSid) && !current.User.IsWellKnown(WellKnownSidType.NetworkServiceSid))
								{
									ExTraceGlobals.DirectoryServicesTracer.TraceDebug<string>(0L, "Current identity: {0}", current.Name);
									int num = current.Name.IndexOfAny(ServicePrincipalName.domainDelimiters);
									if (num != -1)
									{
										if (current.Name[num] == '\\')
										{
											text = current.Name.Substring(0, num);
										}
										else
										{
											text = current.Name.Substring(num + 1);
										}
									}
									return text;
								}
							}
							catch (SystemException arg)
							{
								ExTraceGlobals.DirectoryServicesTracer.TraceError<SystemException>(0L, "Could not determine username, exception {0}", arg);
								break;
							}
						}
					}
					result = text;
				}
				finally
				{
					if (windowsImpersonationContext != null)
					{
						windowsImpersonationContext.Undo();
						windowsImpersonationContext.Dispose();
					}
				}
			}
			catch
			{
				throw;
			}
			return result;
		}

		private const int ErrorInvalidData = 13;

		private static readonly char[] domainDelimiters = new char[]
		{
			'\\',
			'@'
		};

		private string spn;
	}
}
