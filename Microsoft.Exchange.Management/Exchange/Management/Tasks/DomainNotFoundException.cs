using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DomainNotFoundException : LocalizedException
	{
		public DomainNotFoundException(string dom) : base(Strings.DomainNotFoundException(dom))
		{
			this.dom = dom;
		}

		public DomainNotFoundException(string dom, Exception innerException) : base(Strings.DomainNotFoundException(dom), innerException)
		{
			this.dom = dom;
		}

		protected DomainNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dom = (string)info.GetValue("dom", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dom", this.dom);
		}

		public string Dom
		{
			get
			{
				return this.dom;
			}
		}

		private readonly string dom;
	}
}
