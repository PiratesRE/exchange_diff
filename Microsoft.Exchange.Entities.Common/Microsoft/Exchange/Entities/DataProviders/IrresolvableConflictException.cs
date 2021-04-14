using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.DataProviders
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IrresolvableConflictException : StoragePermanentException
	{
		public IrresolvableConflictException(ConflictResolutionResult conflictResolutionResult) : base(Strings.IrresolvableConflict(conflictResolutionResult))
		{
			this.conflictResolutionResult = conflictResolutionResult;
		}

		public IrresolvableConflictException(ConflictResolutionResult conflictResolutionResult, Exception innerException) : base(Strings.IrresolvableConflict(conflictResolutionResult), innerException)
		{
			this.conflictResolutionResult = conflictResolutionResult;
		}

		protected IrresolvableConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.conflictResolutionResult = (ConflictResolutionResult)info.GetValue("conflictResolutionResult", typeof(ConflictResolutionResult));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("conflictResolutionResult", this.conflictResolutionResult);
		}

		public ConflictResolutionResult ConflictResolutionResult
		{
			get
			{
				return this.conflictResolutionResult;
			}
		}

		private readonly ConflictResolutionResult conflictResolutionResult;
	}
}
