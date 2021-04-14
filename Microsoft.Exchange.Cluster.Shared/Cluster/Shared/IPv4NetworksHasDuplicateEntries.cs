using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IPv4NetworksHasDuplicateEntries : ClusCommonValidationFailedException
	{
		public IPv4NetworksHasDuplicateEntries(string duplicate) : base(Strings.IPv4NetworksHasDuplicateEntries(duplicate))
		{
			this.duplicate = duplicate;
		}

		public IPv4NetworksHasDuplicateEntries(string duplicate, Exception innerException) : base(Strings.IPv4NetworksHasDuplicateEntries(duplicate), innerException)
		{
			this.duplicate = duplicate;
		}

		protected IPv4NetworksHasDuplicateEntries(SerializationInfo info, StreamingContext context) : base(info, context)
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
