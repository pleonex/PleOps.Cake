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

using System.Collections.ObjectModel;
using System.Text.Json;

/// <summary>
/// Build contexts with the project deliveries.
/// </summary>
public class DeliveriesContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeliveriesContext"/> class.
    /// </summary>
    public DeliveriesContext()
    {
        NuGetPackages = new Collection<string>();
        BinaryFiles = new Collection<string>();

        DocumentationPath = "build/artifacts/docs";
        DeliveryInfoPath = "build/artifacts/artifacts.json";
        NuGetArtifactsPath = "build/artifacts/nuget";
    }

    /// <summary>
    /// Gets or sets the path to the directory with NuGet artifacts.
    /// </summary>
    public string NuGetArtifactsPath { get; set; }

    /// <summary>
    /// Gets or sets the collection of the project's NuGet packages.
    /// </summary>
    public Collection<string> NuGetPackages { get; set; }

    /// <summary>
    /// Gets or sets the collection of binary delivery files.
    /// </summary>
    public Collection<string> BinaryFiles { get; set; }

    /// <summary>
    /// Gets or sets the path to the directory containing the documentation folder.
    /// </summary>
    public string DocumentationPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the JSON file with the information of the deliveries.
    /// </summary>
    public string DeliveryInfoPath { get; set; }

    /// <summary>
    /// Serializes this class into JSON and store it in the provided path.
    /// </summary>
    public void Save()
    {
        string? outDir = Path.GetDirectoryName(DeliveryInfoPath);
        if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir)) {
            _ = Directory.CreateDirectory(outDir);
        }

        string json = JsonSerializer.Serialize(this);
        File.WriteAllText(DeliveryInfoPath, json);
    }

    /// <summary>
    /// Initializes this class from the build context and information of disk.
    /// </summary>
    /// <param name="context">Build context.</param>
    /// <exception cref="FormatException">Invalid JSON file on disk.</exception>
    /// <remarks>
    /// If the artifacts.json file exists, it will load the information from there.
    /// </remarks>
    public void Initialize(PleOpsBuildContext context)
    {
        DeliveryInfoPath = Path.Combine(context.ArtifactsPath, "artifacts.json");

        if (File.Exists(DeliveryInfoPath)) {
            string json = File.ReadAllText(DeliveryInfoPath);
            DeliveriesContext actual = JsonSerializer.Deserialize<DeliveriesContext>(json)
                ?? throw new FormatException("Cannot deserialize deliveries info");

            DocumentationPath = actual.DocumentationPath;
            NuGetArtifactsPath = actual.NuGetArtifactsPath;
            NuGetPackages = actual.NuGetPackages;
            BinaryFiles = actual.BinaryFiles;
        } else {
            DocumentationPath = Path.Combine(context.ArtifactsPath, "docs");
            NuGetArtifactsPath = Path.Combine(context.ArtifactsPath, "nuget");
        }
    }
}
