using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class CrossValidationResult
	{
		public double SuccessRate
		{
			get
			{
				return this.successRate;
			}
		}

		public double BlankRate
		{
			get
			{
				return this.blankRate;
			}
		}

		public double FailureRate
		{
			get
			{
				return this.failureRate;
			}
		}

		public CrossValidationResult(double success, double blank, double failure)
		{
			this.successRate = success;
			this.blankRate = blank;
			this.failureRate = failure;
		}

		private readonly double successRate;

		private readonly double blankRate;

		private readonly double failureRate;
	}
}
