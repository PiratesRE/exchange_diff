using System;

namespace Microsoft.Exchange.Data
{
	public interface IConfigurable
	{
		ObjectId Identity { get; }

		bool IsValid { get; }

		ObjectState ObjectState { get; }

		ValidationError[] Validate();

		void CopyChangesFrom(IConfigurable source);

		void ResetChangeTracking();
	}
}
