using System;
using System.Configuration;
using Microsoft.Exchange.Transport.Extensibility;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal class AgentErrorHandlingOverrideSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true, IsRequired = true)]
		public AgentErrorHandlingOverrideSection.OverrideList Overrides
		{
			get
			{
				return (AgentErrorHandlingOverrideSection.OverrideList)base[""];
			}
		}

		public class Override : ConfigurationElement
		{
			[ConfigurationProperty("name", IsRequired = true)]
			public string Name
			{
				get
				{
					return (string)base["name"];
				}
			}

			[ConfigurationProperty("condition", IsRequired = true)]
			public AgentErrorHandlingOverrideSection.Condition Condition
			{
				get
				{
					return (AgentErrorHandlingOverrideSection.Condition)base["condition"];
				}
			}

			[ConfigurationProperty("action", IsRequired = true)]
			public AgentErrorHandlingOverrideSection.Action Action
			{
				get
				{
					return (AgentErrorHandlingOverrideSection.Action)base["action"];
				}
			}
		}

		internal class Condition : ConfigurationElement
		{
			[ConfigurationProperty("contextId", IsRequired = true)]
			public string ContextId
			{
				get
				{
					return (string)base["contextId"];
				}
			}

			[ConfigurationProperty("exceptionType")]
			public string ExceptionType
			{
				get
				{
					return (string)base["exceptionType"];
				}
			}

			[ConfigurationProperty("exceptionMessage")]
			public string ExceptionMessage
			{
				get
				{
					return (string)base["exceptionMessage"];
				}
			}

			[ConfigurationProperty("deferCount", DefaultValue = 0)]
			public int DeferCount
			{
				get
				{
					return (int)base["deferCount"];
				}
			}
		}

		internal class Action : ConfigurationElement
		{
			[ConfigurationProperty("type", IsRequired = true)]
			public ErrorHandlingActionType ActionType
			{
				get
				{
					return (ErrorHandlingActionType)base["type"];
				}
			}

			[ConfigurationProperty("message")]
			public string NDRMessage
			{
				get
				{
					return (string)base["message"];
				}
			}

			[ConfigurationProperty("statusCode")]
			public string NDRStatusCode
			{
				get
				{
					return (string)base["statusCode"];
				}
			}

			[ConfigurationProperty("enhancedStatusCode")]
			public string NDREnhancedStatusCode
			{
				get
				{
					return (string)base["enhancedStatusCode"];
				}
			}

			[ConfigurationProperty("statusText")]
			public string NDRStatusText
			{
				get
				{
					return (string)base["statusText"];
				}
			}

			[ConfigurationProperty("interval")]
			public TimeSpan DeferInterval
			{
				get
				{
					return (TimeSpan)base["interval"];
				}
			}

			[ConfigurationProperty("isIntervalProgressive", DefaultValue = false)]
			public bool IsIntervalProgressive
			{
				get
				{
					return (bool)base["isIntervalProgressive"];
				}
			}
		}

		internal class OverrideList : ConfigurationElementCollection
		{
			public AgentErrorHandlingOverrideSection.Override this[int index]
			{
				get
				{
					return (AgentErrorHandlingOverrideSection.Override)base.BaseGet(index);
				}
			}

			protected override ConfigurationElement CreateNewElement()
			{
				return new AgentErrorHandlingOverrideSection.Override();
			}

			protected override object GetElementKey(ConfigurationElement element)
			{
				return ((AgentErrorHandlingOverrideSection.Override)element).Name;
			}

			public override ConfigurationElementCollectionType CollectionType
			{
				get
				{
					return ConfigurationElementCollectionType.BasicMap;
				}
			}

			protected override string ElementName
			{
				get
				{
					return "override";
				}
			}
		}
	}
}
