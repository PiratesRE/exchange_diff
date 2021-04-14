using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AvailabilityConfigReadException : LocalizedException
	{
		public AvailabilityConfigReadException(string dn) : base(Strings.AvailabilityConfigReadException(dn))
		{
			this.dn = dn;
		}

		public AvailabilityConfigReadException(string dn, Exception innerException) : base(Strings.AvailabilityConfigReadException(dn), innerException)
		{
			this.dn = dn;
		}

		protected AvailabilityConfigReadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dn = (string)info.GetValue("dn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dn", this.dn);
		}

		public string Dn
		{
			get
			{
				return this.dn;
			}
		}

		private readonly string dn;
	}
}
