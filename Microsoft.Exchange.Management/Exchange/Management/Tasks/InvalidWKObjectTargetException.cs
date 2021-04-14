using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidWKObjectTargetException : LocalizedException
	{
		public InvalidWKObjectTargetException(string guid, string container, string target, string groupType) : base(Strings.InvalidWKObjectTargetException(guid, container, target, groupType))
		{
			this.guid = guid;
			this.container = container;
			this.target = target;
			this.groupType = groupType;
		}

		public InvalidWKObjectTargetException(string guid, string container, string target, string groupType, Exception innerException) : base(Strings.InvalidWKObjectTargetException(guid, container, target, groupType), innerException)
		{
			this.guid = guid;
			this.container = container;
			this.target = target;
			this.groupType = groupType;
		}

		protected InvalidWKObjectTargetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.guid = (string)info.GetValue("guid", typeof(string));
			this.container = (string)info.GetValue("container", typeof(string));
			this.target = (string)info.GetValue("target", typeof(string));
			this.groupType = (string)info.GetValue("groupType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("guid", this.guid);
			info.AddValue("container", this.container);
			info.AddValue("target", this.target);
			info.AddValue("groupType", this.groupType);
		}

		public string Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string Container
		{
			get
			{
				return this.container;
			}
		}

		public string Target
		{
			get
			{
				return this.target;
			}
		}

		public string GroupType
		{
			get
			{
				return this.groupType;
			}
		}

		private readonly string guid;

		private readonly string container;

		private readonly string target;

		private readonly string groupType;
	}
}
