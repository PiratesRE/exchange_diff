using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TenantIsArrivingException : ADTransientException
	{
		public TenantIsArrivingException(string dn) : base(DirectoryStrings.TenantIsArrivingException(dn))
		{
			this.dn = dn;
		}

		public TenantIsArrivingException(string dn, Exception innerException) : base(DirectoryStrings.TenantIsArrivingException(dn), innerException)
		{
			this.dn = dn;
		}

		protected TenantIsArrivingException(SerializationInfo info, StreamingContext context) : base(info, context)
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
