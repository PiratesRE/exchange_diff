using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoAvailableDefaultNamePermanentException : ManagementObjectAlreadyExistsException
	{
		public NoAvailableDefaultNamePermanentException(string mbx) : base(Strings.ErrorNoAvailableDefaultName(mbx))
		{
			this.mbx = mbx;
		}

		public NoAvailableDefaultNamePermanentException(string mbx, Exception innerException) : base(Strings.ErrorNoAvailableDefaultName(mbx), innerException)
		{
			this.mbx = mbx;
		}

		protected NoAvailableDefaultNamePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbx = (string)info.GetValue("mbx", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbx", this.mbx);
		}

		public string Mbx
		{
			get
			{
				return this.mbx;
			}
		}

		private readonly string mbx;
	}
}
