using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IisTasksValidationInvalidVirtualDirectoryCharException : LocalizedException
	{
		public IisTasksValidationInvalidVirtualDirectoryCharException(string virtualDirectory, char badChar, int charIndex, char[] invalidChars) : base(Strings.IisTasksValidationInvalidVirtualDirectoryCharException(virtualDirectory, badChar, charIndex, invalidChars))
		{
			this.virtualDirectory = virtualDirectory;
			this.badChar = badChar;
			this.charIndex = charIndex;
			this.invalidChars = invalidChars;
		}

		public IisTasksValidationInvalidVirtualDirectoryCharException(string virtualDirectory, char badChar, int charIndex, char[] invalidChars, Exception innerException) : base(Strings.IisTasksValidationInvalidVirtualDirectoryCharException(virtualDirectory, badChar, charIndex, invalidChars), innerException)
		{
			this.virtualDirectory = virtualDirectory;
			this.badChar = badChar;
			this.charIndex = charIndex;
			this.invalidChars = invalidChars;
		}

		protected IisTasksValidationInvalidVirtualDirectoryCharException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.virtualDirectory = (string)info.GetValue("virtualDirectory", typeof(string));
			this.badChar = (char)info.GetValue("badChar", typeof(char));
			this.charIndex = (int)info.GetValue("charIndex", typeof(int));
			this.invalidChars = (char[])info.GetValue("invalidChars", typeof(char[]));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("virtualDirectory", this.virtualDirectory);
			info.AddValue("badChar", this.badChar);
			info.AddValue("charIndex", this.charIndex);
			info.AddValue("invalidChars", this.invalidChars);
		}

		public string VirtualDirectory
		{
			get
			{
				return this.virtualDirectory;
			}
		}

		public char BadChar
		{
			get
			{
				return this.badChar;
			}
		}

		public int CharIndex
		{
			get
			{
				return this.charIndex;
			}
		}

		public char[] InvalidChars
		{
			get
			{
				return this.invalidChars;
			}
		}

		private readonly string virtualDirectory;

		private readonly char badChar;

		private readonly int charIndex;

		private readonly char[] invalidChars;
	}
}
