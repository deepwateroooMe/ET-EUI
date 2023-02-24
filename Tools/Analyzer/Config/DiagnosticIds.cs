namespace ET.Analyzer {
    public static class DiagnosticIds {
// 这里是它报错的原则：所以老是报 ET001
        public const string AddChildTypeAnalyzerRuleId = "ET0001";

        public const string EntityFiledAccessAnalyzerRuleId = "ET0002";
        public const string EntityClassDeclarationAnalyzerRuleId = "ET0003";
        public const string HotfixProjectFieldDeclarationAnalyzerRuleId = "ET0004";
        public const string ClassDeclarationInHotfixAnalyzerRuleId = "ET0005";
        public const string EntityMethodDeclarationAnalyzerRuleId = "ET0006";
        public const string EntityComponentAnalyzerRuleId = "ET0007";
        public const string ETTaskInSyncMethodAnalyzerRuleId = "ET0008";
        public const string ETTaskInAsyncMethodAnalyzerRuleId = "ET0009";
    }
}
