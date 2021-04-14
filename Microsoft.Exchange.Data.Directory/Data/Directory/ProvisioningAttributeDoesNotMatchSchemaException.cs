using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ProvisioningAttributeDoesNotMatchSchemaException : InvalidMailboxProvisioningAttributeException
	{
		public ProvisioningAttributeDoesNotMatchSchemaException(string keyName, string validKeys) : base(DirectoryStrings.ErrorMailboxProvisioningAttributeDoesNotMatchSchema(keyName, validKeys))
		{
			this.keyName = keyName;
			this.validKeys = validKeys;
		}

		public ProvisioningAttributeDoesNotMatchSchemaException(string keyName, string validKeys, Exception innerException) : base(DirectoryStrings.ErrorMailboxProvisioningAttributeDoesNotMatchSchema(keyName, validKeys), innerException)
		{
			this.keyName = keyName;
			this.validKeys = validKeys;
		}

		protected ProvisioningAttributeDoesNotMatchSchemaException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyName = (string)info.GetValue("keyName", typeof(string));
			this.validKeys = (string)info.GetValue("validKeys", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyName", this.keyName);
			info.AddValue("validKeys", this.validKeys);
		}

		public string KeyName
		{
			get
			{
				return this.keyName;
			}
		}

		public string ValidKeys
		{
			get
			{
				return this.validKeys;
			}
		}

		private readonly string keyName;

		private readonly string validKeys;
	}
}
