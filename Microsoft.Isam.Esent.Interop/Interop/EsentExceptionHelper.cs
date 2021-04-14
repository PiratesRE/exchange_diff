using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal static class EsentExceptionHelper
	{
		public static EsentErrorException JetErrToException(JET_err err)
		{
			JET_err jet_err = err;
			if (jet_err <= JET_err.ColumnLong)
			{
				if (jet_err <= JET_err.CallbackFailed)
				{
					if (jet_err <= JET_err.InvalidLogDataSequence)
					{
						switch (jet_err)
						{
						case JET_err.FileCompressed:
							return new EsentFileCompressedException();
						case JET_err.FileIOFail:
							return new EsentFileIOFailException();
						case JET_err.FileIORetry:
							return new EsentFileIORetryException();
						case JET_err.FileIOAbort:
							return new EsentFileIOAbortException();
						case JET_err.FileIOBeyondEOF:
							return new EsentFileIOBeyondEOFException();
						case JET_err.FileIOSparse:
							return new EsentFileIOSparseException();
						default:
							switch (jet_err)
							{
							case JET_err.LSNotSet:
								return new EsentLSNotSetException();
							case JET_err.LSAlreadySet:
								return new EsentLSAlreadySetException();
							case JET_err.LSCallbackNotSpecified:
								return new EsentLSCallbackNotSpecifiedException();
							default:
								if (jet_err == JET_err.InvalidLogDataSequence)
								{
									return new EsentInvalidLogDataSequenceException();
								}
								break;
							}
							break;
						}
					}
					else
					{
						switch (jet_err)
						{
						case JET_err.TestInjectionNotSupported:
							return new EsentTestInjectionNotSupportedException();
						case JET_err.TooManyTestInjections:
							return new EsentTooManyTestInjectionsException();
						default:
							switch (jet_err)
							{
							case JET_err.OSSnapshotInvalidSnapId:
								return new EsentOSSnapshotInvalidSnapIdException();
							case JET_err.OSSnapshotNotAllowed:
								return new EsentOSSnapshotNotAllowedException();
							case JET_err.OSSnapshotTimeOut:
								return new EsentOSSnapshotTimeOutException();
							case JET_err.OSSnapshotInvalidSequence:
								return new EsentOSSnapshotInvalidSequenceException();
							default:
								switch (jet_err)
								{
								case JET_err.SpaceHintsInvalid:
									return new EsentSpaceHintsInvalidException();
								case JET_err.CallbackNotResolved:
									return new EsentCallbackNotResolvedException();
								case JET_err.CallbackFailed:
									return new EsentCallbackFailedException();
								}
								break;
							}
							break;
						}
					}
				}
				else if (jet_err <= JET_err.AfterInitialization)
				{
					if (jet_err == JET_err.DatabaseAlreadyRunningMaintenance)
					{
						return new EsentDatabaseAlreadyRunningMaintenanceException();
					}
					switch (jet_err)
					{
					case JET_err.RollbackError:
						return new EsentRollbackErrorException();
					case JET_err.OneDatabasePerSession:
						return new EsentOneDatabasePerSessionException();
					case JET_err.RecordFormatConversionFailed:
						return new EsentRecordFormatConversionFailedException();
					case JET_err.SessionInUse:
						return new EsentSessionInUseException();
					case JET_err.SessionContextNotSetByThisThread:
						return new EsentSessionContextNotSetByThisThreadException();
					case JET_err.SessionContextAlreadySet:
						return new EsentSessionContextAlreadySetException();
					case JET_err.EntryPointNotFound:
						return new EsentEntryPointNotFoundException();
					case JET_err.SessionSharingViolation:
						return new EsentSessionSharingViolationException();
					case JET_err.TooManySplits:
						return new EsentTooManySplitsException();
					case (JET_err)(-1908):
						break;
					case JET_err.AccessDenied:
						return new EsentAccessDeniedException();
					case JET_err.InvalidOperation:
						return new EsentInvalidOperationException();
					default:
						switch (jet_err)
						{
						case JET_err.LogCorrupted:
							return new EsentLogCorruptedException();
						case JET_err.AfterInitialization:
							return new EsentAfterInitializationException();
						}
						break;
					}
				}
				else if (jet_err <= JET_err.TooManySorts)
				{
					switch (jet_err)
					{
					case JET_err.FileInvalidType:
						return new EsentFileInvalidTypeException();
					case JET_err.FileNotFound:
						return new EsentFileNotFoundException();
					case (JET_err)(-1810):
					case (JET_err)(-1807):
					case (JET_err)(-1806):
					case (JET_err)(-1804):
						break;
					case JET_err.PermissionDenied:
						return new EsentPermissionDeniedException();
					case JET_err.DiskFull:
						return new EsentDiskFullException();
					case JET_err.TooManyAttachedDatabases:
						return new EsentTooManyAttachedDatabasesException();
					case JET_err.TempFileOpenError:
						return new EsentTempFileOpenErrorException();
					default:
						switch (jet_err)
						{
						case JET_err.InvalidOnSort:
							return new EsentInvalidOnSortException();
						case JET_err.TooManySorts:
							return new EsentTooManySortsException();
						}
						break;
					}
				}
				else
				{
					switch (jet_err)
					{
					case JET_err.UpdateMustVersion:
						return new EsentUpdateMustVersionException();
					case JET_err.DecompressionFailed:
						return new EsentDecompressionFailedException();
					case JET_err.LanguageNotSupported:
						return new EsentLanguageNotSupportedException();
					case (JET_err)(-1618):
					case (JET_err)(-1617):
					case (JET_err)(-1616):
					case (JET_err)(-1615):
					case (JET_err)(-1614):
					case (JET_err)(-1613):
					case (JET_err)(-1612):
					case (JET_err)(-1610):
					case (JET_err)(-1606):
						break;
					case JET_err.DataHasChanged:
						return new EsentDataHasChangedException();
					case JET_err.UpdateNotPrepared:
						return new EsentUpdateNotPreparedException();
					case JET_err.KeyNotMade:
						return new EsentKeyNotMadeException();
					case JET_err.AlreadyPrepared:
						return new EsentAlreadyPreparedException();
					case JET_err.KeyDuplicate:
						return new EsentKeyDuplicateException();
					case JET_err.RecordPrimaryChanged:
						return new EsentRecordPrimaryChangedException();
					case JET_err.NoCurrentRecord:
						return new EsentNoCurrentRecordException();
					case JET_err.RecordNoCopy:
						return new EsentRecordNoCopyException();
					case JET_err.RecordNotFound:
						return new EsentRecordNotFoundException();
					default:
						switch (jet_err)
						{
						case JET_err.ColumnCannotBeCompressed:
							return new EsentColumnCannotBeCompressedException();
						case JET_err.InvalidPlaceholderColumn:
							return new EsentInvalidPlaceholderColumnException();
						case JET_err.DerivedColumnCorruption:
							return new EsentDerivedColumnCorruptionException();
						case JET_err.MultiValuedDuplicateAfterTruncation:
							return new EsentMultiValuedDuplicateAfterTruncationException();
						case JET_err.LVCorrupted:
							return new EsentLVCorruptedException();
						case JET_err.MultiValuedDuplicate:
							return new EsentMultiValuedDuplicateException();
						case JET_err.DefaultValueTooBig:
							return new EsentDefaultValueTooBigException();
						case JET_err.CannotBeTagged:
							return new EsentCannotBeTaggedException();
						case JET_err.ColumnInRelationship:
							return new EsentColumnInRelationshipException();
						case JET_err.BadItagSequence:
							return new EsentBadItagSequenceException();
						case JET_err.BadColumnId:
							return new EsentBadColumnIdException();
						case JET_err.KeyIsMade:
							return new EsentKeyIsMadeException();
						case JET_err.NoCurrentIndex:
							return new EsentNoCurrentIndexException();
						case JET_err.TaggedNotNULL:
							return new EsentTaggedNotNULLException();
						case JET_err.InvalidColumnType:
							return new EsentInvalidColumnTypeException();
						case JET_err.ColumnRedundant:
							return new EsentColumnRedundantException();
						case JET_err.MultiValuedColumnMustBeTagged:
							return new EsentMultiValuedColumnMustBeTaggedException();
						case JET_err.ColumnDuplicate:
							return new EsentColumnDuplicateException();
						case JET_err.ColumnNotFound:
							return new EsentColumnNotFoundException();
						case JET_err.ColumnTooBig:
							return new EsentColumnTooBigException();
						case JET_err.ColumnIndexed:
							return new EsentColumnIndexedException();
						case JET_err.NullInvalid:
							return new EsentNullInvalidException();
						case JET_err.ColumnDoesNotFit:
							return new EsentColumnDoesNotFitException();
						case JET_err.ColumnNoChunk:
							return new EsentColumnNoChunkException();
						case JET_err.ColumnLong:
							return new EsentColumnLongException();
						}
						break;
					}
				}
			}
			else if (jet_err <= JET_err.InvalidLoggedOperation)
			{
				if (jet_err <= JET_err.TermInProgress)
				{
					switch (jet_err)
					{
					case JET_err.IndexTuplesKeyTooSmall:
						return new EsentIndexTuplesKeyTooSmallException();
					case JET_err.IndexTuplesCannotRetrieveFromIndex:
						return new EsentIndexTuplesCannotRetrieveFromIndexException();
					case JET_err.IndexTuplesInvalidLimits:
						return new EsentIndexTuplesInvalidLimitsException();
					case JET_err.IndexTuplesVarSegMacNotAllowed:
						return new EsentIndexTuplesVarSegMacNotAllowedException();
					case JET_err.IndexTuplesTextBinaryColumnsOnly:
						return new EsentIndexTuplesTextBinaryColumnsOnlyException();
					case JET_err.IndexTuplesNonUniqueOnly:
						return new EsentIndexTuplesNonUniqueOnlyException();
					case JET_err.IndexTuplesTooManyColumns:
						return new EsentIndexTuplesTooManyColumnsException();
					case JET_err.IndexTuplesSecondaryIndexOnly:
						return new EsentIndexTuplesSecondaryIndexOnlyException();
					case (JET_err)(-1429):
					case (JET_err)(-1428):
					case (JET_err)(-1427):
					case (JET_err)(-1426):
					case (JET_err)(-1425):
					case (JET_err)(-1424):
					case (JET_err)(-1423):
					case (JET_err)(-1422):
					case (JET_err)(-1421):
					case (JET_err)(-1420):
					case (JET_err)(-1419):
					case (JET_err)(-1418):
					case (JET_err)(-1417):
					case (JET_err)(-1415):
					case (JET_err)(-1408):
					case (JET_err)(-1407):
						break;
					case JET_err.InvalidIndexId:
						return new EsentInvalidIndexIdException();
					case JET_err.SecondaryIndexCorrupted:
						return new EsentSecondaryIndexCorruptedException();
					case JET_err.PrimaryIndexCorrupted:
						return new EsentPrimaryIndexCorruptedException();
					case JET_err.IndexBuildCorrupted:
						return new EsentIndexBuildCorruptedException();
					case JET_err.MultiValuedIndexViolation:
						return new EsentMultiValuedIndexViolationException();
					case JET_err.TooManyOpenIndexes:
						return new EsentTooManyOpenIndexesException();
					case JET_err.InvalidCreateIndex:
						return new EsentInvalidCreateIndexException();
					case JET_err.IndexInvalidDef:
						return new EsentIndexInvalidDefException();
					case JET_err.IndexMustStay:
						return new EsentIndexMustStayException();
					case JET_err.IndexNotFound:
						return new EsentIndexNotFoundException();
					case JET_err.IndexDuplicate:
						return new EsentIndexDuplicateException();
					case JET_err.IndexHasPrimary:
						return new EsentIndexHasPrimaryException();
					case JET_err.IndexCantBuild:
						return new EsentIndexCantBuildException();
					default:
						switch (jet_err)
						{
						case JET_err.CannotAddFixedVarColumnToDerivedTable:
							return new EsentCannotAddFixedVarColumnToDerivedTableException();
						case JET_err.ClientRequestToStopJetService:
							return new EsentClientRequestToStopJetServiceException();
						case JET_err.InvalidSettings:
							return new EsentInvalidSettingsException();
						case (JET_err)(-1327):
						case (JET_err)(-1321):
						case (JET_err)(-1320):
						case (JET_err)(-1315):
						case (JET_err)(-1309):
						case (JET_err)(-1306):
							break;
						case JET_err.DDLNotInheritable:
							return new EsentDDLNotInheritableException();
						case JET_err.CannotNestDDL:
							return new EsentCannotNestDDLException();
						case JET_err.FixedInheritedDDL:
							return new EsentFixedInheritedDDLException();
						case JET_err.FixedDDL:
							return new EsentFixedDDLException();
						case JET_err.ExclusiveTableLockRequired:
							return new EsentExclusiveTableLockRequiredException();
						case JET_err.CannotDeleteTemplateTable:
							return new EsentCannotDeleteTemplateTableException();
						case JET_err.CannotDeleteSystemTable:
							return new EsentCannotDeleteSystemTableException();
						case JET_err.CannotDeleteTempTable:
							return new EsentCannotDeleteTempTableException();
						case JET_err.InvalidObject:
							return new EsentInvalidObjectException();
						case JET_err.ObjectDuplicate:
							return new EsentObjectDuplicateException();
						case JET_err.TooManyOpenTablesAndCleanupTimedOut:
							return new EsentTooManyOpenTablesAndCleanupTimedOutException();
						case JET_err.IllegalOperation:
							return new EsentIllegalOperationException();
						case JET_err.TooManyOpenTables:
							return new EsentTooManyOpenTablesException();
						case JET_err.InvalidTableId:
							return new EsentInvalidTableIdException();
						case JET_err.TableNotEmpty:
							return new EsentTableNotEmptyException();
						case JET_err.DensityInvalid:
							return new EsentDensityInvalidException();
						case JET_err.ObjectNotFound:
							return new EsentObjectNotFoundException();
						case JET_err.TableInUse:
							return new EsentTableInUseException();
						case JET_err.TableDuplicate:
							return new EsentTableDuplicateException();
						case JET_err.TableLocked:
							return new EsentTableLockedException();
						default:
							switch (jet_err)
							{
							case JET_err.NoAttachmentsFailedIncrementalReseed:
								return new EsentNoAttachmentsFailedIncrementalReseedException();
							case JET_err.DatabaseFailedIncrementalReseed:
								return new EsentDatabaseFailedIncrementalReseedException();
							case JET_err.DatabaseInvalidIncrementalReseed:
								return new EsentDatabaseInvalidIncrementalReseedException();
							case JET_err.DatabaseIncompleteIncrementalReseed:
								return new EsentDatabaseIncompleteIncrementalReseedException();
							case JET_err.InvalidCreateDbVersion:
								return new EsentInvalidCreateDbVersionException();
							case JET_err.DatabaseCorruptedNoRepair:
								return new EsentDatabaseCorruptedNoRepairException();
							case JET_err.DatabaseSignInUse:
								return new EsentDatabaseSignInUseException();
							case JET_err.PartiallyAttachedDB:
								return new EsentPartiallyAttachedDBException();
							case JET_err.CatalogCorrupted:
								return new EsentCatalogCorruptedException();
							case JET_err.ForceDetachNotAllowed:
								return new EsentForceDetachNotAllowedException();
							case JET_err.DatabaseIdInUse:
								return new EsentDatabaseIdInUseException();
							case JET_err.DatabaseInvalidPath:
								return new EsentDatabaseInvalidPathException();
							case JET_err.AttachedDatabaseMismatch:
								return new EsentAttachedDatabaseMismatchException();
							case JET_err.DatabaseSharingViolation:
								return new EsentDatabaseSharingViolationException();
							case JET_err.TooManyInstances:
								return new EsentTooManyInstancesException();
							case JET_err.PageSizeMismatch:
								return new EsentPageSizeMismatchException();
							case JET_err.Database500Format:
								return new EsentDatabase500FormatException();
							case JET_err.Database400Format:
								return new EsentDatabase400FormatException();
							case JET_err.Database200Format:
								return new EsentDatabase200FormatException();
							case JET_err.InvalidDatabaseVersion:
								return new EsentInvalidDatabaseVersionException();
							case JET_err.CannotDisableVersioning:
								return new EsentCannotDisableVersioningException();
							case JET_err.DatabaseLocked:
								return new EsentDatabaseLockedException();
							case JET_err.DatabaseCorrupted:
								return new EsentDatabaseCorruptedException();
							case JET_err.DatabaseInvalidPages:
								return new EsentDatabaseInvalidPagesException();
							case JET_err.DatabaseInvalidName:
								return new EsentDatabaseInvalidNameException();
							case JET_err.DatabaseNotFound:
								return new EsentDatabaseNotFoundException();
							case JET_err.DatabaseInUse:
								return new EsentDatabaseInUseException();
							case JET_err.DatabaseDuplicate:
								return new EsentDatabaseDuplicateException();
							case JET_err.DTCCallbackUnexpectedError:
								return new EsentDTCCallbackUnexpectedErrorException();
							case JET_err.DTCMissingCallbackOnRecovery:
								return new EsentDTCMissingCallbackOnRecoveryException();
							case JET_err.DTCMissingCallback:
								return new EsentDTCMissingCallbackException();
							case JET_err.CannotNestDistributedTransactions:
								return new EsentCannotNestDistributedTransactionsException();
							case JET_err.DistributedTransactionNotYetPreparedToCommit:
								return new EsentDistributedTransactionNotYetPreparedToCommitException();
							case JET_err.NotInDistributedTransaction:
								return new EsentNotInDistributedTransactionException();
							case JET_err.DistributedTransactionAlreadyPreparedToCommit:
								return new EsentDistributedTransactionAlreadyPreparedToCommitException();
							case JET_err.MustCommitDistributedTransactionToLevel0:
								return new EsentMustCommitDistributedTransactionToLevel0Exception();
							case JET_err.FilteredMoveNotSupported:
								return new EsentFilteredMoveNotSupportedException();
							case JET_err.RecoveryVerifyFailure:
								return new EsentRecoveryVerifyFailureException();
							case JET_err.FileSystemCorruption:
								return new EsentFileSystemCorruptionException();
							case JET_err.ReadLostFlushVerifyFailure:
								return new EsentReadLostFlushVerifyFailureException();
							case JET_err.ReadPgnoVerifyFailure:
								return new EsentReadPgnoVerifyFailureException();
							case JET_err.DirtyShutdown:
								return new EsentDirtyShutdownException();
							case JET_err.InvalidInstance:
								return new EsentInvalidInstanceException();
							case JET_err.SesidTableIdMismatch:
								return new EsentSesidTableIdMismatchException();
							case JET_err.CannotMaterializeForwardOnlySort:
								return new EsentCannotMaterializeForwardOnlySortException();
							case JET_err.RecordTooBigForBackwardCompatibility:
								return new EsentRecordTooBigForBackwardCompatibilityException();
							case JET_err.SessionWriteConflict:
								return new EsentSessionWriteConflictException();
							case JET_err.TransReadOnly:
								return new EsentTransReadOnlyException();
							case JET_err.RollbackRequired:
								return new EsentRollbackRequiredException();
							case JET_err.InTransaction:
								return new EsentInTransactionException();
							case JET_err.WriteConflictPrimaryIndex:
								return new EsentWriteConflictPrimaryIndexException();
							case JET_err.InvalidSesid:
								return new EsentInvalidSesidException();
							case JET_err.TransTooDeep:
								return new EsentTransTooDeepException();
							case JET_err.WriteConflict:
								return new EsentWriteConflictException();
							case JET_err.OutOfSessions:
								return new EsentOutOfSessionsException();
							case JET_err.InstanceUnavailableDueToFatalLogDiskFull:
								return new EsentInstanceUnavailableDueToFatalLogDiskFullException();
							case JET_err.DatabaseUnavailable:
								return new EsentDatabaseUnavailableException();
							case JET_err.InstanceUnavailable:
								return new EsentInstanceUnavailableException();
							case JET_err.SystemParameterConflict:
								return new EsentSystemParameterConflictException();
							case JET_err.InstanceNameInUse:
								return new EsentInstanceNameInUseException();
							case JET_err.TempPathInUse:
								return new EsentTempPathInUseException();
							case JET_err.LogFilePathInUse:
								return new EsentLogFilePathInUseException();
							case JET_err.SystemPathInUse:
								return new EsentSystemPathInUseException();
							case JET_err.SystemParamsAlreadySet:
								return new EsentSystemParamsAlreadySetException();
							case JET_err.RunningInMultiInstanceMode:
								return new EsentRunningInMultiInstanceModeException();
							case JET_err.RunningInOneInstanceMode:
								return new EsentRunningInOneInstanceModeException();
							case JET_err.OutOfSequentialIndexValues:
								return new EsentOutOfSequentialIndexValuesException();
							case JET_err.OutOfDbtimeValues:
								return new EsentOutOfDbtimeValuesException();
							case JET_err.OutOfAutoincrementValues:
								return new EsentOutOfAutoincrementValuesException();
							case JET_err.OutOfLongValueIDs:
								return new EsentOutOfLongValueIDsException();
							case JET_err.OutOfObjectIDs:
								return new EsentOutOfObjectIDsException();
							case JET_err.TooManyMempoolEntries:
								return new EsentTooManyMempoolEntriesException();
							case JET_err.RecordNotDeleted:
								return new EsentRecordNotDeletedException();
							case JET_err.CannotIndex:
								return new EsentCannotIndexException();
							case JET_err.CurrencyStackOutOfMemory:
								return new EsentCurrencyStackOutOfMemoryException();
							case JET_err.VersionStoreOutOfMemory:
								return new EsentVersionStoreOutOfMemoryException();
							case JET_err.VersionStoreOutOfMemoryAndCleanupTimedOut:
								return new EsentVersionStoreOutOfMemoryAndCleanupTimedOutException();
							case JET_err.VersionStoreEntryTooBig:
								return new EsentVersionStoreEntryTooBigException();
							case JET_err.InvalidLCMapStringFlags:
								return new EsentInvalidLCMapStringFlagsException();
							case JET_err.InvalidCodePage:
								return new EsentInvalidCodePageException();
							case JET_err.InvalidLanguageId:
								return new EsentInvalidLanguageIdException();
							case JET_err.InvalidCountry:
								return new EsentInvalidCountryException();
							case JET_err.TooManyActiveUsers:
								return new EsentTooManyActiveUsersException();
							case JET_err.MustRollback:
								return new EsentMustRollbackException();
							case JET_err.NotInTransaction:
								return new EsentNotInTransactionException();
							case JET_err.NullKeyDisallowed:
								return new EsentNullKeyDisallowedException();
							case JET_err.LinkNotSupported:
								return new EsentLinkNotSupportedException();
							case JET_err.IndexInUse:
								return new EsentIndexInUseException();
							case JET_err.ColumnNotUpdatable:
								return new EsentColumnNotUpdatableException();
							case JET_err.InvalidBufferSize:
								return new EsentInvalidBufferSizeException();
							case JET_err.ColumnInUse:
								return new EsentColumnInUseException();
							case JET_err.InvalidBookmark:
								return new EsentInvalidBookmarkException();
							case JET_err.InvalidFilename:
								return new EsentInvalidFilenameException();
							case JET_err.ContainerNotEmpty:
								return new EsentContainerNotEmptyException();
							case JET_err.TooManyColumns:
								return new EsentTooManyColumnsException();
							case JET_err.BufferTooSmall:
								return new EsentBufferTooSmallException();
							case JET_err.SQLLinkNotSupported:
								return new EsentSQLLinkNotSupportedException();
							case JET_err.QueryNotSupported:
								return new EsentQueryNotSupportedException();
							case JET_err.FileAccessDenied:
								return new EsentFileAccessDeniedException();
							case JET_err.InitInProgress:
								return new EsentInitInProgressException();
							case JET_err.AlreadyInitialized:
								return new EsentAlreadyInitializedException();
							case JET_err.NotInitialized:
								return new EsentNotInitializedException();
							case JET_err.InvalidDatabase:
								return new EsentInvalidDatabaseException();
							case JET_err.TooManyOpenDatabases:
								return new EsentTooManyOpenDatabasesException();
							case JET_err.RecordTooBig:
								return new EsentRecordTooBigException();
							case JET_err.InvalidLogDirectory:
								return new EsentInvalidLogDirectoryException();
							case JET_err.InvalidSystemPath:
								return new EsentInvalidSystemPathException();
							case JET_err.InvalidPath:
								return new EsentInvalidPathException();
							case JET_err.DiskIO:
								return new EsentDiskIOException();
							case JET_err.DiskReadVerificationFailure:
								return new EsentDiskReadVerificationFailureException();
							case JET_err.OutOfFileHandles:
								return new EsentOutOfFileHandlesException();
							case JET_err.PageNotInitialized:
								return new EsentPageNotInitializedException();
							case JET_err.ReadVerifyFailure:
								return new EsentReadVerifyFailureException();
							case JET_err.RecordDeleted:
								return new EsentRecordDeletedException();
							case JET_err.TooManyKeys:
								return new EsentTooManyKeysException();
							case JET_err.TooManyIndexes:
								return new EsentTooManyIndexesException();
							case JET_err.OutOfBuffers:
								return new EsentOutOfBuffersException();
							case JET_err.OutOfCursors:
								return new EsentOutOfCursorsException();
							case JET_err.OutOfDatabaseSpace:
								return new EsentOutOfDatabaseSpaceException();
							case JET_err.OutOfMemory:
								return new EsentOutOfMemoryException();
							case JET_err.InvalidDatabaseId:
								return new EsentInvalidDatabaseIdException();
							case JET_err.DatabaseFileReadOnly:
								return new EsentDatabaseFileReadOnlyException();
							case JET_err.InvalidParameter:
								return new EsentInvalidParameterException();
							case JET_err.InvalidName:
								return new EsentInvalidNameException();
							case JET_err.FeatureNotAvailable:
								return new EsentFeatureNotAvailableException();
							case JET_err.TermInProgress:
								return new EsentTermInProgressException();
							}
							break;
						}
						break;
					}
				}
				else
				{
					if (jet_err == JET_err.InvalidGrbit)
					{
						return new EsentInvalidGrbitException();
					}
					if (jet_err == JET_err.BackupAbortByServer)
					{
						return new EsentBackupAbortByServerException();
					}
					switch (jet_err)
					{
					case JET_err.TransactionTooLong:
						return new EsentTransactionTooLongException();
					case JET_err.SurrogateBackupInProgress:
						return new EsentSurrogateBackupInProgressException();
					case JET_err.LogFileNotCopied:
						return new EsentLogFileNotCopiedException();
					case JET_err.RestoreOfNonBackupDatabase:
						return new EsentRestoreOfNonBackupDatabaseException();
					case JET_err.CheckpointDepthTooDeep:
						return new EsentCheckpointDepthTooDeepException();
					case JET_err.LogReadVerifyFailure:
						return new EsentLogReadVerifyFailureException();
					case JET_err.ExistingLogFileIsNotContiguous:
						return new EsentExistingLogFileIsNotContiguousException();
					case JET_err.ExistingLogFileHasBadSignature:
						return new EsentExistingLogFileHasBadSignatureException();
					case JET_err.UnicodeLanguageValidationFailure:
						return new EsentUnicodeLanguageValidationFailureException();
					case JET_err.UnicodeNormalizationNotSupported:
						return new EsentUnicodeNormalizationNotSupportedException();
					case JET_err.UnicodeTranslationFail:
						return new EsentUnicodeTranslationFailException();
					case JET_err.UnicodeTranslationBufferTooSmall:
						return new EsentUnicodeTranslationBufferTooSmallException();
					case JET_err.CommittedLogFileCorrupt:
						return new EsentCommittedLogFileCorruptException();
					case JET_err.RecoveredWithoutUndoDatabasesConsistent:
						return new EsentRecoveredWithoutUndoDatabasesConsistentException();
					case JET_err.SectorSizeNotSupported:
						return new EsentSectorSizeNotSupportedException();
					case JET_err.CommittedLogFilesMissing:
						return new EsentCommittedLogFilesMissingException();
					case JET_err.SoftRecoveryOnSnapshot:
						return new EsentSoftRecoveryOnSnapshotException();
					case JET_err.DatabasesNotFromSameSnapshot:
						return new EsentDatabasesNotFromSameSnapshotException();
					case JET_err.RecoveredWithoutUndo:
						return new EsentRecoveredWithoutUndoException();
					case JET_err.BadRestoreTargetInstance:
						return new EsentBadRestoreTargetInstanceException();
					case JET_err.MustDisableLoggingForDbUpgrade:
						return new EsentMustDisableLoggingForDbUpgradeException();
					case JET_err.LogCorruptDuringHardRecovery:
						return new EsentLogCorruptDuringHardRecoveryException();
					case JET_err.LogCorruptDuringHardRestore:
						return new EsentLogCorruptDuringHardRestoreException();
					case JET_err.LogTornWriteDuringHardRecovery:
						return new EsentLogTornWriteDuringHardRecoveryException();
					case JET_err.LogTornWriteDuringHardRestore:
						return new EsentLogTornWriteDuringHardRestoreException();
					case JET_err.MissingFileToBackup:
						return new EsentMissingFileToBackupException();
					case JET_err.DbTimeTooNew:
						return new EsentDbTimeTooNewException();
					case JET_err.DbTimeTooOld:
						return new EsentDbTimeTooOldException();
					case JET_err.MissingCurrentLogFiles:
						return new EsentMissingCurrentLogFilesException();
					case JET_err.DatabaseIncompleteUpgrade:
						return new EsentDatabaseIncompleteUpgradeException();
					case JET_err.DatabaseAlreadyUpgraded:
						return new EsentDatabaseAlreadyUpgradedException();
					case JET_err.BadBackupDatabaseSize:
						return new EsentBadBackupDatabaseSizeException();
					case JET_err.MissingFullBackup:
						return new EsentMissingFullBackupException();
					case JET_err.MissingRestoreLogFiles:
						return new EsentMissingRestoreLogFilesException();
					case JET_err.GivenLogFileIsNotContiguous:
						return new EsentGivenLogFileIsNotContiguousException();
					case JET_err.GivenLogFileHasBadSignature:
						return new EsentGivenLogFileHasBadSignatureException();
					case JET_err.StartingRestoreLogTooHigh:
						return new EsentStartingRestoreLogTooHighException();
					case JET_err.EndingRestoreLogTooLow:
						return new EsentEndingRestoreLogTooLowException();
					case JET_err.DatabasePatchFileMismatch:
						return new EsentDatabasePatchFileMismatchException();
					case JET_err.ConsistentTimeMismatch:
						return new EsentConsistentTimeMismatchException();
					case JET_err.DatabaseDirtyShutdown:
						return new EsentDatabaseDirtyShutdownException();
					case JET_err.StreamingDataNotLogged:
						return new EsentStreamingDataNotLoggedException();
					case JET_err.LogSequenceEndDatabasesConsistent:
						return new EsentLogSequenceEndDatabasesConsistentException();
					case JET_err.LogSectorSizeMismatchDatabasesConsistent:
						return new EsentLogSectorSizeMismatchDatabasesConsistentException();
					case JET_err.LogSectorSizeMismatch:
						return new EsentLogSectorSizeMismatchException();
					case JET_err.LogFileSizeMismatchDatabasesConsistent:
						return new EsentLogFileSizeMismatchDatabasesConsistentException();
					case JET_err.SoftRecoveryOnBackupDatabase:
						return new EsentSoftRecoveryOnBackupDatabaseException();
					case JET_err.RequiredLogFilesMissing:
						return new EsentRequiredLogFilesMissingException();
					case JET_err.CheckpointFileNotFound:
						return new EsentCheckpointFileNotFoundException();
					case JET_err.LogFileSizeMismatch:
						return new EsentLogFileSizeMismatchException();
					case JET_err.DatabaseStreamingFileMismatch:
						return new EsentDatabaseStreamingFileMismatchException();
					case JET_err.DatabaseLogSetMismatch:
						return new EsentDatabaseLogSetMismatchException();
					case JET_err.PatchFileMissing:
						return new EsentPatchFileMissingException();
					case JET_err.RedoAbruptEnded:
						return new EsentRedoAbruptEndedException();
					case JET_err.BadPatchPage:
						return new EsentBadPatchPageException();
					case JET_err.MissingPatchPage:
						return new EsentMissingPatchPageException();
					case JET_err.CheckpointCorrupt:
						return new EsentCheckpointCorruptException();
					case JET_err.BadCheckpointSignature:
						return new EsentBadCheckpointSignatureException();
					case JET_err.BadDbSignature:
						return new EsentBadDbSignatureException();
					case JET_err.BadLogSignature:
						return new EsentBadLogSignatureException();
					case JET_err.LogDiskFull:
						return new EsentLogDiskFullException();
					case JET_err.MissingLogFile:
						return new EsentMissingLogFileException();
					case JET_err.RecoveredWithErrors:
						return new EsentRecoveredWithErrorsException();
					case JET_err.InvalidBackup:
						return new EsentInvalidBackupException();
					case JET_err.MakeBackupDirectoryFail:
						return new EsentMakeBackupDirectoryFailException();
					case JET_err.DeleteBackupFileFail:
						return new EsentDeleteBackupFileFailException();
					case JET_err.BackupNotAllowedYet:
						return new EsentBackupNotAllowedYetException();
					case JET_err.InvalidBackupSequence:
						return new EsentInvalidBackupSequenceException();
					case JET_err.NoBackup:
						return new EsentNoBackupException();
					case JET_err.LogSequenceEnd:
						return new EsentLogSequenceEndException();
					case JET_err.LogBufferTooSmall:
						return new EsentLogBufferTooSmallException();
					case JET_err.LoggingDisabled:
						return new EsentLoggingDisabledException();
					case JET_err.InvalidLogSequence:
						return new EsentInvalidLogSequenceException();
					case JET_err.BadLogVersion:
						return new EsentBadLogVersionException();
					case JET_err.LogGenerationMismatch:
						return new EsentLogGenerationMismatchException();
					case JET_err.CannotLogDuringRecoveryRedo:
						return new EsentCannotLogDuringRecoveryRedoException();
					case JET_err.LogDisabledDueToRecoveryFailure:
						return new EsentLogDisabledDueToRecoveryFailureException();
					case JET_err.LogWriteFail:
						return new EsentLogWriteFailException();
					case JET_err.MissingPreviousLogFile:
						return new EsentMissingPreviousLogFileException();
					case JET_err.RestoreInProgress:
						return new EsentRestoreInProgressException();
					case JET_err.BackupInProgress:
						return new EsentBackupInProgressException();
					case JET_err.BackupDirectoryNotEmpty:
						return new EsentBackupDirectoryNotEmptyException();
					case JET_err.NoBackupDirectory:
						return new EsentNoBackupDirectoryException();
					case JET_err.LogFileCorrupt:
						return new EsentLogFileCorruptException();
					case JET_err.InvalidLoggedOperation:
						return new EsentInvalidLoggedOperationException();
					}
				}
			}
			else if (jet_err <= JET_err.KeyTooBig)
			{
				switch (jet_err)
				{
				case JET_err.InvalidPreread:
					return new EsentInvalidPrereadException();
				case JET_err.MustBeSeparateLongValue:
					return new EsentMustBeSeparateLongValueException();
				case (JET_err)(-422):
					break;
				case JET_err.SeparatedLongValue:
					return new EsentSeparatedLongValueException();
				default:
					if (jet_err == JET_err.CannotSeparateIntrinsicLV)
					{
						return new EsentCannotSeparateIntrinsicLVException();
					}
					if (jet_err == JET_err.KeyTooBig)
					{
						return new EsentKeyTooBigException();
					}
					break;
				}
			}
			else if (jet_err <= JET_err.PreviousVersion)
			{
				switch (jet_err)
				{
				case JET_err.BadEmptyPage:
					return new EsentBadEmptyPageException();
				case (JET_err)(-350):
				case (JET_err)(-349):
				case (JET_err)(-347):
				case (JET_err)(-345):
				case (JET_err)(-339):
				case (JET_err)(-337):
				case (JET_err)(-336):
				case (JET_err)(-335):
					break;
				case JET_err.DatabaseLeakInSpace:
					return new EsentDatabaseLeakInSpaceException();
				case JET_err.KeyTruncated:
					return new EsentKeyTruncatedException();
				case JET_err.DbTimeCorrupted:
					return new EsentDbTimeCorruptedException();
				case JET_err.SPOwnExtCorrupted:
					return new EsentSPOwnExtCorruptedException();
				case JET_err.SPAvailExtCacheOutOfMemory:
					return new EsentSPAvailExtCacheOutOfMemoryException();
				case JET_err.SPAvailExtCorrupted:
					return new EsentSPAvailExtCorruptedException();
				case JET_err.SPAvailExtCacheOutOfSync:
					return new EsentSPAvailExtCacheOutOfSyncException();
				case JET_err.BadParentPageLink:
					return new EsentBadParentPageLinkException();
				case JET_err.NTSystemCallFailed:
					return new EsentNTSystemCallFailedException();
				default:
					switch (jet_err)
					{
					case JET_err.BadBookmark:
						return new EsentBadBookmarkException();
					case JET_err.BadPageLink:
						return new EsentBadPageLinkException();
					case JET_err.KeyBoundary:
						return new EsentKeyBoundaryException();
					case JET_err.PageBoundary:
						return new EsentPageBoundaryException();
					case JET_err.PreviousVersion:
						return new EsentPreviousVersionException();
					}
					break;
				}
			}
			else
			{
				if (jet_err == JET_err.DatabaseBufferDependenciesCorrupted)
				{
					return new EsentDatabaseBufferDependenciesCorruptedException();
				}
				switch (jet_err)
				{
				case JET_err.UnloadableOSFunctionality:
					return new EsentUnloadableOSFunctionalityException();
				case JET_err.DisabledFunctionality:
					return new EsentDisabledFunctionalityException();
				case JET_err.InternalError:
					return new EsentInternalErrorException();
				case JET_err.TaskDropped:
					return new EsentTaskDroppedException();
				case JET_err.TooManyIO:
					return new EsentTooManyIOException();
				case JET_err.OutOfThreads:
					return new EsentOutOfThreadsException();
				case JET_err.FileClose:
					return new EsentFileCloseException();
				case JET_err.RfsNotArmed:
					return new EsentRfsNotArmedException();
				case JET_err.RfsFailure:
					return new EsentRfsFailureException();
				}
			}
			IntPtr intPtr = new IntPtr((int)err);
			string message;
			int num = Api.Impl.JetGetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, JET_param.ErrorToString, ref intPtr, out message, 1024);
			err = (JET_err)intPtr.ToInt32();
			if (num != 0)
			{
				message = "Unknown error";
			}
			return new EsentErrorException(message, err);
		}
	}
}
