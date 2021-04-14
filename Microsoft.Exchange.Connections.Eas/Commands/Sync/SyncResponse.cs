using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Sync
{
	[XmlRoot(Namespace = "AirSync", ElementName = "Sync")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "SyncResponse")]
	public class SyncResponse : Sync, IEasServerResponse<SyncStatus>, IHaveAnHttpStatus
	{
		public SyncResponse()
		{
			base.Collections = new List<Collection>();
		}

		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		[XmlIgnore]
		public List<FetchResponse> Fetches
		{
			get
			{
				return base.Collections.SelectMany((Collection collection) => collection.Responses.Fetch).ToList<FetchResponse>();
			}
		}

		[XmlIgnore]
		public List<AddResponse> AddResponses
		{
			get
			{
				return base.Collections.SelectMany((Collection collection) => collection.Responses.Add).ToList<AddResponse>();
			}
		}

		[XmlIgnore]
		public List<ChangeResponse> ChangeResponses
		{
			get
			{
				return base.Collections.SelectMany((Collection collection) => collection.Responses.Change).ToList<ChangeResponse>();
			}
		}

		[XmlIgnore]
		public List<AddCommand> Additions
		{
			get
			{
				return base.Collections.SelectMany((Collection collection) => collection.Commands.Add).ToList<AddCommand>();
			}
		}

		[XmlIgnore]
		public List<ChangeCommand> Changes
		{
			get
			{
				return base.Collections.SelectMany((Collection c) => c.Commands.Change).ToList<ChangeCommand>();
			}
		}

		[XmlIgnore]
		public List<DeleteCommand> Deletions
		{
			get
			{
				return base.Collections.SelectMany((Collection c) => c.Commands.Delete).ToList<DeleteCommand>();
			}
		}

		public List<SoftDeleteCommand> GetAllSoftDeletions
		{
			get
			{
				return base.Collections.SelectMany((Collection c) => c.Commands.SoftDelete).ToList<SoftDeleteCommand>();
			}
		}

		public SyncStatus GetChangeResponseStatus(int index)
		{
			if (this.ChangeResponses == null || this.ChangeResponses.Count <= index || this.ChangeResponses[index] == null)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			ChangeResponse changeResponse = this.ChangeResponses[index];
			byte status = changeResponse.Status;
			if (!SyncResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (SyncStatus)status;
			}
			return SyncResponse.StatusToEnumMap[status];
		}

		public SyncStatus GetAddResponseStatus(int index)
		{
			if (this.AddResponses == null || this.AddResponses.Count <= index || this.AddResponses[index] == null)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			AddResponse addResponse = this.AddResponses[index];
			byte status = addResponse.Status;
			if (!SyncResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (SyncStatus)status;
			}
			return SyncResponse.StatusToEnumMap[status];
		}

		bool IEasServerResponse<SyncStatus>.IsSucceeded(SyncStatus status)
		{
			return SyncStatus.Success == status;
		}

		SyncStatus IEasServerResponse<SyncStatus>.ConvertStatusToEnum()
		{
			if (base.Collections.Count > 1)
			{
				IEnumerable<byte> source = from c in base.Collections
				where c.Status != 1
				select c.Status;
				if (source.Count<byte>() == 0)
				{
					return SyncStatus.Success;
				}
				return SyncStatus.CompositeStatusError;
			}
			else
			{
				if (base.Collections.Count == 0 && base.Status == 0)
				{
					return SyncStatus.Success;
				}
				byte b = (base.Collections.Count == 0) ? base.Status : base.Collections[0].Status;
				if (!SyncResponse.StatusToEnumMap.ContainsKey(b))
				{
					return (SyncStatus)b;
				}
				return SyncResponse.StatusToEnumMap[b];
			}
		}

		private static readonly IReadOnlyDictionary<byte, SyncStatus> StatusToEnumMap = new Dictionary<byte, SyncStatus>
		{
			{
				1,
				SyncStatus.Success
			},
			{
				3,
				SyncStatus.InvalidSyncKey
			},
			{
				4,
				SyncStatus.ProtocolError
			},
			{
				5,
				SyncStatus.ServerError
			},
			{
				6,
				SyncStatus.ErrorInClientServerConversion
			},
			{
				7,
				SyncStatus.Conflict
			},
			{
				8,
				SyncStatus.SyncItemNotFound
			},
			{
				9,
				SyncStatus.OutOfDisk
			},
			{
				12,
				SyncStatus.FolderHierarchyChanged
			},
			{
				13,
				SyncStatus.IncompleteSyncCommand
			},
			{
				14,
				SyncStatus.InvalidWaitTime
			},
			{
				15,
				SyncStatus.SyncTooManyFolders
			},
			{
				16,
				SyncStatus.Retry
			},
			{
				110,
				SyncStatus.ServerBusy
			}
		};
	}
}
