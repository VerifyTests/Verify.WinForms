public static class ModuleInit
{
    #region enable

    [ModuleInitializer]
    public static void Init() =>
        VerifyWinForms.Enable();

    #endregion

    [ModuleInitializer]
    public static void InitOther()
    {
        VerifierSettings.UniqueForRuntime();
        VerifyPhash.RegisterComparer("png", .99f);

        VerifyDiffPlex.Initialize();
    }
}