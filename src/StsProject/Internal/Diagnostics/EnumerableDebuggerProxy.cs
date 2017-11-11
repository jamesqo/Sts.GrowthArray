using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StsProject.Internal.Diagnostics
{
    [ExcludeFromCodeCoverage]
    internal class EnumerableDebuggerProxy<T>
    {
        private readonly IEnumerable<T> _items;

        public EnumerableDebuggerProxy(IEnumerable<T> items)
        {
            _items = items;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _items.ToArray();
    }
}
