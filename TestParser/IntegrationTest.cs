using AKG.Core.Model;
using AKG.Core.Parser;
using NUnit.Framework;

namespace TestParser;

[TestFixture]
public class IntegrationTests
{
    private const string TestObjContent = """

                                          # Test OBJ file
                                          v 1.0 0.0 0.0
                                          v 0.0 1.0 0.0
                                          v 0.0 0.0 1.0
                                          v -1.0 0.0 0.0

                                          vt 0.0 0.0
                                          vt 1.0 0.0
                                          vt 0.0 1.0

                                          vn 0.0 0.0 1.0
                                          vn 0.0 1.0 0.0

                                          f 1/1/1 2/2/1 3/3/2
                                          f 1/1/1 3/3/2 4/1/2

                                          """;

    [Test]
    public async Task ParseSimpleObjFile_CompleteModel()
    {
        string tempFile = Path.GetTempFileName() + ".obj";
        await File.WriteAllTextAsync(tempFile, TestObjContent);

        try
        {
            async IAsyncEnumerable<string[]> MockFileLoader()
            {
                string[] lines = TestObjContent.Split('\n');
                List<string> chunk = new List<string>();
                    
                foreach (string line in lines)
                {
                    var trimmed = line.Trim();
                    if (!string.IsNullOrEmpty(trimmed) && !trimmed.StartsWith("#"))
                    {
                        chunk.Add(trimmed);
                    }
                        
                    if (chunk.Count >= 256)
                    {
                        yield return chunk.ToArray();
                        chunk.Clear();
                    }
                }
                    
                if (chunk.Count > 0)
                {
                    yield return chunk.ToArray();
                }
                    
                await Task.CompletedTask;
            }

            var parser = new ObjParser();

            
            ObjModel model = await parser.ParseAsync(MockFileLoader());

            
            Assert.Multiple(() =>
            {
                Assert.That(model.Vertices, Has.Count.EqualTo(4));
                Assert.That(model.TextureCoords, Has.Count.EqualTo(3));
                Assert.That(model.Normals, Has.Count.EqualTo(2));
                Assert.That(model.Faces, Has.Count.EqualTo(2));
                    
                
                var firstFace = model.Faces[0];
                Assert.That(firstFace, Has.Length.EqualTo(3));
                    
                
                Assert.That(firstFace.First.VertexIndex, Is.EqualTo(0));
                Assert.That(firstFace.First.TextureIndex, Is.EqualTo(0));
                Assert.That(firstFace.First.NormalIndex, Is.EqualTo(0));
            });
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Test]
    public void FaceIndices_IsEmpty_WorksCorrectly()
    {
        
        var emptyIndices = new FaceIndices();
        var nonEmptyIndices = new FaceIndices(1, 2, 3);

        
        Assert.Multiple(() =>
        {
            Assert.That(emptyIndices.IsEmpty, Is.True);
            Assert.That(nonEmptyIndices.IsEmpty, Is.False);
        });
    }
}