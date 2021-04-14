using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum PermissionLevel
	{
		None,
		Owner,
		PublishingEditor,
		Editor,
		PublishingAuthor,
		Author,
		NonEditingAuthor,
		Reviewer,
		Contributor,
		Custom
	}
}
