public static class ModuleInit
{
    #region enable

    [ModuleInitializer]
    public static void Init() =>
        VerifyWinForms.Initialize();

    #endregion

    [ModuleInitializer]
    public static void InitOther()
    {
        VerifierSettings.UniqueForRuntime();
        VerifyPhash.RegisterComparer("png", .80f);

        VerifyDiffPlex.Initialize();
    }
}