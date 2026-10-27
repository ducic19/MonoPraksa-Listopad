namespace Gymap.Service;

// Ishod operacije pisanja; kontroleri ga prevode u HTTP status.
public enum SaveResult
{
    Success,
    NotFound,
    InvalidReference,
    Duplicate,
    InUse
}
