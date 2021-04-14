using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning
{
	internal class LiveIdentityManager
	{
		public LiveIdentityManager() : this(new NativeMethods())
		{
		}

		internal LiveIdentityManager(NativeMethods nativeLibrary)
		{
			this.nativeMethods = nativeLibrary;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public string LogOnUser(string federationProviderId, string userName, string password, string siteName, string policy, string environment)
		{
			this.CloseIdentity();
			this.Initialize(environment);
			this.OpenIdentity(federationProviderId, userName, password);
			this.LogonPassport(policy, siteName);
			return this.LogonService(siteName, policy);
		}

		internal void CloseIdentity()
		{
			if (IntPtr.Zero != this.identityPtr && this.nativeMethods.CloseIdentityHandle(this.identityPtr) == 0)
			{
				this.identityPtr = IntPtr.Zero;
			}
		}

		internal virtual void Initialize(string environment)
		{
			this.Uninitialize();
			NativeMethods.IdcrlOption[] array = null;
			uint dwOptions = 0U;
			GCHandle gchandle = default(GCHandle);
			try
			{
				if (!string.IsNullOrEmpty(environment))
				{
					array = new NativeMethods.IdcrlOption[1];
					dwOptions = 1U;
					byte[] bytes = Encoding.Unicode.GetBytes(environment);
					byte[] value = new byte[bytes.Length];
					gchandle = GCHandle.Alloc(value, GCHandleType.Pinned);
					IntPtr intPtr = gchandle.AddrOfPinnedObject();
					Marshal.Copy(bytes, 0, intPtr, bytes.Length);
					array[0].EnvironmentId = 64;
					array[0].EnvironmentValue = intPtr;
					array[0].EnvironmentLength = (uint)bytes.Length;
				}
				int num = this.nativeMethods.InitializeEx(ref this.serviceGuid, 1, 0U, array, dwOptions);
				if (num < 0)
				{
					string message = string.Format(CultureInfo.CurrentCulture, "Failed to initialize the environment: {0} , HR: {1}", new object[]
					{
						environment,
						num.ToString(CultureInfo.InvariantCulture)
					});
					WindowsLiveException ex = new WindowsLiveException(num, message);
					throw ex;
				}
				this.initialized = true;
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
		}

		internal void LogonPassport(string policy, string siteName)
		{
			NativeMethods.RstParams[] array = new NativeMethods.RstParams[1];
			array[0].CbSize = 0;
			array[0].ServiceName = siteName;
			array[0].ServicePolicy = policy;
			array[0].TokenFlags = 0;
			array[0].TokenParams = 0;
			int num = this.nativeMethods.LogonIdentityEx(this.identityPtr, policy, 0U, array, (uint)array.Length);
			this.GetAuthenticationStatus(siteName);
			if (num < 0)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "FailLoginIdentity: policy: {0}, HR: {1}", new object[]
				{
					policy,
					num.ToString(CultureInfo.InvariantCulture)
				});
				WindowsLiveException ex = new WindowsLiveException(num, message);
				throw ex;
			}
		}

		internal void GetAuthenticationStatus(string siteName)
		{
			IntPtr zero = IntPtr.Zero;
			try
			{
				int authenticationStatus = this.nativeMethods.GetAuthenticationStatus(this.identityPtr, siteName, 1U, out zero);
				if (authenticationStatus < 0)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "FailGetAuthState: HR: {0}", new object[]
					{
						authenticationStatus.ToString(CultureInfo.InvariantCulture)
					});
					throw new WindowsLiveException(authenticationStatus, message);
				}
				NativeMethods.IdcrlStatusCurrent idcrlStatusCurrent = new NativeMethods.IdcrlStatusCurrent();
				Marshal.PtrToStructure(zero, idcrlStatusCurrent);
				if (296963 != idcrlStatusCurrent.AuthState)
				{
					string message2 = string.Format(CultureInfo.InvariantCulture, "FailGetAuthInvalidState: AuthState: {0}, RequestStatus: {1}, HR: {2}", new object[]
					{
						"0x" + idcrlStatusCurrent.AuthState.ToString("X", CultureInfo.InvariantCulture),
						"0x" + idcrlStatusCurrent.RequestStatus.ToString("X", CultureInfo.InvariantCulture),
						authenticationStatus.ToString(CultureInfo.InvariantCulture)
					});
					throw new WindowsLiveException(idcrlStatusCurrent.RequestStatus, message2);
				}
			}
			finally
			{
				this.FreeResource(ref zero);
			}
		}

		internal virtual string LogonService(string siteName, string policy)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			uint num = 0U;
			uint num2 = 0U;
			int num3 = this.nativeMethods.AuthIdentityToService(this.identityPtr, siteName, policy, 131072U, out zero, out num2, out zero2, out num);
			if (num3 < 0)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "FailAuthIdentityToService: site name: {0}, policy: {1}, HR: {2}", new object[]
				{
					siteName,
					policy,
					num3.ToString(CultureInfo.InvariantCulture)
				});
				throw new WindowsLiveException(num3, message);
			}
			string result;
			try
			{
				result = Marshal.PtrToStringUni(zero);
			}
			finally
			{
				this.FreeResource(ref zero);
				this.FreeResource(ref zero2);
			}
			return result;
		}

		internal void OpenIdentity(string federationProviderId, string userName, string password)
		{
			try
			{
				int num = this.nativeMethods.CreateIdentityHandle2(federationProviderId, userName, 255U, out this.identityPtr);
				if (num < 0)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "FailCreateIdentityHandle: user name: {0}, HR: {1}", new object[]
					{
						userName,
						num.ToString(CultureInfo.InvariantCulture)
					});
					throw new WindowsLiveException(num, message);
				}
				if (!string.IsNullOrEmpty(password))
				{
					num = this.nativeMethods.SetCredential(this.identityPtr, "ps:password", password);
					if (num < 0)
					{
						throw new WindowsLiveException(num, "Messages.FailSetCredential");
					}
				}
			}
			catch
			{
				this.CloseIdentity();
				throw;
			}
		}

		internal string GetLoggedOnUser()
		{
			string text = WindowsIdentity.GetCurrent().Name.ToLower();
			string[] array = text.Split(new char[]
			{
				'\\'
			});
			if (array.Length == 1 && text.Contains("@"))
			{
				return text;
			}
			IntPtr hEnumHandle;
			int num = this.nativeMethods.EnumIdentitiesWithCachedCredentials(null, out hEnumHandle);
			string text2 = null;
			string text3 = null;
			if (num == 0)
			{
				while (this.nativeMethods.NextIdentity(hEnumHandle, ref text2) == 0)
				{
					if (!string.IsNullOrEmpty(text2) && text2.Contains(array[1]))
					{
						text3 = text2;
						break;
					}
				}
			}
			this.nativeMethods.CloseEnumIdentitiesHandle(hEnumHandle);
			if (num != 0 || text3 == null)
			{
				throw new WindowsLiveException("Messages.FailAuthIdentityToService");
			}
			return text3;
		}

		internal void Uninitialize()
		{
			if (this.initialized)
			{
				int num = this.nativeMethods.Uninitialize();
				if (num < 0)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "FailUninitialize: HR: {0}", new object[]
					{
						num.ToString(CultureInfo.InvariantCulture)
					});
					throw new WindowsLiveException(num, message);
				}
				this.initialized = false;
			}
		}

		private void FreeResource(ref IntPtr resource)
		{
			if (IntPtr.Zero != resource && this.nativeMethods.PassportFreeMemory(resource) == 0)
			{
				resource = IntPtr.Zero;
			}
		}

		private const string CredentialTypePassword = "ps:password";

		private const int IdcrlCurrentVersion = 1;

		private const int IdcrlAuthStateAuthenticatedPassword = 296963;

		private const int ResultCode = 0;

		private readonly NativeMethods nativeMethods;

		private IntPtr identityPtr = IntPtr.Zero;

		private bool initialized;

		private Guid serviceGuid = Guid.NewGuid();
	}
}
