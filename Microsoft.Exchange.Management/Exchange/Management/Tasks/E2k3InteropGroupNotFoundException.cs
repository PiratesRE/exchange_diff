using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class E2k3InteropGroupNotFoundException : LocalizedException
	{
		public E2k3InteropGroupNotFoundException(Guid guid) : base(Strings.E2k3InteropGroupNotFoundException(guid))
		{
			this.guid = guid;
		}

		public E2k3InteropGroupNotFoundException(Guid guid, Exception innerException) : base(Strings.E2k3InteropGroupNotFoundException(guid), innerException)
		{
			this.guid = guid;
		}

		protected E2k3InteropGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
