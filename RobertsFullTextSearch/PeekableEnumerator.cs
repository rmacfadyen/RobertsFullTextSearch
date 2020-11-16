using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobertsFullTextSearch
{
    public static class PeekableEnumeratorExtension
    {
        public static PeekableEnumerator<T> AsPeekable<T>(this IEnumerator<T> enumerator)
        {
            return new PeekableEnumerator<T>(enumerator);
        }
    }

    public class PeekableEnumerator<T> : IEnumerator<T>
    {
        protected enum Status { Uninitialized, Starting, Started, Ending, Ended }

        protected IEnumerator<T> enumerator;

        protected Status status;

        protected T current;

        protected T peek;

        public PeekableEnumerator(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
            status = Status.Uninitialized;
            MoveNext();
        }

        public T Current
        {
            get
            {
                if (Status.Starting == status)
                    throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                if (Status.Ended == status)
                    throw new InvalidOperationException("Enumeration already finished.");

                return current;
            }
        }

        object System.Collections.IEnumerator.Current { get { return Current; } }

        public T Peek
        {
            get
            {
                if (Status.Ending == status)
                    return default(T);
                else if (Status.Ended == status)
                    throw new InvalidOperationException("Enumeration already finished.");

                return peek;
            }
        }

        public bool MoveNext()
        {
            current = peek;
            switch (status)
            {
                case Status.Uninitialized:
                case Status.Starting:
                    if (enumerator.MoveNext())
                    {
                        status++;
                        peek = enumerator.Current;
                    }
                    else
                        status = Status.Ending;
                    break;
                case Status.Started:
                    if (enumerator.MoveNext())
                        peek = enumerator.Current;
                    else
                        status++;
                    break;
                case Status.Ending:
                    status++;
                    break;
            }

            return Status.Ended != status;
        }

        public void Reset()
        {
            enumerator.Reset();
            status = Status.Uninitialized;
            MoveNext();
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }
    }
}
