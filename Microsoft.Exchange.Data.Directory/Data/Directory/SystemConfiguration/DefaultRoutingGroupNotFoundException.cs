using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DefaultRoutingGroupNotFoundException : MandatoryContainerNotFoundException
	{
		public DefaultRoutingGroupNotFoundException(string rgName) : base(DirectoryStrings.DefaultRoutingGroupNotFoundException(rgName))
		{
			this.rgName = rgName;
		}

		public DefaultRoutingGroupNotFoundException(string rgName, Exception innerException) : base(DirectoryStrings.DefaultRoutingGroupNotFoundException(rgName), innerException)
		{
			this.rgName = rgName;
		}

		protected DefaultRoutingGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.rgName = (string)info.GetValue("rgName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("rgName", this.rgName);
		}

		public string RgName
		{
			get
			{
				return this.rgName;
			}
		}

		private readonly string rgName;
	}
}
