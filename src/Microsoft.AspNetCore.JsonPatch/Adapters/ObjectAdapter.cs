using Microsoft.AspNetCore.JsonPatch.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch.Adapters
{
    /// <inheritdoc />
    public class ObjectAdapter : ObjectAdapterTyped<ParsedPath, ObjectVisitor>
    {
        public ObjectAdapter(IContractResolver contractResolver, Action<JsonPatchError> logErrorAction) : base(contractResolver, logErrorAction)
        {
        }
    }
}
