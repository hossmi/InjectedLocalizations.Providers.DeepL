using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.MemberParsing;

namespace InjectedLocalizations.Models
{
    public class FakeParsedMember : IParsedMember
    {
        private readonly IReadOnlyList<IToken> tokens;

        public FakeParsedMember(MemberInfo member, params IToken[] tokens)
        {
            this.Member = member;
            this.tokens = tokens.ToList();
        }

        public MemberInfo Member { get; }
        public int Count => this.tokens.Count;

        public static IParsedMember BuildFor<T>(string memberName, params IToken[] tokens)
        {
            MemberInfo member;

            member = typeof(T)
                .GetMember(memberName)
                .Single();

            return new FakeParsedMember(member, tokens);
        }

        public IEnumerator<IToken> GetEnumerator() => this.tokens.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}