using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ManageIsapiExtensionFoundMoreThanOneExtensionException : LocalizedException
	{
		public ManageIsapiExtensionFoundMoreThanOneExtensionException(string groupId, string extensionBinary, string matches) : base(Strings.ManageIsapiExtensionFoundMoreThanOneExtensionException(groupId, extensionBinary, matches))
		{
			this.groupId = groupId;
			this.extensionBinary = extensionBinary;
			this.matches = matches;
		}

		public ManageIsapiExtensionFoundMoreThanOneExtensionException(string groupId, string extensionBinary, string matches, Exception innerException) : base(Strings.ManageIsapiExtensionFoundMoreThanOneExtensionException(groupId, extensionBinary, matches), innerException)
		{
			this.groupId = groupId;
			this.extensionBinary = extensionBinary;
			this.matches = matches;
		}

		protected ManageIsapiExtensionFoundMoreThanOneExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupId = (string)info.GetValue("groupId", typeof(string));
			this.extensionBinary = (string)info.GetValue("extensionBinary", typeof(string));
			this.matches = (string)info.GetValue("matches", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupId", this.groupId);
			info.AddValue("extensionBinary", this.extensionBinary);
			info.AddValue("matches", this.matches);
		}

		public string GroupId
		{
			get
			{
				return this.groupId;
			}
		}

		public string ExtensionBinary
		{
			get
			{
				return this.extensionBinary;
			}
		}

		public string Matches
		{
			get
			{
				return this.matches;
			}
		}

		private readonly string groupId;

		private readonly string extensionBinary;

		private readonly string matches;
	}
}
