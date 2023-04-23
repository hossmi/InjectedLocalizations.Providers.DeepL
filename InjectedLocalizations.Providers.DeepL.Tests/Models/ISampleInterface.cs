namespace InjectedLocalizations.Models
{
    public interface ISampleInterface : ILocalizations
    {
        string The_file_already_exists { get; }
        string There_are_0_apples(int count);
    }
}