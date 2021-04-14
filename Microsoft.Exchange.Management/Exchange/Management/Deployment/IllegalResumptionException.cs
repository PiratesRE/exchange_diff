using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IllegalResumptionException : LocalizedException
	{
		public IllegalResumptionException(string oldVerb, string newVerb) : base(Strings.IllegalResumptionException(oldVerb, newVerb))
		{
			this.oldVerb = oldVerb;
			this.newVerb = newVerb;
		}

		public IllegalResumptionException(string oldVerb, string newVerb, Exception innerException) : base(Strings.IllegalResumptionException(oldVerb, newVerb), innerException)
		{
			this.oldVerb = oldVerb;
			this.newVerb = newVerb;
		}

		protected IllegalResumptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.oldVerb = (string)info.GetValue("oldVerb", typeof(string));
			this.newVerb = (string)info.GetValue("newVerb", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("oldVerb", this.oldVerb);
			info.AddValue("newVerb", this.newVerb);
		}

		public string OldVerb
		{
			get
			{
				return this.oldVerb;
			}
		}

		public string NewVerb
		{
			get
			{
				return this.newVerb;
			}
		}

		private readonly string oldVerb;

		private readonly string newVerb;
	}
}
