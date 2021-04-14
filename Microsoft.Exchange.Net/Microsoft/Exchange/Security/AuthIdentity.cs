using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct AuthIdentity : IDisposable
	{
		internal AuthIdentity(string userName, SecureString password, string domain)
		{
			this.UserName = userName;
			this.UserNameLength = (string.IsNullOrEmpty(userName) ? 0 : userName.Length);
			this.Domain = domain;
			this.DomainLength = (string.IsNullOrEmpty(domain) ? 0 : domain.Length);
			if (password == null)
			{
				this.Password = IntPtr.Zero;
				this.PasswordLength = 0;
			}
			else
			{
				this.Password = Marshal.SecureStringToGlobalAllocUnicode(password);
				this.PasswordLength = password.Length;
			}
			this.Flags = 2;
		}

		public void Dispose()
		{
			if (this.Password != IntPtr.Zero)
			{
				Marshal.ZeroFreeGlobalAllocUnicode(this.Password);
			}
		}

		public override string ToString()
		{
			return ValidationHelper.ToString(this.Domain) + "\\" + ValidationHelper.ToString(this.UserName);
		}

		internal string UserName;

		internal int UserNameLength;

		internal string Domain;

		internal int DomainLength;

		internal IntPtr Password;

		internal int PasswordLength;

		internal int Flags;

		public static AuthIdentity Default = new AuthIdentity(null, null, null);

		public static AuthIdentity LocalMachine = new AuthIdentity(Environment.MachineName + '$', null, null);
	}
}
