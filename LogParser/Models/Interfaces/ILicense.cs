namespace LogParser.Models.Interfaces
{
    public interface ILicense
    {
        string Product { get; }

        string Owner { get; }

        string LicenseType { get; }

        string Content { get; }
    }
}
