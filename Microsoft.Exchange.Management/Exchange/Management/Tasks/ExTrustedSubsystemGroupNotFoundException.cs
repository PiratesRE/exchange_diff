using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExTrustedSubsystemGroupNotFoundException : LocalizedException
	{
		public ExTrustedSubsystemGroupNotFoundException(Guid guid) : base(Strings.ExTrustedSubsystemGroupNotFoundException(guid))
		{
			this.guid = guid;
		}

		public ExTrustedSubsystemGroupNotFoundException(Guid guid, Exception innerException) : base(Strings.ExTrustedSubsystemGroupNotFoundException(guid), innerException)
		{
			this.guid = guid;
		}

		protected ExTrustedSubsystemGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.guid = (Guid)info.GetValue("guid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("guid", this.guid);
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		private readonly Guid guid;
	}
}
