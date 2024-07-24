using CigarBazaar.Shared.Models;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace CigarBazaar.Shared.UnitTest.Models;

public class CigarTests
{
    [Theory]
    [InlineData("")]
    [InlineData("3 REYNAS Belicoso 6x54 (20)")]
    [InlineData("A.FLORES EL TROVADOR Gran Toro (24)")]
    [InlineData("ADRIAN MAGNUS Adrian Magnus Millennium Robustos (3)")]
    public void Cigar_Code_Should_Return_Name_With_Transformation(string cigarName)
    {
        // Arrange
        Cigar cigar = new Cigar()
        {
            Name = cigarName
        };

        // Act
        string result = Regex.Replace(cigarName, @"[\s\(\)]+", "_");
        result = Regex.Replace(result, @"[^a-zA-Z0-9_]", "");
        result = result.ToUpper();
        result = result.Trim('_');

        // Assert
        cigar.Should().NotBeNull();
        cigar.Name.Should().Be(cigarName);
        cigar.Code.Should().Be(result);
    }
}
