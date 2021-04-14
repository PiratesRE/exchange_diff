using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecommendedGroupsAccessor : IRecommendedGroupsAccessor, IRecommendedGroupsGetter
	{
		public RecommendedGroupsAccessor()
		{
			this.modelVersionSelector = new GroupingModelVersionSelector(RecommendedGroupsAccessor.HookableGroupingModelConfiguration.Value);
		}

		public static Hookable<IGroupingModelConfiguration> HookableGroupingModelConfiguration
		{
			get
			{
				return RecommendedGroupsAccessor.hookableGroupingModelConfiguration;
			}
		}

		public RecommendedGroupsInfo ReadItem(Stream stream)
		{
			if (stream.Length <= 0L)
			{
				return null;
			}
			BinaryReader reader = new BinaryReader(stream);
			RecommendedGroupsInfo recommendedGroupsInfo = new RecommendedGroupsInfo();
			recommendedGroupsInfo.Read(reader);
			return recommendedGroupsInfo;
		}

		public void WriteItem(Stream stream, RecommendedGroupsInfo item)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			item.Write(writer);
			stream.SetLength(stream.Position);
		}

		public IReadOnlyList<IRecommendedGroupInfo> GetRecommendedGroups(MailboxSession session, Action<string> traceDelegate, Action<Exception> traceErrorDelegate)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("traceDelegate", traceDelegate);
			ArgumentValidator.ThrowIfNull("traceErrorDelegate", traceErrorDelegate);
			IReadOnlyList<IRecommendedGroupInfo> result = null;
			Exception ex = null;
			string text = string.Format("{0}.{1}", "Inference.RecommendedGroups", this.modelVersionSelector.GetModelVersionToAccessRecommendedGroups());
			try
			{
				traceDelegate(string.Format("Loading recommended groups from {0}", text));
				using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(session, session.GetDefaultFolderId(DefaultFolderType.Inbox), text, UserConfigurationTypes.Stream, false, false))
				{
					if (folderConfiguration != null)
					{
						using (Stream stream = folderConfiguration.GetStream())
						{
							RecommendedGroupsInfo recommendedGroupsInfo = this.ReadItem(stream);
							if (recommendedGroupsInfo != null)
							{
								result = recommendedGroupsInfo.RecommendedGroups;
							}
						}
					}
				}
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (CorruptDataException ex3)
			{
				ex = ex3;
			}
			catch (SerializationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				traceErrorDelegate(ex);
			}
			return result;
		}

		public void SetRecommendedGroups(MailboxSession session, RecommendedGroupsInfo groupsInfo, int version, Action<string> traceDelegate, Action<Exception> traceErrorDelegate)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("traceDelegate", traceDelegate);
			ArgumentValidator.ThrowIfNull("traceErrorDelegate", traceErrorDelegate);
			string text = string.Format("{0}.{1}", "Inference.RecommendedGroups", version);
			try
			{
				traceDelegate(string.Format("Writing out recommended groups to {0}", text));
				using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(session, session.GetDefaultFolderId(DefaultFolderType.Inbox), text, UserConfigurationTypes.Stream, true, false))
				{
					if (folderConfiguration != null)
					{
						using (Stream stream = folderConfiguration.GetStream())
						{
							this.WriteItem(stream, groupsInfo);
							folderConfiguration.Save();
						}
					}
				}
			}
			catch (SerializationException obj)
			{
				traceErrorDelegate(obj);
			}
		}

		private static readonly Hookable<IGroupingModelConfiguration> hookableGroupingModelConfiguration = Hookable<IGroupingModelConfiguration>.Create(true, GroupingModelConfiguration.LoadFromFile().AsReadOnly());

		private readonly GroupingModelVersionSelector modelVersionSelector;
	}
}
