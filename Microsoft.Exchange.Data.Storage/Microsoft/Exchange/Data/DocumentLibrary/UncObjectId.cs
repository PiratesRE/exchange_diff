using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UncObjectId : DocumentLibraryObjectId
	{
		internal UncObjectId(Uri path, UriFlags uriFlags) : base(uriFlags)
		{
			if (!path.IsUnc)
			{
				throw new ArgumentException("path");
			}
			this.path = path;
		}

		public override bool Equals(object obj)
		{
			UncObjectId uncObjectId = obj as UncObjectId;
			return uncObjectId != null && uncObjectId.path == this.path;
		}

		public override int GetHashCode()
		{
			return this.path.GetHashCode();
		}

		public override string ToString()
		{
			return new UriBuilder(this.path)
			{
				Query = UncObjectId.QueryPart + base.UriFlags
			}.ToString();
		}

		public Uri Path
		{
			get
			{
				return this.path;
			}
		}

		private readonly Uri path;

		internal static readonly string QueryPart = "UriFlags=";
	}
}
