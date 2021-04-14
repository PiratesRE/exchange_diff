using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.SmartCategorization
{
	internal class FailureDetails
	{
		public Component Component { get; private set; }

		public FailureType FailureType { get; private set; }

		public string Details { get; private set; }

		public string Categorization { get; private set; }

		public FailureDetails()
		{
			this.FailureType = FailureType.Unrecognized;
		}

		public FailureDetails(FailureType failureType, Component faultedComponent) : this(failureType, faultedComponent, string.Empty, string.Empty)
		{
		}

		public FailureDetails(FailureType failureType, Component faultedComponent, string details, string categorization)
		{
			if (faultedComponent == null)
			{
				throw new ArgumentNullException("faultedComponent");
			}
			if (details == null)
			{
				throw new ArgumentNullException("details");
			}
			this.FailureType = failureType;
			this.Component = faultedComponent;
			this.Details = details;
			this.Categorization = categorization;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Details))
			{
				return this.Details;
			}
			if (this.FailureType == FailureType.Unrecognized || this.Component == null)
			{
				return SCStrings.UnrecognizedFailure;
			}
			return string.Format(SCStrings.FailureDetails, this.FailureType.ToString(), this.Component.ShortName);
		}
	}
}
