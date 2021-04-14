using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ActivityManagerConfig : ActivityConfig
	{
		internal ActivityManagerConfig(ActivityManagerConfig manager) : base(manager)
		{
		}

		internal ActivityConfig InitialActivity
		{
			get
			{
				return this.firstActivityConfig;
			}
			set
			{
				this.firstActivityConfig = value;
			}
		}

		internal string ClassName
		{
			get
			{
				return this.className;
			}
		}

		internal Type FsmProxyType
		{
			get
			{
				return this.fsmProxyType;
			}
		}

		internal static string BuildScopedConfigMapId(ActivityManagerConfig config, string id)
		{
			return config.UniqueId + "::" + id;
		}

		internal ActivityConfig GetScopedConfig(string id)
		{
			ActivityConfig activityConfig = (ActivityConfig)ActivityManagerConfig.scopedConfigMap[ActivityManagerConfig.BuildScopedConfigMapId(this, id)];
			if (activityConfig == null)
			{
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Request for unknown scoped activityid: {0}.", new object[]
				{
					id
				});
				throw new FsmConfigurationException(Strings.UnknownTransitionId(id, base.ActivityId));
			}
			return activityConfig;
		}

		internal bool TryGetScopedConfig(string id, out ActivityConfig config)
		{
			config = (ActivityConfig)ActivityManagerConfig.scopedConfigMap[ActivityManagerConfig.BuildScopedConfigMapId(this, id)];
			return config != null;
		}

		internal abstract ActivityManager CreateActivityManager(ActivityManager manager);

		internal override ActivityBase CreateActivity(ActivityManager manager)
		{
			return this.CreateActivityManager(manager);
		}

		internal override void Load(XmlNode rootNode)
		{
			this.className = rootNode.LocalName;
			Type type = GlobalActivityManager.ConfigClass.CoreAssembly.GetType("Microsoft.Exchange.UM.UMCore." + this.className);
			this.fsmProxyType = GlobalActivityManager.ConfigClass.CoreAssembly.GetType("Microsoft.Exchange.UM.Fsm." + this.className);
			if (null == type || null == this.fsmProxyType)
			{
				throw new FsmConfigurationException(Strings.InvalidActivityManager(rootNode.Name));
			}
			if (this is GlobalActivityManager.ConfigClass)
			{
				ActivityManagerConfig.scopedConfigMap.Clear();
			}
			base.Load(rootNode);
		}

		protected override void LoadChildren(XmlNode rootNode)
		{
			foreach (object obj in rootNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string text = string.Intern(xmlNode.Name);
				string text2 = (xmlNode.Attributes != null && xmlNode.Attributes["id"] != null) ? string.Intern(xmlNode.Attributes["id"].Value) : string.Empty;
				string key;
				switch (key = text)
				{
				case "Menu":
					this.AddScopedConfig(new MenuConfig(this), text2);
					continue;
				case "SpeechMenu":
					this.AddScopedConfig(new SpeechMenuConfig(this), text2);
					continue;
				case "Record":
					this.AddScopedConfig(new RecordConfig(this), text2);
					continue;
				case "CallTransfer":
					this.AddScopedConfig(new CallTransferConfig(this), text2);
					continue;
				case "PlayBackMenu":
					this.AddScopedConfig(new MenuConfig(this), text2);
					continue;
				case "FaxRequest":
					this.AddScopedConfig(new FaxRequestConfig(this), text2);
					continue;
				}
				if (text.EndsWith("Manager"))
				{
					string text3 = typeof(ActivityManagerConfig).Namespace + "." + text + "+ConfigClass";
					Type type = Type.GetType(text3, true);
					ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(ActivityManagerConfig)
					}, null);
					if (constructor == null)
					{
						throw new FsmConfigurationException(Strings.UnKnownManager(text3));
					}
					this.AddScopedConfig((ActivityManagerConfig)constructor.Invoke(new object[]
					{
						this
					}), text2);
				}
			}
			foreach (object obj2 in rootNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj2;
				string text = string.Intern(xmlNode2.Name);
				if (string.Equals(text, "Transition", StringComparison.OrdinalIgnoreCase))
				{
					base.ParseTransitionNode(xmlNode2, this);
				}
				else
				{
					XmlAttribute xmlAttribute = (xmlNode2.Attributes == null) ? null : xmlNode2.Attributes["id"];
					string text2 = (xmlAttribute == null) ? null : string.Intern(xmlAttribute.Value);
					if (!string.IsNullOrEmpty(text2) && text2 != "0")
					{
						this.GetScopedConfig(text2).Load(xmlNode2);
					}
				}
			}
			try
			{
				string text2 = string.Intern(rootNode.Attributes["firstActivityId"].Value);
				this.firstActivityConfig = this.GetScopedConfig(text2);
			}
			catch (FsmConfigurationException innerException)
			{
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Unknown first activity in activity manager {0}.", new object[]
				{
					this
				});
				throw new FsmConfigurationException(Strings.UnknownFirstActivityId(this.ToString()), innerException);
			}
		}

		private void AddScopedConfig(ActivityConfig config, string id)
		{
			string text = ActivityManagerConfig.BuildScopedConfigMapId(this, id);
			if (ActivityManagerConfig.scopedConfigMap.ContainsKey(text))
			{
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Duplicate scoped config id {0} found in xml configuration file.", new object[]
				{
					text
				});
				throw new FsmConfigurationException(Strings.DuplicateScopedId(text));
			}
			ActivityManagerConfig.scopedConfigMap[text] = config;
		}

		private static Hashtable scopedConfigMap = new Hashtable();

		private ActivityConfig firstActivityConfig;

		private string className;

		private Type fsmProxyType;
	}
}
