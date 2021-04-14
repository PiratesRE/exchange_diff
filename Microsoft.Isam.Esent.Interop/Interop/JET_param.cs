﻿using System;

namespace Microsoft.Isam.Esent.Interop
{
	public enum JET_param
	{
		SystemPath,
		TempPath,
		LogFilePath,
		BaseName,
		EventSource,
		MaxSessions,
		MaxOpenTables,
		MaxCursors = 8,
		MaxVerPages,
		MaxTemporaryTables,
		LogFileSize,
		LogBuffers,
		CircularLog = 17,
		DbExtensionSize,
		PageTempDBMin,
		CacheSizeMax = 23,
		CheckpointDepthMax,
		LrukCorrInterval,
		LrukTimeout = 28,
		OutstandingIOMax = 30,
		StartFlushThreshold,
		StopFlushThreshold,
		Recovery = 34,
		EnableOnlineDefrag,
		CacheSize = 41,
		EnableIndexChecking = 45,
		EventSourceKey = 49,
		NoInformationEvent,
		EventLoggingLevel,
		DeleteOutOfRangeLogs,
		EnableIndexCleanup = 54,
		CacheSizeMin = 60,
		PreferredVerPages = 63,
		DatabasePageSize,
		ErrorToString = 70,
		RuntimeCallback = 73,
		CleanupMismatchedLogFiles = 77,
		ExceptionAction = 98,
		CreatePathIfNotExist = 100,
		OneDatabasePerSession = 102,
		MaxInstances = 104,
		VersionStoreTaskQueueMax
	}
}