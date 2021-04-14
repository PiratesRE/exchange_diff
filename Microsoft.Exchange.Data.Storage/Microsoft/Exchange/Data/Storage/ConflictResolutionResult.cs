using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConflictResolutionResult
	{
		public ConflictResolutionResult(SaveResult saveResult, PropertyConflict[] propertyConflicts)
		{
			EnumValidator.ThrowIfInvalid<SaveResult>(saveResult, "saveResult");
			this.SaveStatus = saveResult;
			this.PropertyConflicts = propertyConflicts;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.SaveStatus);
			if (this.PropertyConflicts != null)
			{
				stringBuilder.Append(" (");
				foreach (PropertyConflict value in this.PropertyConflicts)
				{
					stringBuilder.Append(value);
				}
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		public readonly SaveResult SaveStatus;

		public readonly PropertyConflict[] PropertyConflicts;

		internal static readonly ConflictResolutionResult Success = new ConflictResolutionResult(SaveResult.Success, null);

		internal static readonly ConflictResolutionResult Failure = new ConflictResolutionResult(SaveResult.IrresolvableConflict, null);

		internal static readonly ConflictResolutionResult SuccessWithoutSaving = new ConflictResolutionResult(SaveResult.SuccessWithoutSaving, null);
	}
}
