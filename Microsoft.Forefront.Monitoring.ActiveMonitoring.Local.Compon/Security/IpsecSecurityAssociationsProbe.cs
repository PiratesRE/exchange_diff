using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Security
{
	public class IpsecSecurityAssociationsProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.logger = new ProbeWorkItemLogger(this, false, true);
			this.logger.LogMessage("IpsecSecurityAssociationsProbe started");
			this.logger.LogMessage("Verifying quick mode");
			this.VerifyAssociations("VerifyQuickModeSecurityAssociations", "QuickModeSecurityAssociationsMinCount", NetHelpers.GetQuickModeSecurityAssociations());
			this.logger.LogMessage("Verifying main mode");
			this.VerifyAssociations("VerifyMainModeSecurityAssociations", "MainModeSecurityAssociationsMinCount", NetHelpers.GetMainModeSecurityAssociations());
		}

		private void VerifyAssociations(string attributeShouldVerify, string attributeMinCount, List<Dictionary<string, string>> associations)
		{
			bool flag = bool.Parse(ProbeHelper.GetExtensionAttribute(this.logger, this, attributeShouldVerify));
			int num = int.Parse(ProbeHelper.GetExtensionAttribute(this.logger, this, attributeMinCount));
			this.logger.LogMessage(string.Format("associations.Count:{0}", associations.Count));
			if (!flag)
			{
				this.logger.LogMessage("Skipping SA verification");
				return;
			}
			if (associations.Count < num)
			{
				throw new Exception(string.Format("SA count is {0} but should be at least {1}", associations.Count, num));
			}
			this.logger.LogMessage(string.Format("SA count is {0} which is greater than min {1}", associations.Count, num));
		}

		private IHygieneLogger logger = new NullHygieneLogger();
	}
}
