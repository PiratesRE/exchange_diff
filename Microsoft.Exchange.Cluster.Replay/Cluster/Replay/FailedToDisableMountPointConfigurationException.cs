using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToDisableMountPointConfigurationException : DatabaseVolumeInfoException
	{
		public FailedToDisableMountPointConfigurationException(string regkeyroot) : base(ReplayStrings.FailedToDisableMountPointConfigurationException(regkeyroot))
		{
			this.regkeyroot = regkeyroot;
		}

		public FailedToDisableMountPointConfigurationException(string regkeyroot, Exception innerException) : base(ReplayStrings.FailedToDisableMountPointConfigurationException(regkeyroot), innerException)
		{
			this.regkeyroot = regkeyroot;
		}

		protected FailedToDisableMountPointConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.regkeyroot = (string)info.GetValue("regkeyroot", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("regkeyroot", this.regkeyroot);
		}

		public string Regkeyroot
		{
			get
			{
				return this.regkeyroot;
			}
		}

		private readonly string regkeyroot;
	}
}
