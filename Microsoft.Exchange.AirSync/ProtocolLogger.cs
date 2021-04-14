using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class ProtocolLogger
	{
		public ProtocolLogger()
		{
			this.storedData = new Hashtable();
			this.storedFolders = new Dictionary<string, Hashtable>();
		}

		public void RecordData(IAirSyncResponse response)
		{
			response.AppendToLog(this.ToString());
		}

		public void SetValue(ProtocolLoggerData dataType, string value)
		{
			if (dataType == ProtocolLoggerData.DomainController && string.IsNullOrEmpty(value))
			{
				return;
			}
			this.storedData[dataType] = value;
		}

		public void AppendValue(ProtocolLoggerData dataType, string value)
		{
			if (dataType == ProtocolLoggerData.DomainController && string.IsNullOrEmpty(value))
			{
				return;
			}
			if (this.storedData.ContainsKey(dataType) && this.storedData[dataType] != null)
			{
				this.storedData[dataType] = (string)this.storedData[dataType] + ";" + value;
				return;
			}
			this.SetValue(dataType, value);
		}

		public void SetTrimmedValue(ProtocolLoggerData dataType, string value, int maxLength)
		{
			if (dataType == ProtocolLoggerData.DomainController && string.IsNullOrEmpty(value))
			{
				return;
			}
			if (value != null && value.Length > maxLength)
			{
				value = value.Remove(maxLength);
			}
			this.storedData[dataType] = value;
		}

		public void SetValue(ProtocolLoggerData dataType, int value)
		{
			this.storedData[dataType] = value;
		}

		public void SetValue(ProtocolLoggerData dataType, long value)
		{
			this.storedData[dataType] = value;
		}

		public void SetValue(ProtocolLoggerData dataType, uint value)
		{
			this.storedData[dataType] = value;
		}

		public void SetValue(ProtocolLoggerData dataType, ExDateTime value)
		{
			this.storedData[dataType] = value;
		}

		public void SetValueIfNotSet(ProtocolLoggerData dataType, string value)
		{
			if (this.storedData[dataType] == null)
			{
				this.storedData[dataType] = value;
			}
		}

		public void SetValueIfNotSet(ProtocolLoggerData dataType, int value)
		{
			if (this.storedData[dataType] == null)
			{
				this.storedData[dataType] = value;
			}
		}

		public void SetValueIfNotSet(ProtocolLoggerData dataType, uint value)
		{
			if (this.storedData[dataType] == null)
			{
				this.storedData[dataType] = value;
			}
		}

		public void SetValueIfNotSet(ProtocolLoggerData dataType, ExDateTime value)
		{
			if (this.storedData[dataType] == null)
			{
				this.storedData[dataType] = value;
			}
		}

		public int GetIntegerValue(ProtocolLoggerData dataType)
		{
			object obj = this.storedData[dataType];
			if (obj == null)
			{
				return 0;
			}
			if (obj is int)
			{
				return (int)obj;
			}
			throw new ArgumentException("The dataType is not an integer");
		}

		public bool TryGetValue<T>(ProtocolLoggerData key, out T data)
		{
			object obj = this.storedData[key];
			if (obj != null && obj is T)
			{
				data = (T)((object)obj);
				return true;
			}
			data = default(T);
			return false;
		}

		public void IncrementValue(ProtocolLoggerData dataType)
		{
			this.IncrementValueBy(dataType, 1);
		}

		public void IncrementValueBy(ProtocolLoggerData dataType, int incBy)
		{
			if (incBy == 0)
			{
				return;
			}
			object obj = this.storedData[dataType];
			if (obj == null)
			{
				this.storedData[dataType] = incBy;
				return;
			}
			if (obj is int)
			{
				this.storedData[dataType] = (int)obj + incBy;
				return;
			}
			throw new ArgumentException("The dataType is not an integer");
		}

		public void SetValue(string folderId, PerFolderProtocolLoggerData dataType, int value)
		{
			this.InternalSetValue(folderId, dataType, value);
		}

		public void SetProviderSyncType(string folderId, ProviderSyncType providerSyncType)
		{
			if (folderId == null)
			{
				folderId = "null";
			}
			Hashtable hashtable;
			if (!this.storedFolders.TryGetValue(folderId, out hashtable))
			{
				hashtable = new Hashtable();
				this.storedFolders[folderId] = hashtable;
			}
			if (providerSyncType == ProviderSyncType.None)
			{
				hashtable.Remove(PerFolderProtocolLoggerData.ProviderSyncType);
				return;
			}
			ProviderSyncType providerSyncType2 = ProviderSyncType.None;
			object obj = hashtable[PerFolderProtocolLoggerData.ProviderSyncType];
			if (obj != null)
			{
				providerSyncType2 = (ProviderSyncType)obj;
			}
			hashtable[PerFolderProtocolLoggerData.ProviderSyncType] = (providerSyncType2 | providerSyncType);
		}

		public void SetValue(string folderId, PerFolderProtocolLoggerData dataType, uint value)
		{
			this.InternalSetValue(folderId, dataType, value);
		}

		public void SetValue(string folderId, PerFolderProtocolLoggerData dataType, string value)
		{
			this.InternalSetValue(folderId, dataType, value);
		}

		public int GetIntegerValue(string folderId, PerFolderProtocolLoggerData dataType)
		{
			if (folderId == null)
			{
				folderId = "null";
			}
			Hashtable hashtable;
			if (!this.storedFolders.TryGetValue(folderId, out hashtable))
			{
				hashtable = new Hashtable();
				this.storedFolders[folderId] = hashtable;
			}
			object obj = hashtable[dataType];
			if (obj == null)
			{
				return 0;
			}
			if (obj is int)
			{
				return (int)obj;
			}
			throw new ArgumentException("The dataType is not an integer");
		}

		public void IncrementValue(string folderId, PerFolderProtocolLoggerData dataType)
		{
			this.IncrementValueBy(folderId, dataType, 1);
		}

		public void IncrementValueBy(string folderId, PerFolderProtocolLoggerData dataType, int incBy)
		{
			if (folderId == null)
			{
				folderId = "null";
			}
			Hashtable hashtable;
			if (!this.storedFolders.TryGetValue(folderId, out hashtable))
			{
				hashtable = new Hashtable();
				this.storedFolders[folderId] = hashtable;
			}
			if (incBy == 0)
			{
				return;
			}
			object obj = hashtable[dataType];
			if (obj == null)
			{
				hashtable[dataType] = incBy;
				return;
			}
			if (obj is int)
			{
				hashtable[dataType] = (int)obj + incBy;
				return;
			}
			throw new ArgumentException("The dataType is not an integer");
		}

		public int GetTotalClientChangesCount()
		{
			int num = 0;
			foreach (KeyValuePair<string, Hashtable> keyValuePair in this.storedFolders)
			{
				object obj = keyValuePair.Value[PerFolderProtocolLoggerData.ClientAdds];
				object obj2 = keyValuePair.Value[PerFolderProtocolLoggerData.ClientChanges];
				object obj3 = keyValuePair.Value[PerFolderProtocolLoggerData.ClientDeletes];
				num += this.ConvertObjectToInt(obj) + this.ConvertObjectToInt(obj2) + this.ConvertObjectToInt(obj3);
			}
			return num;
		}

		private int ConvertObjectToInt(object obj)
		{
			if (obj is int)
			{
				return (int)obj;
			}
			return 0;
		}

		private object GetIntegerValueAsObject(string folderId, PerFolderProtocolLoggerData dataType)
		{
			Hashtable hashtable;
			if (!this.storedFolders.TryGetValue(folderId, out hashtable))
			{
				return ProtocolLogger.PreBoxedZero;
			}
			object obj = hashtable[dataType];
			if (obj == null)
			{
				return ProtocolLogger.PreBoxedZero;
			}
			return obj;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("&Log=", 200);
			ProtocolLogger.AppendToLog(stringBuilder, "PrxTo", this.storedData[ProtocolLoggerData.ProxyingTo]);
			ProtocolLogger.AppendToLog(stringBuilder, "PrxFrom", this.storedData[ProtocolLoggerData.ProxyingFrom]);
			ProtocolLogger.AppendToLog(stringBuilder, "RdirTo", this.storedData[ProtocolLoggerData.RedirectTo]);
			ProtocolLogger.AppendToLog(stringBuilder, "PrxUser", this.storedData[ProtocolLoggerData.ProxyUser]);
			ProtocolLogger.AppendToLog(stringBuilder, "V", this.storedData[ProtocolLoggerData.ProtocolVersion]);
			ProtocolLogger.AppendToLog(stringBuilder, "HH", this.storedData[ProtocolLoggerData.Host]);
			ProtocolLogger.AppendToLog(stringBuilder, "OlkExtHdr", this.storedData[ProtocolLoggerData.OutlookExtensionsHeader]);
			ProtocolLogger.AppendToLog(stringBuilder, "SmtpAdrs", this.storedData[ProtocolLoggerData.UserSmtpAddress]);
			ProtocolLogger.AppendToLog(stringBuilder, "Puid", this.storedData[ProtocolLoggerData.PUID]);
			ProtocolLogger.AppendToLog(stringBuilder, "Oid", this.storedData[ProtocolLoggerData.OrganizationId]);
			ProtocolLogger.AppendToLog(stringBuilder, "OrgType", this.storedData[ProtocolLoggerData.OrganizationType]);
			ProtocolLogger.AppendToLog(stringBuilder, "DRmv", this.storedData[ProtocolLoggerData.NoOfDevicesRemoved]);
			ProtocolLogger.AppendToLog(stringBuilder, "NMS", this.storedData[ProtocolLoggerData.NewMailboxSession]);
			ProtocolLogger.AppendToLog(stringBuilder, "Ssnf", this.storedData[ProtocolLoggerData.SyncStateNotFound]);
			ProtocolLogger.AppendToLog(stringBuilder, "Fc", this.storedData[ProtocolLoggerData.TotalFolders]);
			if (this.storedFolders.Count <= GlobalSettings.MaxCollectionsToLog)
			{
				foreach (KeyValuePair<string, Hashtable> keyValuePair in this.storedFolders)
				{
					string key = keyValuePair.Key;
					Hashtable value = keyValuePair.Value;
					ProtocolLogger.AppendToLog(stringBuilder, "Fid", value[PerFolderProtocolLoggerData.FolderId]);
					ProtocolLogger.AppendToLog(stringBuilder, "Ty", value[PerFolderProtocolLoggerData.FolderDataType]);
					ProtocolLogger.AppendToLog(stringBuilder, "Filt", value[PerFolderProtocolLoggerData.FilterType]);
					ProtocolLogger.AppendToLog(stringBuilder, "Filts", value[PerFolderProtocolLoggerData.SmsFilterType]);
					ProtocolLogger.AppendToLog(stringBuilder, "St", value[PerFolderProtocolLoggerData.SyncType]);
					ProtocolLogger.AppendToLog(stringBuilder, "Sk", value[PerFolderProtocolLoggerData.ClientSyncKey]);
					ProtocolLogger.AppendToLog(stringBuilder, "Sks", value[PerFolderProtocolLoggerData.ServerSyncKey]);
					ProtocolLogger.AppendToLog(stringBuilder, "Sst", value[PerFolderProtocolLoggerData.SyncStateKb]);
					ProtocolLogger.AppendToLog(stringBuilder, "Sslc", value[PerFolderProtocolLoggerData.SyncStateKbLeftCompressed]);
					ProtocolLogger.AppendToLog(stringBuilder, "SsCmt", value[PerFolderProtocolLoggerData.SyncStateKbCommitted]);
					ProtocolLogger.AppendToLog(stringBuilder, "TotSvC", value[PerFolderProtocolLoggerData.TotalSaveCount]);
					ProtocolLogger.AppendToLog(stringBuilder, "ColdSvC", value[PerFolderProtocolLoggerData.ColdSaveCount]);
					ProtocolLogger.AppendToLog(stringBuilder, "ColdCpyC", value[PerFolderProtocolLoggerData.ColdCopyCount]);
					ProtocolLogger.AppendToLog(stringBuilder, "TotLdC", value[PerFolderProtocolLoggerData.TotalLoadCount]);
					ProtocolLogger.AppendToLog(stringBuilder, "MR", value[PerFolderProtocolLoggerData.MidnightRollover]);
					ProtocolLogger.AppendToLog(stringBuilder, "FtsR", value[PerFolderProtocolLoggerData.FirstTimeSyncItemsDiscarded]);
					ProtocolLogger.AppendToLog(stringBuilder, "ProvSyncType", value[PerFolderProtocolLoggerData.ProviderSyncType]);
					ProtocolLogger.AppendToLog(stringBuilder, "GetChgsIter", value[PerFolderProtocolLoggerData.GetChangesIterations]);
					ProtocolLogger.AppendToLog(stringBuilder, "GetChgsTime", value[PerFolderProtocolLoggerData.GetChangesTime]);
					if (this.GetIntegerValue(key, PerFolderProtocolLoggerData.ClientAdds) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ClientChanges) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ClientDeletes) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ClientFetches) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ClientFailedToConvert) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ClientSends) != 0)
					{
						string objectToLog = string.Format(CultureInfo.InvariantCulture, "{0}a{1}c{2}d{3}f{4}e{5}s{6}fs", new object[]
						{
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ClientAdds),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ClientChanges),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ClientDeletes),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ClientFetches),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ClientFailedToConvert),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ClientSends),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ClientFailedToSend)
						});
						ProtocolLogger.AppendToLog(stringBuilder, "Cli", objectToLog);
					}
					if (this.GetIntegerValue(key, PerFolderProtocolLoggerData.ServerAdds) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ServerChanges) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ServerDeletes) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ServerSoftDeletes) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ServerFailedToConvert) != 0 || this.GetIntegerValue(key, PerFolderProtocolLoggerData.ServerAssociatedAdds) != 0)
					{
						string objectToLog2 = string.Format(CultureInfo.InvariantCulture, "{0}a{1}c{2}d{3}s{4}e{5}r{6}A{7}sd", new object[]
						{
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ServerAdds),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ServerChanges),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ServerDeletes),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ServerSoftDeletes),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ServerFailedToConvert),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ServerChangeTrackingRejected),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.ServerAssociatedAdds),
							this.GetIntegerValueAsObject(key, PerFolderProtocolLoggerData.SkippedDeletes)
						});
						ProtocolLogger.AppendToLog(stringBuilder, "Srv", objectToLog2);
					}
					ProtocolLogger.AppendToLog(stringBuilder, "Pfs", value[PerFolderProtocolLoggerData.PerFolderStatus]);
					ProtocolLogger.AppendToLog(stringBuilder, "BR", value[PerFolderProtocolLoggerData.BodyRequested]);
					ProtocolLogger.AppendToLog(stringBuilder, "BPR", value[PerFolderProtocolLoggerData.BodyPartRequested]);
				}
			}
			ProtocolLogger.AppendToLog(stringBuilder, "E", this.storedData[ProtocolLoggerData.NumErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "Io", this.storedData[ProtocolLoggerData.NumItemsOpened]);
			ProtocolLogger.AppendToLog(stringBuilder, "Hb", this.storedData[ProtocolLoggerData.HeartBeatInterval]);
			ProtocolLogger.AppendToLog(stringBuilder, "Rto", this.storedData[ProtocolLoggerData.RequestTimedOut]);
			ProtocolLogger.AppendToLog(stringBuilder, "Hang", this.storedData[ProtocolLoggerData.RequestHangTime]);
			ProtocolLogger.AppendToLog(stringBuilder, "Erq", this.storedData[ProtocolLoggerData.EmptyRequest]);
			ProtocolLogger.AppendToLog(stringBuilder, "Ers", this.storedData[ProtocolLoggerData.EmptyResponse]);
			ProtocolLogger.AppendToLog(stringBuilder, "Cpo", this.storedData[ProtocolLoggerData.CompletionOffset]);
			ProtocolLogger.AppendToLog(stringBuilder, "Fet", this.storedData[ProtocolLoggerData.FinalElapsedTime]);
			ProtocolLogger.AppendToLog(stringBuilder, "ExStk", this.storedData[ProtocolLoggerData.ExceptionStackTrace]);
			ProtocolLogger.AppendToLog(stringBuilder, "Ssp", this.storedData[ProtocolLoggerData.SharePointDocs]);
			ProtocolLogger.AppendToLog(stringBuilder, "Sspb", this.storedData[ProtocolLoggerData.SharePointBytes]);
			ProtocolLogger.AppendToLog(stringBuilder, "Unc", this.storedData[ProtocolLoggerData.UNCFiles]);
			ProtocolLogger.AppendToLog(stringBuilder, "Uncb", this.storedData[ProtocolLoggerData.UNCBytes]);
			ProtocolLogger.AppendToLog(stringBuilder, "Att", this.storedData[ProtocolLoggerData.Attachments]);
			ProtocolLogger.AppendToLog(stringBuilder, "Attb", this.storedData[ProtocolLoggerData.AttachmentBytes]);
			ProtocolLogger.AppendToLog(stringBuilder, "Pk", this.storedData[ProtocolLoggerData.PolicyKeyReceived]);
			ProtocolLogger.AppendToLog(stringBuilder, "Pa", this.storedData[ProtocolLoggerData.PolicyAckStatus]);
			ProtocolLogger.AppendToLog(stringBuilder, "Oof", this.storedData[ProtocolLoggerData.OOFVerb]);
			ProtocolLogger.AppendToLog(stringBuilder, "UserInfo", this.storedData[ProtocolLoggerData.UserInformationVerb]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevModel", this.storedData[ProtocolLoggerData.DeviceInfoModel]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevIMEI", this.storedData[ProtocolLoggerData.DeviceInfoIMEI]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevName", this.storedData[ProtocolLoggerData.DeviceInfoFriendlyName]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevOS", this.storedData[ProtocolLoggerData.DeviceInfoOS]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevLang", this.storedData[ProtocolLoggerData.DeviceInfoOSLanguage]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevAgent", this.storedData[ProtocolLoggerData.DeviceInfoUserAgent]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevEnaSMS", this.storedData[ProtocolLoggerData.DeviceInfoEnableOutboundSMS]);
			ProtocolLogger.AppendToLog(stringBuilder, "DevMoOp", this.storedData[ProtocolLoggerData.DeviceInfoMobileOperator]);
			ProtocolLogger.AppendToLog(stringBuilder, "S", this.storedData[ProtocolLoggerData.StatusCode]);
			ProtocolLogger.AppendToLog(stringBuilder, "Error", this.storedData[ProtocolLoggerData.Error]);
			ProtocolLogger.AppendToLog(stringBuilder, "Msg", this.storedData[ProtocolLoggerData.Message]);
			ProtocolLogger.AppendToLog(stringBuilder, "ADWR", this.storedData[ProtocolLoggerData.ADWriteReason]);
			ProtocolLogger.AppendToLog(stringBuilder, "RR", this.storedData[ProtocolLoggerData.NumberOfRecipientsToResolve]);
			ProtocolLogger.AppendToLog(stringBuilder, "Fb", this.storedData[ProtocolLoggerData.AvailabilityRequested]);
			ProtocolLogger.AppendToLog(stringBuilder, "Ct", this.storedData[ProtocolLoggerData.CertificatesRequested]);
			ProtocolLogger.AppendToLog(stringBuilder, "Pic", this.storedData[ProtocolLoggerData.PictureRequested]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOefc", this.storedData[ProtocolLoggerData.IOEmptyFolderContents]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOeefc", this.storedData[ProtocolLoggerData.IOEmptyFolderContentsErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOfd", this.storedData[ProtocolLoggerData.IOFetchDocs]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOefd", this.storedData[ProtocolLoggerData.IOFetchDocErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOfi", this.storedData[ProtocolLoggerData.IOFetchItems]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOefi", this.storedData[ProtocolLoggerData.IOFetchItemErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOfa", this.storedData[ProtocolLoggerData.IOFetchAtts]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOfea", this.storedData[ProtocolLoggerData.IOFetchEntAtts]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOefa", this.storedData[ProtocolLoggerData.IOFetchAttErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOm", this.storedData[ProtocolLoggerData.IOMoves]);
			ProtocolLogger.AppendToLog(stringBuilder, "ItOem", this.storedData[ProtocolLoggerData.IOMoveErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "MRi", this.storedData[ProtocolLoggerData.MRItems]);
			ProtocolLogger.AppendToLog(stringBuilder, "MRe", this.storedData[ProtocolLoggerData.MRErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "Mi", this.storedData[ProtocolLoggerData.MItems]);
			ProtocolLogger.AppendToLog(stringBuilder, "MeI", this.storedData[ProtocolLoggerData.MIErrors]);
			ProtocolLogger.AppendToLog(stringBuilder, "SrchP", this.storedData[ProtocolLoggerData.SearchProvider]);
			ProtocolLogger.AppendToLog(stringBuilder, "SrchD", this.storedData[ProtocolLoggerData.SearchDeep]);
			ProtocolLogger.AppendToLog(stringBuilder, "SrchL", this.storedData[ProtocolLoggerData.SearchQueryLength]);
			ProtocolLogger.AppendToLog(stringBuilder, "SrchTime", this.storedData[ProtocolLoggerData.SearchQueryTime]);
			ProtocolLogger.AppendToLog(stringBuilder, "Tpr", this.storedData[ProtocolLoggerData.TotalPhotoRequests]);
			ProtocolLogger.AppendToLog(stringBuilder, "Spr", this.storedData[ProtocolLoggerData.SuccessfulPhotoRequests]);
			ProtocolLogger.AppendToLog(stringBuilder, "Pfc", this.storedData[ProtocolLoggerData.PhotosFromCache]);
			ProtocolLogger.AppendToLog(stringBuilder, "VCh", this.storedData[ProtocolLoggerData.VCertChains]);
			ProtocolLogger.AppendToLog(stringBuilder, "VCe", this.storedData[ProtocolLoggerData.VCerts]);
			ProtocolLogger.AppendToLog(stringBuilder, "VCRL", this.storedData[ProtocolLoggerData.VCertCRL]);
			ProtocolLogger.AppendToLog(stringBuilder, "ClsName", this.storedData[ProtocolLoggerData.ClassName]);
			ProtocolLogger.AppendToLog(stringBuilder, "Uuhp", this.storedData[ProtocolLoggerData.UpdateUserHasPartnerships]);
			ProtocolLogger.AppendToLog(stringBuilder, "SkipSend", this.storedData[ProtocolLoggerData.SkipSend]);
			ProtocolLogger.AppendToLog(stringBuilder, "MOLk", this.storedData[ProtocolLoggerData.MeetingOrganizerLookup]);
			ProtocolLogger.AppendToLog(stringBuilder, "Em", this.storedData[ProtocolLoggerData.ExternallyManaged]);
			ProtocolLogger.AppendToLog(stringBuilder, "GaC", this.storedData[ProtocolLoggerData.GraphApiCallData]);
			ProtocolLogger.AppendToLog(stringBuilder, "As", this.storedData[ProtocolLoggerData.AccessStateAndReason]);
			ProtocolLogger.AppendToLog(stringBuilder, "Ms", this.storedData[ProtocolLoggerData.MailSent]);
			ProtocolLogger.AppendToLog(stringBuilder, "Ssu", this.storedData[ProtocolLoggerData.Ssu]);
			ProtocolLogger.AppendToLog(stringBuilder, "Mbx", this.storedData[ProtocolLoggerData.MailboxServer]);
			ProtocolLogger.AppendToLog(stringBuilder, "SNSSN", this.storedData[ProtocolLoggerData.SNSServiceServerName]);
			ProtocolLogger.AppendToLog(stringBuilder, "Dc", this.storedData[ProtocolLoggerData.DomainController]);
			ProtocolLogger.AppendToLog(stringBuilder, "Throttle", this.storedData[ProtocolLoggerData.ThrottledTime]);
			ProtocolLogger.AppendToLog(stringBuilder, "SBkOffD", this.storedData[ProtocolLoggerData.SuggestedBackOffValue]);
			ProtocolLogger.AppendToLog(stringBuilder, "BkOffRsn", this.storedData[ProtocolLoggerData.BackOffReason]);
			ProtocolLogger.AppendToLog(stringBuilder, "DBL", this.storedData[ProtocolLoggerData.DeviceBehaviorLoaded]);
			ProtocolLogger.AppendToLog(stringBuilder, "DBS", this.storedData[ProtocolLoggerData.DeviceBehaviorSaved]);
			ProtocolLogger.AppendToLog(stringBuilder, "CmdHC", this.storedData[ProtocolLoggerData.CommandHashCode]);
			ProtocolLogger.AppendToLog(stringBuilder, "SyncHC", this.storedData[ProtocolLoggerData.SyncHashCode]);
			ProtocolLogger.AppendToLog(stringBuilder, "Ab", this.storedData[ProtocolLoggerData.AutoBlockEvent]);
			ProtocolLogger.AppendToLog(stringBuilder, "Erd", this.storedData[ProtocolLoggerData.EmptyResponseDelayed]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmRcv", this.storedData[ProtocolLoggerData.TimeReceived]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmSt", this.storedData[ProtocolLoggerData.TimeStarted]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmDASt", this.storedData[ProtocolLoggerData.TimeDeviceAccessCheckStarted]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmPolSt", this.storedData[ProtocolLoggerData.TimePolicyCheckStarted]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmExSt", this.storedData[ProtocolLoggerData.TimeExecuteStarted]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmExFin", this.storedData[ProtocolLoggerData.TimeExecuteFinished]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmFin", this.storedData[ProtocolLoggerData.TimeFinished]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmCmpl", this.storedData[ProtocolLoggerData.TimeCompleted]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmHang", this.storedData[ProtocolLoggerData.TimeHang]);
			ProtocolLogger.AppendToLog(stringBuilder, "TmCnt", this.storedData[ProtocolLoggerData.TimeContinued]);
			ProtocolLogger.AppendToLog(stringBuilder, "NmDeferred", this.storedData[ProtocolLoggerData.NMDeferred]);
			ProtocolLogger.AppendToLog(stringBuilder, "IcsHier", this.storedData[ProtocolLoggerData.QuickHierarchyChangeCheck]);
			ProtocolLogger.AppendToLog(stringBuilder, "BlCARName", this.storedData[ProtocolLoggerData.BlockingClientAccessRuleName]);
			ProtocolLogger.AppendToLog(stringBuilder, "LatCAR", this.storedData[ProtocolLoggerData.ClientAccessRulesLatency]);
			ProtocolLogger.AppendToLog(stringBuilder, "ActivityContextData", this.storedData[ProtocolLoggerData.ActivityContextData]);
			ProtocolLogger.AppendToLog(stringBuilder, "Budget", this.storedData[ProtocolLoggerData.Budget]);
			return stringBuilder.ToString();
		}

		private static void AppendToLog(StringBuilder logString, string prefix, object objectToLog)
		{
			if (objectToLog == null)
			{
				return;
			}
			ProtocolLogger.AppendToLogInternal(logString, prefix, objectToLog);
		}

		private static void AppendToLogInternal(StringBuilder logString, string prefix, object objectToLog)
		{
			logString.Append(prefix);
			if (objectToLog != null)
			{
				string text = objectToLog as string;
				if (text != null)
				{
					logString.Append(':');
					string text2 = HttpUtility.UrlEncode(text);
					text2 = text2.Replace("_", "%5F");
					logString.Append(text2);
				}
				else if (objectToLog is ExDateTime)
				{
					logString.Append(((ExDateTime)objectToLog).ToString("HH:mm:ss.FFFFFFF"));
				}
				else if (objectToLog is int || objectToLog is uint || objectToLog is long)
				{
					logString.Append(objectToLog.ToString());
				}
				else if (objectToLog is ProviderSyncType)
				{
					logString.Append(objectToLog.ToString().Replace(", ", "-"));
				}
				else
				{
					AirSyncDiagnostics.Assert(false, "Cannot log objects of this type", new object[0]);
				}
			}
			logString.Append('_');
		}

		private static void AppendToLog(StringBuilder logString, string flag)
		{
			ProtocolLogger.AppendToLogInternal(logString, flag, null);
		}

		private void InternalSetValue(string folderId, PerFolderProtocolLoggerData dataType, object value)
		{
			if (folderId == null)
			{
				folderId = "null";
			}
			if (value == null)
			{
				return;
			}
			Hashtable hashtable;
			if (!this.storedFolders.TryGetValue(folderId, out hashtable))
			{
				hashtable = new Hashtable();
				this.storedFolders[folderId] = hashtable;
			}
			hashtable[dataType] = value;
		}

		public const string FolderCommands = "F";

		public const string SearchCommand = "S";

		private static readonly object PreBoxedZero = 0;

		private Hashtable storedData;

		private Dictionary<string, Hashtable> storedFolders;
	}
}
