using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class SingleProxySession : IDisposable
	{
		public ProxyAddressTemplate BaseAddress
		{
			get
			{
				return this.baseAddress;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public ProxyDLL ProxyDll
		{
			get
			{
				return this.proxyDll;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.isInitialized;
			}
		}

		public SingleProxySession(ProxyDLL proxyDll, ProxyAddressTemplate baseAddress, string serverName)
		{
			this.proxyDll = proxyDll;
			this.serverName = serverName;
			this.baseAddress = baseAddress;
			this.hProxySession = IntPtr.Zero;
			this.isInitialized = false;
		}

		protected void Dispose(bool disposing)
		{
			if (disposing && this.isInitialized)
			{
				this.CloseProxies();
				this.isInitialized = false;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Initialize()
		{
			if (!this.isInitialized)
			{
				this.InitProxies();
				this.isInitialized = true;
			}
		}

		public void UnInitialize()
		{
			if (this.isInitialized)
			{
				this.CloseProxies();
				this.isInitialized = false;
			}
		}

		public string GenerateProxy(RecipientInfo pRecipientInfo, int nRetries)
		{
			IntPtr zero = IntPtr.Zero;
			string result = null;
			try
			{
				this.GenerateProxy(pRecipientInfo, nRetries, out zero);
				if (zero != IntPtr.Zero)
				{
					result = Marshal.PtrToStringUni(zero);
				}
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					this.FreeProxy(zero);
				}
			}
			return result;
		}

		private void InitProxies()
		{
			ReturnCode rc = this.proxyDll.RcInitProxies(this.BaseAddress.ToString(), this.ServerName, out this.hProxySession);
			this.CheckReturnCode(rc);
		}

		private void GenerateProxy(RecipientInfo pRecipientInfo, int nRetries, out IntPtr ppszProxyAddr)
		{
			ReturnCode rc = this.proxyDll.RcGenerateProxy(this.hProxySession, ref pRecipientInfo, nRetries, out ppszProxyAddr);
			this.CheckReturnCode(rc);
		}

		private void FreeProxy(IntPtr pszProxy)
		{
			this.proxyDll.FreeProxy(pszProxy);
		}

		public bool CheckProxy(RecipientInfo pRecipientInfo, string pwszProxyAddr)
		{
			bool result = false;
			if (this.proxyDll.RcCheckProxy != null)
			{
				ReturnCode rc = this.proxyDll.RcCheckProxy(this.hProxySession, ref pRecipientInfo, pwszProxyAddr, out result);
				this.CheckReturnCode(rc);
			}
			return result;
		}

		public bool ValidateBaseAddress(string pszBaseAddress)
		{
			StringBuilder pwstrSiteProxy = new StringBuilder(pszBaseAddress);
			bool result = false;
			ReturnCode rc = this.proxyDll.RcValidateSiteProxy(this.hProxySession, pwstrSiteProxy, out result);
			this.CheckReturnCode(rc);
			return result;
		}

		private void CloseProxies()
		{
			this.proxyDll.CloseProxies(this.hProxySession);
		}

		public bool ValidateProxy(string proxyAddress)
		{
			StringBuilder stringBuilder = new StringBuilder(proxyAddress);
			bool flag = false;
			ReturnCode rc = this.proxyDll.RcValidateProxy(this.hProxySession, stringBuilder, out flag);
			this.CheckReturnCode(rc);
			return flag && StringComparer.InvariantCultureIgnoreCase.Equals(stringBuilder.ToString(), proxyAddress);
		}

		private void CheckReturnCode(ReturnCode rc)
		{
			if (rc == ReturnCode.RC_SUCCESS)
			{
				return;
			}
			LocalizedString value;
			switch (rc)
			{
			case ReturnCode.RC_ERROR:
				value = Strings.ProxyDLLError;
				break;
			case ReturnCode.RC_PROTOCOL:
				value = Strings.ProxyDLLProtocol;
				break;
			case ReturnCode.RC_SYNTAX:
				value = Strings.ProxyDLLSyntax;
				break;
			case ReturnCode.RC_EOF:
				value = Strings.ProxyDLLEOF;
				break;
			case ReturnCode.RC_IMPLEMENTATION:
				value = Strings.ProxyNotImplemented;
				break;
			case ReturnCode.RC_SOFTWARE:
				value = Strings.ProxyDLLSoftware;
				break;
			case ReturnCode.RC_CONFIG:
				value = Strings.ProxyDLLConfig;
				break;
			case ReturnCode.RC_MEMORY:
				value = Strings.ProxyDLLOOM;
				break;
			case ReturnCode.RC_CONTENTION:
				value = Strings.ProxyDLLContention;
				break;
			case ReturnCode.RC_NOTFOUND:
				value = Strings.ProxyDLLNotFound;
				break;
			case ReturnCode.RC_DISKSPACE:
				value = Strings.ProxyDLLDiskSpace;
				break;
			default:
				value = Strings.ProxyDLLDefault;
				break;
			}
			throw new RusException(Strings.ErrorProxyGeneration(value));
		}

		private ProxyDLL proxyDll;

		private IntPtr hProxySession;

		private bool isInitialized;

		private ProxyAddressTemplate baseAddress;

		private string serverName;
	}
}
