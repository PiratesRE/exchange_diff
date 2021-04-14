using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TenantIsRelocatedException : ADTransientException
	{
		public TenantIsRelocatedException(string dn) : base(DirectoryStrings.TenantIsRelocatedException(dn))
		{
			this.dn = dn;
		}

		public TenantIsRelocatedException(string dn, Exception innerException) : base(DirectoryStrings.TenantIsRelocatedException(dn), innerException)
		{
			this.dn = dn;
		}

		protected TenantIsRelocatedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
