using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TooManyFoldersException : IMAPException
	{
		public TooManyFoldersException(int maxNumber) : base(Strings.TooManyFoldersException(maxNumber))
		{
			this.maxNumber = maxNumber;
		}

		public TooManyFoldersException(int maxNumber, Exception innerException) : base(Strings.TooManyFoldersException(maxNumber), innerException)
		{
			this.maxNumber = maxNumber;
		}

		protected TooManyFoldersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.maxNumber = (int)info.GetValue("maxNumber", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("maxNumber", this.maxNumber);
		}

		public int MaxNumber
		{
			get
			{
				return this.maxNumber;
			}
		}

		private readonly int maxNumber;
	}
}
