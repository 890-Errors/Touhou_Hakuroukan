namespace AnimationImporter
{
    public interface IAnimationImporterPlugin
    {
        ImportedAnimationSheet Import(AnimationImportJob job, AnimationImporterSharedConfig config);
        bool IsValid();
        bool IsConfigured();
    }
}