using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsScopeAlreadyExistsException : ExchangeSettingsException
	{
		public ExchangeSettingsScopeAlreadyExistsException(string groupName, string id) : base(Strings.ExchangeSettingsScopeAlreadyExists(groupName, id))
		{
			this.groupName = groupName;
			this.id = id;
		}

		public ExchangeSettingsScopeAlreadyExistsException(string groupName, string id, Exception innerException) : base(Strings.ExchangeSettingsScopeAlreadyExists(groupName, id), innerException)
		{
			this.groupName = groupName;
			this.id = id;
		}

		protected ExchangeSettingsScopeAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupName = (string)info.GetValue("groupName", typeof(string));
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupName", this.groupName);
			info.AddValue("id", this.id);
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string groupName;

		private readonly string id;
	}
}
