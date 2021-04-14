using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnwillingToPerformException : LocalizedException
	{
		public UnwillingToPerformException(string name, string dom) : base(Strings.UnwillingToPerformException(name, dom))
		{
			this.name = name;
			this.dom = dom;
		}

		public UnwillingToPerformException(string name, string dom, Exception innerException) : base(Strings.UnwillingToPerformException(name, dom), innerException)
		{
			this.name = name;
			this.dom = dom;
		}

		protected UnwillingToPerformException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.dom = (string)info.GetValue("dom", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("dom", this.dom);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Dom
		{
			get
			{
				return this.dom;
			}
		}

		private readonly string name;

		private readonly string dom;
	}
}
