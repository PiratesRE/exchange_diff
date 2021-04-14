using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OverrideNotFoundException : LocalizedException
	{
		public OverrideNotFoundException(string identity, string type, string propertyName) : base(Strings.OverrideNotFound(identity, type, propertyName))
		{
			this.identity = identity;
			this.type = type;
			this.propertyName = propertyName;
		}

		public OverrideNotFoundException(string identity, string type, string propertyName, Exception innerException) : base(Strings.OverrideNotFound(identity, type, propertyName), innerException)
		{
			this.identity = identity;
			this.type = type;
			this.propertyName = propertyName;
		}

		protected OverrideNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("type", this.type);
			info.AddValue("propertyName", this.propertyName);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string identity;

		private readonly string type;

		private readonly string propertyName;
	}
}
