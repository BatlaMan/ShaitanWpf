using SoundIdentification.Command;


namespace SoundIdentification.Builder
{
    public interface IFingerprintCommandBuilder
    {
        ISourceFrom BuildFingerprintCommand();
    }
}
