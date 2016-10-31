// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.JsonPatch.Operations
{
    public class OperationBase
    {
        private string _operation;
        private OperationType _operationType;

        [JsonIgnore]
        public OperationType OperationType
        {
            get
            {
                return _operationType;
            }
        }

        [JsonProperty("path")]
        public string path { get; set; }

        [JsonProperty("op")]
        public string op
        {
            get
            {
                return _operation;
            }
            set
            {
                OperationType result;
                if (!Enum.TryParse<OperationType>(value, ignoreCase: true, result: out result))
                {
                    throw new JsonPatchException($"Invalid JsonPatch operation '{value}'.", innerException: null);
                }
                _operation = value;
                _operationType = result;
            }
        }

        [JsonProperty("from")]
        public string from { get; set; }

        public OperationBase()
        {

        }

        public OperationBase(string op, string path, string from)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.op = op;
            this.path = path;
            this.from = from;
        }

        public bool ShouldSerializefrom()
        {
            return (OperationType == OperationType.Move
                || OperationType == OperationType.Copy);
        }
    }
}