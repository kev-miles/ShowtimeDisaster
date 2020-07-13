using Utilities;

namespace Gameplay.Sound
{
    public interface ISound
    {
        void PlayButtonSound(ButtonSounds id);
        void PlaySound(MiscSounds id);
        void StopSound();
    }
}