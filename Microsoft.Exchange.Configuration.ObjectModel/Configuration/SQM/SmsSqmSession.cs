using System;
using Microsoft.Exchange.Sqm;

namespace Microsoft.Exchange.Configuration.SQM
{
	internal class SmsSqmSession : SqmSession
	{
		private SmsSqmSession() : base(SqmAppID.SMS, SqmSession.Scope.Process)
		{
			base.Open();
			AppDomain.CurrentDomain.ProcessExit += SmsSqmSession.CloseSessionEventHandler;
			AppDomain.CurrentDomain.DomainUnload += delegate(object param0, EventArgs param1)
			{
				if (SmsSqmSession.instance != null)
				{
					SmsSqmSession.instance.Close();
					SmsSqmSession.instance = null;
				}
				AppDomain.CurrentDomain.ProcessExit -= SmsSqmSession.CloseSessionEventHandler;
			};
		}

		public static SmsSqmSession Instance
		{
			get
			{
				lock (SmsSqmSession.GetInstanceSyncObject)
				{
					if (SmsSqmSession.instance == null)
					{
						SmsSqmSession.instance = new SmsSqmSession();
					}
				}
				return SmsSqmSession.instance;
			}
		}

		private static void CloseSessionEventHandler(object sender, EventArgs e)
		{
			if (SmsSqmSession.instance != null)
			{
				SmsSqmSession.instance.Close();
				SmsSqmSession.instance = null;
			}
		}

		private static readonly object GetInstanceSyncObject = new object();

		private static SmsSqmSession instance = null;
	}
}
