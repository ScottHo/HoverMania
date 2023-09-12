public enum AudioAction : int
{
    None = 0,
    Jump = 1,
    Collect = 2,
    Click = 3,
    Win = 4,
    Battery = 5,
    Hover = 6,
    Idle = 7,
    Drive = 8,
}

public enum UserCreatedStatus : int
{
    Success = 0,
    UsernameExists = 1,
    InvalidUsername = 2,
    InvalidID = 3,
    UnknownError = 4,
}
