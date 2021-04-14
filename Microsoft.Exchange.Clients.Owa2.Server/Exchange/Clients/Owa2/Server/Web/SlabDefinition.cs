using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabDefinition
	{
		public SlabDefinition()
		{
			this.types = new List<string>();
			this.templates = new List<string>();
			this.bindings = new List<SlabBinding>();
		}

		protected IList<SlabBinding> Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		protected IList<string> Templates
		{
			get
			{
				return this.templates;
			}
		}

		protected IList<string> Types
		{
			get
			{
				return this.types;
			}
		}

		public void AddType(string typeName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			this.types.Add(typeName);
		}

		public void AddTemplate(string templateName)
		{
			if (templateName == null)
			{
				throw new ArgumentNullException("templateName");
			}
			this.templates.Add(templateName);
		}

		public string[] GetTypes()
		{
			return this.types.ToArray<string>();
		}

		public void AddBinding(SlabBinding binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}
			this.bindings.Add(binding);
		}

		public IEnumerable<string> GetFeatures()
		{
			return from binding in this.bindings
			from feature in binding.Features
			select feature;
		}

		public bool HasBinding(string featureName)
		{
			if (featureName == null)
			{
				throw new ArgumentNullException("featureName");
			}
			return this.HasBinding(new string[]
			{
				featureName
			});
		}

		public bool HasBinding(string[] featureNames)
		{
			if (featureNames == null)
			{
				throw new ArgumentNullException("featureNames");
			}
			return this.bindings.Any((SlabBinding b) => b.Implement(featureNames));
		}

		public bool HasDefaultBinding()
		{
			return this.bindings.Any((SlabBinding b) => b.IsDefault);
		}

		public Slab GetDefaultSlab(LayoutType layout, out IEnumerable<string> slabDependencies)
		{
			SlabBinding binding = this.FindDefaultBinding();
			return this.GetSlabForBinding(binding, layout, out slabDependencies);
		}

		public virtual Slab GetSlab(string[] featureNames, LayoutType layout, out IEnumerable<string> slabDependencies)
		{
			if (featureNames == null)
			{
				throw new ArgumentNullException("featureNames");
			}
			SlabBinding binding = this.FindBinding(featureNames);
			return this.GetSlabForBinding(binding, layout, out slabDependencies);
		}

		protected virtual Slab GetSlabForBinding(SlabBinding binding, LayoutType layout, out IEnumerable<string> slabDependencies)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}
			slabDependencies = binding.Dependencies;
			return new Slab
			{
				Types = this.types.ToArray<string>(),
				Templates = this.templates.ToArray<string>(),
				Styles = (from s in binding.Styles
				where s.IsForLayout(layout)
				select s).ToArray<SlabStyleFile>(),
				Configurations = (from s in binding.Configurations.Distinct<SlabConfiguration>()
				where s.IsForLayout(layout)
				select s).ToArray<SlabConfiguration>(),
				Dependencies = binding.Dependencies.Distinct<string>().ToArray<string>(),
				PackagedSources = (from s in binding.PackagedSources
				where s.IsForLayout(layout)
				select s).ToArray<SlabSourceFile>(),
				Images = (from s in binding.Images
				where s.IsForLayout(layout)
				select s).ToArray<SlabImageFile>(),
				Fonts = (from s in binding.Fonts
				where s.IsForLayout(layout)
				select s).ToArray<SlabFontFile>(),
				Sources = (from s in binding.Sources
				where s.IsForLayout(layout)
				select s).ToArray<SlabSourceFile>(),
				Strings = (from s in binding.Strings
				where s.IsForLayout(layout)
				select s).ToArray<SlabStringFile>(),
				PackagedStrings = (from s in binding.PackagedStrings
				where s.IsForLayout(layout)
				select s).ToArray<SlabStringFile>()
			};
		}

		protected SlabBinding FindDefaultBinding()
		{
			SlabBinding slabBinding = this.bindings.FirstOrDefault((SlabBinding b) => b.IsDefault);
			if (slabBinding == null)
			{
				throw new SlabBindingNotFoundException("Default Binding not found");
			}
			return slabBinding;
		}

		private SlabBinding FindBinding(string[] featureNames)
		{
			using (IEnumerator<SlabBinding> enumerator = this.Bindings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SlabBinding binding = enumerator.Current;
					if (featureNames.Any((string feature) => binding.Implement(new string[]
					{
						feature
					})))
					{
						return binding;
					}
				}
			}
			throw new SlabBindingNotFoundException(string.Format("Binding not found for feature '{0}'", string.Join(",", featureNames)));
		}

		private readonly IList<string> types;

		private readonly IList<string> templates;

		private readonly IList<SlabBinding> bindings;
	}
}
