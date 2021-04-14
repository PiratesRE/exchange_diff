using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LocalServerNotFoundException : ADOperationException
	{
		public LocalServerNotFoundException(string fqdn) : base(DirectoryStrings.LocalServerNotFound(fqdn))
		{
			this.fqdn = fqdn;
		}

		public LocalServerNotFoundException(string fqdn, Exception innerException) : base(DirectoryStrings.LocalServerNotFound(fqdn), innerException)
		{
			this.fqdn = fqdn;
		}

		protected LocalServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fqdn = (string)info.GetValue("fqdn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fqdn", this.fqdn);
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		private readonly string fqdn;
	}
}
