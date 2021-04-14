using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ProvisioningObjectTagAttribute : Attribute
	{
		public ProvisioningObjectTagAttribute(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw new ArgumentNullException(tag);
			}
			this.tag = tag;
		}

		public string Tag
		{
			get
			{
				return this.tag;
			}
		}

		private string tag;
	}
}
