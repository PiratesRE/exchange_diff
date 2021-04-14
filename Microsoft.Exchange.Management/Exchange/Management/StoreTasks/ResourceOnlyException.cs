using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ResourceOnlyException : LocalizedException
	{
		public ResourceOnlyException(string parm) : base(Strings.ResourceOnly(parm))
		{
			this.parm = parm;
		}

		public ResourceOnlyException(string parm, Exception innerException) : base(Strings.ResourceOnly(parm), innerException)
		{
			this.parm = parm;
		}

		protected ResourceOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parm = (string)info.GetValue("parm", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parm", this.parm);
		}

		public string Parm
		{
			get
			{
				return this.parm;
			}
		}

		private readonly string parm;
	}
}
