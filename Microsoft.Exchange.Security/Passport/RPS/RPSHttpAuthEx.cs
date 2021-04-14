using System;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Passport.RPS.Native;

namespace Microsoft.Passport.RPS
{
	public sealed class RPSHttpAuthEx : IDisposable
	{
		public RPSHttpAuthEx(RPS rps)
		{
			if (rps == null)
			{
				throw new ArgumentException("RPS object cannot be null");
			}
			this.m_rps = rps;
			this.m_IRPSHttpAuth = (IRPSHttpAuth)rps.GetObjectInternal("rps.http.auth");
		}

		~RPSHttpAuthEx()
		{
			this.Cleanup(false);
		}

		public void Dispose()
		{
			this.Cleanup(true);
		}

		private void Cleanup(bool bSuppress)
		{
			if (!this.m_bHasDisposed)
			{
				this.m_bHasDisposed = true;
				if (this.m_IRPSHttpAuth != null)
				{
					Marshal.ReleaseComObject(this.m_IRPSHttpAuth);
					this.m_IRPSHttpAuth = null;
				}
				if (bSuppress)
				{
					GC.SuppressFinalize(this);
				}
				return;
			}
		}

		public IRPSHttpAuth NativeInterface
		{
			get
			{
				return this.m_IRPSHttpAuth;
			}
		}

		public RPSTicket Authenticate(string siteName, string headers, string path, string method, string querystring, RPSPropBag propBag, out bool needBody)
		{
			needBody = false;
			bool flag = true;
			IRPSPropBag irpspropBag;
			if (propBag != null)
			{
				irpspropBag = propBag.NativeInterface;
			}
			else
			{
				irpspropBag = null;
			}
			try
			{
				IRPSTicket irpsticket = (IRPSTicket)this.m_IRPSHttpAuth.AuthenticateRawHttp(siteName, method, path, querystring, null, flag, headers, null, irpspropBag);
				if (irpsticket != null)
				{
					return new RPSTicket(this.m_rps, irpsticket);
				}
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode != -2147184099)
				{
					throw ex;
				}
				needBody = true;
			}
			return null;
		}

		public RPSTicket Authenticate(string siteName, string headers, string path, string method, string querystring, string body, RPSPropBag propBag)
		{
			bool flag = true;
			IRPSPropBag irpspropBag;
			if (propBag != null)
			{
				irpspropBag = propBag.NativeInterface;
			}
			else
			{
				irpspropBag = null;
			}
			IRPSTicket irpsticket = (IRPSTicket)this.m_IRPSHttpAuth.AuthenticateRawHttp(siteName, method, path, querystring, null, flag, headers, body, irpspropBag);
			if (irpsticket != null)
			{
				return new RPSTicket(this.m_rps, irpsticket);
			}
			return null;
		}

		public string GetLogoutHeaders(string siteName)
		{
			string logoutHeaders = this.m_IRPSHttpAuth.GetLogoutHeaders(siteName);
			GC.KeepAlive(this);
			return logoutHeaders;
		}

		public string GetTweenerChallengeHeader(string siteName, RPSPropBag propBag)
		{
			IRPSPropBag irpspropBag;
			if (propBag != null)
			{
				irpspropBag = propBag.NativeInterface;
			}
			else
			{
				irpspropBag = null;
			}
			string tweenerChallengeHeader = this.m_IRPSHttpAuth.GetTweenerChallengeHeader(siteName, irpspropBag);
			GC.KeepAlive(this);
			return tweenerChallengeHeader;
		}

		public string GetLiveIDChallengeHeader(string siteName, RPSPropBag propBag)
		{
			IRPSPropBag irpspropBag;
			if (propBag != null)
			{
				irpspropBag = propBag.NativeInterface;
			}
			else
			{
				irpspropBag = null;
			}
			string liveIDChallengeHeader = this.m_IRPSHttpAuth.GetLiveIDChallengeHeader(siteName, irpspropBag);
			GC.KeepAlive(this);
			return liveIDChallengeHeader;
		}

		public string LogoTag(bool bLogin, bool bSecure, string urlName, string domainName, string siteName, RPSPropBag propBag)
		{
			IRPSPropBag irpspropBag;
			if (propBag != null)
			{
				irpspropBag = propBag.NativeInterface;
			}
			else
			{
				irpspropBag = null;
			}
			string result = this.m_IRPSHttpAuth.LogoTag(bLogin, bSecure, urlName, domainName, siteName, irpspropBag);
			GC.KeepAlive(this);
			return result;
		}

		public string TextTag(bool bLogin, bool bSecure, string urlName, string domainName, string siteName, RPSPropBag propBag)
		{
			IRPSPropBag irpspropBag;
			if (propBag != null)
			{
				irpspropBag = propBag.NativeInterface;
			}
			else
			{
				irpspropBag = null;
			}
			string result = this.m_IRPSHttpAuth.TextTag(bLogin, bSecure, urlName, domainName, siteName, irpspropBag);
			GC.KeepAlive(this);
			return result;
		}

		internal static void InternalWriteHeaders(HttpResponse response, string headers)
		{
			if (response == null || headers == null)
			{
				throw new ArgumentException("response and headers cannot be null.");
			}
			int num;
			for (int i = 0; i < headers.Length; i = num + 2)
			{
				num = headers.IndexOf('\r', i);
				if (num < 0)
				{
					num = headers.Length;
				}
				string text = headers.Substring(i, num - i);
				int num2 = text.IndexOf(':');
				if (num2 > 0)
				{
					string name = text.Substring(0, num2);
					string value = text.Substring(num2 + 1);
					response.AddHeader(name, value);
				}
			}
		}

		public void WriteHeaders(HttpResponse response, string headers)
		{
			RPSHttpAuthEx.InternalWriteHeaders(response, headers);
		}

		private RPS m_rps;

		private bool m_bHasDisposed;

		private IRPSHttpAuth m_IRPSHttpAuth;
	}
}
