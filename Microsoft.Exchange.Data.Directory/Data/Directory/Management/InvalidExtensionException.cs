using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidExtensionException : LocalizedException
	{
		public InvalidExtensionException(string s, int i) : base(DirectoryStrings.ExtensionIsInvalid(s, i))
		{
			this.s = s;
			this.i = i;
		}

		public InvalidExtensionException(string s, int i, Exception innerException) : base(DirectoryStrings.ExtensionIsInvalid(s, i), innerException)
		{
			this.s = s;
			this.i = i;
		}

		protected InvalidExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.s = (string)info.GetValue("s", typeof(string));
			this.i = (int)info.GetValue("i", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("s", this.s);
			info.AddValue("i", this.i);
		}

		public string S
		{
			get
			{
				return this.s;
			}
		}

		public int I
		{
			get
			{
				return this.i;
			}
		}

		private readonly string s;

		private readonly int i;
	}
}
