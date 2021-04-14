using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SystemContainerNotFoundException : LocalizedException
	{
		public SystemContainerNotFoundException(string domain, Guid guid) : base(Strings.SystemContainerNotFoundException(domain, guid))
		{
			this.domain = domain;
			this.guid = guid;
		}

		public SystemContainerNotFoundException(string domain, Guid guid, Exception innerException) : base(Strings.SystemContainerNotFoundException(domain, guid), innerException)
		{
			this.domain = domain;
			this.guid = guid;
		}

		protected SystemContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domain = (string)info.GetValue("domain", typeof(string));
			this.guid = (Guid)info.GetValue("guid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domain", this.domain);
			info.AddValue("guid", this.guid);
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		private readonly string domain;

		private readonly Guid guid;
	}
}
