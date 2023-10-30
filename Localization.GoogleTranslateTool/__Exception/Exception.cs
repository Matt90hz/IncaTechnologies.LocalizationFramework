using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization.GoogleTranslateTool.__Exception
{
    internal class Exception<T> : Exception
    {
        public bool IsThrown { get; } = true;

        public T? ValueOrDefult { get; }

        public T ValueOrThrow => IsThrown ? ValueOrDefult! : throw this;

        public Exception(T value)
        {
            IsThrown = false;
            ValueOrDefult = value;
        }

        public Exception(string message) : base(message) { }

        public Exception(string message, Exception inner) : base(message, inner) { }
    }


}
