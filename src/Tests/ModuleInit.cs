public static class ModuleInit
{
    #region enable

    [ModuleInitializer]
    public static void Init()
    {
        VerifyWinForms.Enable();

        #endregion

        VerifierSettings.UniqueForRuntime();
        VerifyPhash.RegisterComparer("png", .99f);

        VerifyDiffPlex.Initialize();
    }
}