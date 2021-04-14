using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class NativeHelpers : INativeMethods
	{
		internal static IDisposable SetTestHook(INativeMethods wrapper)
		{
			return NativeHelpers.hookableInstance.SetTestHook(wrapper);
		}

		public static string GetLocalComputerFqdn(bool throwOnException)
		{
			return NativeHelpers.GetComputerNameHelper(NativeMethods.ComputerNameFormat.ComputerNameDnsFullyQualified);
		}

		public static string GetSiteName(bool throwOnErrorNoSite = false)
		{
			return NativeHelpers.hookableInstance.Value.GetSiteNameHookable(throwOnErrorNoSite);
		}

		public static StringCollection GetDcSiteCoverage(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			StringCollection stringCollection = new StringCollection();
			SafeDsSiteNameHandle safeDsSiteNameHandle = null;
			long num = 0L;
			try
			{
				int num2 = NativeMethods.DsGetDcSiteCoverage(fqdn, out num, out safeDsSiteNameHandle);
				if (num2 != 0)
				{
					throw new CannotGetSiteInfoException(DirectoryStrings.CannotGetSiteInfo(num2.ToString("X")));
				}
				int num3 = 0;
				while ((long)num3 < num)
				{
					stringCollection.Add(Marshal.PtrToStringAuto(Marshal.ReadIntPtr(safeDsSiteNameHandle.DangerousGetHandle(), num3 * IntPtr.Size)));
					num3++;
				}
			}
			finally
			{
				if (safeDsSiteNameHandle != null)
				{
					safeDsSiteNameHandle.Dispose();
				}
			}
			return stringCollection;
		}

		public static string GetForestName()
		{
			return NativeHelpers.hookableInstance.Value.GetForestNameHookable();
		}

		public static string GetDomainName()
		{
			return NativeHelpers.hookableInstance.Value.GetDomainNameHookable();
		}

		public static bool LocalMachineRoleIsDomainController()
		{
			NativeMethods.InteropDsRolePrimaryDomainInfoBasic primaryDomainInformation = NativeHelpers.GetPrimaryDomainInformation(null);
			return primaryDomainInformation.machineRole == NativeMethods.MachineRole.PrimaryDomainController || primaryDomainInformation.machineRole == NativeMethods.MachineRole.BackupDomainController;
		}

		public static string DistinguishedNameFromCanonicalName(string canonicalName)
		{
			return NativeHelpers.DistinguishedNameFromCanonicalName(canonicalName, null);
		}

		public static string DistinguishedNameFromCanonicalName(string canonicalName, ADServerSettings serverSettings)
		{
			if (canonicalName.IndexOf('/') != canonicalName.LastIndexOf('/') && !canonicalName.EndsWith("\\/"))
			{
				canonicalName = canonicalName.TrimEnd(new char[]
				{
					'/'
				});
			}
			if (!canonicalName.Contains("/"))
			{
				canonicalName += "/";
			}
			string text = null;
			uint err = 0U;
			bool flag = NativeHelpers.TryDsCrackNames(canonicalName, ExtendedNameFormat.NameCanonical, ExtendedNameFormat.NameFullyQualifiedDN, serverSettings, out text, out err);
			bool flag2 = ADObjectId.IsValidDistinguishedName(text);
			if (!flag || !flag2)
			{
				throw new NameConversionException(DirectoryStrings.ErrorConversionFailedWithError(canonicalName, err));
			}
			return text;
		}

		public static string CanonicalNameFromDistinguishedName(string distinguishedName)
		{
			string text = null;
			uint err = 0U;
			if (!NativeHelpers.TryDsCrackNames(distinguishedName, ExtendedNameFormat.NameFullyQualifiedDN, ExtendedNameFormat.NameCanonical, null, out text, out err))
			{
				throw new NameConversionException(DirectoryStrings.ErrorConversionFailedWithError(distinguishedName, err));
			}
			if (text.IndexOf('/') == text.Length - 1)
			{
				text = text.TrimEnd(new char[]
				{
					'/'
				});
			}
			return text;
		}

		internal static StringCollection FindAllDomainControllers(string domainFqdn, string siteName = null)
		{
			return NativeHelpers.hookableInstance.Value.FindAllDomainControllersHookable(domainFqdn, siteName);
		}

		internal static StringCollection FindAllGlobalCatalogs(string forestFqdn, string siteName = null)
		{
			return NativeHelpers.hookableInstance.Value.FindAllGlobalCatalogsHookable(forestFqdn, siteName);
		}

		internal static string ResetSecureChannelDCForDomain(string domainFqdn, bool throwOnError = true)
		{
			return NativeHelpers.hookableInstance.Value.ResetSecureChannelDCForDomainHookable(domainFqdn, throwOnError);
		}

		internal static string GetSecureChannelDCForDomain(string domainFqdn, bool throwOnError = true)
		{
			return NativeHelpers.hookableInstance.Value.GetSecureChannelDCForDomainHookable(domainFqdn, throwOnError);
		}

		public static void RemoveDsServer(string serverDN, string serverToUseFqdn, bool fCommit)
		{
			if (fCommit)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>(0L, "Removing server {0} via server {1}", serverDN, serverToUseFqdn);
			}
			else
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>(0L, "Checking for existence of server {0} via server {1}", serverDN, serverToUseFqdn);
			}
			SafeDsBindHandle safeDsBindHandle = null;
			try
			{
				uint num = NativeMethods.DsBind(serverToUseFqdn, null, out safeDsBindHandle);
				if (num != 0U)
				{
					throw new ADExternalException(DirectoryStrings.ExceptionCannotBindToDC(serverToUseFqdn), new Win32Exception((int)num));
				}
				bool flag;
				num = NativeMethods.DsRemoveDsServer(safeDsBindHandle, serverDN, null, out flag, fCommit);
				if (num != 0U)
				{
					throw new ADExternalException(DirectoryStrings.ExceptionCannotRemoveDsServer(serverDN), new Win32Exception((int)num));
				}
			}
			finally
			{
				if (safeDsBindHandle != null)
				{
					safeDsBindHandle.Dispose();
				}
			}
		}

		public static string GetPrimaryDomainInformationFromServer(string serverFqdn)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			NativeMethods.InteropDsRolePrimaryDomainInfoBasic primaryDomainInformation = NativeHelpers.GetPrimaryDomainInformation(serverFqdn);
			return primaryDomainInformation.domainNameDns;
		}

		public static string GetDomainForestNameFromServer(string serverFqdn)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			NativeMethods.InteropDsRolePrimaryDomainInfoBasic primaryDomainInformation = NativeHelpers.GetPrimaryDomainInformation(serverFqdn);
			return primaryDomainInformation.domainForestName;
		}

		private NativeHelpers()
		{
		}

		private static string GetErrorMessageFromNativeError(int nativeError)
		{
			return new Win32Exception(nativeError).Message;
		}

		private static bool TryDsCrackNames(string input, ExtendedNameFormat formatOffered, ExtendedNameFormat formatDesired, ADServerSettings serverSettings, out string result, out uint err)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (formatOffered != ExtendedNameFormat.NameCanonical && formatOffered != ExtendedNameFormat.NameFullyQualifiedDN)
			{
				throw new ArgumentException(DirectoryStrings.ExArgumentException("formatOffered", formatOffered.ToString()), "formatOffered");
			}
			if (formatDesired != ExtendedNameFormat.NameCanonical && formatDesired != ExtendedNameFormat.NameFullyQualifiedDN)
			{
				throw new ArgumentException(DirectoryStrings.ExArgumentException("formatDesired", formatDesired.ToString()), "formatDesired");
			}
			result = null;
			err = 0U;
			SafeDsNameResultHandle safeDsNameResultHandle = null;
			SafeDsBindHandle safeDsBindHandle = null;
			NativeMethods.DsNameFlags dsNameFlags = NativeMethods.DsNameFlags.SyntacticalOnly;
			bool result2;
			try
			{
				if (formatOffered == ExtendedNameFormat.NameCanonical)
				{
					string[] array = input.Split(new char[]
					{
						'/'
					});
					if (array.Length == 0)
					{
						return false;
					}
					if (array.Length > 1 && array[1].Length > 0)
					{
						dsNameFlags = NativeMethods.DsNameFlags.NoFlags;
						string text = array[0];
						ExTraceGlobals.ADTopologyTracer.TraceDebug<string>(0L, "Calling DsBind for domain {0}", text ?? "<null>");
						if (text.Length == 0)
						{
							return false;
						}
						string domainControllerName = null;
						string distinguishedName = NativeHelpers.DistinguishedNameFromCanonicalName(text);
						if (serverSettings == null)
						{
							serverSettings = ADSessionSettings.ExternalServerSettings;
						}
						if (serverSettings != null)
						{
							domainControllerName = serverSettings.GetPreferredDC(new ADObjectId(distinguishedName));
						}
						err = NativeMethods.DsBind(domainControllerName, text, out safeDsBindHandle);
						if (err != 0U)
						{
							return false;
						}
					}
				}
				if (safeDsBindHandle == null)
				{
					safeDsBindHandle = new SafeDsBindHandle();
				}
				ExTraceGlobals.ADTopologyTracer.TraceDebug(0L, "Calling DsCrackNames with input={0}, formatOffered={1}, formatDesired={2}, flags={3}", new object[]
				{
					input,
					formatOffered,
					formatDesired,
					dsNameFlags
				});
				err = NativeMethods.DsCrackNames(safeDsBindHandle, dsNameFlags, formatOffered, formatDesired, 1U, new string[]
				{
					input
				}, out safeDsNameResultHandle);
				if (err != 0U)
				{
					result2 = false;
				}
				else
				{
					NativeMethods.DsNameResult dsNameResult = new NativeMethods.DsNameResult();
					Marshal.PtrToStructure(safeDsNameResultHandle.DangerousGetHandle(), dsNameResult);
					uint cItems = dsNameResult.cItems;
					if (cItems < 1U)
					{
						result2 = false;
					}
					else
					{
						NativeMethods.DsNameResultItem dsNameResultItem = new NativeMethods.DsNameResultItem();
						Marshal.PtrToStructure(dsNameResult.rItems, dsNameResultItem);
						if (dsNameResultItem.status != 0)
						{
							err = (uint)dsNameResultItem.status;
							result2 = false;
						}
						else if (dsNameResultItem.name == null)
						{
							result2 = false;
						}
						else
						{
							result = dsNameResultItem.name;
							result2 = true;
						}
					}
				}
			}
			finally
			{
				if (safeDsBindHandle != null)
				{
					safeDsBindHandle.Dispose();
				}
				if (safeDsNameResultHandle != null)
				{
					safeDsNameResultHandle.Dispose();
				}
			}
			return result2;
		}

		private static string GetComputerNameHelper(NativeMethods.ComputerNameFormat computerNameFormat)
		{
			uint capacity = 0U;
			NativeMethods.GetComputerNameEx((int)computerNameFormat, null, ref capacity);
			StringBuilder stringBuilder = new StringBuilder((int)capacity);
			if (!NativeMethods.GetComputerNameEx((int)computerNameFormat, stringBuilder, ref capacity))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new CannotGetComputerNameException(DirectoryStrings.CannotGetComputerName(lastWin32Error.ToString("X")))
				{
					ErrorCode = lastWin32Error
				};
			}
			return stringBuilder.ToString();
		}

		private static NativeMethods.InteropDsRolePrimaryDomainInfoBasic GetPrimaryDomainInformation(string serverFqdn = null)
		{
			SafeDsRolePrimaryDomainInfoLevelHandle safeDsRolePrimaryDomainInfoLevelHandle = null;
			int num = 0;
			NativeMethods.InteropDsRolePrimaryDomainInfoBasic result;
			try
			{
				int num2;
				for (;;)
				{
					num2 = NativeMethods.DsRoleGetPrimaryDomainInformation(serverFqdn, NativeMethods.DsRolePrimaryDomainInfoBasic, out safeDsRolePrimaryDomainInfoLevelHandle);
					if (num2 == 0)
					{
						goto IL_8A;
					}
					if (num2 != 1723)
					{
						goto IL_73;
					}
					if (num >= NativeHelpers.MaxCallRetry)
					{
						break;
					}
					num++;
					Thread.Sleep(num * NativeHelpers.CallRetryBaseSleepTime);
				}
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_RPC_SERVER_TOO_BUSY, "DsRoleGetPrimaryDomainInformation", new object[]
				{
					"DsRoleGetPrimaryDomainInformation",
					1723
				});
				throw new ADTransientException(DirectoryStrings.ExceptionDomainInfoRpcTooBusy);
				IL_73:
				throw new CannotGetDomainInfoException(DirectoryStrings.CannotGetDomainInfo(num2.ToString("X")));
				IL_8A:
				NativeMethods.InteropDsRolePrimaryDomainInfoBasic interopDsRolePrimaryDomainInfoBasic = new NativeMethods.InteropDsRolePrimaryDomainInfoBasic();
				Marshal.PtrToStructure(safeDsRolePrimaryDomainInfoLevelHandle.DangerousGetHandle(), interopDsRolePrimaryDomainInfoBasic);
				if (string.IsNullOrEmpty(interopDsRolePrimaryDomainInfoBasic.domainNameDns))
				{
					throw new CannotGetDomainInfoException(DirectoryStrings.CannotGetUsefulDomainInfo);
				}
				result = interopDsRolePrimaryDomainInfoBasic;
			}
			finally
			{
				if (safeDsRolePrimaryDomainInfoLevelHandle != null)
				{
					safeDsRolePrimaryDomainInfoLevelHandle.Dispose();
				}
			}
			return result;
		}

		public string GetSiteNameHookable(bool throwOnErrorNoSite = false)
		{
			SafeDsSiteNameHandle safeDsSiteNameHandle = null;
			string result;
			try
			{
				int num = NativeMethods.DsGetSiteName(null, out safeDsSiteNameHandle);
				if (num == NativeMethods.ERROR_NO_SITE)
				{
					if (throwOnErrorNoSite)
					{
						throw new CannotGetSiteInfoException(DirectoryStrings.CannotGetUsefulSiteInfo)
						{
							ErrorCode = num
						};
					}
					result = null;
				}
				else
				{
					if (num != 0)
					{
						throw new CannotGetSiteInfoException(DirectoryStrings.CannotGetSiteInfo(num.ToString("X")))
						{
							ErrorCode = num
						};
					}
					result = Marshal.PtrToStringUni(safeDsSiteNameHandle.DangerousGetHandle());
				}
			}
			finally
			{
				if (safeDsSiteNameHandle != null)
				{
					safeDsSiteNameHandle.Dispose();
				}
			}
			return result;
		}

		public string GetDomainNameHookable()
		{
			NativeMethods.InteropDsRolePrimaryDomainInfoBasic primaryDomainInformation = NativeHelpers.GetPrimaryDomainInformation(null);
			return primaryDomainInformation.domainNameDns;
		}

		public string GetForestNameHookable()
		{
			NativeMethods.InteropDsRolePrimaryDomainInfoBasic primaryDomainInformation = NativeHelpers.GetPrimaryDomainInformation(null);
			return primaryDomainInformation.domainForestName;
		}

		public StringCollection FindAllDomainControllersHookable(string domainFqdn, string siteName)
		{
			return this.FindAllDirectoryServers(domainFqdn, false, siteName);
		}

		public StringCollection FindAllGlobalCatalogsHookable(string forestFqdn, string siteName = null)
		{
			return this.FindAllDirectoryServers(forestFqdn, true, siteName);
		}

		private StringCollection FindAllDirectoryServers(string forestFqdn, bool requireGCs, string siteName = null)
		{
			Hashtable hashtable = new Hashtable();
			SafeDsGetDcContextHandle safeDsGetDcContextHandle = null;
			SafeDnsHostNameHandle safeDnsHostNameHandle = null;
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			NativeMethods.DsGetDcOpenFlags dsGetDcOpenFlags = NativeMethods.DsGetDcOpenFlags.ForceRediscovery;
			if (requireGCs)
			{
				dsGetDcOpenFlags |= NativeMethods.DsGetDcOpenFlags.GCRequired;
			}
			try
			{
				int num = NativeMethods.DsGetDcOpen(forestFqdn, 0, siteName, IntPtr.Zero, null, (int)dsGetDcOpenFlags, out safeDsGetDcContextHandle);
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3854970173U);
				if (num != 0)
				{
					throw new ADTransientException(DirectoryStrings.ExceptionNativeErrorWhenLookingForServersInDomain(num, forestFqdn, NativeHelpers.GetErrorMessageFromNativeError(num)));
				}
				num = NativeMethods.DsGetDcNext(safeDsGetDcContextHandle, ref zero, out zero2, out safeDnsHostNameHandle);
				if (num != 0 && num != 1101 && num != 9003 && num != 259)
				{
					throw new ADTransientException(DirectoryStrings.ExceptionNativeErrorWhenLookingForServersInDomain(num, forestFqdn, NativeHelpers.GetErrorMessageFromNativeError(num)));
				}
				while (num != 259)
				{
					if (num != 1101 && num != 9003)
					{
						try
						{
							string text = Marshal.PtrToStringUni(safeDnsHostNameHandle.DangerousGetHandle());
							string key = text.ToLower(CultureInfo.InvariantCulture);
							if (!hashtable.ContainsKey(key))
							{
								hashtable.Add(key, null);
							}
						}
						finally
						{
							if (safeDnsHostNameHandle != null)
							{
								safeDnsHostNameHandle.Dispose();
								safeDnsHostNameHandle = null;
							}
						}
					}
					num = NativeMethods.DsGetDcNext(safeDsGetDcContextHandle, ref zero, out zero2, out safeDnsHostNameHandle);
					if (num != 0 && num != 1101 && num != 9003 && num != 259)
					{
						throw new ADTransientException(DirectoryStrings.ExceptionNativeErrorWhenLookingForServersInDomain(num, forestFqdn, NativeHelpers.GetErrorMessageFromNativeError(num)));
					}
				}
			}
			finally
			{
				if (safeDsGetDcContextHandle != null)
				{
					safeDsGetDcContextHandle.Dispose();
				}
				if (safeDnsHostNameHandle != null)
				{
					safeDnsHostNameHandle.Dispose();
				}
			}
			StringCollection stringCollection = new StringCollection();
			foreach (object obj in hashtable.Keys)
			{
				string value = (string)obj;
				stringCollection.Add(value);
			}
			return stringCollection;
		}

		public string ResetSecureChannelDCForDomainHookable(string domainFqdn, bool throwOnError = true)
		{
			return this.ExecuteNetLogonOperation(domainFqdn, NativeMethods.NetLogonControlOperation.NetLogonControlRediscover, throwOnError).TrustedDcName;
		}

		public string GetSecureChannelDCForDomainHookable(string domainFqdn, bool throwOnError = true)
		{
			return this.ExecuteNetLogonOperation(domainFqdn, NativeMethods.NetLogonControlOperation.NetLogonControlTrustedChannelStatus, throwOnError).TrustedDcName;
		}

		private NativeMethods.NetLogonInfo2 ExecuteNetLogonOperation(string domainFqdn, NativeMethods.NetLogonControlOperation operation, bool throwOnError = true)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("domainFqdn", domainFqdn);
			IntPtr zero = IntPtr.Zero;
			IntPtr intPtr = Marshal.StringToCoTaskMemAuto(domainFqdn);
			NativeMethods.NetLogonInfo2 result;
			try
			{
				int num = NativeMethods.I_NetLogonControl2(null, (uint)operation, 2U, ref intPtr, out zero);
				if (num != 0)
				{
					Win32Exception ex = new Win32Exception(num);
					ExTraceGlobals.ADTopologyTracer.TraceWarning<string, NativeMethods.NetLogonControlOperation, string>((long)this.GetHashCode(), "{0} - NetLogon Error. Operation {1}. Error {2}", domainFqdn, operation, ex.Message);
					if (throwOnError)
					{
						throw new ADExternalException(DirectoryStrings.ExceptionNetLogonOperation(operation.ToString(), domainFqdn), ex);
					}
					return default(NativeMethods.NetLogonInfo2);
				}
				else
				{
					result = (NativeMethods.NetLogonInfo2)Marshal.PtrToStructure(zero, typeof(NativeMethods.NetLogonInfo2));
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
				if (zero != IntPtr.Zero)
				{
					NativeMethods.NetApiBufferFree(zero);
				}
			}
			if (!string.IsNullOrEmpty(result.TrustedDcName))
			{
				result.TrustedDcName = result.TrustedDcName.Replace("\\", string.Empty);
			}
			return result;
		}

		private const uint TransientExceptionNativeErrorWhenLookingForServersInDomain = 3854970173U;

		private static Hookable<INativeMethods> hookableInstance = Hookable<INativeMethods>.Create(true, new NativeHelpers());

		private static readonly int MaxCallRetry = 4;

		private static readonly int CallRetryBaseSleepTime = 150;
	}
}
