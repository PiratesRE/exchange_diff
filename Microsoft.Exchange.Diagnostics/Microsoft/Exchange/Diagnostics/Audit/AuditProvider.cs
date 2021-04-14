using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public abstract class AuditProvider
	{
		protected AuditProvider(string sourceName)
		{
			if (string.IsNullOrEmpty(sourceName))
			{
				throw new ArgumentNullException(DiagnosticsResources.NullSourceName);
			}
			if (!AuditProvider.IsSourceNameValid(sourceName))
			{
				throw new ArgumentException(DiagnosticsResources.InvalidSourceName, sourceName);
			}
			this.Init();
			Privilege privilege = new Privilege("SeAuditPrivilege");
			try
			{
				privilege.Enable();
				if (!NativeMethods.AuthzRegisterSecurityEventSource(0U, sourceName, out this.securityLogHandle))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 8)
					{
						throw new OutOfMemoryException();
					}
					if (lastWin32Error == 87)
					{
						throw new Win32Exception(lastWin32Error);
					}
					if (lastWin32Error == 5)
					{
						throw new UnauthorizedAccessException();
					}
					if (lastWin32Error == 1314)
					{
						throw new PrivilegeNotHeldException("SeAuditPrivilege");
					}
					throw new Win32Exception(lastWin32Error);
				}
			}
			catch (EntryPointNotFoundException)
			{
				throw new NotSupportedException();
			}
			finally
			{
				privilege.Dispose();
			}
		}

		internal static bool IsSourceNameValid(string sourceName)
		{
			if (string.IsNullOrEmpty(sourceName))
			{
				throw new ArgumentNullException(DiagnosticsResources.InvalidSourceName);
			}
			return !string.IsNullOrEmpty(sourceName) && sourceName.Length <= 255 && sourceName.IndexOf(';') < 0 && sourceName[0] != '\\' && !sourceName.ToUpper(CultureInfo.InvariantCulture).Equals("SECURITY");
		}

		protected void ReportAudit(int auditId, bool success, params object[] parameters)
		{
			if (this.securityLogHandle == null || this.securityLogHandle.IsInvalid)
			{
				return;
			}
			if (parameters != null)
			{
				if (parameters.Length > AuditProvider.MaximumNumberOfParameters)
				{
					throw new ArgumentOutOfRangeException(parameters.ToString(), DiagnosticsResources.ToomanyParams);
				}
				foreach (object obj in parameters)
				{
					if (!AuditProvider.mapper.Contains(obj.GetType()))
					{
						throw new ArgumentException(DiagnosticsResources.TypeNotSupported, parameters.ToString());
					}
				}
			}
			this.WriteToSecurityLog(auditId, success, parameters);
		}

		private void Init()
		{
			if (AuditProvider.mapper == null)
			{
				Interlocked.Exchange<HybridDictionary>(ref AuditProvider.mapper, new HybridDictionary
				{
					{
						typeof(string),
						AUDIT_PARAM_TYPE.APT_String
					},
					{
						typeof(uint),
						AUDIT_PARAM_TYPE.APT_Ulong
					},
					{
						typeof(Guid),
						AUDIT_PARAM_TYPE.APT_Guid
					},
					{
						typeof(DateTime),
						AUDIT_PARAM_TYPE.APT_Time
					},
					{
						typeof(ulong),
						AUDIT_PARAM_TYPE.APT_Int64
					}
				});
			}
		}

		private void WriteToSecurityLog(int auditId, bool success, params object[] parameters)
		{
			IntPtr parameters2 = IntPtr.Zero;
			GCHandle[] array = null;
			try
			{
				if (parameters.Length > 0)
				{
					array = new GCHandle[parameters.Length + 1];
					int num = 0;
					array[num] = GCHandle.Alloc(new byte[Marshal.SizeOf(typeof(NativeMethods.AUDIT_PARAM)) * parameters.Length], GCHandleType.Pinned);
					parameters2 = array[num].AddrOfPinnedObject();
					num++;
					for (int i = 0; i < parameters.Length; i++)
					{
						NativeMethods.AUDIT_PARAM audit_PARAM = default(NativeMethods.AUDIT_PARAM);
						audit_PARAM.Flags = 0U;
						audit_PARAM.Length = 0U;
						Type type = parameters[i].GetType();
						if (type == typeof(string))
						{
							audit_PARAM.Type = 2U;
							array[num] = GCHandle.Alloc(parameters[i] as string, GCHandleType.Pinned);
							audit_PARAM.Data0 = array[num].AddrOfPinnedObject();
							audit_PARAM.Data1 = IntPtr.Zero;
							num++;
						}
						else if (type == typeof(uint) || type == typeof(ushort) || type == typeof(byte) || type == typeof(int) || type == typeof(short) || type == typeof(sbyte))
						{
							audit_PARAM.Type = 3U;
							audit_PARAM.Data0 = new IntPtr((int)parameters[i]);
							audit_PARAM.Data1 = IntPtr.Zero;
						}
						else if (type == typeof(Guid))
						{
							audit_PARAM.Type = 9U;
							array[num] = GCHandle.Alloc(((Guid)parameters[i]).ToByteArray(), GCHandleType.Pinned);
							audit_PARAM.Data0 = array[num].AddrOfPinnedObject();
							audit_PARAM.Data1 = IntPtr.Zero;
							num++;
						}
						else if (type == typeof(DateTime))
						{
							audit_PARAM.Type = 10U;
							audit_PARAM.Data0 = new IntPtr((int)(((DateTime)parameters[i]).ToFileTime() & (long)((ulong)-1)));
							audit_PARAM.Data1 = new IntPtr((int)(((DateTime)parameters[i]).ToFileTime() >> 32 & (long)((ulong)-1)));
						}
						else
						{
							if (!(type == typeof(long)) && !(type == typeof(ulong)))
							{
								throw new ArgumentException(DiagnosticsResources.TypeNotSupported, type.ToString());
							}
							audit_PARAM.Type = 11U;
							audit_PARAM.Data0 = new IntPtr((long)((uint)parameters[i]) & (long)((ulong)-1));
							audit_PARAM.Data1 = new IntPtr((int)((long)((int)parameters[i]) & (long)((ulong)-1)));
						}
						Marshal.StructureToPtr(audit_PARAM, new IntPtr(parameters2.ToInt64() + (long)(i * Marshal.SizeOf(typeof(NativeMethods.AUDIT_PARAM)))), false);
					}
				}
				NativeMethods.AUDIT_PARAMS audit_PARAMS;
				audit_PARAMS.Length = 0U;
				audit_PARAMS.Flags = (success ? 1U : 0U);
				audit_PARAMS.Count = (ushort)parameters.Length;
				audit_PARAMS.Parameters = parameters2;
				if (!NativeMethods.AuthzReportSecurityEventFromParams(0U, this.securityLogHandle, (uint)auditId, null, ref audit_PARAMS))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 8)
					{
						throw new OutOfMemoryException();
					}
					if (lastWin32Error == 5)
					{
						throw new UnauthorizedAccessException();
					}
					throw new Win32Exception(lastWin32Error);
				}
			}
			finally
			{
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].IsAllocated)
						{
							array[j].Free();
						}
					}
				}
			}
		}

		private static readonly int MaximumNumberOfParameters = 32;

		private static HybridDictionary mapper = null;

		private SafeAuditHandle securityLogHandle;
	}
}
