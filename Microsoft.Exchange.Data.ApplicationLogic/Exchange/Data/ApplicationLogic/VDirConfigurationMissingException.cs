using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class VDirConfigurationMissingException : DataSourceOperationException
	{
		public VDirConfigurationMissingException(string lastServer, string urlType, string missingServiceType) : base(Strings.ErrorVDirConfigurationMissing(lastServer, urlType, missingServiceType))
		{
			this.lastServer = lastServer;
			this.urlType = urlType;
			this.missingServiceType = missingServiceType;
		}

		public VDirConfigurationMissingException(string lastServer, string urlType, string missingServiceType, Exception innerException) : base(Strings.ErrorVDirConfigurationMissing(lastServer, urlType, missingServiceType), innerException)
		{
			this.lastServer = lastServer;
			this.urlType = urlType;
			this.missingServiceType = missingServiceType;
		}

		protected VDirConfigurationMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.lastServer = (string)info.GetValue("lastServer", typeof(string));
			this.urlType = (string)info.GetValue("urlType", typeof(string));
			this.missingServiceType = (string)info.GetValue("missingServiceType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lastServer", this.lastServer);
			info.AddValue("urlType", this.urlType);
			info.AddValue("missingServiceType", this.missingServiceType);
		}

		public string LastServer
		{
			get
			{
				return this.lastServer;
			}
		}

		public string UrlType
		{
			get
			{
				return this.urlType;
			}
		}

		public string MissingServiceType
		{
			get
			{
				return this.missingServiceType;
			}
		}

		private readonly string lastServer;

		private readonly string urlType;

		private readonly string missingServiceType;
	}
}
