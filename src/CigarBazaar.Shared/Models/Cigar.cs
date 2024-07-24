using System.Text.RegularExpressions;

namespace CigarBazaar.Shared.Models;

public class Cigar
{
    public string Code
    {
        get { return TransformString(name); }
    }

    private string name = string.Empty;
    public string Name
    {
        get { return name; }
        set { name = value.Trim(); }
    }

    public decimal Price { get; set; }

    public decimal? ChargedPrice { get; set; }

    private string TransformString(string input)
    {
        // Step 1: Replace spaces and special characters with underscores
        string result = Regex.Replace(input, @"[\s\(\)]+", "_");

        // Step 2: Remove any characters that are not alphanumeric or underscores
        result = Regex.Replace(result, @"[^a-zA-Z0-9_]", "");

        // Step 3: Convert the string to uppercase
        result = result.ToUpper();

        // Remove any leading or trailing underscores
        result = result.Trim('_');

        return result;
    }
}
    