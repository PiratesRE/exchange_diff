using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class MRSProxyTask : SessionTask
	{
		public MRSProxyTask() : base(HybridStrings.MRSProxyTaskName, 2)
		{
		}

		public override bool CheckPrereqs(ITaskContext taskContext)
		{
			if (!base.CheckPrereqs(taskContext))
			{
				base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.HybridInfoBasePrereqsFailed));
				return false;
			}
			return true;
		}

		public override bool NeedsConfiguration(ITaskContext taskContext)
		{
			bool flag = base.NeedsConfiguration(taskContext);
			return this.CheckOrVerifyConfiguration(taskContext, false) || flag;
		}

		public override bool Configure(ITaskContext taskContext)
		{
			if (!base.Configure(taskContext))
			{
				return false;
			}
			ADWebServicesVirtualDirectory[] vdirsToEnable = this.GetVdirsToEnable();
			foreach (ADWebServicesVirtualDirectory adwebServicesVirtualDirectory in vdirsToEnable)
			{
				base.OnPremisesSession.SetWebServicesVirtualDirectory(adwebServicesVirtualDirectory.DistinguishedName, true);
			}
			return true;
		}

		public override bool ValidateConfiguration(ITaskContext taskContext)
		{
			return base.ValidateConfiguration(taskContext) && !this.CheckOrVerifyConfiguration(taskContext, true);
		}

		private bool CheckOrVerifyConfiguration(ITaskContext taskContext, bool fVerifyOnly)
		{
			ADWebServicesVirtualDirectory[] vdirsToEnable = this.GetVdirsToEnable();
			return vdirsToEnable.Length != 0;
		}

		private ADWebServicesVirtualDirectory[] GetVdirsToEnable()
		{
			ServerVersion b = new ServerVersion(14, 0, 100, 0);
			List<ADWebServicesVirtualDirectory> list = new List<ADWebServicesVirtualDirectory>(1);
			IEnumerable<ADWebServicesVirtualDirectory> webServicesVirtualDirectory = base.OnPremisesSession.GetWebServicesVirtualDirectory(null);
			foreach (ADWebServicesVirtualDirectory adwebServicesVirtualDirectory in webServicesVirtualDirectory)
			{
				if (ServerVersion.Compare(adwebServicesVirtualDirectory.AdminDisplayVersion, b) >= 0 && !adwebServicesVirtualDirectory.MRSProxyEnabled)
				{
					list.Add(adwebServicesVirtualDirectory);
				}
			}
			return list.ToArray();
		}
	}
}
