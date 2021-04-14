using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAttachment : IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		AttachmentType AttachmentType { get; }

		bool IsContactPhoto { get; }

		AttachmentId Id { get; }

		string ContentType { get; set; }

		string CalculatedContentType { get; }

		string DisplayName { get; }

		string FileExtension { get; }

		string FileName { get; set; }

		bool IsInline { get; set; }

		ExDateTime LastModifiedTime { get; }

		long Size { get; }

		void Save();
	}
}
