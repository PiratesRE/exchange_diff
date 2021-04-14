using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RegistryParameterKeyNotOpenedException : RegistryParameterException
	{
		public RegistryParameterKeyNotOpenedException(string keyName) : base(ReplayStrings.RegistryParameterKeyNotOpenedException(keyName))
		{
			this.keyName = keyName;
		}

		public RegistryParameterKeyNotOpenedException(string keyName, Exception innerException) : base(ReplayStrings.RegistryParameterKeyNotOpenedException(keyName), innerException)
		{
			this.keyName = keyName;
		}

		protected RegistryParameterKeyNotOpenedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyName = (string)info.GetValue("keyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyName", this.keyName);
		}

		public string KeyName
		{
			get
			{
				return this.keyName;
			}
		}

		private readonly string keyName;
	}
}
