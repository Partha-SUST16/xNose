using System.Linq;
using System;
using xNose.Core.Walkers;

namespace xNose.Core.Smells
{
	public class InappropriateAssertionsTestSmell : ASmell
    {

        public override bool HasSmell()
        {
            var root = GetRoot();
            var methodBodyWalker = new MethodBodyWalker();
            methodBodyWalker.Visit(root);
            var invocations = methodBodyWalker.Invocations
                 .Where(x => (x.Expression.ToString().Contains("Assert.False", StringComparison.InvariantCultureIgnoreCase) ||
                            x.Expression.ToString().Contains("Assert.True", StringComparison.InvariantCultureIgnoreCase)));

            foreach (var invocation in invocations)
            {
                if (invocation.ArgumentList.Arguments.ToString().Contains('='))
                    return true;
            }
            invocations = methodBodyWalker.Invocations
                 .Where(x => x.Expression.ToString().Contains("Assert.Equal", StringComparison.InvariantCultureIgnoreCase));

            foreach (var invocation in invocations)
            {
                var arguments = invocation.ArgumentList;
                var firstArgumentParsed = bool.TryParse(arguments.Arguments[0].ToString(), out var _);
                var secondArgumentParsed = bool.TryParse(arguments.Arguments[1].ToString(), out var _);
                if (firstArgumentParsed || secondArgumentParsed)
                    return true;
            }
            return false;
        }

        public override string Name()
        {
            return nameof(InappropriateAssertionsTestSmell);
        }
    }
}

