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
namespace Cake.Frosting.PleOps.Recipe.Dotnet;

using Cake.Common.Tools.DotNet.MSBuild;

/// <summary>
/// Extensions for MSBuild settings.
/// </summary>
internal static class MSBuildSettingExtensions
{
    /// <summary>
    /// Hide the detailed summary for compatibility with VS Code.
    /// </summary>
    /// <param name="settings">Settings parameter.</param>
    /// <returns>The same instance.</returns>
    /// <exception cref="ArgumentNullException">Settings is null.</exception>
    public static DotNetMSBuildSettings HideDetailedSummary(this DotNetMSBuildSettings settings)
    {
        if (settings == null) {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.DetailedSummary = false;
        return settings;
    }
}
