using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExForeignForestSGroupNotFoundException : LocalizedException
	{
		public ExForeignForestSGroupNotFoundException(string name, string domain) : base(Strings.ExForeignForestSGroupNotFoundException(name, domain))
		{
			this.name = name;
			this.domain = domain;
		}

		public ExForeignForestSGroupNotFoundException(string name, string domain, Exception innerException) : base(Strings.ExForeignForestSGroupNotFoundException(name, domain), innerException)
		{
			this.name = name;
			this.domain = domain;
		}

		protected ExForeignForestSGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.domain = (string)info.GetValue("domain", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("domain", this.domain);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		private readonly string name;

		private readonly string domain;
	}
}
