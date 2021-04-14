using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class PostconditionAttribute : BasicPostconditionAttribute
	{
		public UpdatableHelpStrings.IDs FailureDescriptionId
		{
			get
			{
				return this.failureDescriptionId;
			}
			set
			{
				base.FailureDescription = UpdatableHelpStrings.GetLocalizedString(value);
				this.failureDescriptionId = value;
			}
		}

		private UpdatableHelpStrings.IDs failureDescriptionId;
	}
}
