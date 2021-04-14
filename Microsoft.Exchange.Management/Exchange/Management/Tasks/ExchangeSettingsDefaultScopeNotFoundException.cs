using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsDefaultScopeNotFoundException : ExchangeSettingsException
	{
		public ExchangeSettingsDefaultScopeNotFoundException(string groupName) : base(Strings.ExchangeSettingsDefaultScopeNotFound(groupName))
		{
			this.groupName = groupName;
		}

		public ExchangeSettingsDefaultScopeNotFoundException(string groupName, Exception innerException) : base(Strings.ExchangeSettingsDefaultScopeNotFound(groupName), innerException)
		{
			this.groupName = groupName;
		}

		protected ExchangeSettingsDefaultScopeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupName = (string)info.GetValue("groupName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupName", this.groupName);
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		private readonly string groupName;
	}
}
