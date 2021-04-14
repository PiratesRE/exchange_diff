using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TSCrashDumpsOnlyAvailableOnLocalMachineException : LocalizedException
	{
		public TSCrashDumpsOnlyAvailableOnLocalMachineException() : base(Strings.TSCrashDumpsOnlyAvailableOnLocalMachine)
		{
		}

		public TSCrashDumpsOnlyAvailableOnLocalMachineException(Exception innerException) : base(Strings.TSCrashDumpsOnlyAvailableOnLocalMachine, innerException)
		{
		}

		protected TSCrashDumpsOnlyAvailableOnLocalMachineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
