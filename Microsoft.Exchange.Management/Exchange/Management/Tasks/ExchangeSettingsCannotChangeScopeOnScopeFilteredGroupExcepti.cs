using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException : ExchangeSettingsException
	{
		public ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException(string groupName) : base(Strings.ExchangeSettingsCannotChangeScopeOnScopeFilteredGroup(groupName))
		{
			this.groupName = groupName;
		}

		public ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException(string groupName, Exception innerException) : base(Strings.ExchangeSettingsCannotChangeScopeOnScopeFilteredGroup(groupName), innerException)
		{
			this.groupName = groupName;
		}

		protected ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
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
