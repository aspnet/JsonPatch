// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public struct ParsedPath
    {
        private static readonly string[] Empty = null;

        private readonly string[] _segments;

        private static readonly Regex escapingRegex = new Regex("~(?<code>0|1)", RegexOptions.Compiled);

        public ParsedPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            _segments = path
                .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => escapingRegex.Replace(segment, (match) => match.Groups["code"].Value == "0" ? "~" : "/"))
                .ToArray();
        }

        public string LastSegment
        {
            get
            {
                if (_segments == null || _segments.Length == 0)
                {
                    return null;
                }

                return _segments[_segments.Length - 1];
            }
        }

        public IReadOnlyList<string> Segments => _segments ?? Empty;
    }
}
