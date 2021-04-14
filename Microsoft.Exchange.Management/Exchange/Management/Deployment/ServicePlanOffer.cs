using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class ServicePlanOffer
	{
		public string ProgramId { get; private set; }

		public string OfferId { get; private set; }

		internal ServicePlanOffer(string programId, string offerId)
		{
			this.ProgramId = programId;
			this.OfferId = offerId;
		}

		public override string ToString()
		{
			return (this.ProgramId ?? string.Empty) + "-" + (this.OfferId ?? string.Empty);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is ServicePlanOffer)
			{
				ServicePlanOffer servicePlanOffer = obj as ServicePlanOffer;
				return string.Equals(this.ProgramId, servicePlanOffer.ProgramId, StringComparison.OrdinalIgnoreCase) && string.Equals(this.OfferId, servicePlanOffer.OfferId, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public override int GetHashCode()
		{
			string text = this.ToString().ToLower();
			return text.GetHashCode();
		}
	}
}
