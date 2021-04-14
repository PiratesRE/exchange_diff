using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MapiExceptionHelper
	{
		private static Exception CreateLowLevel(string message, int hr, int ec, DiagnosticContext diagCtx, Exception innerException)
		{
			if (ec <= -582)
			{
				if (ec <= -1216)
				{
					if (ec > -1808)
					{
						if (ec <= -1506)
						{
							if (ec <= -1524)
							{
								switch (ec)
								{
								case -1605:
									return new MapiExceptionJetErrorKeyDuplicate(message, hr, ec, diagCtx, innerException);
								case -1604:
								case -1602:
									goto IL_124A;
								case -1603:
									return new MapiExceptionJetErrorNoCurrentRecord(message, hr, ec, diagCtx, innerException);
								case -1601:
									break;
								default:
									switch (ec)
									{
									case -1526:
										return new MapiExceptionJetErrorLVCorrupted(message, hr, ec, diagCtx, innerException);
									case -1525:
										goto IL_124A;
									case -1524:
										return new MapiExceptionJetErrorDefaultValueTooBig(message, hr, ec, diagCtx, innerException);
									default:
										goto IL_124A;
									}
									break;
								}
							}
							else
							{
								if (ec == -1517)
								{
									return new MapiExceptionJetErrorBadColumnId(message, hr, ec, diagCtx, innerException);
								}
								switch (ec)
								{
								case -1507:
									break;
								case -1506:
									return new MapiExceptionJetErrorColumnTooBig(message, hr, ec, diagCtx, innerException);
								default:
									goto IL_124A;
								}
							}
						}
						else if (ec <= -1311)
						{
							if (ec == -1404)
							{
								return new MapiExceptionJetErrorIndexNotFound(message, hr, ec, diagCtx, innerException);
							}
							switch (ec)
							{
							case -1313:
								return new MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut(message, hr, ec, diagCtx, innerException);
							case -1312:
								goto IL_124A;
							case -1311:
								return new MapiExceptionJetErrorTooManyOpenTables(message, hr, ec, diagCtx, innerException);
							default:
								goto IL_124A;
							}
						}
						else
						{
							switch (ec)
							{
							case -1305:
								break;
							case -1304:
								return new MapiExceptionJetErrorTableInUse(message, hr, ec, diagCtx, innerException);
							case -1303:
								return new MapiExceptionJetErrorTableDuplicate(message, hr, ec, diagCtx, innerException);
							case -1302:
								return new MapiExceptionJetErrorTableLocked(message, hr, ec, diagCtx, innerException);
							default:
								switch (ec)
								{
								case -1217:
									return new MapiExceptionJetErrorDatabaseInvalidPath(message, hr, ec, diagCtx, innerException);
								case -1216:
									return new MapiExceptionJetErrorAttachedDatabaseMismatch(message, hr, ec, diagCtx, innerException);
								default:
									goto IL_124A;
								}
								break;
							}
						}
						return new MapiExceptionNotFound(message, hr, ec, diagCtx, innerException);
					}
					if (ec <= -2147220733)
					{
						if (ec == -2147221246)
						{
							return new MapiExceptionNoSupport(message, hr, ec, diagCtx, innerException);
						}
						if (ec == -2147221221)
						{
							return new MapiExceptionCorruptData(message, hr, ec, diagCtx, innerException);
						}
						switch (ec)
						{
						case -2147220735:
							return new MapiExceptionBadValue(message, hr, ec, diagCtx, innerException);
						case -2147220734:
							return new MapiExceptionInvalidType(message, hr, ec, diagCtx, innerException);
						case -2147220733:
							return new MapiExceptionTypeNoSupport(message, hr, ec, diagCtx, innerException);
						default:
							goto IL_124A;
						}
					}
					else if (ec <= -4001)
					{
						switch (ec)
						{
						case -2147219968:
							return new MapiExceptionCorruptStore(message, hr, ec, diagCtx, innerException);
						case -2147219967:
							return new MapiExceptionNotInQueue(message, hr, ec, diagCtx, innerException);
						case -2147219966:
						case -2147219965:
						case -2147219962:
						case -2147219961:
						case -2147219960:
							goto IL_124A;
						case -2147219964:
							return new MapiExceptionCollision(message, hr, ec, diagCtx, innerException);
						case -2147219963:
							return new MapiExceptionNotInitialized(message, hr, ec, diagCtx, innerException);
						case -2147219959:
							return new MapiExceptionHasFolders(message, hr, ec, diagCtx, innerException);
						case -2147219958:
							return new MapiExceptionHasMessages(message, hr, ec, diagCtx, innerException);
						case -2147219957:
							return new MapiExceptionFolderCycle(message, hr, ec, diagCtx, innerException);
						default:
							if (ec != -4001)
							{
								goto IL_124A;
							}
							return new MapiExceptionJetErrorFileIOBeyondEOF(message, hr, ec, diagCtx, innerException);
						}
					}
					else
					{
						if (ec == -1811)
						{
							return new MapiExceptionJetErrorFileNotFound(message, hr, ec, diagCtx, innerException);
						}
						if (ec != -1808)
						{
							goto IL_124A;
						}
						return new MapiExceptionJetErrorDiskFull(message, hr, ec, diagCtx, innerException);
					}
				}
				else if (ec <= -1066)
				{
					if (ec <= -1101)
					{
						if (ec == -1206)
						{
							goto IL_10EE;
						}
						if (ec == -1203)
						{
							return new MapiExceptionJetErrorDatabaseNotFound(message, hr, ec, diagCtx, innerException);
						}
						switch (ec)
						{
						case -1104:
							return new MapiExceptionJetErrorInvalidSesid(message, hr, ec, diagCtx, innerException);
						case -1103:
							goto IL_124A;
						case -1102:
							return new MapiExceptionJetErrorWriteConflict(message, hr, ec, diagCtx, innerException);
						case -1101:
							return new MapiExceptionJetErrorOutOfSessions(message, hr, ec, diagCtx, innerException);
						default:
							goto IL_124A;
						}
					}
					else if (ec <= -1086)
					{
						switch (ec)
						{
						case -1092:
							return new MapiExceptionJetErrorInstanceUnavailableDueToFatalLogDiskFull(message, hr, ec, diagCtx, innerException);
						case -1091:
							goto IL_124A;
						case -1090:
							return new MapiExceptionJetErrorInstanceUnavailable(message, hr, ec, diagCtx, innerException);
						default:
							if (ec != -1086)
							{
								goto IL_124A;
							}
							return new MapiExceptionJetErrorInstanceNameInUse(message, hr, ec, diagCtx, innerException);
						}
					}
					else
					{
						if (ec == -1069)
						{
							return new MapiExceptionJetErrorVersionStoreOutOfMemory(message, hr, ec, diagCtx, innerException);
						}
						if (ec != -1066)
						{
							goto IL_124A;
						}
						return new MapiExceptionJetErrorVersionStoreOutOfMemoryAndCleanupTimedOut(message, hr, ec, diagCtx, innerException);
					}
				}
				else if (ec <= -1011)
				{
					if (ec <= -1047)
					{
						if (ec == -1062)
						{
							return new MapiExceptionJetErrorInvalidLanguageId(message, hr, ec, diagCtx, innerException);
						}
						if (ec != -1047)
						{
							goto IL_124A;
						}
						return new MapiExceptionJetErrorInvalidBufferSize(message, hr, ec, diagCtx, innerException);
					}
					else
					{
						if (ec == -1026)
						{
							return new MapiExceptionJetErrorRecordTooBig(message, hr, ec, diagCtx, innerException);
						}
						switch (ec)
						{
						case -1022:
							return new MapiExceptionJetErrorDiskIO(message, hr, ec, diagCtx, innerException);
						case -1021:
						case -1020:
						case -1016:
						case -1015:
						case -1012:
							goto IL_124A;
						case -1019:
							return new MapiExceptionJetErrorPageNotInitialized(message, hr, ec, diagCtx, innerException);
						case -1018:
							return new MapiExceptionJetErrorReadVerifyFailure(message, hr, ec, diagCtx, innerException);
						case -1017:
							return new MapiExceptionJetErrorRecordDeleted(message, hr, ec, diagCtx, innerException);
						case -1014:
							return new MapiExceptionJetErrorOutOfBuffers(message, hr, ec, diagCtx, innerException);
						case -1013:
							return new MapiExceptionJetErrorOutOfCursors(message, hr, ec, diagCtx, innerException);
						case -1011:
							return new MapiExceptionJetErrorOutOfMemory(message, hr, ec, diagCtx, innerException);
						default:
							goto IL_124A;
						}
					}
				}
				else if (ec <= -602)
				{
					if (ec == -614)
					{
						return new MapiExceptionJetErrorCheckpointDepthTooDeep(message, hr, ec, diagCtx, innerException);
					}
					if (ec != -602)
					{
						goto IL_124A;
					}
					return new MapiExceptionJetErrorUnicodeTranslationFail(message, hr, ec, diagCtx, innerException);
				}
				else if (ec != -586 && ec != -582)
				{
					goto IL_124A;
				}
			}
			else if (ec <= 1261)
			{
				if (ec <= 1000)
				{
					if (ec <= -528)
					{
						if (ec == -551)
						{
							return new MapiExceptionJetErrorConsistentTimeMismatch(message, hr, ec, diagCtx, innerException);
						}
						if (ec != -543)
						{
							switch (ec)
							{
							case -529:
								return new MapiExceptionJetErrorLogDiskFull(message, hr, ec, diagCtx, innerException);
							case -528:
								return new MapiExceptionJetErrorMissingLogFile(message, hr, ec, diagCtx, innerException);
							default:
								goto IL_124A;
							}
						}
					}
					else if (ec <= -338)
					{
						if (ec == -510)
						{
							return new MapiExceptionJetErrorLogWriteFail(message, hr, ec, diagCtx, innerException);
						}
						if (ec != -338)
						{
							goto IL_124A;
						}
						goto IL_10EE;
					}
					else
					{
						if (ec == -255)
						{
							return new MapiExceptionJetErrorDatabaseBufferDependenciesCorrupted(message, hr, ec, diagCtx, innerException);
						}
						if (ec != 1000)
						{
							goto IL_124A;
						}
						return new MapiExceptionStoreTestFailure(message, hr, ec, diagCtx, innerException);
					}
				}
				else if (ec <= 1132)
				{
					if (ec <= 1100)
					{
						switch (ec)
						{
						case 1003:
							return new MapiExceptionUnknownUser(message, hr, ec, diagCtx, innerException);
						case 1004:
							goto IL_124A;
						case 1005:
							return new MapiExceptionExiting(message, hr, ec, diagCtx, innerException);
						default:
							if (ec != 1100)
							{
								goto IL_124A;
							}
							return new MapiExceptionNoFreeJses(message, hr, ec, diagCtx, innerException);
						}
					}
					else
					{
						switch (ec)
						{
						case 1108:
							return new MapiExceptionDatabaseError(message, hr, ec, diagCtx, innerException);
						case 1109:
							goto IL_124A;
						case 1110:
							return new MapiExceptionUnsupportedProp(message, hr, ec, diagCtx, innerException);
						default:
							switch (ec)
							{
							case 1127:
								return new MapiExceptionInvalidRecipients(message, hr, ec, diagCtx, innerException);
							case 1128:
								return new MapiExceptionNoReplicaHere(message, hr, ec, diagCtx, innerException);
							case 1129:
								return new MapiExceptionNoReplicaAvailable(message, hr, ec, diagCtx, innerException);
							case 1130:
							case 1131:
								goto IL_124A;
							case 1132:
								return new MapiExceptionNoRecordFound(message, hr, ec, diagCtx, innerException);
							default:
								goto IL_124A;
							}
							break;
						}
					}
				}
				else if (ec <= 1199)
				{
					switch (ec)
					{
					case 1140:
						return new MapiExceptionMaxTimeExpired(message, hr, ec, diagCtx, innerException);
					case 1141:
					case 1143:
					case 1145:
					case 1146:
					case 1147:
					case 1148:
					case 1152:
					case 1153:
					case 1154:
					case 1155:
					case 1156:
					case 1158:
					case 1159:
					case 1160:
					case 1161:
					case 1162:
					case 1166:
					case 1178:
					case 1179:
					case 1180:
					case 1183:
					case 1184:
						goto IL_124A;
					case 1142:
						return new MapiExceptionMdbOffline(message, hr, ec, diagCtx, innerException);
					case 1144:
						return new MapiExceptionWrongServer(message, hr, ec, diagCtx, innerException);
					case 1149:
						return new MapiExceptionRpcBufferTooSmall(message, hr, ec, diagCtx, innerException);
					case 1150:
						return new MapiExceptionRequiresRefResolve(message, hr, ec, diagCtx, innerException);
					case 1151:
						return new MapiExceptionServerPaused(message, hr, ec, diagCtx, innerException);
					case 1157:
						return new MapiExceptionDataLoss(message, hr, ec, diagCtx, innerException);
					case 1163:
						return new MapiExceptionNotPrivateMDB(message, hr, ec, diagCtx, innerException);
					case 1164:
						return new MapiExceptionIsintegMDB(message, hr, ec, diagCtx, innerException);
					case 1165:
						return new MapiExceptionRecoveryMDBMismatch(message, hr, ec, diagCtx, innerException);
					case 1167:
						return new MapiExceptionSearchFolderNotEmpty(message, hr, ec, diagCtx, innerException);
					case 1168:
						return new MapiExceptionSearchFolderScopeViolation(message, hr, ec, diagCtx, innerException);
					case 1169:
						return new MapiExceptionCannotDeriveMsgViewFromBase(message, hr, ec, diagCtx, innerException);
					case 1170:
						return new MapiExceptionMsgHeaderIndexMismatch(message, hr, ec, diagCtx, innerException);
					case 1171:
						return new MapiExceptionMsgHeaderViewTableMismatch(message, hr, ec, diagCtx, innerException);
					case 1172:
						return new MapiExceptionCategViewTableMismatch(message, hr, ec, diagCtx, innerException);
					case 1173:
						return new MapiExceptionCorruptConversation(message, hr, ec, diagCtx, innerException);
					case 1174:
						return new MapiExceptionConversationNotFound(message, hr, ec, diagCtx, innerException);
					case 1175:
						return new MapiExceptionConversationMemberNotFound(message, hr, ec, diagCtx, innerException);
					case 1176:
						return new MapiExceptionVersionStoreBusy(message, hr, ec, diagCtx, innerException);
					case 1177:
						return new MapiExceptionSearchEvaluationInProgress(message, hr, ec, diagCtx, innerException);
					case 1181:
						return new MapiExceptionRecursiveSearchChainTooDeep(message, hr, ec, diagCtx, innerException);
					case 1182:
						return new MapiExceptionEmbeddedMessagePropertyCopyFailed(message, hr, ec, diagCtx, innerException);
					case 1185:
						return new MapiExceptionGlobalCounterRangeExceeded(message, hr, ec, diagCtx, innerException);
					case 1186:
						return new MapiExceptionCorruptMidsetDeleted(message, hr, ec, diagCtx, innerException);
					default:
						if (ec != 1199)
						{
							goto IL_124A;
						}
						return new MapiExceptionAssertionFailedError(message, hr, ec, diagCtx, innerException);
					}
				}
				else
				{
					if (ec == 1212)
					{
						return new MapiExceptionRpcAuthentication(message, hr, ec, diagCtx, innerException);
					}
					switch (ec)
					{
					case 1235:
						return new MapiExceptionInvalidRTF(message, hr, ec, diagCtx, innerException);
					case 1236:
						return new MapiExceptionMessageTooBig(message, hr, ec, diagCtx, innerException);
					case 1237:
						return new MapiExceptionFormNotValid(message, hr, ec, diagCtx, innerException);
					case 1238:
						return new MapiExceptionNotAuthorized(message, hr, ec, diagCtx, innerException);
					case 1239:
					case 1240:
					case 1248:
					case 1249:
					case 1250:
					case 1257:
					case 1258:
					case 1259:
					case 1260:
						goto IL_124A;
					case 1241:
						return new MapiExceptionQuotaExceeded(message, hr, ec, diagCtx, innerException);
					case 1242:
						return new MapiExceptionMaxSubmissionExceeded(message, hr, ec, diagCtx, innerException);
					case 1243:
						return new MapiExceptionMaxAttachmentExceeded(message, hr, ec, diagCtx, innerException);
					case 1244:
						return new MapiExceptionSendAsDenied(message, hr, ec, diagCtx, innerException);
					case 1245:
						return new MapiExceptionShutoffQuotaExceeded(message, hr, ec, diagCtx, innerException);
					case 1246:
					case 1252:
					case 1253:
					case 1254:
					case 1255:
					case 1256:
						return new MapiExceptionMaxObjsExceeded(message, hr, ec, diagCtx, innerException);
					case 1247:
						return new MapiExceptionClientVersionDisallowed(message, hr, ec, diagCtx, innerException);
					case 1251:
						return new MapiExceptionFolderNotCleanedUp(message, hr, ec, diagCtx, innerException);
					case 1261:
						return new MapiExceptionFormatError(message, hr, ec, diagCtx, innerException);
					default:
						goto IL_124A;
					}
				}
			}
			else if (ec <= 2039)
			{
				if (ec <= 1512)
				{
					if (ec <= 1305)
					{
						switch (ec)
						{
						case 1275:
							return new MapiExceptionFolderDisabled(message, hr, ec, diagCtx, innerException);
						case 1276:
						case 1277:
						case 1278:
						case 1281:
						case 1283:
						case 1287:
						case 1288:
						case 1289:
							goto IL_124A;
						case 1279:
							return new MapiExceptionNoCreateRight(message, hr, ec, diagCtx, innerException);
						case 1280:
							return new MapiExceptionPublicRoot(message, hr, ec, diagCtx, innerException);
						case 1282:
							return new MapiExceptionNoCreateSubfolderRight(message, hr, ec, diagCtx, innerException);
						case 1284:
							return new MapiExceptionMsgCycle(message, hr, ec, diagCtx, innerException);
						case 1285:
							return new MapiExceptionTooManyRecips(message, hr, ec, diagCtx, innerException);
						case 1286:
							return new MapiExceptionTooManyProps(message, hr, ec, diagCtx, innerException);
						case 1290:
							return new MapiExceptionVirusScanInProgress(message, hr, ec, diagCtx, innerException);
						case 1291:
							return new MapiExceptionVirusDetected(message, hr, ec, diagCtx, innerException);
						case 1292:
							return new MapiExceptionMailboxInTransit(message, hr, ec, diagCtx, innerException);
						case 1293:
							return new MapiExceptionBackupInProgress(message, hr, ec, diagCtx, innerException);
						case 1294:
							return new MapiExceptionVirusMessageDeleted(message, hr, ec, diagCtx, innerException);
						default:
							if (ec != 1305)
							{
								goto IL_124A;
							}
							return new MapiExceptionPropsDontMatch(message, hr, ec, diagCtx, innerException);
						}
					}
					else
					{
						if (ec == 1401)
						{
							return new MapiExceptionDuplicateObject(message, hr, ec, diagCtx, innerException);
						}
						if (ec != 1512)
						{
							goto IL_124A;
						}
						return new MapiExceptionJetWarningColumnMaxTruncated(message, hr, ec, diagCtx, innerException);
					}
				}
				else if (ec <= 1702)
				{
					switch (ec)
					{
					case 1608:
						return new MapiExceptionWrongMailbox(message, hr, ec, diagCtx, innerException);
					case 1609:
					case 1610:
					case 1611:
						goto IL_124A;
					case 1612:
						return new MapiExceptionPasswordChangeRequired(message, hr, ec, diagCtx, innerException);
					case 1613:
						return new MapiExceptionPasswordExpired(message, hr, ec, diagCtx, innerException);
					case 1614:
						return new MapiExceptionInvalidWorkstationAccount(message, hr, ec, diagCtx, innerException);
					case 1615:
						return new MapiExceptionInvalidAccessTime(message, hr, ec, diagCtx, innerException);
					case 1616:
						return new MapiExceptionAccountDisabled(message, hr, ec, diagCtx, innerException);
					default:
						switch (ec)
						{
						case 1700:
							return new MapiExceptionRuleVersion(message, hr, ec, diagCtx, innerException);
						case 1701:
							return new MapiExceptionRuleFormat(message, hr, ec, diagCtx, innerException);
						case 1702:
							return new MapiExceptionRuleSendAsDenied(message, hr, ec, diagCtx, innerException);
						default:
							goto IL_124A;
						}
						break;
					}
				}
				else
				{
					if (ec == 2008)
					{
						return new MapiExceptionProtocolDisabled(message, hr, ec, diagCtx, innerException);
					}
					if (ec != 2039)
					{
						goto IL_124A;
					}
					return new MapiExceptionCrossPostDenied(message, hr, ec, diagCtx, innerException);
				}
			}
			else if (ec <= 2430)
			{
				if (ec <= 2084)
				{
					if (ec == 2053)
					{
						return new MapiExceptionNoMessages(message, hr, ec, diagCtx, innerException);
					}
					if (ec != 2084)
					{
						goto IL_124A;
					}
					return new MapiExceptionNoRpcInterface(message, hr, ec, diagCtx, innerException);
				}
				else
				{
					switch (ec)
					{
					case 2202:
						return new MapiExceptionAmbiguousAlias(message, hr, ec, diagCtx, innerException);
					case 2203:
						return new MapiExceptionUnknownMailbox(message, hr, ec, diagCtx, innerException);
					default:
						switch (ec)
						{
						case 2407:
							return new MapiExceptionEventError(message, hr, ec, diagCtx, innerException);
						case 2408:
							return new MapiExceptionWatermarkError(message, hr, ec, diagCtx, innerException);
						case 2409:
							return new MapiExceptionNonCanonicalACL(message, hr, ec, diagCtx, innerException);
						case 2410:
						case 2411:
						case 2413:
						case 2426:
							goto IL_124A;
						case 2412:
							return new MapiExceptionMailboxDisabled(message, hr, ec, diagCtx, innerException);
						case 2414:
							return new MapiExceptionADUnavailable(message, hr, ec, diagCtx, innerException);
						case 2415:
							return new MapiExceptionADError(message, hr, ec, diagCtx, innerException);
						case 2416:
							return new MapiExceptionNotEncrypted(message, hr, ec, diagCtx, innerException);
						case 2417:
							return new MapiExceptionADNotFound(message, hr, ec, diagCtx, innerException);
						case 2418:
							return new MapiExceptionADPropertyError(message, hr, ec, diagCtx, innerException);
						case 2419:
							return new MapiExceptionRpcServerTooBusy(message, hr, ec, diagCtx, innerException);
						case 2420:
							return new MapiExceptionRpcOutOfMemory(message, hr, ec, diagCtx, innerException);
						case 2421:
							return new MapiExceptionRpcServerOutOfMemory(message, hr, ec, diagCtx, innerException);
						case 2422:
							return new MapiExceptionRpcOutOfResources(message, hr, ec, diagCtx, innerException);
						case 2423:
							return new MapiExceptionNetworkError(message, hr, ec, diagCtx, innerException);
						case 2424:
							return new MapiExceptionADDuplicateEntry(message, hr, ec, diagCtx, innerException);
						case 2425:
							return new MapiExceptionImailConversion(message, hr, ec, diagCtx, innerException);
						case 2427:
							return new MapiExceptionImailConversionProhibited(message, hr, ec, diagCtx, innerException);
						case 2428:
							return new MapiExceptionEventsDeleted(message, hr, ec, diagCtx, innerException);
						case 2429:
							return new MapiExceptionSubsystemStopping(message, hr, ec, diagCtx, innerException);
						case 2430:
							return new MapiExceptionSystemAttendantUnavailable(message, hr, ec, diagCtx, innerException);
						default:
							goto IL_124A;
						}
						break;
					}
				}
			}
			else if (ec <= 2704)
			{
				switch (ec)
				{
				case 2600:
					return new MapiExceptionCIStopping(message, hr, ec, diagCtx, innerException);
				case 2601:
					return new MapiExceptionFxInvalidState(message, hr, ec, diagCtx, innerException);
				case 2602:
					return new MapiExceptionFxUnexpectedMarker(message, hr, ec, diagCtx, innerException);
				case 2603:
					return new MapiExceptionDuplicateDelivery(message, hr, ec, diagCtx, innerException);
				case 2604:
					return new MapiExceptionConditionViolation(message, hr, ec, diagCtx, innerException);
				case 2605:
				case 2608:
				case 2614:
				case 2615:
				case 2616:
				case 2622:
				case 2623:
				case 2624:
				case 2627:
				case 2629:
				case 2630:
				case 2631:
				case 2633:
					goto IL_124A;
				case 2606:
					return new MapiExceptionNetworkError(message, hr, ec, diagCtx, innerException);
				case 2607:
					return new MapiExceptionEventNotFound(message, hr, ec, diagCtx, innerException);
				case 2609:
					return new MapiExceptionLowDatabaseDiskSpace(message, hr, ec, diagCtx, innerException);
				case 2610:
					return new MapiExceptionLowDatabaseLogDiskSpace(message, hr, ec, diagCtx, innerException);
				case 2611:
					return new MapiExceptionMailboxQuarantined(message, hr, ec, diagCtx, innerException);
				case 2612:
					return new MapiExceptionMountInProgress(message, hr, ec, diagCtx, innerException);
				case 2613:
					return new MapiExceptionDismountInProgress(message, hr, ec, diagCtx, innerException);
				case 2617:
					return new MapiExceptionNetworkError(message, hr, ec, diagCtx, innerException);
				case 2618:
					return new MapiExceptionVirusScannerError(message, hr, ec, diagCtx, innerException);
				case 2619:
					return new MapiExceptionGranularReplInitFailed(message, hr, ec, diagCtx, innerException);
				case 2620:
					return new MapiExceptionCannotRegisterNewReplidGuidMapping(message, hr, ec, diagCtx, innerException);
				case 2621:
					return new MapiExceptionCannotRegisterNewNamedPropertyMapping(message, hr, ec, diagCtx, innerException);
				case 2625:
					return new MapiExceptionGranularReplInvalidParameter(message, hr, ec, diagCtx, innerException);
				case 2626:
					return new MapiExceptionGranularReplStillInUse(message, hr, ec, diagCtx, innerException);
				case 2628:
					return new MapiExceptionGranularCommunicationFailed(message, hr, ec, diagCtx, innerException);
				case 2632:
					return new MapiExceptionCannotPreserveMailboxSignature(message, hr, ec, diagCtx, innerException);
				case 2634:
					return new MapiExceptionUnexpectedMailboxState(message, hr, ec, diagCtx, innerException);
				case 2635:
					return new MapiExceptionMailboxSoftDeleted(message, hr, ec, diagCtx, innerException);
				case 2636:
					return new MapiExceptionDatabaseStateConflict(message, hr, ec, diagCtx, innerException);
				case 2637:
					return new MapiExceptionNetworkError(message, hr, ec, diagCtx, innerException);
				default:
					switch (ec)
					{
					case 2700:
						return new MapiExceptionMaxThreadsPerMdbExceeded(message, hr, ec, diagCtx, innerException);
					case 2701:
						return new MapiExceptionMaxThreadsPerSCTExceeded(message, hr, ec, diagCtx, innerException);
					case 2702:
						return new MapiExceptionWrongProvisionedFid(message, hr, ec, diagCtx, innerException);
					case 2703:
						return new MapiExceptionISIntegMdbTaskExceeded(message, hr, ec, diagCtx, innerException);
					case 2704:
						return new MapiExceptionISIntegQueueFull(message, hr, ec, diagCtx, innerException);
					default:
						goto IL_124A;
					}
					break;
				}
			}
			else
			{
				switch (ec)
				{
				case 2800:
					return new MapiExceptionInvalidMultiMailboxSearchRequest(message, hr, ec, diagCtx, innerException);
				case 2801:
					return new MapiExceptionInvalidMultiMailboxKeywordStatsRequest(message, hr, ec, diagCtx, innerException);
				case 2802:
					return new MapiExceptionMultiMailboxSearchFailed(message, hr, ec, diagCtx, innerException);
				case 2803:
					return new MapiExceptionMaxMultiMailboxSearchExceeded(message, hr, ec, diagCtx, innerException);
				case 2804:
					return new MapiExceptionMultiMailboxSearchOperationFailed(message, hr, ec, diagCtx, innerException);
				case 2805:
					return new MapiExceptionMultiMailboxSearchNonFullTextSearch(message, hr, ec, diagCtx, innerException);
				case 2806:
					return new MapiExceptionMultiMailboxSearchTimeOut(message, hr, ec, diagCtx, innerException);
				case 2807:
					return new MapiExceptionMultiMailboxKeywordStatsTimeOut(message, hr, ec, diagCtx, innerException);
				case 2808:
					return new MapiExceptionMultiMailboxSearchInvalidSortBy(message, hr, ec, diagCtx, innerException);
				case 2809:
					return new MapiExceptionMultiMailboxSearchNonFullTextSortBy(message, hr, ec, diagCtx, innerException);
				case 2810:
					return new MapiExceptionMultiMailboxSearchInvalidPagination(message, hr, ec, diagCtx, innerException);
				case 2811:
					return new MapiExceptionMultiMailboxSearchNonFullTextPropertyInPagination(message, hr, ec, diagCtx, innerException);
				case 2812:
					return new MapiExceptionMultiMailboxSearchMailboxNotFound(message, hr, ec, diagCtx, innerException);
				case 2813:
					return new MapiExceptionMultiMailboxSearchInvalidRestriction(message, hr, ec, diagCtx, innerException);
				case 2814:
				case 2815:
				case 2816:
				case 2817:
				case 2818:
				case 2819:
				case 2820:
				case 2821:
				case 2822:
				case 2823:
				case 2824:
				case 2825:
				case 2826:
				case 2827:
				case 2828:
				case 2829:
					goto IL_124A;
				case 2830:
					return new MapiExceptionUserInformationAlreadyExists(message, hr, ec, diagCtx, innerException);
				case 2831:
					return new MapiExceptionUserInformationLockTimeout(message, hr, ec, diagCtx, innerException);
				case 2832:
					return new MapiExceptionUserInformationNotFound(message, hr, ec, diagCtx, innerException);
				case 2833:
					return new MapiExceptionUserInformationNoAccess(message, hr, ec, diagCtx, innerException);
				default:
					if (ec != 263552)
					{
						goto IL_124A;
					}
					return new MapiExceptionCancelMessage(message, hr, ec, diagCtx, innerException);
				}
			}
			return new MapiExceptionJetErrorLogFilesMissingOrCorrupt(message, hr, ec, diagCtx, innerException);
			IL_10EE:
			return new MapiExceptionJetDatabaseCorruption(message, hr, ec, diagCtx, innerException);
			IL_124A:
			return null;
		}

		private static Exception Create(string message, int hr, int ec, bool allowWarnings, DiagnosticContext diagCtx, Exception innerException)
		{
			if (hr == 0)
			{
				return null;
			}
			if (hr > 0)
			{
				if (!allowWarnings)
				{
					return null;
				}
				if (hr == 263808)
				{
					return new MapiExceptionPartialCompletion(message, hr, ec, diagCtx, innerException);
				}
				if (hr == 263815)
				{
					return new MapiExceptionPartialItems(message, hr, ec, diagCtx, innerException);
				}
				if (hr != 264225)
				{
					return null;
				}
				return new MapiExceptionSyncClientChangeNewer(message, hr, ec, diagCtx, innerException);
			}
			else
			{
				if (hr <= -2147220475)
				{
					if (hr <= -2147286953)
					{
						if (hr <= -2147287032)
						{
							if (hr == -2147467262)
							{
								return new MapiExceptionInterfaceNotSupported(message, hr, ec, diagCtx, innerException);
							}
							if (hr != -2147467259)
							{
								if (hr != -2147287032)
								{
									goto IL_7EF;
								}
							}
							else
							{
								Exception ex = MapiExceptionHelper.CreateLowLevel(message, hr, ec, diagCtx, innerException);
								if (ex != null)
								{
									return ex;
								}
								return new MapiExceptionCallFailed(message, hr, ec, diagCtx, innerException);
							}
						}
						else
						{
							if (hr == -2147287015)
							{
								return new MapiExceptionStreamSeekError(message, hr, ec, diagCtx, innerException);
							}
							if (hr == -2147287007)
							{
								return new MapiExceptionLockViolation(message, hr, ec, diagCtx, innerException);
							}
							if (hr != -2147286953)
							{
								goto IL_7EF;
							}
							goto IL_394;
						}
					}
					else if (hr <= -2147220990)
					{
						if (hr == -2147286928)
						{
							return new MapiExceptionStreamSizeError(message, hr, ec, diagCtx, innerException);
						}
						switch (hr)
						{
						case -2147221246:
							return new MapiExceptionNoSupport(message, hr, ec, diagCtx, innerException);
						case -2147221245:
							return new MapiExceptionBadCharWidth(message, hr, ec, diagCtx, innerException);
						case -2147221244:
						case -2147221236:
							goto IL_7EF;
						case -2147221243:
							return new MapiExceptionStringTooLong(message, hr, ec, diagCtx, innerException);
						case -2147221242:
							return new MapiExceptionUnknownFlags(message, hr, ec, diagCtx, innerException);
						case -2147221241:
							return new MapiExceptionInvalidEntryId(message, hr, ec, diagCtx, innerException);
						case -2147221240:
							return new MapiExceptionInvalidObject(message, hr, ec, diagCtx, innerException);
						case -2147221239:
							return new MapiExceptionObjectChanged(message, hr, ec, diagCtx, innerException);
						case -2147221238:
							return new MapiExceptionObjectDeleted(message, hr, ec, diagCtx, innerException);
						case -2147221237:
							return new MapiExceptionBusy(message, hr, ec, diagCtx, innerException);
						case -2147221235:
							return new MapiExceptionNotEnoughDisk(message, hr, ec, diagCtx, innerException);
						case -2147221234:
							return new MapiExceptionNotEnoughResources(message, hr, ec, diagCtx, innerException);
						case -2147221233:
							return new MapiExceptionNotFound(message, hr, ec, diagCtx, innerException);
						case -2147221232:
							return new MapiExceptionVersion(message, hr, ec, diagCtx, innerException);
						case -2147221231:
							return new MapiExceptionLogonFailed(message, hr, ec, diagCtx, innerException);
						case -2147221230:
							return new MapiExceptionSessionLimit(message, hr, ec, diagCtx, innerException);
						case -2147221229:
							return new MapiExceptionUserCancel(message, hr, ec, diagCtx, innerException);
						case -2147221228:
							return new MapiExceptionUnableToAbort(message, hr, ec, diagCtx, innerException);
						case -2147221227:
							return new MapiExceptionNetworkError(message, hr, ec, diagCtx, innerException);
						case -2147221226:
							return new MapiExceptionDiskError(message, hr, ec, diagCtx, innerException);
						case -2147221225:
							return new MapiExceptionTooComplex(message, hr, ec, diagCtx, innerException);
						case -2147221224:
							return new MapiExceptionBadColumn(message, hr, ec, diagCtx, innerException);
						case -2147221223:
							return new MapiExceptionExtendedError(message, hr, ec, diagCtx, innerException);
						case -2147221222:
							return new MapiExceptionComputed(message, hr, ec, diagCtx, innerException);
						case -2147221221:
							return new MapiExceptionCorruptData(message, hr, ec, diagCtx, innerException);
						case -2147221220:
							return new MapiExceptionUnconfigured(message, hr, ec, diagCtx, innerException);
						case -2147221219:
							return new MapiExceptionFailOneProvider(message, hr, ec, diagCtx, innerException);
						case -2147221218:
							return new MapiExceptionUnknownCpid(message, hr, ec, diagCtx, innerException);
						case -2147221217:
							return new MapiExceptionUnknownLcid(message, hr, ec, diagCtx, innerException);
						case -2147221216:
							return new MapiExceptionPasswordChangeRequired(message, hr, ec, diagCtx, innerException);
						case -2147221215:
							return new MapiExceptionPasswordExpired(message, hr, ec, diagCtx, innerException);
						case -2147221214:
							return new MapiExceptionInvalidWorkstationAccount(message, hr, ec, diagCtx, innerException);
						case -2147221213:
							return new MapiExceptionInvalidAccessTime(message, hr, ec, diagCtx, innerException);
						case -2147221212:
							return new MapiExceptionAccountDisabled(message, hr, ec, diagCtx, innerException);
						case -2147221211:
							return new MapiExceptionConflict(message, hr, ec, diagCtx, innerException);
						default:
							switch (hr)
							{
							case -2147220992:
								return new MapiExceptionEndOfSession(message, hr, ec, diagCtx, innerException);
							case -2147220991:
								return new MapiExceptionUnknownEntryId(message, hr, ec, diagCtx, innerException);
							case -2147220990:
								return new MapiExceptionMissingRequiredColumn(message, hr, ec, diagCtx, innerException);
							default:
								goto IL_7EF;
							}
							break;
						}
					}
					else
					{
						if (hr == -2147220967)
						{
							return new MapiExceptionFailCallback(message, hr, ec, diagCtx, innerException);
						}
						switch (hr)
						{
						case -2147220735:
							return new MapiExceptionBadValue(message, hr, ec, diagCtx, innerException);
						case -2147220734:
							return new MapiExceptionInvalidType(message, hr, ec, diagCtx, innerException);
						case -2147220733:
							return new MapiExceptionTypeNoSupport(message, hr, ec, diagCtx, innerException);
						case -2147220732:
							return new MapiExceptionUnexpectedType(message, hr, ec, diagCtx, innerException);
						case -2147220731:
						{
							Exception ex2 = MapiExceptionHelper.CreateLowLevel(message, hr, ec, diagCtx, innerException);
							if (ex2 != null)
							{
								return ex2;
							}
							return new MapiExceptionTooBig(message, hr, ec, diagCtx, innerException);
						}
						case -2147220730:
							return new MapiExceptionDeclineCopy(message, hr, ec, diagCtx, innerException);
						case -2147220729:
							return new MapiExceptionUnexpectedId(message, hr, ec, diagCtx, innerException);
						default:
							switch (hr)
							{
							case -2147220480:
								return new MapiExceptionUnableToComplete(message, hr, ec, diagCtx, innerException);
							case -2147220479:
								return new MapiExceptionTimeout(message, hr, ec, diagCtx, innerException);
							case -2147220478:
								return new MapiExceptionTableEmpty(message, hr, ec, diagCtx, innerException);
							case -2147220477:
								return new MapiExceptionTableTooBig(message, hr, ec, diagCtx, innerException);
							case -2147220476:
								goto IL_7EF;
							case -2147220475:
								return new MapiExceptionInvalidBookmark(message, hr, ec, diagCtx, innerException);
							default:
								goto IL_7EF;
							}
							break;
						}
					}
				}
				else if (hr <= -2147219452)
				{
					if (hr <= -2147219954)
					{
						if (hr == -2147220347)
						{
							return new MapiExceptionDataLoss(message, hr, ec, diagCtx, innerException);
						}
						switch (hr)
						{
						case -2147220224:
							return new MapiExceptionWait(message, hr, ec, diagCtx, innerException);
						case -2147220223:
							return new MapiExceptionCancel(message, hr, ec, diagCtx, innerException);
						case -2147220222:
							return new MapiExceptionNotMe(message, hr, ec, diagCtx, innerException);
						default:
							switch (hr)
							{
							case -2147219968:
								return new MapiExceptionCorruptStore(message, hr, ec, diagCtx, innerException);
							case -2147219967:
								return new MapiExceptionNotInQueue(message, hr, ec, diagCtx, innerException);
							case -2147219966:
								return new MapiExceptionNoSuppress(message, hr, ec, diagCtx, innerException);
							case -2147219965:
								goto IL_7EF;
							case -2147219964:
								return new MapiExceptionCollision(message, hr, ec, diagCtx, innerException);
							case -2147219963:
								return new MapiExceptionNotInitialized(message, hr, ec, diagCtx, innerException);
							case -2147219962:
								return new MapiExceptionNonStandard(message, hr, ec, diagCtx, innerException);
							case -2147219961:
								return new MapiExceptionNoRecipients(message, hr, ec, diagCtx, innerException);
							case -2147219960:
								return new MapiExceptionSubmitted(message, hr, ec, diagCtx, innerException);
							case -2147219959:
								return new MapiExceptionHasFolders(message, hr, ec, diagCtx, innerException);
							case -2147219958:
								return new MapiExceptionHasMessages(message, hr, ec, diagCtx, innerException);
							case -2147219957:
								return new MapiExceptionFolderCycle(message, hr, ec, diagCtx, innerException);
							case -2147219956:
								return new MapiExceptionRecursionLimit(message, hr, ec, diagCtx, innerException);
							case -2147219955:
								return new MapiExceptionLockIdLimit(message, hr, ec, diagCtx, innerException);
							case -2147219954:
								return new MapiExceptionTooManyMountedDatabases(message, hr, ec, diagCtx, innerException);
							default:
								goto IL_7EF;
							}
							break;
						}
					}
					else
					{
						if (hr == -2147219834)
						{
							return new MapiExceptionPartialItem(message, hr, ec, diagCtx, innerException);
						}
						if (hr == -2147219712)
						{
							return new MapiExceptionAmbiguousRecip(message, hr, ec, diagCtx, innerException);
						}
						switch (hr)
						{
						case -2147219456:
							return new MapiExceptionObjectDeleted(message, hr, ec, diagCtx, innerException);
						case -2147219455:
							return new MapiExceptionSyncIgnore(message, hr, ec, diagCtx, innerException);
						case -2147219454:
							return new MapiExceptionSyncConflict(message, hr, ec, diagCtx, innerException);
						case -2147219453:
							return new MapiExceptionSyncNoParent(message, hr, ec, diagCtx, innerException);
						case -2147219452:
							return new MapiExceptionSyncIncest(message, hr, ec, diagCtx, innerException);
						default:
							goto IL_7EF;
						}
					}
				}
				else if (hr <= -2147024882)
				{
					if (hr == -2147219200)
					{
						return new MapiExceptionNamedPropsQuotaExceeded(message, hr, ec, diagCtx, innerException);
					}
					switch (hr)
					{
					case -2147024893:
						return new MapiExceptionPathNotFound(message, hr, ec, diagCtx, innerException);
					case -2147024892:
						goto IL_7EF;
					case -2147024891:
					{
						Exception ex3 = MapiExceptionHelper.CreateLowLevel(message, hr, ec, diagCtx, innerException);
						if (ex3 != null)
						{
							return ex3;
						}
						return new MapiExceptionNoAccess(message, hr, ec, diagCtx, innerException);
					}
					default:
						if (hr != -2147024882)
						{
							goto IL_7EF;
						}
						break;
					}
				}
				else if (hr <= -2147024784)
				{
					if (hr == -2147024809)
					{
						goto IL_394;
					}
					if (hr != -2147024784)
					{
						goto IL_7EF;
					}
					return new MapiExceptionNotEnoughDisk(message, hr, ec, diagCtx, innerException);
				}
				else
				{
					if (hr == -2147023893)
					{
						return new MapiExceptionCanNotComplete(message, hr, ec, diagCtx, innerException);
					}
					if (hr != -1073478950)
					{
						goto IL_7EF;
					}
					return new MapiExceptionTooManyRecips(message, hr, ec, diagCtx, innerException);
				}
				return new MapiExceptionNotEnoughMemory(message, hr, ec, diagCtx, innerException);
				IL_394:
				return new MapiExceptionInvalidParameter(message, hr, ec, diagCtx, innerException);
				IL_7EF:
				Exception ex4;
				if (hr == ec && Hresult.GetFacility(hr) == Hresult.Facility.ITF)
				{
					ex4 = MapiExceptionHelper.CreateLowLevel(message, hr, Hresult.GetScode(ec), diagCtx, innerException);
				}
				else
				{
					ex4 = MapiExceptionHelper.CreateLowLevel(message, hr, ec, diagCtx, innerException);
				}
				if (ex4 != null)
				{
					return ex4;
				}
				return new MapiExceptionCallFailed(message, hr, ec, diagCtx, innerException);
			}
		}

		private static void InternalThrowIfErrorOrWarning(string message, int hresult, bool allowWarnings, int ec, DiagnosticContext diagCtx, Exception innerException)
		{
			if (hresult != 0)
			{
				Exception ex = MapiExceptionHelper.Create(message, hresult, ec, allowWarnings, diagCtx, innerException);
				if (ex != null)
				{
					throw ex;
				}
			}
		}

		private static IExLastErrorInfo GetILastErrorInfo(IExInterface iUnknown)
		{
			IExInterface exInterface = null;
			IExLastErrorInfo result = null;
			try
			{
				if (iUnknown.QueryInterface(InterfaceIds.ILastErrorInfo, out exInterface) == 0 && exInterface != null && !exInterface.IsInvalid)
				{
					result = exInterface.ToInterface<IExLastErrorInfo>();
					exInterface = null;
				}
			}
			finally
			{
				exInterface.DisposeIfValid();
			}
			return result;
		}

		private static ILastErrorInfo GetILastErrorInfo(object objUnk)
		{
			return objUnk as ILastErrorInfo;
		}

		private static int GetLowLevelError(int hResult, IExInterface iUnknown)
		{
			return MapiExceptionHelper.GetLowLevelError(hResult, false, iUnknown);
		}

		private static int GetLowLevelError(int hResult, bool fAllowWarning, IExInterface iUnknown)
		{
			int result = 0;
			if (iUnknown == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("iUnknown");
			}
			if (hResult < 0 || (hResult > 0 && fAllowWarning))
			{
				IExLastErrorInfo exLastErrorInfo = null;
				try
				{
					exLastErrorInfo = MapiExceptionHelper.GetILastErrorInfo(iUnknown);
					if (exLastErrorInfo != null)
					{
						exLastErrorInfo.GetLastError(hResult, out result);
					}
				}
				finally
				{
					exLastErrorInfo.DisposeIfValid();
				}
			}
			return result;
		}

		private static int GetLowLevelError(int hResult, bool fAllowWarning, object objUnk)
		{
			int result = 0;
			if (objUnk == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("objUnk");
			}
			if (hResult < 0 || (hResult > 0 && fAllowWarning))
			{
				ILastErrorInfo ilastErrorInfo = MapiExceptionHelper.GetILastErrorInfo(objUnk);
				if (ilastErrorInfo != null)
				{
					SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
					try
					{
						if (ilastErrorInfo.GetLastError(hResult, out safeExLinkedMemoryHandle) == 0 && !safeExLinkedMemoryHandle.IsInvalid)
						{
							result = Marshal.ReadInt32(safeExLinkedMemoryHandle.DangerousGetHandle(), MAPIERROR.LowLevelErrorOffset);
						}
					}
					finally
					{
						if (safeExLinkedMemoryHandle != null)
						{
							safeExLinkedMemoryHandle.Dispose();
						}
					}
				}
			}
			return result;
		}

		private static DiagnosticContext GetDiagnosticContext(IExInterface iUnknown)
		{
			if (iUnknown == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("iUnknown");
			}
			IExLastErrorInfo exLastErrorInfo = null;
			DiagnosticContext result;
			try
			{
				exLastErrorInfo = MapiExceptionHelper.GetILastErrorInfo(iUnknown);
				if (exLastErrorInfo != null)
				{
					DiagnosticContext diagnosticContext = null;
					exLastErrorInfo.GetExtendedErrorInfo(out diagnosticContext);
					result = diagnosticContext;
				}
				else
				{
					result = new DiagnosticContext(null);
				}
			}
			finally
			{
				exLastErrorInfo.DisposeIfValid();
			}
			return result;
		}

		private unsafe static DiagnosticContext GetDiagnosticContext(object objUnk)
		{
			if (objUnk == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("objUnk");
			}
			SafeExMemoryHandle safeExMemoryHandle = null;
			DiagnosticContext result;
			try
			{
				ILastErrorInfo ilastErrorInfo = MapiExceptionHelper.GetILastErrorInfo(objUnk);
				if (ilastErrorInfo != null)
				{
					ilastErrorInfo.GetExtendedErrorInfo(out safeExMemoryHandle);
					result = new DiagnosticContext((THREAD_DIAG_CONTEXT*)safeExMemoryHandle.DangerousGetHandle().ToPointer());
				}
				else
				{
					result = new DiagnosticContext(null);
				}
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		internal static void ThrowIfError(string message, int hresult)
		{
			if (hresult < 0)
			{
				MapiExceptionHelper.InternalThrowIfErrorOrWarning(message, hresult, false, 0, null, null);
			}
		}

		internal static void ThrowIfError(string message, int hresult, int ec)
		{
			if (hresult < 0)
			{
				MapiExceptionHelper.InternalThrowIfErrorOrWarning(message, hresult, false, ec, null, null);
			}
		}

		internal static void ThrowIfError(string message, int hresult, IExInterface iUnknown, Exception innerException)
		{
			if (hresult < 0)
			{
				MapiExceptionHelper.InternalThrowIfErrorOrWarning(message, hresult, false, (iUnknown != null) ? MapiExceptionHelper.GetLowLevelError(hresult, false, iUnknown) : 0, (iUnknown != null) ? MapiExceptionHelper.GetDiagnosticContext(iUnknown) : null, innerException);
			}
		}

		internal static void ThrowIfError(string message, int hresult, object objLastErrorInfo, Exception innerException)
		{
			if (hresult < 0)
			{
				MapiExceptionHelper.InternalThrowIfErrorOrWarning(message, hresult, false, MapiExceptionHelper.GetLowLevelError(hresult, false, objLastErrorInfo), MapiExceptionHelper.GetDiagnosticContext(objLastErrorInfo), innerException);
			}
		}

		internal static void ThrowIfErrorOrWarning(string message, int hresult, bool allowWarnings)
		{
			if (hresult != 0)
			{
				MapiExceptionHelper.InternalThrowIfErrorOrWarning(message, hresult, allowWarnings, 0, null, null);
			}
		}

		internal static void ThrowIfErrorOrWarning(string message, int hresult, bool allowWarnings, IExInterface iUnknown, Exception innerException)
		{
			if (hresult != 0)
			{
				MapiExceptionHelper.InternalThrowIfErrorOrWarning(message, hresult, allowWarnings, (iUnknown != null) ? MapiExceptionHelper.GetLowLevelError(hresult, false, iUnknown) : 0, (iUnknown != null) ? MapiExceptionHelper.GetDiagnosticContext(iUnknown) : null, innerException);
			}
		}

		internal static void ThrowIfErrorOrWarning(string message, int hresult, bool allowWarnings, object objLastErrorInfo, Exception innerException)
		{
			if (hresult != 0)
			{
				MapiExceptionHelper.InternalThrowIfErrorOrWarning(message, hresult, allowWarnings, (objLastErrorInfo != null) ? MapiExceptionHelper.GetLowLevelError(hresult, false, objLastErrorInfo) : 0, (objLastErrorInfo != null) ? MapiExceptionHelper.GetDiagnosticContext(objLastErrorInfo) : null, innerException);
			}
		}

		internal static void ThrowImportFailureException(string message, Exception inner)
		{
			if (inner is MapiRetryableException)
			{
				throw new MapiExceptionRetryableImportFailure(message, inner);
			}
			throw new MapiExceptionPermanentImportFailure(message, inner);
		}

		internal static MapiExceptionArgument ArgumentException(string argument, string message)
		{
			return new MapiExceptionArgument(argument, message);
		}

		internal static MapiExceptionArgumentNull ArgumentNullException(string argument)
		{
			return new MapiExceptionArgumentNull(argument);
		}

		internal static MapiExceptionArgumentOutOfRange ArgumentOutOfRangeException(string argument, string message)
		{
			return new MapiExceptionArgumentOutOfRange(argument, message);
		}

		internal static MapiExceptionInvalidCast InvalidCastException(string message)
		{
			return new MapiExceptionInvalidCast(message);
		}

		internal static MapiExceptionObjectDisposed ObjectDisposedException(string message)
		{
			return new MapiExceptionObjectDisposed(message);
		}

		public static MapiExceptionOutOfMemory OutOfMemoryException(string message)
		{
			return new MapiExceptionOutOfMemory(message);
		}

		public static MapiExceptionTimeout TimeoutException(string message, Exception innerException)
		{
			return new MapiExceptionTimeout(message, -2147220479, 0, null, innerException);
		}

		public static MapiExceptionCancel CancelException(string message, Exception innerException)
		{
			return new MapiExceptionCancel(message, -2147220223, 0, null, innerException);
		}

		internal static MapiExceptionDataIntegrity DataIntegrityException(string message)
		{
			return new MapiExceptionDataIntegrity(message);
		}

		internal static MapiExceptionNotSupported NotSupportedException(string message)
		{
			return new MapiExceptionNotSupported(message);
		}

		internal static MapiExceptionInvalidOperation InvalidOperationException(string message)
		{
			return new MapiExceptionInvalidOperation(message);
		}

		internal static MapiExceptionInvalidType InvalidTypeException(string message)
		{
			return new MapiExceptionInvalidType(message, -2147220734, 0, null, null);
		}

		internal static MapiExceptionLowLevelInitializationFailure LowLevelInitializationFailureException(string message)
		{
			return new MapiExceptionLowLevelInitializationFailure(message);
		}

		internal static MapiExceptionFileOpenFailure FileOpenFailureException(string message, int hr)
		{
			return new MapiExceptionFileOpenFailure(message, hr);
		}

		internal static Exception NoMoreConnectionsException(string message)
		{
			return new MapiExceptionNoMoreConnections(message);
		}

		internal static MapiExceptionExceededMapiStoreLimit ExceededMapiStoreLimitException(string message)
		{
			return new MapiExceptionExceededMapiStoreLimit(message);
		}

		internal static MapiExceptionObjectReentered ObjectReenteredException(string message)
		{
			return new MapiExceptionObjectReentered(message);
		}

		internal static MapiExceptionObjectNotLocked ObjectNotLockedException(string message)
		{
			return new MapiExceptionObjectNotLocked(message);
		}

		internal static MapiExceptionObjectLockCountOverflow ObjectLockCountOverflowException(string message)
		{
			return new MapiExceptionObjectLockCountOverflow(message);
		}

		internal static MapiExceptionIncompatiblePropType IncompatiblePropTypeException(string message)
		{
			return new MapiExceptionIncompatiblePropType(message);
		}

		internal static MapiExceptionNonCanonicalACL NonCanonicalACLException(string message)
		{
			return new MapiExceptionNonCanonicalACL(message, -2147467259, 2409, null, null);
		}

		internal static MapiExceptionCallFailed CallFailedException(string message)
		{
			return new MapiExceptionCallFailed(message, -2147467259, -2147467259, null, null);
		}

		internal static MapiExceptionWrongServer WrongServerException(string message)
		{
			return new MapiExceptionWrongServer(message, -2147467259, 1144, null, null);
		}

		internal static MapiExceptionIllegalCrossServerConnection IllegalCrossServerConnection(string message)
		{
			return new MapiExceptionIllegalCrossServerConnection(message);
		}

		internal static MapiExceptionInvalidParameter InvalidParameterException(string message)
		{
			return new MapiExceptionInvalidParameter(message, -2147024809, -2147024809, null, null);
		}

		internal static MapiExceptionNotFound NotFoundException(string message)
		{
			return new MapiExceptionNotFound(message, -2147221233, -2147221233, null, null);
		}

		internal static MapiExceptionMailboxInTransit MailboxInTransitException(string message)
		{
			return new MapiExceptionMailboxInTransit(message, -2147467259, 1292, null, null);
		}

		internal static MapiExceptionSessionLimit SessionLimitException(string message)
		{
			return new MapiExceptionSessionLimit(message, -2147221230, -2147221230, null, null);
		}

		internal static MapiExceptionRpcServerTooBusy RpcServerTooBusyException(string message)
		{
			return new MapiExceptionRpcServerTooBusy(message, 2419, 2419, null, null);
		}

		internal static MapiExceptionNotEnoughMemory NotEnoughMemoryException(string message)
		{
			return new MapiExceptionNotEnoughMemory(message, -2147024882, -2147024882, null, null);
		}

		internal static MapiExceptionMaxThreadsPerMdbExceeded MaxThreadsPerMdbExceededException(string message)
		{
			return new MapiExceptionMaxThreadsPerMdbExceeded(message, 2700, 2700, null, null);
		}

		internal static MapiExceptionMaxThreadsPerSCTExceeded MaxThreadsPerSCTExceededException(string message)
		{
			return new MapiExceptionMaxThreadsPerSCTExceeded(message, 2701, 2701, null, null);
		}

		internal static MapiExceptionShutoffQuotaExceeded ShutoffQuotaExceededException(string message)
		{
			return new MapiExceptionShutoffQuotaExceeded(message, -2147467259, 1245, null, null);
		}

		internal static MapiExceptionTooBig TooBigException(string message)
		{
			return new MapiExceptionTooBig(message, -2147220731, -2147220731, null, null);
		}

		internal static MapiExceptionExiting ExitingException(string message)
		{
			return new MapiExceptionExiting(message, -2147467259, 1005, null, null);
		}

		internal static MapiExceptionBackupInProgress BackupInProgressException(string message)
		{
			return new MapiExceptionBackupInProgress(message, -2147467259, 1293, null, null);
		}

		internal static MapiExceptionEndOfSession EndOfSessionException(string message)
		{
			return new MapiExceptionEndOfSession(message, -2147220992, -2147220992, null, null);
		}

		internal static MapiExceptionLogonFailed LogonFailedException(string message)
		{
			return new MapiExceptionLogonFailed(message, -2147221231, -2147221231, null, null);
		}

		internal static MapiExceptionCanNotComplete CanNotCompleteException(string message)
		{
			return new MapiExceptionCanNotComplete(message, -2147023893, -2147023893, null, null);
		}

		internal static MapiExceptionMdbOffline MdbOfflineException(string message)
		{
			return new MapiExceptionMdbOffline(message, -2147467259, 1142, null, null);
		}

		internal static MapiExceptionServerPaused ServerPausedException(string message)
		{
			return new MapiExceptionServerPaused(message, -2147467259, 1151, null, null);
		}

		internal static MapiExceptionUnconfigured UnconfiguredException(string message)
		{
			return new MapiExceptionUnconfigured(message, -2147221220, -2147221220, null, null);
		}

		internal static MapiExceptionUnknownUser UnknownUserException(string message)
		{
			return new MapiExceptionUnknownUser(message, -2147467259, 1003, null, null);
		}

		internal static MapiExceptionConditionViolation ConditionViolationException(string message)
		{
			return new MapiExceptionConditionViolation(message, -2147467259, 2604, null, null);
		}

		internal static MapiExceptionPartialCompletion PartialCompletionException(string message)
		{
			return new MapiExceptionPartialCompletion(message, 263808, 263808, null, null);
		}

		internal static MapiExceptionClientVersionDisallowed ClientVersionDisallowedException(string message)
		{
			return new MapiExceptionClientVersionDisallowed(message, -2147467259, 1247, null, null);
		}

		internal static MapiExceptionTooComplex TooComplexException(string message)
		{
			return new MapiExceptionTooComplex(message, -2147221225, -2147221225, null, null);
		}

		internal static MapiExceptionJetDatabaseCorruption JetDatabaseCorruptionException(string message)
		{
			return new MapiExceptionJetDatabaseCorruption(message, -2147467259, -1206, null, null);
		}

		internal static MapiExceptionJetErrorDiskIO JetErrorDiskIOException(string message)
		{
			return new MapiExceptionJetErrorDiskIO(message, -2147467259, -1022, null, null);
		}

		internal static MapiExceptionTooManyMountedDatabases TooManyMountedDatabasesException(string message)
		{
			return new MapiExceptionTooManyMountedDatabases(message, -2147219954, -2147219954, null, null);
		}

		internal static MapiExceptionNetworkError NetworkErrorException(string message)
		{
			return new MapiExceptionNetworkError(message, -2147221227, -2147221227, null, null);
		}
	}
}
