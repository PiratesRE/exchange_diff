using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NameMustBeUniquePermanentException : ManagementObjectAlreadyExistsException
	{
		public NameMustBeUniquePermanentException(string name, string mbx) : base(Strings.ErrorNameMustBeUniquePerMailbox(name, mbx))
		{
			this.name = name;
			this.mbx = mbx;
		}

		public NameMustBeUniquePermanentException(string name, string mbx, Exception innerException) : base(Strings.ErrorNameMustBeUniquePerMailbox(name, mbx), innerException)
		{
			this.name = name;
			this.mbx = mbx;
		}

		protected NameMustBeUniquePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.mbx = (string)info.GetValue("mbx", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("mbx", this.mbx);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Mbx
		{
			get
			{
				return this.mbx;
			}
		}

		private readonly string name;

		private readonly string mbx;
	}
}
