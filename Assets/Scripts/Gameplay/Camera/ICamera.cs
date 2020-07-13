namespace Gameplay.Camera
{
    public interface ICamera
    {
        void Activate();
        void FixIssue();
        void FixDisconnect();
        void TriggerDisconnect();
    }
}