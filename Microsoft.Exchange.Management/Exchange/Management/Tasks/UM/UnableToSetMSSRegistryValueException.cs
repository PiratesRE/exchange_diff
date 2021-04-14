using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToSetMSSRegistryValueException : LocalizedException
	{
		public UnableToSetMSSRegistryValueException(string registryKey, string exceptionMessage) : base(Strings.UnableToSetMSSRegistryValue(registryKey, exceptionMessage))
		{
			this.registryKey = registryKey;
			this.exceptionMessage = exceptionMessage;
		}

		public UnableToSetMSSRegistryValueException(string registryKey, string exceptionMessage, Exception innerException) : base(Strings.UnableToSetMSSRegistryValue(registryKey, exceptionMessage), innerException)
		{
			this.registryKey = registryKey;
			this.exceptionMessage = exceptionMessage;
		}

		protected UnableToSetMSSRegistryValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.registryKey = (string)info.GetValue("registryKey", typeof(string));
			this.exceptionMessage = (string)info.GetValue("exceptionMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("registryKey", this.registryKey);
			info.AddValue("exceptionMessage", this.exceptionMessage);
		}

		public string RegistryKey
		{
			get
			{
				return this.registryKey;
			}
		}

		public string ExceptionMessage
		{
			get
			{
				return this.exceptionMessage;
			}
		}

		private readonly string registryKey;

		private readonly string exceptionMessage;
	}
}
