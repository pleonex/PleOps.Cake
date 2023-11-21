// Copyright (c) 2023 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Cake.Frosting.PleOps.Recipe;

using Cake.Core;

/// <summary>
/// Extension methods for ICakeArguments class.
/// </summary>
public static class CakeArgumentsExtensions
{
    /// <summary>
    /// Run the setter if the argument name is present.
    /// </summary>
    /// <param name="handler">Cake argument handler.</param>
    /// <param name="argName">Argument name.</param>
    /// <param name="setter">Setter to run.</param>
    public static void SetIfPresent(this ICakeArguments handler, string argName, Action<string> setter)
    {
        if (handler.HasArgument(argName)) {
            setter(handler.GetArgument(argName));
        }
    }

    /// <summary>
    /// Run the setter if the argument name is present.
    /// </summary>
    /// <param name="handler">Cake argument handler.</param>
    /// <param name="argName">Argument name.</param>
    /// <param name="setter">Setter to run.</param>
    public static void SetIfPresent(this ICakeArguments handler, string argName, Action<bool> setter)
    {
        if (handler.HasArgument(argName)) {
            string valueText = handler.GetArgument(argName);
            if (string.IsNullOrEmpty(valueText)) {
                // If it's present but without value, assume is like a set -> true
                setter(true);
            } else {
                bool value = bool.Parse(handler.GetArgument(argName));
                setter(value);
            }
        }
    }
}
