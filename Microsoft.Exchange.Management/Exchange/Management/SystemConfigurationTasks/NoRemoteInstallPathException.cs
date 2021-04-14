using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoRemoteInstallPathException : LocalizedException
	{
		public NoRemoteInstallPathException(string s) : base(Strings.NoRemoteInstallPath(s))
		{
			this.s = s;
		}

		public NoRemoteInstallPathException(string s, Exception innerException) : base(Strings.NoRemoteInstallPath(s), innerException)
		{
			this.s = s;
		}

		protected NoRemoteInstallPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.s = (string)info.GetValue("s", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("s", this.s);
		}

		public string S
		{
			get
			{
				return this.s;
			}
		}

		private readonly string s;
	}
}
