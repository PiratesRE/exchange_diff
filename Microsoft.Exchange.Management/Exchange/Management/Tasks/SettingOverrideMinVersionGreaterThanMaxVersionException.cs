using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideMinVersionGreaterThanMaxVersionException : SettingOverrideException
	{
		public SettingOverrideMinVersionGreaterThanMaxVersionException(string minVersion, string maxVersion) : base(Strings.ErrorMinVersionGreaterThanMaxVersion(minVersion, maxVersion))
		{
			this.minVersion = minVersion;
			this.maxVersion = maxVersion;
		}

		public SettingOverrideMinVersionGreaterThanMaxVersionException(string minVersion, string maxVersion, Exception innerException) : base(Strings.ErrorMinVersionGreaterThanMaxVersion(minVersion, maxVersion), innerException)
		{
			this.minVersion = minVersion;
			this.maxVersion = maxVersion;
		}

		protected SettingOverrideMinVersionGreaterThanMaxVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.minVersion = (string)info.GetValue("minVersion", typeof(string));
			this.maxVersion = (string)info.GetValue("maxVersion", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("minVersion", this.minVersion);
			info.AddValue("maxVersion", this.maxVersion);
		}

		public string MinVersion
		{
			get
			{
				return this.minVersion;
			}
		}

		public string MaxVersion
		{
			get
			{
				return this.maxVersion;
			}
		}

		private readonly string minVersion;

		private readonly string maxVersion;
	}
}
