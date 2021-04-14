using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RegistryValueMissingOrInvalidException : LocalizedException
	{
		public RegistryValueMissingOrInvalidException(string keyPath, string valueName) : base(Strings.RegistryValueMissingOrInvalidException(keyPath, valueName))
		{
			this.keyPath = keyPath;
			this.valueName = valueName;
		}

		public RegistryValueMissingOrInvalidException(string keyPath, string valueName, Exception innerException) : base(Strings.RegistryValueMissingOrInvalidException(keyPath, valueName), innerException)
		{
			this.keyPath = keyPath;
			this.valueName = valueName;
		}

		protected RegistryValueMissingOrInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyPath = (string)info.GetValue("keyPath", typeof(string));
			this.valueName = (string)info.GetValue("valueName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyPath", this.keyPath);
			info.AddValue("valueName", this.valueName);
		}

		public string KeyPath
		{
			get
			{
				return this.keyPath;
			}
		}

		public string ValueName
		{
			get
			{
				return this.valueName;
			}
		}

		private readonly string keyPath;

		private readonly string valueName;
	}
}
