// <auto-generated />
namespace Microsoft.AspNetCore.JsonPatch
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.AspNetCore.JsonPatch.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// The property at '{0}' could not be copied.
        /// </summary>
        internal static string CannotCopyProperty
        {
            get => GetString("CannotCopyProperty");
        }

        /// <summary>
        /// The property at '{0}' could not be copied.
        /// </summary>
        internal static string FormatCannotCopyProperty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CannotCopyProperty"), p0);

        /// <summary>
        /// The type of the property at path '{0}' could not be determined.
        /// </summary>
        internal static string CannotDeterminePropertyType
        {
            get => GetString("CannotDeterminePropertyType");
        }

        /// <summary>
        /// The type of the property at path '{0}' could not be determined.
        /// </summary>
        internal static string FormatCannotDeterminePropertyType(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CannotDeterminePropertyType"), p0);

        /// <summary>
        /// The '{0}' operation at path '{1}' could not be performed.
        /// </summary>
        internal static string CannotPerformOperation
        {
            get => GetString("CannotPerformOperation");
        }

        /// <summary>
        /// The '{0}' operation at path '{1}' could not be performed.
        /// </summary>
        internal static string FormatCannotPerformOperation(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("CannotPerformOperation"), p0, p1);

        /// <summary>
        /// The property at '{0}' could not be read.
        /// </summary>
        internal static string CannotReadProperty
        {
            get => GetString("CannotReadProperty");
        }

        /// <summary>
        /// The property at '{0}' could not be read.
        /// </summary>
        internal static string FormatCannotReadProperty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CannotReadProperty"), p0);

        /// <summary>
        /// The property at path '{0}' could not be updated.
        /// </summary>
        internal static string CannotUpdateProperty
        {
            get => GetString("CannotUpdateProperty");
        }

        /// <summary>
        /// The property at path '{0}' could not be updated.
        /// </summary>
        internal static string FormatCannotUpdateProperty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("CannotUpdateProperty"), p0);

        /// <summary>
        /// The expression '{0}' is not supported. Supported expressions include member access and indexer expressions.
        /// </summary>
        internal static string ExpressionTypeNotSupported
        {
            get => GetString("ExpressionTypeNotSupported");
        }

        /// <summary>
        /// The expression '{0}' is not supported. Supported expressions include member access and indexer expressions.
        /// </summary>
        internal static string FormatExpressionTypeNotSupported(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("ExpressionTypeNotSupported"), p0);

        /// <summary>
        /// The index value provided by path segment '{0}' is out of bounds of the array size.
        /// </summary>
        internal static string IndexOutOfBounds
        {
            get => GetString("IndexOutOfBounds");
        }

        /// <summary>
        /// The index value provided by path segment '{0}' is out of bounds of the array size.
        /// </summary>
        internal static string FormatIndexOutOfBounds(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("IndexOutOfBounds"), p0);

        /// <summary>
        /// The path segment '{0}' is invalid for an array index.
        /// </summary>
        internal static string InvalidIndexValue
        {
            get => GetString("InvalidIndexValue");
        }

        /// <summary>
        /// The path segment '{0}' is invalid for an array index.
        /// </summary>
        internal static string FormatInvalidIndexValue(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InvalidIndexValue"), p0);

        /// <summary>
        /// The type '{0}' was malformed and could not be parsed.
        /// </summary>
        internal static string InvalidJsonPatchDocument
        {
            get => GetString("InvalidJsonPatchDocument");
        }

        /// <summary>
        /// The type '{0}' was malformed and could not be parsed.
        /// </summary>
        internal static string FormatInvalidJsonPatchDocument(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InvalidJsonPatchDocument"), p0);

        /// <summary>
        /// Invalid JsonPatch operation '{0}'.
        /// </summary>
        internal static string InvalidJsonPatchOperation
        {
            get => GetString("InvalidJsonPatchOperation");
        }

        /// <summary>
        /// Invalid JsonPatch operation '{0}'.
        /// </summary>
        internal static string FormatInvalidJsonPatchOperation(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InvalidJsonPatchOperation"), p0);

        /// <summary>
        /// The provided path segment '{0}' cannot be converted to the target type.
        /// </summary>
        internal static string InvalidPathSegment
        {
            get => GetString("InvalidPathSegment");
        }

        /// <summary>
        /// The provided path segment '{0}' cannot be converted to the target type.
        /// </summary>
        internal static string FormatInvalidPathSegment(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InvalidPathSegment"), p0);

        /// <summary>
        /// The provided string '{0}' is an invalid path.
        /// </summary>
        internal static string InvalidValueForPath
        {
            get => GetString("InvalidValueForPath");
        }

        /// <summary>
        /// The provided string '{0}' is an invalid path.
        /// </summary>
        internal static string FormatInvalidValueForPath(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InvalidValueForPath"), p0);

        /// <summary>
        /// The value '{0}' is invalid for target location.
        /// </summary>
        internal static string InvalidValueForProperty
        {
            get => GetString("InvalidValueForProperty");
        }

        /// <summary>
        /// The value '{0}' is invalid for target location.
        /// </summary>
        internal static string FormatInvalidValueForProperty(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("InvalidValueForProperty"), p0);

        /// <summary>
        /// '{0}' must be of type '{1}'.
        /// </summary>
        internal static string ParameterMustMatchType
        {
            get => GetString("ParameterMustMatchType");
        }

        /// <summary>
        /// '{0}' must be of type '{1}'.
        /// </summary>
        internal static string FormatParameterMustMatchType(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("ParameterMustMatchType"), p0, p1);

        /// <summary>
        /// The type '{0}' which is an array is not supported for json patch operations as it has a fixed size.
        /// </summary>
        internal static string PatchNotSupportedForArrays
        {
            get => GetString("PatchNotSupportedForArrays");
        }

        /// <summary>
        /// The type '{0}' which is an array is not supported for json patch operations as it has a fixed size.
        /// </summary>
        internal static string FormatPatchNotSupportedForArrays(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("PatchNotSupportedForArrays"), p0);

        /// <summary>
        /// The type '{0}' which is a non generic list is not supported for json patch operations. Only generic list types are supported.
        /// </summary>
        internal static string PatchNotSupportedForNonGenericLists
        {
            get => GetString("PatchNotSupportedForNonGenericLists");
        }

        /// <summary>
        /// The type '{0}' which is a non generic list is not supported for json patch operations. Only generic list types are supported.
        /// </summary>
        internal static string FormatPatchNotSupportedForNonGenericLists(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("PatchNotSupportedForNonGenericLists"), p0);

        /// <summary>
        /// The target location specified by path segment '{0}' was not found.
        /// </summary>
        internal static string TargetLocationAtPathSegmentNotFound
        {
            get => GetString("TargetLocationAtPathSegmentNotFound");
        }

        /// <summary>
        /// The target location specified by path segment '{0}' was not found.
        /// </summary>
        internal static string FormatTargetLocationAtPathSegmentNotFound(object p0)
            => string.Format(CultureInfo.CurrentCulture, GetString("TargetLocationAtPathSegmentNotFound"), p0);

        /// <summary>
        /// For operation '{0}', the target location specified by path '{1}' was not found.
        /// </summary>
        internal static string TargetLocationNotFound
        {
            get => GetString("TargetLocationNotFound");
        }

        /// <summary>
        /// For operation '{0}', the target location specified by path '{1}' was not found.
        /// </summary>
        internal static string FormatTargetLocationNotFound(object p0, object p1)
            => string.Format(CultureInfo.CurrentCulture, GetString("TargetLocationNotFound"), p0, p1);

        /// <summary>
        /// The current value '{0}' at position '{2}' is not equal to the test value '{1}'.
        /// </summary>
        internal static string ValueAtListPositionNotEqualToTestValue
        {
            get => GetString("ValueAtListPositionNotEqualToTestValue");
        }

        /// <summary>
        /// The current value '{0}' at position '{2}' is not equal to the test value '{1}'.
        /// </summary>
        internal static string FormatValueAtListPositionNotEqualToTestValue(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("ValueAtListPositionNotEqualToTestValue"), p0, p1, p2);

        /// <summary>
        /// The current value '{0}' at path '{2}' is not equal to the test value '{1}'.
        /// </summary>
        internal static string ValueNotEqualToTestValue
        {
            get => GetString("ValueNotEqualToTestValue");
        }

        /// <summary>
        /// The current value '{0}' at path '{2}' is not equal to the test value '{1}'.
        /// </summary>
        internal static string FormatValueNotEqualToTestValue(object p0, object p1, object p2)
            => string.Format(CultureInfo.CurrentCulture, GetString("ValueNotEqualToTestValue"), p0, p1, p2);

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
