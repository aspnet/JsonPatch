using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch.Extensions
{
    public static class ObjectExtension
    {
        public static JsonPatchDocument<T> GeneratePatch<T>(this T updated, T original) where T : class
        {
            var source = original as JToken;
            var target = updated as JToken;

            if (source == null || target == null)
            {
                source = original != null ? JToken.FromObject(original) : null;
                target = updated != null ? JToken.FromObject(updated) : null;
            }
            var operations = Diff<T>(source, target);
            return new JsonPatchDocument<T>(operations, new DefaultContractResolver());
        }

        private static List<Operation<T>> Diff<T>(JToken source, JToken target, List<Operation<T>> operations = null)
            where T : class
        {
            if (operations == null)
            {
                operations = new List<Operation<T>>();
            }

            if (source == null)
            {
                source = new JObject();
            }

            if (target == null)
            {
                target = new JObject();
            }

            foreach (var i in source)
            {
                var targetToken = target.FirstOrDefault(x => x.Path.Equals(i.Path));
                var sourceToken = source.FirstOrDefault(x => x.Path.Equals(i.Path));
                if (targetToken == null)
                {
                    operations.Add(new Operation<T>("remove", NormalizePath(i.Path), null));
                }
                else
                {
                    DiffField(sourceToken, targetToken, operations);
                }
            }

            foreach (var i in target)
            {
                var sourceToken = source.FirstOrDefault(x => x.Path.Equals(i.Path));
                if (sourceToken == null)
                {
                    operations.Add(new Operation<T>("add", NormalizePath(i.Path), null, i));
                }
            }

            return operations;
        }

        private static void DiffField<T>(JToken source, JToken target, List<Operation<T>> operations)
            where T : class
        {
            if (source.Type == JTokenType.Property)
            {
                Diff(source as JProperty, target as JProperty, operations);
            }
            else if (source.Type == JTokenType.Object)
            {
                Diff(source as JObject, target as JObject, operations);
            }
            else if (source.Type == JTokenType.Array)
            {
                var sourceArray = (source as JArray);
                var targetArray = (target as JArray);
                if (targetArray == null)
                {
                    operations.Add(new Operation<T>("remove", NormalizePath(target.Path), null));
                }

                var maxIndex = Math.Max(sourceArray.Count, targetArray.Count);
                var minIndex = Math.Min(sourceArray.Count, targetArray.Count);

                for (var i = 0; i < maxIndex; i++)
                {
                    if (i < minIndex)
                    {
                        DiffField(sourceArray[i], targetArray[i], operations);
                    }
                    else if (i >= sourceArray.Count)
                    {
                        operations.Add(new Operation<T>("add", NormalizePath(targetArray[i].Path), null, targetArray[i]));
                    }
                    else
                    {
                        operations.Add(new Operation<T>("remove", NormalizePath(sourceArray[i].Path), null));
                    }
                }
            }
            else
            {
                if (!JToken.DeepEquals(source, target))
                {
                    operations.Add(new Operation<T>("replace", NormalizePath(target.Path), null, target));
                }
            }
        }

        private static string NormalizePath(string path)
        {
            path = path.Replace('[', '/');
            path = path.Replace("].", "/");
            path = path.Replace("]", "/");
            path = path.Replace('.', '/');

            if (path.EndsWith("/"))
            {
                path = path.Remove(path.Length - 1);
            }

            if (!(path.StartsWith("/")))
            {
                return "/" + path;
            }
            else
            {
                return path;
            }
        }
    }
}