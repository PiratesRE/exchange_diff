using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PromptUtils
	{
		internal static ResourceManager PromptResourceManager
		{
			get
			{
				if (PromptUtils.promptResources == null)
				{
					PromptUtils.promptResources = new ResourceManager("Microsoft.Exchange.UM.Prompts.Prompts.Strings", Assembly.Load("Microsoft.Exchange.UM.Prompts"));
				}
				return PromptUtils.promptResources;
			}
		}

		internal static SingleStatementPrompt CreateSingleStatementPrompt(string promptName, CultureInfo culture, params Prompt[] paramPrompt)
		{
			SingleStatementPrompt singleStatementPrompt = new SingleStatementPrompt(paramPrompt);
			PromptSetting config = new PromptSetting(promptName);
			singleStatementPrompt.Initialize(config, culture);
			return singleStatementPrompt;
		}

		internal static List<Prompt> CreateStatementPrompt(string promptName, List<Prompt> parameterPrompts, CultureInfo culture)
		{
			string locString = PromptUtils.GetLocString(promptName, culture);
			StatementParser statementParser = new StatementParser(promptName, culture, locString);
			List<Prompt> list = new List<Prompt>();
			StatementChunk statementChunk;
			while ((statementChunk = statementParser.NextChunk()) != null)
			{
				switch (statementChunk.Type)
				{
				case ChunkType.Text:
					list.Add(new TextPrompt(statementParser.SubPromptFileName, culture, (string)statementChunk.Value));
					break;
				case ChunkType.File:
					list.Add(new FilePrompt(statementParser.SubPromptFileName, culture));
					break;
				case ChunkType.Variable:
					list.Add(parameterPrompts[(int)statementChunk.Value]);
					break;
				}
			}
			return list;
		}

		internal static List<PromptConfigBase> CreateStatementPromptConfig(string promptName, string conditionString, List<PromptConfigBase> parameterPrompts, CultureInfo culture, ActivityManagerConfig managerConfig)
		{
			string locString = PromptUtils.GetLocString(promptName, culture);
			StatementParser statementParser = new StatementParser(promptName, culture, locString);
			List<PromptConfigBase> list = new List<PromptConfigBase>();
			StatementChunk statementChunk;
			while ((statementChunk = statementParser.NextChunk()) != null)
			{
				switch (statementChunk.Type)
				{
				case ChunkType.Text:
					list.Add(new ConstTextPromptConfig(statementParser.SubPromptFileName, (string)statementChunk.Value, conditionString, culture, managerConfig));
					break;
				case ChunkType.File:
					list.Add(new ConstFilePromptConfig(statementParser.SubPromptFileName, conditionString, culture, managerConfig));
					break;
				case ChunkType.Variable:
					list.Add(parameterPrompts[(int)statementChunk.Value]);
					break;
				}
			}
			return list;
		}

		private static string GetLocString(string promptName, CultureInfo culture)
		{
			string @string = PromptUtils.PromptResourceManager.GetString(promptName, culture);
			if (@string == null)
			{
				throw new FsmConfigurationException(Strings.InvalidPromptResourceId(promptName));
			}
			return @string;
		}

		private static ResourceManager promptResources;
	}
}
