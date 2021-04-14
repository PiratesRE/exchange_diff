using System;
using System.Security;

namespace System.IO.IsolatedStorage
{
	[SecurityCritical]
	public class IsolatedStorageSecurityState : SecurityState
	{
		internal static IsolatedStorageSecurityState CreateStateToIncreaseQuotaForApplication(long newQuota, long usedSize)
		{
			return new IsolatedStorageSecurityState
			{
				m_Options = IsolatedStorageSecurityOptions.IncreaseQuotaForApplication,
				m_Quota = newQuota,
				m_UsedSize = usedSize
			};
		}

		[SecurityCritical]
		private IsolatedStorageSecurityState()
		{
		}

		public IsolatedStorageSecurityOptions Options
		{
			get
			{
				return this.m_Options;
			}
		}

		public long UsedSize
		{
			get
			{
				return this.m_UsedSize;
			}
		}

		public long Quota
		{
			get
			{
				return this.m_Quota;
			}
			set
			{
				this.m_Quota = value;
			}
		}

		[SecurityCritical]
		public override void EnsureState()
		{
			if (!base.IsStateAvailable())
			{
				throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Operation"));
			}
		}

		private long m_UsedSize;

		private long m_Quota;

		private IsolatedStorageSecurityOptions m_Options;
	}
}
