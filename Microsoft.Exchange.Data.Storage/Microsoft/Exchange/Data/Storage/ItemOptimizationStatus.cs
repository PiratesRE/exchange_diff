using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ItemOptimizationStatus
	{
		None = 0,
		LeafNode = 1,
		Extracted = 2,
		Opened = 4,
		SummaryConstructed = 8,
		BodyTagNotPresent = 16,
		BodyTagMismatched = 32,
		BodyFormatMismatched = 64,
		NonMsHeader = 128,
		ExtraPropertiesNeeded = 256,
		ParticipantNotFound = 512,
		AttachmentPresnet = 1024,
		PossibleInlines = 2048,
		IrmProtected = 4096,
		MapiAttachmentPresent = 8192
	}
}
