using System.Collections.Generic;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public interface IParsedPath
    {
        string LastSegment { get; }
        IReadOnlyList<string> Segments { get; }
    }
}