using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationDataCorruptionException : LocalizedException
	{
		public MigrationDataCorruptionException(string internalDetails) : this(internalDetails, null)
		{
		}

		public MigrationDataCorruptionException(string internalDetails, Exception innerException)
		{
			this.createdStack = new StackTrace();
			base..ctor(Strings.MigrationDataCorruptionError, innerException);
			this.InternalError = internalDetails;
		}

		protected MigrationDataCorruptionException(SerializationInfo info, StreamingContext context)
		{
			this.createdStack = new StackTrace();
			base..ctor(info, context);
		}

		public string InternalError
		{
			get
			{
				return (string)this.Data["InternalError"];
			}
			internal set
			{
				this.Data["InternalError"] = value;
			}
		}

		public override string StackTrace
		{
			get
			{
				if (!string.IsNullOrEmpty(base.StackTrace))
				{
					return base.StackTrace;
				}
				return this.createdStack.ToString();
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InternalError", this.InternalError);
		}

		private readonly StackTrace createdStack;
	}
}
