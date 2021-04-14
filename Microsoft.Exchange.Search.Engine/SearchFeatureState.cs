using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Engine
{
	internal class SearchFeatureState
	{
		internal SearchFeatureState(ISearchServiceConfig config, List<MdbInfo> mdbInfoList, IDiagnosticsSession diagnosticsSession)
		{
			this.config = config;
			this.MdbInfos = mdbInfoList;
			this.diagnosticsSession = diagnosticsSession;
			this.searchMemoryModel = new SearchMemoryModel(config, diagnosticsSession);
			this.searchMemoryModel.SearchMemoryUsage = SearchMemoryModel.GetFreshSearchMemoryUsage();
		}

		internal SearchFeatureState(SearchFeatureState searchFeatureState)
		{
			this.config = searchFeatureState.Config;
			this.MdbInfos = SearchFeatureState.GetCopiedMdbInfoList(searchFeatureState.MdbInfos);
			this.diagnosticsSession = searchFeatureState.DiagnosticsSession;
			this.searchMemoryModel = new SearchMemoryModel(searchFeatureState.SearchMemoryModel);
		}

		internal ISearchServiceConfig Config
		{
			get
			{
				return this.config;
			}
		}

		internal IDiagnosticsSession DiagnosticsSession
		{
			get
			{
				return this.diagnosticsSession;
			}
		}

		internal List<MdbInfo> MdbInfos { get; set; }

		internal SearchMemoryModel SearchMemoryModel
		{
			get
			{
				return this.searchMemoryModel;
			}
		}

		internal static SearchFeatureState GetCurrentState(ISearchServiceConfig config, IDiagnosticsSession diagnosticsSession, IEnumerable<MdbInfo> allMDBs)
		{
			foreach (MdbInfo mdbInfo in allMDBs)
			{
				bool flag;
				if (SearchConfig.GetRegistry(mdbInfo.Guid, "DisableInstantSearch", ref flag))
				{
					mdbInfo.IsInstantSearchEnabled = (mdbInfo.MountedOnLocalServer && !flag);
				}
				if (SearchConfig.GetRegistry(mdbInfo.Guid, "AutoSuspendCatalog", ref flag))
				{
					mdbInfo.IsCatalogSuspended = (!mdbInfo.MountedOnLocalServer && flag);
				}
			}
			return new SearchFeatureState(config, new List<MdbInfo>(allMDBs), diagnosticsSession);
		}

		internal static SearchFeatureState GetAllOnState(ISearchServiceConfig config, IDiagnosticsSession diagnosticsSession, IEnumerable<MdbInfo> allMDBs)
		{
			SearchFeatureState currentState = SearchFeatureState.GetCurrentState(config, diagnosticsSession, allMDBs);
			currentState.SearchMemoryModel.SearchMemoryOperation.MemoryUsageHighLine = long.MaxValue;
			currentState.SearchMemoryModel.SearchMemoryOperation.MemoryUsageLowLine = long.MaxValue;
			return currentState.GetNextState();
		}

		internal SearchFeatureState GetNextState()
		{
			SearchFeatureState searchFeatureState = new SearchFeatureState(this);
			SearchMemoryModel.ActionDirection actionDirection = searchFeatureState.SearchMemoryModel.GetActionDirection();
			if (actionDirection != SearchMemoryModel.ActionDirection.None)
			{
				if (actionDirection == SearchMemoryModel.ActionDirection.Degrade)
				{
					searchFeatureState.MdbInfos.Sort(new Comparison<MdbInfo>(SearchFeatureState.CompareMdbInfoInDescendingAP));
				}
				else
				{
					searchFeatureState.MdbInfos.Sort(new Comparison<MdbInfo>(SearchFeatureState.CompareMdbInfoInAscendingAP));
				}
				int count = SearchFeature.SearchFeatureTypeList.Count;
				for (int i = 0; i < count; i++)
				{
					SearchFeature.SearchFeatureType searchFeatureType = (actionDirection == SearchMemoryModel.ActionDirection.Degrade) ? SearchFeature.SearchFeatureTypeList[i] : SearchFeature.SearchFeatureTypeList[count - i - 1];
					foreach (MdbInfo mdbInfo in searchFeatureState.MdbInfos)
					{
						SearchFeature searchFeature = new SearchFeature(searchFeatureType, searchFeatureState.Config, mdbInfo);
						if (searchFeature.IsFlipAllowed())
						{
							long potentialMemoryChange = searchFeature.GetPotentialMemoryChange();
							if (searchFeatureState.SearchMemoryModel.IsBetter(potentialMemoryChange))
							{
								searchFeatureState.ToggleSettings(mdbInfo, searchFeatureType, potentialMemoryChange);
							}
						}
					}
				}
			}
			return searchFeatureState;
		}

		internal void ToggleSettings(MdbInfo mdbInfo, SearchFeature.SearchFeatureType searchFeatureType, long memoryChange)
		{
			string text;
			bool flag;
			switch (searchFeatureType)
			{
			case SearchFeature.SearchFeatureType.InstantSearch:
				mdbInfo.IsInstantSearchEnabled = !mdbInfo.IsInstantSearchEnabled;
				text = "DisableInstantSearch";
				flag = !mdbInfo.IsInstantSearchEnabled;
				break;
			case SearchFeature.SearchFeatureType.PassiveCatalog:
				mdbInfo.IsCatalogSuspended = !mdbInfo.IsCatalogSuspended;
				text = "AutoSuspendCatalog";
				flag = mdbInfo.IsCatalogSuspended;
				break;
			default:
				throw new ArgumentException("Unexpected search feature type: " + searchFeatureType.ToString() + ". Accepted values: InstantSearch, PassiveCatalog, Refiners.", "searchFeatureType");
			}
			this.diagnosticsSession.TraceDebug<MdbInfo, string, bool>("Database {0} is affected. The affected registry key is {1}. Its value is set to {2}.", mdbInfo, text, flag);
			this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Database {0} is affected. The affected registry key is {1}. Its value is set to {2}.", new object[]
			{
				mdbInfo,
				text,
				flag
			});
			this.searchMemoryModel.ApplyMemoryChange(memoryChange, mdbInfo.ToString());
		}

		internal void WriteCurrentStateToRegistry()
		{
			if (this.MdbInfos.Count > 0)
			{
				List<Guid> list = new List<Guid>(this.MdbInfos.Count);
				foreach (MdbInfo mdbInfo in this.MdbInfos)
				{
					list.Add(mdbInfo.Guid);
					SearchConfig.SetRegistry(mdbInfo.Guid, "DisableInstantSearch", !mdbInfo.IsInstantSearchEnabled);
					SearchConfig.SetRegistry(mdbInfo.Guid, "AutoSuspendCatalog", mdbInfo.IsCatalogSuspended);
					this.diagnosticsSession.TraceDebug<MdbInfo, bool, bool>("Graceful Degradation affected Database {0}. DisableInstantSearch: {1}; AutoSuspendCatalog: {2}.", mdbInfo, !mdbInfo.IsInstantSearchEnabled, mdbInfo.IsCatalogSuspended);
				}
			}
		}

		internal bool Equals(SearchFeatureState searchFeatureState)
		{
			if (searchFeatureState.MdbInfos.Count != this.MdbInfos.Count)
			{
				return false;
			}
			for (int i = 0; i < this.MdbInfos.Count; i++)
			{
				if (this.MdbInfos[i].Guid != searchFeatureState.MdbInfos[i].Guid || this.MdbInfos[i].IsInstantSearchEnabled != searchFeatureState.MdbInfos[i].IsInstantSearchEnabled || this.MdbInfos[i].IsCatalogSuspended != searchFeatureState.MdbInfos[i].IsCatalogSuspended)
				{
					return false;
				}
			}
			return true;
		}

		private static int CompareMdbInfoInDescendingAP(MdbInfo x, MdbInfo y)
		{
			if (x.ActivationPreference == y.ActivationPreference)
			{
				return y.Guid.CompareTo(x.Guid);
			}
			return y.ActivationPreference.CompareTo(x.ActivationPreference);
		}

		private static int CompareMdbInfoInAscendingAP(MdbInfo x, MdbInfo y)
		{
			return SearchFeatureState.CompareMdbInfoInDescendingAP(x, y) * -1;
		}

		private static List<MdbInfo> GetCopiedMdbInfoList(List<MdbInfo> mdbInfoList)
		{
			if (mdbInfoList.Count == 0)
			{
				throw new ArgumentException("mdbInfoList");
			}
			List<MdbInfo> list = new List<MdbInfo>(mdbInfoList.Count);
			foreach (MdbInfo mdbInfo in mdbInfoList)
			{
				list.Add(new MdbInfo(mdbInfo));
			}
			return list;
		}

		private readonly ISearchServiceConfig config;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly SearchMemoryModel searchMemoryModel;
	}
}
