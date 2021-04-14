using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MultipleOrgMbxesWithCapabilityException : LocalizedException
	{
		public MultipleOrgMbxesWithCapabilityException(string capability) : base(Strings.MultipleOrgMbxesWithCapability(capability))
		{
			this.capability = capability;
		}

		public MultipleOrgMbxesWithCapabilityException(string capability, Exception innerException) : base(Strings.MultipleOrgMbxesWithCapability(capability), innerException)
		{
			this.capability = capability;
		}

		protected MultipleOrgMbxesWithCapabilityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.capability = (string)info.GetValue("capability", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("capability", this.capability);
		}

		public string Capability
		{
			get
			{
				return this.capability;
			}
		}

		private readonly string capability;
	}
}
