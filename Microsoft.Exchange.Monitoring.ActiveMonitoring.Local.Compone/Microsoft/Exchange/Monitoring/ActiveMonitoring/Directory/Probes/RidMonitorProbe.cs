using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class RidMonitorProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (!propertyBag.ContainsKey("RidsLeftThreshold"))
			{
				throw new ArgumentException("Please specify value forRidsLeftThreshold");
			}
			pDef.Attributes["RidsLeftThreshold"] = propertyBag["RidsLeftThreshold"].ToString().Trim();
			if (!propertyBag.ContainsKey("RidsLeftLimit"))
			{
				throw new ArgumentException("Please specify value forRidsLeftLimit");
			}
			pDef.Attributes["RidsLeftLimit"] = propertyBag["RidsLeftLimit"].ToString().Trim();
			if (!propertyBag.ContainsKey("RidsLeftLimitLowValue"))
			{
				throw new ArgumentException("Please specify value forRidsLeftLimitSDF");
			}
			pDef.Attributes["RidsLeftLimitLowValue"] = propertyBag["RidsLeftLimitLowValue"].ToString().Trim();
			if (propertyBag.ContainsKey("RidsLeftLimitSDF"))
			{
				pDef.Attributes["RidsLeftLimitSDF"] = propertyBag["RidsLeftLimitSDF"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value forRidsLeftLimitSDF");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestRidMonitor, delegate
			{
				if (!DirectoryUtils.IsRidMaster())
				{
					this.Result.StateAttribute5 = "This DC is not a RID master.  Probe will be skipped.";
					return;
				}
				int ridsLeft = DirectoryUtils.GetRidsLeft();
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				if (!this.Definition.Attributes.ContainsKey("RidsLeftThreshold"))
				{
					stringBuilder.Append(string.Format("ERROR:  Argument Exception found:  {0}\n", "RidsLeftThreshold"));
					flag = true;
				}
				else
				{
					int num = int.Parse(this.Definition.Attributes["RidsLeftThreshold"]);
					this.Result.StateAttribute1 = ridsLeft.ToString();
					ProbeResult lastProbeResult = DirectoryUtils.GetLastProbeResult(this, this.Broker, cancellationToken);
					if (lastProbeResult == null)
					{
						this.Result.StateAttribute2 = "This is first monitoring occurrence for Rid Monitor Probe";
					}
					else if (lastProbeResult.StateAttribute1 != null)
					{
						int num2 = int.Parse(lastProbeResult.StateAttribute1);
						this.Result.StateAttribute3 = (num2 - ridsLeft).ToString();
						if (num2 - ridsLeft > num)
						{
							stringBuilder.Append(string.Format("Rids used {0} Threshold {1}\n", num2 - ridsLeft, num));
							flag = true;
						}
					}
				}
				bool flag2 = false;
				if (!this.Definition.Attributes.ContainsKey("RidsLeftLimit"))
				{
					stringBuilder.Append(string.Format("ERROR:  Argument Exception found:  {0}\n", "RidsLeftLimit"));
					flag2 = true;
				}
				if (!this.Definition.Attributes.ContainsKey("RidsLeftLimitLowValue"))
				{
					stringBuilder.Append(string.Format("ERROR:  Argument Exception found:  {0}\n", "RidsLeftLimitLowValue"));
					flag2 = true;
				}
				if (!this.Definition.Attributes.ContainsKey("RidsLeftLimitSDF"))
				{
					stringBuilder.Append(string.Format("ERROR:  Argument Exception found:  {0}\n", "RidsLeftLimitSDF"));
					flag2 = true;
				}
				if (!flag2)
				{
					int num3 = int.Parse(this.Definition.Attributes["RidsLeftLimit"]);
					int num4 = int.Parse(this.Definition.Attributes["RidsLeftLimitLowValue"]);
					int num5 = int.Parse(this.Definition.Attributes["RidsLeftLimitSDF"]);
					int num6 = num3;
					if (num3 > 0 && num4 > 0 && num5 > 0)
					{
						string localFQDN = DirectoryGeneralUtils.GetLocalFQDN();
						if (localFQDN.Contains("namsdf01"))
						{
							num6 = num5;
						}
						else if (localFQDN.Contains("namprd02"))
						{
							num6 = num4;
						}
						if (ridsLeft < num6)
						{
							stringBuilder.Append(string.Format("ERROR:  Rid limit has reached in the forest.  Current Rids Left is: {0}\n", ridsLeft));
							flag2 = true;
						}
						this.Result.StateAttribute4 = string.Format("RIDs left is {0}, which is within the set limit in the forest:  {1}", ridsLeft, num6);
					}
				}
				if (flag || flag2)
				{
					throw new Exception(stringBuilder.ToString());
				}
			});
		}
	}
}
