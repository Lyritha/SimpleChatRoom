namespace StdbModule
{
    public static partial class Module
    {
        private static void AssignIfNotNull<T>(ref T target, T? value)
        {
            if (value != null)
                target = value;
        }
    }
}
