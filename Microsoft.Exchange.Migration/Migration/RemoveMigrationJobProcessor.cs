using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Migration
{
	internal class RemoveMigrationJobProcessor : MigrationJobProcessorBase
	{
		public RemoveMigrationJobProcessor(MigrationJob migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override Func<int?, IEnumerable<StoreObjectId>>[] ProcessableChildObjectQueries
		{
			get
			{
				return new Func<int?, IEnumerable<StoreObjectId>>[]
				{
					(int? maxCount) => MigrationJobItem.GetAllIds(this.DataProvider, this.MigrationObject, maxCount)
				};
			}
		}

		protected override MigrationProcessorResponse ProcessChild(MigrationJobItem child)
		{
			MigrationJobItemProcessorBase migrationJobItemProcessorBase = new RemoveMigrationJobItemProcessor(child, this.DataProvider);
			return migrationJobItemProcessorBase.Process();
		}

		protected override MigrationJobProcessorResponse ProcessObject()
		{
			return MigrationJobProcessorResponse.Create(MigrationProcessorResult.Deleted, null, null, null, null, null);
		}

		protected override MigrationJobProcessorResponse ApplyResponse(MigrationJobProcessorResponse response)
		{
			if (response.Result == MigrationProcessorResult.Deleted)
			{
				if (!this.GetChildObjectIds(this.ProcessableChildObjectQueries, new int?(1)).Any<StoreObjectId>())
				{
					this.MigrationObject.Delete(this.DataProvider, false);
					return response;
				}
				response.Result = MigrationProcessorResult.Working;
			}
			else if (response.Result == MigrationProcessorResult.Failed && response.Error == null)
			{
				response.Error = new FailureToRemoveTransientException();
			}
			return base.ApplyResponse(response);
		}
	}
}
