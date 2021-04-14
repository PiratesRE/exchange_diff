using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal sealed class ACRTraceString
	{
		public ACRTraceString(ConflictResolutionResult saveResults)
		{
			if (saveResults == null)
			{
				throw new ArgumentNullException("saveResults");
			}
			this.saveResults = saveResults;
		}

		public override string ToString()
		{
			if (this.saveResults == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder("Save results: ");
			stringBuilder.AppendLine(this.saveResults.SaveStatus.ToString());
			if (this.saveResults.PropertyConflicts != null && this.saveResults.PropertyConflicts.Length > 0)
			{
				for (int i = 0; i < this.saveResults.PropertyConflicts.Length; i++)
				{
					PropertyConflict propertyConflict = this.saveResults.PropertyConflicts[i];
					stringBuilder.AppendFormat("Resolvable: {0} Property: {1}\n", propertyConflict.ConflictResolvable, propertyConflict.PropertyDefinition);
					stringBuilder.AppendFormat("\tOriginal value: {0}\n", propertyConflict.OriginalValue);
					stringBuilder.AppendFormat("\tClient value: {0}\n", propertyConflict.ClientValue);
					stringBuilder.AppendFormat("\tServer value: {0}\n", propertyConflict.ServerValue);
					stringBuilder.AppendFormat("\tResolved value: {0}\n", propertyConflict.ResolvedValue);
				}
			}
			else
			{
				stringBuilder.Append("Zero properties in conflict");
			}
			return stringBuilder.ToString();
		}

		private ConflictResolutionResult saveResults;
	}
}
