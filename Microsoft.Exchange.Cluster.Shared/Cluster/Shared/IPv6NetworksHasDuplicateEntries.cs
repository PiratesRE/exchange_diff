using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IPv6NetworksHasDuplicateEntries : ClusCommonValidationFailedException
	{
		public IPv6NetworksHasDuplicateEntries(string duplicate) : base(Strings.IPv6NetworksHasDuplicateEntries(duplicate))
		{
			this.duplicate = duplicate;
		}

		public IPv6NetworksHasDuplicateEntries(string duplicate, Exception innerException) : base(Strings.IPv6NetworksHasDuplicateEntries(duplicate), innerException)
		{
			this.duplicate = duplicate;
		}

		protected IPv6NetworksHasDuplicateEntries(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.duplicate = (string)info.GetValue("duplicate", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("duplicate", this.duplicate);
		}

		public string Duplicate
		{
			get
			{
				return this.duplicate;
			}
		}

		private readonly string duplicate;
	}
}
