﻿using System;

namespace Entoarox.Framework.Config
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigMinMaxAttribute : ConfigAttribute
    {
        internal int Min;
        internal int Max;
        /// <summary>
        /// Configs that can be accessed using <see cref="IModHelperExtensions.RegisterDynamicConfig{T}(IModHelper, T, Action{T})"/> must use this attribute or the parent <see cref="ConfigAttribute"/> attribute on properties they wish to be dynamic.
        /// This attribute should only be used on values that are of the <see cref="int"/> type.
        /// </summary>
        /// <param name="label">The label to display in front of this config option</param>
        /// <param name="description">The description to show when the label for this config option is hovered over, should explain what the option does</param>
        /// <param name="options">The list of dropdown options to display</param>
        /// <param name="min">The minimum value that is allowed</param>
        /// <param name="max">the maximum value that is allowed</param>
        public ConfigMinMaxAttribute(string label, string description, int min, int max) : base(label, description)
        {
            Min = min;
            Max = max;
        }
    }
}
