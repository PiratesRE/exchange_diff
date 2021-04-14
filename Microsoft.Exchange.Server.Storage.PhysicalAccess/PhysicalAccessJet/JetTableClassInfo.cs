using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetTableClassInfo
	{
		public JetTableClassInfo(string name, JET_param param, OpenTableGrbit grbit)
		{
			this.name = name;
			this.jetParam = param;
			this.openTableGrbit = grbit;
		}

		public static IDictionary<TableClass, JetTableClassInfo> Classes
		{
			get
			{
				return JetTableClassInfo.classes;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public JET_param JetParam
		{
			get
			{
				return this.jetParam;
			}
		}

		public OpenTableGrbit OpenTableGrbit
		{
			get
			{
				return this.openTableGrbit;
			}
		}

		private static readonly JetTableClassInfo miscClassInfo = new JetTableClassInfo("Misc", (JET_param)143, OpenTableGrbit.TableClass7);

		private static readonly Dictionary<TableClass, JetTableClassInfo> classes = new Dictionary<TableClass, JetTableClassInfo>
		{
			{
				TableClass.LazyIndex,
				new JetTableClassInfo("LazyIndex", (JET_param)137, OpenTableGrbit.TableClass1)
			},
			{
				TableClass.Message,
				new JetTableClassInfo("Message", (JET_param)138, OpenTableGrbit.TableClass2)
			},
			{
				TableClass.Attachment,
				new JetTableClassInfo("Attachment", (JET_param)139, OpenTableGrbit.TableClass3)
			},
			{
				TableClass.Folder,
				new JetTableClassInfo("Folder", (JET_param)140, OpenTableGrbit.TableClass4)
			},
			{
				TableClass.PseudoIndexMaintenance,
				new JetTableClassInfo("PseudoIndexMaintenance", (JET_param)141, OpenTableGrbit.TableClass5)
			},
			{
				TableClass.Events,
				new JetTableClassInfo("Events", (JET_param)142, OpenTableGrbit.TableClass6)
			},
			{
				TableClass.DeliveredTo,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ExtendedPropertyNameMapping,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.Globals,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.InferenceLog,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.Mailbox,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.MailboxIdentity,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.PerUser,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.PseudoIndexControl,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.PseudoIndexDefinition,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ReceiveFolder,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ReceiveFolder2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ReplidGuidMap,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.SearchQueue,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.TimedEvents,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.Tombstone,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.UpgradeHistory,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.Watermarks,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.UserInfo,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ApplyOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.CategorizedTableOperatorSuiteHeader,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.CategorizedTableOperatorSuiteLeaf,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.CategorizedTableOperatorSuiteMessage,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ColumnSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ColumnSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.CommonDataRowSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ConnectionSuiteHelper,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.CountOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.DataRowSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.DeleteOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.IndexAndOperatorSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.IndexAndOperatorSuite3,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.IndexNotOperatorSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.IndexNotOperatorSuite3,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.IndexOrOperatorSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.IndexOrOperatorSuite3,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.InsertOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.InsertOperatorSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.InsertOperatorSuite3,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.JetColumnSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.JetTableSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.JoinOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.JoinOperatorSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.OrdinalPositionOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.OrdinalPositionOperatorSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.Partitioned,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.PreReadOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ReaderSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.ReaderSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.SearchCriteriaSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.SortOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.SqlConnectionSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.TableOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.TableOperatorSuite2,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.TableOperatorSuite3,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.TableOperatorSuite4,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.Unknown,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.UpdateOperatorSuite,
				JetTableClassInfo.miscClassInfo
			},
			{
				TableClass.DistinctOperatorSuite,
				JetTableClassInfo.miscClassInfo
			}
		};

		private readonly string name;

		private readonly OpenTableGrbit openTableGrbit;

		private readonly JET_param jetParam;
	}
}
