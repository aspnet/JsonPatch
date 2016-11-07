﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test
{
    public class ParsedPathTests
    {
        [Theory]
        [InlineData("foo/bar~0baz", new string[] { "foo", "bar~baz" })]
        [InlineData("foo/bar~00baz", new string[] { "foo", "bar~0baz" })]
        [InlineData("foo/bar~01baz", new string[] { "foo", "bar~1baz" })]
        [InlineData("foo/bar~10baz", new string[] { "foo", "bar/0baz" })]
        [InlineData("foo/bar~1baz", new string[] { "foo", "bar/baz" })]
        [InlineData("foo/bar~0/~0/~1~1/~0~0/baz", new string[] { "foo", "bar~", "~", "//", "~~", "baz" })]
        [InlineData("~0~1foo", new string[] { "~/foo" })]
        public void ParsingValidPathShouldSucceed(string path, string[] expected)
        {
            var parsedPath = new ParsedPath(path);
            Assert.Equal(expected, parsedPath.Segments);
        }

        [Theory]
        [InlineData("foo/bar~")]
        [InlineData("~")]
        [InlineData("~2")]
        [InlineData("foo~3bar")]
        public void PathWithInvalidEscapeSequenceShouldFail(string path)
        {
            Assert.Throws<JsonPatchException>(() =>
            {
                var parsedPath = new ParsedPath(path);
            });
        }
    }
}
