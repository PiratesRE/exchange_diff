using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.RbacDefinition;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExOrgReadAdminSGroupNotFoundException : LocalizedException
	{
		public ExOrgReadAdminSGroupNotFoundException(Guid guid) : base(Strings.ExOrgReadAdminSGroupNotFoundException(guid))
		{
			this.guid = guid;
		}

		public ExOrgReadAdminSGroupNotFoundException(Guid guid, Exception innerException) : base(Strings.ExOrgReadAdminSGroupNotFoundException(guid), innerException)
		{
			this.guid = guid;
		}

		protected ExOrgReadAdminSGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
