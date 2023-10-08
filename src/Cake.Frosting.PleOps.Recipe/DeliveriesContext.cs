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

public class DeliveriesContext
{
    public DeliveriesContext()
    {
        NuGetLibraries = new Collection<string>();
        BinaryFiles = new Collection<string>();

        DocumentationPath = "build/artifacts/docs";
        DeliveryInfoPath = "build/artifacts/artifacts.json";
        NuGetArtifactsPath = "build/artifacts/nuget";
    }

    public string NuGetArtifactsPath { get; set; }

    public Collection<string> NuGetLibraries { get; set; }

    public Collection<string> BinaryFiles { get; set; }

    public string DocumentationPath { get; set; }

    public string DeliveryInfoPath { get; private set; }

    public void Save()
    {
        string json = JsonSerializer.Serialize(this);
        File.WriteAllText(DeliveryInfoPath, json);
    }

    public void Initialize(BuildContext context)
    {
        DeliveryInfoPath = Path.Combine(context.ArtifactsPath, "artifacts.json");

        if (File.Exists(DeliveryInfoPath)) {
            string json = File.ReadAllText(DeliveryInfoPath);
            DeliveriesContext actual = JsonSerializer.Deserialize<DeliveriesContext>(json)
                ?? throw new FormatException("Cannot deserialize deliveries info");

            DocumentationPath = actual.DocumentationPath;
            NuGetArtifactsPath = actual.NuGetArtifactsPath;
            NuGetLibraries = actual.NuGetLibraries;
            BinaryFiles = actual.BinaryFiles;
        } else {
            DocumentationPath = Path.Combine(context.ArtifactsPath, "docs");
            NuGetArtifactsPath = Path.Combine(context.ArtifactsPath, "nuget");
        }
    }
}
