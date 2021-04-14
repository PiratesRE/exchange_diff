using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.TextProcessing;

namespace Microsoft.Exchange.TextProcessing
{
	internal class MatchFactory
	{
		private Dictionary<BoundaryType, TrieInfo> TrieMap
		{
			get
			{
				if (this.trieMap == null)
				{
					this.trieMap = new Dictionary<BoundaryType, TrieInfo>();
					this.AddTrieToMap(BoundaryType.None);
					this.AddTrieToMap(BoundaryType.Normal);
					this.AddTrieToMap(BoundaryType.NormalLeftOnly);
					this.AddTrieToMap(BoundaryType.NormalRightOnly);
					this.AddTrieToMap(BoundaryType.Url);
					this.AddTrieToMap(BoundaryType.FullUrl);
				}
				return this.trieMap;
			}
		}

		private TrieInfo NoBoundaryTrie
		{
			get
			{
				if (this.noBoundaryTrie == null)
				{
					this.noBoundaryTrie = new TrieInfo(IDGenerator.GetNextID(), new Trie(BoundaryType.None, false));
				}
				return this.noBoundaryTrie;
			}
		}

		public IMatch CreateTermSet(IEnumerable<string> terms, BoundaryType boundaryType)
		{
			return new TermMatch(terms, boundaryType, this.TrieMap);
		}

		public IMatch CreateRegexTermSet(IEnumerable<string> terms)
		{
			return new RegexTermMatch(terms);
		}

		public IMatch CreateSingleExecutionTermSet(ICollection<string> terms)
		{
			if (terms.Count > 50)
			{
				return new TermMatch(terms, BoundaryType.Normal, this.TrieMap);
			}
			return new RegexTermMatch(terms);
		}

		public IMatch CreateSingleTrieTermSet(ICollection<string> terms, BoundaryType boundaryType)
		{
			return new SingleTrieTermMatch(terms, boundaryType, this.NoBoundaryTrie);
		}

		public IMatch CreateRegex(string pattern, bool caseSensitive = true, bool compiled = false)
		{
			return this.CreateRegex(pattern, caseSensitive ? CaseSensitivityMode.Sensitive : CaseSensitivityMode.Insensitive, compiled ? MatchRegexOptions.Compiled : MatchRegexOptions.None, MatcherConstants.DefaultRegexMatchTimeout);
		}

		public IMatch CreateRegex(string pattern, CaseSensitivityMode caseSensitivityMode, MatchRegexOptions options)
		{
			return this.CreateRegex(pattern, caseSensitivityMode, options, MatcherConstants.DefaultRegexMatchTimeout);
		}

		public IMatch CreateRegex(string pattern, CaseSensitivityMode caseSensitivityMode, MatchRegexOptions options, TimeSpan matchTimeout)
		{
			return new RegexMatch(pattern, caseSensitivityMode, options, matchTimeout);
		}

		public IMatch CreateConditional(IMatch match, IMatch precondition)
		{
			if (match == null || precondition == null)
			{
				throw new ArgumentException(Strings.NullIMatch);
			}
			return new ConditionalMatch(match, precondition);
		}

		public IMatch CreateConditionalRegex(string pattern, IEnumerable<string> termConditions)
		{
			return new ConditionalMatch(this.CreateRegex(pattern, true, true), this.CreateTermSet(termConditions, BoundaryType.None));
		}

		public IMatch CreateSimilarityMatch(LshFingerprint fingerprint, double coefficient)
		{
			if (fingerprint == null)
			{
				throw new ArgumentException(Strings.NullFingerprint);
			}
			if (coefficient < MatcherConstants.MinimumCoefficient || coefficient > MatcherConstants.MaximumCoefficient)
			{
				throw new ArgumentException(Strings.InvalidCoefficient(coefficient));
			}
			return new FingerprintMatch(fingerprint, coefficient, false);
		}

		public IMatch CreateContainmentMatch(LshFingerprint fingerprint, double coefficient)
		{
			if (fingerprint == null)
			{
				throw new ArgumentException(Strings.NullFingerprint);
			}
			if (coefficient < MatcherConstants.MinimumCoefficient || coefficient > MatcherConstants.MaximumCoefficient)
			{
				throw new ArgumentException(Strings.InvalidCoefficient(coefficient));
			}
			return new FingerprintMatch(fingerprint, coefficient, true);
		}

		public IMatch Copy(IMatch match)
		{
			if (match is ConditionalMatch)
			{
				return new ConditionalMatch(match as ConditionalMatch, this);
			}
			if (match is RegexMatch)
			{
				return new RegexMatch(match as RegexMatch);
			}
			if (match is TermMatch)
			{
				return new TermMatch(match as TermMatch, this.TrieMap);
			}
			if (match is RegexTermMatch)
			{
				return match;
			}
			if (match is SingleTrieTermMatch)
			{
				return new SingleTrieTermMatch(match as SingleTrieTermMatch, this.NoBoundaryTrie);
			}
			if (match is FingerprintMatch)
			{
				return match;
			}
			if (match is NullMatch)
			{
				return match;
			}
			ExTraceGlobals.MatcherTracer.TraceDebug<Type>((long)this.GetHashCode(), "Attempting to copy an unknown match type {0}", match.GetType());
			throw new ArgumentException(Strings.UnsupportedMatch);
		}

		private void AddTrieToMap(BoundaryType boundaryType)
		{
			this.trieMap[boundaryType] = new TrieInfo(IDGenerator.GetNextID(), new Trie(boundaryType, false));
		}

		private const int RegexTermsCreationThreshold = 50;

		private Dictionary<BoundaryType, TrieInfo> trieMap;

		private TrieInfo noBoundaryTrie;
	}
}
