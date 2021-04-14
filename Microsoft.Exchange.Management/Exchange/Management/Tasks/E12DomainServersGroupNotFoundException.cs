using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class E12DomainServersGroupNotFoundException : LocalizedException
	{
		public E12DomainServersGroupNotFoundException(string dn, string dc) : base(Strings.E12DomainServersGroupNotFoundException(dn, dc))
		{
			this.dn = dn;
			this.dc = dc;
		}

		public E12DomainServersGroupNotFoundException(string dn, string dc, Exception innerException) : base(Strings.E12DomainServersGroupNotFoundException(dn, dc), innerException)
		{
			this.dn = dn;
			this.dc = dc;
		}

		protected E12DomainServersGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dn = (string)info.GetValue("dn", typeof(string));
			this.dc = (string)info.GetValue("dc", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dn", this.dn);
			info.AddValue("dc", this.dc);
		}

		public string Dn
		{
			get
			{
				return this.dn;
			}
		}

		public string Dc
		{
			get
			{
				return this.dc;
			}
		}

		private readonly string dn;

		private readonly string dc;
	}
}
