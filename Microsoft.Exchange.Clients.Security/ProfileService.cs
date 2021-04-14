using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.Clients.Security
{
	internal class ProfileService
	{
		private ProfileService()
		{
		}

		internal static ProfileService Instance
		{
			get
			{
				if (ProfileService.instance == null)
				{
					Interlocked.CompareExchange<ProfileService>(ref ProfileService.instance, new ProfileService(), null);
				}
				return ProfileService.instance;
			}
		}

		internal LiveIdManager LiveIdManager
		{
			get
			{
				if (this.liveIdManager == null)
				{
					this.liveIdManager = PassportLiveIdManager.CreateLiveIdManager(LiveIdInstanceType.Business, delegate(LocalizedException exception, ExchangeErrorCategory category)
					{
						ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError(0L, exception.ToString());
					}, null, null, false);
				}
				return this.liveIdManager;
			}
		}

		internal DateTime GetAccountCreationTime(string memberName)
		{
			DateTime result;
			lock (this.lockObject)
			{
				LiveIdManager liveIdManager = this.LiveIdManager;
				DateTime accountCreationTime = liveIdManager.GetAccountCreationTime(new SmtpAddress(memberName));
				result = accountCreationTime;
			}
			return result;
		}

		private void DisposeLiveIdManager()
		{
			if (this.liveIdManager != null)
			{
				this.liveIdManager.Dispose();
				this.liveIdManager = null;
			}
		}

		private static ProfileService instance;

		private object lockObject = new object();

		private LiveIdManager liveIdManager;
	}
}
