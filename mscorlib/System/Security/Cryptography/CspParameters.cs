using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class CspParameters
	{
		public CspProviderFlags Flags
		{
			get
			{
				return (CspProviderFlags)this.m_flags;
			}
			set
			{
				int num = 255;
				if ((value & (CspProviderFlags)(~(CspProviderFlags)num)) != CspProviderFlags.NoFlags)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
					{
						(int)value
					}), "value");
				}
				this.m_flags = (int)value;
			}
		}

		public CryptoKeySecurity CryptoKeySecurity
		{
			get
			{
				return this.m_cryptoKeySecurity;
			}
			set
			{
				this.m_cryptoKeySecurity = value;
			}
		}

		public SecureString KeyPassword
		{
			get
			{
				return this.m_keyPassword;
			}
			set
			{
				this.m_keyPassword = value;
				this.m_parentWindowHandle = IntPtr.Zero;
			}
		}

		public IntPtr ParentWindowHandle
		{
			get
			{
				return this.m_parentWindowHandle;
			}
			set
			{
				this.m_parentWindowHandle = value;
				this.m_keyPassword = null;
			}
		}

		public CspParameters() : this(24, null, null)
		{
		}

		public CspParameters(int dwTypeIn) : this(dwTypeIn, null, null)
		{
		}

		public CspParameters(int dwTypeIn, string strProviderNameIn) : this(dwTypeIn, strProviderNameIn, null)
		{
		}

		public CspParameters(int dwTypeIn, string strProviderNameIn, string strContainerNameIn) : this(dwTypeIn, strProviderNameIn, strContainerNameIn, CspProviderFlags.NoFlags)
		{
		}

		public CspParameters(int providerType, string providerName, string keyContainerName, CryptoKeySecurity cryptoKeySecurity, SecureString keyPassword) : this(providerType, providerName, keyContainerName)
		{
			this.m_cryptoKeySecurity = cryptoKeySecurity;
			this.m_keyPassword = keyPassword;
		}

		public CspParameters(int providerType, string providerName, string keyContainerName, CryptoKeySecurity cryptoKeySecurity, IntPtr parentWindowHandle) : this(providerType, providerName, keyContainerName)
		{
			this.m_cryptoKeySecurity = cryptoKeySecurity;
			this.m_parentWindowHandle = parentWindowHandle;
		}

		internal CspParameters(int providerType, string providerName, string keyContainerName, CspProviderFlags flags)
		{
			this.ProviderType = providerType;
			this.ProviderName = providerName;
			this.KeyContainerName = keyContainerName;
			this.KeyNumber = -1;
			this.Flags = flags;
		}

		internal CspParameters(CspParameters parameters)
		{
			this.ProviderType = parameters.ProviderType;
			this.ProviderName = parameters.ProviderName;
			this.KeyContainerName = parameters.KeyContainerName;
			this.KeyNumber = parameters.KeyNumber;
			this.Flags = parameters.Flags;
			this.m_cryptoKeySecurity = parameters.m_cryptoKeySecurity;
			this.m_keyPassword = parameters.m_keyPassword;
			this.m_parentWindowHandle = parameters.m_parentWindowHandle;
		}

		public int ProviderType;

		public string ProviderName;

		public string KeyContainerName;

		public int KeyNumber;

		private int m_flags;

		private CryptoKeySecurity m_cryptoKeySecurity;

		private SecureString m_keyPassword;

		private IntPtr m_parentWindowHandle;
	}
}
