using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdminSDHolderNotFoundException : LocalizedException
	{
		public AdminSDHolderNotFoundException(string system) : base(Strings.AdminSDHolderNotFoundException(system))
		{
			this.system = system;
		}

		public AdminSDHolderNotFoundException(string system, Exception innerException) : base(Strings.AdminSDHolderNotFoundException(system), innerException)
		{
			this.system = system;
		}

		protected AdminSDHolderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.system = (string)info.GetValue("system", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("system", this.system);
		}

		public string System
		{
			get
			{
				return this.system;
			}
		}

		private readonly string system;
	}
}
