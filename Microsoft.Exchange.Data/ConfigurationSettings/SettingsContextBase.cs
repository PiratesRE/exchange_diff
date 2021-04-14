using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SettingsContextBase : ISettingsContext, IDiagnosableObject
	{
		public SettingsContextBase(SettingsContextBase nextContext)
		{
			this.HashableIdentity = Guid.NewGuid().ToString();
			this.NextContext = nextContext;
		}

		public static Func<ISettingsContext> DefaultContextGetter { get; set; }

		public string HashableIdentity { get; private set; }

		public SettingsContextBase NextContext { get; private set; }

		public virtual Guid? ServerGuid
		{
			get
			{
				return null;
			}
		}

		public virtual string ServerName
		{
			get
			{
				return null;
			}
		}

		public virtual ServerVersion ServerVersion
		{
			get
			{
				return null;
			}
		}

		public virtual string ServerRole
		{
			get
			{
				return null;
			}
		}

		public virtual string ProcessName
		{
			get
			{
				return null;
			}
		}

		public virtual Guid? DatabaseGuid
		{
			get
			{
				return null;
			}
		}

		public virtual string DatabaseName
		{
			get
			{
				return null;
			}
		}

		public virtual ServerVersion DatabaseVersion
		{
			get
			{
				return null;
			}
		}

		public virtual Guid? DagOrServerGuid
		{
			get
			{
				return null;
			}
		}

		public virtual string OrganizationName
		{
			get
			{
				return null;
			}
		}

		public virtual ExchangeObjectVersion OrganizationVersion
		{
			get
			{
				return null;
			}
		}

		public virtual Guid? MailboxGuid
		{
			get
			{
				return null;
			}
		}

		public virtual string GetGenericProperty(string propertyName)
		{
			return null;
		}

		public static List<SettingsContextBase> GetCurrentContexts()
		{
			List<SettingsContextBase> list = new List<SettingsContextBase>();
			for (SettingsContextBase.NestedContext nestedContext = SettingsContextBase.NestedContext.Current; nestedContext != null; nestedContext = nestedContext.ParentContext)
			{
				if (nestedContext.Context != null)
				{
					list.Insert(0, nestedContext.Context);
				}
			}
			return list;
		}

		public static void RunOperationInContext(List<SettingsContextBase> contexts, Action operation)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				if (contexts != null)
				{
					foreach (SettingsContextBase settingsContextBase in contexts)
					{
						disposeGuard.Add<IDisposable>(settingsContextBase.Activate());
					}
				}
				operation();
			}
		}

		public static IDisposable ActivateContext(ISettingsContextProvider provider)
		{
			if (provider != null)
			{
				SettingsContextBase settingsContextBase = provider.GetSettingsContext() as SettingsContextBase;
				if (settingsContextBase != null)
				{
					return settingsContextBase.Activate();
				}
			}
			return null;
		}

		public IDisposable Activate()
		{
			return new SettingsContextBase.NestedContext(this);
		}

		public virtual XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement("context");
			if (this.ServerName != null || this.ServerVersion != null)
			{
				xelement.Add(new XElement("server", new object[]
				{
					new XAttribute("guid", (this.ServerGuid != null) ? this.ServerGuid.Value.ToString() : "null"),
					new XAttribute("name", this.ServerName ?? "null"),
					new XAttribute("role", this.ServerRole ?? "null"),
					new XAttribute("version", (this.ServerVersion != null) ? this.ServerVersion.ToString() : "null")
				}));
			}
			if (this.ProcessName != null)
			{
				xelement.Add(new XElement("process", new XAttribute("name", this.ProcessName)));
			}
			if (this.DagOrServerGuid != null)
			{
				xelement.Add(new XElement("dagorserver", new XAttribute("guid", this.DagOrServerGuid.Value.ToString())));
			}
			if (this.DatabaseName != null || this.DatabaseVersion != null)
			{
				xelement.Add(new XElement("database", new object[]
				{
					new XAttribute("guid", (this.DatabaseGuid != null) ? this.DatabaseGuid.Value.ToString() : "null"),
					new XAttribute("name", this.DatabaseName ?? "null"),
					new XAttribute("version", (this.DatabaseVersion != null) ? this.DatabaseVersion.ToString() : "null")
				}));
			}
			if (this.OrganizationName != null || this.OrganizationVersion != null)
			{
				xelement.Add(new XElement("organization", new object[]
				{
					new XAttribute("name", this.OrganizationName ?? "null"),
					new XAttribute("version", (this.OrganizationVersion != null) ? this.OrganizationVersion.ToString() : "null")
				}));
			}
			if (this.MailboxGuid != null)
			{
				xelement.Add(new XElement("mailbox", new XAttribute("guid", this.MailboxGuid)));
			}
			IDiagnosableObject nextContext = this.NextContext;
			if (nextContext != null)
			{
				XElement diagnosticInfo = nextContext.GetDiagnosticInfo(argument);
				diagnosticInfo.Name = "nextContext";
				xelement.Add(diagnosticInfo);
			}
			return xelement;
		}

		public static readonly ISettingsContext EffectiveContext = new SettingsContextBase.EffectiveContextObject();

		private class NestedContext : DisposeTrackableBase
		{
			public NestedContext(SettingsContextBase context)
			{
				this.Context = context;
				this.ParentContext = SettingsContextBase.NestedContext.current;
				SettingsContextBase.NestedContext.current = this;
			}

			public static SettingsContextBase.NestedContext Current
			{
				get
				{
					return SettingsContextBase.NestedContext.current;
				}
			}

			public SettingsContextBase.NestedContext ParentContext { get; private set; }

			public SettingsContextBase Context { get; private set; }

			protected override void InternalDispose(bool disposing)
			{
				SettingsContextBase.NestedContext.current = this.ParentContext;
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<SettingsContextBase.NestedContext>(this);
			}

			[ThreadStatic]
			private static SettingsContextBase.NestedContext current;
		}

		private class EffectiveContextObject : ISettingsContext
		{
			Guid? ISettingsContext.ServerGuid
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<Guid?>((SettingsContextBase ctx) => ctx.ServerGuid);
				}
			}

			string ISettingsContext.ServerName
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<string>((SettingsContextBase ctx) => ctx.ServerName);
				}
			}

			ServerVersion ISettingsContext.ServerVersion
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<ServerVersion>((SettingsContextBase ctx) => ctx.ServerVersion);
				}
			}

			string ISettingsContext.ServerRole
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<string>((SettingsContextBase ctx) => ctx.ServerRole);
				}
			}

			string ISettingsContext.ProcessName
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<string>((SettingsContextBase ctx) => ctx.ProcessName);
				}
			}

			Guid? ISettingsContext.DatabaseGuid
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<Guid?>((SettingsContextBase ctx) => ctx.DatabaseGuid);
				}
			}

			string ISettingsContext.DatabaseName
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<string>((SettingsContextBase ctx) => ctx.DatabaseName);
				}
			}

			ServerVersion ISettingsContext.DatabaseVersion
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<ServerVersion>((SettingsContextBase ctx) => ctx.DatabaseVersion);
				}
			}

			Guid? ISettingsContext.DagOrServerGuid
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<Guid?>((SettingsContextBase ctx) => ctx.DagOrServerGuid);
				}
			}

			string ISettingsContext.OrganizationName
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<string>((SettingsContextBase ctx) => ctx.OrganizationName);
				}
			}

			ExchangeObjectVersion ISettingsContext.OrganizationVersion
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<ExchangeObjectVersion>((SettingsContextBase ctx) => ctx.OrganizationVersion);
				}
			}

			Guid? ISettingsContext.MailboxGuid
			{
				get
				{
					return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<Guid?>((SettingsContextBase ctx) => ctx.MailboxGuid);
				}
			}

			string ISettingsContext.GetGenericProperty(string propertyName)
			{
				return SettingsContextBase.EffectiveContextObject.ComputeInheritedValue<string>((SettingsContextBase ctx) => ctx.GetGenericProperty(propertyName));
			}

			private static T ComputeInheritedValue<T>(Func<SettingsContextBase, T> valueGetter)
			{
				for (SettingsContextBase.NestedContext nestedContext = SettingsContextBase.NestedContext.Current; nestedContext != null; nestedContext = nestedContext.ParentContext)
				{
					T t = SettingsContextBase.EffectiveContextObject.ComputeChainedValue<T>(nestedContext.Context, valueGetter);
					if (t != null && !t.Equals(default(T)))
					{
						return t;
					}
				}
				ISettingsContext settingsContext = (SettingsContextBase.DefaultContextGetter != null) ? SettingsContextBase.DefaultContextGetter() : null;
				return SettingsContextBase.EffectiveContextObject.ComputeChainedValue<T>(settingsContext as SettingsContextBase, valueGetter);
			}

			private static T ComputeChainedValue<T>(SettingsContextBase firstContext, Func<SettingsContextBase, T> valueGetter)
			{
				for (SettingsContextBase settingsContextBase = firstContext; settingsContextBase != null; settingsContextBase = settingsContextBase.NextContext)
				{
					T t = valueGetter(settingsContextBase);
					if (t != null && !t.Equals(default(T)))
					{
						return t;
					}
				}
				return default(T);
			}
		}
	}
}
