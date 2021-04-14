using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal interface IContextPlugin
	{
		Guid? LocalId { get; set; }

		bool IsContextPresent { get; }

		void SetId();

		bool CheckId();

		void Clear();
	}
}
