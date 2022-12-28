namespace Clean.Domain.Common.Enums
{
    public enum ValidationType
    {
        IsBlank = 0,
        LengthExceeded = 1,
    }

    public static class ValidationTypeExtensions
    {
        public static string Message(this ValidationType validationType, string objectName = "Value")
        {
            return validationType switch
            {
                ValidationType.IsBlank => $"{objectName} cannot be empty or null",
                ValidationType.LengthExceeded => $"{objectName} has exceeded the maximum length",
                _ => throw new ArgumentOutOfRangeException("validationType"),
            };
        }
    }
}
