﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    internal static class JsonPatchErrorHelper
    {
        public static void ReportError(Action<JsonPatchError> logErrorAction, JsonPatchError error)
        {
            if (logErrorAction != null)
            {
                logErrorAction(error);
            }
            else
            {
                throw new JsonPatchException(error);
            }
        }
    }
}
