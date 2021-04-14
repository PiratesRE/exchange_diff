﻿using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[Flags]
	internal enum Breadcrumb
	{
		None = 0,
		NewItem = 16777216,
		CloneItem = 33554432,
		CloseItem = 50331648,
		BeginTransaction = 67108864,
		CommitTransaction = 83886080,
		MarkDeleted = 100663296,
		Loaded = 117440512,
		Deleted = 134217728,
		Seek = 150994944,
		SeekFail = 167772160,
		InternalCommitTransaction = 184549376,
		MaterializeToRow = 201326592,
		MaterializeNew = 218103808,
		MaterializeUpdate = 234881024,
		AcknowledgeA = 251658240,
		AcknowledgeB = 268435456,
		AsyncExecute = 285212672,
		CommitNow = 301989888,
		CommitForReceive = 318767104,
		CommitLazy = 335544320,
		AppendingToExisting = 352321536,
		Push = 369098752,
		Pending = 385875968,
		TimedOut = 402653184,
		Moved = 419430400,
		Background = 436207616,
		Execute = 452984832,
		Done = 469762048,
		Shutdown = 486539264,
		Signaled = 503316480,
		DehydrateOnAddToShadowQueue = 520093696,
		DehydrateOnLimitedMailItemMemoryPressure = 536870912,
		DehydrateOnBackPressure = 553648128,
		ScopedRecipients = 570425344,
		UnscopedRecipients = 587202560,
		DehydrateOnRoutedMailItemDeferral = 603979776,
		DehydrateOnRoutingDone = 620756992,
		DehydrationSkippedItemInDelivery = 637534208,
		DehydrateOnCategorizerDeferral = 654311424,
		DehydrateOnMailItemUpdate = 671088640,
		DehydrateOnReleaseFromDumpster = 687865856,
		DehydrateOnReleaseFromShadowRedundancy = 704643072,
		DehydrateOnReleaseFromRemoteDelivery = 721420288,
		DehydrateMinimizedMemory = 738197504,
		DehydrateOnCategorizerMemoryPressure = 754974720,
		Dehydrated = 771751936,
		DehydrationSkippedItemLock = 788529152,
		MailItemDelivered = 973078528,
		MailItemDeleted = 989855744,
		MailItemPoison = 1006632960,
		DehydrateOnMailItemLocked = 1023410176,
		OpenMimeReadStream = 1040187392,
		OpenMimeWriteStream = 1056964608,
		PromoteHeaders = 1073741824,
		RestoreLastSavedMime = 1090519040,
		SetMimeDocument = 1107296256,
		SetReadOnly = 1124073472,
		MinimizeMemory = 1140850688,
		LoadFromParentRow = 1157627904,
		SaveToParentRow = 1174405120,
		Cleanup = 1191182336,
		NewSideEffectItem = 1207959552
	}
}