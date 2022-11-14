namespace Camera
{
    public enum CameraState:int
    {
        Unavailable = 0,
        Inactive = 1,
        Default = 10,
        Active = 100,
        HighPriority = 101
    }
}
