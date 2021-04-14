using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	internal sealed class DiagnosticStrideJustificationAttribute : Attribute
	{
		public string SpoofingJustification { get; set; }

		public string TamperingJustification { get; set; }

		public string RepudiationJustification { get; set; }

		public string InformationDisclosureJustification { get; set; }

		public string DenialOfServiceJustification { get; set; }

		public string ElevationOfPrivligesJustification { get; set; }

		public bool IsAuthorizedForSupportDiagnosticRoleUse
		{
			get
			{
				return !string.IsNullOrEmpty(this.SpoofingJustification) && !string.IsNullOrEmpty(this.TamperingJustification) && !string.IsNullOrEmpty(this.RepudiationJustification) && !string.IsNullOrEmpty(this.InformationDisclosureJustification) && !string.IsNullOrEmpty(this.DenialOfServiceJustification) && !string.IsNullOrEmpty(this.ElevationOfPrivligesJustification);
			}
		}
	}
}
