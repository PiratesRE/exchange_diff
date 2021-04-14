using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.OutlookProtectionRules
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OutlookProtectionRuleNameIsNotUniqueException : LocalizedException
	{
		public OutlookProtectionRuleNameIsNotUniqueException(string name) : base(Strings.OutlookProtectionRuleNameIsNotUnique(name))
		{
			this.name = name;
		}

		public OutlookProtectionRuleNameIsNotUniqueException(string name, Exception innerException) : base(Strings.OutlookProtectionRuleNameIsNotUnique(name), innerException)
		{
			this.name = name;
		}

		protected OutlookProtectionRuleNameIsNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
