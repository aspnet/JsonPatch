// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Converters;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch
{
    // Implementation details: the purpose of this type of patch document is to allow creation of such
    // documents for cases where there's no class/DTO to work on. Typical use case: backend not built in
    // .NET or architecture doesn't contain a shared DTO layer.
    [JsonConverter(typeof(JsonPatchDocumentConverter))]
    public class JsonPatchDocument : IJsonPatchDocument
    {
        public List<Operation> Operations { get; private set; }

        [JsonIgnore]
        public IContractResolver ContractResolver { get; set; }

        public JsonPatchDocument()
        {
            Operations = new List<Operation>();
            ContractResolver = new DefaultContractResolver();
        }

        public JsonPatchDocument(List<Operation> operations, IContractResolver contractResolver)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            if (contractResolver == null)
            {
                throw new ArgumentNullException(nameof(contractResolver));
            }

            Operations = operations;
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Add operation.  Will result in, for example,
        /// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
        /// </summary>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument Add(string path, object value)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation("add", PathHelpers.NormalizePath(path), null, value));
            return this;
        }

        /// <summary>
        /// Remove value at target location.  Will result in, for example,
        /// { "op": "remove", "path": "/a/b/c" }
        /// </summary>
        /// <param name="path">target location</param>
        /// <returns></returns>
        public JsonPatchDocument Remove(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation("remove", PathHelpers.NormalizePath(path), null, null));
            return this;
        }

        /// <summary>
        /// Replace value.  Will result in, for example,
        /// { "op": "replace", "path": "/a/b/c", "value": 42 }
        /// </summary>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument Replace(string path, object value)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation("replace", PathHelpers.NormalizePath(path), null, value));
            return this;
        }

        /// <summary>
        /// Test value.  Will result in, for example,
        /// { "op": "test", "path": "/a/b/c", "value": 42 }
        /// </summary>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument Test(string path, object value)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation("test", PathHelpers.NormalizePath(path), null, value));
            return this;
        }

        /// <summary>
        /// Removes value at specified location and add it to the target location.  Will result in, for example:
        /// { "op": "move", "from": "/a/b/c", "path": "/a/b/d" }
        /// </summary>
        /// <param name="from">source location</param>
        /// <param name="path">target location</param>
        /// <returns></returns>
        public JsonPatchDocument Move(string from, string path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation("move", PathHelpers.NormalizePath(path), PathHelpers.NormalizePath(from)));
            return this;
        }

        /// <summary>
        /// Copy the value at specified location to the target location.  Will result in, for example:
        /// { "op": "copy", "from": "/a/b/c", "path": "/a/b/e" }
        /// </summary>
        /// <param name="from">source location</param>
        /// <param name="path">target location</param>
        /// <returns></returns>
        public JsonPatchDocument Copy(string from, string path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation("copy", PathHelpers.NormalizePath(path), PathHelpers.NormalizePath(from)));
            return this;
        }

        /// <summary>
        /// Apply this JsonPatchDocument
        /// </summary>
        /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
        public void ApplyTo(object objectToApplyTo)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            ApplyTo(objectToApplyTo, new ObjectAdapter(ContractResolver, logErrorAction: null));
        }

        /// <summary>
        /// Apply this JsonPatchDocument
        /// </summary>
        /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
        /// <param name="logErrorAction">Action to log errors</param>
        public void ApplyTo(object objectToApplyTo, Action<JsonPatchError> logErrorAction)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            var adapter = new ObjectAdapter(ContractResolver, logErrorAction);
            foreach (var op in Operations)
            {
                try
                {
                    op.Apply(objectToApplyTo, adapter);
                }
                catch (JsonPatchException jsonPatchException)
                {
                    var errorReporter = logErrorAction ?? ErrorReporter.Default;
                    errorReporter(new JsonPatchError(objectToApplyTo, op, jsonPatchException.Message));

                    // As per JSON Patch spec if an operation results in error, further operations should not be executed.
                    break;
                }
            }
        }

        /// <summary>
        /// Apply this JsonPatchDocument
        /// </summary>
        /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
        /// <param name="adapter">IObjectAdapter instance to use when applying</param>
        public void ApplyTo(object objectToApplyTo, IObjectAdapter adapter)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            // apply each operation in order
            foreach (var op in Operations)
            {
                op.Apply(objectToApplyTo, adapter);
            }
        }

        /// <summary>
        /// Apply this JsonPatchDocument
        /// </summary>
        /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
        /// <param name="adapter">IObjectAdapterWithTest instance to use when applying</param>
        public void ApplyTo(object objectToApplyTo, IObjectAdapterWithTest adapter)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            // apply each operation in order
            foreach (var op in Operations)
            {
                op.Apply(objectToApplyTo, adapter);
            }
        }

        IList<Operation> IJsonPatchDocument.GetOperations()
        {
            var allOps = new List<Operation>();

            if (Operations != null)
            {
                foreach (var op in Operations)
                {
                    var untypedOp = new Operation();

                    untypedOp.op = op.op;
                    untypedOp.value = op.value;
                    untypedOp.path = op.path;
                    untypedOp.from = op.from;

                    allOps.Add(untypedOp);
                }
            }

            return allOps;
        }
    }
}
