using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MaSGroupNotFoundException : LocalizedException
	{
		public MaSGroupNotFoundException(Guid guid) : base(Strings.MaSGroupNotFoundException(guid))
		{
			this.guid = guid;
		}

		public MaSGroupNotFoundException(Guid guid, Exception innerException) : base(Strings.MaSGroupNotFoundException(guid), innerException)
		{
			this.guid = guid;
		}

		protected MaSGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
