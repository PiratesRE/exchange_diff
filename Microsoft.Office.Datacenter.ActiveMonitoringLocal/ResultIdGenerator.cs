using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal abstract class ResultIdGenerator<TResult> where TResult : WorkItemResult, IPersistence, new()
	{
		static ResultIdGenerator()
		{
			List<SecurityIdentifier> list = new List<SecurityIdentifier>();
			list.Add(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null));
			list.Add(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null));
			list.Add(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null));
			list.Add(new SecurityIdentifier(WellKnownSidType.LocalServiceSid, null));
			SecurityIdentifier securityIdentifier = null;
			try
			{
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					if (current.User != null)
					{
						securityIdentifier = current.User.AccountDomainSid;
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(WTFLog.Core, TracingContext.Default, ex.ToString(), null, ".cctor", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\ResultIdGenerator.cs", 90);
			}
			if (securityIdentifier != null)
			{
				list.Add(new SecurityIdentifier(WellKnownSidType.AccountAdministratorSid, securityIdentifier));
				list.Add(new SecurityIdentifier(WellKnownSidType.AccountEnterpriseAdminsSid, securityIdentifier));
			}
			ResultIdGenerator<TResult>.mutexSecurity = new MutexSecurity();
			foreach (SecurityIdentifier identity in list)
			{
				MutexAccessRule rule = new MutexAccessRule(identity, MutexRights.Modify | MutexRights.Synchronize, AccessControlType.Allow);
				ResultIdGenerator<TResult>.mutexSecurity.AddAccessRule(rule);
			}
		}

		public ResultIdGenerator()
		{
			try
			{
				this.Counter.IncrementBy(0L);
			}
			catch (InvalidOperationException)
			{
				this.counterExists = false;
			}
		}

		protected abstract ExPerformanceCounter Counter { get; }

		public int NextId()
		{
			if (!this.counterExists)
			{
				return 0;
			}
			if (this.Counter.RawValue == 0L)
			{
				bool flag;
				using (Mutex mutex = new Mutex(true, ResultIdGenerator<TResult>.mutexName, ref flag, ResultIdGenerator<TResult>.mutexSecurity))
				{
					if (!flag)
					{
						try
						{
							flag = mutex.WaitOne(1000);
						}
						catch (AbandonedMutexException)
						{
						}
					}
					try
					{
						if (this.Counter.RawValue == 0L)
						{
							using (CrimsonReader<TResult> crimsonReader = new CrimsonReader<TResult>())
							{
								TResult tresult = crimsonReader.ReadLast();
								if (tresult != null)
								{
									return (int)this.Counter.IncrementBy((long)(tresult.ResultId + 1000));
								}
							}
						}
					}
					finally
					{
						if (flag)
						{
							mutex.ReleaseMutex();
						}
					}
				}
			}
			return (int)this.Counter.Increment();
		}

		private const int CounterJump = 1000;

		private readonly bool counterExists = true;

		private static string mutexName = typeof(TResult).Name + "{0AA95102-2F00-48A8-B1FC-8E1A42569BC8}";

		private static MutexSecurity mutexSecurity;
	}
}
