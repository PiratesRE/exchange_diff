using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ManageIsapiExtensionCouldNotFindExtensionException : LocalizedException
	{
		public ManageIsapiExtensionCouldNotFindExtensionException(string groupId, string binary) : base(Strings.ManageIsapiExtensionCouldNotFindExtensionException(groupId, binary))
		{
			this.groupId = groupId;
			this.binary = binary;
		}

		public ManageIsapiExtensionCouldNotFindExtensionException(string groupId, string binary, Exception innerException) : base(Strings.ManageIsapiExtensionCouldNotFindExtensionException(groupId, binary), innerException)
		{
			this.groupId = groupId;
			this.binary = binary;
		}

		protected ManageIsapiExtensionCouldNotFindExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupId = (string)info.GetValue("groupId", typeof(string));
			this.binary = (string)info.GetValue("binary", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupId", this.groupId);
			info.AddValue("binary", this.binary);
		}

		public string GroupId
		{
			get
			{
				return this.groupId;
			}
		}

		public string Binary
		{
			get
			{
				return this.binary;
			}
		}

		private readonly string groupId;

		private readonly string binary;
	}
}
