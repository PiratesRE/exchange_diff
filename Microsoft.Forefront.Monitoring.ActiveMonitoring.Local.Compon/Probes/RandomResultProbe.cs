using System;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Probes
{
	public class RandomResultProbe : ProbeWorkItem
	{
		public RandomResultProbe()
		{
			RandomResultProbe.random = new Random();
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			int num = 50;
			if (base.Definition != null && base.Definition.Attributes != null && base.Definition.Attributes.ContainsKey("PassPercentage"))
			{
				string text = base.Definition.Attributes["PassPercentage"];
				if (!int.TryParse(text, out num) || num < 0 || num > 100)
				{
					ProbeResult result = base.Result;
					result.ExecutionContext += string.Format("Invalid value \"{0}\" for {1}. Valid values are from 0 to 100 inclusive. Default value of {2} will be used. ", text, "PassPercentage", 50);
					num = 50;
				}
			}
			int num2 = RandomResultProbe.random.Next(1, 101);
			bool flag = num2 <= num;
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += string.Format("Random number was {0}. In order for the RandomResultProbe to pass, this number must be less than or equal to the {1} value of {2}. ", num2, "PassPercentage", num);
			if (!flag)
			{
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += "RandomResultProbe will fail.";
				base.Result.FailureContext = "Sometimes bad things happen to good probes.";
				throw new Exception("The RandomResultProbe failed.");
			}
			ProbeResult result4 = base.Result;
			result4.ExecutionContext += "RandomResultProbe has passed.";
		}

		private static Random random;
	}
}
