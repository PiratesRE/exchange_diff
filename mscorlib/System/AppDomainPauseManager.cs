using System;
using System.Security;
using System.Threading;

namespace System
{
	[SecurityCritical]
	internal class AppDomainPauseManager
	{
		[SecurityCritical]
		public AppDomainPauseManager()
		{
			AppDomainPauseManager.isPaused = false;
		}

		internal static AppDomainPauseManager Instance
		{
			[SecurityCritical]
			get
			{
				return AppDomainPauseManager.instance;
			}
		}

		[SecurityCritical]
		public void Pausing()
		{
		}

		[SecurityCritical]
		public void Paused()
		{
			if (AppDomainPauseManager.ResumeEvent == null)
			{
				AppDomainPauseManager.ResumeEvent = new ManualResetEvent(false);
			}
			else
			{
				AppDomainPauseManager.ResumeEvent.Reset();
			}
			Timer.Pause();
			AppDomainPauseManager.isPaused = true;
		}

		[SecurityCritical]
		public void Resuming()
		{
			AppDomainPauseManager.isPaused = false;
			AppDomainPauseManager.ResumeEvent.Set();
		}

		[SecurityCritical]
		public void Resumed()
		{
			Timer.Resume();
		}

		internal static bool IsPaused
		{
			[SecurityCritical]
			get
			{
				return AppDomainPauseManager.isPaused;
			}
		}

		internal static ManualResetEvent ResumeEvent { [SecurityCritical] get; [SecurityCritical] set; }

		private static readonly AppDomainPauseManager instance = new AppDomainPauseManager();

		private static volatile bool isPaused;
	}
}
