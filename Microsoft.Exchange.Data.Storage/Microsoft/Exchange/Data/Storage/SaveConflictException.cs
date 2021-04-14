using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SaveConflictException : StorageTransientException
	{
		public SaveConflictException(LocalizedString message, ConflictResolutionResult conflictResolutionResult) : base(message)
		{
			this.conflictResolutionResult = conflictResolutionResult;
		}

		public SaveConflictException(LocalizedString message, Exception inner) : base(message, inner)
		{
			this.conflictResolutionResult = SaveConflictException.mapiIrresolvableConflict;
		}

		public SaveConflictException(LocalizedString message) : base(message)
		{
			this.conflictResolutionResult = SaveConflictException.mapiIrresolvableConflict;
		}

		public ConflictResolutionResult ConflictResolutionResult
		{
			get
			{
				return this.conflictResolutionResult;
			}
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(base.Message);
				if (this.conflictResolutionResult != null && this.conflictResolutionResult.PropertyConflicts != null)
				{
					stringBuilder.AppendLine("SaveStatus: " + this.conflictResolutionResult.SaveStatus.ToString());
					stringBuilder.AppendLine("PropertyConflicts:");
					foreach (PropertyConflict propertyConflict in this.conflictResolutionResult.PropertyConflicts)
					{
						stringBuilder.AppendLine(propertyConflict.ToString());
					}
				}
				return stringBuilder.ToString();
			}
		}

		private static readonly ConflictResolutionResult mapiIrresolvableConflict = new ConflictResolutionResult(SaveResult.IrresolvableConflict, Array<PropertyConflict>.Empty);

		private readonly ConflictResolutionResult conflictResolutionResult;
	}
}
