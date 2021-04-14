using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Authentication;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Transport.Agent.LiveIdAuth
{
	internal sealed class LiveIdAuthAgent : SmtpReceiveAgent
	{
		public LiveIdAuthAgent()
		{
			this.liveAuth = new LiveIdBasicAuthentication();
			this.liveAuth.ApplicationName = "Microsoft.Exchange.SMTP";
			base.OnProcessAuthentication += this.BeginLiveIdAuth;
		}

		private static byte[] GetPasswordFromSecureString(SecureString password)
		{
			IntPtr intPtr = IntPtr.Zero;
			char[] array = new char[password.Length];
			byte[] array2;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
				Marshal.Copy(intPtr, array, 0, password.Length);
				int num = password.Length;
				int num2 = password.Length - 1;
				while (num2 >= 1 && array[num2] == '\0' && array[num2 - 1] == '\0')
				{
					num--;
					num2--;
				}
				array2 = new byte[num];
				for (int i = 0; i < num; i++)
				{
					array2[i] = (byte)array[i];
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
				}
				Array.Clear(array, 0, array.Length);
			}
			return array2;
		}

		private void BeginLiveIdAuth(ReceiveCommandEventSource source, ProcessAuthenticationEventArgs args)
		{
			this.args = args;
			this.asyncContext = base.GetAgentAsyncContext();
			if (this.args.SmtpSession.LastExternalIPAddress != null)
			{
				this.liveAuth.UserIpAddress = this.args.SmtpSession.LastExternalIPAddress.ToString();
			}
			else
			{
				this.liveAuth.UserIpAddress = null;
			}
			this.liveAuth.BeginGetWindowsIdentity(this.args.UserName, LiveIdAuthAgent.GetPasswordFromSecureString(this.args.Password), new AsyncCallback(this.OnLiveIdAuthCallback), null, default(Guid));
			ExTraceGlobals.DefaultTracer.TraceDebug<string>((long)this.GetHashCode(), "Attempt to authenticate Live account {0}", Encoding.ASCII.GetString(this.args.UserName));
		}

		private void OnLiveIdAuthCallback(IAsyncResult ar)
		{
			this.Resume();
			WindowsIdentity windowsIdentity;
			this.args.AuthResult = this.liveAuth.EndGetWindowsIdentity(ar, out windowsIdentity);
			this.args.Identity = windowsIdentity;
			this.args.AuthErrorDetails = this.liveAuth.LastRequestErrorMessage;
			string text = null;
			try
			{
				text = ((windowsIdentity != null) ? windowsIdentity.Name : string.Empty);
			}
			catch (IdentityNotMappedException)
			{
				this.args.Identity = null;
			}
			catch (SystemException)
			{
				this.args.Identity = null;
			}
			ExTraceGlobals.DefaultTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Live account {0} authenticated as {1}", Encoding.ASCII.GetString(this.args.UserName), (!string.IsNullOrEmpty(text)) ? text : "Anonymous");
			this.AsyncCompleted();
		}

		private void AsyncCompleted()
		{
			if (this.asyncContext != null)
			{
				AgentAsyncContext agentAsyncContext = this.asyncContext;
				this.asyncContext = null;
				agentAsyncContext.Complete();
				return;
			}
			ExTraceGlobals.DefaultTracer.TraceError((long)this.GetHashCode(), "AsyncCompleted() was called but MEx context is null. This should never happen.");
		}

		private void Resume()
		{
			if (this.asyncContext != null)
			{
				this.asyncContext.Resume();
			}
		}

		private readonly LiveIdBasicAuthentication liveAuth;

		private AgentAsyncContext asyncContext;

		private ProcessAuthenticationEventArgs args;
	}
}
