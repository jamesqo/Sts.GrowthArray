using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StsProject.Tests.TestInternal
{
    // This needs to be public since public classes inherit from it.
    public abstract class ClassData<T> : IEnumerable<object[]>
    {
        public abstract IEnumerable<T> GetData();

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            foreach (T datum in GetData())
            {
                yield return new object[] { datum };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.AsEnumerable().GetEnumerator();
    }
}
