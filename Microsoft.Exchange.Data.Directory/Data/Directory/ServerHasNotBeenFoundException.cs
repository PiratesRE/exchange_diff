using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerHasNotBeenFoundException : ADOperationException
	{
		public ServerHasNotBeenFoundException(int versionNumber, string identifier, bool needsExactVersionMatch, string siteName) : base(DirectoryStrings.ServerHasNotBeenFound(versionNumber, identifier, needsExactVersionMatch, siteName))
		{
			this.versionNumber = versionNumber;
			this.identifier = identifier;
			this.needsExactVersionMatch = needsExactVersionMatch;
			this.siteName = siteName;
		}

		public ServerHasNotBeenFoundException(int versionNumber, string identifier, bool needsExactVersionMatch, string siteName, Exception innerException) : base(DirectoryStrings.ServerHasNotBeenFound(versionNumber, identifier, needsExactVersionMatch, siteName), innerException)
		{
			this.versionNumber = versionNumber;
			this.identifier = identifier;
			this.needsExactVersionMatch = needsExactVersionMatch;
			this.siteName = siteName;
		}

		protected ServerHasNotBeenFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.versionNumber = (int)info.GetValue("versionNumber", typeof(int));
			this.identifier = (string)info.GetValue("identifier", typeof(string));
			this.needsExactVersionMatch = (bool)info.GetValue("needsExactVersionMatch", typeof(bool));
			this.siteName = (string)info.GetValue("siteName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("versionNumber", this.versionNumber);
			info.AddValue("identifier", this.identifier);
			info.AddValue("needsExactVersionMatch", this.needsExactVersionMatch);
			info.AddValue("siteName", this.siteName);
		}

		public int VersionNumber
		{
			get
			{
				return this.versionNumber;
			}
		}

		public string Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public bool NeedsExactVersionMatch
		{
			get
			{
				return this.needsExactVersionMatch;
			}
		}

		public string SiteName
		{
			get
			{
				return this.siteName;
			}
		}

		private readonly int versionNumber;

		private readonly string identifier;

		private readonly bool needsExactVersionMatch;

		private readonly string siteName;
	}
}
