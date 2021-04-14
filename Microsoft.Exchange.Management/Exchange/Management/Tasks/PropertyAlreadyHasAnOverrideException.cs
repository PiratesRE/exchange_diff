using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PropertyAlreadyHasAnOverrideException : LocalizedException
	{
		public PropertyAlreadyHasAnOverrideException(string property, string overrideName, string workitemType) : base(Strings.PropertyAlreadyHasAnOverride(property, overrideName, workitemType))
		{
			this.property = property;
			this.overrideName = overrideName;
			this.workitemType = workitemType;
		}

		public PropertyAlreadyHasAnOverrideException(string property, string overrideName, string workitemType, Exception innerException) : base(Strings.PropertyAlreadyHasAnOverride(property, overrideName, workitemType), innerException)
		{
			this.property = property;
			this.overrideName = overrideName;
			this.workitemType = workitemType;
		}

		protected PropertyAlreadyHasAnOverrideException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
			this.overrideName = (string)info.GetValue("overrideName", typeof(string));
			this.workitemType = (string)info.GetValue("workitemType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
			info.AddValue("overrideName", this.overrideName);
			info.AddValue("workitemType", this.workitemType);
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		public string OverrideName
		{
			get
			{
				return this.overrideName;
			}
		}

		public string WorkitemType
		{
			get
			{
				return this.workitemType;
			}
		}

		private readonly string property;

		private readonly string overrideName;

		private readonly string workitemType;
	}
}
