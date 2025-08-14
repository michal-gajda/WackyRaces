namespace WackyRaces.Domain.UnitTests.Templates;

using WackyRaces.Domain.Templates;
using WackyRaces.Domain.Types;

[TestClass]
public sealed class ConcatExampleTests
{
    [TestMethod]
    public void LoadConcatExample_ShouldLoadSuccessfully()
    {
        // Arrange
        var examplePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "..",
            "src", "Domain", "Templates", "Examples", "concat-demo.json"
        );

        // Skip test if file doesn't exist
        if (!File.Exists(examplePath))
        {
            Assert.Inconclusive("Example file not found");
            return;
        }

        // Act
        var table = SimpleJsonTemplateLoader.LoadFromJsonFile(examplePath);

        // Assert
        table.ShouldNotBeNull();
        table.Name.ShouldBe("Contact List with CONCAT");

        // Check that CONCAT functions are loaded correctly
        var fullNameFunction = table.GetCell(Coordinate.Parse("D2"));
        fullNameFunction.IsT6.ShouldBeTrue(); // Should be Function
        fullNameFunction.AsT6.Value.ShouldBe("CONCAT(A2,\" \",B2)");
        fullNameFunction.AsT6.Format.ShouldBe(typeof(string));

        var emailFunction = table.GetCell(Coordinate.Parse("E2"));
        emailFunction.IsT6.ShouldBeTrue(); // Should be Function
        emailFunction.AsT6.Value.ShouldBe("CONCAT(A2,\".\",B2,\"@company.com\")");
        emailFunction.AsT6.Format.ShouldBe(typeof(string));

        // Check column headers
        table.GetCell(Coordinate.Parse("A1")).AsT0.ShouldBe("First Name");
        table.GetCell(Coordinate.Parse("D1")).AsT0.ShouldBe("Full Name");
        table.GetCell(Coordinate.Parse("E1")).AsT0.ShouldBe("Email");

        // Check sample data
        table.GetCell(Coordinate.Parse("B2")).AsT0.ShouldBe("Doe");
        table.GetCell(Coordinate.Parse("C2")).AsT0.ShouldBe("Engineering");
    }

    [TestMethod]
    public void ConcatExample_RoundTrip_ShouldPreserveData()
    {
        // Arrange
        var examplePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..", "..", "..",
            "src", "Domain", "Templates", "Examples", "concat-demo.json"
        );

        if (!File.Exists(examplePath))
        {
            Assert.Inconclusive("Example file not found");
            return;
        }

        // Act - Load and save back
        var originalTable = SimpleJsonTemplateLoader.LoadFromJsonFile(examplePath);
        var json = SimpleJsonTemplateLoader.SaveToJson(originalTable);
        var reloadedTable = SimpleJsonTemplateLoader.LoadFromJson(json);

        // Assert - Verify CONCAT functions are preserved
        var originalFunction = originalTable.GetCell(Coordinate.Parse("D2"));
        var reloadedFunction = reloadedTable.GetCell(Coordinate.Parse("D2"));

        originalFunction.AsT6.Value.ShouldBe(reloadedFunction.AsT6.Value);
        originalFunction.AsT6.Format.ShouldBe(reloadedFunction.AsT6.Format);

        // Verify JSON contains CONCAT
        json.ShouldContain("CONCAT");
        json.ShouldContain("\"Type\": \"text\"");
    }
}
