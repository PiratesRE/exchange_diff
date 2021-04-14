using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ReachMaxProcessedTimesException : LocalizedException
	{
		public ReachMaxProcessedTimesException(string argName) : base(Strings.ReachMaxProcessedTimes(argName))
		{
			this.argName = argName;
		}

		public ReachMaxProcessedTimesException(string argName, Exception innerException) : base(Strings.ReachMaxProcessedTimes(argName), innerException)
		{
			this.argName = argName;
		}

		protected ReachMaxProcessedTimesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.argName = (string)info.GetValue("argName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("argName", this.argName);
		}

		public string ArgName
		{
			get
			{
				return this.argName;
			}
		}

		private readonly string argName;
	}
}
