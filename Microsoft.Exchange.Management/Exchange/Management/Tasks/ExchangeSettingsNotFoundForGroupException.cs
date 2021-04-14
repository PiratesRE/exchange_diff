using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsNotFoundForGroupException : ExchangeSettingsException
	{
		public ExchangeSettingsNotFoundForGroupException(string groupName, string key) : base(Strings.ExchangeSettingsNotFoundForGroup(groupName, key))
		{
			this.groupName = groupName;
			this.key = key;
		}

		public ExchangeSettingsNotFoundForGroupException(string groupName, string key, Exception innerException) : base(Strings.ExchangeSettingsNotFoundForGroup(groupName, key), innerException)
		{
			this.groupName = groupName;
			this.key = key;
		}

		protected ExchangeSettingsNotFoundForGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupName = (string)info.GetValue("groupName", typeof(string));
			this.key = (string)info.GetValue("key", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupName", this.groupName);
			info.AddValue("key", this.key);
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		private readonly string groupName;

		private readonly string key;
	}
}
