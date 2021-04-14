using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class RecipientPickerFilterBase : RecipientFilter
	{
		public override string RbacScope
		{
			get
			{
				return string.Empty;
			}
		}

		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return null;
			}
		}
	}
}
