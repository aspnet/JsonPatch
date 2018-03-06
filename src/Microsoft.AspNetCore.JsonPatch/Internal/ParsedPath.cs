﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.JsonPatch.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public class ParsedPath : IParsedPath
    {
        private static readonly string[] Empty = null;

        private readonly string[] _segments;

        public static List<char> Separators = new List<char> { '/' };

        public ParsedPath(string path):this(path, null)
        {
        }

        public ParsedPath(string path, List<char> separators)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (separators!=null)
            {
                Separators = separators;
            }

            _segments = ParsePath(path);
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

        private static string[] ParsePath(string path)
        {
            var strings = new List<string>();
            var sb = new StringBuilder(path.Length);

            for (var i = 0; i < path.Length; i++)
            {
                if (Separators.Contains(path[i]))
                {
                    if (sb.Length > 0)
                    {
                        strings.Add(sb.ToString());
                        sb.Length = 0;
                    }
                }
                else if (path[i] == '~')
                {
                    ++i;
                    if (i >= path.Length)
                    {
                        throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
                    }

                    if (path[i] == '0')
                    {
                        sb.Append('~');
                    }
                    else if (path[i] == '1')
                    {
                        sb.Append('/');
                    }
                    else
                    {
                        throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
                    }
                }
                else
                {
                    sb.Append(path[i]);
                }
            }

            if (sb.Length > 0)
            {
                strings.Add(sb.ToString());
            }

            return strings.ToArray();
        }
    }
}
